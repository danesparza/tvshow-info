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

            //  See how we made out...
            if(seMatch.Success)
            {
                parsedShowName = Regex.Replace(seMatch.Groups["series_name"].ToString().Trim(), @"[\W]|_", " ");
                parsedSeasonNumber = Convert.ToInt32(seMatch.Groups["season_num"].Value);
                parsedEpisodeNumber = Convert.ToInt32(seMatch.Groups["ep_num"].Value);
            }
            else if(seMatch2.Success)
            {
                parsedShowName = Regex.Replace(seMatch2.Groups["series_name"].ToString().Trim(), @"[\W]|_", " ");
                parsedSeasonNumber = Convert.ToInt32(seMatch.Groups["season_num"].Value);
                parsedEpisodeNumber = Convert.ToInt32(seMatch.Groups["ep_num"].Value);
            }
            //  See if we have a year/month/day match:
            else if(dMatch.Success)
            {
                parsedShowName = Regex.Replace(dMatch.Groups["series_name"].ToString().Trim(), @"[\W]|_", " ");
                parsedAirdateYear = Convert.ToInt32(seMatch.Groups["year"].Value);
                parsedAirdateMonth = Convert.ToInt32(seMatch.Groups["month"].Value);
                parsedAirdateDay = Convert.ToInt32(seMatch.Groups["day"].Value);
            }

            //  If it parses to a show/season/episode, get 
            //  information from the SEShowInfoProviders
            if(seMatch.Success || seMatch2.Success)
            {
                foreach(var provider in SEShowInfoProviders)
                {
                    retval = provider.GetShowInfo(parsedShowName, parsedSeasonNumber, parsedEpisodeNumber);

                    //  If we found our information, get out
                    if(retval != null)
                        break;
                }
            }

            //  If it parses to show/airdate, get information
            //  from the ADShowInfoProviders
            if(dMatch.Success)
            { 
                
            }

            return retval;
        }

        
    }
}
