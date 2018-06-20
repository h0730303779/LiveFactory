using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Linq;

namespace LiveFactory.Web.Common
{
    public interface IIdentityManager
    {
        ClaimsIdentity CreateIdentity(IdentityUser user);
        Task SignInAsync(IdentityUser user);
        Task SignOutAsync();
    }
    public class IdentityManager : IIdentityManager
    {
        IHttpContextAccessor _contextAccessor;
        public IdentityManager(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public virtual ClaimsIdentity CreateIdentity(IdentityUser user)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.PrimarySid, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            return identity;
        }

        public virtual async Task SignInAsync(IdentityUser user)
        {
            await _contextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(CreateIdentity(user)));
        }

        public virtual async Task SignOutAsync()
        {
            await _contextAccessor.HttpContext.SignOutAsync();
        }
    }


}
