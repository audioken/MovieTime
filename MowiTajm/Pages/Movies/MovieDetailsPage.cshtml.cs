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
        public string DateSortText = "";
        public List<Review> FilteredReviews { get; set; } = new();
        public bool IsStarFilterActive { get; set; } = false;


        public async Task OnGetAsync(string imdbID)
        {
            // Kontrollera att imdbID inte är null eller tomt innan vi fortsätter
            if (!string.IsNullOrWhiteSpace(imdbID))
            {
                UserContext = await _userService.GetUserContextAsync(User);                             // Hämta användardata från service               
                (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(imdbID);    // Hämta filmen, recensioner och genomsnittlig rating från service
                ViewData["MowiTajmRating"] = MowiTajmRating;                                            // Spara MowiTajmRating i ViewData för att användas på sidan
                Review.ImdbID = imdbID;                                                                 // Spara IMDB-ID för att kunna använda det i formuläret                                                                 // Anropa metoden för att filtrera recensioner baserat på datum
                FilterValue = 6; // Standard: Senaste
                DateSortText = "Senaste";
                Reviews = Reviews.OrderByDescending(r => r.DateTime).ToList();
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
                                                                                            // Spara filmens titel i recensionen
            Review.DateTime = DateTime.Now;                                                 // Spara aktuellt datum och tid
            await _reviewService.AddReviewAsync(Review);                                    // Använd ReviewService för att lägga till recensionen
            return RedirectToPage("MovieDetailsPage", new { imdbId = Review.ImdbID });      // Ladda om sidan och dess innehåll
        }

        public async Task<IActionResult> OnPostStarFilter()
        {
            UserContext = await _userService.GetUserContextAsync(User);
            (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(Review.ImdbID);
            ViewData["MowiTajmRating"] = MowiTajmRating;

            // Rensa listan med filtrerade recensioner
            FilteredReviews.Clear();

            // Filtrera recensionerna baserat på stjärnorna
            if (FilterValue >= 1 && FilterValue <= 5)
            {
                FilteredReviews = Reviews.Where(r => r.Rating == FilterValue).ToList();
                IsStarFilterActive = true;
            }
            else
            {
                IsStarFilterActive = false;
            }

            // Sätt standardvärde för DateSortText och sortera recensionerna
            DateSortText = "Senaste";
            FilteredReviews = FilteredReviews.OrderByDescending(r => r.DateTime).ToList();


            // Återgå till sidan med uppdaterad information
            TempData["ScrollToReviews"] = true;
            return Page();
        }

        public async Task<IActionResult> OnPostDateFilter()
        {
            UserContext = await _userService.GetUserContextAsync(User);
            (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(Review.ImdbID);
            ViewData["MowiTajmRating"] = MowiTajmRating;

            // Läs in föregående FilterValue om det finns, annars sätt ett defaultvärde (6 = "Senaste")
            if (TempData["FilterValue"] is int prevFilterValue)
            {
                FilterValue = prevFilterValue;
            }
            else
            {
                FilterValue = 6;
            }

            // Toggla FilterValue: om den är 6 (nyaste) blir den 7 (äldsta), annars blir den 6
            FilterValue = (FilterValue == 6) ? 7 : 6;

            // Sätt DateSortText baserat på den nya FilterValue
            DateSortText = (FilterValue == 6) ? "Senaste" : "Äldsta";

            if (FilterValue == 6)
            {
                Reviews = Reviews.OrderByDescending(r => r.DateTime).ToList();
            }
            else
            {
                Reviews = Reviews.OrderBy(r => r.DateTime).ToList();
            }

            // Spara värdena i TempData för nästa anrop
            TempData["FilterValue"] = FilterValue;
            TempData["DateSortOrder"] = DateSortText;
            TempData["ScrollToReviews"] = true; // Sätt flaggan för scroll

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
