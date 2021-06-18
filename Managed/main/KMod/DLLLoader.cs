using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace KMod
{
	internal static class DLLLoader
	{
		private const string managed_path = "Resources/Data/Managed";

		public static bool LoadUserModLoaderDLL()
		{
			try
			{
				string path = Path.Combine(Path.Combine(Application.dataPath, "Resources/Data/Managed"), "ModLoader.dll");
				if (!File.Exists(path))
				{
					return false;
				}
				Assembly assembly = Assembly.LoadFile(path);
				if (assembly == null)
				{
					return false;
				}
				Type type = assembly.GetType("ModLoader.ModLoader");
				if (type == null)
				{
					return false;
				}
				MethodInfo method = type.GetMethod("Start");
				if (method == null)
				{
					return false;
				}
				method.Invoke(null, null);
				Debug.Log("Successfully started ModLoader.dll");
				return true;
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
			}
			return false;
		}

		public static LoadedModData LoadDLLs(Mod ownerMod, string harmonyId, string path, bool isDev)
		{
			LoadedModData loadedModData = new LoadedModData();
			try
			{
				if (Testing.dll_loading == Testing.DLLLoading.Fail)
				{
					return null;
				}
				if (Testing.dll_loading == Testing.DLLLoading.UseModLoaderDLLExclusively)
				{
					return null;
				}
				DirectoryInfo directoryInfo = new DirectoryInfo(path);
				if (!directoryInfo.Exists)
				{
					return null;
				}
				List<Assembly> list = new List<Assembly>();
				FileInfo[] files = directoryInfo.GetFiles();
				foreach (FileInfo fileInfo in files)
				{
					if (fileInfo.Name.ToLower().EndsWith(".dll"))
					{
						Debug.Log($"Loading MOD dll: {fileInfo.Name}");
						Assembly assembly = Assembly.LoadFrom(fileInfo.FullName);
						if (assembly != null)
						{
							list.Add(assembly);
						}
					}
				}
				if (list.Count == 0)
				{
					return null;
				}
				loadedModData.dlls = new HashSet<Assembly>();
				loadedModData.userMod2Instances = new Dictionary<Assembly, UserMod2>();
				foreach (Assembly item in list)
				{
					loadedModData.dlls.Add(item);
					UserMod2 userMod = null;
					Type[] types = item.GetTypes();
					foreach (Type type in types)
					{
						if (!(type == null) && typeof(UserMod2).IsAssignableFrom(type))
						{
							if (userMod != null)
							{
								Debug.LogError("Found more than one class inheriting `UserMod2` in " + item.FullName + ", only one per assembly is allowed. Aborting load.");
								return null;
							}
							userMod = Activator.CreateInstance(type) as UserMod2;
						}
					}
					if (userMod == null)
					{
						if (isDev)
						{
							Debug.LogWarning($"{item.GetName()} at {path} has no classes inheriting from UserMod, creating one...");
						}
						userMod = new UserMod2();
					}
					userMod.assembly = item;
					userMod.path = path;
					userMod.mod = ownerMod;
					loadedModData.userMod2Instances[item] = userMod;
				}
				loadedModData.harmony = new Harmony(harmonyId);
				if (loadedModData.harmony != null)
				{
					foreach (KeyValuePair<Assembly, UserMod2> userMod2Instance in loadedModData.userMod2Instances)
					{
						UserMod2 value = userMod2Instance.Value;
						value.OnLoad(loadedModData.harmony);
					}
				}
				loadedModData.patched_methods = loadedModData.harmony.GetPatchedMethods().Where(delegate(MethodBase method)
				{
					Patches patchInfo = Harmony.GetPatchInfo(method);
					return patchInfo.Owners.Contains(harmonyId);
				}).ToList();
				return loadedModData;
			}
			catch (Exception e)
			{
				DebugUtil.LogException(null, "Exception while loading mod " + harmonyId + " at " + path + ".", e);
				return null;
			}
		}

		public static void PostLoadDLLs(string harmonyId, LoadedModData modData, IReadOnlyList<Mod> mods)
		{
			try
			{
				foreach (KeyValuePair<Assembly, UserMod2> userMod2Instance in modData.userMod2Instances)
				{
					userMod2Instance.Value.OnAllModsLoaded(modData.harmony, mods);
				}
			}
			catch (Exception e)
			{
				DebugUtil.LogException(null, "Exception while postLoading mod " + harmonyId + ".", e);
			}
		}
	}
}
