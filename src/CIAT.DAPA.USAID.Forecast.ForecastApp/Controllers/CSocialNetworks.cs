using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Controllers
{
    public class CSocialNetworks
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        public CSocialNetworks()
        {

        }

        /// <summary>
        /// Method to share a post in social networks when it is finished
        /// </summary>
        /// <returns>Always return true</returns>
        public async Task<bool> shareForecast()
        {   
            Auth.SetUserCredentials(Program.settings.Social_Network_Twitter_ConsumerKey, Program.settings.Social_Network_Twitter_ConsumerKeySecret,
                Program.settings.Social_Network_Twitter_AccessToken, Program.settings.Social_Network_Twitter_AccessTokenSecret);
            var firstTweet = Tweet.PublishTweet(Program.settings.Social_Network_Message);
            return true;
        }
    }
}
