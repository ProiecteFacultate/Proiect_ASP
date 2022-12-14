using Microsoft.AspNetCore.Identity;

namespace Proiect.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual Profile? UserProfile { get; set; }

        public virtual ICollection<Friendship>? FriendShips { get; set; }

        public virtual ICollection<Post>? Posts { get; set; }

        public virtual ICollection<Post_Comment>? Post_Comments { get; set; }
    }
}
