using System;
using System.ComponentModel;
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

        [DisplayName("User")]
        public int? UserId { get; set; }

        public User User { get; set; }

        [DisplayName("Creation Timestamp")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime CreationTimestamp { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("Related Article")]
        [ForeignKey("RelatedArticle")]
        public int RelatedArticleId { get; set; }

        [DisplayName("Related Article")]
        public Article RelatedArticle { get; set; }
    }
}