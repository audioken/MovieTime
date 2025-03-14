using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MowiTajm.Data;
using MowiTajm.Models;

namespace MowiTajm.Pages.Movies
{
    public class EditReviewPageModel : PageModel
    {
        private readonly ApplicationDbContext _database;
        private readonly IUserService _userService;

        public EditReviewPageModel(ApplicationDbContext database, IUserService userService)
        {
            _database = database;
            _userService = userService;
        }

        [BindProperty]
        public Review Review { get; set; } = new();

        public bool IsUserSignedIn => User.Identity?.IsAuthenticated ?? false;
        public string DisplayName { get; set; } = string.Empty;

        ////////////////////////////////////////////////////////////////////////////////////////////

        // Metod som körs när sidan laddas
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                // Nullcheck innan vi tilldelar Review det hämtade värdet
                Review? review = await _database.Reviews.FindAsync(id);
                if (review == null) { return NotFound(); }
                Review = review;

                // Hämta användarkontext
                var userContext = await _userService.GetUserContextAsync(User);

                // Hämter användarens visningsnamn eller sätter det till "Okänd användare"
                DisplayName = userContext?.DisplayName ?? "Okänd användare";

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid hämtning av recension: {ex.Message}");
                ViewData["ErrorMessage"] = "Ett fel uppstod vid hämtning av recensionen. Försök igen senare.";
                return Page();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        // Metod som körs när formuläret postas
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Kontrollerar att recensionen finns i databasen
                if (Review == null) { return NotFound(); }

                // Kontrollerar att formuläret är korrekt ifyllt
                if (!ModelState.IsValid) { return Page(); }

                Review.DateTime = DateTime.Now;

                // Uppdaterar databasen med den nya recensionen
                _database.Reviews.Update(Review);
                await _database.SaveChangesAsync();

                // Returnerar användaren till MovieDetailsPage och filmen som recensionen tillhör
                return RedirectToPage("MovieDetailsPage", new { imdbID = Review.ImdbID });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid uppdatering av recension: {ex.Message}");
                ViewData["ErrorMessage"] = "Ett fel uppstod vid uppdatering av recensionen. Försök igen senare.";
                return Page();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
    }
}
