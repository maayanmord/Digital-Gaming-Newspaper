using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DGN.Models
{
    public class Branch
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Branch Name")]
        public string BranchName { get; set; }

        [DisplayName("Phone Number")]
        public int PhoneNumber { get; set; }

        [DisplayName("Branch's Activity Time")]
        public string ActivityTime { get; set; }

        [Required]
        public double LocationLatitude { get; set; }

        [Required]
        public double LocationLongitude { get; set; }
    }
}
