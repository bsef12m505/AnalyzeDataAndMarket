using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
namespace AdamDal
{
    
    public class ExtractingHtml
    {
       public static AdamDatabaseEntities2 ed1=new AdamDatabaseEntities2();
       

       public static Product p = new Product();
       static List<string> positiveReviews = new List<string>();
       static List<string> negativeReviews = new List<string>();
       public static IDictionary<string, string> dictString = new Dictionary<string, string>();
       public static IDictionary<string, double> dictValue = new Dictionary<string, double>();

        public static void ExtractDetailsEbuyer()
        {
            ed1.Configuration.ProxyCreationEnabled = false;
           // FileStream fs = new FileStream("../../../Users/Hp Mobile Workstatio/Documents/Visual Studio 2013/Projects/Working/FYPAdam/AdamDal/bin/Debug/UrlEbuyer.txt", FileMode.Open);
            FileStream fs = new FileStream("../../../Users/Hp Mobile Workstatio/Documents/Visual Studio 2013/Projects/FYP DB Fix UP/FYPAdam/AdamDal/bin/Debug/UrlEbuyer.txt", FileMode.Open);
            //string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //string txtLocation = Path.Combine(resourceName, "url.txt");
            //string xsltFile = System.Net.Mime.MediaTypeNames.Application.;
            Type myType = typeof(ExtractingHtml);
            //string resourceName = "AdamDal.url.txt";
            string resourceName = string.Format("{0}.url.txt", myType.Namespace);
            Stream sr1=Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
           // var arr=sr.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.None);
           // FileStream fs = new FileStream(txtLocation, FileMode.Open);
            
           StreamReader sr = new StreamReader(fs);
            string str = "";
            int i = 1;
            while ((str = sr.ReadLine()) != null)
            {
                //checkProductID = false;         //To check if same product id is stored twice
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(str);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10; 
                //request.UseDefaultCredentials = true;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse(); //getting response

                StreamReader stream = new StreamReader(response.GetResponseStream());
                string final_response = stream.ReadToEnd();     //Page's html is now in string format
                Console.WriteLine("Extracting Data From Page:" + i);
                Console.WriteLine();
                //i++;


                ExtractingHtmlEbuyer(final_response, str);


            }
            sr.Close();
            fs.Close();

        }
       
        public static void GetReviews(string url)
        {
            var webHtml = new HtmlWeb();
            var domTree = webHtml.Load(url);

            foreach (HtmlNode div in domTree.DocumentNode.SelectNodes("//dd[@class='pros']"))
            {
                positiveReviews.Add(div.InnerText);
            }
            foreach (HtmlNode div in domTree.DocumentNode.SelectNodes("//dd[@class='cons']"))
            {
                negativeReviews.Add(div.InnerText);
            }

        }

        public static void ExtractSpecs(string spec, HtmlDocument doc, int uListi)
        {
            dictString[spec] = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[" + uListi + "]").InnerText;
            if (spec.Equals("Memory"))
            {
                try
                {
                    dictValue[spec] = double.Parse(Regex.Match(dictString[spec], @"\d+").Value);
                }
                catch (Exception)
                { };
            }
            else if (spec.Equals("HardDrive") || spec.Equals("Harddrive"))
            {
                try
                {
                    double tempHard = double.Parse(Regex.Match(dictString[spec], @"\d+").Value);
                    if (tempHard < 16)
                    {
                        tempHard = tempHard * 1024;
                    }
                    dictValue[spec] = tempHard;
                }
                catch (Exception)
                { };

            }
            else if (spec.Equals("Display"))
            {
                string temp = "";
                try
                {
                    temp = Regex.Match(dictString[spec], @"\d+[i][n]").Value;
                    dictValue[spec] = double.Parse(Regex.Match(temp, @"\d+").Value);
                }
                catch (Exception)
                {
                }
                if (temp == "")
                {
                    try
                    {
                        temp = Regex.Match(dictString[spec], @"\d+[.]*\d*[\""]+").Value;
                        dictValue[spec] = double.Parse(Regex.Match(temp, @"\d+").Value);
                    }
                    catch (Exception)
                    { };
                }


            }
            else if (spec.Equals("Dimensions"))
            {
                try
                {

                    string temp = (string)Regex.Match(dictString[spec], @"[W][e][i][g][h][t][\s][\d]*[.]*[\d]*").Value;
                    dictValue[spec] = double.Parse(Regex.Match(temp, @"\d+").Value);
                }
                catch (Exception)
                { };


            }
            else
            {
                dictValue[spec] = 0.0;
            }

        }
        public static void ExtractingHtmlEbuyer(string html, string str)
        {
            positiveReviews.Clear();
            negativeReviews.Clear();
            var webGet = new HtmlWeb();
            var doc = webGet.Load(str);
            string title = "";
            string price = "";
            string imgUrl = "";
            string description = "";
            double rating = 0.0;
            try
            {

                title = doc.DocumentNode.SelectSingleNode("//h1[@class='product-title']").InnerText;
                if (title.Contains("EXDISPLAY"))
                {
                    title = title.Replace("EXDISPLAY ", "");
                    if (title.Contains("Laptop"))
                    {
                        title = title.Substring(0, title.IndexOf(@"Laptop") + 6);
                    }
                    else if (title.Contains("Tablet"))
                    {
                        title = title.Substring(0, title.IndexOf(@"Tablet") + 6);
                    }
                    else if (title.Contains("Ultrabook"))
                    {
                        title = title.Substring(0, title.IndexOf(@"Ultrabook") + 9);
                    }
                }

                try
                {
                    price = doc.DocumentNode.SelectSingleNode("//div[@class='inc-vat']/p[1]/span[2]").InnerText;
                }
                catch (Exception)
                { }

                var tempImgUrl = doc.DocumentNode.SelectSingleNode("//div[@class='product-img-hero']//a").Descendants("img").Where(x => x.Attributes.Contains("src"));

                foreach (var link in tempImgUrl)
                {
                    imgUrl = link.Attributes["src"].Value;
                }

                try
                {
                    description = doc.DocumentNode.SelectSingleNode("//div[@class='product-info']//ul[1]//li[1]").InnerText;
                    description = description + "\n" + doc.DocumentNode.SelectSingleNode("//div[@class='product-info']//ul[1]//li[2]").InnerText;
                    description = description + "\n" + doc.DocumentNode.SelectSingleNode("//div[@class='product-info']//ul[1]//li[3]").InnerText;
                    description = description + "\n" + doc.DocumentNode.SelectSingleNode("//div[@class='product-info']//ul[1]//li[4]").InnerText;
                    description = description + "\n" + doc.DocumentNode.SelectSingleNode("//div[@class='product-info']//ul[1]//li[5]").InnerText;
                }
                catch (Exception)
                { };

                try
                {
                    var reviewLink = doc.DocumentNode.SelectSingleNode("//div[@class='button-reevoo']").Descendants("a").Where(x => x.Attributes.Contains("href"));
                    string reviewUrl = "";
                    foreach (var link in reviewLink)
                    {
                        reviewUrl = link.Attributes["href"].Value;
                    }
                    if (reviewUrl != null)
                    {
                        //Get product Number
                        int productNumber = int.Parse(Regex.Match(reviewUrl, @"\d+").Value);

                        //Load the dom tree
                        var webHtml = new HtmlWeb();
                        string reviewPageUrl = "http://mark.reevoo.com/reevoomark/en-GB/product.html?page=1&sku=" + productNumber + "&tab=reviews&trkref=EBU";
                        var domTree = webHtml.Load(reviewPageUrl);

                        //------------------------Extract rating for different features-----------------------------

                        //Feature Name
                        string temp = domTree.DocumentNode.SelectSingleNode("//table[@class='scores']//tr[1]//th[1]").InnerText;
                        temp = temp.Replace(" ", "");
                        //Feature Value
                        string value = domTree.DocumentNode.SelectSingleNode("//table[@class='scores']//tr[1]//td[1]").InnerText;

                        //Feature Name
                        temp = domTree.DocumentNode.SelectSingleNode("//table[@class='scores']//tr[2]//th[1]").InnerText;
                        temp = temp.Replace(" ", "");
                        //Feature Value
                        value = domTree.DocumentNode.SelectSingleNode("//table[@class='scores']//tr[2]//td[1]").InnerText;


                        temp = domTree.DocumentNode.SelectSingleNode("//table[@class='scores']//tr[3]//th[1]").InnerText;
                        temp = temp.Replace(" ", "");
                        value = domTree.DocumentNode.SelectSingleNode("//table[@class='scores']//tr[3]//td[1]").InnerText;


                        temp = domTree.DocumentNode.SelectSingleNode("//table[@class='scores']//tr[4]//th[1]").InnerText;
                        temp = temp.Replace(" ", "");
                        value = domTree.DocumentNode.SelectSingleNode("//table[@class='scores']//tr[4]//td[1]").InnerText;


                        temp = domTree.DocumentNode.SelectSingleNode("//table[@class='scores']//tr[5]//th[1]").InnerText;
                        temp = temp.Replace(" ", "");
                        value = domTree.DocumentNode.SelectSingleNode("//table[@class='scores']//tr[5]//td[1]").InnerText;


                        temp = domTree.DocumentNode.SelectSingleNode("//table[@class='scores']//tr[6]//th[1]").InnerText;
                        temp = temp.Replace(" ", "");
                        rating = double.Parse(domTree.DocumentNode.SelectSingleNode("//table[@class='scores']//tr[6]//td[1]").InnerText);
                        rating = rating / 2;

                        //----------------------------------Get Reviews
                        try
                        {
                            int pageNo = 1;
                            while (true)
                            {
                                GetReviews(reviewPageUrl);

                                reviewPageUrl = reviewPageUrl.Replace("page=" + pageNo, "page=" + (pageNo + 1));
                                pageNo++;

                            }

                        }
                        catch (Exception)
                        {

                        }



                    }

                    else
                    {
                        //here add the code for getting te reviews from amazon
                    }
                }
                catch (Exception)
                {
                }
                //FileStream fs = new FileStream("pReviews.txt", FileMode.Append);
                //StreamWriter sw = new StreamWriter(fs);
                //for (int i = 0; i < positiveReviews.Count; i++)
                //{
                //    sw.WriteLine(positiveReviews[i]);
                //}
                //sw.Close();
                //fs.Close();

                //fs = new FileStream("nReviews.txt", FileMode.Append);
                //sw = new StreamWriter(fs);
                //for (int i = 0; i < negativeReviews.Count; i++)
                //{
                //    sw.WriteLine(negativeReviews[i]);
                //}
                //sw.Close();
                //fs.Close();

                //Getting Specifications
                // string t=doc.DocumentNode.SelectSingleNode("//div[@class='product-description']//p[2]").InnerText;
                var p = doc.DocumentNode.Descendants().Where(n => n.GetAttributeValue("class", "").Equals("product-description")).Single();
                var paraHtml = p.InnerHtml;
                MatchCollection collectedSpec = Regex.Matches(paraHtml, "\\s*(.+?)\\s*\\n<ul>");
                int uListi = 1;


                foreach (Match m in collectedSpec)
                {

                    string spec = m.Groups[1].Value;
                    spec = spec.Replace(" ", "");
                    try
                    {
                        spec = spec.Replace("<p>", "");
                        spec = spec.Replace("</p>", "");
                        spec = spec.Replace("<span>", "");
                        spec = spec.Replace("</span>", "");


                    }
                    catch (Exception)
                    {

                    }

                    ExtractSpecs(spec, doc, uListi);
                    uListi++;

                }
                if (uListi == 1)
                {
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class='product-description']//p"))
                    {
                        string spec = node.InnerText;
                        spec = spec.Replace(" ", "");
                        if (spec.Equals("Processor") || spec.Equals("Memory") || spec.Equals("HardDrive") || spec.Equals("OperatingSystem") || spec.Equals("Software") || spec.Equals("OpticalDrive") || spec.Equals("Display") || spec.Equals("Graphics") || spec.Equals("Audio") || spec.Equals("InputDevices") || spec.Equals("Networking") || spec.Equals("PowerSupply") || spec.Equals("Dimensions") || spec.Equals("Interfaces"))
                        {
                            ExtractSpecs(spec, doc, uListi);

                            uListi++;

                        }
                    }
                }
                Product prod = new Product();
                prod.Title = title;
                prod.Price = price;
                prod.ImageUrl = imgUrl;
                prod.ProductDescription = description;
                prod.Rating = rating;

                Product_Specification ps = new Product_Specification();
                DbWrappers wrap = new DbWrappers();
                ps.ProductId = wrap.AddProductDetails(prod, "Laptop");

                //ps.SpecificationId =GetSpecificationId("HardDrive");
                //ps.Value = dictString["HardDrive"];
                //ps.NumericValue = dictValue["HardDrive"];
                //SaveSpecification(ps);

                //ps.SpecificationId = GetSpecificationId("Memory");
                //ps.Value = dictString["Memory"];
                //ps.NumericValue = dictValue["Memory"];
                //SaveSpecification(ps);

                //ps.SpecificationId = GetSpecificationId("ProcessorSpeed");
                //ps.Value = dictString["Processor"];
                //ps.NumericValue = dictValue["Processor"];
                //SaveSpecification(ps);

                //ps.SpecificationId = GetSpecificationId("OperayingSystem");
                //ps.Value = dictString["OperayingSystem"];
                //ps.NumericValue = dictValue["OperayingSystem"];
                //SaveSpecification(ps);

                //ps.SpecificationId = GetSpecificationId("OpticalDrive");
                //ps.Value = dictString["OpticalDrive"];
                //ps.NumericValue = dictValue["OpticalDrive"];
                //SaveSpecification(ps);

                //ps.SpecificationId = GetSpecificationId("Display");
                //ps.Value = dictString["Display"];
                //ps.NumericValue = dictValue["Display"];
                //SaveSpecification(ps);

                //ps.SpecificationId = GetSpecificationId("Graphics");
                //ps.Value = dictString["Graphics"];
                //ps.NumericValue = dictValue["Graphics"];
                //SaveSpecification(ps);

                //ps.SpecificationId = GetSpecificationId("Audio");
                //ps.Value = dictString["Audio"];
                //ps.NumericValue = dictValue["Audio"];
                //SaveSpecification(ps);

                List<Specification> specifications = wrap.GetAllSpecifications();

                foreach (var s in specifications)
                {
                    //Change it (Processor) in db so that this check can be removed
                    if (s.Name.Equals("ProcessorSpeed"))
                    {
                        try
                        {
                            ps.SpecificationId = s.Id;
                            ps.Value = dictString["Processor"];
                            ps.NumericValue = dictValue["Processor"];
                            wrap.SaveSpecification(ps);
                        }
                        catch (Exception)
                        { };
                    }

                        // Change Dimesnion to Dimensions in Db
                    else if (s.Name.Equals("Dimension"))
                    {
                        try
                        {
                            ps.SpecificationId = s.Id;
                            ps.Value = dictString["Dimensions"];
                            ps.NumericValue = dictValue["Dimensions"];
                            wrap.SaveSpecification(ps);
                        }
                        catch (Exception)
                        { };
                    }

                    else if (s.Name.Equals("OperatingSystem"))
                    {
                        try
                        {
                            ps.SpecificationId = s.Id;
                            ps.Value = dictString["OperatingSystem"];
                            ps.NumericValue = dictValue["OperatingSystem"];
                            wrap.SaveSpecification(ps);
                        }
                        catch (Exception)
                        {
                            if (s.Name.Equals("OperatingSystem"))
                            {
                                try
                                {
                                    ps.SpecificationId = s.Id;
                                    ps.Value = dictString["Software"];
                                    ps.NumericValue = dictValue["Software"];
                                    wrap.SaveSpecification(ps);
                                }
                                catch (Exception)
                                { };
                            }
                        };
                    }

                    else
                    {
                        try
                        {
                            ps.SpecificationId = s.Id;
                            ps.Value = dictString[s.Name];
                            ps.NumericValue = dictValue[s.Name];
                            wrap.SaveSpecification(ps);
                        }
                        catch (Exception)
                        {

                        };
                    }
                }

                ProductReview pr = new ProductReview();

                foreach (var i in positiveReviews)
                {
                    pr.ProductId = ps.ProductId;
                    pr.Review = i;
                    pr.ReviewScore = 4;
                    wrap.SaveReview(pr);

                }
                foreach (var i in negativeReviews)
                {
                    pr.ProductId = ps.ProductId;
                    pr.Review = i;
                    pr.ReviewScore = 1;
                    wrap.SaveReview(pr);

                }

                positiveReviews.Clear();
                negativeReviews.Clear();
            }
            catch (Exception)
            { };





        }
//Crawling code for GSM

        public static void ExtractingDetailsGSM()
        {
            FileStream fs = new FileStream("../../../Users/Hp Mobile Workstatio/Documents/Visual Studio 2013/Projects/Working/FYPAdam/AdamDal/bin/Debug/UrlGSM.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string str = "";
           
            int i = 1;
            while ((str = sr.ReadLine()) != null)
            {
               // checkProductID = false;         //To check if same product id is stored twice
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(str);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse(); //getting response

                StreamReader stream = new StreamReader(response.GetResponseStream());
                string final_response = stream.ReadToEnd();     //Page's html is now in string format
                Console.WriteLine("Extracting Data From Page:" + i);
                Console.WriteLine();
                i++;

                ExtractingHtmlGSM(final_response, str);

                ////Extracting only product details from the html
                //ExtractProductDetails(final_response);
                //if (checkProductID == false)
                //{
                //    //Extracting only product's Description from the complete html
                //    ExtractProductDescription(final_response);

                //    //Extracting only product's Specification from the complete html
                //    ExtractProductSpecification(final_response);
                //}


            }
            sr.Close();
            fs.Close();

        }
        public static void ExtractingHtmlGSM(string html, string str)
        {
            Product p = new Product();
            Product_Specification prodSpecs = new Product_Specification();
            DbWrappers wrapper = new DbWrappers();
            string temp = "";
            //Product Title
            Regex r = new Regex("<h1 class=\"specs-phone-name-title\">\\s*(.+?)\\s*</h1>");
            Match m = r.Match(html);
            string title = "";
            try
            {
                p.Title = m.Groups[1].Value;
            }catch(Exception)
            {

            }
           
            
            

            if (p.Title != "")
            {

                //Price
                try
                {
                    r = new Regex("<span class=\"price\">\\s*(.+?)\\s*</span>");
                    m = r.Match(html);
                    p.Price = m.Groups[1].Value;
                }catch(Exception)
                {

                }

                
                try { 
                var webGet = new HtmlWeb();
                var doc = webGet.Load(str);
                string imgUrl = "";
                string memory = "";
                string operatingSystem = "";
                string camera = "";
                string screenSize = "";

                //description
                try
                {
                    var description = doc.DocumentNode.SelectSingleNode("//div[@class='center-stage light nobg specs-accent']/ul/li").InnerText;
                    p.ProductDescription = description;

                }
                catch (Exception)
                {

                }

                try
                {
                    //Image Url

                    var links = doc.DocumentNode.SelectSingleNode("//div[@class='specs-photo-main']").Descendants("img").Where(x => x.Attributes.Contains("src"));
                    
                    foreach (var link in links)
                    {
                        p.ImageUrl = link.Attributes["src"].Value;
                    }
                }catch(Exception)
                {

                }

                //Release Date
                try
                {
                    var l = doc.DocumentNode.SelectSingleNode("//div[@id='specs-list']//table[2]//tr[2]//td[2]");
                    p.ReleaseDate = l.InnerText;
                }catch(Exception e)
                {
                }
                //Screen Size
                try
                {
                    var l = doc.DocumentNode.SelectSingleNode("//div[@id='specs-list']//table[4]//tr[2]//td[2]");
                    screenSize = l.InnerText;
                   
                }catch(Exception)
                {

                }
                try
                {
                    var l1 = doc.DocumentNode.SelectSingleNode("//div[@id='specs-list']//table[5]//tr[1]//th[1]");
                    string platform = l1.InnerText;
                    if ((platform.Equals("Platform")))
                    {
                        operatingSystem = (doc.DocumentNode.SelectSingleNode("//div[@id='specs-list']//table[5]//tr[1]//td[2]")).InnerText;
                        try
                        {
                            memory = (doc.DocumentNode.SelectSingleNode("//div[@id='specs-list']//table[6]//tr[1]//td[2]")).InnerText;
                            memory = memory + (doc.DocumentNode.SelectSingleNode("//div[@id='specs-list']//table[6]//tr[2]//td[2]")).InnerText;
                        }
                        catch (Exception)
                        { }
                    }
                    else if ((platform.Equals("Memory")))
                    {
                        try
                        {
                            memory = (doc.DocumentNode.SelectSingleNode("//div[@id='specs-list']//table[5]//tr[1]//td[2]")).InnerText;
                            memory = memory + (doc.DocumentNode.SelectSingleNode("//div[@id='specs-list']//table[5]//tr[4]//td[2]")).InnerText;
                        }
                        catch (Exception)
                        { }
                    }
                    if ((temp = (doc.DocumentNode.SelectSingleNode("//div[@id='specs-list']//table[7]//tr[1]//th[1]")).InnerText).Equals("Camera"))
                    {
                        camera = "Primary :" + (doc.DocumentNode.SelectSingleNode("//div[@id='specs-list']//table[7]//tr[1]//td[2]")).InnerText;
                        try
                        {
                            camera = camera + ", Secondary :" + (doc.DocumentNode.SelectSingleNode("//div[@id='specs-list']//table[7]//tr[4]//td[2]")).InnerText;
                        }
                        catch (Exception)
                        { }
                    }
                    else if ((temp = (doc.DocumentNode.SelectSingleNode("//div[@id='specs-list']//table[6]//tr[1]//th[1]")).InnerText).Equals("Camera"))
                    {
                        camera = "Primary: " + (doc.DocumentNode.SelectSingleNode("//div[@id='specs-list']//table[6]//tr[1]//td[2]")).InnerText;
                        try
                        {
                            camera = camera + ", Secondary: " + (doc.DocumentNode.SelectSingleNode("//div[@id='specs-list']//table[6]//tr[3]//td[2]")).InnerText;
                        }
                        catch (Exception)
                        { }
                    }

                }catch(Exception)
                {

                }
                    DbWrappers prodWrapper = new DbWrappers();
                    prodSpecs.ProductId=prodWrapper.AddProductDetails(p,"Mobile");
                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("screensize");
                    prodSpecs.Value = screenSize;
                    wrapper.SaveSpecification(prodSpecs);

                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("memory");
                    prodSpecs.Value = memory;
                    wrapper.SaveSpecification(prodSpecs);

                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("camera");
                    prodSpecs.Value = camera;
                    wrapper.SaveSpecification(prodSpecs);

                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("operatingsystem");
                    prodSpecs.Value = operatingSystem;
                    wrapper.SaveSpecification(prodSpecs);


                   

                }catch(Exception)
                {

                }
                //FileStream fs = new FileStream("data.txt", FileMode.Append);
                //StreamWriter sw = new StreamWriter(fs);
                //sw.WriteLine(title + " | " + price + " | " + releaseDate + " | " + imgUrl + " | " + screenSize + " | " + memory + " | " + operatingSystem + " | " + camera);
                //sw.Close();
                //fs.Close();
            
            }


        }

    }
}
