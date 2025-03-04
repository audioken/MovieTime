using MowiTajm.Models;
using Newtonsoft.Json;

namespace MowiTajm.Services
{
    public class OmdbService
    {
        private readonly HttpClient _httpClient;

        public OmdbService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SearchResult> SearchMoviesAsync(string searchInput)
        {
            // Anropar OMDB API för att söka efter filmer baserat på angiven söktext
            var response = await _httpClient.GetAsync($"http://www.omdbapi.com/?s={searchInput}&apikey=2e1cb575");

            // Säkerställer att HTTP-anropet lyckades (kastar undantag om statuskod inte är 2xx)
            response.EnsureSuccessStatusCode();

            // Läser och lagrar svaret som en JSON-sträng
            var content = await response.Content.ReadAsStringAsync();

            // Deserialiserar JSON-strängen till ett C#-objekt av typen SearchResult
            var result = JsonConvert.DeserializeObject<SearchResult>(content);

            // Returnerar det deserialiserade resultatet
            return result;
        }

        public async Task<MovieFull> GetMovieByIdAsync(string imdbID)
        {
            // Anropar OMDB API för att hämta information om en specifik film baserat på IMDB-ID
            var response = await _httpClient.GetAsync($"http://www.omdbapi.com/?i={imdbID}&apikey=2e1cb575");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<MovieFull>(content);

            return result;
        }
    }
}
