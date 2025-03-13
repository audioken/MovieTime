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

        // Metod som körs när sidan laddas
        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Nullcheck vid hämtning av review
            Review? review = await _database.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            Review = review;

            // Använd UserService för att hämta användarkontext
            var userContext = await _userService.GetUserContextAsync(User);

            // Visa användarnamn om användaren är inloggad och använd "Okänd användare" som fallback
            DisplayName = userContext?.DisplayName ?? "Okänd användare";

            return Page();
        }

        // Metod som körs när formuläret postas
        public async Task<IActionResult> OnPostAsync()
        {
            if (Review == null)
            {
                return NotFound();
            }

            // Kontrollerar att formuläret är korrekt ifyllt
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Review.DateTime = DateTime.Now; // Sätt nuvarande tid för recensionen

            // Uppdaterar databasen med den nya recensionen
            _database.Reviews.Update(Review);
            await _database.SaveChangesAsync();

            // Returnerar användaren till MovieDetailsPage och filmen som recensionen tillhör
            return RedirectToPage("MovieDetailsPage", new { imdbID = Review.ImdbID });
        }
    }
}
