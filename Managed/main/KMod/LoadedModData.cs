using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace KMod
{
	public class LoadedModData
	{
		public Harmony harmony;

		public Dictionary<Assembly, UserMod2> userMod2Instances;

		public ICollection<Assembly> dlls;

		public ICollection<MethodBase> patched_methods;
	}
}
