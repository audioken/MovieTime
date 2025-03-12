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

        public bool IsUserSignedIn => User.Identity?.IsAuthenticated ?? false;
        public string DisplayName { get; set; } = string.Empty;

        [BindProperty]
        public Review? Review { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Hämta recension från databasen
            Review = await _database.Reviews.FindAsync(id);

            if (Review == null)
            {
                return NotFound();
            }

            // Använd UserService för att hämta användarkontext
            var userContext = await _userService.GetUserContextAsync(User);
            DisplayName = userContext?.DisplayName ?? "Okänd användare";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Review == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Review.DateTime = DateTime.Now;

            _database.Reviews.Update(Review);
            await _database.SaveChangesAsync();

            return RedirectToPage("MovieDetailsPage", new { imdbID = Review.ImdbID });
        }
    }
}
