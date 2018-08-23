using MyNPO.DataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MyNPO.Controllers
{
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



            var connectionString = ConfigurationManager.AppSettings["DbConnectionString"];            
            var entityContext = new EntityContext();
            var linfos= entityContext.loginInfos.ToList();
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
    }
}