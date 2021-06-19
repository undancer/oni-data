using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Klei;
using UnityEngine;

namespace ProcGen
{
	public class ClusterLayout
	{
		public enum Skip
		{
			Never = 0,
			Always = 99,
			EditorOnly = 100
		}

		public enum ClusterCategory
		{
			vanilla,
			spacedOutVanillaStyle,
			spacedOutStyle
		}

		public const string directory = "clusters";

		public string filePath;

		public List<WorldPlacement> worldPlacements
		{
			get;
			set;
		}

		public List<SpaceMapPOIPlacement> poiPlacements
		{
			get;
			set;
		}

		public string name
		{
			get;
			set;
		}

		public string description
		{
			get;
			set;
		}

		public string requiredDlcId
		{
			get;
			set;
		}

		public string forbiddenDlcId
		{
			get;
			set;
		}

		public int difficulty
		{
			get;
			set;
		}

		public Skip skip
		{
			get;
			private set;
		}

		public int clusterCategory
		{
			get;
			private set;
		}

		public int startWorldIndex
		{
			get;
			set;
		}

		public int width
		{
			get;
			set;
		}

		public int height
		{
			get;
			set;
		}

		public int numRings
		{
			get;
			set;
		}

		public int menuOrder
		{
			get;
			set;
		}

		public string coordinatePrefix
		{
			get;
			private set;
		}

		public ClusterLayout()
		{
			numRings = 12;
		}

		public static string GetName(string path, string addPrefix)
		{
			string filename = System.IO.Path.Combine(addPrefix + "clusters", System.IO.Path.GetFileNameWithoutExtension(path));
			return FileSystem.Normalize(filename);
		}

		public string GetStartWorld()
		{
			return worldPlacements[startWorldIndex].world;
		}

		public string GetCoordinatePrefix()
		{
			if (string.IsNullOrEmpty(coordinatePrefix))
			{
				string text = "";
				string[] array = Strings.Get(name).String.Split(' ');
				int a = 5 - array.Length;
				bool flag = true;
				string[] array2 = array;
				foreach (string input in array2)
				{
					if (!flag)
					{
						text += "-";
					}
					string text2 = Regex.Replace(input, "(a|e|i|o|u)", "");
					text += text2.Substring(0, Mathf.Min(a, text2.Length)).ToUpper();
					flag = false;
				}
				coordinatePrefix = text;
			}
			return coordinatePrefix;
		}
	}
}
