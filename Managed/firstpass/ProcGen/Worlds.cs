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
			if (worldCache.TryGetValue(name, out var value))
			{
				return value;
			}
			return worldCache["worlds/SandstoneDefault"];
		}

		public List<string> GetNames()
		{
			return new List<string>(worldCache.Keys);
		}

		public static string GetWorldName(string path)
		{
			return "worlds/" + System.IO.Path.GetFileNameWithoutExtension(path);
		}

		public void LoadFiles(string path, List<YamlIO.Error> errors)
		{
			worldCache.Clear();
			UpdateWorldCache(path, errors);
		}

		private void UpdateWorldCache(string path, List<YamlIO.Error> errors)
		{
			ListPool<FileHandle, Worlds>.PooledList pooledList = ListPool<FileHandle, Worlds>.Allocate();
			FileSystem.GetFiles(FileSystem.Normalize(System.IO.Path.Combine(path, "worlds")), "*.yaml", pooledList);
			foreach (FileHandle item in pooledList)
			{
				World world = YamlIO.LoadFile<World>(item.full_path, delegate(YamlIO.Error error, bool force_log_as_warning)
				{
					errors.Add(error);
				});
				if (world == null)
				{
					DebugUtil.LogWarningArgs("Failed to load world: ", item.full_path);
				}
				else if (world.skip != World.Skip.Always && (world.skip != World.Skip.EditorOnly || Application.isEditor))
				{
					world.filePath = GetWorldName(item.full_path);
					worldCache[world.filePath] = world;
				}
			}
			pooledList.Recycle();
		}
	}
}
