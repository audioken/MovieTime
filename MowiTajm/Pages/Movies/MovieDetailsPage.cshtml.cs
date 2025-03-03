using Microsoft.AspNetCore.Mvc.RazorPages;
using MowiTajm.Models;
using MowiTajm.Services;

namespace MowiTajm.Pages.Movies
{
    public class MovieDetailsPageModel : PageModel
    {
        private readonly OmdbService _omdbService;

        public MovieDetailsPageModel(OmdbService omdbService)
        {
            _omdbService = omdbService;
        }

        public MovieFull Movie { get; set; } = new();

        public async Task OnGetAsync(string imdbId)
        {
            Movie = await _omdbService.GetMovieByIdAsync(imdbId);
        }
    }
}
