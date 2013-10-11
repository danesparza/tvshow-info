using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ShowInfoProvider;
using ServiceStack.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;

namespace RoviShowInfo
{
    /// <summary>
    /// Looks up TV episode information with Rovi: http://developer.rovicorp.com/.  
    /// This API requires an API key.  For an API key, please visit the Rovi developers site.
    /// </summary>
    [Export(typeof(ISeasonEpisodeShowInfoProvider))]
    public class RoviInfoProvider : ISeasonEpisodeShowInfoProvider
    {
        /// <summary>
        /// The API key
        /// </summary>
        private string APIKey = string.Empty;

        /// <summary>
        /// The API secret
        /// </summary>
        private string APISecret = string.Empty;

        public TVEpisodeInfo GetShowInfo(string showName, int season, int episode)
        {
            TVEpisodeInfo retval = null;

            //  Get API key information from application config
            APIKey = ConfigurationManager.AppSettings["Rovi_APIKey"];
            APISecret = ConfigurationManager.AppSettings["Rovi_APISecret"];

            //  If we can't find it, throw an exception
            if(string.IsNullOrEmpty(this.APIKey) || string.IsNullOrEmpty(this.APISecret))
            {
                throw new ApplicationException("Missing Rovi API key or secret.  Please include the Rovi_APIKey and Rovi_APISecret in the application config");
                return retval;
            }

            //  The Rovi result information
            RoviVideoResult roviResult = null;

            try
            {
                //  Get the sig
                string sig = GetSig();

                //  Construct the url
                string baseUrl = string.Format("http://api.rovicorp.com/data/v1.1/video/season/{2}/episode/{3}/info?apikey={0}&sig={1}&video={4}&include=synopsis",
                    this.APIKey,
                    sig,
                    season,
                    episode,
                    showName
                    );

                //  Get our JSON back
                roviResult = baseUrl.GetJsonFromUrl().FromJson<RoviVideoResult>();
            }
            catch(Exception ex)
            {
                /* Not sure what to do here yet */
            }

            if(roviResult != null)
            {
                retval = new TVEpisodeInfo()
                {
                    EpisodeNumber = episode,
                    EpisodeSummary = roviResult.EpisodeInfo.Synopsis.SynopsisText,
                    EpisodeTitle = roviResult.EpisodeInfo.EpisodeTitle,
                    OriginalAirDate = roviResult.EpisodeInfo.OriginalAirDate,
                    SeasonNumber = season,
                    ShowName = roviResult.EpisodeInfo.ShowTitle
                };
            }

            //  Return it
            return retval;
        }

        #region API tools

        /// <summary>
        /// Create an MD5 hash for our api call
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string CreateMD5Hash(string input)
        {
            // Use input string to calculate MD5 hash
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < hashBytes.Length; i++)
            {
                //this will use lowercase letters, use "X2" instead of "x2" to get uppercase
                sb.Append(hashBytes[i].ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get a signature
        /// </summary>
        /// <returns></returns>
        private string GetSig()
        {
            //get the timestamp value
            string timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds.ToString();

            //grab just the integer portion
            timestamp = timestamp.Substring(0, timestamp.IndexOf("."));

            //call the function to create the hash
            return CreateMD5Hash(APIKey + APISecret + timestamp);
        }

        #endregion
    }
}
