using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShowInfoProvider;

namespace ShowInfo.Tests
{
    [TestClass]
    public class ShowInfoTests
    {
        [TestMethod]
        public void ForFilename_WithValidSEFileName_ReturnsTVEpisodeInfo()
        {
            //  Arrange
            ShowInformationManager showMgr = new ShowInformationManager();
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
            ShowInformationManager showMgr = new ShowInformationManager();
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
            ShowInformationManager showMgr = new ShowInformationManager();
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
            ShowInformationManager showMgr = new ShowInformationManager();
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
            ShowInformationManager showMgr = new ShowInformationManager();
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
            ShowInformationManager showMgr = new ShowInformationManager();
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
            ShowInformationManager showMgr = new ShowInformationManager();

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
            ShowInformationManager showMgr = new ShowInformationManager();

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
            ShowInformationManager showMgr = new ShowInformationManager();

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
            ShowInformationManager showMgr = new ShowInformationManager();

            //  Act
            List<TVEpisodeInfo> episodes = showMgr.GetAllEpisodes("Once Upon a Time").ToList();

            //  Assert
            
        }
    }
}
