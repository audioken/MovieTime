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

        // Metod som k�rs n�r sidan laddas
        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Nullcheck vid h�mtning av review
            Review? review = await _database.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            Review = review;

            // Anv�nd UserService f�r att h�mta anv�ndarkontext
            var userContext = await _userService.GetUserContextAsync(User);

            // Visa anv�ndarnamn om anv�ndaren �r inloggad och anv�nd "Ok�nd anv�ndare" som fallback
            DisplayName = userContext?.DisplayName ?? "Ok�nd anv�ndare";

            return Page();
        }

        // Metod som k�rs n�r formul�ret postas
        public async Task<IActionResult> OnPostAsync()
        {
            if (Review == null)
            {
                return NotFound();
            }

            // Kontrollerar att formul�ret �r korrekt ifyllt
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Review.DateTime = DateTime.Now; // S�tt nuvarande tid f�r recensionen

            // Uppdaterar databasen med den nya recensionen
            _database.Reviews.Update(Review);
            await _database.SaveChangesAsync();

            // Returnerar anv�ndaren till MovieDetailsPage och filmen som recensionen tillh�r
            return RedirectToPage("MovieDetailsPage", new { imdbID = Review.ImdbID });
        }
    }
}
