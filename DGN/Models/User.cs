using System;
using System.Collections.Generic;
using System.Linq;
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
        [Display (Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [RegularExpretion("^[A-Z]+[a-z]*$")]
        public string Password { get; set; }
    }
}