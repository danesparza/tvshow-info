using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowInfoProvider
{
    public interface ISeasonEpisodeShowInfoProvider
    {
        TVEpisodeInfo GetShowInfo(string showName, int season, int episode);
        IEnumerable<TVEpisodeInfo> GetAllEpisodesForShow(string showname);
    }
}
