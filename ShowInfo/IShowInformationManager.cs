using System.Collections.Generic;
using ShowInfoProvider;

namespace ShowInfo
{
	public interface IShowInformationManager
	{
		IEnumerable<IAirdateShowInfoProvider> ADShowInfoProviders { get; set; }
		IEnumerable<ISeasonEpisodeShowInfoProvider> SEShowInfoProviders { get; set; }

		TVEpisodeInfo GetEpisodeInfo(string showName, int season, int episode);
		TVEpisodeInfo GetEpisodeInfo(string showName, int year, int month, int day);
		TVEpisodeInfo GetEpisodeInfoForFilename(string filename);
		TVSeriesInfo GetSeriesInfo(string showName);
	}
}