using Microsoft.AspNetCore.Identity;

namespace MowiTajm.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty; // Initerar med en tom sträng för att undvika null-värden
    }
}
