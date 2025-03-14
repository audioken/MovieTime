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

        // Metod för att söka efter filmer baserat på angiven söktext
        public async Task<SearchResult> SearchMoviesAsync(string searchInput)
        {
            try
            {
                // Anropar OMDB API för att söka efter filmer baserat på angiven söktext
                var response = await _httpClient.GetAsync($"http://www.omdbapi.com/?s={searchInput}&apikey=2e1cb575");

                response.EnsureSuccessStatusCode(); // Säkerställer att HTTP-anropet lyckades

                // Läser och lagrar svaret som en JSON-sträng
                var content = await response.Content.ReadAsStringAsync();

                // Deserialiserar JSON-strängen till ett C#-objekt av typen SearchResult
                var result = JsonConvert.DeserializeObject<SearchResult>(content);

                // Kasta ett InvalidOperationException om deserialiseringen misslyckades
                if (result is null)
                {
                    throw new InvalidOperationException("Kunde inte deserialisera svaret till ett SearchResult-objekt.");
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Nätverksfel vid anrop till OMDb API: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Fel vid deserialisering av JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett oväntat fel uppstod i SearchMoviesAsync: {ex.Message}");
            }

            // Returnerar en tom SearchResult om ett fel uppstår
            return new SearchResult();
        }

        // Metod för att hämta information om en specifik film baserat på IMDB-ID
        public async Task<MovieFull> GetMovieByIdAsync(string imdbID)
        {
            try
            {
                // Anropar OMDB API för att hämta information om en specifik film baserat på IMDB-ID
                var response = await _httpClient.GetAsync($"http://www.omdbapi.com/?i={imdbID}&apikey=2e1cb575");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<MovieFull>(content);

                if (result is null)
                {
                    throw new InvalidOperationException("Kunde inte deserialisera svaret till ett SearchResult-objekt.");
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Nätverksfel vid anrop till OMDb API: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Fel vid deserialisering av JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett oväntat fel uppstod i GetMovieByIdAsync: {ex.Message}");
            }

            // Returnerar en tom MovieFull om ett fel uppstår
            return new MovieFull();
        }
    }
}
