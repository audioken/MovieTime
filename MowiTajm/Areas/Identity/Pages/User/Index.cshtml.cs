using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MowiTajm.Data;
using MowiTajm.Models;

namespace MowiTajm.Areas.Identity.Pages.User
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;
        private readonly ReviewService _reviewService;

        public IndexModel(IUserService userService, ApplicationDbContext context, ReviewService reviewService)
        {
            _userService = userService;
            _context = context;
            _reviewService = reviewService;
        }

        // Säkrar nullvärde genom att sätta en tom lista som standardvärde
        public List<Review> Reviews { get; set; } = new List<Review>();

        ////////////////////////////////////////////////////////////////////////////////////////////

        // Metod som hämtar recensioner från databasen baserat på inloggad användare
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Hämta användarkontext
                var user = await _userService.GetUserContextAsync(User);

                var displayName = user.DisplayName; //  Viktigt, inte UserName

                // Hämta recensioner från databasen baserat på användarens visningsnamn
                Reviews = await _context.Reviews
                    .Where(r => r.Username == displayName) //  Matcha Username mot DisplayName
                    .ToListAsync();

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid hämtning av recensioner: {ex.Message}");
                ViewData["ErrorMessage"] = "Ett fel uppstod vid hämtning av recensioner. Försök igen senare.";
                return Page();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        // Metod som tar bort recensioner från databasen
        public async Task<IActionResult> OnPostDeleteReviewAsync(int reviewId)
        {
            try
            {
                await _reviewService.DeleteReviewAsync(reviewId);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid borttagning av recension: {ex.Message}");
                ViewData["ErrorMessage"] = "Ett fel uppstod vid borttagning av recensionen. Försök igen senare.";
                return Page();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        // Metod som hanterar redigering av recensioner
        public async Task<IActionResult> OnPostEditReviewAsync(int reviewId)
        {
            try
            {
                // Hämta recensionen från databasen med reviewId
                var review = await _context.Reviews.FindAsync(reviewId);
                if (review == null)
                {
                    return NotFound();
                }

                // Skicka vidare till EditReviewPage med den här recensionen
                return RedirectToPage("/Movies/EditReviewPage", new { id = review.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid redigering av recension: {ex.Message}");
                ViewData["ErrorMessage"] = "Ett fel uppstod vid redigering av recensionen. Försök igen senare.";
                return Page();
            }
        }
    }
}


