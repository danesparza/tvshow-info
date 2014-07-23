using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using NLog;
using System.Linq;
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
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private string APIKey = string.Empty;
        private string language = string.Empty;
        private string cacheDirectory = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TheTVDBInfoProvider()
        {
            //  Get API key information from application config
            APIKey = ConfigurationManager.AppSettings["TheTVDB_APIKey"] ?? "";
            logger.Debug("Using TVDB API key: {0}", this.APIKey);
        }

        #region ISeasonEpisodeShowInfoProvider Members

        public TVEpisodeInfo GetShowInfo(string showName, int season, int episode)
        {
            TVEpisodeInfo retval = null;
            logger.Debug("Getting TVDB show information for: {0} season {1}, episode {2}", showName, season, episode);

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

        /// <summary>
        /// Gets all show information, including all episodes for a show
        /// </summary>
        /// <param name="showname"></param>
        /// <returns></returns>
        public TVSeriesInfo GetSeriesInfo(string showname)
        {
            TVSeriesInfo retval = new TVSeriesInfo();

            //  Get the show information
            TVDBSeriesResult seriesResult = GetSeriesForShow(showname);
            TVDBSeriesInfo series = null;

            if(seriesResult != null)
                series = seriesResult.SeriesInfo;

            //  Get all episodes for the show:
            if(series != null)
            {
                //  Set basic show information:
                retval.Name = series.SeriesName;

                //  Get the language from the configuration file
                language = ConfigurationManager.AppSettings["TheTVDB_Language"] ?? "en";

                //  Construct the cache path
                string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                cacheDirectory = ConfigurationManager.AppSettings["TheTVDB_CacheDir"] ?? "cache";
                cacheDirectory = Path.Combine(currentPath, cacheDirectory, series.SeriesId);

                //  Construct the cache filename
                string cacheFile = cacheDirectory + Path.DirectorySeparatorChar + "episodes.zip";

                //  Make sure the cache file exists:
                if(!Directory.Exists(cacheDirectory))
                {
                    //  If the directory doesn't exist, create it:
                    if(!Directory.Exists(cacheDirectory))
                        Directory.CreateDirectory(cacheDirectory);
                }

                //  Check to see if we have cached results for the series
                if(!File.Exists(cacheFile))
                {
                    //  Get the latest episode zip and save it to the cache path
                    GetEpisodeBundle(series, language, cacheFile);
                }

                //  If the cache file exists...
                if(File.Exists(cacheFile))
                {
                    XDocument doc = new XDocument();

                    //  Open it as a zipfile:
                    using(ZipArchive zip = ZipFile.Open(cacheFile, ZipArchiveMode.Read))
                    {
                        foreach(ZipArchiveEntry entry in zip.Entries)
                        {
                            if(entry.Name == language + ".xml")
                            {
                                //  Attempt to create an XDocument from it
                                doc = XDocument.Load(entry.Open());

                                break;
                            }
                        }
                    }

                    //  Using the XDocument return a list of TVEpisodeInfo's
                    var episodes = from item in doc.Descendants("Episode")
                                   select new TVEpisodeInfo()
                                   {
                                       EpisodeTitle = item.Element("EpisodeName").Value,
                                       EpisodeSummary = item.Element("Overview").Value,
                                       EpisodeNumber = Convert.ToInt32(item.Element("EpisodeNumber").Value),
                                       OriginalAirDate = DateTime.Parse(item.Element("FirstAired").Value),
                                       SeasonNumber = Convert.ToInt32(item.Element("SeasonNumber").Value),
                                       ShowName = series.SeriesName
                                   };

                    retval.Seasons = from episode in episodes
                                  group episode by episode.SeasonNumber into seasonGroup
                                  orderby seasonGroup.Key
                                  select seasonGroup;
                }
            }

            return retval;
        }

        #endregion

        #region IAirdateShowInfoProvider Members

        public TVEpisodeInfo GetShowInfo(string showName, int year, int month, int day)
        {
            TVEpisodeInfo retval = null;
            logger.Debug("Getting TVDB show information for: {0} date: {1}-{2}-{3}", showName, year, month, day);

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

            logger.Debug("Calling TVDB api: {0}", url);

            try
            {
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                string responseData = webClient.DownloadString(url);

                logger.Debug("Serializing TVDB response: {0}", responseData);
                
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                XmlReader reader = XmlReader.Create(new StringReader(responseData));
                result = (T)serializer.Deserialize(reader);
                
                reader.Close();
            }
            catch(Exception ex)
            {
                logger.ErrorException("There was a problem using the TVDB api", ex);
                /* Just fail silently for now and return default(T) */
            }

            return result;
        }

        /// <summary>
        /// For a given series and language, download the zip file containing all episode information
        /// to the specified file
        /// </summary>
        /// <param name="series"></param>
        /// <param name="language"></param>
        /// <param name="cacheFile">The file to use to cache the episode information</param>
        /// <returns></returns>
        private void GetEpisodeBundle(TVDBSeriesInfo series, string language, string cacheFile)
        {
            //  Construct the url path
            string url = string.Format("http://thetvdb.com/api/{0}/series/{1}/all/{2}.zip",
                this.APIKey,
                series.SeriesId,
                language
            );

            //  Attempt to download the file:
            try 
	        {
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                webClient.DownloadFile(url, cacheFile);
	        }
	        catch (Exception ex)
	        {
                logger.ErrorException("There was a problem downloading the list of episodes", ex);
	        }
        }

        #endregion
    }
}
