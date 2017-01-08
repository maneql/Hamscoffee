using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hamscoffee.Models;
using System.Security.Cryptography;
using System.Text;
using WebApplication2;

//Code Page Cart



namespace Hamscoffee.Controllers
{
    public class CartController : Controller
    {
        HamscoffeeDBDataContext data = new HamscoffeeDBDataContext();
        
        //Xay dung trang Gio hang
        public ActionResult Cart()
        {
            List<cart> lstCart = GetCart();
            if (lstCart.Count == 0)
            {
                return RedirectToAction("Index", "ProductIndex", new { id = 0});
            }
            ViewBag.NumTotal = NumTotal();
            ViewBag.Total = Total();
            return View(lstCart);
        }

        
        //Lay gio hang
        public List<cart> GetCart()
        {
            List<cart> lstCart = Session["CART"] as List<cart>;
            if (lstCart == null)
            {
                //Neu gio hang chua ton tai thi khoi tao listGiohang
                lstCart = new List<cart>();
                Session["CART"] = lstCart;
            }
            return lstCart;
        }

        //Them hang vao gio
        public ActionResult AddCart(string id, string strURL)
        {
            //Lay ra Session gio hang
            List<cart> lstCart = GetCart();  
         
            //Kiem tra sách này tồn tại trong Session["CART"] chưa?

            cart product = lstCart.Find(n => n.sProductID == id);
            
            if (product == null)
            {
                product = new cart(id);
                lstCart.Add(product);
                Session["SL"] = NumTotal().ToString();
                return Redirect(strURL);
            }
            else
            {
                product.iQuantity++;
                Session["SL"] = NumTotal().ToString();
                return Redirect(strURL);
            }
        }

        
        private int NumTotal()
        {
            int numtotal = 0;
            List<cart> lstCart= Session["CART"] as List<cart>;
            if (lstCart != null)
            {
                numtotal = lstCart.Sum(n => n.iQuantity);
            }
            return numtotal;
        }
        //Tinh tong tien
        private double Total()
        {
            double Total = 0;
            List<cart> lstCart = Session["CART"] as List<cart>;
            if (lstCart != null)
            {
                Total = lstCart.Sum(n => n.dTotal);
            }
            return Total;
        }

        
        private void getdataaccount()
        {
            var cs = data.Customers.Join(data.Customer_Accs, c => c.Customer_ID, a => a.Customer_ID, (c, a)
                => new { c.Name, a.User, c.Mail, c.Address, c.Phone }).Where(c => c.User == Session["Taikhoan"]).FirstOrDefault();
            ViewBag.name = cs.Name;
            ViewBag.mail = cs.Mail;
            ViewBag.phone = cs.Phone;
            ViewBag.address = cs.Address;
        }
        //Xoa Giohang
        public ActionResult DeleteCart(string id)
        {
            //Lay gio hang tu Session
            List<cart> lstCart = GetCart();
            //Kiem tra sach da co trong Session["CART"]
            cart product = lstCart.SingleOrDefault(n => n.sProductID == id);
            //Neu ton tai thi cho sua Soluong
            if (product != null)
            {
                lstCart.RemoveAll(n => n.sProductID == id);
                return RedirectToAction("Cart");
        
            }
            if (lstCart.Count == 0)
            {
                return RedirectToAction("Index", "ProductIndex", new { id = 0 });
            }
            return RedirectToAction("Cart");
        }
        //Xoa tat ca thong tin trong Gio hang
        public ActionResult DeleteAllCart()
        {
            //Lay gio hang tu Session
            List<cart> lstCart = GetCart();
            lstCart.Clear();
            return RedirectToAction("Index", "ProductIndex", new { id = 0 });
        }
        //
        //Hien thi View DatHang de cap nhat cac thong tin cho Don hang
        [HttpGet]
        public ActionResult OrderDetails()
        {
            //Kiem tra dang nhap
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("LoginSignup", "LoginSignup");
            }
            if (Session["CART"] == null)
            {
                return RedirectToAction("Index", "ProductIndex", new { id = 0 });
            }
        
            //Lay gio hang tu Session
            List<cart> lstCart = GetCart();
            ViewBag.NumTotal = NumTotal();
            ViewBag.Total = Total();
            getdataaccount();
            return View(lstCart);
        }

        //Xay dung chuc nang Dathang
        
        public ActionResult Order()
        {
            string SECURE_SECRET = "A3EFDFABA8653DF2342E8DAC29B51AF0";
            string hashvalidateResult = "";
            // Khoi tao lop thu vien
            VPCRequest conn = new VPCRequest("http://onepay.vn");
            conn.SetSecureSecret(SECURE_SECRET);
            // Xu ly tham so tra ve va kiem tra chuoi du lieu ma hoa
            hashvalidateResult = conn.Process3PartyResponse(Request.QueryString);

            // Lay gia tri tham so tra ve tu cong thanh toan
            String vpc_TxnResponseCode = conn.GetResultField("vpc_TxnResponseCode", "Unknown");
            string amount = conn.GetResultField("vpc_Amount", "Unknown");
            string localed = conn.GetResultField("vpc_Locale", "Unknown");
            string command = conn.GetResultField("vpc_Command", "Unknown");
            string version = conn.GetResultField("vpc_Version", "Unknown");
            string cardBin = conn.GetResultField("vpc_Card", "Unknown");
            string orderInfo = conn.GetResultField("vpc_OrderInfo", "Unknown");
            string merchantID = conn.GetResultField("vpc_Merchant", "Unknown");
            string authorizeID = conn.GetResultField("vpc_AuthorizeId", "Unknown");
            string merchTxnRef = conn.GetResultField("vpc_MerchTxnRef", "Unknown");
            string transactionNo = conn.GetResultField("vpc_TransactionNo", "Unknown");
            string txnResponseCode = vpc_TxnResponseCode;
            string message = conn.GetResultField("vpc_Message", "Unknown");
            Session["MerchTxnRef"] = merchTxnRef;
            Session["Amount"] = decimal.Parse(amount) / 100;
            Session["OrderInfo"] = orderInfo;
            Session["Merchant"] = merchantID;

            if (hashvalidateResult == "CORRECTED" && txnResponseCode.Trim() == "0")
            {
                Bill bill = new Bill();
                var cus = data.Customers.Join(data.Customer_Accs, x => x.Customer_ID, y => y.Customer_ID,
                    (x, y) => new { x.Customer_ID, y.User }).Where(y => y.User == Session["Taikhoan"]).FirstOrDefault();
                List<cart> ca = GetCart();
                bill.Customer_ID = cus.Customer_ID;
                bill.Date = DateTime.Now;
                bill.Total = (decimal)Total();
                data.Bills.InsertOnSubmit(bill);
                data.SubmitChanges();

               // var product = (dynamic)null;
                //Them chi tiet don hang            
                foreach (var item in ca)
                {
                    Details_Bill dtb = new Details_Bill();
                    dtb.Bill_ID = bill.Bill_ID;
                    dtb.Product_ID = item.sProductID;
                    dtb.Quantity = item.iQuantity;
                    dtb.Total_Detail = (decimal)item.dTotal;
                    data.Details_Bills.InsertOnSubmit(dtb);

                    Product pd = data.Products.SingleOrDefault(d=>d.Product_ID == item.sProductID) as Product;
                    pd.Quantity = pd.Quantity - item.iQuantity;
                    UpdateModel(pd);
                }
                data.SubmitChanges();
                Session["CART"] = null;
                return RedirectToAction("OrderConfirm", "Cart", new { f = true});
            }           
            else
            {
                return RedirectToAction("OrderConfirm", "Cart", new { f = true });
            }
            //Them Don hang
            
        }
        public ActionResult OrderConfirm(bool f)
        {
            if (f==true)
            {
                ViewData["Thong Bao"] = "Đã xác nhận đơn hàng. Xin chân thành cảm ơn !";
                getdataaccount();
                return View();
            }
            else
            {
                ViewData["Thong Bao"] = "Thanh toán không thành công !";
                return View();
            }
            
        }
        


        //Thanh toan qua One_Pay
        public ActionResult thanhtoan()
        {
            double total = Total() * 100;

            var id = GetCart().ToList().LastOrDefault();
            string text = "";
            foreach (var cart in GetCart())
            {
                text = text + cart.sProductName + "  " + cart.iQuantity;
                if (cart.sProductID != id.sProductID)
                {
                    text = text + " + ";
                }
            }

            //Session["Taikhoan"]
            
            //Lay thong tin khach hang dang nhap

            var cus = data.Customers.Join(data.Customer_Accs, c => c.Customer_ID, a => a.Customer_ID, (c, a) 
                => new { a.User, c.Mail, c.Address, c.Phone }).Where(c => c.User == Session["Taikhoan"]).FirstOrDefault();

            string SECURE_SECRET = "A3EFDFABA8653DF2342E8DAC29B51AF0";
            // Khoi tao lop thu vien va gan gia tri cac tham so gui sang cong thanh toan

            VPCRequest conn = new VPCRequest("https://mtf.onepay.vn/onecomm-pay/vpc.op");
            conn.SetSecureSecret(SECURE_SECRET);
            
            // Add the Digital Order Fields for the functionality you wish to use
            // Core Transaction Fields
            conn.AddDigitalOrderField("Title", "onepay paygate");
            conn.AddDigitalOrderField("vpc_Locale", "vn");//Chon ngon ngu hien thi tren cong thanh toan (vn/en)
            conn.AddDigitalOrderField("vpc_Version", "2");
            conn.AddDigitalOrderField("vpc_Command", "pay");
            conn.AddDigitalOrderField("vpc_Merchant", "ONEPAY");
            conn.AddDigitalOrderField("vpc_AccessCode", "D67342C2");
            conn.AddDigitalOrderField("vpc_MerchTxnRef", MaHoaMD5(ngaunhien().ToString()));
            conn.AddDigitalOrderField("vpc_OrderInfo", text);
            conn.AddDigitalOrderField("vpc_Amount", total.ToString());
            conn.AddDigitalOrderField("vpc_Currency", "VND");
            conn.AddDigitalOrderField("vpc_ReturnURL", Url.Action("Order", "Cart", null, Request.Url.Scheme));
            // Thong tin them ve khach hang. De trong neu khong co thong tin
            conn.AddDigitalOrderField("vpc_SHIP_Street01", cus.Address);
            conn.AddDigitalOrderField("vpc_SHIP_Provice", "");
            conn.AddDigitalOrderField("vpc_SHIP_City", "");
            conn.AddDigitalOrderField("vpc_SHIP_Country", "Vietnam");
            conn.AddDigitalOrderField("vpc_Customer_Phone", cus.Phone);
            conn.AddDigitalOrderField("vpc_Customer_Email", cus.Mail);
            conn.AddDigitalOrderField("vpc_Customer_Id", "onepay_paygate");
            // Dia chi IP cua khach hang
            conn.AddDigitalOrderField("vpc_TicketNo", "");
            // Chuyen huong trinh duyet sang cong thanh toan
            String url = conn.Create3PartyQueryString();
            return Redirect(url);
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

        private int ngaunhien()
        {
            Random i = new Random();
            int i2;
            i2 = i.Next(-2147483648, 2147483647);
            return i2;
        }
    }
}