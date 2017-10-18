using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PinotageTodo.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        [HttpGet("register")]
        public async Task<IActionResult> RegisterAsync()
        {
            var id = Guid.NewGuid();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, id.ToString())
            };

            var userIdentity = new ClaimsIdentity(claims, "login");

            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync("defaultCookieAuthScheme", principal);

            return new OkResult();
        }
    }
}
