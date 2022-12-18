using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models
{
    public class Group_Member
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }

        [ForeignKey("Group")]
        public int? GroupId { get; set; }

        public string Role { get; set; }      //Admin or Member

        public virtual ApplicationUser? ApplicationUser { get; set; }

        public virtual Group? Group { get; set; }
    }
}
