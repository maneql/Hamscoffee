using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hamscoffee.Models;
using System.Security.Cryptography;
using System.Text;
using WebApplication2;

namespace Hamscoffee.Controllers
{
    public class ProductDetailsController : Controller
    {

        HamscoffeeDBDataContext data = new HamscoffeeDBDataContext();

        

        public struct product
        {
            public string product_id;
            public string product_name;
            public string cost;
            public int quantity;
            public string url;
            public string title;
            public string description;
            public string provider_name;
        };

        // GET: Product
        public ActionResult ProductDetails(string id)
        {
            List<product> listtb = new List<product>();            

            var result = data.Products
                .Join(data.Image_stores, p => p.Product_ID, i => i.Product_ID, 
                (p, i) => new { p.Product_ID, p.Product_Name,p.Cost,p.Quantity,p.Provider_ID, i.URL ,p.Info_ID}).Where(p => p.Product_ID == id)
                .Join(data.Infos, r => r.Info_ID, i => i.Info_ID, 
                (r, i) => new { r.Product_ID, r.Product_Name, r.Cost,r.Quantity, r.URL,r.Provider_ID, i.Title, i.Description })
                .Join(data.Providers, r => r.Provider_ID, i => i.Provider_ID, 
                (r, i) => new { r.Product_ID, r.Product_Name, r.Cost,r.Quantity, r.URL, r.Title, r.Description,i.Provider_Name });


            
            foreach (var item in result)
            {
                product tb = new product();
                tb.product_id = item.Product_ID.ToString();
                tb.product_name = item.Product_Name.ToString();
                tb.cost = item.Cost.ToString();
                tb.quantity = Convert.ToInt32(item.Quantity);
                tb.url = item.URL.ToString();
                if (item.Title != null)
                {
                    tb.title = item.Title.ToString();
                }
                else
                {
                    tb.title = "";
                }
                tb.description = item.Description.ToString();
                tb.provider_name = item.Provider_Name.ToString();
                listtb.Add(tb);
            }
            ViewBag.prodc = listtb;
            ViewBag.catagory = data.Catagories;

            ViewBag.provider = data.Providers;
            return View();
        }

        
    }
}