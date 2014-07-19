using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowInfo
{
    /// <summary>
    /// Uses the ESPN API to parse a given filename and find team information
    /// </summary>
    public class SportInformationParser
    {
        /// <summary>
        /// The filename to parse
        /// </summary>
        private string _filename = string.Empty;

        /// <summary>
        /// The API key for ESPN.  If this isn't supplied, no 
        /// searches can take place.  All matches will fail silently
        /// </summary>
        private string _apiKey = string.Empty;

        /// <summary>
        /// Constructs a sport information parser using the given ESPN API key
        /// </summary>
        /// <param name="apiKey">The ESPN api key to use</param>
        public SportInformationParser(string apiKey)
        {
            this._apiKey = apiKey;
        }

        /// <summary>
        /// Returns 'true' if we can find a match, false if we can't
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public SportInformationMatch Match(string filename)
        {
            this._filename = filename;
        }
    }

    public class SportInformationMatch
    {
        public bool Success { get; set; }
        public List<string> Teams { get; set; }
    }

    /// <summary>
    /// An enumeration of supported sports types
    /// </summary>
    public enum SportType
    {
        CollegeFootball,
        NFLFootball
    }
}
