using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("get/{id}/data")]
        public IActionResult Get(int id)
        {
            return Ok(
                "this is payload this is payload this is payload this is payload this is payload this is payload ");
        }

        [HttpPost("post/{id}/data")]
        public IActionResult Post(int id)
        {
            return Ok(
                "this is payload this is payload");
        }
    }
}