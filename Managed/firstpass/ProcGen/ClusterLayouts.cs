using System.Collections.Generic;
using System.IO;
using Klei;
using UnityEngine;

namespace ProcGen
{
	public class ClusterLayouts
	{
		public Dictionary<string, ClusterLayout> clusterCache = new Dictionary<string, ClusterLayout>();

		public ClusterLayout GetClusterData(string name)
		{
			if (clusterCache.TryGetValue(name, out var value))
			{
				return value;
			}
			return clusterCache[WorldGenSettings.ClusterDefaultName];
		}

		public Dictionary<string, string> GetStartingBaseNames()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, ClusterLayout> item in clusterCache)
			{
				dictionary.Add(item.Key, item.Value.worldPlacements[item.Value.startWorldIndex].world);
			}
			return dictionary;
		}

		public static string GetClusterName(string path)
		{
			return "clusters/" + System.IO.Path.GetFileNameWithoutExtension(path);
		}

		public void LoadFiles(string path, List<YamlIO.Error> errors)
		{
			clusterCache.Clear();
			UpdateClusterCache(path, errors);
		}

		private void UpdateClusterCache(string path, List<YamlIO.Error> errors)
		{
			ListPool<FileHandle, Worlds>.PooledList pooledList = ListPool<FileHandle, Worlds>.Allocate();
			FileSystem.GetFiles(FileSystem.Normalize(System.IO.Path.Combine(path, "clusters")), "*.yaml", pooledList);
			foreach (FileHandle cluster_file in pooledList)
			{
				ClusterLayout clusterLayout = YamlIO.LoadFile<ClusterLayout>(cluster_file.full_path, delegate(YamlIO.Error error, bool force_log_as_warning)
				{
					error.file = cluster_file;
					errors.Add(error);
				});
				if (clusterLayout == null)
				{
					DebugUtil.LogWarningArgs("Failed to load solar system: ", cluster_file.full_path);
				}
				else if (clusterLayout.skip != ClusterLayout.Skip.Always && (clusterLayout.skip != ClusterLayout.Skip.EditorOnly || Application.isEditor) && (clusterLayout.requiredDlcId == null || DlcManager.IsContentActive(clusterLayout.requiredDlcId)) && (clusterLayout.forbiddenDlcId == null || !DlcManager.IsContentActive(clusterLayout.forbiddenDlcId)))
				{
					string key = (clusterLayout.filePath = ClusterLayout.GetName(cluster_file.full_path));
					clusterCache[key] = clusterLayout;
				}
			}
			pooledList.Recycle();
		}

		public World GetWorldData(string clusterID, int worldID)
		{
			ClusterLayout clusterData = GetClusterData(clusterID);
			WorldPlacement worldPlacement = clusterData.worldPlacements[worldID];
			return SettingsCache.worlds.GetWorldData(worldPlacement.world);
		}
	}
}
