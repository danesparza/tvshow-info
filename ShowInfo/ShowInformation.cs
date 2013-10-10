using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShowInfoProvider;

namespace ShowInfo
{
    public class ShowInformationManager
    {
        #region MEF helpers and imports
        
        [ImportMany]
        public IEnumerable<ISeasonEpisodeShowInfoProvider> SEShowInfoProviders { get; set; }

        private void Compose()
        {
            var catalog = new AggregateCatalog();

            //  Add the current directory
            catalog.Catalogs.Add(new DirectoryCatalog(@"./"));

            //  Add the directory in the config file if it exists:
            string pluginDir = ConfigurationManager.AppSettings["PluginDirectory"];
            if(!string.IsNullOrEmpty(pluginDir))
                catalog.Catalogs.Add(new DirectoryCatalog(pluginDir));

            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        } 

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public ShowInformationManager()
        {
            Compose();
        }

        /// <summary>
        /// Gets show information for the given filename
        /// </summary>
        /// <param name="Filename">The filename to get TV show information for</param>
        /// <returns></returns>
        public TVEpisodeInfo GetEpisodeInfoForFilename(string Filename)
        {
            //  Create our new episode information:
            TVEpisodeInfo retval = null;

            //  Parse the filename

            //  If it parses to a show/season/episode, get 
            //  information from the SEShowInfoProviders
            foreach(var provider in SEShowInfoProviders)
            {
                retval = provider.GetShowInfo("Once Upon A Time", 3, 1);

                //  If we found our information, get out
                if(retval != null)
                    break;
            }

            //  If it parses to show/airdate, get information
            //  from the ADShowInfoProviders

            return retval;
        }

        
    }
}
