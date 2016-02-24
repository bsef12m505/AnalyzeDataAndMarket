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
        
        public  ActionResult Index(string categoryName)
        {
            if (categoryName == null)
            {
                categoryName = "Laptop";
            }

            List<Category> categoryList = new List<Category>();
            Category category = new Category();
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost/Engine/Home/ViewAllCategories");
                request.Timeout = 12000000;
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                var serializer = new JavaScriptSerializer();
                
                StreamReader stream = new StreamReader(response.GetResponseStream());
                string finalResponse = stream.ReadToEnd();
                var f = serializer.Deserialize<object>(finalResponse);
                string s = (string)f;
                //categoryList = serializer.Deserialize<List<Category>>(finalResponse);
                //List<string> nameOfCategories = new List<string>();

                //foreach (var cat in categoryList)
                //{
                //    if (cat.Name.ToLower().Equals(categoryName.ToLower()))
                //    {
                //        category = cat;
                //    }
                //    nameOfCategories.Add(cat.Name);

                //}
                //ViewBag.NameOfCategories = nameOfCategories;
                ViewBag.Name = f;
                return View();
            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                        .ReadToEnd();
            }
                return View();
            

        }

        public ActionResult Products()
        {
            return View();
        }

        public ActionResult ProductDetails()
        {
            return View();
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
