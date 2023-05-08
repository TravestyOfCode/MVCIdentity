using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MVCIdentity.Areas.Account.Controllers
{
    [AllowAnonymous]
    [Area("Account")]
    public class RegisterConfirmationController : Controller
    {
        // PROPERTIES /////////////////////////////////////////////////////////
        private readonly UserManager<IdentityUser> _userManager;

        private readonly IEmailSender _emailSender;

        // CONSTRUCTORS ///////////////////////////////////////////////////////
        public RegisterConfirmationController(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // HTTP ACTIONS ///////////////////////////////////////////////////////
        [HttpGet]
        public IActionResult Index([FromQuery] string email)
        {
            return View(model: email);
        }

        [HttpGet]
        public async Task<IActionResult> Resend([FromQuery]string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user != null)
            {
                var emailConfirmCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                emailConfirmCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmCode));

                var callbackUrl = Url.Action(
                        action: "Index",
                        controller: "ConfirmEmail",
                        values: new { area = "Account", userId = user.Id, code = emailConfirmCode },
                        protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    email: email,
                    subject: "Confirm your email",
                    htmlMessage: $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            }

            return View();
        }            
    }
}
