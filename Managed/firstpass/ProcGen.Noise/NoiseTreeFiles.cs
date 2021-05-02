using System.Collections.Generic;
using Klei;
using LibNoiseDotNet.Graphics.Tools.Noise;

namespace ProcGen.Noise
{
	public class NoiseTreeFiles
	{
		public static string NOISE_FILE = "noise";

		private Dictionary<string, Tree> trees;

		public List<string> tree_files
		{
			get;
			set;
		}

		public static string GetDirectoryRel()
		{
			return "worldgen/noise/";
		}

		public static string GetPathRel()
		{
			return "worldgen/" + NOISE_FILE + ".yaml";
		}

		public static string GetTreeFilePathRel(string filename)
		{
			return "worldgen/noise/" + filename + ".yaml";
		}

		public void Clear()
		{
			tree_files.Clear();
			trees.Clear();
		}

		public NoiseTreeFiles()
		{
			trees = new Dictionary<string, Tree>();
			tree_files = new List<string>();
		}

		public Tree LoadTree(string name)
		{
			if (name != null && name.Length > 0)
			{
				if (!trees.ContainsKey(name))
				{
					Tree tree = YamlIO.LoadFile<Tree>(SettingsCache.RewriteWorldgenPathYaml(name));
					if (tree != null)
					{
						trees.Add(name, tree);
					}
				}
				return trees[name];
			}
			return null;
		}

		public float GetZoomForTree(string name)
		{
			if (!trees.ContainsKey(name))
			{
				return 1f;
			}
			return trees[name].settings.zoom;
		}

		public bool ShouldNormaliseTree(string name)
		{
			if (!trees.ContainsKey(name))
			{
				return false;
			}
			return trees[name].settings.normalise;
		}

		public string[] GetTreeNames()
		{
			string[] array = new string[trees.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, Tree> tree in trees)
			{
				array[num++] = tree.Key;
			}
			return array;
		}

		public Tree GetTree(string name)
		{
			if (!trees.ContainsKey(name))
			{
				string text = SettingsCache.RewriteWorldgenPathYaml(name);
				Tree tree = YamlIO.LoadFile<Tree>(text);
				if (tree == null)
				{
					DebugUtil.LogArgs("NoiseArgs.GetTree failed to load " + name + " at " + text);
					return null;
				}
				trees.Add(name, tree);
			}
			return trees[name];
		}

		public IModule3D BuildTree(string name, int globalSeed)
		{
			if (!trees.ContainsKey(name))
			{
				return null;
			}
			return trees[name].BuildFinalModule(globalSeed);
		}
	}
}
