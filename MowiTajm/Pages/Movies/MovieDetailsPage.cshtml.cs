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

        public UserContext UserContext { get; set; } = new UserContext();
        public MovieFull Movie { get; set; } = new();

        [BindProperty]
        public Review Review { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();

        [BindProperty]
        public int FilterValue { get; set; } // Nummer som kontrollerer vilket filter som används
        public double MowiTajmRating { get; set; } // Genomsnittlig review för filmen baserat på reviews på MowiTajm
        public bool IsUserSignedIn => User.Identity?.IsAuthenticated ?? false;
        public string DateSortText = "";
        public List<Review> FilteredReviews { get; set; } = new();
        public bool IsStarFilterActive { get; set; } = false;

        ////////////////////////////////////////////////////////////////////////////////////////////

        // Metod som hämtar filmens detaljer och recensioner från databasen
        public async Task OnGetAsync(string imdbID)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(imdbID))
                {
                    await LoadMovieAndUserDataAsync(imdbID);

                    Review.ImdbID = imdbID; // Spara filmens ID i recensionen
                    FilterValue = 6; // Standard: Senaste
                    DateSortText = "Senaste";

                    // Anropa metoden för att filtrera recensioner baserat på datum
                    Reviews = Reviews.OrderByDescending(r => r.DateTime).ToList();
                }
                else
                {
                    ViewData["ErrorMessage"] = "Filmens ID saknas eller är ogiltigt.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid hämtning av film och recensioner: {ex.Message}");
                ViewData["ErrorMessage"] = "Ett fel uppstod vid hämtning av filmdetaljer eller recensioner.";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        // Metod som lägger till en recension i databasen
        public async Task<IActionResult> OnPostAddReview()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Hämta filmdetaljer och recensioner igen om valideringen misslyckas
                    await LoadMovieAndUserDataAsync(Review.ImdbID);
                    return Page();
                }

                // Spara aktuellt datum och tid
                Review.DateTime = DateTime.Now;

                // Anropa ReviewService för att lägga till recensionen
                await _reviewService.AddReviewAsync(Review);

                // Ladda om sidan och dess innehåll
                return RedirectToPage("MovieDetailsPage", new { imdbId = Review.ImdbID });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid tillägg av recension: {ex.Message}");
                ViewData["ErrorMessage"] = "Ett fel uppstod vid tillägg av recensionen.";
                return Page();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        // Metod som filtrerar recensioner baserat på antal stjärnor
        public async Task<IActionResult> OnPostStarFilter()
        {
            try
            {
                await LoadMovieAndUserDataAsync(Review.ImdbID);

                // Rensa listan med filtrerade recensioner
                FilteredReviews.Clear();

                // Filtrera recensionerna baserat på stjärnorna
                if (FilterValue >= 1 && FilterValue <= 5)
                {
                    FilteredReviews = Reviews.Where(r => r.Rating == FilterValue).ToList();
                    IsStarFilterActive = true;
                }
                else { IsStarFilterActive = false; }

                // Sortera de filtrerade recensionerna baserat på datum
                DateSortText = "Senaste";
                FilteredReviews = FilteredReviews.OrderByDescending(r => r.DateTime).ToList();

                // Återgå till sidan med uppdaterad information
                TempData["ScrollToReviews"] = true;

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid filtrering av recensioner: {ex.Message}");
                ViewData["ErrorMessage"] = "Ett fel uppstod vid filtrering av recensioner.";
                return Page();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        // Metod som sorterar recensioner baserat på datum
        public async Task<IActionResult> OnPostDateFilter()
        {
            try
            {
                await LoadMovieAndUserDataAsync(Review.ImdbID);

                // Läs in föregående FilterValue om det finns, annars sätt ett defaultvärde (6 = "Senaste")
                if (TempData["FilterValue"] is int prevFilterValue) { FilterValue = prevFilterValue; }
                else { FilterValue = 6; }

                // Toggla FilterValue: om den är 6 (nyaste) blir den 7 (äldsta), annars blir den 6
                FilterValue = (FilterValue == 6) ? 7 : 6;

                // Sätt DateSortText baserat på den nya FilterValue
                DateSortText = (FilterValue == 6) ? "Senaste" : "Äldsta";

                // Sortera recensionerna baserat på FilterValue
                if (FilterValue == 6) { Reviews = Reviews.OrderByDescending(r => r.DateTime).ToList(); }
                else { Reviews = Reviews.OrderBy(r => r.DateTime).ToList(); }

                // Spara värdena i TempData för nästa anrop
                TempData["FilterValue"] = FilterValue;
                TempData["DateSortOrder"] = DateSortText;
                TempData["ScrollToReviews"] = true; // Sätt flaggan för scroll

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid sortering av recensioner: {ex.Message}");
                ViewData["ErrorMessage"] = "Ett fel uppstod vid sortering av recensioner.";
                return Page();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        // Metod som tar bort en recension från databasen
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await _reviewService.DeleteReviewAsync(id);
                await LoadMovieAndUserDataAsync(Review.ImdbID);

                // Ladda om sidan och dess innehåll
                return RedirectToPage("MovieDetailsPage", new { imdbId = Review.ImdbID });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid borttagning av recension: {ex.Message}");
                ViewData["ErrorMessage"] = "Ett fel uppstod vid borttagning av recensionen.";
                return Page();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        // Hjälpmetod för att ladda filmens detaljer och användardata
        private async Task LoadMovieAndUserDataAsync(string imdbID)
        {
            try
            {
                // Hämta användardata från service
                UserContext = await _userService.GetUserContextAsync(User);

                // Hämta filmen, recensioner och genomsnittlig rating från service
                (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(imdbID);

                // Sätt MowiTajmRating i ViewData för att användas på sidan
                ViewData["MowiTajmRating"] = MowiTajmRating;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid inläsning av film och användardata: {ex.Message}");
                ViewData["ErrorMessage"] = "Ett fel uppstod vid hämtning av film och användardata.";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
    }
}
