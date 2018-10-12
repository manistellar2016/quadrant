using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyNPO.Models
{
    public class Donation
    {
        [Required]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required]
        public string DonationAmount { get; set; }
        [Required]
        public string Reason { get; set; }
    }
}