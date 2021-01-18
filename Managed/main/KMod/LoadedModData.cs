using System.Collections.Generic;
using System.Reflection;

namespace KMod
{
	public class LoadedModData
	{
		public ICollection<Assembly> dlls;

		public ICollection<MethodBase> patched_methods;
	}
}
