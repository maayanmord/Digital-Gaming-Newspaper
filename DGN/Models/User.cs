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

        [Required]
        public Password Password { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z][a-z]+$", ErrorMessage = "A name must begin with a capital letter.")]
        public string Firstname { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z][a-z]+$", ErrorMessage = "A name must begin with a capital letter.")]
        public string Lastname { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }

        public UserRole Role { get; set; } = UserRole.Client;

        [DisplayName("The Profile Image Location")]
        public string ImageLocation { get; set; }

        [DisplayName("Basic info about the user")]
        [DataType(DataType.MultilineText)]
        public string About { get; set; }

        public string FullName
        {
            get
            {
                return Firstname + " " + Lastname;
            }
        }

        // this is one to many with articles the user wrote
        // Shouldn't display in the form.
        [ForeignKey("AuthorId")]
        public IList<Article> Articles { get; set; }

        // Shouldn't display in the form.
        public IList<Article> ArticleLikes { get; set; }


    }
}