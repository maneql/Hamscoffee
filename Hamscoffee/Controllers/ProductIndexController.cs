using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hamscoffee.Models;

namespace Hamscoffee.Controllers
{
    public class ProductIndexController : Controller
    {
        HamscoffeeDBDataContext data = new HamscoffeeDBDataContext();

        
        public struct datatable
        {
            public string product_name;
            public string cost;
            public string url;
            public string product_id;
        };
        // GET: HamsIndex
        public ActionResult Index(string id,string pid)
        {
            if (id == "5")
            {
                Session["id"] = "5"; 
            }
            else
            {
                Session["id"] = "0"; 
            }
            
            var result = (dynamic)null;



            if (id != "0")
            {
                result = data.Products.Join(data.Image_stores, p => p.Product_ID, i => i.Product_ID,
                         (p, i) => new { p.Product_ID, p.Product_Name, p.Cost, i.URL, p.Catagory_ID }).Where(p => p.Catagory_ID == id).Take(18);
            }
            else
            {
                result = data.Products.Join(data.Image_stores, p => p.Product_ID, i => i.Product_ID,
                         (p, i) => new { p.Product_ID, p.Product_Name, p.Cost, i.URL, p.Catagory_ID }).Where(p => p.Catagory_ID != "5").Take(18);                             
            }

            if (pid != null)
            {
                result = data.Products.Join(data.Image_stores, p => p.Product_ID, i => i.Product_ID,
                         (p, i) => new { p.Product_ID, p.Product_Name, p.Cost, i.URL, p.Provider_ID }).Where(p => p.Provider_ID == pid).Take(18);
            }

            List<datatable> listtb = new List<datatable>();
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
            ViewBag.prod = listtb;
            //Lay catagory 
            ViewBag.catagory = data.Catagories;

            ViewBag.provider = data.Providers;

            ViewBag.slider = data.Sliders.Select(s=>s).ToList();

            return View();
            
        }
    }
}