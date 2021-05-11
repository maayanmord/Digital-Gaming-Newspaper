using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

        public int PssswordID { get; set; }

        [Required]
        [RegularExpretion(@"^[A-Z]+[a-z].*$")]
        public string Password { get; set; }

        [DisplayName("The Profile Image Location")]
        public string ImageLocation { get; set; }

        [DisplayName("Basic info about the user")]
        [DataType(DataType.MultilineText)]
        public string About { get; set; }

        [DisplayName("The articles that the user liked")]
        public IList<ArticleLikes> ArticleLikes { get; set; }

        // this is one to many with articles the user wrote
        [DisplayName("Articles The User Wrote")]
        public IList<Article> Articles { get; set; }
    }
}