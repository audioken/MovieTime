using Microsoft.AspNetCore.Mvc.RazorPages;
using MowiTajm.Models;
using MowiTajm.Services;

namespace MowiTajm.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly OmdbService _omdbService;

        public IndexModel(OmdbService omdbService)
        {
            _omdbService = omdbService;
        }

        public List<MovieLite> Movies { get; set; } = new();
        public string TotalResults { get; set; } = "";
        public string SearchInput { get; set; } = "";

        public async Task OnGetAsync(string searchInput)
        {
            // Om s�kstr�ngen inte �r tom, s�k efter filmer
            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                SearchInput = searchInput;                                      // Spara s�kstr�ngen f�r att kunna anv�nda den i vyn
                var result = await _omdbService.SearchMoviesAsync(searchInput); // Anropa API:et f�r att s�ka efter filmer och h�mta resultatet              
                Movies = result.Search ?? new List<MovieLite>();                // H�mta filmlista att visa i s�kresultat. Skapa tom lista vid null           
                TotalResults = result.TotalResults ?? "0";                      // F�r att kunna visa i vyn. Undvik null genom att anv�nda "0" som fallback
            }
        }
    }
}
