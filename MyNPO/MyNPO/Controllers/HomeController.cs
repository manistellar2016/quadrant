using MyNPO.DataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyNPO.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var connectionString = ConfigurationManager.AppSettings["DbConnectionString"];
            var entityContext = new EntityContext(connectionString);
            var linfos= entityContext.loginInfos.ToList();
            return View();
        }

        public ActionResult Donation()
        {
            ViewBag.Message = "Sai Babha Donation";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Sai Babha";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Information";

            return View();
        }
    }
}