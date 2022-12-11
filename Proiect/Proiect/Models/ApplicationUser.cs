using Microsoft.AspNetCore.Identity;

namespace Proiect.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? ProfileUsername { get; set; }
        public string? Description { get; set; }
    }
}
