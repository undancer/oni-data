using System;
using System.Collections.Generic;
using KSerialization.Converters;

namespace ProcGen
{
	[Serializable]
	public class FeatureSettings
	{
		[StringEnumConverter]
		public Room.Shape shape
		{
			get;
			private set;
		}

		public List<int> borders
		{
			get;
			private set;
		}

		public MinMax blobSize
		{
			get;
			private set;
		}

		public string forceBiome
		{
			get;
			private set;
		}

		public List<string> biomeTags
		{
			get;
			private set;
		}

		public List<MobReference> internalMobs
		{
			get;
			private set;
		}

		public List<string> tags
		{
			get;
			private set;
		}

		public Dictionary<string, ElementChoiceGroup<WeightedSimHash>> ElementChoiceGroups
		{
			get;
			private set;
		}

		public FeatureSettings()
		{
			ElementChoiceGroups = new Dictionary<string, ElementChoiceGroup<WeightedSimHash>>();
			borders = new List<int>();
			tags = new List<string>();
			internalMobs = new List<MobReference>();
		}

		public bool HasGroup(string item)
		{
			return ElementChoiceGroups.ContainsKey(item);
		}

		public WeightedSimHash GetOneWeightedSimHash(string item, SeededRandom rnd)
		{
			if (ElementChoiceGroups.ContainsKey(item))
			{
				return WeightedRandom.Choose(ElementChoiceGroups[item].choices, rnd);
			}
			Debug.LogError("Couldnt get SimHash [" + item + "]");
			return null;
		}

		public WeightedSimHash GetWeightedSimHashAtChoice(string item, float percentage)
		{
			if (ElementChoiceGroups.ContainsKey(item))
			{
				List<WeightedSimHash> choices = ElementChoiceGroups[item].choices;
				if (choices.Count > 0)
				{
					float num = 0f;
					for (int i = 0; i < choices.Count; i++)
					{
						num += choices[i].weight;
					}
					float num2 = 0f;
					for (int j = 0; j < choices.Count; j++)
					{
						num2 += choices[j].weight;
						if (num2 > percentage)
						{
							return choices[j];
						}
					}
					return choices[choices.Count - 1];
				}
			}
			Debug.LogError("Couldnt get SimHash [" + item + "]");
			return null;
		}
	}
}
