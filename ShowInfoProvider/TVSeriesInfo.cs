using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowInfoProvider
{
    public class TVSeriesInfo
    {
        public string Name { get; set; }
        public IEnumerable<IGrouping<int, TVEpisodeInfo>> Seasons { get; set; }
    }
}
