using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DGN.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        public int? UserId { get; set; }

        public User User { get; set; }

        [Required]
        [ForeignKey("RelatedArticle")]
        public int RelatedArticleId { get; set; }

        public Article RelatedArticle { get; set; }
    }
}