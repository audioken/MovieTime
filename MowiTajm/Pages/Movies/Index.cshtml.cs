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
        public string TotalResults { get; set; }
        public string SearchInput { get; set; }

        public async Task OnGetAsync(string searchInput)
        {
            // Spara s�kstr�ngen f�r att kunna anv�nda den i vyn
            SearchInput = searchInput;

            // Om s�kstr�ngen inte �r tom, s�k efter filmer
            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                // Anropa API:et f�r att s�ka efter filmer och h�mta resultatet
                var result = await _omdbService.SearchMoviesAsync(searchInput);
                Movies = result.Search;

                // Spara totala antalet resultat f�r att kunna visa det i vyn
                TotalResults = result.TotalResults;
            }
        }
    }
}
