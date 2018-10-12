using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyNPO.Models
{
    public class Report
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public Guid TransactionGuid { get; set; }
        public string Date { get; set; }  
        public string Time { get; set; }
        public string TimeZone { get; set; }
        public string Description { get; set; }
       public string CurrencyType { get; set; }
       public string Gross { get; set; }
       public string Fee { get; set; }
       public string Net { get; set; }
       public string TransactionID { get; set; }
       public string FromEmailAddress { get; set; }
       public string Name { get; set; }
       public string BankName { get; set; }
       public string BankAccount { get; set; }
       public string ShippingAmount { get; set; }
       public string SalesTax { get; set; }
       public string InvoiceID { get; set; }
       public string ReferenceTxnID { get; set; }
       public DateTime UploadDateTime { get; set; }
       public string PhoneNo { get; set; }
       public string Reason { get; set; }

    }
}