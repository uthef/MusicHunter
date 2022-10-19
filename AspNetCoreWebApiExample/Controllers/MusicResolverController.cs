using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Uthef.MusicResolver;

namespace AspNetCoreWebApiExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MusicResolverController : ControllerBase
    {
        private static MusicResolver _resolver;

        static MusicResolverController()
        {
            _resolver = new MusicResolver(
                new MusicResolverConfiguration(
                    spotifyClientId: "c65a78a3e14048edaecd6858adf234cc",
                    spotifyClientSecret: "3aed4954a11a4214ac2b3f4fff435d2a",
                    soundCloudClientId: "KghzQtan1SZyxSkNGHNepznp7GWgaYMG",
                    true
                )
           );
        }

        private readonly ILogger<MusicResolverController> _logger;

        public MusicResolverController(ILogger<MusicResolverController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("/Resolve")]
        public async Task<IActionResult> Resolve(ResolverRequestBody body)
        {
            var results = await _resolver.SearchAsync(body.Query, body.Type, body.Services, new StrictFilter(body.StrictArtist, body.StrictTitle));
            return new JsonResult(results);
        }
    }
}