using Microsoft.EntityFrameworkCore;
using MowiTajm.Data;
using MowiTajm.Models;

public class ReviewService
{
    private readonly ApplicationDbContext _database;

    public ReviewService(ApplicationDbContext database)
    {
        _database = database;
    }

    // Metod för att lägga till en recension i databasen
    public async Task AddReviewAsync(Review review)
    {
        try
        {
            _database.Reviews.Add(review);
            await _database.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Databasfel vid tillägg av recension: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ett oväntat fel uppstod i AddReviewAsync: {ex.Message}");
        }
    }

    // Metod för att ta bort en recension från databasen
    public async Task DeleteReviewAsync(int reviewId)
    {
        try
        {
            var review = await _database.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                _database.Reviews.Remove(review);
                await _database.SaveChangesAsync();
            }
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Databasfel vid borttagning av recension: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ett oväntat fel uppstod i DeleteReviewAsync: {ex.Message}");
        }
    }

    // Metod för att hämta alla recensioner som tillhör en viss film
    public async Task<List<Review>> GetReviewsByImdbIdAsync(string imdbId)
    {
        try
        {
            // Returnera en lista med alla recensioner som har samma imdbId som den angivna
            return await _database.Reviews.Where(r => r.ImdbID == imdbId).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ett oväntat fel uppstod i GetReviewsByImdbIdAsync: {ex.Message}");
            return new List<Review>();
        }
    }
}
