using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            //  Get the episode information using the seriesId, season, episode

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
    }
}
