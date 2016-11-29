using ShowInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowInfo
{
    public class AliasProvider : IAliasProvider
    {
		public IEnumerable<ShowAlias> ShowAliases { get; private set; }


		public AliasProvider()
		{
		//	"Show" : "Once Upon a Time",
		//"Alias" : "Once Upon a Time (2011)"
			ShowAlias showAlias = new ShowInfo.ShowAlias();
			showAlias.Alias = "Once Upon a Time (2011)";
			showAlias.Show = "Once Upon a Time";

			List<ShowAlias> showAliases = new List<ShowAlias>();
			showAliases.Add(showAlias);

			ShowAliases = showAliases;
		}
    }
}
