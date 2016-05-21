using AdamDal;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Twitter;

namespace Engine
{
    public class WeekTrend : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var twitter = new Program
            {
                OAuthConsumerKey = "OAuth Consumer Key",
                OAuthConsumerSecret = "OAuth Consumer Secret"
            };
            try
            {
                DbWrappers wrap = new DbWrappers();
                List<Brand> mobBrands = wrap.GetAllBrandOfMobiles();//geting all the mobile brands coz we need them for twitter username



                foreach (var b in mobBrands)
                {
                    var count = twitter.GetBrandFollowers(b.UserName);
                    BrandFollower follower = new BrandFollower();
                    follower.BrandId = b.Id;
                    follower.Date = DateTime.Today.Date;
                    follower.FollowersCount = count.Result;
                    wrap.AddBrandFollower(follower);
                }


                List<Brand> laptopBrands = wrap.GetAllBrandNamesOfLaptops(); ;//geting all the mobile brands coz we need them for twitter username



                foreach (var b in laptopBrands)
                {
                    var count = twitter.GetBrandFollowers(b.UserName);
                    BrandFollower follower = new BrandFollower();
                    follower.BrandId = b.Id;
                    follower.Date = DateTime.Today.Date;
                    follower.FollowersCount = count.Result;
                    wrap.AddBrandFollower(follower);
                }

            }
            catch (Exception e)
            {

            }
        }
    }
}