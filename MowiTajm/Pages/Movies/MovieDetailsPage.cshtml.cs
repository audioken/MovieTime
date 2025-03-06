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

        public MovieDetailsPageModel(OmdbService omdbService, ApplicationDbContext database, SignInManager<IdentityUser> signÍnManager)
        {
            _omdbService = omdbService;
            _database = database;
            _signInManager = signÍnManager;
        }

        public bool IsUserSignedIn => _signInManager.IsSignedIn(User);

        public MovieFull Movie { get; set; } = new();

        [BindProperty]
        public Review Review { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();

        //En INT vi använder för att sortera reviews baserat på hur många stjärnor den har
        [BindProperty]
        public int SearchReview { get; set; } // 0 = alla, 1-5 stjärnor, 6 = stigande datum, 7 fallande datum

        //Genomsnittlig review för filmen baserat på reviews på MovieTime
        public double MovieTimeReview { get; set; }

		public async Task OnGetAsync(string imdbID)
		{
			if (!string.IsNullOrWhiteSpace(imdbID))
			{
				// Hämta filmen från API:et och recensionerna från databasen
				Movie = await _omdbService.GetMovieByIdAsync(imdbID);
				Reviews = await _database.Reviews.Where(r => r.ImdbID == imdbID).ToListAsync();

				// Spara IMDB-ID för att kunna använda det i formuläret
				Review.ImdbID = imdbID;
			}

			// Beräkna MovieTimeReview baserat på alla recensioner (inte filtrerade recensioner)
			MovieTimeReview = Reviews.Count > 0 ? Math.Round(Reviews.Average(r => r.Rating), 2) : 0;

			// Spara MovieTimeReview i ViewData för att användas på sidan
			ViewData["MovieTimeReview"] = MovieTimeReview;
		}




		public async Task<IActionResult> OnPostAddReview()
        {
            if (!ModelState.IsValid)
            {
                // Ladda om sidan och dess innehåll om valideringen misslyckas
                Movie = await _omdbService.GetMovieByIdAsync(Review.ImdbID);
                Reviews = await _database.Reviews.Where(r => r.ImdbID == Review.ImdbID).ToListAsync();
                return Page();
            }

            // Spara aktuellt datum och tid
            Review.DateTime = DateTime.Now;

            // Lägg till recensionen i databasen och spara ändringarna
            _database.Reviews.Add(Review);
            await _database.SaveChangesAsync();

            // Ladda om sidan och dess innehåll
            return RedirectToPage("MovieDetailsPage", new { imdbId = Review.ImdbID });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // Hämta recensionen från databasen
            var review = await _database.Reviews.FindAsync(id);

            if (review != null)
            {
                // Ta bort recensionen från databasen och spara ändringarna
                _database.Reviews.Remove(review);
                await _database.SaveChangesAsync();
            }
            // Ladda om sidan och dess innehåll
            return RedirectToPage("MovieDetailsPage", new { imdbId = review.ImdbID });
        }

		//Vi laddar om sidan när användaren väljer att filtrera reviews baserat på stjärnor
		public async Task<IActionResult> OnPostStarFilter()
		{
			Movie = await _omdbService.GetMovieByIdAsync(Review.ImdbID);
			Reviews = await _database.Reviews.Where(r => r.ImdbID == Review.ImdbID).ToListAsync();

			// Filtrera recensionerna baserat på stjärnorna
			if (SearchReview >= 1 && SearchReview <= 5)
			{
				Reviews = Reviews.Where(r => r.Rating == SearchReview).ToList();
			}

			// Spara de filtrerade recensionerna
			ViewData["ReviewFilter"] = Reviews;
			ViewData["Movie"] = Movie;

			// Återgå till sidan med uppdaterad information
			TempData["ScrollToReviews"] = true;
			return Page();
		}


		//Vi laddar om sidan när användaren väljer att filtrera reviews baserat på datum
		public async Task<IActionResult> OnPostDateFilter()
		{
			Movie = await _omdbService.GetMovieByIdAsync(Review.ImdbID);
			Reviews = await _database.Reviews.Where(r => r.ImdbID == Review.ImdbID).ToListAsync();

			// Läs in föregående SearchReview om det finns
			if (TempData["SearchReview"] is int prevSearchReview)
			{
				SearchReview = prevSearchReview;
			}

			// Om SearchReview är mindre än 6, sätt den till 6
			if (SearchReview < 6)
			{
				SearchReview = 6;
			}
			else if (SearchReview == 6)
			{
				SearchReview = 7;
			}
			else if (SearchReview == 7)
			{
				SearchReview = 6;
			}

			// Spara SearchReview i TempData för att behålla värdet
			TempData["SearchReview"] = SearchReview;
			//TempData["MovieTimeReview"] = MovieTimeReview; // Spara värdet i TempData

			TempData["ScrollToReviews"] = true; // Sätt flaggan
			return Page();
		}

	}
}
