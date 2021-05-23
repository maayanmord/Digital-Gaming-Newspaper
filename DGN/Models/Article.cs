using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string Body { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [Required]
        public Category Category { get; set; }

        [Required]
        [ForeignKey("Profile")]
        public int AuthorId { get; set; }

        [Required]
        public Profile Author { get; set; }

        [Required]
        [DisplayName("Creation Timestamp")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreationTimestamp { get; set; }

        [Required]
        [DisplayName("Last Updated Timestamp")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LastUpdatedTimestamp { get; set; }
        
        public IList<Comment> Comments { get; set; }

        // Check how to implement many-to-many relationship 
        public IList<User> Likes { get; set; }
    }
}