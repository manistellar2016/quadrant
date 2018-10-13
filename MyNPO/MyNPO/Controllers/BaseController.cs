using MyNPO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyNPO.Controllers
{
    public class BaseController : Controller
    {
        private int userId;
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (Session[Constants.UserId] == null || string.IsNullOrEmpty(Session[Constants.UserId].ToString()))
                Response.Redirect(Constants.RedirectToLogin);
            userId = Convert.ToInt16(Session[Constants.UserId].ToString());
        }

        public int UserId => userId;
    }
}