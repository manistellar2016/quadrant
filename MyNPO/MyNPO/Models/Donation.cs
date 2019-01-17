using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [RegularExpression("[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}")]
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

    public class FamilyReportInfo
    {
            [Required]
            public string FromDate { get; set; }
            [Required]
            public string ToDate { get; set; }
            [Required]
            public int TypeOfReport { get; set; }
            public List<FamilyInfo> ReportInfo { get; set; }
        
    }

    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Details { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UploadFileName { get; set; }
    }

    public class AdminUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}