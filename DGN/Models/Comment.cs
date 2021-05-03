using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGN.Models
{
    public class Comment
    {
        // Primery key
        public int Id { get; set; }

        [Required]
        [StringLenght(20), MinimumLenght = 5]
        public string Title { get; set; }

        [StringLenght(200), MinimumLenght = 5]
        public string Body { get; set; }

        // Shouldnt display in the GET form
        // Foreign key
        public User CommentWriter { get; set; }

        // Shouldnt display in the GET form
        // Foreign Key
        public Article RelatedArticle { get; set; }

    }
}