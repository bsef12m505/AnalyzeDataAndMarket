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

namespace Engine
{
    public class LoggingJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                //Checking the Time Duration of Servive with Email
                using (var message = new MailMessage("fariyah129@gmail.com", "bsef12m505@pucit.edu.pk"))
                {
                    message.Subject = "Service";
                    message.Body = "Service is being started at " + DateTime.Now;
                    using (SmtpClient client = new SmtpClient
                    {
                        EnableSsl = true,
                        Host = "smtp.gmail.com",
                        Port = 587,
                        Credentials = new NetworkCredential("fariyah129@gmail.com", "03244047800f")
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


            //List<dynamic> Tweets_HashTag = twitter.GetTwitts_HashTags("samsung").Result; //fetchng top 100 tweets related to a hash tag
            //twitter.GetFollowersCount();

            // ---Set Abot to Crawl Links-----

            CrawlingLinks links = new CrawlingLinks();

         //   CrawlingLinks.StartCrawlEbuyer("http://www.ebuyer.com/store/Computer/cat/Laptops");


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

        }
    }
}