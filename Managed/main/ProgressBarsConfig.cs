using System;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarsConfig : ScriptableObject
{
	[Serializable]
	public struct BarData
	{
		public string barName;

		public Color barColor;

		public string barDescriptionKey;
	}

	public GameObject progressBarPrefab;

	public GameObject progressBarUIPrefab;

	public GameObject healthBarPrefab;

	public List<BarData> barColorDataList = new List<BarData>();

	public Dictionary<string, BarData> barColorMap = new Dictionary<string, BarData>();

	private static ProgressBarsConfig instance;

	public static ProgressBarsConfig Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load<ProgressBarsConfig>("ProgressBarsConfig");
				instance.Initialize();
			}
			return instance;
		}
	}

	public static void DestroyInstance()
	{
		instance = null;
	}

	public void Initialize()
	{
		foreach (BarData barColorData in barColorDataList)
		{
			barColorMap.Add(barColorData.barName, barColorData);
		}
	}

	public string GetBarDescription(string barName)
	{
		string result = "";
		if (IsBarNameValid(barName))
		{
			result = Strings.Get(barColorMap[barName].barDescriptionKey);
		}
		return result;
	}

	public Color GetBarColor(string barName)
	{
		Color result = Color.clear;
		if (IsBarNameValid(barName))
		{
			result = barColorMap[barName].barColor;
		}
		return result;
	}

	public bool IsBarNameValid(string barName)
	{
		if (string.IsNullOrEmpty(barName))
		{
			Debug.LogError("The barName provided was null or empty. Don't do that.");
			return false;
		}
		if (!barColorMap.ContainsKey(barName))
		{
			Debug.LogError($"No BarData found for the entry [ {barName} ]");
			return false;
		}
		return true;
	}
}
