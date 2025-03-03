using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MowiTajm.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Redirecta användaren till huvudsidan för sökning på filmtitlar
            return RedirectToPage("/Movies/Index");
        }
    }
}
