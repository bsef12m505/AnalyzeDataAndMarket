using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DTO;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http.Headers;

namespace FYPAdam.Controllers
{
    public class ADAMHomeController : Controller
    {

        public async Task<ActionResult> Index(string catName, string bName)
        {
            if (catName == null && bName == null)
            {
                catName = "Laptop";
                bName = "acer";
            }

            List<Category> categoryList = new List<Category>();
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Utilities.EngineUrl+"Home/ViewAllCategories");
                request.Timeout = 12000000;
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                var serializer = new JavaScriptSerializer();

                StreamReader stream = new StreamReader(response.GetResponseStream());
                string finalResponse = stream.ReadToEnd();
                categoryList = serializer.Deserialize<List<Category>>(finalResponse);


                ViewBag.NameOfCategories = categoryList;


            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                        .ReadToEnd();
            }
            List<Product> productList = new List<Product>();
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Utilities.EngineUrl + "Home/AllTopProductsAgainstBrandAndCategory?catName=" + catName + "&bName=" + bName);
                request.Timeout = 12000000;
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                var serializer = new JavaScriptSerializer();

                StreamReader stream = new StreamReader(response.GetResponseStream());
                string finalResponse = stream.ReadToEnd();
                productList = serializer.Deserialize<List<Product>>(finalResponse);

                ViewBag.ProductList = productList;


            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                        .ReadToEnd();
            }
            return View();


        }

        public ActionResult Products(int bId)
        {
            List<Brand> brandList = new List<Brand>();
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Utilities.EngineUrl + "Home/GetAllProductsAgainstBrand?bId=" + bId);
                request.Timeout = 12000000;
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                var serializer = new JavaScriptSerializer();

                StreamReader stream = new StreamReader(response.GetResponseStream());
                string finalResponse = stream.ReadToEnd();
                brandList = serializer.Deserialize<List<Brand>>(finalResponse);


            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                        .ReadToEnd();
            }
            return View(brandList[0]);
        }

        public ActionResult ProductDetails(int pId)
        {
            List<Product> productList = new List<Product>();
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Utilities.EngineUrl + "Home/ProductSpecifications?pId=" + pId);
                request.Timeout = 12000000;
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var serializer = new JavaScriptSerializer();
                StreamReader stream = new StreamReader(response.GetResponseStream());
                string finalResponse = stream.ReadToEnd();
                productList = serializer.Deserialize<List<Product>>(finalResponse);

            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                        .ReadToEnd();
            }

            return View(productList[0]);
        }

        public string CheckAjax(string brandName)
        {
            return brandName;
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }
        
        //public ActionResult Products(string brandName, int categoryId)
        //{
        //    List<Brand> productAgainstBrand=new List<Brand>();
        //    Brand brand = new Brand();
        //    try
        //    {

        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost/Engine/Home/ViewAllProducts");
        //        request.Timeout = 12000000;
        //        request.KeepAlive = false;
        //        request.ProtocolVersion = HttpVersion.Version10;
        //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //        var serializer = new JavaScriptSerializer();
           
        //        StreamReader stream = new StreamReader(response.GetResponseStream());
        //        string finalResponse = stream.ReadToEnd();
        //        productAgainstBrand = serializer.Deserialize<List<Brand>>(finalResponse);
        //        foreach (var b in productAgainstBrand)
        //        {
        //            if (b.Name.ToLower().Equals(brandName.ToLower()) && b.CategoryId==categoryId)
        //            {
        //                brand = b;
        //            }
        //        }
        //    }
        //    catch (WebException wex)
        //    {
        //        var pageContent = new StreamReader(wex.Response.GetResponseStream())
        //                .ReadToEnd();
        //    }

        //    return View(brand);
        //}
        //public ActionResult ProductDetails(int productId)
        //{
        //    List<Product> productList = new List<Product>();
        //    Product prod = new Product();
        //    try
        //    {

        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost/Engine/Home/ProductSpecifications");
        //        request.Timeout = 12000000;
        //        request.KeepAlive = false;
        //        request.ProtocolVersion = HttpVersion.Version10;
        //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //        var serializer = new JavaScriptSerializer();
        //        StreamReader stream = new StreamReader(response.GetResponseStream());
        //        string finalResponse = stream.ReadToEnd();
        //        productList = serializer.Deserialize<List<Product>>(finalResponse);
        //        foreach (var p in productList)
        //        {
        //            if (p.Id == productId)
        //            {
        //                prod = p;
        //            }


        //        }
        //    }
        //    catch (WebException wex)
        //    {
        //        var pageContent = new StreamReader(wex.Response.GetResponseStream())
        //                .ReadToEnd();
        //    }
        //    return View(prod);
        //}

    }
}
