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

        public static void ExtractDetailsEbuyer()
        {
            ed1.Configuration.ProxyCreationEnabled = false;
            FileStream fs = new FileStream("../../../Users/Hp Mobile Workstatio/Documents/Visual Studio 2013/Projects/Working/FYPAdam/AdamDal/bin/Debug/UrlEbuyer.txt", FileMode.Open);
            
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
        public static void ExtractingHtmlEbuyer(string html, string str)
        {
            Product_Specification prodSpecs = new Product_Specification();
            ed1.Configuration.ProxyCreationEnabled = false;
            //try
            //{
                var webGet = new HtmlWeb();
                var doc = webGet.Load(str);


                try
                {


                    string title = doc.DocumentNode.SelectSingleNode("//h1[@class='product-title']").InnerText;
                    p.Title = title;

                    try
                    {
                        string price = doc.DocumentNode.SelectSingleNode("//div[@class='inc-vat']/p[1]/span[2]").InnerText;
                        p.Price = price;
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        var tempImgUrl = doc.DocumentNode.SelectSingleNode("//div[@class='product-img-hero']//a").Descendants("img").Where(x => x.Attributes.Contains("src"));
                        string imgUrl = "";

                        foreach (var link in tempImgUrl)
                        {
                            imgUrl = link.Attributes["src"].Value;
                            p.ImageUrl = imgUrl;
                        }
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        string description = doc.DocumentNode.SelectSingleNode("//div[@class='product-info']//ul[1]//li[1]").InnerText;
                        description = description + "\n" + doc.DocumentNode.SelectSingleNode("//div[@class='product-info']//ul[1]//li[2]").InnerText;
                        description = description + "\n" + doc.DocumentNode.SelectSingleNode("//div[@class='product-info']//ul[1]//li[3]").InnerText;
                        description = description + "\n" + doc.DocumentNode.SelectSingleNode("//div[@class='product-info']//ul[1]//li[4]").InnerText;
                        description = description + "\n" + doc.DocumentNode.SelectSingleNode("//div[@class='product-info']//ul[1]//li[5]").InnerText;

                        p.ProductDescription = description;
                    }
                    catch (Exception e)
                    {

                    }
                    string processorSpeed = "";
                    string memory = "";
                    string hardDrive = "";
                    string opticalDrive = "";
                    string operatingSystem = "";
                    string display = "";
                    string graphics = "";
                    string audio = "";
                    string inputDevices = "";
                    string networking = "";
                    string powerSupply = "";
                    string dimesnion = "";
                    string interfaces = "";
                    //Getting Specifications
                    if (title.Contains("ASUS") || title.Contains("Asus") || title.Contains("asus"))
                    {
                        //processorSpeed = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[1]").InnerText;
                        //memory = doc.DocumentNode.SelectSingleNode(("//div[@class='product-description']/ul[2]")).InnerText;
                        //hardDrive = doc.DocumentNode.SelectSingleNode(("//div[@class='product-description']/ul[3]")).InnerText;
                        //opticalDrive = doc.DocumentNode.SelectSingleNode(("//div[@class='product-description']/ul[4]")).InnerText;
                        //operatingSystem = doc.DocumentNode.SelectSingleNode(("//div[@class='product-descr iption']/ul[5]")).InnerText;
                        //display = doc.DocumentNode.SelectSingleNode(("//div[@class='product-description']/ul[7]")).InnerText;
                        //graphics = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[8]").InnerText;
                        //audio = doc.DocumentNode.SelectSingleNode(("//div[@class='product-description']/ul[9]")).InnerText;
                        //inputDevices = doc.DocumentNode.SelectSingleNode(("//div[@class='product-description']/ul[10]")).InnerText;
                        //networking = doc.DocumentNode.SelectSingleNode(("//div[@class='product-description']/ul[11]")).InnerText;
                        //powerSupply = doc.DocumentNode.SelectSingleNode(("//div[@class='product-description']/ul[12]")).InnerText;
                        //dimesnion = doc.DocumentNode.SelectSingleNode(("//div[@class='product-description']/ul[13]")).InnerText;
                        //interfaces = doc.DocumentNode.SelectSingleNode(("//div[@class='product-description']/ul[14]")).InnerText;

                        try
                        {
                            processorSpeed = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[1]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }
                        try
                        {
                            memory = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[2]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            hardDrive = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[3]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            opticalDrive = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[4]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            operatingSystem = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[5]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            display = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[6]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            graphics = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[7]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            audio = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[8]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            inputDevices = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[9]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            networking = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[10]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            powerSupply = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[11]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            dimesnion = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[12]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            interfaces = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[13]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }





                    }
                    else
                    {
                        try
                        {
                            processorSpeed = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[1]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }
                        try
                        {
                            memory = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[2]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            hardDrive = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[3]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            opticalDrive = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[4]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            operatingSystem = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[5]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            display = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[6]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            graphics = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[7]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            audio = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[8]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            inputDevices = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[9]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            networking = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[10]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            powerSupply = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[11]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            dimesnion = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[12]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            interfaces = doc.DocumentNode.SelectSingleNode("//div[@class='product-description']/ul[13]").InnerText;
                        }
                        catch (Exception e)
                        {

                        }




                    }

                    DbWrappers wrapper = new DbWrappers();
                    prodSpecs.ProductId=wrapper.AddProductDetails(p,"Laptop");
                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("HardDrive");
                    prodSpecs.Value =hardDrive;
                    wrapper.SaveSpecification(prodSpecs);

                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("Memory");
                    prodSpecs.Value = memory;
                    wrapper.SaveSpecification(prodSpecs);

                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("ProcessorSpeed");
                    prodSpecs.Value = processorSpeed;
                    wrapper.SaveSpecification(prodSpecs);


                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("OperayingSystem");
                    prodSpecs.Value = operatingSystem;
                    wrapper.SaveSpecification(prodSpecs);

                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("OpticalDrive");
                    prodSpecs.Value = opticalDrive;
                    wrapper.SaveSpecification(prodSpecs);

                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("Display");
                    prodSpecs.Value = display;
                    wrapper.SaveSpecification(prodSpecs);

                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("Graphics");
                    prodSpecs.Value = graphics;
                    wrapper.SaveSpecification(prodSpecs);


                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("Audio");
                    prodSpecs.Value = audio;
                    wrapper.SaveSpecification(prodSpecs);

                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("InputDevices");
                    prodSpecs.Value = inputDevices;
                    wrapper.SaveSpecification(prodSpecs);

                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("Networking");
                    prodSpecs.Value = networking;
                    wrapper.SaveSpecification(prodSpecs);

                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("PowerSupply");
                    prodSpecs.Value = powerSupply;
                    wrapper.SaveSpecification(prodSpecs);


                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("Dimension");
                    prodSpecs.Value = dimesnion;
                    wrapper.SaveSpecification(prodSpecs);

                    prodSpecs.SpecificationId = wrapper.GetSpecificationId("Interfaces");
                    prodSpecs.Value = interfaces;
                    wrapper.SaveSpecification(prodSpecs);
                }
                catch (Exception e)
                {

                }



                




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
