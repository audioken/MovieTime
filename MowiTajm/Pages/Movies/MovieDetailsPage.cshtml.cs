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
        public int FilterValue { get; set; }                            // Nummer som kontrollerer vilket filter som anv�nds
        public double MowiTajmRating { get; set; }                      // Genomsnittlig review f�r filmen baserat p� reviews p� MowiTajm
        public bool IsUserSignedIn => User.Identity.IsAuthenticated;    // True om anv�ndaren �r inloggad, annars false
        public string DateSortText = "";
        public List<Review> FilteredReviews { get; set; } = new();
        public bool IsStarFilterActive { get; set; } = false;


        public async Task OnGetAsync(string imdbID)
        {
            // Kontrollera att imdbID inte �r null eller tomt innan vi forts�tter
            if (!string.IsNullOrWhiteSpace(imdbID))
            {
                UserContext = await _userService.GetUserContextAsync(User);                             // H�mta anv�ndardata fr�n service               
                (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(imdbID);    // H�mta filmen, recensioner och genomsnittlig rating fr�n service
                ViewData["MowiTajmRating"] = MowiTajmRating;                                            // Spara MowiTajmRating i ViewData f�r att anv�ndas p� sidan
                Review.ImdbID = imdbID;                                                                 // Spara IMDB-ID f�r att kunna anv�nda det i formul�ret                                                                 // Anropa metoden f�r att filtrera recensioner baserat p� datum
                FilterValue = 6; // Standard: Senaste
                DateSortText = "Senaste";
                Reviews = Reviews.OrderByDescending(r => r.DateTime).ToList();
            }
            else
            {
                ViewData["ErrorMessage"] = "Filmens ID saknas eller �r ogiltigt.";
            }
        }

        public async Task<IActionResult> OnPostAddReview()
        {
            if (!ModelState.IsValid)
            {
                // H�mta filmdetaljer och recensioner igen om valideringen misslyckas
                (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(Review.ImdbID);
                return Page();
            }
                                                                                            // Spara filmens titel i recensionen
            Review.DateTime = DateTime.Now;                                                 // Spara aktuellt datum och tid
            await _reviewService.AddReviewAsync(Review);                                    // Anv�nd ReviewService f�r att l�gga till recensionen
            return RedirectToPage("MovieDetailsPage", new { imdbId = Review.ImdbID });      // Ladda om sidan och dess inneh�ll
        }

        public async Task<IActionResult> OnPostStarFilter()
        {
            UserContext = await _userService.GetUserContextAsync(User);
            (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(Review.ImdbID);
            ViewData["MowiTajmRating"] = MowiTajmRating;

            // Rensa listan med filtrerade recensioner
            FilteredReviews.Clear();

            // Filtrera recensionerna baserat p� stj�rnorna
            if (FilterValue >= 1 && FilterValue <= 5)
            {
                FilteredReviews = Reviews.Where(r => r.Rating == FilterValue).ToList();
                IsStarFilterActive = true;
            }
            else
            {
                IsStarFilterActive = false;
            }

            // S�tt standardv�rde f�r DateSortText och sortera recensionerna
            DateSortText = "Senaste";
            FilteredReviews = FilteredReviews.OrderByDescending(r => r.DateTime).ToList();


            // �terg� till sidan med uppdaterad information
            TempData["ScrollToReviews"] = true;
            return Page();
        }

        public async Task<IActionResult> OnPostDateFilter()
        {
            UserContext = await _userService.GetUserContextAsync(User);
            (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(Review.ImdbID);
            ViewData["MowiTajmRating"] = MowiTajmRating;

            // L�s in f�reg�ende FilterValue om det finns, annars s�tt ett defaultv�rde (6 = "Senaste")
            if (TempData["FilterValue"] is int prevFilterValue)
            {
                FilterValue = prevFilterValue;
            }
            else
            {
                FilterValue = 6;
            }

            // Toggla FilterValue: om den �r 6 (nyaste) blir den 7 (�ldsta), annars blir den 6
            FilterValue = (FilterValue == 6) ? 7 : 6;

            // S�tt DateSortText baserat p� den nya FilterValue
            DateSortText = (FilterValue == 6) ? "Senaste" : "�ldsta";

            if (FilterValue == 6)
            {
                Reviews = Reviews.OrderByDescending(r => r.DateTime).ToList();
            }
            else
            {
                Reviews = Reviews.OrderBy(r => r.DateTime).ToList();
            }

            // Spara v�rdena i TempData f�r n�sta anrop
            TempData["FilterValue"] = FilterValue;
            TempData["DateSortOrder"] = DateSortText;
            TempData["ScrollToReviews"] = true; // S�tt flaggan f�r scroll

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // Anv�nd ReviewService f�r att ta bort recensionen
            await _reviewService.DeleteReviewAsync(id);

            // Ladda om filmdata inklusive recensioner
            (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(Review.ImdbID);

            // Ladda om sidan och dess inneh�ll
            return RedirectToPage("MovieDetailsPage", new { imdbId = Review.ImdbID });
        }
    }
}
