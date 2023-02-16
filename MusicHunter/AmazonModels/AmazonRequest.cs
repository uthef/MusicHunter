using System.Text.Json;
using System.Text.Json.Serialization;

namespace Uthef.MusicHunter.AmazonModels
{
    public class AmazonRequest
    {
        public string Keyword { get; }
        public string UserHash { get; }
        public string Headers { get; }

        [JsonConstructor]
        public AmazonRequest(string keyword, string userHash, string headers)
        {
            Keyword = keyword;
            UserHash = userHash;
            Headers = headers;
        }

        public AmazonRequest(string keyword)
        {
            Keyword = $"{{\"interface\":\"Web.TemplatesInterface.v1_0.Touch.SearchTemplateInterface.SearchKeywordClientInformation\",\"keyword\":\"{keyword}\"}}";
            UserHash = "{\"level\":\"LIBRARY_MEMBER\"}";
            Headers = "{\"x-amzn-authentication\":\"{\\\"interface\\\":\\\"ClientAuthenticationInterface.v1_0.ClientTokenElement\\\",\\\"accessToken\\\":\\\"\\\"}\",\"x-amzn-device-model\":\"WEBPLAYER\",\"x-amzn-device-width\":\"1920\",\"x-amzn-device-family\":\"WebPlayer\",\"x-amzn-device-id\":\"13152957259670857\",\"x-amzn-user-agent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/109.0\",\"x-amzn-session-id\":\"134-2432793-5495646\",\"x-amzn-device-height\":\"1080\",\"x-amzn-request-id\":\"72f89a0e-7966-495a-9617-4124af8ef192\",\"x-amzn-device-language\":\"en_US\",\"x-amzn-currency-of-preference\":\"USD\",\"x-amzn-os-version\":\"1.0\",\"x-amzn-application-version\":\"1.0.11821.0\",\"x-amzn-device-time-zone\":\"Europe/Moscow\",\"x-amzn-timestamp\":\"1676544722738\",\"x-amzn-csrf\":\"{\\\"interface\\\":\\\"CSRFInterface.v1_0.CSRFHeaderElement\\\",\\\"token\\\":\\\"AIfHhfifImKwn0q0XBE3dBSrxEv1A7Yj6BwZ1Yq9nDE=\\\",\\\"timestamp\\\":\\\"1676544710581\\\",\\\"rndNonce\\\":\\\"755410613\\\"}\",\"x-amzn-music-domain\":\"music.amazon.com\",\"x-amzn-referer\":\"music.amazon.com\",\"x-amzn-affiliate-tags\":\"\",\"x-amzn-ref-marker\":\"\",\"x-amzn-page-url\":\"https://music.amazon.com/search\",\"x-amzn-weblab-id-overrides\":\"\",\"x-amzn-video-player-token\":\"\",\"x-amzn-feature-flags\":\"hd-supported\"}";
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}
