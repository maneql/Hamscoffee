using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hamscoffee.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Hamscoffee.Controllers
{
    public class DataEditController : Controller
    {
        // GET: DataEdit
        List<string> listname = new List<string>();
        HamscoffeeDBDataContext data = new HamscoffeeDBDataContext();
        public struct datatableproduct
        {
            public string Product_ID;
            public string Product_Name;
            public string NameUnit;
            public string Provider_Name;
            public string Catagory_Name;
            public string Cost;
            public string Wholesale;
            public string Quantity;
            public string Description;
            public string URL;
        };

        public struct datatablebill
        {
            public string Bill_ID;
            public string Date;
            public string Total;
            public string Customer_Name;
        };

        public struct tabledetailbill
        {
            public int Bill_ID;
            public string Product_Name;
            public string Quantity;
            public string Total_Detail;
        };
        public ActionResult ListProduct()
        {
            if (Equals(Session["admin"], null))
            {
                return RedirectToAction("LoginAdmin", "LoginAdmin");
            }
            var product = data.Products.Join(data.Image_stores, p => p.Product_ID, i => i.Product_ID, (p, i) => new { p.Product_ID, p.Product_Name, p.Unit_ID, p.Provider_ID, p.Catagory_ID, p.Cost, p.Wholesale, p.Quantity, p.Info_ID, i.URL })
                .Join(data.Units, p => p.Unit_ID, i => i.Unit_ID, (p, i) => new { p.Product_ID, p.Product_Name, i.NameUnit, p.Provider_ID, p.Catagory_ID, p.Cost, p.Wholesale, p.Quantity, p.Info_ID, p.URL })
                .Join(data.Providers, p => p.Provider_ID, i => i.Provider_ID, (p, i) => new { p.Product_ID, p.Product_Name, p.NameUnit, i.Provider_Name, p.Catagory_ID, p.Cost, p.Wholesale, p.Quantity, p.Info_ID, p.URL })
                .Join(data.Catagories, p => p.Catagory_ID, i => i.Catagory_ID, (p, i) => new { p.Product_ID, p.Product_Name, p.NameUnit, p.Provider_Name, i.Catagory_Name, p.Cost, p.Wholesale, p.Quantity, p.Info_ID, p.URL })
                .Join(data.Infos, p => p.Info_ID, i => i.Info_ID, (p, i) => new { p.Product_ID, p.Product_Name, p.NameUnit, p.Provider_Name, p.Catagory_Name, p.Cost, p.Wholesale, p.Quantity, i.Description, p.URL }).OrderBy(p => p.Product_ID);

            List<datatableproduct> listtbproduct = new List<datatableproduct>();
            int x = 0;
            foreach (var item in product)
            {
                if (x != int.Parse(item.Product_ID))
                {
                    datatableproduct tb = new datatableproduct();
                    tb.Product_ID = item.Product_ID.ToString();
                    tb.Product_Name = item.Product_Name.ToString();
                    tb.NameUnit = item.NameUnit.ToString();
                    tb.Provider_Name = item.Provider_Name.ToString();
                    tb.Catagory_Name = item.Catagory_Name.ToString();
                    tb.Cost = item.Cost.ToString();
                    tb.Wholesale = item.Wholesale.ToString();
                    tb.Quantity = item.Quantity.ToString();
                    tb.Description = item.Description.ToString();
                    tb.URL = item.URL.ToString();
                    listtbproduct.Add(tb);
                    x = Convert.ToInt32(item.Product_ID);
                }
            }
            
            ViewBag.product = listtbproduct;
            
            ViewBag.catagory = data.Catagories;
            ViewBag.provider = data.Providers;
            ViewBag.slider = data.Sliders;
            ViewBag.unit = data.Units;

            return View();
        }

        public ActionResult RpProduct()
        {
            if (Equals(Session["admin"], null))
            {
                return RedirectToAction("LoginAdmin", "LoginAdmin");
            }
            ViewBag.products = data.Products;
            return View();
        }
        public ActionResult Bill()
        {
            if (Equals(Session["admin"], null))
            {
                return RedirectToAction("LoginAdmin", "LoginAdmin");
            }
            var bills = data.Bills.Join(data.Customers, b => b.Customer_ID, c => c.Customer_ID, (b, c) => new {b.Bill_ID,b.Date,b.Total,c.Name});
            List<datatablebill> listbills = new List<datatablebill>();
            foreach (var item in bills)
            {
                datatablebill tb = new datatablebill();
                tb.Bill_ID = item.Bill_ID.ToString();
                tb.Date = item.Date.ToString();
                tb.Total = item.Total.ToString();
                tb.Customer_Name = item.Name.ToString();
                listbills.Add(tb);
            }
            ViewBag.bill = listbills;

            return View();
        }

        public ActionResult DetailBill(int id)
        {
            if (Equals(Session["admin"], null))
            {
                return RedirectToAction("LoginAdmin", "LoginAdmin");
            }
            var bills = data.Bills.Join(data.Customers, b => b.Customer_ID, c => c.Customer_ID, (b, c) => new { b.Bill_ID, b.Date, b.Total, c.Name }).Where(b=>b.Bill_ID == id).FirstOrDefault();
            List<datatablebill> listbills = new List<datatablebill>();  
            datatablebill tb = new datatablebill();
            tb.Bill_ID = bills.Bill_ID.ToString();
            tb.Date = bills.Date.ToString();
            tb.Total = bills.Total.ToString();
            tb.Customer_Name = bills.Name.ToString();
            listbills.Add(tb);

            
            
            var detail = data.Details_Bills.Where(b => b.Bill_ID == id).Select(b => new { b.Bill_ID, b.Product.Product_Name, b.Quantity, b.Total_Detail });
            List<tabledetailbill> listdtlbill = new List<tabledetailbill>();
            foreach (var dtl in detail)
            {
                tabledetailbill tbb = new tabledetailbill();
                tbb.Bill_ID = dtl.Bill_ID;
                tbb.Product_Name = dtl.Product_Name;
                tbb.Quantity = dtl.Quantity.ToString();
                tbb.Total_Detail = dtl.Total_Detail.ToString();
                listdtlbill.Add(tbb);
            }
            
            
            ViewBag.bill = listbills;
            ViewBag.detail = listdtlbill;

            return View();
        }
        public ActionResult DeleteBill(int id)
        {
            if (Equals(Session["admin"], null))
            {
                return RedirectToAction("LoginAdmin", "LoginAdmin");
            }
            Bill bill = data.Bills.SingleOrDefault(b=>b.Bill_ID == id);
            if (bill == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            var dls = data.Details_Bills.Where(d => d.Bill_ID == id).ToList();
            foreach (var a in dls)
            {
                data.Details_Bills.DeleteOnSubmit(a);
            }
            data.SubmitChanges();

            data.Bills.DeleteOnSubmit(bill);
            data.SubmitChanges();
            return RedirectToAction("Bill");
        }
        public ActionResult Update(string id)
        {
            if (Equals(Session["admin"], null))
            {
                return RedirectToAction("LoginAdmin", "LoginAdmin");
            }
            var product = data.Products.Join(data.Image_stores, p => p.Product_ID, i => i.Product_ID, (p, i) => new { p.Product_ID, p.Product_Name, p.Unit_ID, p.Provider_ID, p.Catagory_ID, p.Cost, p.Wholesale, p.Quantity, p.Info_ID, i.URL })
                .Join(data.Units, p => p.Unit_ID, i => i.Unit_ID, (p, i) => new { p.Product_ID, p.Product_Name, i.NameUnit, p.Provider_ID, p.Catagory_ID, p.Cost, p.Wholesale, p.Quantity, p.Info_ID, p.URL })
                .Join(data.Providers, p => p.Provider_ID, i => i.Provider_ID, (p, i) => new { p.Product_ID, p.Product_Name, p.NameUnit, i.Provider_Name, p.Catagory_ID, p.Cost, p.Wholesale, p.Quantity, p.Info_ID, p.URL })
                .Join(data.Catagories, p => p.Catagory_ID, i => i.Catagory_ID, (p, i) => new { p.Product_ID, p.Product_Name, p.NameUnit, p.Provider_Name, i.Catagory_Name, p.Cost, p.Wholesale, p.Quantity, p.Info_ID, p.URL })
                .Join(data.Infos, p => p.Info_ID, i => i.Info_ID, (p, i) => new { p.Product_ID, p.Product_Name, p.NameUnit, p.Provider_Name, p.Catagory_Name, p.Cost, p.Wholesale, p.Quantity, i.Description, p.URL }).Where(p => p.Product_ID == id);

            List<datatableproduct> listtbproduct = new List<datatableproduct>();
            int x = 0;
            foreach (var item in product)
            {
                if (x != int.Parse(item.Product_ID))
                {
                    datatableproduct tb = new datatableproduct();
                    tb.Product_ID = item.Product_ID.ToString();
                    tb.Product_Name = item.Product_Name.ToString();
                    tb.NameUnit = item.NameUnit.ToString();
                    tb.Provider_Name = item.Provider_Name.ToString();
                    tb.Catagory_Name = item.Catagory_Name.ToString();
                    tb.Cost = item.Cost.ToString();
                    tb.Wholesale = item.Wholesale.ToString();
                    tb.Quantity = item.Quantity.ToString();
                    tb.Description = item.Description.ToString();
                    tb.URL = item.URL.ToString();
                    listtbproduct.Add(tb);
                    x = Convert.ToInt32(item.Product_ID);
                }
            }

            ViewBag.Item = listtbproduct;
            ViewBag.unit = data.Units;
            ViewBag.provider = data.Providers;
            ViewBag.catagory = data.Catagories;
            List<string> lst = new List<string>();
            var im = data.Image_stores.Where(p => p.Product_ID == id).Select(p => new { p.URL });
            foreach (var i in im)
            {
                lst.Add(i.URL);
            }
            return View(lst);
        }
        [HttpPost]
        public ActionResult Update(FormCollection collection, string id)
        {
            if (Equals(Session["admin"], null))
            {
                return RedirectToAction("LoginAdmin", "LoginAdmin");
            }
            resizeimg b = new resizeimg();
            List<string> name = Session["listname"] as List<string>;
            var img = data.Image_stores.Where(p => p.Product_ID == id).Select(p => p).ToList();
            int position = 0;
            var Product_ID = collection["ID"];
            var Product_Name = collection["name"];
            var Unit_ID = collection.GetValue("unit").AttemptedValue;
            var Provider_ID = collection.GetValue("provider").AttemptedValue.ToString();
            var Catagory_ID = collection.GetValue("catagory").AttemptedValue.ToString();
            var Cost = collection["cost"];
            var Wholesale = collection["wholesale"];
            var Quantity = collection["quantity"];

            var Title = collection["title"];
            var Description = collection["message"];
            ViewData["Product_ID"] = Product_ID;
            ViewData["Product_Name"] = Product_Name;
            ViewData["cost"] = Cost;
            ViewData["wholesale"] = Wholesale;
            ViewData["quantity"] = Quantity;

            var product = data.Products.Where(p => p.Product_ID == id).Select(p => p).ToList();
            
            foreach (var pro in product)
            {
                pro.Product_ID = Product_ID;
                pro.Product_Name = Product_Name;
                pro.Unit_ID = int.Parse(Unit_ID);
                pro.Provider_ID = Provider_ID;
                pro.Catagory_ID = Catagory_ID;
                pro.Cost = decimal.Parse(Cost);
                pro.Wholesale = decimal.Parse(Wholesale);
                pro.Quantity = decimal.Parse(Quantity);
            }

            foreach (var n in img)
            {
                if(position < img.Count())
                {
                    n.Product_ID = id;
                    n.URL = "/images/" + name[position];
                    n.Image_Name = name[position];
                    position++;
                }
            }
            data.SubmitChanges();
            return View();
        }

        public ActionResult CreateProduct()
        {
            if (Equals(Session["admin"], null))
            {
                return RedirectToAction("LoginAdmin", "LoginAdmin");
            }
            ViewBag.unit = data.Units;
            ViewBag.provider = data.Providers;
            ViewBag.catagory = data.Catagories;
            return View();
        }

        [HttpPost]
        public ActionResult CreateProduct(List<HttpPostedFileBase> fileupload)
        {
            if (Equals(Session["admin"], null))
            {
                return RedirectToAction("LoginAdmin", "LoginAdmin");
            }
            resizeimg b = new resizeimg();
            foreach (var filename in fileupload)
            {
                if (filename != null && filename.ContentLength > 0)
                {
                    var f = Path.GetFileName(filename.FileName);
                    string direction = Path.Combine(Server.MapPath("~/images"), f);
                    if (System.IO.File.Exists(direction) == true)
                    {
                        listname.Add(f);
                    }
                    else
                    {
                        var im = b.resize(Image.FromStream(filename.InputStream), 200, 200);
                        Bitmap c = new Bitmap(im);
                        Image_store imgs = new Image_store();
                        c.Save(direction);
                        listname.Add(f);
                    }

                }
                else
                {
                    ViewBag.noimg = "không có hình";
                }

            }
            ViewBag.unit = data.Units;
            ViewBag.provider = data.Providers;
            ViewBag.catagory = data.Catagories;
            ViewBag.listname = listname;
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(FormCollection collection, Info i)
        {
            if (Equals(Session["admin"], null))
            {
                return RedirectToAction("LoginAdmin", "LoginAdmin");
            }
            var Product_ID = collection["ID"];
            var Product_Name = collection["name"];
            var Unit_ID = collection.GetValue("unit").AttemptedValue;
            var Provider_ID = collection.GetValue("provider").AttemptedValue.ToString();
            var Catagory_ID = collection.GetValue("catagory").AttemptedValue.ToString();
            var Cost = collection["cost"];
            var Wholesale = collection["wholesale"];
            var Quantity = collection["quantity"];

            var Title = collection["title"];
            var Description = collection["message"];
            ViewData["Product_ID"] = Product_ID;
            ViewData["Product_Name"] = Product_Name;
            ViewData["cost"] = Cost;
            ViewData["wholesale"] = Wholesale;
            ViewData["quantity"] = Quantity;

            var id = data.Products.Where(r => r.Product_ID == Product_ID);
            //var img = data.Image_stores.Where(r=>r.Image_Name == );


            if (id.Count() == 0)
            {
                i.Title = Title;
                i.Description = Description;
                i.Time = DateTime.Now;

                data.Infos.InsertOnSubmit(i);
                data.SubmitChanges();

                var p = new Product
                {
                    Product_ID = Product_ID,
                    Product_Name = Product_Name,
                    Unit_ID = int.Parse(Unit_ID),
                    Provider_ID = Provider_ID,
                    Catagory_ID = Catagory_ID,
                    Cost = decimal.Parse(Cost),
                    Wholesale = decimal.Parse(Wholesale),
                    Quantity = decimal.Parse(Quantity),
                    Info_ID = i.Info_ID
                };
                data.Products.InsertOnSubmit(p);
                data.SubmitChanges();


                List<string> name = Session["listname"] as List<string>;
                foreach (var n in name)
                {
                    Image_store img = new Image_store();
                    img.Image_Name = n;
                    img.URL = "/images/" + n;
                    img.Product_ID = Product_ID;
                    data.Image_stores.InsertOnSubmit(img);
                    data.SubmitChanges();
                }

            }
            else
            {
                ViewData["Thông báo san pham"] = "Mã sản phẩm này đã tồn tại !";
                ViewBag.unit = data.Units;
                ViewBag.provider = data.Providers;
                ViewBag.catagory = data.Catagories;
                ViewBag.listname = Session["listname"];
                return View();
            }
            Session["listname"] = null;
            return RedirectToAction("ListProduct", "DataEdit");
        }

        [HttpPost]
        public ActionResult FixImage(string id, List<HttpPostedFileBase> files)
        {
            if (Equals(Session["admin"], null))
            {
                return RedirectToAction("LoginAdmin", "LoginAdmin");
            }
            var im1 = data.Image_stores.Where(p => p.Product_ID == id).Select(p => new { p.URL }).ToList();

            //resizeimg b = new resizeimg();

            resizeimg b = new resizeimg();
            var images = new List<string>();
            if (files == null || files.Count == 0)
            {
                foreach (var item in im1)
                {
                    images.Add(item.URL);
                }
                return PartialView("_FixImage", images);
            }
            else
            {
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var f = Path.GetFileName(file.FileName);
                        string direction = Path.Combine(Server.MapPath("~/images"), f);
                        if (System.IO.File.Exists(direction) == true)
                        {
                            if (!images.Contains(f))
                            {
                                images.Add("/images/" + f);
                            }

                        }
                        else
                        {
                            var im = b.resize(Image.FromStream(file.InputStream), 200, 200);
                            Bitmap c = new Bitmap(im);
                            Image_store imgs = new Image_store();
                            c.Save(direction);


                            if (!images.Contains(f))
                            {
                                images.Add("/images" + f);
                            }

                        }

                    }
                }
                Session["listname"] = images;
                return PartialView("_FixImage", images);
            }
        }

        public ActionResult DeleteProduct(string id)
        {
            if (Equals(Session["admin"], null))
            {
                return RedirectToAction("LoginAdmin", "LoginAdmin");
            }
            Product pd = data.Products.SingleOrDefault(p=>p.Product_ID == id);
            if (pd == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            var imgs = data.Image_stores.Where(m=>m.Product_ID == id).ToList();
            foreach(var a in imgs)
            {
                Image_store img = data.Image_stores.FirstOrDefault(x => x.Image_Name == a.Image_Name);
                data.Image_stores.DeleteOnSubmit(img);
            }
            data.SubmitChanges();
            data.Products.DeleteOnSubmit(pd);
            data.SubmitChanges();

            Info inf = data.Infos.SingleOrDefault(p=>p.Info_ID == pd.Info_ID);
            data.Infos.DeleteOnSubmit(inf);
            data.SubmitChanges();
            return RedirectToAction("ListProduct");
        }

        

        [HttpPost]
        public ActionResult UploadImage(List<string> images, List<HttpPostedFileBase> files)
        {
            if (Equals(Session["admin"], null))
            {
                return RedirectToAction("LoginAdmin", "LoginAdmin");
            }
            resizeimg b = new resizeimg();

            if (files == null || files.Count == 0)
            {
                return PartialView("_UploadImage", images);
            }

            images = images ?? new List<string>();

            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var f = Path.GetFileName(file.FileName);
                    string direction = Path.Combine(Server.MapPath("~/images"), f);
                    if (System.IO.File.Exists(direction) == true)
                    {
                        if (!images.Contains(f))
                        {
                            images.Add(f);
                        }

                    }
                    else
                    {
                        var im = b.resize(Image.FromStream(file.InputStream), 200, 200);
                        Bitmap c = new Bitmap(im);
                        Image_store imgs = new Image_store();
                        c.Save(direction);


                        if (!images.Contains(f))
                        {
                            images.Add(f);
                        }

                    }

                }
                else
                {
                    ViewBag.Error = "không có hình";
                }

            }
            Session["listname"] = images;
            return PartialView("_UploadImage", images);
        }
    }
}