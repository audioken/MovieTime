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

        public async Task OnGetAsync(string imdbId)
        {
            if (!string.IsNullOrWhiteSpace(imdbId))
            {
                Movie = await _omdbService.GetMovieByIdAsync(imdbId);
                Reviews = await _database.Reviews.Where(r => r.ImdbID == imdbId).ToListAsync();
                Review.ImdbID = imdbId;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Hämta om filmen och recensionerna om det blev valideringsfel
                Movie = await _omdbService.GetMovieByIdAsync(Review.ImdbID);
                Reviews = await _database.Reviews.Where(r => r.ImdbID == Review.ImdbID).ToListAsync();
                return Page();
            }

            Review.DateTime = DateTime.Now;

            _database.Reviews.Add(Review);
            await _database.SaveChangesAsync();

            return RedirectToPage("MovieDetailsPage", new { imdbId = Review.ImdbID });
        }
    }
}
