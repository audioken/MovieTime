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

        public MovieFull Movie { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public double MowiTajmRating { get; set; } //Genomsnittlig review för filmen baserat på reviews på MovieTime
        public UserContext UserContext { get; set; }
        [BindProperty]
        public Review Review { get; set; } = new();

        // True om användaren är inloggad, annars false
        public bool IsUserSignedIn => User.Identity.IsAuthenticated;

        //En INT vi använder för att sortera reviews baserat på hur många stjärnor den har
        [BindProperty]
        public int SearchReview { get; set; }


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

            // Spara aktuellt datum och tid
            Review.DateTime = DateTime.Now;

            // Använd ReviewService för att lägga till recensionen
            await _reviewService.AddReviewAsync(Review);

            // Ladda om sidan och dess innehåll
            return RedirectToPage("MovieDetailsPage", new { imdbId = Review.ImdbID });
        }

        public async Task<IActionResult> OnPostStarFilter()
        {
            (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(Review.ImdbID);

            // Hämta användarkontexten via IUserService
            UserContext = await _userService.GetUserContextAsync(User);

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

        public async Task<IActionResult> OnPostDateFilter()
        {
            (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(Review.ImdbID);

            UserContext = await _userService.GetUserContextAsync(User);

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
