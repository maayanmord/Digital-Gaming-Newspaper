using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DGN.Models
{
    public class ContactUs
    {
        public int Id { get; set; }

        [DisplayName("Branch name")]
        public string BranchName { get; set; }

        [DisplayName("Address")]
        public Location BranchLocation { get; set; }

        public int PhoneNumber { get; set; }

        [DisplayName("Branch's Activity Time")]
        public string ActivityTime { get; set; }
    }
}
