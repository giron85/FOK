using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Reti.Lab.FoodOnKontainers.Web.SPA.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly IOptionsSnapshot<AppSettings> _settings;

        public SampleDataController(IHostingEnvironment env, IOptionsSnapshot<AppSettings> settings)
        {
            _env = env;
            _settings = settings;
        }

        [HttpGet("[action]")]        
        public IActionResult MicroservicesEndpoint()
        {
            return Json(_settings.Value);
        }

        public class MicroserviceEndpoint
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }
    }
}
