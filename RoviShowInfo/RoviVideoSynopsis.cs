using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoviShowInfo
{
    /// <summary>
    /// A short synopsis of the associated video
    /// </summary>
    [DataContract]
    public class RoviVideoSynopsis
    {
        /// <summary>
        /// The full synopsis text.  If you want a shorter synopsis, use the
        /// 'cuttingPositions' property to determine what lengths of synopsis you
        /// can use
        /// </summary>
        [DataMember(Name = "synopsis")]
        public string SynopsisText { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }

        /// <summary>
        /// The language of the synopsis
        /// </summary>
        [DataMember(Name = "language")]
        public string Language { get; set; }

        /// <summary>
        /// The cutting positions of the synopsis text.  Use these as good stopping points
        /// for shorter synopsis
        /// </summary>
        [DataMember(Name = "cuttingPositions")]
        public string CuttingPositions { get; set; }
    }
}
