﻿using System;
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
        public void ForFilename_WithValidAirdateFileName_ReturnsTVEpisodeInfo()
        {
            //  Arrange
            ShowInformationManager showMgr = new ShowInformationManager();
            string filename = "The.Colbert.Report.2013.09.30.Vince.Gilligan.HDTV.x264-2HD.mp4";

            //  Act
            TVEpisodeInfo episode = showMgr.GetEpisodeInfoForFilename(filename);

            //  Assert
            Assert.AreEqual<int>(10, episode.SeasonNumber);
            Assert.AreEqual<int>(1, episode.EpisodeNumber);
            Assert.AreEqual<string>("The Colbert Report", episode.ShowName);
            Assert.AreEqual<string>("Vince Gilligan", episode.EpisodeTitle);
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
    }
}
