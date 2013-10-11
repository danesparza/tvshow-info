using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheTVDBShowInfo
{
    public class TVDBEpisodeInfo
    {
        public string EpisodeNumber { get; set; }
        public string SeasonNumber { get; set; }

        [XmlElement(ElementName = "FirstAired")]
        public DateTime OriginalAirDate { get; set; }

        public string EpisodeName { get; set; }

        [XmlElement(ElementName = "Overview")]
        public string EpisodeSummary { get; set; }
    }

    [XmlRoot("Data")]
    public class TVDBEpisodeResult
    {
        [XmlElement(ElementName = "Episode")]
        public TVDBEpisodeInfo EpisodeInfo { get; set; }
    }
}
