using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MowiTajm.Data;
using MowiTajm.Models;

namespace MowiTajm.Areas.Identity.Pages.User
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;
        private readonly ReviewService _reviewService;

        public IndexModel(IUserService userService, ApplicationDbContext context, ReviewService reviewService)
        {
            _userService = userService;
            _context = context;
            _reviewService = reviewService;
        }

        // Initierar Reviews med en tom lista f�r att eliminera risken f�r null-v�rde.
        // Detta s�kerst�ller att vi alltid kan arbeta med listan utan att beh�va g�ra null-kontroller,
        // vilket g�r koden mer robust och f�rhindrar NullReferenceException vid �tkomst.
        public List<Review> Reviews { get; set; } = new List<Review>(); // 

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userService.GetUserContextAsync(User);


            var displayName = user.DisplayName; //  Viktigt, inte UserName

            Reviews = await _context.Reviews
                .Where(r => r.Username == displayName) //  Matcha Username mot DisplayName
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteReviewAsync(int reviewId)
        {
            await _reviewService.DeleteReviewAsync(reviewId);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditReviewAsync(int reviewId)
        {
            // H�mta recensionen fr�n databasen med reviewId
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                return NotFound();
            }

            // Skicka vidare till EditReviewPage med den h�r recensionen
            return RedirectToPage("/Movies/EditReviewPage", new { id = review.Id });
        }
    }
}
   

