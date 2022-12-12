using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models
{
    public class Profile
    {
        [Key]
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Username is mandatory")]
        [StringLength(20, ErrorMessage = "The username should be shorter than 100 characters")]
        [MinLength(3, ErrorMessage = "The username should be longer than 3 characters")]
        public string? ProfileUsername { get; set; }
        public string? Description { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
