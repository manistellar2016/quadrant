using MyNPO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyNPO.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Login login)
        {
            if (login.UserName == "Admin@gmail.com" && login.Password == "Admin")
            {
                Session[Constants.UserId] = "99";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
    }
}