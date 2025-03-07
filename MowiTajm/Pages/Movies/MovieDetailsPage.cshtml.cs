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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public MovieDetailsPageModel(OmdbService omdbService, ApplicationDbContext database, SignInManager<ApplicationUser> sign�nManager, UserManager<ApplicationUser> userManager)
        {
            _omdbService = omdbService;
            _database = database;
            _signInManager = sign�nManager;
            _userManager = userManager;
        }

        // True om anv�ndaren �r inloggad, annars false
        public bool IsUserSignedIn => _signInManager.IsSignedIn(User);
        public bool IsAdmin { get; set; }
        public string DisplayName { get; set; }

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

                // H�mta anv�ndarens DisplayName
                var user = await _userManager.GetUserAsync(User);
                IsAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");
                DisplayName = user?.DisplayName;

                // Spara IMDB-ID f�r att kunna anv�nda det i formul�ret
                Review.ImdbID = imdbID;
            }

            // Ber�kna MovieTimeReview baserat p� alla recensioner (inte filtrerade recensioner)
            MovieTimeReview = Reviews.Count > 0 ? Math.Round(Reviews.Average(r => r.Rating), 2) : 0;

            // Spara MovieTimeReview i ViewData f�r att anv�ndas p� sidan
            ViewData["MovieTimeReview"] = MovieTimeReview;
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

        public async Task<IActionResult> OnPostStarFilter()
        {
            Movie = await _omdbService.GetMovieByIdAsync(Review.ImdbID);
            Reviews = await _database.Reviews.Where(r => r.ImdbID == Review.ImdbID).ToListAsync();

            var user = await _userManager.GetUserAsync(User); // H�mta anv�ndare
            IsAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");
            DisplayName = user?.DisplayName; // Fyll i DisplayName

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
            Movie = await _omdbService.GetMovieByIdAsync(Review.ImdbID);
            Reviews = await _database.Reviews.Where(r => r.ImdbID == Review.ImdbID).ToListAsync();

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
    }
}
