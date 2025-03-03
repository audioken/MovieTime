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
            // Spara söksträngen för att kunna använda den i vyn
            SearchInput = searchInput;

            // Om söksträngen inte är tom, sök efter filmer
            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                // Anropa API:et för att söka efter filmer och hämta resultatet
                var result = await _omdbService.SearchMoviesAsync(searchInput);
                Movies = result.Search;

                // Spara totala antalet resultat för att kunna visa det i vyn
                TotalResults = result.TotalResults;
            }
        }
    }
}
