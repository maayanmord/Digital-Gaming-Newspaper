using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DGN.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(50, MinimumLength = 5)]
        [RegularExpression(@"^[A-Z\d].*$", ErrorMessage = "The title must begin with a capital letter or a number")]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        [DisplayName("The article image location")]
        public string ImageLocation { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [Required]
        public Category Category { get; set; }

        [ForeignKey("Author")]
        public int? AuthorId { get; set; }

        public User Author { get; set; }

        [Required]
        [DisplayName("Creation Timestamp")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreationTimestamp { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("Last Updated Timestamp")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LastUpdatedTimestamp { get; set; } = DateTime.Now;

        public IList<Comment> Comments { get; set; } = new List<Comment>();

        [InverseProperty("ArticleLikes")]
        public IList<User> Likes { get; set; } = new List<User>();
    }
}