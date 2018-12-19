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

namespace MyNPO.Controllers
{
    public class EventCalendarController : BaseController
    {
        // GET: EventCalendar
        public ActionResult Index()
        {
            var scheduler = new DHXScheduler(this);
            var currentDate = DateTime.Now;
            scheduler.InitialDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);

            scheduler.LoadData = true;
            scheduler.EnableDataprocessor = true;

            return View(scheduler);
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
                            cInfo.Id = ++Id; cInfo.Text = changedEvent.text; cInfo.StartDate = changedEvent.start_date; cInfo.EndDate = changedEvent.end_date; cInfo.Type = "TempleEvent";
                            entityContext.calendarInfo.Add(cInfo);
                            typeOfAction = "Temple Event Created";
                        }
                        break;
                    case DataActionTypes.Delete:
                        {
                            cInfo = entityContext.calendarInfo.FirstOrDefault(q => q.Id == changedEvent.id);
                            entityContext.calendarInfo.Remove(cInfo);
                            typeOfAction = "Temple Event Cancelled";
                        }
                        break;
                    default:// "update"
                        {
                            cInfo = entityContext.calendarInfo.FirstOrDefault(q => q.Id == changedEvent.id);
                            cInfo.Id = changedEvent.id; cInfo.Text = changedEvent.text; cInfo.StartDate = changedEvent.start_date; cInfo.EndDate = changedEvent.end_date; cInfo.Type = "TempleEvent";
                            entityContext.calendarInfo.AddOrUpdate(cInfo);
                            typeOfAction = "Temple Event Modified";
                        }
                        break;
                }
                entityContext.SaveChanges();
                Helper.NotificationToAdmins(typeOfAction, cInfo);
            }
            catch (Exception ex)
            {
                action.Type = DataActionTypes.Error;
            }
            return (ContentResult)new AjaxSaveResponse(action);
        }
    }
}