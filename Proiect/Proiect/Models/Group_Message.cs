using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Group_Message
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Group")]
        public int? GroupId { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? CreatorId { get; set; }

        [Required(ErrorMessage = "Your message can't be empty")]
        [StringLength(300, ErrorMessage = "Your messageshould be shorter than 300 characters")]
        public string Message { get; set; }

        public DateTime DatePosted { get; set; }

        public string? Status { get; set; }

        public virtual Group? Group { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
