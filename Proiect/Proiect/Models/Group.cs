using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Group name can't be empty")]
        [StringLength(20, ErrorMessage = "Group name should be shorter than 20 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Group description can't be empty")]
        [StringLength(300, ErrorMessage = "Group description should be shorter than 300 characters")]
        public string Description { get; set; }

        public string? GroupImage { get; set; }

        public virtual ICollection<Group_Member>? Group_Members { get; set; }

        public virtual ICollection<Group_Message>? Group_Messages { get; set; }
    }
}
