using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models
{
    public class Friendship
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Friend_1")]
        public string? Friend_1_Key { get; set; }

        [ForeignKey("Friend_2")]
        public string? Friend_2_Key { get; set; }
    }
}
