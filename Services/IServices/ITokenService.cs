using IdentityApp.Models;

namespace IdentityApp.Services.IServices
{
    public interface ITokenService
    {
        string CreateJWT(AppUser user);
    }
}
