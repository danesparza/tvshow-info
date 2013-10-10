using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoviShowInfo
{
    [DataContract]
    public class RoviTVEpisodeInfo
    {
        [DataMember(Name = "masterTitle")]
        public string ShowTitle { get; set; }

        [DataMember(Name = "episodeTitle")]
        public string EpisodeTitle { get; set; }

        [DataMember(Name = "originalAirDate")]
        public DateTime OriginalAirDate { get; set; }

        [DataMember(Name = "synopsis")]
        public RoviVideoSynopsis Synopsis { get; set; }

        [DataMember(Name = "tvRating")]
        public string TVRating { get; set; }
    }
}
