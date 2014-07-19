TVshow-info
===========

A .NET class library for getting information about a TV show episode.  

Uses a [MEF](http://msdn.microsoft.com/en-us/library/dd460648.aspx)-based providers to get episode information.  The library ships with providers for [Rovi](http://developer.rovicorp.com/) and [TheTVDB](http://thetvdb.com/wiki/index.php?title=Programmers_API).  Want to provide information using a different online source?  You can create your own pluggable provider!

[![Build status](https://ci.appveyor.com/api/projects/status/b778xp0kh3gr6vr7)](https://ci.appveyor.com/project/danesparza/tvshow-info)

### Quick Start

Install the [NuGet package](https://www.nuget.org/packages/TV-show-info/) from the package manager console:
```powershell
Install-Package TV-show-info
```

Make sure the provider plugin .dlls are in the same directory as your executing assembly.  TVShow-info ships with 2 providers: Rovi and TheTVDB.  REMOVE ANY PROVIDER .DLLS YOU DON'T WANT TO USE -- otherwise they will get loaded automatically using MEF.

Set the appropriate configuration values for your providers (like API keys) in the app.config or web.config.
- For TheTVDB:

```xml
<appSettings>
    <!-- You can leave this blank if all data provider .dlls reside in your application directory -->
    <add key="PluginDirectory" value=""/>

    <!-- TheTVDB API information -->
    <add key="TheTVDB_APIKey" value="YOUR_API_KEY"/>
</appSettings>
```        
- For Rovi
```xml
<appSettings>
    <!-- You can leave this blank if all data provider .dlls reside in your application directory -->
    <add key="PluginDirectory" value=""/>
      
    <!-- Rovi API information -->
    <add key="Rovi_APIKey" value="YOUR_API_KEY"/>
    <add key="Rovi_APISecret" value="YOUR_API_SECRET"/>
</appSettings>
```

### Examples

```CSharp
ShowInformationManager showMgr = new ShowInformationManager();
string filename = "Once.Upon.a.Time.S03E01.720p.HDTV.X264-DIMENSION.mkv";

TVEpisodeInfo episode = showMgr.GetEpisodeInfoForFilename(filename);

//  Assert
Assert.AreEqual<int>(3, episode.SeasonNumber);
Assert.AreEqual<int>(1, episode.EpisodeNumber);
Assert.AreEqual<string>("Once Upon a Time (2011)", episode.ShowName); /* Changes when using an alias */
Assert.AreEqual<string>("The Heart of the Truest Believer", episode.EpisodeTitle);
```
