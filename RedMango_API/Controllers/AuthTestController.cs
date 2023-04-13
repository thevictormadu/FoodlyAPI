using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RedMango_API.Controllers
{
    [Route("api/AuthTest")]
    [ApiController]
    public class AuthTestController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<string>> GetSomething()
        {
            return "you are authenticated";
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<string>> GetSomething(int someIntValue)
        {
            return "you are authorized with role of admin";
        }
    }
}
