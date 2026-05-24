using Microsoft.AspNetCore.Mvc;

namespace CleanCity.MVC.Controllers
{
    public class ApiPingController : Controller
    {
        private readonly IHttpClientFactory _factory;
        public ApiPingController(IHttpClientFactory factory) => _factory = factory;

        [HttpGet("/api-ping")]
        public async Task<IActionResult> Ping()
        {
            var client = _factory.CreateClient("CleanCityApi");
            var json = await client.GetStringAsync("/swagger/v1/swagger.json");
            return Content($"API Connected OK. Swagger JSON length = {json.Length}");
        }
    }
}
