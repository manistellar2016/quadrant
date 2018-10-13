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


    public class Login
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class ReportUserInfo
    {
        [Required]
        public string FromDate { get; set; }
        [Required]    
        public string ToDate { get; set; }
        [Required]
        public int TypeOfReport { get; set; }
        public List<Report> ReportInfo { get; set; }
    }
}