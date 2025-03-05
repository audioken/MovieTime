using Microsoft.AspNetCore.Identity;

namespace MowiTajm.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
    }
}
