using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DGN.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [RegularExpression(@"^[A-Z\d].*$", ErrorMessage = "The title must begin with a capital letter or a number")]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public DateTime CreationTimestamp { get; set; }

        [Required]
        public DateTime LastUpdatedTimestamp { get; set; }

        public IList<Comment> Comments { get; set; }

        public IList<Profile> Likes { get; set; }
    }
}
