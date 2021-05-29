using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DGN.Models
{
    public class Comment
    {
        // Primery key
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        // Shouldnt display in the GET form
        [ForeignKey("Author")]
        public int? AuthorId { get; set; }

        public User Author { get; set; }

        // Shouldnt display in the GET form
        [Required]
        [ForeignKey("RelatedArticle")]
        public int RelatedArticleId { get; set; }

        [Required]
        public Article RelatedArticle { get; set; }
    }
}