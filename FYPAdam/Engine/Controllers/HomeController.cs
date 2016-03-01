using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdamDal;
using System.Web.Script.Serialization;
using System.IO;
using System.Net;
using System.Threading;

namespace Engine.Controllers
{
    public class HomeController : Controller
    {

        public JsonResult Index()
        {
            List<Product> p = new List<Product>();
            try
            {
                AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
                ed.Configuration.ProxyCreationEnabled = false;
                DbWrappers wrap = new DbWrappers();
                p = wrap.GetAllProducts();


            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                        .ReadToEnd();
            }
            return this.Json(p, JsonRequestBehavior.AllowGet);

        }
        //new
        public JsonResult ViewAllCategories()
        {
            List<Category> categoryList = new List<Category>();

            try
            {
                AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
                //ed.Configuration.ProxyCreationEnabled = false;
                DbWrappers wrap = new DbWrappers();
                categoryList = wrap.GetAllCategories();

            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                     .ReadToEnd();
            }
            return this.Json(categoryList, JsonRequestBehavior.AllowGet);

        }

        //new one
        public JsonResult AllTopProductsAgainstBrandAndCategory(string catName, string bName)
        {
            List<Product> productList = new List<Product>();

            try
            {
                DbWrappers wrap = new DbWrappers();
                productList = wrap.GetTopProductsAgainstBrandAndCategory(catName, bName);


            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                     .ReadToEnd();
            }
            return this.Json(productList, JsonRequestBehavior.AllowGet);

        }
        //new 
        public JsonResult GetAllProductsAgainstBrand(int bId)
        {
            List<Brand> brand = new List<Brand>();

            try
            {
                DbWrappers wrap = new DbWrappers();
                brand = wrap.GetAllProductsAgainstBrand(bId);
            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                     .ReadToEnd();
            }
            return this.Json(brand, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductSpecifications(int pId)
        {
            List<Product> productList = new List<Product>();

            try
            {
                DbWrappers wrap = new DbWrappers();
                productList = wrap.GetAllSpecificationsAgainstProduct(pId);

            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                        .ReadToEnd();
            }
            return this.Json(productList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllMobileBrands()
        {

            List<Brand> brands = new List<Brand>();
            try
            {
                DbWrappers wrap = new DbWrappers();
               // brands=wrap.GetAllBrandNamesOfLaptops();
                brands = wrap.GetAllBrandOfMobiles();
            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                        .ReadToEnd();
            }
            return this.Json(brands, JsonRequestBehavior.AllowGet);
            
        }

        public JsonResult GetAllLaptopBrands()
        {

            List<Brand> brands = new List<Brand>();
            try
            {
                DbWrappers wrap = new DbWrappers();
                 brands=wrap.GetAllBrandNamesOfLaptops();
                //brands = wrap.GetAllBrandOfMobiles();
            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                        .ReadToEnd();
            }
            return this.Json(brands, JsonRequestBehavior.AllowGet);

        }
        //public JsonResult ViewAllProducts()
        //{
        //    List<Brand> productListAgainstBrands = new List<Brand>();

        //    try
        //    {
        //        AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
        //        //ed.Configuration.ProxyCreationEnabled = false;
        //        DbWrappers wrap = new DbWrappers();
        //        productListAgainstBrands = wrap.GetAllProductsAgainstBrands();
        //    }
        //    catch (WebException wex)
        //    {
        //        var pageContent = new StreamReader(wex.Response.GetResponseStream())
        //             .ReadToEnd();
        //    }
        //    return this.Json(productListAgainstBrands, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult ProductSpecifications()
        //{
        //    List<Product> productList = new List<Product>();

        //    try
        //    {
        //        AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
        //        //ed.Configuration.ProxyCreationEnabled = false;
        //        DbWrappers wrap = new DbWrappers();
        //        productList = wrap.GetAllSpecificationsAgainstProduct();

        //    }
        //    catch (WebException wex)
        //    {
        //        var pageContent = new StreamReader(wex.Response.GetResponseStream())
        //                .ReadToEnd();
        //    }
        //    return this.Json(productList, JsonRequestBehavior.AllowGet);
        //}

        //Fariyah Code
        // GET: /Home/
        public static ManualResetEvent mre = new ManualResetEvent(false);
        //public JsonResult Index()
        //{
        //    List<Product> p = new List<Product>();
        //    try
        //    {
        //        AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
        //        ed.Configuration.ProxyCreationEnabled = false;
        //        DbWrappers wrap = new DbWrappers();
        //        p = wrap.GetAllProducts();


        //    }
        //    catch (WebException wex)
        //    {
        //        var pageContent = new StreamReader(wex.Response.GetResponseStream())
        //                .ReadToEnd();
        //    }
        //    return this.Json(p, JsonRequestBehavior.AllowGet);

        //}
        //public JsonResult ViewAllCategories()
        //{
        //    List<Category> categoryList = new List<Category>();

        //    try
        //    {
        //        AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
        //        //ed.Configuration.ProxyCreationEnabled = false;
        //        DbWrappers wrap = new DbWrappers();
        //        categoryList = wrap.GetAllCategories();

        //    }
        //    catch (WebException wex)
        //    {
        //        var pageContent = new StreamReader(wex.Response.GetResponseStream())
        //             .ReadToEnd();
        //    }
        //    return this.Json(categoryList, JsonRequestBehavior.AllowGet);

        //}
        //public JsonResult ViewAllProducts()
        //{
        //    List<Brand> productListAgainstBrands = new List<Brand>();

        //    try
        //    {
        //        AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
        //        //ed.Configuration.ProxyCreationEnabled = false;
        //        DbWrappers wrap = new DbWrappers();
        //        productListAgainstBrands = wrap.GetAllProductsAgainstBrands();
        //    }
        //    catch (WebException wex)
        //    {
        //        var pageContent = new StreamReader(wex.Response.GetResponseStream())
        //             .ReadToEnd();
        //    }
        //    return this.Json(productListAgainstBrands, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult ProductSpecifications()
        //{
        //    List<Product> productList = new List<Product>();
            
        //    try
        //    {
        //        AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
        //        //ed.Configuration.ProxyCreationEnabled = false;
        //        DbWrappers wrap = new DbWrappers();
        //        productList = wrap.GetAllSpecificationsAgainstProduct();

        //    }
        //    catch (WebException wex)
        //    {
        //        var pageContent = new StreamReader(wex.Response.GetResponseStream())
        //                .ReadToEnd();
        //    }
        //    return this.Json(productList, JsonRequestBehavior.AllowGet);
        //}
       
    }
}
