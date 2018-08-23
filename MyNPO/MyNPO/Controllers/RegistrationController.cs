using MyNPO.DataAccess;
using MyNPO.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyNPO.Controllers
{
    public class RegistrationController : Controller
    {
        static string connectionString = ConfigurationManager.AppSettings["DbConnectionString"];
        EntityContext entityContext = new EntityContext();
        // GET: Registration
        public ActionResult Index()
        {           

            var it=entityContext.familyInfos.ToList();
            it.ForEach(q => q.DependentDetails = entityContext.dependentInfos.Where(t => t.PrimaryId == q.PrimaryId).ToList());            
            return View(it);
        }

        // GET: Registration/Details/5
        public ActionResult Details(Guid id)
        {
            var it = entityContext.familyInfos.FirstOrDefault(q => q.PrimaryId == id);
            it.DependentDetails = entityContext.dependentInfos.Where(q => q.PrimaryId == it.PrimaryId).ToList();

            return View(it);
        }

        // GET: Registration/Create
        public ActionResult Create()
        {           
            //ViewBag.VolunteerInfo = "PrimaryInfo";
            return View();
        }

        public ActionResult Next(FamilyInfo family)
        {
            family.DependentDetails = new List<DependentInfo>();

            return View("Create", family);
        }
        // POST: Registration/Create
        [HttpPost]
        public JsonResult Create(FamilyInfo familyInfo)
        {
            string status = "";
            try
            {              
                // TODO: Add insert logic here
                Guid transactionId = Guid.NewGuid();

                if (familyInfo.MarriageDate == DateTime.MinValue)
                    familyInfo.MarriageDate = null;

                var famInfo = entityContext.familyInfos.FirstOrDefault(q => q.FirstName == familyInfo.FirstName && q.LastName == familyInfo.LastName && q.DateOfBirth == familyInfo.DateOfBirth);
                if (famInfo != null && !string.IsNullOrEmpty(famInfo.FirstName))
                {
                    //ViewBag.Error = $"Already {familyInfo.FirstName} is there";
                    status = $"Already {familyInfo.FirstName} is there";
                }
                else
                {

                    familyInfo.PrimaryId = transactionId;
                    familyInfo?.DependentDetails?.ForEach(s => s.PrimaryId = transactionId);

                    entityContext.familyInfos.Add(familyInfo);
                    entityContext.SaveChanges();
                    status = "Saved";
                }
               
            }
            catch(Exception ex)
            {
                status = "Sorry Try Again";
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        // GET: Registration/Edit/5
        public ActionResult Edit(Guid id)
        {
            var it = entityContext.familyInfos.FirstOrDefault(q => q.PrimaryId == id);
            it.DependentDetails = entityContext.dependentInfos.Where(q => q.PrimaryId == it.PrimaryId).ToList();

            return View(it);
        }

        // POST: Registration/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, FamilyInfo collection)
        {
            try
            {
                // TODO: Add update logic here

                var familInfo = entityContext.familyInfos.FirstOrDefault(q => q.PrimaryId == id);

                familInfo.DependentDetails = entityContext.dependentInfos.Where(t => t.PrimaryId == collection.PrimaryId).ToList();

                foreach(var dependent in familInfo.DependentDetails)
                {
                    var changeDependent = collection.DependentDetails.FirstOrDefault(q => q.Id == dependent.Id);
                    if(changeDependent!=null)
                    {
                        dependent.DOB = changeDependent.DOB;
                        dependent.Name = changeDependent.Name;
                        dependent.RelationShip = changeDependent.RelationShip;                      
                        
                    }
                }

                familInfo.FirstName = collection.FirstName;
                familInfo.LastName = collection.LastName;
                familInfo.Address = collection.Address;
                familInfo.City = collection.City;
                familInfo.DateOfBirth = collection.DateOfBirth;
                familInfo.Email = collection.Email;
                familInfo.MaritalStatus = collection.MaritalStatus;
                familInfo.MarriageDate = collection.MarriageDate;
                familInfo.MobileNo = collection.MobileNo;
                familInfo.NoOfDependents = collection.NoOfDependents;
                familInfo.ZipCode = collection.ZipCode;
                familInfo.IsVolunteer = collection.IsVolunteer;

                entityContext.Entry(familInfo).State = System.Data.Entity.EntityState.Modified;               
                entityContext.familyInfos.AddOrUpdate(familInfo);

                entityContext.SaveChanges();

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: Registration/Delete/5
        public ActionResult Delete(Guid id)
        {
            var it = entityContext.familyInfos.FirstOrDefault(q => q.PrimaryId == id);
            it.DependentDetails = entityContext.dependentInfos.Where(q => q.PrimaryId == it.PrimaryId).ToList();
            return View(it);
        }

        // POST: Registration/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var it = entityContext.familyInfos.FirstOrDefault(q => q.PrimaryId == id);
                it.DependentDetails = entityContext.dependentInfos.Where(q => q.PrimaryId == it.PrimaryId).ToList();
                entityContext.dependentInfos.RemoveRange(it.DependentDetails);
                entityContext.familyInfos.Remove(it);               
                entityContext.SaveChanges();

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return View();
            }
        }
    }
}
