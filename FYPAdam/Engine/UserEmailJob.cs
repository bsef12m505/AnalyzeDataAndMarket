using AdamDal;
using AmazonSOAP;

using Engine.Controllers;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using System.Web;
using System.Web.Mvc;
using TextMining;
using Twitter;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;

namespace Engine
{
    public class UserEmailJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            List<Customer_AreaOfInterest> custInterrest = new List<Customer_AreaOfInterest>();

            var twitter = new Program
            {
                OAuthConsumerKey = "OAuth Consumer Key",
                OAuthConsumerSecret = "OAuth Consumer Secret"
            };
            //Refinement refinedReviews = new Refinement();

            ReviewCollection col = new ReviewCollection();
            

            try
            {
                DbWrappers wrap = new DbWrappers();
                custInterrest = wrap.GetAllCustomersInterest();
                List<Brand> b = wrap.GetAllBrands();
                foreach (var cust in custInterrest)
                {
                    foreach (var brand in b)
                    {
                        if (cust.AreaOfInterest.Equals(brand.Name))
                        {
                            List<dynamic> Tweets_HashTag = twitter.GetTweetsUserName(brand.UserName, 25).Result; //fetchng top 100 tweets related to a hash tag
                            DataTable hashtagTable = twitter.add_HashTags(Tweets_HashTag);//populating the data table
                            List<dynamic> refinedTags=twitter.Frequency_Analysis_HashTag(hashtagTable); //frequeny analysis 
                            foreach(var prodName in refinedTags)
                            {
                             Dictionary<string,string> prodDesc= col.GetProductAgainstHashTag(prodName);
                             List<string> reviews=ReviewCollection.reviewList;
                             var arr = prodDesc.Keys.ToArray();
                             var prodValues = prodDesc.Values.ToArray();
                            test.GetSentimentandNouns(reviews);
                            Dictionary<string, int> posFeatures = test.pfeatureDictionary;
                            Dictionary<string, int> negFeatures = test.nfeatureDictionary;
                            var otherController = DependencyResolver.Current.GetService<HomeController>();
                            var result = otherController.DrawChart(negFeatures,posFeatures);

                            using (var message = new MailMessage("adamanalyzer@gmail.com", "adamtestreceiver@gmail.com"))
                            {
                                message.Subject = prodName+"Analysis Report";
                                // Construct the alternate body as HTML.
                                string body = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
                                body += "<HTML><HEAD><META http-equiv=Content-Type content=\"text/html; charset=iso-8859-1\">";
                                body += "</HEAD><BODY><DIV><FONT face=Arial color=#000000 size=3>" + prodName + "<br/><p >" + arr[0] + " :" + prodValues[0] + "</p>" + "<br/><p >" + arr[1] + " :" + prodValues[1] + "</p>" + "<br/><p >" + arr[2] + " :" + prodValues[2] + "</p>" + "<br/><p >" + arr[3] + " :" + prodValues[3] + "</p>" + "<br/><p >" + arr[4] + " :" + prodValues[4] + "</p>" + "<br/><p >" + arr[5] + " :" + prodValues[5] + "</p>";
                                body += "</FONT><IMG src=\"" + ImageUtitlites.imgUrl + "\">  </DIV>";
                                body += "</BODY></HTML>";

                                ContentType mimeType = new System.Net.Mime.ContentType("text/html");
                                // Add the alternate body to the message.

                                AlternateView alternate = AlternateView.CreateAlternateViewFromString(body, mimeType);
                                message.AlternateViews.Add(alternate);

                                using (SmtpClient client = new SmtpClient
                                {
                                    EnableSsl = true,
                                    Host = "smtp.gmail.com",
                                    Port = 587,
                                    Credentials = new NetworkCredential("adamanalyzer@gmail.com", "analyzedataandmarket")
                                })
                                {
                                    client.Send(message);
                                }

                            }    



                            }

                        }
                    }
                }

            }
            catch (Exception)
            {

            }
        }
    }
}