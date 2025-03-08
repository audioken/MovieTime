using Microsoft.AspNetCore.Identity;
using MowiTajm.Models;
using System.Security.Claims;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserContext> GetUserContextAsync(ClaimsPrincipal user)
    {
        var appUser = await _userManager.GetUserAsync(user);
        if (appUser == null)
        {
            return new UserContext { IsAdmin = false, DisplayName = string.Empty };
        }

        var isAdmin = await _userManager.IsInRoleAsync(appUser, "Admin");
        return new UserContext
        {
            IsAdmin = isAdmin,
            DisplayName = appUser.DisplayName ?? string.Empty
        };
    }
}
