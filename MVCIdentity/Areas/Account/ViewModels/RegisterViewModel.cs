using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;

namespace MVCIdentity.Areas.Account.ViewModels
{
    public class RegisterViewModel
    {
        public RegisterModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public RegisterViewModel()
        {
            ExternalLogins = new List<AuthenticationScheme>();
        }

        public RegisterViewModel(IList<AuthenticationScheme> externalLogins, string returnUrl)
        {
            externalLogins ??= new List<AuthenticationScheme>();

            ExternalLogins = externalLogins;

            Input = new RegisterModel()
            {
                ReturnUrl = returnUrl
            };
            
        }
    }
}
