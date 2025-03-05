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

        public MovieDetailsPageModel(OmdbService omdbService, ApplicationDbContext database)
        {
            _omdbService = omdbService;
            _database = database;
        }

        public MovieFull Movie { get; set; } = new();

        [BindProperty]
        public Review Review { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();

        //En INT vi använder för att sortera reviews baserat på hur många stjärnor den har
        [BindProperty]
        public int SearchReview { get; set; }

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

        public async Task<IActionResult> OnPostStarFilter()
        {
            Movie = await _omdbService.GetMovieByIdAsync(Review.ImdbID);
            Reviews = await _database.Reviews.Where(r => r.ImdbID == Review.ImdbID).ToListAsync();

            //Abdi la till
            ViewData["ReviewFilter"] = Reviews;
            ViewData["Movie"] = Movie;

            TempData["ScrollToReviews"] = true; // Sätt flaggan
            return Page();
        }
    }
}
