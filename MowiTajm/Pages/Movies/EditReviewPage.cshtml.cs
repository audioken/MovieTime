using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MowiTajm.Data;
using MowiTajm.Models;

namespace MowiTajm.Pages.Movies
{
    public class EditReviewPageModel : PageModel
    {
        private readonly ApplicationDbContext _database;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly UserManager<ApplicationUser> _userManager;

        public EditReviewPageModel(ApplicationDbContext database, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _database = database;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public bool IsUserSignedIn => _signInManager.IsSignedIn(User);
        public string DisplayName { get; set; }


        [BindProperty]
        public Review Review { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Review = await _database.Reviews.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            DisplayName = user?.DisplayName;


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
