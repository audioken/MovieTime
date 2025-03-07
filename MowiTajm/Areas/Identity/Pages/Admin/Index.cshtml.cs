using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MowiTajm.Data;
using MowiTajm.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public IndexModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public Dictionary<string, string> UserRoles { get; set; } = new Dictionary<string, string>();
    public IList<Review> Reviews { get; set; } = new List<Review>();

    [BindProperty]
    public bool ShowReviews { get; set; } = false; // Standard: Visa användare först

    public async Task OnGetAsync()
    {
        await LoadUsersAsync(); // Standard: Ladda användare direkt
    }

    public async Task<IActionResult> OnPostLoadUsersAsync()
    {
        ShowReviews = false;
        await LoadUsersAsync();
        return Page();
    }

    public IActionResult OnPostLoadReviewsAsync()
    {
        ShowReviews = true;
        Reviews = _context.Reviews.ToList();
        return Page();
    }

    private async Task LoadUsersAsync()
    {
        Users = _userManager.Users.ToList();
        foreach (var user in Users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            UserRoles[user.Id] = roles.FirstOrDefault() ?? "Ingen roll";
        }
    }

    public async Task<IActionResult> OnPostUpdateRoleAsync(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        var result = await _userManager.AddToRoleAsync(user, newRole);

        if (!result.Succeeded) return BadRequest("Fel vid ändring av roll.");

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest("Fel vid borttagning av användare.");
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteReviewAsync(int reviewId)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}