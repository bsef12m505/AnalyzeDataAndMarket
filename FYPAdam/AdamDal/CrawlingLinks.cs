using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Abot.Poco;
using Abot.Crawler;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using AutoMapper;

namespace AdamDal
{
   public class CrawlingLinks
    {

        public static void StartCrawlEbuyer(string url)
        {
            try
            {
                PoliteWebCrawler crawler = new PoliteWebCrawler();
                crawler.PageCrawlStartingAsync +=crawler_ProcessPageCrawlStarting;
                crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
                crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
                crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;


                TimeSpan ts = new TimeSpan(0, 0, 5);
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(ts);
                CrawlResult result =crawler.Crawl(new Uri(url), cancellationTokenSource);

                if (result.ErrorOccurred)
                    Console.WriteLine("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
                else
                    Console.WriteLine("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri);
            }catch(Exception)
            {

            }
          ExtractingHtml.ExtractDetailsEbuyer();

        }


        //static void Main(string[] args)
        //{
        //    FileStream fs = new FileStream("ebuyer.txt", FileMode.Open);
        //    StreamReader sr = new StreamReader(fs);
        //    string str = "";
        //    while ((str = sr.ReadLine()) != null)
        //    {
        //        StartCrawl(str);
        //    }


        //    //DisplayDetails();








        //}
        public static void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            try
            {
                PageToCrawl pageToCrawl = e.PageToCrawl;

                string url = pageToCrawl.ToString();
                if ((url.Contains("apple") || url.Contains("hp") || url.Contains("lenovo") || url.Contains("asus") || url.Contains("dell") || url.Contains("acer")) && url.Contains("-"))
                {
                    FileStream fs = new FileStream("../../../Users/Hp Mobile Workstatio/Documents/Visual Studio 2013/Projects/Working/FYPAdam/AdamDal/bin/Debug/UrlEbuyer.txt", FileMode.Append);
                    //FileStream fs = new FileStream(@"~\AdamDal\bin\Debug\url.txt", FileMode.Append);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(url);
                    sw.Close();
                    fs.Close();

                }

                Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
            }catch(Exception )
            {

            }
        }

        public static void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            try
            {
                CrawledPage crawledPage = e.CrawledPage;

                if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                    Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
                else
                    Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);

                if (string.IsNullOrEmpty(crawledPage.Content.Text))
                    Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
            }catch(Exception)
            {

            }
        }

        public static void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            try
            {
                CrawledPage crawledPage = e.CrawledPage;
                Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
            }catch(Exception)
            {
                
            }
           
        }

        public static void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            try
            {
                PageToCrawl pageToCrawl = e.PageToCrawl;
                Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
            }catch (Exception)
            {

            }
        }



//Crawling code for GSM
        public static void StartCrawlGSM(string url)
        {
            PoliteWebCrawler crawler = new PoliteWebCrawler();
            crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStartingGSM;
            crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompletedGSM;
            crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowedGSM;
            crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowedGSM;


            TimeSpan ts = new TimeSpan(0, 0 , 0);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(ts);
            CrawlResult result = crawler.Crawl(new Uri(url), cancellationTokenSource);

            if (result.ErrorOccurred)
                Console.WriteLine("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
            else
                Console.WriteLine("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri);


            //FileStream fs = new FileStream("url.txt", FileMode.Open);
            //StreamReader sr = new StreamReader(fs);
            //string str = "";
            //while ((str = sr.ReadLine()) != null)
            //{
            //    StartCrawl(str);
            //}

            ExtractingHtml.ExtractingDetailsGSM();

        }
        //static void Main(string[] args)
        //{
        //    FileStream fs = new FileStream("gsm.txt", FileMode.Open);
        //    StreamReader sr = new StreamReader(fs);
        //    string str = "";
        //    while ((str = sr.ReadLine()) != null)
        //    {
        //        StartCrawl(str);
        //    }
        //    //StartCrawl("http://www.gsmarena.com/huawei-phones-58.php");

        //    //DisplayDetails();

        //    /*This is the code with which Abot Crawl Links/HyperLinks from some Specific Website*/
        //    /*Abot Crawler does Depth Crawling that is it jumps from one hyper link to another*/
        //    /*I have Crawled links of iphone6 and stored them in a file*/
        //    /*This code is commented because once Abot start Crawling it won't stop. It may take several hours*/
        //    /*Can be uncommented to verify*/






        //}
        static void crawler_ProcessPageCrawlStartingGSM(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;

            string url = pageToCrawl.ToString();
            if ((url.Contains("samsung") || url.Contains("apple") || url.Contains("microsoft") || url.Contains("nokia") || url.Contains("sony") || url.Contains("lg") || url.Contains("htc") || url.Contains("motorola") || url.Contains("huawei") || url.Contains("blackberry") || url.Contains("lenovo") || url.Contains("oppo") || url.Contains("lava")) && (url.Contains("_") && (!(url.Contains("pictures"))) && (!(url.Contains("reviews"))) && (!(url.Contains("review")))))
            {
                FileStream fs = new FileStream("../../../Users/Hp Mobile Workstatio/Documents/Visual Studio 2013/Projects/Working/FYPAdam/AdamDal/bin/Debug/UrlGSM.txt", FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(url);
                sw.Close();
                fs.Close();

            }

            Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
        }

        static void crawler_ProcessPageCrawlCompletedGSM(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
            else
                Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
        }

        static void crawler_PageLinksCrawlDisallowedGSM(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }

        static void crawler_PageCrawlDisallowedGSM(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }
    }
}
