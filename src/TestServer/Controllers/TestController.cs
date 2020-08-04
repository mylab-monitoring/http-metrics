using System;
using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("get/exception")]
        public IActionResult Get()
        {
            throw new Exception();
        }

        [HttpPost("post/{id}/data")]
        public IActionResult Post(int id, [FromBody]string data)
        {
            return Ok(data);
        }
    }
}