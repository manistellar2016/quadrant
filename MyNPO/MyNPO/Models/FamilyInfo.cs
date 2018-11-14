using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MyNPO.Models
{   
    public class FamilyInfo
    {
        [Key]       
        public Guid PrimaryId { get; set; }
        
        public string FirstName { get; set; }      
        
        public string LastName { get; set; }
        [DisplayFormat(ApplyFormatInEditMode=true ,DataFormatString = "{0:MM/dd/yyyy}")]

        public DateTime DateOfBirth { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Address { get; set; }
        public string MaritalStatus { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? MarriageDate { get; set; }
        public int NoOfDependents { get; set; }
        public bool IsVolunteer { get; set; }
        public List<DependentInfo> DependentDetails { get; set; }
        public DateTime CreateDate { get; set; }
    }
}