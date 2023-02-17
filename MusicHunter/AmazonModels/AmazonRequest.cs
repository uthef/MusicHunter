using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Uthef.MusicHunter.AmazonModels
{
    public class AmazonRequest
    {
        public string Keyword { get; }
        public string UserHash { get; }

        public string Headers { get; }

        private readonly JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        [JsonConstructor]
        public AmazonRequest(string keyword, string userHash, string headers)
        {
            Keyword = keyword;
            UserHash = userHash;
            Headers = headers;
        }

        public AmazonRequest(string query)
        {
            Keyword = JsonSerializer.Serialize(new
            {
                Interface = "Web.TemplatesInterface.v1_0.Touch.SearchTemplateInterface.SearchKeywordClientInformation",
                Keyword = query
            }, options);

            UserHash = JsonSerializer.Serialize(new
            {
                Level = "LIBRARY_MEMBER"
            }, options);

            var authentication = new
            {
                Interface = "ClientAuthenticationInterface.v1_0.ClientTokenElement",
                AccessToken = ""
            };

            var csrf = new
            {
                Interface = "CSRFInterface.v1_0.CSRFHeaderElement",
                Token = "AIfHhfifImKwn0q0XBE3dBSrxEv1A7Yj6BwZ1Yq9nDE=",
                Timestamp = "1676544710581",
                RndNonce = "755410613"
            };

            var headers = new Dictionary<string, string>
            {
                { "x-amzn-authentication", JsonSerializer.Serialize(authentication, options) },
                { "x-amzn-device-model", "WEBPLAYER" },
                { "x-amzn-device-width", "1920" },
                { "x-amzn-device-height", "1080" },
                { "x-amzn-device-family", "WebPlayer" },
                { "x-amzn-device-id", "13152957259670857" },
                { "x-amzn-user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/109.0" },
                { "x-amzn-session-id", "134-2432793-5495646" },
                { "x-amzn-request-id", "72f89a0e-7966-495a-9617-4124af8ef192" },
                { "x-amzn-os-version", "1.0" },
                { "x-amzn-timestamp", "1676544722738" },
                { "x-amzn-csrf", JsonSerializer.Serialize(csrf, options) },
                { "x-amzn-music-domain", "music.amazon.com" },
                { "x-amzn-page-url", "https://music.amazon.com/search" }
            };

            Headers = JsonSerializer.Serialize(headers, options);
        }

        public override string ToString() => JsonSerializer.Serialize(this, options);
    }
}
