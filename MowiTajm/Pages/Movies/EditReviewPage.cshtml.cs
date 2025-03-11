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

        public bool IsUserSignedIn => User.Identity.IsAuthenticated;
        public string DisplayName { get; set; } = string.Empty;


        [BindProperty]
        public Review Review { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // H�mta recension fr�n databasen
            Review = await _database.Reviews.FindAsync(id);

            // Anv�nd UserService f�r att h�mta anv�ndarkontext
            var userContext = await _userService.GetUserContextAsync(User);
            DisplayName = userContext.DisplayName;

            if (Review == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
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
