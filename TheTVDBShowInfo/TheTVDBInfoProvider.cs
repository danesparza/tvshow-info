using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
            Trace.TraceInformation("Getting TVDB show information for: {0} season {1}, episode {2}", showName, season, episode);

            //  Get API key information from application config
            APIKey = ConfigurationManager.AppSettings["TheTVDB_APIKey"];
            Trace.TraceInformation("Using TVDB API key: {0}", this.APIKey);

            //  If we can't find it, throw an exception
            if(string.IsNullOrEmpty(this.APIKey))
            {
                throw new ApplicationException("Missing TheTVDB API key.  Please include the TheTVDB_APIKey in the application config");
                return retval;
            }

            //  Get the seriesId for the show
            TVDBSeriesResult series = GetSeriesForShow(showName);

            //  If we were able to get the seriesId...
            if(series != null && series.SeriesInfo != null && series.SeriesInfo.SeriesId != null)
            {
                //  Get the episode information using the API key, seriesId, season, episode
                string apiUrl = string.Format("http://thetvdb.com/api/{0}/series/{1}/default/{2}/{3}",
                    this.APIKey,
                    series.SeriesInfo.SeriesId,
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
                        ShowName = series.SeriesInfo.SeriesName
                    };
                }
            }

            return retval;
        }

        #endregion

        #region IAirdateShowInfoProvider Members

        public TVEpisodeInfo GetShowInfo(string showName, int year, int month, int day)
        {
            TVEpisodeInfo retval = null;
            Trace.TraceInformation("Getting TVDB show information for: {0} date: {1}-{2}-{3}", showName, year, month, day);

            //  Get API key information from application config
            APIKey = ConfigurationManager.AppSettings["TheTVDB_APIKey"];
            Trace.TraceInformation("Using TVDB API key: {0}", this.APIKey);

            //  If we can't find it, throw an exception
            if(string.IsNullOrEmpty(this.APIKey))
            {
                throw new ApplicationException("Missing TheTVDB API key.  Please include the TheTVDB_APIKey in the application config");
                return retval;
            }

            //  Get the seriesId for the show
            TVDBSeriesResult series = GetSeriesForShow(showName);

            //  If we were able to get the seriesId...
            if(series != null && series.SeriesInfo != null && series.SeriesInfo.SeriesId != null)
            {
                //  Get the episode information using the seriesId, airdate
                string apiUrl = string.Format("http://thetvdb.com/api/GetEpisodeByAirDate.php?apikey={0}&seriesid={1}&airdate={2}-{3}-{4}",
                    this.APIKey,
                    series.SeriesInfo.SeriesId,
                    year,
                    month,
                    day);

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
                        ShowName = series.SeriesInfo.SeriesName
                    };
                }
            }

            return retval;
        }

        #endregion

        #region API tools
        
        /// <summary>
        /// Gets the first listed TVDB seriesId for a given show name
        /// </summary>
        /// <param name="showName"></param>
        /// <returns></returns>
        private TVDBSeriesResult GetSeriesForShow(string showName)
        {
            TVDBSeriesResult retval = null;

            //  The formatted API url to call
            string apiUrl = string.Format("http://thetvdb.com/api/GetSeries.php?seriesname={0}&apikey={1}", showName, this.APIKey);

            //  Call the API
            retval = GetAPIResponse<TVDBSeriesResult>(apiUrl);

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

            Trace.TraceInformation("Calling TVDB api: {0}", url);

            try
            {
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                string responseData = webClient.DownloadString(url);

                Trace.TraceInformation("Serializing TVDB response: {0}", responseData);
                
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                XmlReader reader = XmlReader.Create(new StringReader(responseData));
                result = (T)serializer.Deserialize(reader);
                
                reader.Close();
            }
            catch(Exception ex)
            {
                Trace.TraceError("TVDB api exception: {0}", ex.Message);
                /* Just fail silently for now and return default(T) */
            }

            return result;
        } 

        #endregion
    }
}
