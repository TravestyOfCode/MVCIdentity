using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCIdentity.Areas.Account.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MVCIdentity.Areas.Account.Controllers
{
    [AllowAnonymous]
    [Area("Account")]
    public class LoginController : Controller
    {
        // PROPERTIES /////////////////////////////////////////////////////////
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly ILogger<LoginController> _logger;

        // CONSTRUCTORS ///////////////////////////////////////////////////////
        public LoginController(SignInManager<IdentityUser> signInManager, ILogger<LoginController> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        // HTTP ACTIONS ///////////////////////////////////////////////////////
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery]string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return View(await GenerateViewModel(returnUrl));
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm]LoginModel input, [FromQuery]string returnUrl)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(input.Email, input.Password, input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(await GenerateViewModel(returnUrl));
        }

        // PRIVATE FUNCTIONS //////////////////////////////////////////////////
        private async Task<LoginViewModel> GenerateViewModel(string returnUrl)
        {
            var externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = externalLogins
            };
        }

    }
}
