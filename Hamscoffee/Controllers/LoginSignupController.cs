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
    public class LoginSignupController : Controller
    {

        HamscoffeeDBDataContext data = new HamscoffeeDBDataContext();
        
        public ActionResult LoginSignup()
        {
            return View();
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

        [HttpPost]       
        public ActionResult Signup(FormCollection collection, Customer c, Customer_Acc ca)
        {
            // Gán các giá tị người dùng nhập liệu cho các biến
            var user  = collection["ID"];
            var email = collection["email"];
            var name  = collection["name"];
            var pass  = collection["pass"];
            var phone = collection["phone"];
            var address = collection["address"];
            var sex = collection["sex"];
            ViewData["user"] = user;
            ViewData["email"] = email;
            ViewData["name"] = name;
            ViewData["pass"] = pass;
            ViewData["phone"] = phone;
            ViewData["address"] = address;
            ViewData["sex"] = sex;
            //Tìm tên user người nhập đã tồn tại hay không
            var u = data.Customer_Accs.Where(r => r.User == user);
            var m = data.Customers.Where(r => r.Mail == email);
            
            if (u.Count() == 0)
            {
                if (m.Count() == 0)
                {
                    //Thêm thông tin đăng ký của khách hàng vào database
                    c.Name = name;
                    c.Address = address;
                    c.Phone = phone;
                    c.Mail = email;
                    c.Sex = sex;

                    data.Customers.InsertOnSubmit(c);
                    data.SubmitChanges();

                    //var id = data.Customers.OrderByDescending(x => x.Customer_ID).FirstOrDefault();

                    ca.Customer_ID = c.Customer_ID;
                    ca.User = user;
                    ca.Pass = MaHoaMD5(pass);
                    ca.Point = 0;

                    data.Customer_Accs.InsertOnSubmit(ca);
                    data.SubmitChanges();
                }
                else
                {
                    ViewData["Thông báo email"] = "Email này đã được sử dụng !";
                    return View("LoginSignup");
                }
            }            
            else
            {
                ViewData["Thông báo"] = "ID này đã được sử dụng !";
                return View("LoginSignup");
            }
            return RedirectToAction("thongbao");            
        }

        public ActionResult thongbao()
        {
            ViewData["Thong Bao"] = "ĐK thanh cong";

            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection, Customer c, Customer_Acc ca)
        {
            var pass = collection["password"];
            var user = collection["userID"];
            ViewData["userID"] = user;
            ViewData["password"] = pass;
            var u = data.Customer_Accs.Where(r => r.User == user).Where(r => r.Pass == MaHoaMD5(pass));
            if (u.Count() != 0)
            {
                Session["Taikhoan"] = user;
                return RedirectToAction("Index", "ProductIndex", new {id = 0 });
            }
            else
            {
                ViewData["thong bao"] = "Sai mật khẩu hoặc tài khoản";
            }
            return View("LoginSignup");
        }
        public ActionResult Dangxuat()
        {
            Session["Taikhoan"] = null;
            return View("LoginSignup");
        }
    }
}