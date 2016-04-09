using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using System.Net;
using System.Web;

using AdamDal;
using Amazon.PAAPI;
using System.ServiceModel;

namespace AmazonSOAP
{
    public class ReviewCollection
    {
        public static bool hasReviews = false;
        public static List<string> reviewList = new List<string>();
        public static bool IsCorrect = false;
        public Dictionary<string, string> GetProdDetails(string pName)
        {
            Dictionary<string, string> proddescDic = new Dictionary<string, string>();
            string title = "";
            string manufacturer = "";
            string model = "";
            string brand = "";
            string priceInDollar = "";
            double price = 0.0;
            string[] features;
            string fea = "";
            // Instantiate Amazon ProductAdvertisingAPI client
            AWSECommerceServicePortTypeClient amazonClient = new AWSECommerceServicePortTypeClient();

            // prepare an ItemSearch request
            ItemSearchRequest request = new ItemSearchRequest();
            request.SearchIndex = "All";

            //request.Title = "WCF";

            request.Keywords = pName;
            request.ResponseGroup = new string[] { "ItemAttributes", "Offers", "OfferSummary", "Reviews" };

            ItemSearch itemSearch = new ItemSearch();
            itemSearch.Request = new ItemSearchRequest[] { request };
            itemSearch.AWSAccessKeyId = ConfigurationManager.AppSettings["accessKeyId"];
            itemSearch.AssociateTag = "newmobiles0d-20";

            // send the ItemSearch request
            try
            {
                bool getNextTitle = false;

                ItemSearchResponse response = amazonClient.ItemSearch(itemSearch);

                foreach (var item in response.Items[0].Item)
                {
                    if (!IsCorrect)
                    {
                        //Getting title
                        try
                        {
                            title = item.ItemAttributes.Title;
                            if (!(title.ToLower().Contains("Stick".ToLower())) || (title.ToLower().Contains("Case".ToLower())) || (title.ToLower().Contains("Cover".ToLower())))
                            {
                                proddescDic["Title"] = title;
                                IsCorrect = true;
                                getNextTitle = false;
                            }
                            else
                            {
                                getNextTitle = true;
                                proddescDic["Title"] = title;
                            }

                        }
                        catch (Exception)
                        { };
                        if (!getNextTitle)
                        {
                            //Getting manufacturer of the item
                            try
                            {
                                manufacturer = item.ItemAttributes.Manufacturer;
                                proddescDic["Manufacturer"] = manufacturer;
                            }
                            catch (Exception)
                            { };
                            //Getting model
                            try
                            {
                                model = item.ItemAttributes.Model;
                                proddescDic["Model"] = model;
                            }
                            catch (Exception) { };
                            //Getting Brand
                            try
                            {
                                brand = item.ItemAttributes.Brand;
                                proddescDic["Brand"] = brand;
                            }
                            catch (Exception)
                            { };
                            //Getting Amount in Dollar
                            try
                            {
                                priceInDollar = item.ItemAttributes.ListPrice.FormattedPrice;
                                proddescDic["Price"] = priceInDollar;
                            }
                            catch (Exception)
                            { };
                            //Getting Amount
                            try
                            {
                                string p = item.ItemAttributes.ListPrice.Amount;
                            }
                            catch (Exception)
                            { };
                            try
                            {
                                features = item.ItemAttributes.Feature;
                                foreach (var f in features)
                                {
                                    fea = fea + "\n" + f;
                                }
                                proddescDic["Features"] = fea;
                                Console.WriteLine(fea);
                            }
                            catch (Exception)
                            { };

                        }
                    }

                    if ((item.CustomerReviews.HasReviews) == true)
                    {
                        string str = item.CustomerReviews.IFrameURL;
                        string final_response = "";
                        try
                        {
                            //Getting html from IFrame URL
                            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(str);
                            req.Method = "POST";
                            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                            StreamReader stream = new StreamReader(res.GetResponseStream());
                            final_response = stream.ReadToEnd();

                            GetMainPageHtml(final_response);



                        }



                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }




                    }




                }

            }
            catch (Exception e1)
            {

            }

            return proddescDic;
        }
        public static void GetMainPageHtml(string res_str, int prodId)
        {

            Regex r = new Regex("<a href=\"\\s*(.+?)\\s*\" target=\"_top\">See all \\s*(.+?)\\s* customer reviews...</a>");
            string stri = r.ToString();
            if (stri != null)
            {

                GetReviewPage(r, res_str, prodId);
            }
            else
            {
                r = new Regex("<a href=\"\\s*(.+?)\\s*\" target=\"_top\">\\s*(.+?)\\s* customer reviews</a>");
                GetReviewPage(r, res_str, prodId);

            }
        }

        public static void ExtractingReviews(string url, int prodId)
        {
            url = url + "&pageNumber=1";
            string final_response = "";
            // int count = 1;
            for (int i = 1; i < 10; i++)
            {
                HttpWebRequest req1 = (HttpWebRequest)HttpWebRequest.Create(url);
                req1.Method = "POST";
                HttpWebResponse res1 = (HttpWebResponse)req1.GetResponse();
                StreamReader stream1 = new StreamReader(res1.GetResponseStream());
                final_response = stream1.ReadToEnd();
                MatchCollection collectedReviews = Regex.Matches(final_response, "<span class=\"a-size-base review-text\">\\s*(.+?)\\s*</span>");
                //FileStream fs = new FileStream("review.txt", FileMode.Append);
                //StreamWriter sw = new StreamWriter(fs);
                DbWrappers wrapper = new DbWrappers();

                foreach (Match singleReview in collectedReviews)
                {
                    string review = singleReview.Groups[1].Value;
                    ProductReview prodReview = new ProductReview();
                    prodReview.ProductId = prodId;
                    prodReview.Review = review;
                    wrapper.AddProductReviews(prodReview);
                    //sw.WriteLine(count);
                    //sw.WriteLine(review);
                    //count++;


                }
                //sw.Close();
                //fs.Close();

                url = url.Replace("pageNumber=" + i, "pageNumber=" + (i + 1));
            }
        }
        //Getting reviews and rating of the products
        public static void GetReviewPage(Regex reg_str, string resp_str, int prodId)
        {

            Match m = reg_str.Match(resp_str);
            string temp = m.Groups[1].Value;
            HttpWebRequest req1 = (HttpWebRequest)HttpWebRequest.Create(temp);
            req1.Method = "POST";
            HttpWebResponse res1 = (HttpWebResponse)req1.GetResponse();
            StreamReader stream1 = new StreamReader(res1.GetResponseStream());
            string final_response1 = stream1.ReadToEnd();
            Regex r4 = new Regex("<span class=\"arp-rating-out-of-text\">\\s*(.+?)\\s*</span>");
            Match m1 = r4.Match(final_response1);
            string temp1 = m1.Groups[1].Value;
            Console.WriteLine("Rating:  " + temp1);
            DbWrappers db = new DbWrappers();
            Product prod = db.GetSpecificProduct(prodId);
            string[] token = temp1.Split(' ');
            prod.Rating = float.Parse(token[0]); ;
            hasReviews = true;
            db.UpdateProduct(prod);

            ExtractingReviews(temp, prodId);


        }
        public void GetReviews()
        {
           
            // Instantiate Amazon ProductAdvertisingAPI client
            AWSECommerceServicePortTypeClient amazonClient = new AWSECommerceServicePortTypeClient();
           
            DbWrappers wrapper = new DbWrappers();
            List<Product> list = wrapper.GetAllProducts();
            foreach (var prod in list)
            {
                if (hasReviews == true)
                {
                    hasReviews = false;
                }
                // prepare an ItemSearch request
                ItemSearchRequest request = new ItemSearchRequest();
                request.SearchIndex = "Electronics";
                //request.Title = "WCF";

                request.Keywords = prod.Title;
                request.ResponseGroup = new string[] { "ItemAttributes", "Offers", "OfferSummary", "Reviews" };
               
                ItemSearch itemSearch = new ItemSearch();
                itemSearch.Request = new ItemSearchRequest[] { request };
                itemSearch.AWSAccessKeyId = ConfigurationManager.AppSettings["accessKeyId"];
                itemSearch.AssociateTag = "newmobiles0d-20";

                // send the ItemSearch request
                try
                {

                    ItemSearchResponse response = amazonClient.ItemSearch(itemSearch);
                 
                    foreach (var item in response.Items[0].Item)
                    {
                        if (!hasReviews)
                        {
                            // Getting title
                            Console.WriteLine("Title: " + item.ItemAttributes.Title);
                            //Getting manufacturer of the item
                            Console.WriteLine("Manufacturer: " + item.ItemAttributes.Manufacturer);
                            //Getting model
                            Console.WriteLine("Model:" + item.ItemAttributes.Model);


                            if ((item.CustomerReviews.HasReviews) == true)
                            {
                                string str = item.CustomerReviews.IFrameURL;
                                string final_response = "";
                                try
                                {
                                    //Getting html from IFrame URL
                                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(str);
                                    req.Method = "POST";
                                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                                    StreamReader stream = new StreamReader(res.GetResponseStream());
                                    final_response = stream.ReadToEnd();

                                    GetMainPageHtml(final_response, prod.Id);



                                }



                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }




                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }
                catch (Exception e1)
                {

                }
            }

            Console.WriteLine("done...enter any key to continue>");
            Console.ReadLine();

        }
        public static bool GetReview = false;
        public static int reviewLoopIterator = 1;
        //geting products for sending mail to the user
        public Dictionary<string, string> GetProductAgainstHashTag(string prodName)
        {
            Dictionary<string, string> proddescDic = new Dictionary<string, string>();
            string title = "";
            string manufacturer = "";
            string model = "";
            string brand = "";
            string priceInDollar = "";
            double price = 0.0;
            string[] features;
            // Instantiate Amazon ProductAdvertisingAPI client
            AWSECommerceServicePortTypeClient amazonClient = new AWSECommerceServicePortTypeClient();
            BasicHttpBinding httpBinding = new BasicHttpBinding();
            httpBinding.Name = "AWSECommerceServiceBindingNoTransport";
            httpBinding.MaxReceivedMessageSize = 2147483647;
            httpBinding.MaxBufferSize = 2147483647;
            // prepare an ItemSearch request
            ItemSearchRequest request = new ItemSearchRequest();
            //request.SearchIndex = "Electronics";
            request.SearchIndex = "All";
            //request.Title = "WCF";

            request.Keywords = prodName;
            request.ResponseGroup = new string[] { "ItemAttributes", "Offers", "OfferSummary", "Reviews" };

            ItemSearch itemSearch = new ItemSearch();
            itemSearch.Request = new ItemSearchRequest[] { request };
            itemSearch.AWSAccessKeyId = ConfigurationManager.AppSettings["accessKeyId"];
            itemSearch.AssociateTag = "newmobiles0d-20";

            // send the ItemSearch request
            try
            {

                ItemSearchResponse response = amazonClient.ItemSearch(itemSearch);

                foreach (var item in response.Items[0].Item)
                {
                    if (!GetReview)
                    {
                        //Getting title
                        try
                        {
                            title = item.ItemAttributes.Title;
                            proddescDic["Title"] = title;
                            if (!(title.Equals("")))
                            {
                                GetReview = true;
                            }
                        }
                        catch (Exception)
                        { };
                        //Getting manufacturer of the item
                        try
                        {
                            manufacturer = item.ItemAttributes.Manufacturer;
                            proddescDic["Manufacturer"] = manufacturer;
                        }
                        catch (Exception)
                        { };
                        //Getting model
                        try
                        {
                            model = item.ItemAttributes.Model;
                            proddescDic["Model"] = model;
                        }
                        catch (Exception) { };
                        //Getting Brand
                        try
                        {
                            brand = item.ItemAttributes.Brand;
                            proddescDic["Brand"] = brand;
                        }
                        catch (Exception)
                        { };
                        //Getting Amount in Dollar
                        try
                        {
                            priceInDollar = item.ItemAttributes.ListPrice.FormattedPrice;
                            proddescDic["Price"] = priceInDollar;
                        }
                        catch (Exception)
                        { };
                        //Getting Amount
                        try
                        {
                            price = int.Parse(item.ItemAttributes.ListPrice.Amount);
                        }
                        catch (Exception)
                        { };
                        try
                        {
                            features = item.ItemAttributes.Feature;
                            string fea = "";
                            foreach (var f in features)
                            {

                                fea = fea + "\n" + f;
                            }
                            proddescDic["Features"] = fea;

                        }
                        catch (Exception)
                        { };

                    }
                    if (reviewLoopIterator < 5)
                    {
                        if ((item.CustomerReviews.HasReviews) == true)
                        {
                            reviewLoopIterator++;
                            string str = item.CustomerReviews.IFrameURL;
                            string final_response = "";
                            try
                            {
                                //Getting html from IFrame URL
                                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(str);
                                req.Method = "POST";
                                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                                StreamReader stream = new StreamReader(res.GetResponseStream());
                                final_response = stream.ReadToEnd();

                                GetMainPageHtml(final_response);



                            }



                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }




                        }
                    }
                    else
                    {
                        break;
                    }


                }

                return proddescDic;

            }
            catch (Exception e1)
            {
                return proddescDic;
            }




        }

        public static void GetMainPageHtml(string res_str)
        {

            Regex r = new Regex("<a href=\"\\s*(.+?)\\s*\" target=\"_top\">See all \\s*(.+?)\\s* customer reviews...</a>");
            string stri = r.ToString();
            if (stri != null)
            {

                GetReviewPage(r, res_str);
            }
            else
            {
                r = new Regex("<a href=\"\\s*(.+?)\\s*\" target=\"_top\">\\s*(.+?)\\s* customer reviews</a>");
                GetReviewPage(r, res_str);

            }
        }

        public static void GetReviewPage(Regex reg_str, string resp_str)
        {

            Match m = reg_str.Match(resp_str);
            string temp = m.Groups[1].Value;
            HttpWebRequest req1 = (HttpWebRequest)HttpWebRequest.Create(temp);
            req1.Method = "POST";
            HttpWebResponse res1 = (HttpWebResponse)req1.GetResponse();
            StreamReader stream1 = new StreamReader(res1.GetResponseStream());
            string final_response1 = stream1.ReadToEnd();
            Regex r4 = new Regex("<span class=\"arp-rating-out-of-text\">\\s*(.+?)\\s*</span>");
            Match m1 = r4.Match(final_response1);
            string temp1 = m1.Groups[1].Value;
            Console.WriteLine("Rating:  " + temp1);
            //DbWrappers db = new DbWrappers();
            //Product prod = db.GetSpecificProduct(prodId);
            //string[] token = temp1.Split(' ');
            //prod.Rating = float.Parse(token[0]); ;
            //hasReviews = true;
            //db.UpdateProduct(prod);

            ExtractingReviews(temp);


        }

        public static void ExtractingReviews(string url)
        {
            url = url + "&pageNumber=1";
            string final_response = "";
            // int count = 1;
            for (int i = 1; i < 10; i++)
            {
                HttpWebRequest req1 = (HttpWebRequest)HttpWebRequest.Create(url);
                req1.Method = "POST";
                HttpWebResponse res1 = (HttpWebResponse)req1.GetResponse();
                StreamReader stream1 = new StreamReader(res1.GetResponseStream());
                final_response = stream1.ReadToEnd();
                MatchCollection collectedReviews = Regex.Matches(final_response, "<span class=\"a-size-base review-text\">\\s*(.+?)\\s*</span>");
                //FileStream fs = new FileStream("review.txt", FileMode.Append);
                //StreamWriter sw = new StreamWriter(fs);
                DbWrappers wrapper = new DbWrappers();

                foreach (Match singleReview in collectedReviews)
                {
                    string review = singleReview.Groups[1].Value;
                    reviewList.Add(review);

                    //ProductReview prodReview = new ProductReview();
                    //prodReview.ProductId = prodId;
                    //prodReview.Review = review;
                    //wrapper.AddProductReviews(prodReview);
                    //sw.WriteLine(count);
                    //sw.WriteLine(review);
                    //count++;


                }
                //sw.Close();
                //fs.Close();

                url = url.Replace("pageNumber=" + i, "pageNumber=" + (i + 1));
            }

        }

    }
}
