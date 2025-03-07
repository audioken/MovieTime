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
        public double MowiTajmRating { get; set; } //Genomsnittlig review f�r filmen baserat p� reviews p� MovieTime
        public UserContext UserContext { get; set; }
        [BindProperty]
        public Review Review { get; set; } = new();

        // True om anv�ndaren �r inloggad, annars false
        public bool IsUserSignedIn => User.Identity.IsAuthenticated;

        //En INT vi anv�nder f�r att sortera reviews baserat p� hur m�nga stj�rnor den har
        [BindProperty]
        public int SearchReview { get; set; }


        public async Task OnGetAsync(string imdbID)
        {
            // Kontrollera att imdbID inte �r null eller tomt innan vi forts�tter
            if (!string.IsNullOrWhiteSpace(imdbID))
            {
                UserContext = await _userService.GetUserContextAsync(User);                             // H�mta anv�ndardata fr�n service               
                (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(imdbID);    // H�mta filmen, recensioner och genomsnittlig rating fr�n service
                ViewData["MowiTajmRating"] = MowiTajmRating;                                            // Spara MowiTajmRating i ViewData f�r att anv�ndas p� sidan
                Review.ImdbID = imdbID;                                                                 // Spara IMDB-ID f�r att kunna anv�nda det i formul�ret               
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

            // Spara aktuellt datum och tid
            Review.DateTime = DateTime.Now;

            // Anv�nd ReviewService f�r att l�gga till recensionen
            await _reviewService.AddReviewAsync(Review);

            // Ladda om sidan och dess inneh�ll
            return RedirectToPage("MovieDetailsPage", new { imdbId = Review.ImdbID });
        }

        public async Task<IActionResult> OnPostStarFilter()
        {
            (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(Review.ImdbID);

            // H�mta anv�ndarkontexten via IUserService
            UserContext = await _userService.GetUserContextAsync(User);

            // Filtrera recensionerna baserat p� stj�rnorna
            if (SearchReview >= 1 && SearchReview <= 5)
            {
                Reviews = Reviews.Where(r => r.Rating == SearchReview).ToList();
            }

            // Spara de filtrerade recensionerna
            ViewData["ReviewFilter"] = Reviews;
            ViewData["Movie"] = Movie;

            // �terg� till sidan med uppdaterad information
            TempData["ScrollToReviews"] = true;
            return Page();
        }

        public async Task<IActionResult> OnPostDateFilter()
        {
            (Movie, Reviews, MowiTajmRating) = await _movieService.GetMovieDetailsAsync(Review.ImdbID);

            UserContext = await _userService.GetUserContextAsync(User);

            // L�s in f�reg�ende SearchReview om det finns
            if (TempData["SearchReview"] is int prevSearchReview)
            {
                SearchReview = prevSearchReview;
            }

            // Om SearchReview �r mindre �n 6, s�tt den till 6
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

            // Spara SearchReview i TempData f�r att beh�lla v�rdet
            TempData["SearchReview"] = SearchReview;
            //TempData["MovieTimeReview"] = MovieTimeReview; // Spara v�rdet i TempData

            TempData["ScrollToReviews"] = true; // S�tt flaggan
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
