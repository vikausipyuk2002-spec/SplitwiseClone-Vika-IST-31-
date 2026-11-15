using Microsoft.AspNetCore.Mvc;

namespace SplitwiseClone.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHealth() => Ok();

        [HttpGet("ready")]
        public IActionResult GetHealthReady() => Ok();
    }
}