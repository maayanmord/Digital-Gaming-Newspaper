using Microsoft.EntityFrameworkCore;
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
        [StringLength(50, MinimumLength = 5)]
        [RegularExpression(@"^[A-Z\d].*$", ErrorMessage = "The title must begin with a capital letter or a number")]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        [DisplayName("Image name")]
        public string ImageLocation { get; set; }

        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public int? UserId { get; set; }

        public User User { get; set; }

        [DisplayName("Creation Timestamp")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreationTimestamp { get; set; } = DateTime.Now;

        [DisplayName("Last Updated Timestamp")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LastUpdatedTimestamp { get; set; } = DateTime.Now;

        public IList<Comment> Comments { get; set; } = new List<Comment>();

        public IList<User> UserLikes { get; set; } = new List<User>();
    }
}