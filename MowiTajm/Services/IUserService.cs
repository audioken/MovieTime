using MowiTajm.Models;
using System.Security.Claims;

public interface IUserService
{
    Task<UserContext> GetUserContextAsync(ClaimsPrincipal user);
}
