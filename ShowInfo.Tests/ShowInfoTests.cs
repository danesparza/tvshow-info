using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShowInfoProvider;
using System.Diagnostics;
using Moq;
using TheTVDBShowInfo;

namespace ShowInfo.Tests
{
	[TestClass]
	public class ShowInfoTests
	{
		IEnumerable<IAirdateShowInfoProvider> GetRealAirDateShowInfoProviders()
		{
			List<IAirdateShowInfoProvider> providers = new List<IAirdateShowInfoProvider>();
			providers.Add(new TheTVDBInfoProvider("6884BE73E9F74743"));
			return providers;
		}

		IEnumerable<ISeasonEpisodeShowInfoProvider> GetRealSeasonEpisodeShowInfoProivders()
		{
			List<ISeasonEpisodeShowInfoProvider> providers = new List<ISeasonEpisodeShowInfoProvider>();
			providers.Add(new TheTVDBInfoProvider("6884BE73E9F74743"));
			return providers;
		}

		IAliasProvider GetShowAliasProvider()
		{
			Mock<IAliasProvider> aliasProvider = new Mock<IAliasProvider>();
			//	"Show" : "Once Upon a Time",
			//"Alias" : "Once Upon a Time (2011)"
			ShowAlias showAlias = new ShowInfo.ShowAlias();
			showAlias.Alias = "Once Upon a Time (2011)";
			showAlias.Show = "Once Upon a Time";

			var aliases = new List<ShowAlias>();
			aliases.Add(showAlias);

			showAlias = new ShowInfo.ShowAlias();
			showAlias.Alias = "Castle";
			showAlias.Show = "Castle 2009";
			aliases.Add(showAlias);

			aliasProvider.Setup(ap => ap.ShowAliases).Returns(aliases);

			return aliasProvider.Object;
		}

		[TestMethod]
		public void ForFilename_WithValidSEFileName_ReturnsTVEpisodeInfo()
		{
			//  Arrange
			ShowInformationManager showMgr = new ShowInformationManager(GetRealSeasonEpisodeShowInfoProivders(),
																		GetRealAirDateShowInfoProviders(),
																		GetShowAliasProvider());

			string filename = "Once.Upon.a.Time.S03E01.720p.HDTV.X264-DIMENSION.mkv";

			//  Act
			TVEpisodeInfo episode = showMgr.GetEpisodeInfoForFilename(filename);

			//  Assert
			Assert.AreEqual<int>(3, episode.SeasonNumber);
			Assert.AreEqual<int>(1, episode.EpisodeNumber);
			Assert.AreEqual<string>("Once Upon a Time (2011)", episode.ShowName);
			Assert.AreEqual<string>("The Heart of the Truest Believer", episode.EpisodeTitle);
		}

		[TestMethod]
		public void ForFilename_PR_ReturnsTVEpisodeInfo()
		{
			//  Arrange
			ShowInformationManager showMgr = new ShowInformationManager(GetRealSeasonEpisodeShowInfoProivders(),
																		GetRealAirDateShowInfoProviders(),
																		GetShowAliasProvider());
			string filename = "Parks.and.Recreation.S06E05.HDTV.x264-LOL.mp4";

			//  Act
			TVEpisodeInfo episode = showMgr.GetEpisodeInfoForFilename(filename);

			//  Assert
			Assert.AreEqual<int>(6, episode.SeasonNumber);
			Assert.AreEqual<int>(5, episode.EpisodeNumber);
			Assert.AreEqual<string>("Parks and Recreation", episode.ShowName);
			Assert.AreEqual<string>("Gin It Up!", episode.EpisodeTitle);
		}

		[TestMethod]
		public void ForFilename_CollegeFootball_ReturnsTVEpisodeInfo()
		{
			//  Arrange
			ShowInformationManager showMgr = new ShowInformationManager(GetRealSeasonEpisodeShowInfoProivders(),
																		GetRealAirDateShowInfoProviders(),
																		GetShowAliasProvider());
			string filename = "Purdue Boilermakers - Ohio State Buckeyes 02.11.13.mkv";

			//  Act
			TVEpisodeInfo episode = showMgr.GetEpisodeInfoForFilename(filename);

			//  Assert
			Assert.AreEqual<int>(6, episode.SeasonNumber);
			Assert.AreEqual<int>(5, episode.EpisodeNumber);
			Assert.AreEqual<string>("Parks and Recreation", episode.ShowName);
			Assert.AreEqual<string>("Gin It Up!", episode.EpisodeTitle);
		}

		[TestMethod]
		public void ForFilenameAlt_WithValidSEFileName_ReturnsTVEpisodeInfo()
		{
			//  Arrange
			ShowInformationManager showMgr = new ShowInformationManager(GetRealSeasonEpisodeShowInfoProivders(),
																		GetRealAirDateShowInfoProviders(),
																		GetShowAliasProvider());
			string filename = "once.upon.a.time.305.hdtv-lol.mp4";

			//  Act
			TVEpisodeInfo episode = showMgr.GetEpisodeInfoForFilename(filename);

			//  Assert
			Assert.AreEqual<int>(3, episode.SeasonNumber);
			Assert.AreEqual<int>(5, episode.EpisodeNumber);
			Assert.AreEqual<string>("Once Upon a Time (2011)", episode.ShowName);
			Assert.AreEqual<string>("Good Form", episode.EpisodeTitle);
		}

		[TestMethod]
		public void ForFilename_WithValidAirdateFileName_ReturnsTVEpisodeInfo()
		{
			//  Arrange
			ShowInformationManager showMgr = new ShowInformationManager(GetRealSeasonEpisodeShowInfoProivders(),
																		GetRealAirDateShowInfoProviders(),
																		GetShowAliasProvider());
			string filename = "Colbert.Report.2013.10.10.Reed.Albergotti.and.Vanessa.OConnell.HDTV.x264-LMAO.mp4";

			//  Act
			TVEpisodeInfo episode = showMgr.GetEpisodeInfoForFilename(filename);

			//  Assert
			Assert.AreEqual<int>(10, episode.SeasonNumber);
			Assert.AreEqual<int>(8, episode.EpisodeNumber);
			Assert.AreEqual<string>("The Colbert Report", episode.ShowName);
			Assert.AreEqual<string>("Reed Albergotti & Vanessa O'Connell", episode.EpisodeTitle);
		}

		[TestMethod]
		public void ForFullPath_WithValidAirdateFileName_ReturnsTVEpisodeInfo()
		{
			//  Arrange
			ShowInformationManager showMgr = new ShowInformationManager(GetRealSeasonEpisodeShowInfoProivders(),
																		GetRealAirDateShowInfoProviders(),
																		GetShowAliasProvider());
			string filename = @"C:\Users\desparza\Downloads\The.Colbert.Report.2013.10.10.Reed.Albergotti.and.Vanessa.OConnell.HDTV.x264-LMAO.mp4";

			//  Act
			TVEpisodeInfo episode = showMgr.GetEpisodeInfoForFilename(filename);

			//  Assert
			Assert.AreEqual<int>(10, episode.SeasonNumber);
			Assert.AreEqual<int>(8, episode.EpisodeNumber);
			Assert.AreEqual<string>("The Colbert Report", episode.ShowName);
			Assert.AreEqual<string>("Reed Albergotti & Vanessa O'Connell", episode.EpisodeTitle);
		}

		[TestMethod]
		public void GetEpisode_WithValidSeasonEpisode_ReturnsTVEpisode()
		{
			//  Arrange
			ShowInformationManager showMgr = new ShowInformationManager(GetRealSeasonEpisodeShowInfoProivders(),
																		GetRealAirDateShowInfoProviders(),
																		GetShowAliasProvider());

			//  Act
			TVEpisodeInfo episode = showMgr.GetEpisodeInfo("The Colbert Report", 10, 1);

			//  Assert
			Assert.AreEqual<int>(10, episode.SeasonNumber);
			Assert.AreEqual<int>(1, episode.EpisodeNumber);
			Assert.AreEqual<string>("The Colbert Report", episode.ShowName);
			Assert.AreEqual<string>("Vince Gilligan", episode.EpisodeTitle);
		}

		[TestMethod]
		public void GetEpisode_WithValidAirdate_ReturnsTVEpisode()
		{
			//  Arrange
			ShowInformationManager showMgr = new ShowInformationManager(GetRealSeasonEpisodeShowInfoProivders(),
																		GetRealAirDateShowInfoProviders(),
																		GetShowAliasProvider());

			//  Act
			TVEpisodeInfo episode = showMgr.GetEpisodeInfo("The Colbert Report", 2013, 9, 30);

			//  Assert
			Assert.AreEqual<int>(10, episode.SeasonNumber);
			Assert.AreEqual<int>(1, episode.EpisodeNumber);
			Assert.AreEqual<string>("The Colbert Report", episode.ShowName);
			Assert.AreEqual<string>("Vince Gilligan", episode.EpisodeTitle);
		}

		[TestMethod]
		public void GetEpisode_WithAliasValidSeasonEpisode_ReturnsTVEpisode()
		{
			//  Arrange
			ShowInformationManager showMgr = new ShowInformationManager(GetRealSeasonEpisodeShowInfoProivders(),
																		GetRealAirDateShowInfoProviders(),
																		GetShowAliasProvider());

			//  Act
			TVEpisodeInfo episode = showMgr.GetEpisodeInfo("Once Upon a Time", 3, 2);

			//  Assert
			Assert.AreEqual<int>(3, episode.SeasonNumber);
			Assert.AreEqual<int>(2, episode.EpisodeNumber);
			Assert.AreEqual<string>("Once Upon a Time (2011)", episode.ShowName); /* Notice this changes */
			Assert.AreEqual<string>("Lost Girl", episode.EpisodeTitle);
		}

		[TestMethod]
		public void GetAllEpisodes_WithValidShowSeason_ReturnsAllEpisodes()
		{
			//  Arrange
			ShowInformationManager showMgr = new ShowInformationManager(GetRealSeasonEpisodeShowInfoProivders(),
																		GetRealAirDateShowInfoProviders(),
																		GetShowAliasProvider());

			//  Act
			TVSeriesInfo seriesInfo = showMgr.GetSeriesInfo("Once Upon a Time");

			//  Assert
			foreach (var seasonGroup in seriesInfo.Seasons)
			{
				Debug.WriteLine("Season number: {0}", seasonGroup.Key);

				foreach (TVEpisodeInfo episode in seasonGroup)
				{
					Debug.WriteLine("Ep {0}: {1}", episode.EpisodeNumber, episode.EpisodeTitle);
				}
			}
		}
	}
}
