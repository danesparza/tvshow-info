using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShowInfoProvider;

namespace ShowInfo.Tests
{
    [TestClass]
    public class ShowInfoTests
    {
        [TestMethod]
        public void ForFilename_WithValidFileName_ReturnsITVEpisodeInfo()
        {
            //  Arrange
            ShowInformationManager showMgr = new ShowInformationManager();
            string filename = "Once.Upon.a.Time.S03E01.720p.HDTV.X264-DIMENSION.mkv";

            //  Act
            TVEpisodeInfo episode = showMgr.GetEpisodeInfoForFilename(filename);

            //  Assert

        }
    }
}
