using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ShowInfoProvider;

namespace TheTVDBShowInfo
{
    /// <summary>
    /// Looks up TV episode information with the TVDB: 
    /// http://thetvdb.com/wiki/index.php?title=Programmers_API
    /// This API requires an API key.  For an API key, please visit the TVDB
    /// developers site
    /// </summary>
    [Export(typeof(ISeasonEpisodeShowInfoProvider))]
    [Export(typeof(IAirdateShowInfoProvider))]
    public class TheTVDBInfoProvider : ISeasonEpisodeShowInfoProvider, IAirdateShowInfoProvider
    {
        private string APIKey = string.Empty;

        #region ISeasonEpisodeShowInfoProvider Members

        public TVEpisodeInfo GetShowInfo(string showName, int season, int episode)
        {
            TVEpisodeInfo retval = null;

            //  Get API key information from application config
            APIKey = ConfigurationManager.AppSettings["TheTVDB_APIKey"];

            //  If we can't find it, throw an exception
            if(string.IsNullOrEmpty(this.APIKey))
            {
                throw new ApplicationException("Missing TheTVDB API key.  Please include the TheTVDB_APIKey in the application config");
                return retval;
            }

            //  Get the seriesId for the show
            string seriesId = GetSeriesIdForShow(showName);

            //  Get the episode information using the API key, seriesId, season, episode
            string apiUrl = string.Format("http://thetvdb.com/api/{0}/series/{1}/default/{2}/{3}", 
                this.APIKey,
                seriesId, 
                season, 
                episode);

            TVDBEpisodeResult response = GetAPIResponse<TVDBEpisodeResult>(apiUrl);

            if(response != null)
            {
                retval = new TVEpisodeInfo()
                {
                    EpisodeNumber = response.EpisodeInfo.EpisodeNumber,
                    EpisodeSummary = response.EpisodeInfo.EpisodeSummary,
                    EpisodeTitle = response.EpisodeInfo.EpisodeName,
                    OriginalAirDate = response.EpisodeInfo.OriginalAirDate,
                    SeasonNumber = response.EpisodeInfo.SeasonNumber,
                    ShowName = showName
                };
            }

            return retval;
        }

        #endregion

        #region IAirdateShowInfoProvider Members

        public TVEpisodeInfo GetShowInfo(string showName, int year, int month, int day)
        {
            TVEpisodeInfo retval = null;

            //  Get API key information from application config
            APIKey = ConfigurationManager.AppSettings["TheTVDB_APIKey"];

            //  If we can't find it, throw an exception
            if(string.IsNullOrEmpty(this.APIKey))
            {
                throw new ApplicationException("Missing TheTVDB API key.  Please include the TheTVDB_APIKey in the application config");
                return retval;
            }

            //  Get the seriesId for the show

            //  Get the episode information using the seriesId, airdate

            return retval;
        }

        #endregion

        /// <summary>
        /// Gets the first listed TVDB seriesId for a given show name
        /// </summary>
        /// <param name="showName"></param>
        /// <returns></returns>
        private string GetSeriesIdForShow(string showName)
        {
            string retval = string.Empty;

            //  The formatted API url to call
            string apiUrl = string.Format("http://thetvdb.com/api/GetSeries.php?seriesname={0}", showName);

            //  Call the API
            TVDBSeriesResult response = GetAPIResponse<TVDBSeriesResult>(apiUrl);
            
            //  If we have valid (deserialized) data
            if(response != null)
                retval = response.SeriesInfo.SeriesId;

            return retval;
        }

        /// <summary>
        /// For a given API url, get the response and deserialize to the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        private T GetAPIResponse<T>(string url)
        {
            T result = default(T);

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                XmlReader reader = XmlReader.Create(url);
                result = (T)serializer.Deserialize(reader);
                reader.Close();
            }
            catch(Exception ex)
            {
                /* Just fail silently for now and return default(T) */
            }
            
            return result;
        }
    }
}
