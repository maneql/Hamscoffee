using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hamscoffee.Models;

namespace Hamscoffee.Models
{
    public class cart
    {
        HamscoffeeDBDataContext data = new HamscoffeeDBDataContext();
        public string sProductID { set; get; }
        public string sProductName { set; get; }
        public string sURLImage { set; get; }
        public Double dCost { set; get; }
        public int iQuantity { set; get; }
        public Double dTotal
        {
            get { return iQuantity * dCost; }

        }
        public cart(string productid)
        {
            sProductID = productid;
            var prod = data.Products.Join(data.Image_stores,r=>r.Product_ID,i=>i.Product_ID,(r,i) => new{r.Product_ID,r.Product_Name,i.URL,r.Cost}).Where(r=>r.Product_ID == productid).FirstOrDefault();
            sProductName = prod.Product_Name;
            sURLImage = prod.URL;
            dCost = double.Parse(prod.Cost.ToString());
            iQuantity = 1;
        }                
    }
}