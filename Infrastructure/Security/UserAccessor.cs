using System.Security.Claims;
using Application.interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Security
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAcc;

        public UserAccessor(IHttpContextAccessor httpContextAcc)
        {
            _httpContextAcc = httpContextAcc;
        }

        public string GetUsername()
        {
            return _httpContextAcc.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }
    }
}
