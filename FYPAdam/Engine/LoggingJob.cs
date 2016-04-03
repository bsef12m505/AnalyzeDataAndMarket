using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz.Impl;
using Quartz;
using System.Net.Mail;
using System.Net;
using AdamDal;
using System.IO;
using AmazonSOAP;
using Twitter;
using TextMining;
using System.Data;
using System.Web.Mvc;
using Engine.Controllers;

namespace Engine
{
    public class LoggingJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {

                FileStream fs = new FileStream("../../UrlGSM1.txt", FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("Hello World");
                sw.Close();
                fs.Close();
                FileStream fs1 = new FileStream("../../UrlGSM1.txt", FileMode.Open);
                string str="";
                string str1 = "";
               
                StreamReader sr = new StreamReader(fs1);
                while ((str = sr.ReadLine()) != null)
                {
                    str1 = str;
                    break;
                }
                sr.Close();
                fs1.Close();
                //Checking the Time Duration of Servive with Email
                using (var message = new MailMessage("adamanalyzer@gmail.com", "adamtestreceiver@gmail.com"))
                {
                    message.Subject = str1+"";
                    message.Body = "Service is being started at " + DateTime.Now;
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
            }catch(Exception )
            {

            }


            //-------Geting Tweets
            var twitter = new Program
                {
                    OAuthConsumerKey = "OAuth Consumer Key",
                    OAuthConsumerSecret = "OAuth Consumer Secret"
                };

            
           // ReviewRefinement refinement = new ReviewRefinement();
            

            //List<dynamic> Tweets_HashTag = twitter.GetTwitts_HashTags("samsung").Result; //fetchng top 100 tweets related to a hash tag
            //twitter.GetFollowersCount();

            // ---Set Abot to Crawl Links-----

            //CrawlingLinks links = new CrawlingLinks();
            //var otherController = DependencyResolver.Current.GetService<HomeController>();
            //var result = otherController.DrawChart();
          // CrawlingLinks.StartCrawlEbuyer("http://www.ebuyer.com/store/Computer/cat/Laptops");


            //FileStream fs1 = new FileStream("../../../Users/Hp Mobile Workstatio/Documents/Visual Studio 2013/Projects/Working/FYPAdam/AdamDal/bin/Debug/gsm.txt", FileMode.Open);
            //StreamReader sr1 = new StreamReader(fs1);
            //string str1 = "";
            //while ((str1 = sr1.ReadLine()) != null)
            //{
            //    CrawlingLinks.StartCrawlGSM(str1);
            //    break;
            //}

            //ReviewCollection col = new ReviewCollection();
            //col.GetReviews();
            //ExtractingHtml.ExtractDetailsEbuyer();
                
            //}

            ///email job
            ///


         

        }
    }
}