using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DGN.Models
{
    public enum UserRole
    {
        Client, 
        Author,
        Admin
    }

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        public Password Password { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z][a-zA-Z]{1,}[a-zA-Z ]*$", ErrorMessage = "A name must begin with a capital letter and must have at least 2 letters on first word.")]
        public string Firstname { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z][a-zA-Z]{1,}[a-zA-Z ]*$", ErrorMessage = "A name must begin with a capital letter and must have at least 2 letters on first word.")]
        public string Lastname { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime Birthday { get; set; }

        public UserRole Role { get; set; } = UserRole.Client;

        [DisplayName("Image Location")]
        public string ImageLocation { get; set; }

        [DisplayName("About")]
        [DataType(DataType.MultilineText)]
        public string About { get; set; }

        [DisplayName("Full Name")]
        public string FullName
        {
            get
            {
                return Firstname + " " + Lastname;
            }
        }

        [InverseProperty("User")]
        public IList<Article> Articles { get; set; }

        [InverseProperty("UserLikes")]
        [DisplayName("Article Likes")]
        public IList<Article> ArticleLikes { get; set; }

        public IList<Comment> Comments { get; set; }
    }
}