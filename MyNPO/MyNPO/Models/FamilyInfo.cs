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
    }

        [XmlRoot(ElementName = "authRequest")]
        public class AuthRequest
        {
            [XmlElement(ElementName = "user")]
            public string User { get; set; }
            [XmlElement(ElementName = "vendor")]
            public string Vendor { get; set; }
            [XmlElement(ElementName = "partner")]
            public string Partner { get; set; }
            [XmlElement(ElementName = "password")]
            public string Password { get; set; }
        }

        [XmlRoot(ElementName = "reportParam")]
        public class ReportParam
        {
            [XmlElement(ElementName = "paramName")]
            public string ParamName { get; set; }
            [XmlElement(ElementName = "paramValue")]
            public string ParamValue { get; set; }
        }

        [XmlRoot(ElementName = "runReportRequest")]
        public class RunReportRequest
        {
            [XmlElement(ElementName = "reportName")]
            public string ReportName { get; set; }
            [XmlElement(ElementName = "reportParam")]
            public ReportParam ReportParam { get; set; }
            [XmlElement(ElementName = "pageSize")]
            public string PageSize { get; set; }
        }

        [XmlRoot(ElementName = "reportingEngineRequest")]
        public class ReportingEngineRequest
        {
            [XmlElement(ElementName = "authRequest")]
            public AuthRequest AuthRequest { get; set; }
            [XmlElement(ElementName = "runReportRequest")]
            public RunReportRequest RunReportRequest { get; set; }
        }


    //Response

    [XmlRoot(ElementName = "baseResponse")]
    public class BaseResponse
    {
        [XmlElement(ElementName = "responseCode")]
        public string ResponseCode { get; set; }
        [XmlElement(ElementName = "responseMsg")]
        public string ResponseMsg { get; set; }
    }

    [XmlRoot(ElementName = "runReportResponse")]
    public class RunReportResponse
    {
        [XmlElement(ElementName = "reportId")]
        public string ReportId { get; set; }
        [XmlElement(ElementName = "statusCode")]
        public string StatusCode { get; set; }
        [XmlElement(ElementName = "statusMsg")]
        public string StatusMsg { get; set; }
    }

    [XmlRoot(ElementName = "reportingEngineResponse")]
    public class ReportingEngineResponse
    {
        [XmlElement(ElementName = "baseResponse")]
        public BaseResponse BaseResponse { get; set; }
        [XmlElement(ElementName = "runReportResponse")]
        public RunReportResponse RunReportResponse { get; set; }
    }

}