using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hamscoffee.Controllers
{
    public class AdminIndexController : Controller
    {
        // GET: AdminIndex
        public ActionResult Index()
        {
            if (Equals(Session["admin"],null))
            {                
                return RedirectToAction("LoginAdmin", "LoginAdmin");
            }
            else
            {
                return View();
            }
        }
    }
}