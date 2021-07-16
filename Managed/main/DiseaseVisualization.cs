using System;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseVisualization : ScriptableObject
{
	[Serializable]
	public struct Info
	{
		public string name;

		public string overlayColourName;

		public Info(string name)
		{
			this.name = name;
			overlayColourName = "germFoodPoisoning";
		}
	}

	public Sprite overlaySprite;

	public List<Info> info = new List<Info>();

	public Info GetInfo(HashedString id)
	{
		foreach (Info item in info)
		{
			if (id == item.name)
			{
				return item;
			}
		}
		return default(Info);
	}
}
