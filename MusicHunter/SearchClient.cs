using System.Reflection;
using System.Text.RegularExpressions;
using System.Net.Http.Json;
using System.Net.Mime;
using iTunesSearch.Library;
using YandexMusicResolver;
using YandexMusicResolver.Config;
using E.Deezer;
using E.Deezer.Api;
using SpotifyAPI.Web;
using YoutubeExplode;
using YoutubeExplode.Common;
using Uthef.MusicHunter.SoundCloudModels;
using Uthef.MusicHunter.BandcampModels;
using System.Collections.Immutable;
using HtmlAgilityPack;
using System.Net;
using Uthef.MusicHunter.Filters;
using System.Text.Json;

namespace Uthef.MusicHunter
{
    public sealed class SearchClient : IDisposable
    {
        public const string BandcampApiUrlPart = "https://bandcamp.com/api/bcsearch_public_api/1/autocomplete_elastic";
        public const string YouTubeUrlPart = "https://music.youtube.com";
        public const string YandexUrlPart = "https://music.yandex.ru";
        public const string SoundCloudSearchApiUrlPart = "https://api-v2.soundcloud.com";
        public const string SoundCloudApiResolverUrlPart = "https://apiresolver-r8gd.onrender.com/sc";

        public const int DefaultLimit = 24;

        public ArtworkResolution DefaultArtworkResolution = ArtworkResolution.Large;

        private readonly YandexMusicMainResolver _yandexMusicResolver;
        private readonly Deezer _deezerResolver;
        private readonly YoutubeClient _ytClient;
        private readonly iTunesSearchManager _iTunesSearchManager;
        private readonly HttpClient _httpClient;
        private readonly SpotifyClient? _spotifyClient;

        private readonly Dictionary<MusicService, SearchMethod> _methods = new();
        private readonly SearchClientConfiguration _configuration;
        private readonly Regex _artworkResolutionPattern = new("((%%|(\\d+x\\d+)))(?!.*(%%|(\\d+x\\d+)))", RegexOptions.Compiled);
        private readonly Regex _amazonRegex = new("\\/[^/]+$", RegexOptions.Compiled);
        private readonly Regex _amazonIdRegex = new("(?<=trackAsin=)[^&;#]+", RegexOptions.Compiled);

        public SearchClient(SearchClientConfiguration config)
        {
            _configuration = config;

            var methods = GetType()
                .GetMethods()
                .Where(x => x.GetCustomAttribute<MethodOf>() != null
                    && x.ReturnType == typeof(Task<SearchItemList>));

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<MethodOf>();
                var searchMethod = (SearchMethod)Delegate.CreateDelegate(typeof(SearchMethod), this, method);

                if (attribute != null)
                    _methods.Add(attribute.Service, searchMethod);
            }

            _yandexMusicResolver = new YandexMusicMainResolver(new EmptyYandexConfig());
            _deezerResolver = DeezerSession.CreateNew();
            _iTunesSearchManager = new iTunesSearchManager();
            var clientHandler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            _httpClient = new HttpClient(clientHandler);
            _ytClient = new YoutubeClient(_httpClient);

            if (_configuration.SpotifyClientId != null && _configuration.SpotifyClientSecret != null)
            {
                var spotifyConfig = SpotifyClientConfig
                  .CreateDefault()
                  .WithAuthenticator(new ClientCredentialsAuthenticator(_configuration.SpotifyClientId,
                    _configuration.SpotifyClientSecret));

                _spotifyClient = new SpotifyClient(spotifyConfig);
            }
        }

        public void Dispose()
        {
            _deezerResolver.Dispose();
            _httpClient.Dispose();
        }

        public async Task<SearchResult> SearchAsync(string query, ItemType itemType, 
            ServicePack pack, ISearchFilter? filter = null, CancellationToken cancellationToken = default)
        {
            List<SearchItem> searchItems = new();
            List<ExceptionView> exceptions = new();
            List<Task> tasks = new();

            foreach (var service in pack.Items)
            {
                var task = _methods[service].Invoke(query, itemType, filter?.Limit ?? DefaultLimit, cancellationToken);
                var stamp = DateTime.Now;

                task.GetAwaiter().OnCompleted(() =>
                {
                    if (!task.IsCompletedSuccessfully)
                    {
                        if (task.Exception != null)
                            exceptions.Add(new ExceptionView(task.Exception.Message, service));

                        return;
                    }

                    foreach (var result in task.Result)
                    {
                        if (filter != null && !filter.IsItemValid(result, itemType))
                            continue;

                        result.ExecutionTimeMs = (DateTime.Now - stamp).TotalMilliseconds;
                        searchItems.Add(result);
                        break;
                    }
                });

                tasks.Add(task);
            }

            var overallStamp = DateTime.Now;

            try
            {
                await Task.WhenAll(tasks).WaitAsync(cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch { }

            return new SearchResult(itemType, searchItems.ToImmutableList(), exceptions.ToImmutableList(), DateTime.Now - overallStamp);
        }

        #region Yandex
        [MethodOf(MusicService.Yandex)]
        public async Task<SearchItemList> SearchYandexAsync(string query, ItemType itemType, int limit = DefaultLimit, CancellationToken cancellationToken = default)
        {
            var searchResultCollection = new SearchItemList(itemType);

            var type = itemType is ItemType.Track ? YandexSearchType.Track : YandexSearchType.Album;
            var result = await _yandexMusicResolver.SearchResultLoader.LoadSearchResult(type, query, limit).WaitAsync(cancellationToken);

            if (result == null) return searchResultCollection;

            if (type is YandexSearchType.Track && result.Tracks != null)
            {
                foreach (var track in result.Tracks)
                {
                    string artworkUrl = FixArtworkUrl(track.ArtworkUrl);
                    searchResultCollection.Add(
                        new SearchItem(
                            track.Id,
                            track.Uri,
                            track.Title,
                            track.Authors.Select(x => x.Name),
                            artworkUrl,
                            MusicService.Yandex
                        )
                    );
                }
            }
            else if (type is YandexSearchType.Album && result.Albums != null)
            {
                foreach (var album in result.Albums)
                {
                    string artworkUrl = FixArtworkUrl(album.ArtworkUrl);
                    searchResultCollection.Add(
                        new SearchItem(
                            album.Id.ToString(),
                            $"{YandexUrlPart}/album/{album.Id}",
                            album.Title,
                            album.Artists.Select(x => x.Name),
                            artworkUrl,
                            MusicService.Yandex
                        )
                    );
                }
            }

            return searchResultCollection;
        }
        #endregion

        #region Deezer
        [MethodOf(MusicService.Deezer)]
        public async Task<SearchItemList> SearchDeezerAsync(string query, ItemType itemType, int limit = DefaultLimit, CancellationToken cancellationToken = default)
        {
            var searchResultCollection = new SearchItemList(itemType);

            if (itemType is ItemType.Track)
            {
                var tracks = await _deezerResolver.Search.Tracks(query, 0, (uint)limit).WaitAsync(cancellationToken);

                foreach (var track in tracks)
                {
                    var artists = new List<string> { track.ArtistName };

                    if (track.Contributors != null)
                        artists.AddRange(track.Contributors.Select(x => x.Name));

                    searchResultCollection.Add(
                        new SearchItem(
                            track.Id.ToString(),
                            track.Link,
                            track.Title,
                            track.ArtistName,
                            FixArtworkUrl(track.GetPicture(PictureSize.ExtraLarge)),
                            MusicService.Deezer
                        )
                   );
                }
            }
            else if (itemType is ItemType.Album)
            {
                var albums = await _deezerResolver.Search.Albums(query, 0, (uint)limit);

                foreach (var album in albums)
                {
                    var artists = new List<string> { album.ArtistName };

                    if (album.Contributors != null)
                        artists.AddRange(album.Contributors.Select(x => x.Name));

                    searchResultCollection.Add(
                        new SearchItem(
                            album.Id.ToString(),
                            album.Link,
                            album.Title,
                            artists,
                            FixArtworkUrl(album.GetPicture(PictureSize.ExtraLarge)),
                            MusicService.Deezer
                        )
                    );
                }
            }

            return searchResultCollection;
        }
        #endregion

        #region AppleMusic
        [MethodOf(MusicService.AppleMusic)]
        public async Task<SearchItemList> SearchAppleMusicAsync(string query, ItemType itemType, int limit = DefaultLimit, CancellationToken cancellationToken = default)
        {
            var searchResultCollection = new SearchItemList(itemType);

            if (itemType is ItemType.Track)
            {
                var searchResult = await _iTunesSearchManager.GetSongsAsync(query, limit).WaitAsync(cancellationToken);

                foreach (var song in searchResult.Songs)
                {
                    searchResultCollection.Add(
                        new SearchItem(
                            song.TrackId.ToString(),
                            song.TrackViewUrl,
                            song.TrackName,
                            song.ArtistName,
                            FixArtworkUrl(song.ArtworkUrl100),
                            MusicService.AppleMusic
                        )
                    );
                }
            }
            else if (itemType is ItemType.Album)
            {
                var searchResult = await _iTunesSearchManager.GetAlbumsAsync(query, limit);

                foreach (var album in searchResult.Albums)
                {
                    searchResultCollection.Add(
                        new SearchItem(
                            album.CollectionId.ToString(),
                            album.CollectionViewUrl,
                            album.CollectionName,
                            album.ArtistName,
                            FixArtworkUrl(album.ArtworkUrl100),
                            MusicService.AppleMusic

                        )
                    );
                }
            }

            return searchResultCollection;
        }
        #endregion

        #region YouTube
        [MethodOf(MusicService.YouTube)]
        public async Task<SearchItemList> SearchYouTubeAsync(string query, ItemType itemType, int limit = DefaultLimit, CancellationToken cancellationToken = default)
        {
            var searchResultCollection = new SearchItemList(itemType);

            if (itemType is ItemType.Track)
            {
                var videos = await _ytClient.Search.GetVideosAsync(query, cancellationToken).CollectAsync(limit);

                foreach (var video in videos)
                {
                    searchResultCollection.Add(
                        new SearchItem(
                            video.Id,
                            $"{YouTubeUrlPart}/watch?v={video.Id}",
                            video.Title,
                            video.Author.ChannelTitle,
                            video.Thumbnails[0]?.Url,
                            MusicService.YouTube
                        )
                    );
                }
            }
            else
            {
                var playlists = await _ytClient.Search.GetPlaylistsAsync(query, cancellationToken).CollectAsync(limit);

                foreach (var playlist in playlists)
                {
                    var author = playlist.Author;

                    searchResultCollection.Add(
                        new SearchItem(
                            playlist.Id,
                            $"{YouTubeUrlPart}/playlist?list={playlist.Id}",
                            playlist.Title,
                            author is null ? "" : author.ChannelTitle,
                            playlist.Thumbnails[0]?.Url,
                            MusicService.YouTube
                        )
                    );
                }
            }

            return searchResultCollection;
        }
        #endregion

        #region Bandcamp
        [MethodOf(MusicService.Bandcamp)]
        public async Task<SearchItemList> SearchBandcampAsync(string query, ItemType itemType, int limit = DefaultLimit, CancellationToken cancellationToken = default)
        {
            var requestUrl = BandcampApiUrlPart;
            var searchResultCollection = new SearchItemList(itemType);

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Content = JsonContent.Create(new BandcampSearchRequest(query, itemType, 0, false));

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<BandcampSearchResult>(cancellationToken: cancellationToken);
            if (result == null) return searchResultCollection;

            var realLimit = Math.Min(result.Data.Items.Count, limit);
            for (int i = 0; i < realLimit; i++)
            {
                var item = result.Data.Items[i];

                searchResultCollection.Add(
                    new SearchItem(
                        item.Id.ToString(),
                        item.Url,
                        item.Name,
                        item.Band,
                        item.Image,
                        MusicService.Bandcamp
                    )
                );
            }

            return searchResultCollection;
        }
        #endregion

        #region Spotify
        [MethodOf(MusicService.Spotify)]
        public async Task<SearchItemList> SearchSpotifyAsync(string query, ItemType itemType, int limit = DefaultLimit, CancellationToken cancellationToken = default)
        {
            if (_spotifyClient == null) throw new Exception("Spotify credentials are not provided");

            var searchResultCollection = new SearchItemList(itemType);

            var type = itemType is ItemType.Track ? SearchRequest.Types.Track : SearchRequest.Types.Album;
            var searchRequest = new SearchRequest(type, query)
            {
                Limit = limit
            };

            var response = await _spotifyClient.Search.Item(searchRequest).WaitAsync(cancellationToken);

            if (itemType is ItemType.Track && response.Tracks.Items != null)
            {
                foreach (var track in response.Tracks.Items)
                {
                    searchResultCollection.Add(
                        new SearchItem(
                            track.Id,
                            track.ExternalUrls["spotify"],
                            track.Name,
                            track.Artists.Select(x => x.Name),
                            track.Album.Images.FirstOrDefault()?.Url,
                            MusicService.Spotify
                        )
                    );
                }
            }
            else if (response.Albums.Items != null)
            {
                foreach (var album in response.Albums.Items)
                {
                    searchResultCollection.Add(
                        new SearchItem(
                            album.Id,
                            album.ExternalUrls["spotify"],
                            album.Name,
                            album.Artists.Select(x => x.Name),
                            album.Images.FirstOrDefault()?.Url,
                            MusicService.Spotify
                        )
                    );
                }
            }

            return searchResultCollection;
        }
        #endregion

        #region SoundCloud
        [MethodOf(MusicService.SoundCloud)]
        public async Task<SearchItemList> SearchSoundCloudAsync(string query, ItemType itemType, int limit = DefaultLimit, CancellationToken cancellationToken = default)
        {
            if (_configuration.SoundCloudClientId == null)
            {
                throw new Exception("SoundCloud Client ID is not provided");
            }

            string type = $"{(itemType is ItemType.Track ? "tracks" : "playlists")}";
            string baseAddr = $"{(_configuration.UseSoundCloudProxy ? SoundCloudApiResolverUrlPart : SoundCloudSearchApiUrlPart)}" +
                $"/search/{type}?";

            string url = $"{baseAddr}" +
                    $"q={query}&" +
                    $"client_id={_configuration.SoundCloudClientId}&" +
                    $"limit={limit}";

            var searchResultCollection = new SearchItemList(itemType);
            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            if (response.Content.Headers.ContentType?.MediaType == MediaTypeNames.Application.Json)
            {
                var result = await response.Content.ReadFromJsonAsync<SoundCloudSearchResult>(cancellationToken: cancellationToken);

                if (result is null) return searchResultCollection;

                foreach (var item in result.Collection)
                {
                    searchResultCollection.Add(
                        new SearchItem(
                            item.Id.ToString(),
                            item.Link,
                            item.Title,
                            item.User.Name,
                            item.ArtworkUrl,
                            MusicService.SoundCloud
                        )
                    );
                }
            }

            return searchResultCollection;

        }
        #endregion

        #region Amazon
        [MethodOf(MusicService.Amazon)]
        public async Task<SearchItemList> SearchAmazonAsync(string query, ItemType itemType, int limit = DefaultLimit, CancellationToken cancellationToken = default)
        {
            var list = new SearchItemList(itemType);
            var type = itemType is ItemType.Track ? "track" : "album";

            using var request = new HttpRequestMessage(HttpMethod.Get, $"https://amazon.com/s?k={query}&i=digital-music-{type}&dc");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:106.0) Gecko/20100101 Firefox/106.0");
            
            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var document = new HtmlDocument();
            document.Load(await response.Content.ReadAsStreamAsync(cancellationToken));

            var cards = document.DocumentNode.SelectNodes("//div[contains(@class, \"s-card-container\")]");

            if (cards is null) return list;

            var count = cards.Count;

            var lim = Math.Min(cards.Count, limit);

            for (int i = 0; i < lim; i++)
            {
                var card = cards[i];

                var rows = card.SelectNodes("//div[@class=\"a-row\"]");
                if (rows is null) return list;

                var links = card.SelectNodes("//a[contains(@class, \"a-link-normal\") and contains(@class, \"s-underline-text\")]");
                var imageUrl = card.SelectSingleNode("//img[@class=\"s-image\"]").Attributes["src"].Value;

                var title = links[1].InnerText.Trim();
                var artist = rows.First().ChildNodes.Last().InnerText;

                var urlPart = links[2].Attributes["href"].Value;
                var filteredUrlPart = _amazonRegex.Replace(urlPart, "");
                var link = $"https://www.amazon.com{filteredUrlPart}";
                string id = "";

                if (itemType is ItemType.Track)
                {
                    id = _amazonIdRegex.Match(urlPart).Value;
                    link += $"?trackAsin={id}";
                }
                else
                {
                    id = filteredUrlPart.Replace("/dp/", "");
                }


                list.Add(new SearchItem(id, link, title, artist, imageUrl, MusicService.Amazon));
            }

            return list;
        }
        #endregion

        private string FixArtworkUrl(string? url)
        {
            if (url == null) return "";

            if (!url.StartsWith("https://") && !url.StartsWith("http://"))
            {
                url = url.Insert(0, "https://");
            }

            return _artworkResolutionPattern.Replace(url, DefaultArtworkResolution);
        }
    }
}