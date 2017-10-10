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
            var message = Program.settings.Social_Network_Message.Replace("#mes",DateTime.Now.ToString("MMMM"));
            var firstTweet = Tweet.PublishTweet(message);
            if (firstTweet != null)
                Console.WriteLine("Published message");
            else
            {
                var e = ExceptionHandler.GetLastException();
                Console.WriteLine(e);
                Console.WriteLine(e.TwitterDescription);
            }
            return true;
        }
    }
}
