﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models
{
    public class Post
    {
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? CreatorId { get; set; }

        [Required(ErrorMessage = "Your post body can't be empty")]
        [StringLength(300, ErrorMessage = "Your post should be shorter than 300 characters")]
        public string Message { get; set; }

        public DateTime DatePosted { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}