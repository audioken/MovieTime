using Microsoft.EntityFrameworkCore;
using MowiTajm.Data;
using MowiTajm.Models;
using MowiTajm.Services;

public class MovieService
{
    private readonly OmdbService _omdbService;
    private readonly ApplicationDbContext _database;

    public MovieService(OmdbService omdbService, ApplicationDbContext database)
    {
        _omdbService = omdbService;
        _database = database;
    }

    public async Task<(MovieFull, List<Review>, double)> GetMovieDetailsAsync(string imdbID)
    {
        var movie = await _omdbService.GetMovieByIdAsync(imdbID);
        var reviews = await _database.Reviews.Where(r => r.ImdbID == imdbID).ToListAsync();
        var averageRating = reviews.Count > 0 ? Math.Round(reviews.Average(r => r.Rating), 2) : 0;

        return (movie, reviews, averageRating);
    }
}
