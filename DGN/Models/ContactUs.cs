using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DGN.Models
{
    public class ContactUs
    {
        public int Id { get; set; }

        [DisplayName("Branch Name")]
        public string BranchName { get; set; }

        [DisplayName("Phone Number")]
        public int PhoneNumber { get; set; }

        [DisplayName("Branch's Activity Time")]
        public string ActivityTime { get; set; }

        public double LocationLatitude { get; set; }
        public double LocationLongitude { get; set; }
    }
}
