using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoviShowInfo
{
    [DataContract]
    public class RoviVideoResult
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "video")]
        public RoviTVEpisodeInfo EpisodeInfo { get; set; }
    }
}
