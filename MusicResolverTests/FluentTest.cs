using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Uthef.MusicResolver;
using Uthef.MusicResolver.Filters;

namespace MusicResolverTests
{
    [TestFixture]
    public class FluentTest
    {
        MusicResolver MusicResolver;
        JsonSerializerOptions JsonSerializerOptions;

        [OneTimeSetUp]
        public void Setup()
        {
            MusicResolver = new MusicResolver(
                new MusicResolverConfiguration(
                    spotifyClientId: "c65a78a3e14048edaecd6858adf234cc",
                    spotifyClientSecret: "3aed4954a11a4214ac2b3f4fff435d2a",
                    useSoundCloudProxy: true
                )
            );

            JsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            };
        }

        [TestCase("celldweller - my disintegration", ItemType.Track)]
        public async Task TestAll(string query, ItemType type)
        {
            var resultCollection = await MusicResolver.SearchAsync(query, type, ServicePack.All);
            AssertSearchResultWithOutput(resultCollection, ServicePack.All);
        }

        [TestCase("celldweller - satellites", ItemType.Album, "celldweller", "satellites")]
        public async Task TestAllStrictly(string query, ItemType type, string strictArtist, string strictTitle)
        {
            var filter = new StrictFilter(strictArtist, strictTitle);

            var resultCollection = await MusicResolver.SearchAsync(query, type, ServicePack.All, filter);
            AssertSearchResultWithOutput(resultCollection, ServicePack.All);
        }

        [TestCase("pirate wicked", ItemType.Track, "coeur de pirate", "wicked games")]
        public async Task TestYoutubeAndSoundCloudStrictly(string query, ItemType itemType, string strictArtist, string strictTitle)
        {
            var filter = new StrictFilter(strictArtist, strictTitle);
            var servicePack = new ServicePack(MusicService.YouTube, MusicService.SoundCloud);
            var resultCollection = await MusicResolver.SearchAsync(query, itemType, servicePack, filter);
            AssertSearchResultWithOutput(resultCollection, servicePack);
        }

        [TestCase("ic3peak - грустная сука", ItemType.Track)]
        public async Task TestSoundCloud(string query, ItemType type)
        {
            var servicePack = MusicService.SoundCloud;
            var resultCollection = await MusicResolver.SearchAsync(query, type, servicePack);
            AssertSearchResultWithOutput(resultCollection, servicePack);
        }

        [TestCase("five crumbs", ItemType.Album)]
        public async Task TestBandcamp(string query, ItemType type)
        {
            var servicePack = MusicService.Bandcamp;
            var resultCollection = await MusicResolver.SearchAsync(query, type, servicePack);
            AssertSearchResultWithOutput(resultCollection, servicePack);
        }

        [TestCase("david galloway - we wait so long to begin", ItemType.Album)]
        [TestCase("machine head - circle the drain", ItemType.Track)]
        public async Task TestAmazon(string query, ItemType itemType)
        {
            var result = await MusicResolver.SearchAsync(query, itemType, MusicService.Amazon);
            AssertSearchResultWithOutput(result, MusicService.Amazon);
        }

        private void AssertSearchResultWithOutput(SearchResult resultCollection, ServicePack pack)
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

            Assert.That(resultCollection.Items, Has.Exactly(pack.Items.Count).Matches<SearchItem>(x => pack.Items.Contains(x.Service)));
        }
    }
}