using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheTVDBShowInfo
{
    public class TVDBSeriesInfo
    {
        [XmlElement(ElementName = "seriesid")]
        public string SeriesId { get; set; }

        [XmlElement(ElementName = "language")]
        public string Language { get; set; }

        public string SeriesName { get; set; }

        [XmlElement(ElementName = "Overview")]
        public string Overview { get; set; }

        [XmlElement(ElementName = "FirstAired")]
        public DateTime FirstAirDate { get; set; }

        public string Network { get; set; }

        [XmlElement(ElementName = "IMDB_ID")]
        public string IMDBId { get; set; }

        [XmlElement(ElementName = "zap2it_id")]
        public string Zap2ItId { get; set; }

        [XmlElement(ElementName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot("Data")]
    public class TVDBSeriesResult
    {
        [XmlElement(ElementName = "Series")]
        public TVDBSeriesInfo SeriesInfo { get; set; }
    }
}
