using CIAT.DAPA.USAID.Forecast.ForecastApp.Models.SocialNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<bool> shareForecast()
        {
            /*var twitter = new TwitterApi(ConsumerKey, ConsumerKeySecret, AccessToken, AccessTokenSecret);
            var response = await twitter.Tweet("This is my first automated tweet!");*/
            return true;
        }
    }
}
