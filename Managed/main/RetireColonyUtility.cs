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
		string s = JsonConvert.SerializeObject(GetCurrentColonyRetiredColonyData());
		if (DlcManager.IsExpansion1Active())
		{
			foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
			{
				if (!worldContainer.IsDiscovered || worldContainer.IsModuleInterior)
				{
					continue;
				}
				string name = worldContainer.GetComponent<ClusterGridEntity>().Name;
				string text4 = Path.Combine(text3, name);
				string text5 = Path.Combine(text3, worldContainer.id.ToString("D5"));
				if (!Directory.Exists(text4))
				{
					continue;
				}
				bool flag = Directory.GetFiles(text4).Length != 0;
				if (!Directory.Exists(text5))
				{
					Directory.CreateDirectory(text5);
				}
				string[] files = Directory.GetFiles(text4);
				foreach (string text6 in files)
				{
					try
					{
						File.Copy(text6, text6.Replace(text4, text5), overwrite: true);
						File.Delete(text6);
					}
					catch (Exception obj)
					{
						flag = false;
						Debug.LogWarning("Error occurred trying to migrate screenshot: " + text6);
						Debug.LogWarning(obj);
					}
				}
				if (flag)
				{
					Directory.Delete(text4);
				}
			}
		}
		bool flag2 = false;
		int num = 0;
		while (!flag2 && num < 5)
		{
			try
			{
				Thread.Sleep(num * 100);
				using FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
				flag2 = true;
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				fileStream.Write(bytes, 0, bytes.Length);
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("SaveColonySummaryData failed attempt {0}: {1}", num + 1, ex.ToString());
			}
			num++;
		}
		return flag2;
	}

	public static RetiredColonyData GetCurrentColonyRetiredColonyData()
	{
		List<MinionAssignablesProxy> list = new List<MinionAssignablesProxy>();
		for (int i = 0; i < Components.MinionAssignablesProxy.Count; i++)
		{
			if (Components.MinionAssignablesProxy[i] != null)
			{
				list.Add(Components.MinionAssignablesProxy[i]);
			}
		}
		List<string> list2 = new List<string>();
		foreach (KeyValuePair<string, ColonyAchievementStatus> achievement in SaveGame.Instance.GetComponent<ColonyAchievementTracker>().achievements)
		{
			if (achievement.Value.success)
			{
				list2.Add(achievement.Key);
			}
		}
		BuildingComplete[] array = new BuildingComplete[Components.BuildingCompletes.Count];
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = Components.BuildingCompletes[j];
		}
		string startWorld = null;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			if (worldContainer.IsDiscovered && !worldContainer.IsModuleInterior)
			{
				dictionary.Add(worldContainer.id.ToString("D5"), worldContainer.worldName);
				if (worldContainer.IsStartWorld)
				{
					startWorld = worldContainer.id.ToString("D5");
				}
			}
		}
		return new RetiredColonyData(SaveGame.Instance.BaseName, GameClock.Instance.GetCycle(), System.DateTime.Now.ToShortDateString(), list2.ToArray(), list.ToArray(), array, startWorld, dictionary);
	}

	private static RetiredColonyData LoadRetiredColony(string file, bool skipStats, Encoding enc)
	{
		RetiredColonyData retiredColonyData = new RetiredColonyData();
		using FileStream stream = File.Open(file, FileMode.Open);
		using StreamReader reader = new StreamReader(stream, enc);
		using JsonReader jsonReader = new JsonTextReader(reader);
		string text = string.Empty;
		List<string> list = new List<string>();
		List<Tuple<string, int>> list2 = new List<Tuple<string, int>>();
		List<RetiredColonyData.RetiredDuplicantData> list3 = new List<RetiredColonyData.RetiredDuplicantData>();
		List<RetiredColonyData.RetiredColonyStatistic> list4 = new List<RetiredColonyData.RetiredColonyStatistic>();
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		while (jsonReader.Read())
		{
			JsonToken tokenType = jsonReader.TokenType;
			if (tokenType == JsonToken.PropertyName)
			{
				text = jsonReader.Value.ToString();
			}
			if (tokenType == JsonToken.String && text == "colonyName")
			{
				retiredColonyData.colonyName = jsonReader.Value.ToString();
			}
			if (tokenType == JsonToken.String && text == "date")
			{
				retiredColonyData.date = jsonReader.Value.ToString();
			}
			if (tokenType == JsonToken.Integer && text == "cycleCount")
			{
				retiredColonyData.cycleCount = int.Parse(jsonReader.Value.ToString());
			}
			if (tokenType == JsonToken.String && text == "achievements")
			{
				list.Add(jsonReader.Value.ToString());
			}
			if (tokenType == JsonToken.StartObject && text == "Duplicants")
			{
				string text2 = null;
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
						text2 = jsonReader.Value.ToString();
					}
					if (text2 == "name" && tokenType == JsonToken.String)
					{
						retiredDuplicantData.name = jsonReader.Value.ToString();
					}
					if (text2 == "age" && tokenType == JsonToken.Integer)
					{
						retiredDuplicantData.age = int.Parse(jsonReader.Value.ToString());
					}
					if (text2 == "skillPointsGained" && tokenType == JsonToken.Integer)
					{
						retiredDuplicantData.skillPointsGained = int.Parse(jsonReader.Value.ToString());
					}
					if (!(text2 == "accessories"))
					{
						continue;
					}
					string text3 = null;
					while (jsonReader.Read())
					{
						tokenType = jsonReader.TokenType;
						if (tokenType == JsonToken.EndObject)
						{
							break;
						}
						if (tokenType == JsonToken.PropertyName)
						{
							text3 = jsonReader.Value.ToString();
						}
						if (text3 != null && jsonReader.Value != null && tokenType == JsonToken.String)
						{
							string value = jsonReader.Value.ToString();
							retiredDuplicantData.accessories.Add(text3, value);
						}
					}
				}
				list3.Add(retiredDuplicantData);
			}
			if (tokenType == JsonToken.StartObject && text == "buildings")
			{
				string text4 = null;
				string a = null;
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
						text4 = jsonReader.Value.ToString();
					}
					if (text4 == "first" && tokenType == JsonToken.String)
					{
						a = jsonReader.Value.ToString();
					}
					if (text4 == "second" && tokenType == JsonToken.Integer)
					{
						b = int.Parse(jsonReader.Value.ToString());
					}
				}
				Tuple<string, int> item = new Tuple<string, int>(a, b);
				list2.Add(item);
			}
			if (tokenType == JsonToken.StartObject && text == "Stats")
			{
				if (skipStats)
				{
					break;
				}
				string text5 = null;
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
						text5 = jsonReader.Value.ToString();
					}
					if (text5 == "id" && tokenType == JsonToken.String)
					{
						retiredColonyStatistic.id = jsonReader.Value.ToString();
					}
					if (text5 == "name" && tokenType == JsonToken.String)
					{
						retiredColonyStatistic.name = jsonReader.Value.ToString();
					}
					if (text5 == "nameX" && tokenType == JsonToken.String)
					{
						retiredColonyStatistic.nameX = jsonReader.Value.ToString();
					}
					if (text5 == "nameY" && tokenType == JsonToken.String)
					{
						retiredColonyStatistic.nameY = jsonReader.Value.ToString();
					}
					if (!(text5 == "value") || tokenType != JsonToken.StartObject)
					{
						continue;
					}
					string text6 = null;
					float a2 = 0f;
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
							text6 = jsonReader.Value.ToString();
						}
						if (text6 == "first" && (tokenType == JsonToken.Float || tokenType == JsonToken.Integer))
						{
							a2 = float.Parse(jsonReader.Value.ToString());
						}
						if (text6 == "second" && (tokenType == JsonToken.Float || tokenType == JsonToken.Integer))
						{
							b2 = float.Parse(jsonReader.Value.ToString());
						}
					}
					Tuple<float, float> item2 = new Tuple<float, float>(a2, b2);
					list5.Add(item2);
				}
				retiredColonyStatistic.value = list5.ToArray();
				list4.Add(retiredColonyStatistic);
			}
			if (tokenType == JsonToken.StartObject && text == "worldIdentities")
			{
				string text7 = null;
				while (jsonReader.Read())
				{
					tokenType = jsonReader.TokenType;
					if (tokenType == JsonToken.EndObject)
					{
						break;
					}
					if (tokenType == JsonToken.PropertyName)
					{
						text7 = jsonReader.Value.ToString();
					}
					if (text7 != null && jsonReader.Value != null && tokenType == JsonToken.String)
					{
						string value2 = jsonReader.Value.ToString();
						dictionary.Add(text7, value2);
					}
				}
			}
			if (tokenType == JsonToken.String && text == "startWorld")
			{
				retiredColonyData.startWorld = jsonReader.Value.ToString();
			}
		}
		retiredColonyData.Duplicants = list3.ToArray();
		retiredColonyData.Stats = list4.ToArray();
		retiredColonyData.achievements = list.ToArray();
		retiredColonyData.buildings = list2;
		retiredColonyData.worldIdentities = dictionary;
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
		for (int i = 0; i < directories.Length; i++)
		{
			string[] files = Directory.GetFiles(directories[i]);
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

	public static string[] LoadColonySlideshowFiles(string colonyName, string world_name)
	{
		string path = StripInvalidCharacters(colonyName);
		string text = Path.Combine(Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName()), path);
		if (!world_name.IsNullOrWhiteSpace())
		{
			text = Path.Combine(text, world_name);
		}
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

	public static Sprite LoadRetiredColonyPreview(string colonyName, string startName = null)
	{
		try
		{
			string path = StripInvalidCharacters(colonyName);
			string text = Path.Combine(Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName()), path);
			if (!startName.IsNullOrWhiteSpace())
			{
				text = Path.Combine(text, startName);
			}
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
			catch (Exception ex)
			{
				Debug.Log("failed to load preview image!? " + ex);
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
		catch (Exception arg)
		{
			Debug.Log($"failed to load fallback timelapse image!? {arg}");
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
