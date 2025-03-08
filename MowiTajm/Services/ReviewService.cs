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

    public async Task AddReviewAsync(Review review)
    {
        _database.Reviews.Add(review);
        await _database.SaveChangesAsync();
    }

    public async Task DeleteReviewAsync(int reviewId)
    {
        var review = await _database.Reviews.FindAsync(reviewId);
        if (review != null)
        {
            _database.Reviews.Remove(review);
            await _database.SaveChangesAsync();
        }
    }

    public async Task<List<Review>> GetReviewsByImdbIdAsync(string imdbId)
    {
        return await _database.Reviews.Where(r => r.ImdbID == imdbId).ToListAsync();
    }
}
