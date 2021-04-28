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

        public string Title { get; set; }

        public string Body { get; set; }

        // Foreign key
        public User CommentWriter { get; set; }

        // Foreign Key
        public Article RelatedArticle { get; set; }

    }
}