using System.Collections.Generic;
using System.IO;
using Klei;
using UnityEngine;

namespace ProcGen
{
	public class Worlds
	{
		public Dictionary<string, World> worldCache = new Dictionary<string, World>();

		public bool HasWorld(string name)
		{
			if (name == null)
			{
				return false;
			}
			return worldCache.ContainsKey(name);
		}

		public World GetWorldData(string name)
		{
			if (!name.IsNullOrWhiteSpace() && worldCache.TryGetValue(name, out var value))
			{
				return value;
			}
			return null;
		}

		public List<string> GetNames()
		{
			return new List<string>(worldCache.Keys);
		}

		public static string GetWorldName(string path, string prefix)
		{
			return prefix + "worlds/" + System.IO.Path.GetFileNameWithoutExtension(path);
		}

		public string GetIconFilename(string iconName)
		{
			if (!DlcManager.FeatureClusterSpaceEnabled())
			{
				return "Asteroid_sandstone";
			}
			return "asteroid_sandstone_start_kanim";
		}

		public void LoadReferencedWorlds(string path, string prefix, ISet<string> referencedWorlds, List<YamlIO.Error> errors)
		{
			UpdateWorldCache(path, prefix, referencedWorlds, errors);
		}

		private void UpdateWorldCache(string path, string prefix, ISet<string> referencedWorlds, List<YamlIO.Error> errors)
		{
			ListPool<FileHandle, Worlds>.PooledList pooledList = ListPool<FileHandle, Worlds>.Allocate();
			FileSystem.GetFiles(FileSystem.Normalize(System.IO.Path.Combine(path, "worlds/")), "*.yaml", pooledList);
			foreach (FileHandle item2 in pooledList)
			{
				string text = item2.full_path.Substring(path.Length);
				text = text.Remove(text.LastIndexOf(".yaml"));
				string item = prefix + text;
				if (referencedWorlds.Contains(item))
				{
					World world = YamlIO.LoadFile<World>(item2.full_path, delegate(YamlIO.Error error, bool force_log_as_warning)
					{
						errors.Add(error);
					});
					if (world == null)
					{
						DebugUtil.LogWarningArgs("Failed to load world: ", item2.full_path);
					}
					else if (world.skip != World.Skip.Always && (world.skip != World.Skip.EditorOnly || Application.isEditor))
					{
						world.filePath = GetWorldName(item2.full_path, prefix);
						worldCache[world.filePath] = world;
					}
				}
			}
			pooledList.Recycle();
		}

		public void Validate()
		{
			foreach (KeyValuePair<string, World> item in worldCache)
			{
				item.Value.Validate();
			}
		}
	}
}
