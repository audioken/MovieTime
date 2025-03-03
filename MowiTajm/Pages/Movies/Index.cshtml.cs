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

        public List<MovieLite> Movies { get; set; } = new List<MovieLite>();

        public async Task OnGetAsync(string searchInput)
        {
            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                var result = await _omdbService.SearchMoviesAsync(searchInput);
                Movies = result.Movies;
            }
        }
    }
}
