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
            // Om söksträngen inte är tom, sök efter filmer
            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                SearchInput = searchInput;                                      // Spara söksträngen för att kunna använda den i vyn
                var result = await _omdbService.SearchMoviesAsync(searchInput); // Anropa API:et för att söka efter filmer och hämta resultatet              
                Movies = result.Search ?? new List<MovieLite>();                // Hämta filmer, men använd en tom lista om inga resultat hittades              
                TotalResults = result.TotalResults;                             // Spara totala antalet resultat för att kunna visa det i vyn
            }
        }
    }
}
