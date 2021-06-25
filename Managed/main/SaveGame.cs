using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using KSerialization;
using Newtonsoft.Json;
using ProcGen;
using ProcGenGame;
using STRINGS;
using UnityEngine;

[SerializationConfig(KSerialization.MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/SaveGame")]
public class SaveGame : KMonoBehaviour, ISaveLoadable
{
	public struct Header
	{
		public uint buildVersion;

		public int headerSize;

		public uint headerVersion;

		public int compression;

		public bool IsCompressed => compression != 0;
	}

	public struct GameInfo
	{
		public int numberOfCycles;

		public int numberOfDuplicants;

		public string baseName;

		public bool isAutoSave;

		public string originalSaveName;

		public int saveMajorVersion;

		public int saveMinorVersion;

		public string worldID;

		public string[] worldTraits;

		public bool sandboxEnabled;

		public Guid colonyGuid;

		public GameInfo(int numberOfCycles, int numberOfDuplicants, string baseName, bool isAutoSave, string originalSaveName, string worldID, string[] worldTraits, Guid colonyGuid, bool sandboxEnabled = false)
		{
			this.numberOfCycles = numberOfCycles;
			this.numberOfDuplicants = numberOfDuplicants;
			this.baseName = baseName;
			this.isAutoSave = isAutoSave;
			this.originalSaveName = originalSaveName;
			this.worldID = worldID;
			this.worldTraits = worldTraits;
			this.colonyGuid = colonyGuid;
			this.sandboxEnabled = sandboxEnabled;
			saveMajorVersion = 7;
			saveMinorVersion = 17;
		}

		public bool IsVersionOlderThan(int major, int minor)
		{
			if (saveMajorVersion >= major)
			{
				if (saveMajorVersion == major)
				{
					return saveMinorVersion < minor;
				}
				return false;
			}
			return true;
		}

		public bool IsVersionExactly(int major, int minor)
		{
			if (saveMajorVersion == major)
			{
				return saveMinorVersion == minor;
			}
			return false;
		}
	}

	[Serialize]
	private int speed;

	[Serialize]
	public List<Tag> expandedResourceTags = new List<Tag>();

	[Serialize]
	public int minGermCountForDisinfect = 10000;

	[Serialize]
	public bool enableAutoDisinfect = true;

	[Serialize]
	public bool sandboxEnabled;

	[Serialize]
	private int autoSaveCycleInterval = 1;

	[Serialize]
	private Vector2I timelapseResolution = new Vector2I(512, 768);

	private string baseName;

	public static SaveGame Instance;

	public EntombedItemManager entombedItemManager;

	public WorldGenSpawner worldGenSpawner;

	[MyCmpReq]
	public MaterialSelectorSerializer materialSelectorSerializer;

	public WorldGen worldGen;

	public int AutoSaveCycleInterval
	{
		get
		{
			return autoSaveCycleInterval;
		}
		set
		{
			autoSaveCycleInterval = value;
		}
	}

	public Vector2I TimelapseResolution
	{
		get
		{
			return timelapseResolution;
		}
		set
		{
			timelapseResolution = value;
		}
	}

	public string BaseName => baseName;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		new ColonyRationMonitor.Instance(this).StartSM();
		new VignetteManager.Instance(this).StartSM();
		entombedItemManager = base.gameObject.AddComponent<EntombedItemManager>();
		worldGen = SaveLoader.Instance.worldGen;
		worldGenSpawner = base.gameObject.AddComponent<WorldGenSpawner>();
	}

	[OnSerializing]
	private void OnSerialize()
	{
		speed = SpeedControlScreen.Instance.GetSpeed();
	}

	[OnDeserializing]
	private void OnDeserialize()
	{
		baseName = SaveLoader.Instance.GameInfo.baseName;
	}

	public int GetSpeed()
	{
		return speed;
	}

	public byte[] GetSaveHeader(bool isAutoSave, bool isCompressed, out Header header)
	{
		string text = null;
		string originalSaveFileName = SaveLoader.GetOriginalSaveFileName(SaveLoader.GetActiveSaveFilePath());
		text = JsonConvert.SerializeObject(new GameInfo(GameClock.Instance.GetCycle(), Components.LiveMinionIdentities.Count, baseName, isAutoSave, originalSaveFileName, SaveLoader.Instance.GameInfo.worldID, SaveLoader.Instance.GameInfo.worldTraits, SaveLoader.Instance.GameInfo.colonyGuid, sandboxEnabled));
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		header = default(Header);
		header.buildVersion = 469300u;
		header.headerSize = bytes.Length;
		header.headerVersion = 1u;
		header.compression = (isCompressed ? 1 : 0);
		return bytes;
	}

	public static string GetSaveUniqueID(GameInfo info)
	{
		if (!(info.colonyGuid != Guid.Empty))
		{
			return info.baseName + "/" + info.worldID;
		}
		return info.colonyGuid.ToString();
	}

	public static Tuple<Header, GameInfo> GetFileInfo(string filename)
	{
		try
		{
			Header header;
			GameInfo b = SaveLoader.LoadHeader(filename, out header);
			if (b.saveMajorVersion >= 7)
			{
				return new Tuple<Header, GameInfo>(header, b);
			}
		}
		catch (Exception obj)
		{
			Debug.LogWarning(obj);
		}
		return null;
	}

	public static GameInfo GetHeader(IReader br, out Header header)
	{
		header = default(Header);
		header.buildVersion = br.ReadUInt32();
		header.headerSize = br.ReadInt32();
		header.headerVersion = br.ReadUInt32();
		if (1 <= header.headerVersion)
		{
			header.compression = br.ReadInt32();
		}
		GameInfo gameInfo = GetGameInfo(br.ReadBytes(header.headerSize));
		if (gameInfo.IsVersionOlderThan(7, 14) && gameInfo.worldTraits != null)
		{
			string[] worldTraits = gameInfo.worldTraits;
			for (int i = 0; i < worldTraits.Length; i++)
			{
				worldTraits[i] = worldTraits[i].Replace('\\', '/');
			}
		}
		return gameInfo;
	}

	public static GameInfo GetGameInfo(byte[] data)
	{
		return JsonConvert.DeserializeObject<GameInfo>(Encoding.UTF8.GetString(data));
	}

	public void SetBaseName(string newBaseName)
	{
		if (string.IsNullOrEmpty(newBaseName))
		{
			Debug.LogWarning("Cannot give the base an empty name");
		}
		else
		{
			baseName = newBaseName;
		}
	}

	protected override void OnSpawn()
	{
		ThreadedHttps<KleiMetrics>.Instance.SendProfileStats();
		Game.Instance.Trigger(-1917495436);
	}

	public List<Tuple<string, ScriptableObject>> GetColonyToolTip()
	{
		List<Tuple<string, ScriptableObject>> list = new List<Tuple<string, ScriptableObject>>();
		list.Add(new Tuple<string, ScriptableObject>(baseName, ToolTipScreen.Instance.defaultTooltipHeaderStyle));
		if (GameClock.Instance != null)
		{
			list.Add(new Tuple<string, ScriptableObject>(" ", null));
			list.Add(new Tuple<string, ScriptableObject>(string.Format(UI.ASTEROIDCLOCK.CYCLES_OLD, GameUtil.GetCurrentCycle()), ToolTipScreen.Instance.defaultTooltipHeaderStyle));
			list.Add(new Tuple<string, ScriptableObject>(string.Format(UI.ASTEROIDCLOCK.TIME_PLAYED, (GameClock.Instance.GetTimePlayedInSeconds() / 3600f).ToString("0.00")), ToolTipScreen.Instance.defaultTooltipBodyStyle));
		}
		ProcGen.World worldData = SettingsCache.worlds.GetWorldData(SaveLoader.Instance.GameInfo.worldID);
		list.Add(new Tuple<string, ScriptableObject>(" ", null));
		list.Add(new Tuple<string, ScriptableObject>(Strings.Get(worldData.name), ToolTipScreen.Instance.defaultTooltipHeaderStyle));
		if (SaveLoader.Instance.GameInfo.worldTraits != null)
		{
			string[] worldTraits = SaveLoader.Instance.GameInfo.worldTraits;
			for (int i = 0; i < worldTraits.Length; i++)
			{
				WorldTrait cachedTrait = SettingsCache.GetCachedTrait(worldTraits[i], assertMissingTrait: false);
				if (cachedTrait != null)
				{
					list.Add(new Tuple<string, ScriptableObject>(Strings.Get(cachedTrait.name), ToolTipScreen.Instance.defaultTooltipBodyStyle));
				}
				else
				{
					list.Add(new Tuple<string, ScriptableObject>(WORLD_TRAITS.MISSING_TRAIT, ToolTipScreen.Instance.defaultTooltipBodyStyle));
				}
			}
		}
		return list;
	}
}
