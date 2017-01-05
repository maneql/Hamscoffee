using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hamscoffee.Models;
using PagedList;
using PagedList.Mvc;

namespace Hamscoffee.Controllers
{
    public class StoreController : Controller
    {
        HamscoffeeDBDataContext data = new HamscoffeeDBDataContext();
        // GET: Store
        public struct datatable
        {
            public string product_name;
            public string cost;
            public string url;
            public string product_id;
        };
        public ActionResult StoreProduct(string id,string pid, int? page)
        {
            //Tao bien quy dinh so san pham tren moi trang
            int pageSize = 6;
            //Tao bien so trang
            int pageNum = (page ?? 1);

            if (id == "5")
            {
                Session["id"] = "5";
            }
            else
            {
                Session["id"] = "0";
            }
            List<datatable> listtb = new List<datatable>();
            //var result = data.Products.Join(data.Image_stores, p => p.Product_ID, i => i.Product_ID, (p, i) => new {p.Product_ID, p.Product_Name, p.Cost, i.URL}) ;
            //foreach(var item in result)
            //{
            //    datatable tb = new datatable();
            //    tb.cost = item.Cost.ToString();
            //    tb.product_name = item.Product_Name.ToString();
            //    tb.url = item.URL.ToString();
            //    tb.product_id = item.Product_ID.ToString();
            //    listtb.Add(tb);
            //}
            var result = (dynamic)null;

            if (id != "0")
            {
                result = data.Products.Join(data.Image_stores, p => p.Product_ID, i => i.Product_ID,
                         (p, i) => new { p.Product_ID, p.Product_Name, p.Cost, i.URL, p.Catagory_ID }).Where(p => p.Catagory_ID == id);
            }
            else
            {
                result = data.Products.Join(data.Image_stores, p => p.Product_ID, i => i.Product_ID,
                         (p, i) => new { p.Product_ID, p.Product_Name, p.Cost, i.URL, p.Catagory_ID }).Where(p => p.Catagory_ID != "5");               
            }

            if (pid != null)
            {
                result = data.Products.Join(data.Image_stores, p => p.Product_ID, i => i.Product_ID,
                         (p, i) => new { p.Product_ID, p.Product_Name, p.Cost, i.URL, p.Provider_ID }).Where(p => p.Provider_ID == pid).Take(18);
            }

            int x = 0;
            foreach (var item in result)
            {
                if (x != int.Parse(item.Product_ID))
                {
                    datatable tb = new datatable();
                    tb.cost = item.Cost.ToString();
                    tb.product_name = item.Product_Name.ToString();
                    tb.url = item.URL.ToString();
                    tb.product_id = item.Product_ID.ToString();
                    listtb.Add(tb);
                    x = Convert.ToInt32(item.Product_ID);
                }
            }
            var catagories = data.Catagories;
            ViewBag.provider = data.Providers;
            ViewBag.catagory = catagories;
            return View(listtb.ToPagedList(pageNum,pageSize));           
        }
    }
}