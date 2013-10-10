using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowInfoProvider
{
    /// <summary>
    /// Information about a given TV show episode
    /// </summary>
    public class TVEpisodeInfo
    {
        /// <summary>
        /// The TV show name
        /// </summary>
        public string ShowName { get; set; }

        /// <summary>
        /// The season number
        /// </summary>
        public int SeasonNumber { get; set; }
        
        /// <summary>
        /// The episode number within the show's season
        /// </summary>
        public int EpisodeNumber { get; set; }

        /// <summary>
        /// The title of the episode
        /// </summary>
        public string EpisodeTitle { get; set; }

        /// <summary>
        /// The episode summary information
        /// </summary>
        public string EpisodeSummary { get; set; }

        /// <summary>
        /// The original airdate of the show
        /// </summary>
        public DateTime OriginalAirDate { get; set; }
    }
}
