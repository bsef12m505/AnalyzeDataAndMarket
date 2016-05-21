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
        public static List<dynamic> extractedTags = new List<dynamic>();
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
            List<Brand> laptopBrands = wrap.GetAllBrandNamesOfLaptops();
            foreach (Brand b in laptopBrands)
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
            foreach (Brand b in mobileBrands)
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

        public async Task<int> GetBrandFollowers(string BUserName,string accessToken = null)
        {
            if (accessToken == null)
            {
                accessToken = await GetAccessToken();
            }

            var requestUserTimeline = new HttpRequestMessage(HttpMethod.Get, string.Format("https://api.twitter.com/1.1/users/show.json?screen_name={0}", BUserName));
            requestUserTimeline.Headers.Add("Authorization", "Bearer " + accessToken);
            var httpClient = new HttpClient();
            HttpResponseMessage responseUserTimeLine = await httpClient.SendAsync(requestUserTimeline);
            var serializer = new JavaScriptSerializer();
            dynamic json = serializer.Deserialize<object>(await responseUserTimeLine.Content.ReadAsStringAsync());
            int followercount = (int)json["followers_count"];
            return followercount;
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


        public List<dynamic> Frequency_Analysis_HashTag(DataTable hashTag)
        {
            try
            {
                var hastTag_Count = from row in hashTag.AsEnumerable()
                                    group row by row.Field<string>("HashTag") into grp
                                    select new { key = grp.Key, cnt = grp.Count() };

                var topTenHashTags = hastTag_Count.OrderByDescending(x => x.cnt).Take(10);
                // List<dynamic> topTags = new List<dynamic>();

                Console.WriteLine("                                      Hash Tags and their count                           ");
                foreach (var o in topTenHashTags)
                {

                    topTags.Add(o);
                    string tag = o.key.ToLower();
                    if (tag.Contains("galaxy"))
                    {
                        extractedTags.Add(o.key);

                    }


                }

                return extractedTags;

            }
            catch (Exception)
            {
                return extractedTags;
            }
            // var result1 = result.Count(x => x.Key);

        }


        public async Task<List<dynamic>> GetTweetsUserName(string userName, int count, string accessToken = null)
        {
            if (accessToken == null)
            {
                accessToken = await GetAccessToken();
            }
            List<dynamic> lst = new List<dynamic>();
            var requestUserTimeline = new HttpRequestMessage(HttpMethod.Get, string.Format("https://api.twitter.com/1.1/statuses/user_timeline.json?count={0}&screen_name={1}&trim_user=1&exclude_replies=1", count, userName));

            requestUserTimeline.Headers.Add("Authorization", "Bearer " + accessToken);
            var httpClient = new HttpClient();
            HttpResponseMessage responseUserTimeLine = await httpClient.SendAsync(requestUserTimeline);
            var serializer = new JavaScriptSerializer();
            dynamic json = serializer.Deserialize<object>(await responseUserTimeLine.Content.ReadAsStringAsync());

            foreach (var kvp in json)
            {


                lst.Add(kvp);

            }

            var enumerableTwitts = (lst as IEnumerable<dynamic>);

            if (enumerableTwitts == null)
            {
                return null;
            }
            //return enumerableTwitts.Select(t => (string)(t["text"].ToString()));

            return enumerableTwitts.ToList<dynamic>();
        }


        public DataTable add_HashTags(List<dynamic> lst)
        {
            DataTable Tweet_info = new DataTable();
            Tweet_info.Columns.Add("Tweets", typeof(string)); //Adding columns to the Tweet_info table
            Tweet_info.Columns.Add("Tweet_Id", typeof(Int64)); //Adding columns to the Tweet_info table


            Tweet_info.Columns.Add("Retweet_Count", typeof(int)); //Adding columns to the Tweet_info table

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


                    newRow["Favourites_Count"] = (int)t["favorite_count"];
                    newRow["Retweet_Count"] = (int)t["retweet_count"];


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

                return HashTag_info;
                // Console.ReadKey();
            }
            catch (Exception e)
            {

            }

            return HashTag_info;
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
