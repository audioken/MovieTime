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
        try
        {
            // Hämta användaren från databasen
            var appUser = await _userManager.GetUserAsync(user);
            if (appUser == null)
            {
                // Om användaren inte finns i databasen, returnera en tom UserContext
                return new UserContext { IsAdmin = false, DisplayName = string.Empty };
            }

            // Kolla om användaren är admin
            var isAdmin = await _userManager.IsInRoleAsync(appUser, "Admin");
            return new UserContext
            {
                // Returnera en UserContext med användarens admin-status och visningsnamn
                IsAdmin = isAdmin,
                DisplayName = appUser.DisplayName ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ett fel uppstod: {ex.Message}");
            return new UserContext { IsAdmin = false, DisplayName = string.Empty };
        }
    }
}
