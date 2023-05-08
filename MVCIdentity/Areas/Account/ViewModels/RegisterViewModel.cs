using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;

namespace MVCIdentity.Areas.Account.ViewModels
{
    public class RegisterViewModel    
    {
        public RegisterModel Input { get; set; } = new RegisterModel();

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();
    }
}
