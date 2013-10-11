TVshow-info
===========

A .NET class library for getting information about a TV show episode.  

Uses a [MEF](http://msdn.microsoft.com/en-us/library/dd460648.aspx)-based providers to get episode information.  The library ships with providers for [Rovi](http://developer.rovicorp.com/) and [TheTVDB](http://thetvdb.com/wiki/index.php?title=Programmers_API).  Want to provide information using a different online source?  You can create your own pluggable provider!

Quickstart to using the library in .NET
-----------------

1. Add a reference to the library in your project
2. Set the 'PluginDirectory' configuration value in your app.config or web.config.  The plugin directory is where the information provider plugin .dll's are located.  TVShow-info ships with 2 providers: Rovi and TheTVDB.
3. Set the appropriate configuration values for your providers (like API keys) in the app.config or web.config.
4. Write code that uses the library:

	    //  Arrange
	    ShowInformationManager showMgr = new ShowInformationManager();
	    string filename = "Once.Upon.a.Time.S03E01.720p.HDTV.X264-DIMENSION.mkv";
	    
	    //  Act
	    TVEpisodeInfo episode = showMgr.GetEpisodeInfoForFilename(filename);
	    
	    //  Assert
	    Assert.AreEqual<int>(3, episode.SeasonNumber);
	    Assert.AreEqual<int>(1, episode.EpisodeNumber);
	    Assert.AreEqual<string>("Once Upon a Time", episode.ShowName);
	    Assert.AreEqual<string>("The Heart of the Truest Believer", episode.EpisodeTitle);
