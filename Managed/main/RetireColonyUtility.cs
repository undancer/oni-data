using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;

public static class RetireColonyUtility
{
	private const int FILE_IO_RETRY_ATTEMPTS = 5;

	private static char[] invalidCharacters = "<>:\"\\/|?*.".ToCharArray();

	private static Encoding[] attempt_encodings = new Encoding[3]
	{
		new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true),
		new UnicodeEncoding(bigEndian: false, byteOrderMark: true, throwOnInvalidBytes: true),
		Encoding.ASCII
	};

	public static bool SaveColonySummaryData()
	{
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		string text = Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName());
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string text2 = StripInvalidCharacters(SaveGame.Instance.BaseName);
		string text3 = Path.Combine(text, text2);
		if (!Directory.Exists(text3))
		{
			Directory.CreateDirectory(text3);
		}
		string path = Path.Combine(text3, text2 + ".json");
		RetiredColonyData currentColonyRetiredColonyData = GetCurrentColonyRetiredColonyData();
		string s = JsonConvert.SerializeObject(currentColonyRetiredColonyData);
		bool flag = false;
		int num = 0;
		while (!flag && num < 5)
		{
			try
			{
				Thread.Sleep(num * 100);
				using FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
				flag = true;
				Encoding uTF = Encoding.UTF8;
				byte[] bytes = uTF.GetBytes(s);
				fileStream.Write(bytes, 0, bytes.Length);
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("SaveColonySummaryData failed attempt {0}: {1}", num + 1, ex.ToString());
			}
			num++;
		}
		return flag;
	}

	public static RetiredColonyData GetCurrentColonyRetiredColonyData()
	{
		MinionAssignablesProxy[] array = new MinionAssignablesProxy[Components.MinionAssignablesProxy.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Components.MinionAssignablesProxy[i];
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, ColonyAchievementStatus> achievement in SaveGame.Instance.GetComponent<ColonyAchievementTracker>().achievements)
		{
			if (achievement.Value.success)
			{
				list.Add(achievement.Key);
			}
		}
		BuildingComplete[] array2 = new BuildingComplete[Components.BuildingCompletes.Count];
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j] = Components.BuildingCompletes[j];
		}
		return new RetiredColonyData(SaveGame.Instance.BaseName, GameClock.Instance.GetCycle(), System.DateTime.Now.ToShortDateString(), list.ToArray(), array, array2);
	}

	private static RetiredColonyData LoadRetiredColony(string file, bool skipStats, Encoding enc)
	{
		RetiredColonyData retiredColonyData = new RetiredColonyData();
		using (FileStream stream = File.Open(file, FileMode.Open))
		{
			using StreamReader reader = new StreamReader(stream, enc);
			using JsonReader jsonReader = new JsonTextReader(reader);
			string a = string.Empty;
			List<string> list = new List<string>();
			List<Tuple<string, int>> list2 = new List<Tuple<string, int>>();
			List<RetiredColonyData.RetiredDuplicantData> list3 = new List<RetiredColonyData.RetiredDuplicantData>();
			List<RetiredColonyData.RetiredColonyStatistic> list4 = new List<RetiredColonyData.RetiredColonyStatistic>();
			while (jsonReader.Read())
			{
				JsonToken tokenType = jsonReader.TokenType;
				if (tokenType == JsonToken.PropertyName)
				{
					a = jsonReader.Value.ToString();
				}
				if (tokenType == JsonToken.String && a == "colonyName")
				{
					retiredColonyData.colonyName = jsonReader.Value.ToString();
				}
				if (tokenType == JsonToken.String && a == "date")
				{
					retiredColonyData.date = jsonReader.Value.ToString();
				}
				if (tokenType == JsonToken.Integer && a == "cycleCount")
				{
					retiredColonyData.cycleCount = int.Parse(jsonReader.Value.ToString());
				}
				if (tokenType == JsonToken.String && a == "achievements")
				{
					list.Add(jsonReader.Value.ToString());
				}
				if (tokenType == JsonToken.StartObject && a == "Duplicants")
				{
					string a2 = null;
					RetiredColonyData.RetiredDuplicantData retiredDuplicantData = new RetiredColonyData.RetiredDuplicantData();
					retiredDuplicantData.accessories = new Dictionary<string, string>();
					while (jsonReader.Read())
					{
						tokenType = jsonReader.TokenType;
						if (tokenType == JsonToken.EndObject)
						{
							break;
						}
						if (tokenType == JsonToken.PropertyName)
						{
							a2 = jsonReader.Value.ToString();
						}
						if (a2 == "name" && tokenType == JsonToken.String)
						{
							retiredDuplicantData.name = jsonReader.Value.ToString();
						}
						if (a2 == "age" && tokenType == JsonToken.Integer)
						{
							retiredDuplicantData.age = int.Parse(jsonReader.Value.ToString());
						}
						if (a2 == "skillPointsGained" && tokenType == JsonToken.Integer)
						{
							retiredDuplicantData.skillPointsGained = int.Parse(jsonReader.Value.ToString());
						}
						if (!(a2 == "accessories"))
						{
							continue;
						}
						string text = null;
						while (jsonReader.Read())
						{
							tokenType = jsonReader.TokenType;
							if (tokenType == JsonToken.EndObject)
							{
								break;
							}
							if (tokenType == JsonToken.PropertyName)
							{
								text = jsonReader.Value.ToString();
							}
							if (text != null && jsonReader.Value != null && tokenType == JsonToken.String)
							{
								string value = jsonReader.Value.ToString();
								retiredDuplicantData.accessories.Add(text, value);
							}
						}
					}
					list3.Add(retiredDuplicantData);
				}
				if (tokenType == JsonToken.StartObject && a == "buildings")
				{
					string a3 = null;
					string a4 = null;
					int b = 0;
					while (jsonReader.Read())
					{
						tokenType = jsonReader.TokenType;
						if (tokenType == JsonToken.EndObject)
						{
							break;
						}
						if (tokenType == JsonToken.PropertyName)
						{
							a3 = jsonReader.Value.ToString();
						}
						if (a3 == "first" && tokenType == JsonToken.String)
						{
							a4 = jsonReader.Value.ToString();
						}
						if (a3 == "second" && tokenType == JsonToken.Integer)
						{
							b = int.Parse(jsonReader.Value.ToString());
						}
					}
					Tuple<string, int> item = new Tuple<string, int>(a4, b);
					list2.Add(item);
				}
				if (tokenType != JsonToken.StartObject || !(a == "Stats"))
				{
					continue;
				}
				if (skipStats)
				{
					break;
				}
				string a5 = null;
				RetiredColonyData.RetiredColonyStatistic retiredColonyStatistic = new RetiredColonyData.RetiredColonyStatistic();
				List<Tuple<float, float>> list5 = new List<Tuple<float, float>>();
				while (jsonReader.Read())
				{
					tokenType = jsonReader.TokenType;
					if (tokenType == JsonToken.EndObject)
					{
						break;
					}
					if (tokenType == JsonToken.PropertyName)
					{
						a5 = jsonReader.Value.ToString();
					}
					if (a5 == "id" && tokenType == JsonToken.String)
					{
						retiredColonyStatistic.id = jsonReader.Value.ToString();
					}
					if (a5 == "name" && tokenType == JsonToken.String)
					{
						retiredColonyStatistic.name = jsonReader.Value.ToString();
					}
					if (a5 == "nameX" && tokenType == JsonToken.String)
					{
						retiredColonyStatistic.nameX = jsonReader.Value.ToString();
					}
					if (a5 == "nameY" && tokenType == JsonToken.String)
					{
						retiredColonyStatistic.nameY = jsonReader.Value.ToString();
					}
					if (!(a5 == "value") || tokenType != JsonToken.StartObject)
					{
						continue;
					}
					string a6 = null;
					float a7 = 0f;
					float b2 = 0f;
					while (jsonReader.Read())
					{
						tokenType = jsonReader.TokenType;
						if (tokenType == JsonToken.EndObject)
						{
							break;
						}
						if (tokenType == JsonToken.PropertyName)
						{
							a6 = jsonReader.Value.ToString();
						}
						if (a6 == "first" && (tokenType == JsonToken.Float || tokenType == JsonToken.Integer))
						{
							a7 = float.Parse(jsonReader.Value.ToString());
						}
						if (a6 == "second" && (tokenType == JsonToken.Float || tokenType == JsonToken.Integer))
						{
							b2 = float.Parse(jsonReader.Value.ToString());
						}
					}
					Tuple<float, float> item2 = new Tuple<float, float>(a7, b2);
					list5.Add(item2);
				}
				retiredColonyStatistic.value = list5.ToArray();
				list4.Add(retiredColonyStatistic);
			}
			retiredColonyData.Duplicants = list3.ToArray();
			retiredColonyData.Stats = list4.ToArray();
			retiredColonyData.achievements = list.ToArray();
			retiredColonyData.buildings = list2;
		}
		return retiredColonyData;
	}

	public static RetiredColonyData[] LoadRetiredColonies(bool skipStats = false)
	{
		List<RetiredColonyData> list = new List<RetiredColonyData>();
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		string path = Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName());
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		path = Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName());
		string[] directories = Directory.GetDirectories(path);
		foreach (string path2 in directories)
		{
			string[] files = Directory.GetFiles(path2);
			foreach (string text in files)
			{
				if (!text.EndsWith(".json"))
				{
					continue;
				}
				for (int k = 0; k < attempt_encodings.Length; k++)
				{
					Encoding encoding = attempt_encodings[k];
					try
					{
						RetiredColonyData retiredColonyData = LoadRetiredColony(text, skipStats, encoding);
						if (retiredColonyData != null)
						{
							if (retiredColonyData.colonyName == null)
							{
								throw new Exception("data.colonyName was null");
							}
							list.Add(retiredColonyData);
						}
					}
					catch (Exception ex)
					{
						Debug.LogWarningFormat("LoadRetiredColonies failed load {0} [{1}]: {2}", encoding, text, ex.ToString());
						continue;
					}
					break;
				}
			}
		}
		return list.ToArray();
	}

	public static string[] LoadColonySlideshowFiles(string colonyName)
	{
		string path = StripInvalidCharacters(colonyName);
		string text = Path.Combine(Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName()), path);
		List<string> list = new List<string>();
		if (Directory.Exists(text))
		{
			string[] files = Directory.GetFiles(text);
			foreach (string text2 in files)
			{
				if (text2.EndsWith(".png"))
				{
					list.Add(text2);
				}
			}
		}
		else
		{
			Debug.LogWarningFormat("LoadColonySlideshow path does not exist or is not directory [{0}]", text);
		}
		return list.ToArray();
	}

	public static Sprite[] LoadColonySlideshow(string colonyName)
	{
		string path = StripInvalidCharacters(colonyName);
		string text = Path.Combine(Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName()), path);
		List<Sprite> list = new List<Sprite>();
		if (Directory.Exists(text))
		{
			string[] files = Directory.GetFiles(text);
			foreach (string text2 in files)
			{
				if (text2.EndsWith(".png"))
				{
					Texture2D texture2D = new Texture2D(512, 768);
					texture2D.filterMode = FilterMode.Point;
					texture2D.LoadImage(File.ReadAllBytes(text2));
					list.Add(Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2(texture2D.width, texture2D.height)), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect));
				}
			}
		}
		else
		{
			Debug.LogWarningFormat("LoadColonySlideshow path does not exist or is not directory [{0}]", text);
		}
		return list.ToArray();
	}

	public static Sprite LoadRetiredColonyPreview(string colonyName)
	{
		try
		{
			string path = StripInvalidCharacters(colonyName);
			string text = Path.Combine(Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName()), path);
			List<string> list = new List<string>();
			if (Directory.Exists(text))
			{
				string[] files = Directory.GetFiles(text);
				foreach (string text2 in files)
				{
					if (text2.EndsWith(".png"))
					{
						list.Add(text2);
					}
				}
			}
			else
			{
				Debug.LogWarningFormat("LoadColonyPreview path does not exist or is not directory [{0}]", text);
			}
			if (list.Count > 0)
			{
				Texture2D texture2D = new Texture2D(512, 768);
				string path2 = list[list.Count - 1];
				if (!texture2D.LoadImage(File.ReadAllBytes(path2)))
				{
					return null;
				}
				if (texture2D.width > SystemInfo.maxTextureSize || texture2D.height > SystemInfo.maxTextureSize)
				{
					return null;
				}
				if (texture2D.width == 0 || texture2D.height == 0)
				{
					return null;
				}
				return Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2(texture2D.width, texture2D.height)), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect);
			}
		}
		catch (Exception ex)
		{
			Debug.Log("Loading timelapse preview failed! reason: " + ex.Message);
		}
		return null;
	}

	public static Sprite LoadColonyPreview(string savePath, string colonyName, bool fallbackToTimelapse = false)
	{
		string path = Path.ChangeExtension(savePath, ".png");
		if (File.Exists(path))
		{
			try
			{
				Texture2D texture2D = new Texture2D(512, 768);
				if (!texture2D.LoadImage(File.ReadAllBytes(path)))
				{
					return null;
				}
				if (texture2D.width > SystemInfo.maxTextureSize || texture2D.height > SystemInfo.maxTextureSize)
				{
					return null;
				}
				if (texture2D.width == 0 || texture2D.height == 0)
				{
					return null;
				}
				return Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2(texture2D.width, texture2D.height)), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect);
			}
			catch (Exception arg)
			{
				Debug.Log("failed to load preview image!? " + arg);
			}
		}
		if (!fallbackToTimelapse)
		{
			return null;
		}
		try
		{
			return LoadRetiredColonyPreview(colonyName);
		}
		catch (Exception arg2)
		{
			Debug.Log($"failed to load fallback timelapse image!? {arg2}");
		}
		return null;
	}

	public static string StripInvalidCharacters(string source)
	{
		char[] array = invalidCharacters;
		foreach (char oldChar in array)
		{
			source = source.Replace(oldChar, '_');
		}
		source = source.Trim();
		return source;
	}
}
