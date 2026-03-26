using API.Utility;
using System.Security.Claims;

namespace IdentityApp.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(StaticDetails.UserId)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
        public static string GetName(this ClaimsPrincipal user)
        {
            return user.FindFirst(StaticDetails.Name)?.Value; ;
        }
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(StaticDetails.UserName)?.Value; ;
        }
        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(StaticDetails.Email)?.Value;
        }
    }
}
