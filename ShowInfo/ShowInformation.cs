using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ShowInfoProvider;

namespace ShowInfo
{
    public class ShowInformationManager
    {
        #region MEF helpers and imports
        
        [ImportMany]
        public IEnumerable<ISeasonEpisodeShowInfoProvider> SEShowInfoProviders { get; set; }

        [ImportMany]
        public IEnumerable<IAirdateShowInfoProvider> ADShowInfoProviders { get; set; }

        private void Compose()
        {
            var catalog = new AggregateCatalog();

            //  Add the current directory
            catalog.Catalogs.Add(new DirectoryCatalog(@"./"));

            //  Add the directory in the config file if it exists:
            string pluginDir = ConfigurationManager.AppSettings["PluginDirectory"];
            if(!string.IsNullOrEmpty(pluginDir))
                catalog.Catalogs.Add(new DirectoryCatalog(pluginDir));

            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        } 

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public ShowInformationManager()
        {
            Compose();
        }

        /// <summary>
        /// Gets show information for the given filename
        /// </summary>
        /// <param name="filename">The filename to get TV show information for</param>
        /// <returns></returns>
        public TVEpisodeInfo GetEpisodeInfoForFilename(string filename)
        {
            //  Create our new episode information:
            TVEpisodeInfo retval = null;
            
            string parsedShowName = string.Empty;
            int parsedSeasonNumber = 0;
            int parsedEpisodeNumber = 0;
            
            int parsedAirdateYear = 0;
            int parsedAirdateMonth = 1;
            int parsedAirdateDay = 1;

            /******* PARSE THE FILENAME ********/
            
            //  Season/Episode parsers
            Regex rxSE = new Regex(@"^((?<series_name>.+?)[. _-]+)?s(?<season_num>\d+)[. _-]*e(?<ep_num>\d+)(([. _-]*e|-)(?<extra_ep_num>(?!(1080|720)[pi])\d+))*[. _-]*((?<extra_info>.+?)((?<![. _-])-(?<release_group>[^-]+))?)?$", RegexOptions.IgnoreCase);
            Regex rxSE2 = new Regex(@"(?<series_name>.*?)\.S?(?<season_num>\d{1,2})[Ex-]?(?<ep_num>\d{2})\.(.*)", RegexOptions.IgnoreCase);

            //  Airdate parsers
            Regex rxD = new Regex(@"^((?<series_name>.+?)[. _-]+)(?<year>\d{4}).(?<month>\d{1,2}).(?<day>\d{1,2})", RegexOptions.IgnoreCase);

            //  Process our regexes:
            var seMatch = rxSE.Match(filename);
            var seMatch2 = rxSE2.Match(filename);
            var dMatch = rxD.Match(filename);
            var currentParseType = ParseType.Unknown;

            //  See how we made out...
            if(seMatch.Success)
            {
                currentParseType = ParseType.SeasonEpisode;
                parsedShowName = Regex.Replace(seMatch.Groups["series_name"].ToString().Trim(), @"[\W]|_", " ");
                parsedSeasonNumber = Convert.ToInt32(seMatch.Groups["season_num"].Value);
                parsedEpisodeNumber = Convert.ToInt32(seMatch.Groups["ep_num"].Value);
            }
            //  See if we have a year/month/day match:
            else if(dMatch.Success)
            {
                currentParseType = ParseType.Airdate;
                parsedShowName = Regex.Replace(dMatch.Groups["series_name"].ToString().Trim(), @"[\W]|_", " ");
                parsedAirdateYear = Convert.ToInt32(dMatch.Groups["year"].Value);
                parsedAirdateMonth = Convert.ToInt32(dMatch.Groups["month"].Value);
                parsedAirdateDay = Convert.ToInt32(dMatch.Groups["day"].Value);
            }
            //  Try an alternate S/E match
            else if(seMatch2.Success)
            {
                currentParseType = ParseType.SeasonEpisode;
                parsedShowName = Regex.Replace(seMatch2.Groups["series_name"].ToString().Trim(), @"[\W]|_", " ");
                parsedSeasonNumber = Convert.ToInt32(seMatch2.Groups["season_num"].Value);
                parsedEpisodeNumber = Convert.ToInt32(seMatch2.Groups["ep_num"].Value);
            }

            //  Based on the type of information parsed, use either 
            //  season/episode data providers or airdate data providers
            switch(currentParseType)
            {
                case ParseType.SeasonEpisode:
                    //  If it parses to a show/season/episode, get 
                    //  information from the SEShowInfoProviders
                    foreach(var provider in SEShowInfoProviders)
                    {
                        retval = provider.GetShowInfo(parsedShowName, parsedSeasonNumber, parsedEpisodeNumber);

                        //  If we found our information, get out
                        if(retval != null)
                            break;
                    }
                    break;
                case ParseType.Airdate:
                    //  If it parses to show/airdate, get information
                    //  from the ADShowInfoProviders
                    foreach(var provider in ADShowInfoProviders)
                    {
                        retval = provider.GetShowInfo(parsedShowName, parsedAirdateYear, parsedAirdateMonth, parsedAirdateDay);

                        //  If we found our information, get out
                        if(retval != null)
                            break;
                    }
                    break;
            }

            return retval;
        }

        /// <summary>
        /// Gets show information for a given show / season / episode
        /// </summary>
        /// <param name="showName">The TV show name to get information for</param>
        /// <param name="season">The show season</param>
        /// <param name="episode">The show episode within the season</param>
        /// <returns></returns>
        public TVEpisodeInfo GetEpisodeInfo(string showName, int season, int episode)
        {
            //  Create our new episode information:
            TVEpisodeInfo retval = null;

            foreach(var provider in SEShowInfoProviders)
            {
                retval = provider.GetShowInfo(showName, season, episode);

                //  If we found our information, get out
                if(retval != null)
                    break;
            }

            return retval;
        }

        /// <summary>
        /// Get show information for a given show / airdate
        /// </summary>
        /// <param name="showName">The TV show name to get information for</param>
        /// <param name="year">The show airdate year</param>
        /// <param name="month">The show airdate month</param>
        /// <param name="day">The show airdate day</param>
        /// <returns></returns>
        public TVEpisodeInfo GetEpisodeInfo(string showName, int year, int month, int day)
        {
            //  Create our new episode information:
            TVEpisodeInfo retval = null;

            foreach(var provider in ADShowInfoProviders)
            {
                retval = provider.GetShowInfo(showName, year, month, day);

                //  If we found our information, get out
                if(retval != null)
                    break;
            }

            return retval;
        }
    }

    /// <summary>
    /// The different ways a filename can be parsed
    /// </summary>
    enum ParseType
    {
        Unknown,
        SeasonEpisode,
        Airdate
    }
}
