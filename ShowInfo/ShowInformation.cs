﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ShowInfoProvider;
using ServiceStack;
using System.Composition.Convention;
using System.Runtime.Loader;
using System.Composition.Hosting;

namespace ShowInfo
{
	public static class ContainerConfigurationExtensions
	{
		public static ContainerConfiguration WithAssembliesInPath(this ContainerConfiguration configuration, string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			return WithAssembliesInPath(configuration, path, null, searchOption);
		}

		public static ContainerConfiguration WithAssembliesInPath(this ContainerConfiguration configuration, string path, AttributedModelProvider conventions, SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			var assemblies = Directory
				.GetFiles(path, "*.dll", searchOption)
				.Select(AssemblyLoadContext.GetAssemblyName)
				.Select(AssemblyLoadContext.Default.LoadFromAssemblyName)
				.ToList();

			configuration = configuration.WithAssemblies(assemblies, conventions);

			return configuration;
		}
	}

	public class ShowInformationManager
	{
		#region MEF helpers and imports

		[ImportMany]
		public IEnumerable<ISeasonEpisodeShowInfoProvider> SEShowInfoProviders { get; set; }

		[ImportMany]
		public IEnumerable<IAirdateShowInfoProvider> ADShowInfoProviders { get; set; }


		private void Compose()
		{
			var conventions = new ConventionBuilder();
			conventions.ForTypesDerivedFrom<ISeasonEpisodeShowInfoProvider>()
				.Export<ISeasonEpisodeShowInfoProvider>()
				.Shared();
			conventions.ForTypesDerivedFrom<IAirdateShowInfoProvider>()
				.Export<IAirdateShowInfoProvider>()
				.Shared();

			var configuration = new ContainerConfiguration().WithAssembliesInPath(".", conventions);

			CompositionHost ch = configuration.CreateContainer();
			//var catalog = new AggregateCatalog();

			////  Add the current directory
			//catalog.Catalogs.Add(new DirectoryCatalog(@"./"));

			////  Add the directory in the config file if it exists:
			//string pluginDir = ConfigurationManager.AppSettings["PluginDirectory"];
			//if(!string.IsNullOrEmpty(pluginDir))
			//    catalog.Catalogs.Add(new DirectoryCatalog(pluginDir));

			//var container = new CompositionContainer(catalog);
			//container.ComposeParts(this);
		}

		#endregion

		private string currentPath = string.Empty;

		#region Aliases

		/// <summary>
		/// Our list of customizable show 'aliases'
		/// </summary>
		private List<ShowAlias> showAliases = new List<ShowAlias>();

		/// <summary>
		/// Loads the aliases from the alias file.  This allows us to map
		/// one TVshow name to another -- like 'Castle' to 'Castle (2009)'
		/// </summary>
		private void LoadAliases()
		{
			//  If the show alias file exists, load it up:
			currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			string data = string.Empty;

			try
			{
				//string aliasFile = ConfigurationManager.AppSettings["AliasFile"];
				//if(string.IsNullOrEmpty(aliasFile))
				string aliasFile = "showalias.json";

				//  If the alias file exists, open it:
				if (File.Exists(Path.Combine(currentPath, aliasFile)))
				{
					//  Open the file and load the aliases
					data = File.ReadAllText(Path.Combine(currentPath, aliasFile));
					showAliases = data.FromJson<List<ShowAlias>>();
				}
			}
			catch (Exception ex)
			{
				/* Silently fail */
			}
		}

		/// <summary>
		/// If an alias exists, the show's alias is returned.  If the show has no alias,
		/// the original show name is returned
		/// </summary>
		/// <param name="showName">The show name to check</param>
		/// <returns></returns>
		private string ResolveShowToAlias(string showName)
		{
			string retval = showName;

			//  If the given show has an alias, return it
			if (this.showAliases.Where(s => s.Show.Equals(showName, StringComparison.OrdinalIgnoreCase)).Any())
			{
				retval = showAliases.Where(s => s.Show.Equals(showName, StringComparison.OrdinalIgnoreCase)).Select(s => s.Alias).FirstOrDefault();
			}

			return retval;
		}

		#endregion

		/// <summary>
		/// Default constructor
		/// </summary>
		public ShowInformationManager(IEnumerable<ISeasonEpisodeShowInfoProvider> seasonShowInfoProviders, IEnumerable<IAirdateShowInfoProvider> airshowInfoProviders, IAliasProvider aliasProvider)
		{
			//  Load our aliases
			showAliases = aliasProvider.ShowAliases.ToList();

			//  Load our provider plugins
			//Compose();
			SEShowInfoProviders = seasonShowInfoProviders;
			ADShowInfoProviders = airshowInfoProviders;
		}

		#region API methods

		/// <summary>
		/// Gets show information for the given filename
		/// </summary>
		/// <param name="filename">The filename to get TV show information for</param>
		/// <returns></returns>
		public TVEpisodeInfo GetEpisodeInfoForFilename(string filename)
		{
			//  Create our new episode information:
			TVEpisodeInfo retval = null;

			string parsedShowName = string.Empty;
			int parsedSeasonNumber = 0;
			int parsedEpisodeNumber = 0;

			int parsedAirdateYear = 0;
			int parsedAirdateMonth = 1;
			int parsedAirdateDay = 1;

			//  Make sure we're just using a filename - not an entire path:
			filename = Path.GetFileName(filename);

			/******* PARSE THE FILENAME ********/

			//  Season/Episode parsers
			Regex rxSE = new Regex(@"^((?<series_name>.+?)[. _-]+)?s(?<season_num>\d+)[. _-]*e(?<ep_num>\d+)(([. _-]*e|-)(?<extra_ep_num>(?!(1080|720)[pi])\d+))*[. _-]*((?<extra_info>.+?)((?<![. _-])-(?<release_group>[^-]+))?)?$", RegexOptions.IgnoreCase);
			Regex rxSE2 = new Regex(@"(?<series_name>.*?)\.S?(?<season_num>\d{1,2})[Ex-]?(?<ep_num>\d{2})\.(.*)", RegexOptions.IgnoreCase);

			//  Airdate parsers
			Regex rxD = new Regex(@"^((?<series_name>.+?)[. _-]+)(?<year>\d{4}).(?<month>\d{1,2}).(?<day>\d{1,2})", RegexOptions.IgnoreCase);

			//  Process our regexes:
			var seMatch = rxSE.Match(filename);
			var seMatch2 = rxSE2.Match(filename);
			var dMatch = rxD.Match(filename);
			var currentParseType = ParseType.Unknown;

			//  See how we made out...
			if (seMatch.Success)
			{
				currentParseType = ParseType.SeasonEpisode;
				parsedShowName = Regex.Replace(seMatch.Groups["series_name"].ToString().Trim(), @"[\W]|_", " ");
				parsedSeasonNumber = Convert.ToInt32(seMatch.Groups["season_num"].Value);
				parsedEpisodeNumber = Convert.ToInt32(seMatch.Groups["ep_num"].Value);
			}
			//  See if we have a year/month/day match:
			else if (dMatch.Success)
			{
				currentParseType = ParseType.Airdate;
				parsedShowName = Regex.Replace(dMatch.Groups["series_name"].ToString().Trim(), @"[\W]|_", " ");
				parsedAirdateYear = Convert.ToInt32(dMatch.Groups["year"].Value);
				parsedAirdateMonth = Convert.ToInt32(dMatch.Groups["month"].Value);
				parsedAirdateDay = Convert.ToInt32(dMatch.Groups["day"].Value);
			}
			//  Try an alternate S/E match
			else if (seMatch2.Success)
			{
				currentParseType = ParseType.SeasonEpisode;
				parsedShowName = Regex.Replace(seMatch2.Groups["series_name"].ToString().Trim(), @"[\W]|_", " ");
				parsedSeasonNumber = Convert.ToInt32(seMatch2.Groups["season_num"].Value);
				parsedEpisodeNumber = Convert.ToInt32(seMatch2.Groups["ep_num"].Value);
			}


			//  Resolve the show alias (if it exists)
			parsedShowName = ResolveShowToAlias(parsedShowName);

			//  Based on the type of information parsed, use either 
			//  season/episode data providers or airdate data providers
			switch (currentParseType)
			{
				case ParseType.SeasonEpisode:
					//  If it parses to a show/season/episode, get 
					//  information from the SEShowInfoProviders
					foreach (var provider in SEShowInfoProviders)
					{
						retval = provider.GetShowInfo(parsedShowName, parsedSeasonNumber, parsedEpisodeNumber);

						//  If we found our information, get out
						if (retval != null)
							break;
					}
					break;
				case ParseType.Airdate:
					//  If it parses to show/airdate, get information
					//  from the ADShowInfoProviders
					foreach (var provider in ADShowInfoProviders)
					{
						retval = provider.GetShowInfo(parsedShowName, parsedAirdateYear, parsedAirdateMonth, parsedAirdateDay);

						//  If we found our information, get out
						if (retval != null)
							break;
					}
					break;
			}

			return retval;
		}

		/// <summary>
		/// Gets show information for a given show / season / episode
		/// </summary>
		/// <param name="showName">The TV show name to get information for</param>
		/// <param name="season">The show season</param>
		/// <param name="episode">The show episode within the season</param>
		/// <returns></returns>
		public TVEpisodeInfo GetEpisodeInfo(string showName, int season, int episode)
		{
			//  Create our new episode information:
			TVEpisodeInfo retval = null;

			//  Resolve the show alias (if it exists)
			showName = ResolveShowToAlias(showName);

			foreach (var provider in SEShowInfoProviders)
			{
				retval = provider.GetShowInfo(showName, season, episode);

				//  If we found our information, get out
				if (retval != null)
					break;
			}

			return retval;
		}

		/// <summary>
		/// Get show information for a given show / airdate
		/// </summary>
		/// <param name="showName">The TV show name to get information for</param>
		/// <param name="year">The show airdate year</param>
		/// <param name="month">The show airdate month</param>
		/// <param name="day">The show airdate day</param>
		/// <returns></returns>
		public TVEpisodeInfo GetEpisodeInfo(string showName, int year, int month, int day)
		{
			//  Create our new episode information:
			TVEpisodeInfo retval = null;

			//  Resolve the show alias (if it exists)
			showName = ResolveShowToAlias(showName);

			foreach (var provider in ADShowInfoProviders)
			{
				retval = provider.GetShowInfo(showName, year, month, day);

				//  If we found our information, get out
				if (retval != null)
					break;
			}

			return retval;
		}

		/// <summary>
		/// Gets all episodes for a given show
		/// </summary>
		/// <param name="showName">The TV show name to get information for</param>
		/// <returns></returns>
		public TVSeriesInfo GetSeriesInfo(string showName)
		{
			TVSeriesInfo seriesInfo = null;

			//  Resolve the show alias (if it exists)
			showName = ResolveShowToAlias(showName);

			foreach (var provider in SEShowInfoProviders)
			{
				seriesInfo = provider.GetSeriesInfo(showName);

				//  If we found our information, get out
				if (seriesInfo != null)
					break;
			}

			return seriesInfo;
		}

		#endregion


	}

	/// <summary>
	/// The different ways a filename can be parsed
	/// </summary>
	enum ParseType
	{
		Unknown,
		SeasonEpisode,
		Airdate
	}
}
