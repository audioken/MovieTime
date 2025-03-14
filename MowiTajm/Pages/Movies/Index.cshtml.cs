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
        public bool IsUserSignedIn => User.Identity?.IsAuthenticated ?? false;

        ////////////////////////////////////////////////////////////////////////////////////////////

        // Metod som hämtar filmer från OMDb baserat på söksträng
        public async Task OnGetAsync(string searchInput)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(searchInput))
                {
                    // Spara söksträngen för att kunna använda den i vyn
                    SearchInput = searchInput;

                    // Anropa API:et för att söka efter filmer och hämta resultatet
                    var result = await _omdbService.SearchMoviesAsync(searchInput);

                    // Hämta filmlista att visa i sökresultat. Skapa tom lista vid null
                    Movies = result.Search ?? new List<MovieLite>();

                    // För att kunna visa i vyn. Undvik null genom att använda "0" som fallback
                    TotalResults = result.TotalResults ?? "0";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid hämtning av filmer från OMDb: {ex.Message}");
                ViewData["ErrorMessage"] = "Ett fel uppstod vid hämtning av filmer. Försök igen senare.";
            }
        }
    }
}
