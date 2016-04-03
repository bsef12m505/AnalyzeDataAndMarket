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

        public JsonResult DownloadImage(string imageUrl)
        {
            ImageUtitlites.imgUrl = imageUrl;
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(imageUrl, "../../../..Users/Hp Mobile Workstatio/Documents/Visual Studio 2013/Projects/FYP DB Fix UP/FYPAdam/FYPAdam/img/chart1.png");
            }

            return this.Json("",JsonRequestBehavior.AllowGet);

        }
        public ActionResult DrawChart(Dictionary<string, int> negFeatures, Dictionary<string, int> posFeatures)
        {
            List<string> colorList = new List<string>();
            colorList.Add("color: #9d426b");
            colorList.Add("color: #3fb0e9");
            colorList.Add("color: #42c698");
            colorList.Add("color: red");
            colorList.Add("color: gold");
            colorList.Add("color: blue");
            colorList.Add("color: red");
            colorList.Add("color: yellow");
            colorList.Add("color: gold");
            ViewBag.color = colorList;
            ViewBag.negativefea = negFeatures.Keys.ToArray();
            ViewBag.negativefeaCount = negFeatures.Values.ToArray();
            ViewBag.positivefea = posFeatures.Keys.ToArray();
            ViewBag.positivefeaCount = posFeatures.Values.ToArray();
            ViewBag.GetChart = true;
            return View();
        }
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

        public JsonResult RelatedProducts(string RelatedProdName, string RelatedProdId)
        {
            int prodId = int.Parse(RelatedProdId);
            List<Product> productList = new List<Product>();
            try
            {
                DbWrappers wrap = new DbWrappers();
                productList = wrap.GetRelatedProducts(RelatedProdName, prodId);

            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                        .ReadToEnd();
            }
            return this.Json(productList, JsonRequestBehavior.AllowGet);
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

        public JsonResult ProductRefinedFeatures(int pId)
        {
            List<FeatureSentiment> featureList = new List<FeatureSentiment>();

            try
            {
                DbWrappers wrap = new DbWrappers();
                featureList = wrap.GetRefinedFeaturesOfProduct(pId);

            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                        .ReadToEnd();
            }
            return this.Json(featureList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AllProductTtiles()
        {
            List<string> productNames = new List<string>();
            try
            {
                DbWrappers wrap = new DbWrappers();
                productNames = wrap.GetAllProductTitles();

            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                        .ReadToEnd();
            }
            return this.Json(productNames, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CompareProducts(string prodName1, string prodName2)
        {
            List<Product> prodList = new List<Product>();
            try
            {
                DbWrappers wrap = new DbWrappers();
                prodList = wrap.CompareProduct(prodName1, prodName2);


            }
            catch (Exception)
            { };
            return this.Json(prodList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchedProduct(string name)
        {
            Product p = new Product();
            try
            {
                DbWrappers wrap = new DbWrappers();
                p = wrap.SearchedProduct(name);
                return this.Json(p, JsonRequestBehavior.AllowGet);

            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                        .ReadToEnd();
                return this.Json(p, JsonRequestBehavior.AllowGet);
            }

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
                brands = wrap.GetAllBrandNamesOfLaptops();
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
