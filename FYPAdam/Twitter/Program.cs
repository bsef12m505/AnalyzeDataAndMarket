using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms.DataVisualization.Charting;
using AdamDal;

namespace Twitter
{
    public class Program
    {

        public static DataTable HashTag_info = new DataTable();
        public static List<int> retweetCount = new List<int>();
        public static List<dynamic> topTags = new List<dynamic>();
        //public static string HistName;
        //public static string HistX_axis_Tiltle;
        //public static string HistY_axis_Tiltle;

        public string OAuthConsumerSecret { get; set; }
        public string OAuthConsumerKey { get; set; }

        public async Task GetFollowersCount(string accessToken = null)
        {
            if (accessToken == null)
            {
                accessToken = await GetAccessToken();
            }
            DbWrappers wrap = new DbWrappers();
            List<Brand> laptopBrands=wrap.GetAllBrandNamesOfLaptops();
            foreach(Brand b in laptopBrands)
            {
                var requestUserTimeline = new HttpRequestMessage(HttpMethod.Get, string.Format("https://api.twitter.com/1.1/users/show.json?screen_name={0}", b.UserName));
                requestUserTimeline.Headers.Add("Authorization", "Bearer " + accessToken);
                var httpClient = new HttpClient();
                HttpResponseMessage responseUserTimeLine = await httpClient.SendAsync(requestUserTimeline);
                var serializer = new JavaScriptSerializer();
                dynamic json = serializer.Deserialize<object>(await responseUserTimeLine.Content.ReadAsStringAsync());
                int followercount = (int)json["followers_count"];
                b.FollowersCount = followercount;
                wrap.UpdateBrand(b);
            }

            List<Brand> mobileBrands = wrap.GetAllBrandOfMobiles();
            foreach(Brand b in mobileBrands)
            {
                var requestUserTimeline = new HttpRequestMessage(HttpMethod.Get, string.Format("https://api.twitter.com/1.1/users/show.json?screen_name={0}", b.UserName));
                requestUserTimeline.Headers.Add("Authorization", "Bearer " + accessToken);
                var httpClient = new HttpClient();
                HttpResponseMessage responseUserTimeLine = await httpClient.SendAsync(requestUserTimeline);
                var serializer = new JavaScriptSerializer();
                dynamic json = serializer.Deserialize<object>(await responseUserTimeLine.Content.ReadAsStringAsync());
                int followercount = (int)json["followers_count"];
                b.FollowersCount = followercount;
                wrap.UpdateBrand(b);
            }

        }
        public async Task<List<dynamic>> GetTwitts_HashTags(string userName, string accessToken = null)
        {
            if (accessToken == null)
            {
                accessToken = await GetAccessToken();
            }
            // var requestUserTimeline = new HttpRequestMessage(HttpMethod.Get, string.Format("https://api.twitter.com/1.1/search/tweets.json?q=%23{0}&count=190&since_id=24012619984051000", userName));
            var requestUserTimeline = new HttpRequestMessage(HttpMethod.Get, string.Format("https://api.twitter.com/1.1/search/tweets.json?q={0}&count=20", userName));

            requestUserTimeline.Headers.Add("Authorization", "Bearer " + accessToken);
            var httpClient = new HttpClient();
            HttpResponseMessage responseUserTimeLine = await httpClient.SendAsync(requestUserTimeline);
            var serializer = new JavaScriptSerializer();
            List<Object> lst = new List<object>();
            dynamic json = serializer.Deserialize<object>(await responseUserTimeLine.Content.ReadAsStringAsync());
            foreach (var kvp in json)
            {

                foreach (var obj in kvp.Value)
                {
                    lst.Add(obj);
                }

                break;
            }

            var enumerableTwitts = (lst as IEnumerable<dynamic>);

            if (enumerableTwitts == null)
            {
                return null;
            }
            //return enumerableTwitts.Select(t => (string)(t["text"].ToString()));

            return enumerableTwitts.ToList<dynamic>();
        }

        public async Task<string> GetAccessToken()
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/oauth2/token ");

            OAuthConsumerKey = "HfSKXt71Xcz2izFQQjJpqDB2s";
            OAuthConsumerSecret = "la8BJwvwoJG69WuWPgO8B4tRn6kusPxtZQ316yGFn4uEbVsBgO";
            var customerInfo = Convert.ToBase64String(new UTF8Encoding().GetBytes(OAuthConsumerKey + ":" + OAuthConsumerSecret));

            request.Headers.Add("Authorization", "Basic " + customerInfo);
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpResponseMessage response = await httpClient.SendAsync(request);

            string json = await response.Content.ReadAsStringAsync();
            var serializer = new JavaScriptSerializer();
            dynamic item = serializer.Deserialize<object>(json);
            return item["access_token"];
        }


        public static void Frequency_Analysis_HashTag()
        {
            var hastTag_Count = from row in HashTag_info.AsEnumerable()
                                group row by row.Field<string>("HashTag") into grp
                                select new { key = grp.Key, cnt = grp.Count() };

            var topTenHashTags = hastTag_Count.OrderByDescending(x => x.cnt).Take(10);
            // List<dynamic> topTags = new List<dynamic>();
            int i = 1;
            Console.WriteLine("                                      Hash Tags and their count                           ");
            foreach (var o in topTenHashTags)
            {
                topTags.Add(o);
                Console.WriteLine(i + "- HashTag Name : " + o.key);
                Console.WriteLine(i + "- HashTag Count : " + o.cnt);
                Console.WriteLine("-------------------------------------------------------------------------");
                i++;
            }


            // var result1 = result.Count(x => x.Key);

        }
        public static void Frequency_Analysis_RetweetCount(DataTable tab)
        {
            var result = from row in tab.AsEnumerable()
                         group row by row.Field<int>("Retweet_Count"); //Grouping the records on the basis of retweet count.result will have group of records on the basis of retweet count  

            List<dynamic> lst1 = new List<object>();

            var result1 = result.OrderByDescending(x => x.Key);  //ordering each group in the decending ordr
            foreach (var res in result1)
            {

                var r2 = res.Select(x => x).Take(1);   //selecting the first record of the group

                foreach (var s in r2)
                {

                    lst1.Add(s);
                }

            }
            var enumerableTwitts = (lst1 as IEnumerable<dynamic>);
            int counter = 0;
            Console.WriteLine("                   Frequency Analysis of Tweets             ");
            Console.WriteLine("-------------------------------------------------------------------------");
            foreach (var tweetObj in enumerableTwitts)
            {
                if (counter <= 10)
                {
                    retweetCount.Add(tweetObj["Retweet_Count"]);
                    Console.WriteLine("ReTweet Count :" + tweetObj["Retweet_Count"]);
                    Console.WriteLine("Tweet :" + tweetObj["Tweets"]);
                    Console.WriteLine("Screne Name :" + tweetObj["ScreenName"]);
                    Console.WriteLine("-------------------------------------------------------------------------");
                }
                counter++;
            }
        }

        public static void ShowBarChartHashTag(List<dynamic> tags) //showing te bar chart of top 10 hashtags and their count in 100 tweets
        {

            Chart barchart = new Chart();
            barchart.Size = new Size(800, 500);//size f te bar chart
            ChartArea area = new ChartArea();
            barchart.ChartAreas.Add(area);
            barchart.BackColor = Color.Transparent;
            barchart.ChartAreas[0].BackColor = Color.Transparent;
            barchart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            barchart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            barchart.ChartAreas[0].AxisX.Title = "HahTags";//title on the x-axis
            barchart.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Center;
            barchart.ChartAreas[0].AxisX.TextOrientation = TextOrientation.Horizontal;
            barchart.ChartAreas[0].AxisX.Interval = 1;
            // barchart.ChartAreas[0].AxisY.LabelAutoFitStyle = LabelAutoFitStyles.None;
            // barchart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 2.25F, System.Drawing.FontStyle.Bold);

            barchart.ChartAreas[0].AxisY.Title = "HashTag Count";//title on the y-axis
            barchart.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Center;
            barchart.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Horizontal;

            Series series = new Series()
            {
                Name = "Series2",
                IsVisibleInLegend = false,
                ChartType = SeriesChartType.Column
            };

            barchart.Series.Add(series);
            int i = 1;
            foreach (var obj in tags)
            {
                DataPoint P1 = new DataPoint(0, obj.cnt);//Adding bars to the barchart
                P1.Color = Color.LightGreen;
                P1.AxisLabel = "" + i;
                P1.LegendText = "" + i;
                P1.Label = obj.key;

                series.Points.Add(P1);
                i++;
            }

            barchart.SaveImage("hashTagHist.png", ChartImageFormat.Png); //saving the barchart in the same folder as exe
        }




        public static void ShowBarChart(List<int> count)
        {

            Chart barchart = new Chart();
            barchart.Size = new Size(800, 500);
            ChartArea area = new ChartArea();
            barchart.ChartAreas.Add(area);
            barchart.BackColor = Color.Transparent;
            barchart.ChartAreas[0].BackColor = Color.Transparent;
            barchart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            barchart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            barchart.ChartAreas[0].AxisX.Title = "Tweets";//title of th x-axis
            barchart.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Center;
            barchart.ChartAreas[0].AxisX.TextOrientation = TextOrientation.Horizontal;
            barchart.ChartAreas[0].AxisX.Interval = 1;


            barchart.ChartAreas[0].AxisY.Title = "Retweet Count";//title on the y -axis
            barchart.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Center;
            barchart.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Horizontal;

            Series series = new Series()
            {
                Name = "Series2",
                IsVisibleInLegend = false,
                ChartType = SeriesChartType.Column
            };

            barchart.Series.Add(series);
            int i = 1;
            foreach (var obj in count)
            {
                DataPoint P1 = new DataPoint(0, obj);//Adding each point to the bar chart
                P1.Color = Color.LightGreen;
                P1.AxisLabel = "" + i;
                P1.LegendText = "" + i;
                P1.Label = obj.ToString();

                series.Points.Add(P1);
                i++;
            }

            barchart.SaveImage("retweetCountHist.png", ChartImageFormat.Png);//saving the barchart in the same folder as the exe
        }

        public static void ShowTableData(DataTable tweetTable)
        {
            foreach (DataRow row in tweetTable.Rows)
            {
                Int64 id = (Int64)row["Tweet_Id"];
                string text = row["Tweets"].ToString();
                int retweetCount = (int)row["Retweet_Count"];
                string name = row["ScreenName"].ToString();
                int fav_count = (int)row["Favourites_Count"];
                string location = row["Location"].ToString();
                int status_Count = (int)row["Status_Count"];
                int friends_Count = (int)row["Friends_Count"];
                int followers_Count = (int)row["Followers_Count"];

                var result = from row1 in HashTag_info.AsEnumerable()
                             where row1.Field<Int64>("Tweet_Id") == id
                             select row1;

                Console.WriteLine("Tweet id" + id);
                Console.WriteLine("Tweet:" + text);
                Console.WriteLine("Screne Name :" + name);
                Console.WriteLine("Location :" + location);
                Console.WriteLine("Total Likes :" + fav_count);
                Console.WriteLine("Statuses Count :" + status_Count);
                Console.WriteLine("Friends Count :" + friends_Count);
                Console.WriteLine("Retweet Count :" + retweetCount);
                Console.WriteLine("Followers Count :" + followers_Count);
                Console.Write("Hashtags : ");

                foreach (var tag in result)
                {
                    Console.Write("#" + tag["HashTag"] + " , ");
                }

                Console.WriteLine();
                Console.WriteLine("-------------------------------------------------------------------------");

            }

        }

        public static DataTable add_HashTags(List<dynamic> lst)
        {
            DataTable Tweet_info = new DataTable();
            Tweet_info.Columns.Add("Tweets", typeof(string)); //Adding columns to the Tweet_info table
            Tweet_info.Columns.Add("Tweet_Id", typeof(Int64)); //Adding columns to the Tweet_info table
            Tweet_info.Columns.Add("ScreenName", typeof(string)); //Adding columns to the Tweet_info table
            Tweet_info.Columns.Add("Location", typeof(string)); //Adding columns to the Tweet_info table
            Tweet_info.Columns.Add("Retweet_Count", typeof(int)); //Adding columns to the Tweet_info table
            Tweet_info.Columns.Add("Status_Count", typeof(int)); //Adding columns to the Tweet_info table
            Tweet_info.Columns.Add("Followers_Count", typeof(int)); //Adding columns to the Tweet_info table
            Tweet_info.Columns.Add("Friends_Count", typeof(int)); //Adding columns to the Tweet_info table
            Tweet_info.Columns.Add("Favourites_Count", typeof(int)); //Adding columns to the Tweet_info table

            //DataTable HashTag_info = new DataTable();
            HashTag_info.Columns.Add("Tweet_Id", typeof(Int64));  //Adding columns to the HashTag_info table
            HashTag_info.Columns.Add("HashTag", typeof(string));  //Adding columns to the HashTag_info table



            try
            {
                foreach (var t in lst)
                {
                    var userData = t["user"];//storing user info in userdata
                    var entities = t["entities"]; //stroring user entities in entities


                    DataRow newRow = Tweet_info.NewRow();
                    newRow["Tweets"] = t["text"].ToString();
                    newRow["Tweet_Id"] = (Int64)t["id"];

                    newRow["ScreenName"] = userData["screen_name"];
                    newRow["Location"] = userData["location"];
                    newRow["Retweet_Count"] = (int)t["retweet_count"];
                    newRow["Status_Count"] = (int)userData["statuses_count"];
                    newRow["Followers_Count"] = (int)userData["followers_count"];
                    newRow["Friends_Count"] = (int)userData["friends_count"];
                    newRow["Favourites_Count"] = (int)userData["favourites_count"];

                    Tweet_info.Rows.Add(newRow);//Adding new data row to the Tweet_info data Table

                    var hash_Tag = entities["hashtags"]; //extracting hashtags from the entities
                    foreach (var tag in hash_Tag)
                    {
                        DataRow row = HashTag_info.NewRow();
                        row["Tweet_Id"] = (Int64)t["id"];
                        row["HashTag"] = tag["text"].ToString();
                        HashTag_info.Rows.Add(row);//saving each hashtag in a data table
                    }


                }

                return Tweet_info;
                // Console.ReadKey();
            }
            catch (Exception e)
            {

            }

            return Tweet_info;
        }

        static void Main(string[] args)
        {
            try
            {
                //    var twitter = new Program
                //    {
                //        OAuthConsumerKey = "OAuth Consumer Key",
                //        OAuthConsumerSecret = "OAuth Consumer Secret"
                //    };


                //    Console.WriteLine("Enter HashTag");
                //    string HashTag = Console.ReadLine();
                //    List<dynamic> Tweets_HashTag = twitter.GetTwitts_HashTags(HashTag).Result; //fetchng top 100 tweets related to a hash tag
                //    DataTable tab = add_HashTags(Tweets_HashTag);//populating the data table
                //    ShowTableData(tab);//shoing table data
                //    Frequency_Analysis_RetweetCount(tab); //analyze the tweets on the basis of retweet count
                //    ShowBarChart(retweetCount); //showing the bar char of the most retweeted tweets
                //    Frequency_Analysis_HashTag(); //frequeny analysis 
                //    ShowBarChartHashTag(topTags);




                Console.ReadKey();
            }
            catch (Exception e)
            {

            }
            //tab.Compute("","");  
        }
    }
}
