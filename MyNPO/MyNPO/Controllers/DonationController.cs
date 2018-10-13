using MyNPO.DataAccess;
using MyNPO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyNPO.Controllers
{
    public class DonationController : BaseController
    {
      
        // GET: Donation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Donation/Create
        [HttpPost]
        public ActionResult Create(MyNPO.Models.Donation donation)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var guid = Guid.NewGuid();
                    var dt = DateTime.Now;
                    EntityContext entityContext = new EntityContext();
                    var report = new Report()
                    {
                        Name = donation.Name,
                        FromEmailAddress = donation.Email,
                        Net = donation.DonationAmount,
                        PhoneNo = donation.Phone,
                        Date = dt,
                        Time = dt.ToString(Constants.HourFormat),
                        Description = $"SystemDonation", // Plan to LoginUser
                        Reason = donation.Reason,
                        TransactionGuid = guid,
                        ReferenceTxnID = guid.ToString().Replace("-", ""),
                        UploadDateTime = dt,
                        TypeOfReport = Constants.SystemDonation

                    };
                    entityContext.reportInfo.Add(report);
                    entityContext.SaveChanges();
                    // TODO: Add insert logic here
                    ModelState.Clear();
                    ViewBag.Status = "Successfully Saved";

                    return View();
                }
                else
                {
                    return View(donation);
                }
            }
            catch
            {
                return View();
            }
        }

        
    }
}
