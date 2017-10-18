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
            // for our current example we issue a long-lived auth cookie and on every refresh 
            // of the app we check here for its existence. If it doesn't exist we create it.
            if (HttpContext.User.Claims.Any(c => c.Type.Equals(ClaimTypes.Name))) return new OkResult();
            
            var id = Guid.NewGuid();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, id.ToString())
            };

            var userIdentity = new ClaimsIdentity(claims, "login");

            var principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync("defaultCookieAuthScheme", principal);

            return new OkResult();
        }
    }
}
