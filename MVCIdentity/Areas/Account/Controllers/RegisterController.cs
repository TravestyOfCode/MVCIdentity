﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MVCIdentity.Areas.Account.ViewModels;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MVCIdentity.Areas.Account.Controllers
{
    [AllowAnonymous]
    [Area("Account")]
    public class RegisterController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly ILogger<RegisterController> _logger;

        private readonly IEmailSender _emailSender;

        public RegisterController(SignInManager<IdentityUser> signInManager, ILogger<RegisterController> logger, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery]string returnUrl = null)
        {
            var externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return View(new RegisterViewModel(externalLogins, returnUrl));
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm]RegisterModel input)
        {
            input.ReturnUrl ??= Url.Content("~/");
            
            if(ModelState.IsValid)
            {
                var user = new IdentityUser() { UserName = input.Email, Email = input.Email };

                var result = await _signInManager.UserManager.CreateAsync(user, input.Password);

                if(result.Succeeded)
                {
                    _logger.LogInformation($"User {input.Email} created a new account.");

                    var emailConfirmCode = await _signInManager.UserManager.GenerateEmailConfirmationTokenAsync(user);

                    emailConfirmCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmCode));

                    var callbackUrl = Url.Action(
                        action: "Index",
                        controller: "ConfirmEmail",
                        values: new { area = "Account", userId = user.Id, code = emailConfirmCode, returnUrl = input.ReturnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(
                        email: input.Email,
                        subject: "Confirm your email",
                        htmlMessage: $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if(_signInManager.UserManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToAction(
                            actionName: "Index",
                            controllerName: "RegisterConfirmation",
                            routeValues: new { email = input.Email, returnUrl = input.ReturnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(input.ReturnUrl);
                    }
                }

                foreach(var error in result.Errors)
                {
                    ModelState.TryAddModelError(string.Empty, error.Description);
                }                
            }

            // Something failed
            var externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return View(new RegisterViewModel(externalLogins, input.ReturnUrl));
        }
    }
}