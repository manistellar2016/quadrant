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
    public class derivedClass : HttpPostedFileBase
    {
        public string Type { get; set; }

        Stream stream;
        string contentType;
        string fileName;

        public derivedClass(Stream stream, string contentType, string fileName)
        {
            this.stream = stream;
            this.contentType = contentType;
            this.fileName = fileName;
        }

        public override int ContentLength
        {
            get { return (int)stream.Length; }
        }

        public override string ContentType
        {
            get { return contentType; }
        }

        public override string FileName
        {
            get { return fileName; }
        }

        public override Stream InputStream
        {
            get { return stream; }
        }

        public override void SaveAs(string filename)
        {
            using (var file = File.Open(filename, FileMode.CreateNew))
                stream.CopyTo(file);
        }
    }


    public class HomeController : Controller
    {
        public ActionResult Index()
        {



            //HttpClient httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Add("user", "saibabaseattle@hotmail.com");
            //httpClient.DefaultRequestHeaders.Add("vendor", "saibabaseattle@hotmail.com");
            //httpClient.DefaultRequestHeaders.Add("password", "saibaba*99");
            //httpClient.DefaultRequestHeaders.Add("partner", "PayPal");
            //httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));

            //string uri = "https://payments-reports.paypal.com/test-reportingengine/runReportRequest?reportName=DailyActivityReport&report_date=2018-08-19"; //<html><body><p>No content-type</p><p>Payflow-Build-ID : <i>release-48625933-prim_5wlapp6</i></p></body></html>

            //var result = httpClient.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;

            //var result = response.Content.ReadAsStringAsync().Result;


            //var report = new ReportingEngineRequest();
            //report.AuthRequest = new AuthRequest();
            //report.AuthRequest.User = "saibabaseattle@hotmail.com";
            //report.AuthRequest.Password = "saibaba*99";
            //report.AuthRequest.Partner = "PayPal";
            //report.AuthRequest.Vendor = "saibabaseattle@hotmail.com";
            //report.RunReportRequest = new RunReportRequest();
            //report.RunReportRequest.ReportName = "DailyActivityReport";
            //report.RunReportRequest.ReportParam = new ReportParam();
            //report.RunReportRequest.ReportParam.ParamValue = "2018-08-19";
            //report.RunReportRequest.ReportParam.ParamName = "report_date";
            //report.RunReportRequest.PageSize = "50";


            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //HttpResponseMessage response = client.PostAsync<ReportingEngineRequest>("https://payments-reports.paypal.com/test-reportingengine/runReportRequest",
            //                                        report, new XmlMediaTypeFormatter()).Result;

            //response.EnsureSuccessStatusCode();
            //var msg = response.Content.ReadAsAsync<ReportingEngineResponse>().Result;
            //var msg = response.Content.ReadAsAsync<ReportingEngineResponse>().Result;
            //Stream receiveStream = response.Content.ReadAsStreamAsync().Result;
            //StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            //var msg = readStream.ReadToEnd();


            //var connectionString = ConfigurationManager.AppSettings["DbConnectionString"];            
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase upload, FormCollection test)
        {
           // Date Time    Time Zone   Description Currency    Gross Fee Net Balance Transaction ID  From Email Address Name    Bank Name   Bank Account    Shipping and Handling Amount    Sales Tax   Invoice ID  Reference Txn ID
           
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
                        return View(csvTable);
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
            return View();
        }

        private List<Report> ConvertCsvTableToKindBaseReport(DataTable csvTable)
        {
            EntityContext entityContext = new EntityContext();

            var lReport = new List<Report>();
            foreach (DataRow dataRow in csvTable.Rows)
            {
                if (entityContext.reportInfo.Any(q => q.TransactionID == dataRow[4].ToString()))
                    continue;

                DateTime dt = Convert.ToDateTime(dataRow[1].ToString());

                var report = new Report();
                report.FromEmailAddress = dataRow[0].ToString();
                report.PhoneNo = dataRow[3].ToString();
                report.TransactionID = dataRow[4].ToString();
                report.Net = dataRow[10].ToString();
                report.Name = dataRow[11].ToString();
                report.Description = "KindBase";
                report.Date = dt.ToString("MM/dd/yyyy");    
                report.Time = dt.ToString("HH:mm:ss");
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
                if (entityContext.reportInfo.Any(q => q.TransactionID == dataRow[9].ToString()))
                     continue;               
                    

                var report = new Report();
                report.Date = dataRow[0].ToString();
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
                lReport.Add(report);
            }
            return lReport;
        }
    }
}