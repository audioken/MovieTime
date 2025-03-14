using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MowiTajm.Data;
using MowiTajm.Models;

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

    ////////////////////////////////////////////////////////////////////////////////////////////

    // Metod som laddar användarna vid sidans första uppslag
    public async Task OnGetAsync()
    {
        try
        {
            await LoadUsersAsync(); // Standard: Ladda användare direkt
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel vid laddning av användare: {ex.Message}");
            ViewData["ErrorMessage"] = "Ett fel uppstod vid laddning av användare. Försök igen senare.";
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////

    // Metod som laddar om användarna när knappen "Hantera användare" trycks
    public async Task<IActionResult> OnPostLoadUsersAsync()
    {
        try
        {
            ShowReviews = false;
            await LoadUsersAsync();
            return Page();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel vid omladdning av användare: {ex.Message}");
            ViewData["ErrorMessage"] = "Ett fel uppstod vid omladdning av användare. Försök igen senare.";
            return Page();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////

    // Metod som laddar recensionerna när knappen "Hantera recensioner" trycks
    public IActionResult OnPostLoadReviewsAsync()
    {
        try
        {
            ShowReviews = true;
            Reviews = _context.Reviews.ToList();
            return Page();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel vid laddning av recensioner: {ex.Message}");
            ViewData["ErrorMessage"] = "Ett fel uppstod vid laddning av recensioner. Försök igen senare.";
            return Page();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////

    // Metod som laddar användarna och deras roller från databasen
    private async Task LoadUsersAsync()
    {
        try
        {
            Users = _userManager.Users.ToList();
            foreach (var user in Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                UserRoles[user.Id] = roles.FirstOrDefault() ?? "Ingen roll";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel vid laddning av användare och deras roller: {ex.Message}");
            ViewData["ErrorMessage"] = "Ett fel uppstod vid laddning av användare och roller. Försök igen senare.";
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////

    // Metod som uppdaterar en användares roll
    public async Task<IActionResult> OnPostUpdateRoleAsync(string userId, string newRole)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            var result = await _userManager.AddToRoleAsync(user, newRole);

            if (!result.Succeeded) return BadRequest("Fel vid ändring av roll.");

            return RedirectToPage();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel vid uppdatering av användarroll: {ex.Message}");
            ViewData["ErrorMessage"] = "Ett fel uppstod vid uppdatering av användarens roll. Försök igen senare.";
            return Page();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////

    // Metod som tar bort en användare från databasen
    public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded) return BadRequest("Fel vid borttagning av användare.");
            }
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel vid borttagning av användare: {ex.Message}");
            ViewData["ErrorMessage"] = "Ett fel uppstod vid borttagning av användaren. Försök igen senare.";
            return Page();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////

    // Metod som tar bort en recension från databasen
    public async Task<IActionResult> OnPostDeleteReviewAsync(int reviewId)
    {
        try
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel vid borttagning av recension: {ex.Message}");
            ViewData["ErrorMessage"] = "Ett fel uppstod vid borttagning av recensionen. Försök igen senare.";
            return Page();
        }
    }
}