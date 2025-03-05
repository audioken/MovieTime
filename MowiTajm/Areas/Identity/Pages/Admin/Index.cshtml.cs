using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MowiTajm.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MowiTajm.Pages.Admin
{

    public class IndexModel : PageModel
    {
        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

       
        public IndexModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

       
        public IList<ApplicationUser> Users { get; set; }

       
        public IList<string> Roles { get; set; } = new List<string> { "User", "Admin" };
        public Dictionary<string, string> UserRoles { get; set; } = new Dictionary<string, string>();

       
        public async Task OnGetAsync()
        {
        
            Users = _userManager.Users.ToList();

            foreach (var user in Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                UserRoles[user.Id] = roles.FirstOrDefault() ?? "No Role"; 
            }
        }
        
        public async Task<IActionResult> OnPostUpdateRoleAsync(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

           
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            
            var result = await _userManager.AddToRoleAsync(user, newRole);

            if (!result.Succeeded)
            {
                return BadRequest("Error while assigning new role.");
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest("Error while deleting user.");
                }
            }
            return RedirectToPage();
        }
    }
}

