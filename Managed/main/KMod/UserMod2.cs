using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace KMod
{
	public class UserMod2
	{
		public Assembly assembly
		{
			get;
			set;
		}

		public string path
		{
			get;
			set;
		}

		public Mod mod
		{
			get;
			set;
		}

		public virtual void OnLoad(Harmony harmony)
		{
			harmony.PatchAll(assembly);
		}

		public virtual void OnAllModsLoaded(Harmony harmony, IReadOnlyList<Mod> mods)
		{
		}
	}
}
