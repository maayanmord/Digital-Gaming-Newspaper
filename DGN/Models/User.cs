using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DGN.Models
{
    public class User
    {
        // Primery key
        public int Id { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DisplayName("Username")]
        public string UserName { get; set; }

        [ForeignKey("Password")]
        public int PasswordId { get; set; }

        [Required]
        public Password Password { get; set; }

        [DisplayName("The Profile Image Location")]
        public string ImageLocation { get; set; }

        [DisplayName("Basic info about the user")]
        [DataType(DataType.MultilineText)]
        public string About { get; set; }

        // Shouldn't display in the form.
        public IList<Article> ArticleLikes { get; set; }

        // this is one to many with articles the user wrote
        // Shouldn't display in the form.
        public IList<Article> Articles { get; set; }
    }
}