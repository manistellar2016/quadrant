using LumenWorks.Framework.IO.Csv;
using MyNPO.DataAccess;
using MyNPO.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MyNPO.Controllers
{  
    public class HomeController : Controller
    {
        public ActionResult Index()
        {          
            return View();
        }

        public ActionResult Donation()
        {
            ViewBag.Message = "Sai Baba Donation";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Sai Baba";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Information";

            return View();
        }

        public ActionResult Event()
        {
            return View();
        }

        public ActionResult PostPage()
        {
            return View();
        }

        public ActionResult Upload()
        {
            //Mail Send Test Code
            //new MailSender().SendMailWithInvite();

            return View();
        }

        private List<SelectListItem> GetTypeOfReport()
        {
            var report = new List<SelectListItem>();
            report.Add(new SelectListItem() { Text = "-Select-", Value = "0" });
            report.Add(new SelectListItem() { Text = "USER-REPORT", Value = "1" });
            report.Add(new SelectListItem() { Text = "EBAY-REPORT", Value = "2" });
            report.Add(new SelectListItem() { Text = "KINDBASE-REPORT", Value = "3" });
            return report;
        }
        public ActionResult Report()
        {
           
            var reportInfo = new ReportUserInfo();
            reportInfo.ReportInfo = new List<Report>();
            ViewBag.Reports = GetTypeOfReport();
            return View(reportInfo);
        }

        [HttpPost]
        public ActionResult Report(ReportUserInfo reportUserInfo)
        {
            var entityContext = new EntityContext();
            var fromDate =Convert.ToDateTime(reportUserInfo.FromDate);
            var toDate = Convert.ToDateTime(reportUserInfo.ToDate);
            var reports = new List<Report>();

            if(reportUserInfo.TypeOfReport == 1)
                reports = entityContext.reportInfo.Where(q => q.Date > fromDate && q.Date < toDate && q.TypeOfReport ==Constants.SystemDonation).ToList();
            else if(reportUserInfo.TypeOfReport == 3)
                reports = entityContext.reportInfo.Where(q => q.Date > fromDate && q.Date < toDate && q.TypeOfReport == Constants.KindBase).ToList();
            else 
                reports= entityContext.reportInfo.Where(q => q.Date > fromDate && q.Date < toDate && q.TypeOfReport == Constants.Ebay).ToList();

            reportUserInfo.ReportInfo = reports;
            ViewBag.Reports = GetTypeOfReport();
            return View(reportUserInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase upload, FormCollection test)
        {
            // Date Time    Time Zone   Description Currency    Gross Fee Net Balance Transaction ID  From Email Address Name    Bank Name   Bank Account    Shipping and Handling Amount    Sales Tax   Invoice ID  Reference Txn ID

            string report = string.Empty;
            var lReport = new List<Report>();
            int fileUploadType =Convert.ToInt16(test["selectReport"]);
            if (fileUploadType == 0)
                return View();

            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    if (upload.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    {
                        Stream stream = upload.InputStream;
                        DataTable csvTable = new DataTable();
                        using (CsvReader csvReader =
                            new CsvReader(new StreamReader(stream), true))
                        {
                            csvTable.Load(csvReader);
                        }
                        if (fileUploadType == 1)
                        {
                            lReport = ConvertCsvTableToEbayReport(csvTable);
                        }
                        else
                        {
                            lReport = ConvertCsvTableToKindBaseReport(csvTable);
                        }
                        using (EntityContext entityContext = new EntityContext())
                        {
                            entityContext.reportInfo.AddRange(lReport);
                            entityContext.SaveChanges();
                        }
                        report = $"ReportStatus -- TotalCount:{csvTable.Rows.Count}; UploadedCount:{lReport.Count}; ExistingCount:{csvTable.Rows.Count - lReport.Count}";
                        //return View(csvTable);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            ViewBag.ReportStatus = report;
            return View();
        }

        private List<Report> ConvertCsvTableToKindBaseReport(DataTable csvTable)
        {
            EntityContext entityContext = new EntityContext();

            var lReport = new List<Report>();
            foreach (DataRow dataRow in csvTable.Rows)
            {
                var tid = dataRow[4].ToString();
                if (entityContext.reportInfo.Any(q => q.TransactionID == tid))
                    continue;

                DateTime dt = Convert.ToDateTime(dataRow[1].ToString());

                var report = new Report();
                report.FromEmailAddress = dataRow[0].ToString();
                report.PhoneNo = dataRow[3].ToString();
                report.TransactionID = dataRow[4].ToString();
                report.Net = dataRow[10].ToString();
                report.Name = dataRow[11].ToString();
                report.Description = Constants.KindBase;
                report.TypeOfReport = Constants.KindBase;
                report.Date = dt;    
                report.Time = dt.ToString(Constants.HourFormat);
                report.TransactionGuid = Guid.NewGuid();
                report.UploadDateTime = DateTime.Now;               
                lReport.Add(report);
            }
            return lReport;
        }
        private List<Report> ConvertCsvTableToEbayReport(DataTable csvTable)
        {
            EntityContext entityContext = new EntityContext();

            var lReport = new List<Report>();
            foreach (DataRow dataRow in csvTable.Rows)
            {
                var tid = dataRow[9].ToString();
                if (entityContext.reportInfo.Any(q => q.TransactionID == tid))
                     continue;               
                    

                var report = new Report();
                report.Date =Convert.ToDateTime(dataRow[0].ToString());
                report.Time = dataRow[1].ToString();
                report.TimeZone = dataRow[2].ToString();
                report.Description = dataRow[3].ToString();
                report.CurrencyType = dataRow[4].ToString();
                report.Gross = dataRow[5].ToString();
                report.Fee = dataRow[6].ToString();
                report.Net = dataRow[7].ToString();
                report.TransactionID = dataRow[9].ToString();
                report.FromEmailAddress = dataRow[10].ToString();
                report.Name = dataRow[11].ToString();
                report.BankName = dataRow[12].ToString();
                report.BankAccount = dataRow[13].ToString();
                report.ShippingAmount = dataRow[14].ToString();
                report.SalesTax = dataRow[15].ToString();
                report.InvoiceID = dataRow[16].ToString();
                report.ReferenceTxnID = dataRow[17].ToString();
                report.TransactionGuid = Guid.NewGuid();
                report.UploadDateTime = DateTime.Now;
                report.TypeOfReport = Constants.Ebay;
                lReport.Add(report);
            }
            return lReport;
        }
    }
}