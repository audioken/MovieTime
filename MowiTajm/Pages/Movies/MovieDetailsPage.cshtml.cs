using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MowiTajm.Data;
using MowiTajm.Models;
using MowiTajm.Services;

namespace MowiTajm.Pages.Movies
{
    public class MovieDetailsPageModel : PageModel
    {
        private readonly OmdbService _omdbService;
        private readonly ApplicationDbContext _database;
        private readonly SignInManager<IdentityUser> _signInManager;

        public MovieDetailsPageModel(OmdbService omdbService, ApplicationDbContext database, SignInManager<IdentityUser> sign�nManager)
        {
            _omdbService = omdbService;
            _database = database;
            _signInManager = sign�nManager;
        }

        public bool IsUserSignedIn => _signInManager.IsSignedIn(User);

        public MovieFull Movie { get; set; } = new();

        [BindProperty]
        public Review Review { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();

        //En INT vi anv�nder f�r att sortera reviews baserat p� hur m�nga stj�rnor den har
        [BindProperty]
        public int SearchReview { get; set; }

        //Genomsnittlig review f�r filmen baserat p� reviews p� MovieTime
        public double MovieTimeReview { get; set; }

        public async Task OnGetAsync(string imdbID)
        {
            if (!string.IsNullOrWhiteSpace(imdbID))
            {
                // H�mta filmen fr�n API:et och recensionerna fr�n databasen
                Movie = await _omdbService.GetMovieByIdAsync(imdbID);
                Reviews = await _database.Reviews.Where(r => r.ImdbID == imdbID).ToListAsync();

                // Spara IMDB-ID f�r att kunna anv�nda det i formul�ret
                Review.ImdbID = imdbID;
            }

            int AllReviews = 0;
            foreach (Review review in Reviews)
            {
				AllReviews += review.Rating;
			}

			//MovieTimeReview = Reviews.Count > 0 ? AllReviews / Reviews.Count : 0;
			MovieTimeReview = Reviews.Count > 0 ? Math.Round((double)AllReviews / Reviews.Count, 2) : 0;


		}

		public async Task<IActionResult> OnPostAddReview()
        {
            if (!ModelState.IsValid)
            {
                // Ladda om sidan och dess inneh�ll om valideringen misslyckas
                Movie = await _omdbService.GetMovieByIdAsync(Review.ImdbID);
                Reviews = await _database.Reviews.Where(r => r.ImdbID == Review.ImdbID).ToListAsync();
                return Page();
            }

            // Spara aktuellt datum och tid
            Review.DateTime = DateTime.Now;

            // L�gg till recensionen i databasen och spara �ndringarna
            _database.Reviews.Add(Review);
            await _database.SaveChangesAsync();

            // Ladda om sidan och dess inneh�ll
            return RedirectToPage("MovieDetailsPage", new { imdbId = Review.ImdbID });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // H�mta recensionen fr�n databasen
            var review = await _database.Reviews.FindAsync(id);

            if (review != null)
            {
                // Ta bort recensionen fr�n databasen och spara �ndringarna
                _database.Reviews.Remove(review);
                await _database.SaveChangesAsync();
            }
            // Ladda om sidan och dess inneh�ll
            return RedirectToPage("MovieDetailsPage", new { imdbId = review.ImdbID });
        }

        public async Task<IActionResult> OnPostStarFilter()
        {
            Movie = await _omdbService.GetMovieByIdAsync(Review.ImdbID);
            Reviews = await _database.Reviews.Where(r => r.ImdbID == Review.ImdbID).ToListAsync();

            //Abdi la till
            ViewData["ReviewFilter"] = Reviews;
            ViewData["Movie"] = Movie;

            TempData["ScrollToReviews"] = true; // S�tt flaggan
            return Page();
        }
    }
}
