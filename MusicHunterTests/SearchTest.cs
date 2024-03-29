﻿using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;
using Uthef.MusicHunter;
using Uthef.MusicHunter.Filters;

namespace MusicHunterTests
{
    [TestFixture]
    [Timeout(5000)]
    public class SearchTest
    {
        SearchClient _searchClient;
        JsonSerializerOptions JsonSerializerOptions;

        [OneTimeSetUp]
        public void Setup()
        {
            // Read configuration file
            using var fs = new FileStream("Config.json", FileMode.Open);
            var config = JsonSerializer.Deserialize<JsonObject>(fs);

            _searchClient = new SearchClient(
                new SearchClientConfiguration(
                    spotifyClientId: config?["SpotifyClientID"]?.ToString(),
                    spotifyClientSecret: config?["SpotifyClientSecret"]?.ToString(),
                    soundCloudClientId: config?["SoundCloudClientId"]?.ToString(),
                    useSoundCloudProxy: true
                )
            ) ;

            JsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            };
        }

        [TestCase("celldweller - my disintegration", ItemType.Track)]
        public async Task TestAll(string query, ItemType type)
        {
            var resultCollection = await _searchClient.SearchAsync(query, type, ServicePack.All);
            AssertSearchResultWithOutput(resultCollection, ServicePack.All);
        }

        [TestCase("celldweller - satellites", ItemType.Album, "celldweller", "satellites")]
        public async Task TestAllStrictly(string query, ItemType type, string strictArtist, string strictTitle)
        {
            var filter = new StrictFilter(strictArtist, strictTitle);

            var resultCollection = await _searchClient.SearchAsync(query, type, ServicePack.All, filter);
            AssertSearchResultWithOutput(resultCollection, ServicePack.All);
        }

        [TestCase("pirate wicked", ItemType.Track, "coeur de pirate", "wicked games")]
        public async Task TestYouTubeAndSoundCloudStrictly(string query, ItemType itemType, string strictArtist, string strictTitle)
        {
            var filter = new StrictFilter(strictArtist, strictTitle);
            var services = MusicService.YouTube | MusicService.SoundCloud;
            var resultCollection = await _searchClient.SearchAsync(query, itemType, services, filter);
            AssertSearchResultWithOutput(resultCollection, services);
        }

        [TestCase("ic3peak - грустная сука", ItemType.Track)]
        public async Task TestSoundCloud(string query, ItemType type)
        {
            var services = MusicService.SoundCloud;
            var resultCollection = await _searchClient.SearchAsync(query, type, services);
            AssertSearchResultWithOutput(resultCollection, services);
        }

        [TestCase("five crumbs", ItemType.Album)]
        public async Task TestBandcamp(string query, ItemType type)
        {
            var services = MusicService.Bandcamp;
            var resultCollection = await _searchClient.SearchAsync(query, type, services);
            AssertSearchResultWithOutput(resultCollection, services);
        }

        [TestCase("baldocaster - moonrise", ItemType.Album)]
        [TestCase("katy perry i kissed a girl", ItemType.Track)]
        public async Task TestAmazon(string query, ItemType itemType)
        {
            var result = await _searchClient.SearchAsync(query, itemType, MusicService.Amazon);
            AssertSearchResultWithOutput(result, MusicService.Amazon);
        }

        private void AssertSearchResultWithOutput(SearchResult resultCollection, MusicService services)
        {
            if (resultCollection.Exceptions.Count > 0)
            {
                TestContext.WriteLine(JsonSerializer.Serialize(
                    resultCollection.Exceptions, JsonSerializerOptions)
                );

                Assert.Fail("The inner exception(s) occurred");
            }

            TestContext.WriteLine("The following items have been found:");
            TestContext.WriteLine(JsonSerializer.Serialize(resultCollection, JsonSerializerOptions));

            Assert.That(resultCollection.Items, Has.Exactly(ServicePack.Count(services)).Matches<SearchItem>(x => (x.Service & services) != 0));
        }
    }
}