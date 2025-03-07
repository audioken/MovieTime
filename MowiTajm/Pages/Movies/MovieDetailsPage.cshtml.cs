using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MowiTajm.Models;

namespace MowiTajm.Pages.Movies
{
    public class MovieDetailsPageModel : PageModel
    {
        private readonly MovieService _movieService;
        private readonly IUserService _userService;
        private readonly ReviewService _reviewService;

        public MovieDetailsPageModel(MovieService movieService, IUserService userService, ReviewService reviewService)
        {
            _movieService = movieService;
            _userService = userService;
            _reviewService = reviewService;
        }

        public UserContext UserContext { get; set; }
        public MovieFull Movie { get; set; } = new();

        [BindProperty]
        public Review Review { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();

        [BindProperty]
        public int FilterValue { get; set; }                            // Nummer som kontrollerer vilket filter som används
        public double MowiTajmRating { get; set; }                      // Genomsnittlig review för filmen baserat på reviews på MowiTajm
        public bool IsUserSignedIn => User.Identity.IsAuthenticated;    // True om användaren är inloggad, annars false

        public async Task OnGetAsync(string imdbID)
        {
            // Kontrollera att imdbID inte är null eller tomt innan vi fortsätter
            if (!string.IsNullOrWhiteSpace(imdbID))
            {
                UserContext = await _userService.GetUserContextAsync(User);                             // Hämta användardata från service               
                (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(imdbID);    // Hämta filmen, recensioner och genomsnittlig rating från service
                ViewData["MowiTajmRating"] = MowiTajmRating;                                            // Spara MowiTajmRating i ViewData för att användas på sidan
                Review.ImdbID = imdbID;                                                                 // Spara IMDB-ID för att kunna använda det i formuläret               
            }
            else
            {
                ViewData["ErrorMessage"] = "Filmens ID saknas eller är ogiltigt.";
            }
        }

        public async Task<IActionResult> OnPostAddReview()
        {
            if (!ModelState.IsValid)
            {
                // Hämta filmdetaljer och recensioner igen om valideringen misslyckas
                (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(Review.ImdbID);
                return Page();
            }

            Review.DateTime = DateTime.Now;                                                 // Spara aktuellt datum och tid
            await _reviewService.AddReviewAsync(Review);                                    // Använd ReviewService för att lägga till recensionen
            return RedirectToPage("MovieDetailsPage", new { imdbId = Review.ImdbID });      // Ladda om sidan och dess innehåll
        }

        public async Task<IActionResult> OnPostStarFilter()
        {
            UserContext = await _userService.GetUserContextAsync(User);
            (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(Review.ImdbID);

            // Filtrera recensionerna baserat på stjärnorna
            if (FilterValue >= 1 && FilterValue <= 5)
            {
                Reviews = Reviews.Where(r => r.Rating == FilterValue).ToList();
            }

            // Spara de filtrerade recensionerna
            ViewData["ReviewFilter"] = Reviews;
            ViewData["Movie"] = Movie;

            // Återgå till sidan med uppdaterad information
            TempData["ScrollToReviews"] = true;
            return Page();
        }

        public async Task<IActionResult> OnPostDateFilter()
        {
            UserContext = await _userService.GetUserContextAsync(User);
            (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(Review.ImdbID);

            // Läs in föregående SearchReview om det finns
            if (TempData["SearchReview"] is int prevSearchReview)
            {
                FilterValue = prevSearchReview;
            }

            // Kontroll för datumfiltret
            if (FilterValue < 6) { FilterValue = 6; } // Byt till datumfiltret
            else if (FilterValue == 6) { FilterValue = 7; } // Byt till motsatt datumfiltret
            else if (FilterValue == 7) { FilterValue = 6; } // Byt till datumfiltret

            // Spara SearchReview i TempData för att behålla värdet
            TempData["SearchReview"] = FilterValue;
            TempData["ScrollToReviews"] = true; // Sätt flaggan

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // Använd ReviewService för att ta bort recensionen
            await _reviewService.DeleteReviewAsync(id);

            // Ladda om filmdata inklusive recensioner
            (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(Review.ImdbID);

            // Ladda om sidan och dess innehåll
            return RedirectToPage("MovieDetailsPage", new { imdbId = Review.ImdbID });
        }
    }
}
