using System.Collections.Generic;
using ShowInfo;

namespace ShowInfo
{
	public interface IAliasProvider
	{
		IEnumerable<ShowAlias> ShowAliases { get; }
	}
}