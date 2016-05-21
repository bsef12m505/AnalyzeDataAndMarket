using AdamDal;
using AmazonSOAP;
using System.Windows.Forms;
using System.Diagnostics;
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
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using System.Web.Hosting;


namespace Engine
{
    public class UserEmailJob : IJob
    {
        public static string enginePath= "";
        public void Execute(IJobExecutionContext context)
        {
            enginePath = HostingEnvironment.MapPath(@"~/packet-files/models");
            List<Customer_AreaOfInterest> custInterrest = new List<Customer_AreaOfInterest>();
            List<Color> colorList = new List<Color>();
            colorList.Add(Color.Gold);
            colorList.Add(Color.Red);
            colorList.Add(Color.YellowGreen);
            colorList.Add(Color.Purple);
            colorList.Add(Color.MediumVioletRed);
            colorList.Add(Color.DarkCyan);
            colorList.Add(Color.Blue);
            //colorList.Add(Color.Yellow);
            colorList.Add(Color.Orange);
            colorList.Add(Color.HotPink);
            colorList.Add(Color.RosyBrown);
            colorList.Add(Color.Indigo);
            colorList.Add(Color.Orange);
            colorList.Add(Color.MediumVioletRed);
            var twitter = new Program
            {
                OAuthConsumerKey = "OAuth Consumer Key",
                OAuthConsumerSecret = "OAuth Consumer Secret"
            };
            //Refinement refinedReviews = new Refinement();

            ReviewCollection col = new ReviewCollection();

            Dictionary<string, int> posFeatures;
            Dictionary<string, int> negFeatures;
            
            

            try
            {
                DbWrappers wrap = new DbWrappers();
                custInterrest = wrap.GetAllCustomersInterest();//fetching the interest of all the customers 
                List<Brand> b = wrap.GetAllBrands();//geting all the brands coz we need them for twitter username
                foreach (var cust in custInterrest)
                {
                    foreach (var brand in b)
                    {
                        if (cust.AreaOfInterest.Equals(brand.Name))
                        {
                            List<dynamic> Tweets_HashTag = twitter.GetTweetsUserName(brand.UserName, 25).Result; //fetchng top 25 tweets related to a brand
                            DataTable hashtagTable = twitter.add_HashTags(Tweets_HashTag);//populating the data table with the hashtags extracted from tweets
                            List<dynamic> refinedTags=twitter.Frequency_Analysis_HashTag(hashtagTable); //Extracting the relevent hashtags(i.e tags that are a product name)
                            foreach(var prodName in refinedTags)
                            {


                             //Dictionary<string,string> prodDesc= col.GetProductAgainstHashTag(prodName);//geting the product details of the top products (extracted using twitter) from amazon 
                             Dictionary<string, string> prodDesc = col.GetProdDetails(prodName);
                             List<string> reviews=ReviewCollection.reviewList; //Reviews of the related product
                             var arr = prodDesc.Keys.ToArray();
                             var prodValues = prodDesc.Values.ToArray();
                             test.GetSentimentandNouns(reviews, enginePath);  //extracting positive and negative features from reviews
                             posFeatures = test.pfeatureDictionary;
                             negFeatures = test.nfeatureDictionary;

                            //making bar chart from positive features
                            Chart barchart = new Chart();
                            barchart.Size = new Size(1200, 500);//size f te bar chart
                            // barchart.BackImage = @"C:\Users\Hp Mobile Workstatio\Documents\Visual Studio 2013\Projects\New folder\FYPAdam\Engine\Charts\download.jpg";
                            ChartArea area = new ChartArea();
                            barchart.ChartAreas.Add(area);
                            barchart.BackColor = Color.Transparent;
                            barchart.ChartAreas[0].BackColor = Color.Transparent;
                            barchart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                            barchart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                            barchart.ChartAreas[0].AxisX.Title = "Positive Features";//title on the x-axis
                            barchart.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Center;
                            barchart.ChartAreas[0].AxisX.TextOrientation = TextOrientation.Horizontal;
                            barchart.ChartAreas[0].AxisX.Interval = 1;
                            barchart.ChartAreas[0].AxisY.LabelAutoFitStyle = LabelAutoFitStyles.None;
                            barchart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 2.25F, System.Drawing.FontStyle.Bold);
                            barchart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 13.25F, System.Drawing.FontStyle.Bold);
                            barchart.ChartAreas[0].AxisY.Title = "Count";//title on the y-axis
                            barchart.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Center;
                            barchart.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Horizontal;
                            barchart.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 13.25F, System.Drawing.FontStyle.Bold);



                            Series series = new Series()
                            {
                                Name = "Series2",
                                IsVisibleInLegend = false,
                                ChartType = SeriesChartType.Bar
                            };

                            barchart.Series.Add(series);
                            
                            int i = 1;
                            int j = 0;
                            foreach (var obj in posFeatures)
                            {
                                DataPoint P1 = new DataPoint(0, obj.Value);//Adding bars to the barchart
                                P1.Color = colorList[j];
                                P1.AxisLabel = "" + i;
                                P1.LegendText = "" + i;
                                P1.Label = obj.Key;
                                P1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Regular);
                                series.Points.Add(P1);
                                i++;
                                j++;
                            }

                            barchart.SaveImage(@"..\..\..\..\Users\Hp Mobile Workstatio\Documents\Visual Studio 2013\Projects\New folder\FYPAdam\Engine\Charts\posFeaChart.png", ChartImageFormat.Png); //saving the barchart in the same folder as exe
                           // var otherController = DependencyResolver.Current.GetService<HomeController>();
                            //var result = otherController.DrawChart(negFeatures,posFeatures);

                            //Making chart of negative features
                            Chart negFeatureBarChart = new Chart();
                            negFeatureBarChart.Size = new Size(1200, 500);//size f te bar chart
                            ChartArea area1 = new ChartArea();
                            negFeatureBarChart.ChartAreas.Add(area1);
                            negFeatureBarChart.BackColor = Color.Transparent;
                            negFeatureBarChart.ChartAreas[0].BackColor = Color.Transparent;
                            negFeatureBarChart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                            negFeatureBarChart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                            negFeatureBarChart.ChartAreas[0].AxisX.Title = "Negative Features";//title on the x-axis
                            negFeatureBarChart.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Center;
                            negFeatureBarChart.ChartAreas[0].AxisX.TextOrientation = TextOrientation.Horizontal;
                            negFeatureBarChart.ChartAreas[0].AxisX.Interval = 1;
                            negFeatureBarChart.ChartAreas[0].AxisY.LabelAutoFitStyle = LabelAutoFitStyles.None;
                            negFeatureBarChart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 2.25F, System.Drawing.FontStyle.Bold);
                            negFeatureBarChart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 13.25F, System.Drawing.FontStyle.Bold);
                            negFeatureBarChart.ChartAreas[0].AxisY.Title = "Count";//title on the y-axis
                            negFeatureBarChart.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Center;
                            negFeatureBarChart.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Horizontal;
                            negFeatureBarChart.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 13.25F, System.Drawing.FontStyle.Bold);



                            Series series1 = new Series()
                            {
                                Name = "Series21",
                                IsVisibleInLegend = true,
                                ChartType = SeriesChartType.Bar
                            };

                            negFeatureBarChart.Series.Add(series1);

                             i = 1;
                             j = 0;
                             foreach (var obj in negFeatures)
                            {
                                DataPoint P1 = new DataPoint(0, obj.Value);//Adding bars to the barchart
                                P1.Color = colorList[j];
                                P1.AxisLabel = "" + i;
                                P1.LegendText = "" + i;
                                P1.Label = obj.Key;
                                P1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Regular);
                                series1.Points.Add(P1);
                                i++;
                                j++;
                            }

                            negFeatureBarChart.SaveImage(@"..\..\..\..\Users\Hp Mobile Workstatio\Documents\Visual Studio 2013\Projects\New folder\FYPAdam\Engine\Charts\negFeaChart.png", ChartImageFormat.Png); //saving the barchart in the same folder as exe
                            var sortedPositiveFeatures = from pair in test.pfeatureDictionary
                                        orderby pair.Value descending
                                        select pair;

                            var sortedNegativeFeatures = from pair in test.nfeatureDictionary
                                                         orderby pair.Value descending
                                                         select pair;

                            var bestFeature = sortedPositiveFeatures.First();
                            var worstFeature = sortedNegativeFeatures.First();
                            
                            using (var message = new MailMessage("adamanalyzer@gmail.com", "adamtestreceiver@gmail.com"))
                            {
                                message.Subject = prodName+"Analysis Report";
                                // Construct the alternate body as HTML.
                                string body = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
                                body += "<HTML><HEAD><META http-equiv=Content-Type content=\"text/html; charset=iso-8859-1\">";
                                body += "</HEAD><BODY><DIV><FONT face=Arial color=#000000 size=3>" + prodName + "<br/><p >" + arr[0] + " :" + prodValues[0] + "</p>" + "<br/><p >" + arr[1] + " :" + prodValues[1] + "</p>" + "<br/><p >" + arr[2] + " :" + prodValues[2] + "</p>" + "<br/><p >" + arr[3] + " :" + prodValues[3] + "</p>" + "<br/><p >" + arr[4] + " :" + prodValues[4] + "</p>" + "<br/><p >" + arr[5] + " :" + prodValues[5] + "<br/></p>";
                                body += "<P> <B>Best Feature :</B>" + bestFeature.Key + "<br/></P>" + "<P><B> Worst Feature :</B>" + worstFeature.Key + "<br/></P>" + "</FONT>  </DIV>";
                                body += "</BODY></HTML>";

                                ContentType mimeType = new System.Net.Mime.ContentType("text/html");
                                // Add the alternate body to the message.

                                AlternateView alternate = AlternateView.CreateAlternateViewFromString(body, mimeType);
                                message.AlternateViews.Add(alternate);
                                string file = @"..\..\..\..\Users\Hp Mobile Workstatio\Documents\Visual Studio 2013\Projects\New folder\FYPAdam\Engine\Charts\posFeaChart.png";
                                


                                // Create  the file attachment for this e-mail message.
                                Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
                                // Add time stamp information for the file.
                                ContentDisposition disposition = data.ContentDisposition;
                                disposition.CreationDate = System.IO.File.GetCreationTime(file);
                                disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                                disposition.ReadDate = System.IO.File.GetLastAccessTime(file);

                                // Add the file attachment to this e-mail message.
                                message.Attachments.Add(data);


                                //adding 2nd attachment
                                string file1 = @"..\..\..\..\Users\Hp Mobile Workstatio\Documents\Visual Studio 2013\Projects\New folder\FYPAdam\Engine\Charts\negFeaChart.png";



                                // Create  the file attachment for this e-mail message.
                                Attachment data1 = new Attachment(file1, MediaTypeNames.Application.Octet);
                                // Add time stamp information for the file.
                                ContentDisposition disposition1 = data1.ContentDisposition;
                                disposition1.CreationDate = System.IO.File.GetCreationTime(file1);
                                disposition1.ModificationDate = System.IO.File.GetLastWriteTime(file1);
                                disposition1.ReadDate = System.IO.File.GetLastAccessTime(file1);

                                // Add the file attachment to this e-mail message.
                                message.Attachments.Add(data1);

                                // Create a message and set up the recipients.
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
                              
                            ReviewCollection.reviewList.Clear();
                            test.negativeFeatures.Clear();
                            test.positiveFeatures.Clear();
                            test.pfeatureDictionary.Clear();
                            test.nfeatureDictionary.Clear();
                            test.positiveFeatures.Columns.Remove("pNouns");
                            test.negativeFeatures.Columns.Remove("nNouns");
                            posFeatures.Clear();
                            negFeatures.Clear();

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