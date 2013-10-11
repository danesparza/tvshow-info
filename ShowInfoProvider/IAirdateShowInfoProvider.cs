using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowInfoProvider
{
    public interface IAirdateShowInfoProvider
    {
        TVEpisodeInfo GetShowInfo(string showName, int year, int month, int day);
    }
}
