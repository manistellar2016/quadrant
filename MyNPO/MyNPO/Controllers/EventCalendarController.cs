using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DHTMLX.Scheduler;
using DHTMLX.Common;
using DHTMLX.Scheduler.Data;
using MyNPO.Models;
using MyNPO.DataAccess;
using System.Data.Entity.Migrations;
using System.Data.Entity;

namespace MyNPO.Controllers
{
    public class EventCalendarController : BaseController
    {
        // GET: EventCalendar
        public ActionResult Index()
        {
            ViewBag.schedule = GetScheduler();
            //var priestServices = new PriestServices();
            //priestServices.Scheduler = GetScheduler();
            return View();
        }

        [HttpPost]
        public JsonResult EventSearch(string keyWord)
        {
            EntityContext entityContext = new EntityContext();
            var result = entityContext.reportInfo.Where(q => q.Name.ToLower().StartsWith(keyWord)).Select(q => q.Name).Distinct().ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult EventInfoSearch(string keyWord)
        {
            EntityContext entityContext = new EntityContext();
            var result1 = entityContext.reportInfo.FirstOrDefault(q => q.Name == keyWord && q.Date.Year >= DateTime.Now.Year && q.Date.Month >= DateTime.Now.Month && q.Date.Day >= DateTime.Now.Day);
            var result2 = entityContext.calendarInfo.FirstOrDefault(q => q.Type == "TempleEvent" && q.ReferenceTxnID == result1.ReferenceTxnID);
            var result = new PriestServices();
            if (result1 != null && result2 != null)
            {
                result = new PriestServices()
                {
                    Id = result2.Id,
                    Address = result2.Address,
                    Comments = result1.Reason,
                    DonationAmount = result1.Net,
                    Email = result1.FromEmailAddress,
                    EventsDate = result2.StartDate,
                    Gothram = result2.Gothram,
                    PaymentMode = result1.Description,// == "PayPal" ? "1" : result1.Description == "Cash" ? "2" : "3",
                    Phone = result1.PhoneNo,
                    PriestServicesList = result1.PriestServicesList
                };
            }
            else
            {
                result.Id = 0;
            }
            ViewBag.schedule = GetScheduler();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private DHXScheduler GetScheduler()
        {
            var scheduler = new DHXScheduler(this);
            var currentDate = DateTime.Now;
            scheduler.InitialDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);

            scheduler.LoadData = true;
            scheduler.EnableDataprocessor = true;
            return scheduler;
        }
        public ContentResult Data()
        {
            var entityContext = new EntityContext();
            var cEvent = new List<CalendarEvent>();
            try
            {
                var data = entityContext.calendarInfo.Where(t => t.Type == "TempleEvent").ToList();
                data.ForEach(q =>
                {
                    cEvent.Add(new CalendarEvent() { id = q.Id, text = q.Text, start_date = q.StartDate, end_date = q.EndDate });
                });
            }
            catch (Exception ex)
            {
                throw;
            }

            var data1 = new SchedulerAjaxData(cEvent);
            return (ContentResult)data1;
        }

        [HttpPost]
        public ActionResult Index(MyNPO.Models.PriestServices priestServices)
        {
            try
            {
                ViewBag.schedule = GetScheduler();
                if (ModelState.IsValid)
                {
                    var guid = Guid.NewGuid();
                    var dt = DateTime.Now;
                    EntityContext entityContext = new EntityContext();


                    var cInfo = entityContext.calendarInfo.FirstOrDefault(q => q.Id == priestServices.Id && q.Type == "TempleEvent");
                    if (cInfo != null)
                    {
                        cInfo.Name = priestServices.Name; cInfo.Text = priestServices.PriestServicesList;
                        cInfo.StartDate = priestServices.EventsDate;
                        cInfo.EndDate = priestServices.EventsDate.AddHours(1);
                        cInfo.Gothram = priestServices.Gothram; cInfo.Address = priestServices.Address;

                        entityContext.Entry(cInfo).State = System.Data.Entity.EntityState.Modified;
                        entityContext.calendarInfo.AddOrUpdate(cInfo);

                        var rInfo = entityContext.reportInfo.FirstOrDefault(q => q.ReferenceTxnID == cInfo.ReferenceTxnID);
                        if (rInfo != null)
                        {
                            rInfo.Name = priestServices.Name; rInfo.FromEmailAddress = priestServices.Email; rInfo.Net = priestServices.DonationAmount;
                            rInfo.PhoneNo = priestServices.Phone; rInfo.PriestServicesList = priestServices.PriestServicesList; rInfo.Date = dt;
                            rInfo.Reason = priestServices.Comments; rInfo.Description = priestServices.PaymentMode == "1" ? "PayPal" : priestServices.PaymentMode == "2" ? "Cash" : "Cheque";
                            rInfo.TypeOfReport = priestServices.PaymentMode == "1" ? Constants.PayPal : Constants.SystemDonation;
                        }
                        entityContext.Entry(rInfo).State = System.Data.Entity.EntityState.Modified;
                        entityContext.reportInfo.AddOrUpdate(rInfo);
                        entityContext.SaveChanges();

                        ModelState.Clear();
                        ViewBag.Status = "Successfully Updated";
                    }
                    else
                    {

                        // Generate the transaction id for cash transactions
                        var existingTransactionsPerDay = entityContext.reportInfo.Where(x => x.Date.Day == DateTime.Today.Day).ToList().Count;
                        var prefixCount = existingTransactionsPerDay <= 0 ? 1 : existingTransactionsPerDay + 1;

                        var transactionId = DateTime.Now.Date.ToString("yyyyMMdd") + "-" + prefixCount;

                        var report = new Report()
                        {
                            Name = priestServices.Name,
                            TransactionID = transactionId,
                            FromEmailAddress = priestServices.Email,
                            Net = priestServices.DonationAmount,
                            PhoneNo = priestServices.Phone,
                            PriestServicesList = priestServices.PriestServicesList,
                            Date = dt,
                            Time = dt.ToString(Constants.HourFormat),
                            Reason = priestServices.Comments, // Plan to LoginUser
                            Description = priestServices.PaymentMode == "1" ? "PayPal" : priestServices.PaymentMode == "2" ? "Cash" : "Cheque",
                            TransactionGuid = guid,
                            ReferenceTxnID = guid.ToString().Replace("-", ""),
                            UploadDateTime = dt,
                            TypeOfReport = priestServices.PaymentMode == "1" ? Constants.PayPal : Constants.SystemDonation
                        };

                        entityContext.reportInfo.Add(report);

                        // TODO: Add insert logic here
                        ModelState.Clear();
                        ViewBag.Status = "Successfully Saved";


                        cInfo = new CalendarInfo();
                        int Id = 0;
                        string typeOfAction = string.Empty;
                        try
                        {
                            var maxData = entityContext.calendarInfo.Where(q => q.Type == "TempleEvent").ToList();
                            if (maxData != null && maxData.Count > 0)
                            {
                                var maxId = maxData?.Max(q => q.Id);
                                Id = maxId.Value;
                            }
                            cInfo.Id = ++Id; cInfo.Name = priestServices.Name; cInfo.Text = priestServices.PriestServicesList;
                            cInfo.ReferenceTxnID = report.ReferenceTxnID;
                            cInfo.StartDate = priestServices.EventsDate; cInfo.Type = "TempleEvent";
                            cInfo.EndDate = priestServices.EventsDate.AddHours(1);
                            cInfo.Gothram = priestServices.Gothram; cInfo.Address = priestServices.Address;

                            entityContext.calendarInfo.Add(cInfo);
                        }
                        catch (Exception ex)
                        {

                        }
                        // Generated PDF Receipt and Send email attachment.
                        // ReceiptGenerator.GenerateDonationReceiptPdf(priestServices, report);
                        entityContext.SaveChanges();
                    }
                    //priestServices = new PriestServices();
                    //priestServices.Scheduler = GetScheduler();
                    return View();
                }
                else
                {
                    return View(priestServices);
                }
            }
            catch
            {
                return View();
            }
        }

        public ContentResult Save(int? id, FormCollection actionValues)
        {
            var entityContext = new EntityContext();
            var action = new DataAction(actionValues);
            var cInfo = new CalendarInfo();
            int Id = 0;
            string typeOfAction = string.Empty;
            try
            {
                var maxData = entityContext.calendarInfo.Where(q => q.Type == "TempleEvent").ToList();
                if (maxData != null && maxData.Count > 0)
                {
                    var maxId = maxData?.Max(q => q.Id);
                    Id = maxId.Value;
                }
                var changedEvent = (CalendarEvent)DHXEventsHelper.Bind(typeof(CalendarEvent), actionValues);
                switch (action.Type)
                {
                    case DataActionTypes.Insert:
                        {
                            //cInfo.Id = ++Id; cInfo.Text = changedEvent.text; cInfo.StartDate = changedEvent.start_date; cInfo.EndDate = changedEvent.end_date; cInfo.Type = "TempleEvent";
                            //entityContext.calendarInfo.Add(cInfo);
                            //typeOfAction = "Temple Event Created";
                        }
                        break;
                    case DataActionTypes.Delete:
                        {
                            cInfo = entityContext.calendarInfo.FirstOrDefault(q => q.Id == changedEvent.id);
                            var report = entityContext.reportInfo.FirstOrDefault(q => q.ReferenceTxnID == cInfo.ReferenceTxnID);
                            entityContext.reportInfo.Remove(report);
                            entityContext.calendarInfo.Remove(cInfo);
                            typeOfAction = "Temple Event Cancelled";
                            entityContext.SaveChanges();
                            //Helper.NotificationToAdmins(typeOfAction, cInfo);
                        }
                        break;
                    default:// "update"
                        {
                            //cInfo = entityContext.calendarInfo.FirstOrDefault(q => q.Id == changedEvent.id);
                            //cInfo.Id = changedEvent.id; cInfo.Text = changedEvent.text; cInfo.StartDate = changedEvent.start_date; cInfo.EndDate = changedEvent.end_date; cInfo.Type = "TempleEvent";
                            //entityContext.calendarInfo.AddOrUpdate(cInfo);
                            //typeOfAction = "Temple Event Modified";
                        }
                        break;
                }
                
            }
            catch (Exception ex)
            {
                action.Type = DataActionTypes.Error;
            }
            ViewBag.schedule = GetScheduler();
            //return (ContentResult)Index();
            return (ContentResult)new AjaxSaveResponse(action);
        }
    }
}