using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;

namespace MVCIdentity.Areas.Account.Controllers
{
    [Area("Account")]
    [AllowAnonymous]
    public class ConfirmEmailController : Controller
    {
        // PROPERTIES /////////////////////////////////////////////////////////
        private readonly UserManager<IdentityUser> _userManager;


        // CONSTRUCTORS ///////////////////////////////////////////////////////
        public ConfirmEmailController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }


        // HTTP ACTIONS ///////////////////////////////////////////////////////
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery]string userId, [FromQuery]string code)
        {
            if(string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                return LocalRedirect("~/");
            }

            var user = await _userManager.FindByIdAsync(userId);

            string statusMessage = "There was an error confirming your email.";

            if(user != null)
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

                var result = await _userManager.ConfirmEmailAsync(user, code);

                if(result.Succeeded)
                {
                    statusMessage = "Your email has been confirmed. Thank you.";
                }
            }

            return View(model: statusMessage);
        }
    }
}
