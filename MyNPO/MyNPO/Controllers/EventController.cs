using MyNPO.DataAccess;
using MyNPO.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyNPO.Controllers
{
    public class EventController : BaseController
    {
        // GET: Event
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase postedFile, FormCollection eventCreation)
        {
            string path = Server.MapPath("~/Images/Events/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!string.IsNullOrEmpty(postedFile?.FileName))
                postedFile.SaveAs(path + Path.GetFileName(postedFile.FileName));

            var createEvent = new Event()
            {
                Name = eventCreation["EventName"],
                Location = eventCreation["EventLocation"],
                Details = eventCreation["EventDescription"],
                StartDate = Convert.ToDateTime(eventCreation["EventStartDate"]),
                EndDate = Convert.ToDateTime(eventCreation["EventEndDate"]),
                UploadFileName = postedFile?.FileName
            };
            var entityContext = new EntityContext();
            entityContext.eventInfos.Add(createEvent);
            entityContext.SaveChanges();

            ViewBag.Status="Successfully Created"
            return View();
        }
        

    }
}