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

namespace AmazonSOAP
{
    public class ReviewCollection
    {
        public static bool hasReviews = false;
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
            Product prod= db.GetSpecificProduct(prodId);
            string []token=temp1.Split(' ');
            prod.Rating = token[0];
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
                if(hasReviews==true)
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
                    if(!hasReviews)
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

                }catch(Exception e1)
                {

                }
            }

            Console.WriteLine("done...enter any key to continue>");
            Console.ReadLine();

        }
      
    }
}
