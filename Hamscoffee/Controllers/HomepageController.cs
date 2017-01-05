using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hamscoffee.Models;

namespace Hamscoffee.Controllers
{
    public class HomepageController : Controller
    {
        // GET: Homepage
        public ActionResult Homepage()
        {
            return View();
            
        }

        [ChildActionOnly]
        public ActionResult Cart()
        {
            List<cart> lstCart = Session["CART"] as List<cart>;
            return PartialView(lstCart);
        }
    }

}