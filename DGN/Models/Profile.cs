
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DGN.Models
{
    // This is the User Profile Model
    // In here there is everything about the user that is displayed in the profile
    public class Profile
    {

        [Key]
        public int Id { get; set; }

        [Index(IsUnique = true)]
        [ForeignKey("User")]
        public int UserId { get; set; }

        // we need this ??
        // [Required(ErrorMessage = "User is required")] 
        public User User { get; set; }

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