using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Post_Comment
    {
        public int Id { get; set; }

        [ForeignKey("Post")]
        public int? PostId { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? CreatorId { get; set; }

        [Required(ErrorMessage = "Your post body can't be empty")]
        [StringLength(300, ErrorMessage = "Your comment should be shorter than 300 characters")]
        public string Message { get; set; }

        public DateTime DatePosted { get; set; }

        public virtual Post? Post { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
