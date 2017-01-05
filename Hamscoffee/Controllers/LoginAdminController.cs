using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hamscoffee.Models;
using System.Security.Cryptography;
using System.Text;

namespace Hamscoffee.Controllers
{
    public class LoginAdminController : Controller
    {
        // GET: LoginAdmin
        public ActionResult LoginAdmin()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session["admin"] = null;
            return Redirect("LoginAdmin");
        }

        private string MaHoaMD5(string input)
        {
            byte[] pass = Encoding.UTF8.GetBytes(input);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(pass);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));
            return sb.ToString();
        }

        HamscoffeeDBDataContext db = new HamscoffeeDBDataContext();
        [HttpPost]
        public ActionResult login(FormCollection collection)
        {
            string adminid = collection["AdminID"];
            string adminpass = collection["pass"];
            var ad = db.Accout_admins.Where(p => p.IDAdmin == adminid).Where(p => p.Pass_admin == MaHoaMD5(adminpass)).Select(p=>p);
            if(ad.Count()==0)
            {
                ViewBag.khongthanhcong = "Sai mat khau hoac tai khoan admin";
                return View("LoginAdmin");
            }
            else
            {
                Session["admin"] = adminid;
                return RedirectToAction("Index", "AdminIndex");               
            }
            
        }
    }
}