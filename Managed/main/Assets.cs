using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KMod;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[AddComponentMenu("KMonoBehaviour/scripts/Assets")]
public class Assets : KMonoBehaviour, ISerializationCallbackReceiver
{
	[Serializable]
	public struct UIPrefabData
	{
		public ProgressBar ProgressBar;

		public HealthBar HealthBar;

		public GameObject ResourceVisualizer;

		public Image RegionCellBlocked;

		public RectTransform PriorityOverlayIcon;

		public RectTransform HarvestWhenReadyOverlayIcon;

		public TableScreenAssets TableScreenWidgets;
	}

	[Serializable]
	public struct TableScreenAssets
	{
		public Material DefaultUIMaterial;

		public Material DesaturatedUIMaterial;

		public GameObject MinionPortrait;

		public GameObject GenericPortrait;

		public GameObject TogglePortrait;

		public GameObject ButtonLabel;

		public GameObject ButtonLabelWhite;

		public GameObject Label;

		public GameObject LabelHeader;

		public GameObject Checkbox;

		public GameObject BlankCell;

		public GameObject SuperCheckbox_Horizontal;

		public GameObject SuperCheckbox_Vertical;

		public GameObject Spacer;

		public GameObject NumericDropDown;

		public GameObject DropDownHeader;

		public GameObject PriorityGroupSelector;

		public GameObject PriorityGroupSelectorHeader;

		public GameObject PrioritizeRowWidget;

		public GameObject PrioritizeRowHeaderWidget;
	}

	public static List<KAnimFile> ModLoadedKAnims = new List<KAnimFile>();

	private static Action<KPrefabID> OnAddPrefab;

	public static List<BuildingDef> BuildingDefs;

	public List<KPrefabID> PrefabAssets = new List<KPrefabID>();

	public static List<KPrefabID> Prefabs = new List<KPrefabID>();

	private static HashSet<Tag> CountableTags = new HashSet<Tag>();

	public List<Sprite> SpriteAssets;

	public static Dictionary<HashedString, Sprite> Sprites;

	public List<string> videoClipNames;

	private const string VIDEO_ASSET_PATH = "video_webm";

	public List<TintedSprite> TintedSpriteAssets;

	public static List<TintedSprite> TintedSprites;

	public List<Texture2D> TextureAssets;

	public static List<Texture2D> Textures;

	public static List<TextureAtlas> TextureAtlases;

	public List<TextureAtlas> TextureAtlasAssets;

	public static List<Material> Materials;

	public List<Material> MaterialAssets;

	public static List<Shader> Shaders;

	public List<Shader> ShaderAssets;

	public static List<BlockTileDecorInfo> BlockTileDecorInfos;

	public List<BlockTileDecorInfo> BlockTileDecorInfoAssets;

	public Material AnimMaterialAsset;

	public static Material AnimMaterial;

	public DiseaseVisualization DiseaseVisualization;

	public Sprite LegendColourBox;

	public Texture2D invalidAreaTex;

	public UIPrefabData UIPrefabAssets;

	public static UIPrefabData UIPrefabs;

	private static Dictionary<Tag, KPrefabID> PrefabsByTag = new Dictionary<Tag, KPrefabID>();

	private static Dictionary<Tag, List<KPrefabID>> PrefabsByAdditionalTags = new Dictionary<Tag, List<KPrefabID>>();

	public List<KAnimFile> AnimAssets;

	public static List<KAnimFile> Anims;

	private static Dictionary<HashedString, KAnimFile> AnimTable = new Dictionary<HashedString, KAnimFile>();

	public Font DebugFontAsset;

	public static Font DebugFont;

	public SubstanceTable substanceTable;

	[SerializeField]
	public TextAsset elementAudio;

	[SerializeField]
	public TextAsset personalitiesFile;

	public LogicModeUI logicModeUIData;

	public CommonPlacerConfig.CommonPlacerAssets commonPlacerAssets;

	public DigPlacerConfig.DigPlacerAssets digPlacerAssets;

	public MopPlacerConfig.MopPlacerAssets mopPlacerAssets;

	public ComicData[] comics;

	public static Assets instance;

	private static Dictionary<string, string> simpleSoundEventNames = new Dictionary<string, string>();

	protected override void OnPrefabInit()
	{
		instance = this;
		if (KPlayerPrefs.HasKey("TemperatureUnit"))
		{
			GameUtil.temperatureUnit = (GameUtil.TemperatureUnit)KPlayerPrefs.GetInt("TemperatureUnit");
		}
		if (KPlayerPrefs.HasKey("MassUnit"))
		{
			GameUtil.massUnit = (GameUtil.MassUnit)KPlayerPrefs.GetInt("MassUnit");
		}
		RecipeManager.DestroyInstance();
		RecipeManager.Get();
		AnimMaterial = AnimMaterialAsset;
		Prefabs = new List<KPrefabID>(PrefabAssets.Where((KPrefabID x) => x != null));
		PrefabsByTag.Clear();
		PrefabsByAdditionalTags.Clear();
		CountableTags.Clear();
		Sprites = new Dictionary<HashedString, Sprite>();
		foreach (Sprite spriteAsset in SpriteAssets)
		{
			if (!(spriteAsset == null))
			{
				HashedString key = new HashedString(spriteAsset.name);
				Sprites.Add(key, spriteAsset);
			}
		}
		TintedSprites = TintedSpriteAssets.Where((TintedSprite x) => x != null && x.sprite != null).ToList();
		Materials = MaterialAssets.Where((Material x) => x != null).ToList();
		Textures = TextureAssets.Where((Texture2D x) => x != null).ToList();
		TextureAtlases = TextureAtlasAssets.Where((TextureAtlas x) => x != null).ToList();
		BlockTileDecorInfos = BlockTileDecorInfoAssets.Where((BlockTileDecorInfo x) => x != null).ToList();
		LoadAnims();
		UIPrefabs = UIPrefabAssets;
		DebugFont = DebugFontAsset;
		AsyncLoadManager<IGlobalAsyncLoader>.Run();
		GameAudioSheets.Get().Initialize();
		SubstanceListHookup();
		BuildingDefs = new List<BuildingDef>();
		foreach (KPrefabID prefabAsset in PrefabAssets)
		{
			if (!(prefabAsset == null))
			{
				AddPrefab(prefabAsset);
			}
		}
		CreatePrefabs();
	}

	private void CreatePrefabs()
	{
		Db.Get();
		LegacyModMain.Load();
		Db.Get().Techs.PostProcess();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Db.Get();
	}

	private static void TryAddCountableTag(KPrefabID prefab)
	{
		foreach (Tag displayAsUnit in GameTags.DisplayAsUnits)
		{
			if (prefab.HasTag(displayAsUnit))
			{
				AddCountableTag(prefab.PrefabTag);
				break;
			}
		}
	}

	public static void AddCountableTag(Tag tag)
	{
		CountableTags.Add(tag);
	}

	public static bool IsTagCountable(Tag tag)
	{
		return CountableTags.Contains(tag);
	}

	private void LoadAnims()
	{
		KAnimBatchManager.DestroyInstance();
		KAnimGroupFile.DestroyInstance();
		KGlobalAnimParser.DestroyInstance();
		KAnimBatchManager.CreateInstance();
		KGlobalAnimParser.CreateInstance();
		KAnimGroupFile.LoadGroupResourceFile();
		if (BundledAssetsLoader.instance.Expansion1Assets != null)
		{
			AnimAssets.AddRange(BundledAssetsLoader.instance.Expansion1Assets.AnimAssets);
		}
		Anims = AnimAssets.Where((KAnimFile x) => x != null).ToList();
		Anims.AddRange(ModLoadedKAnims);
		AnimTable.Clear();
		foreach (KAnimFile anim in Anims)
		{
			if (anim != null)
			{
				HashedString key = anim.name;
				AnimTable[key] = anim;
			}
		}
		KAnimGroupFile.MapNamesToAnimFiles(AnimTable);
		Global.Instance.modManager.Load(Content.Animation);
		Anims.AddRange(ModLoadedKAnims);
		foreach (KAnimFile modLoadedKAnim in ModLoadedKAnims)
		{
			if (modLoadedKAnim != null)
			{
				HashedString key2 = modLoadedKAnim.name;
				AnimTable[key2] = modLoadedKAnim;
			}
		}
		Debug.Assert(AnimTable.Count > 0, "Anim Assets not yet loaded");
		KAnimGroupFile.LoadAll();
		KAnimBatchManager.Instance().CompleteInit();
	}

	private void SubstanceListHookup()
	{
		Dictionary<string, SubstanceTable> dictionary = new Dictionary<string, SubstanceTable> { { "", substanceTable } };
		if (BundledAssetsLoader.instance.Expansion1Assets != null)
		{
			dictionary["EXPANSION1_ID"] = BundledAssetsLoader.instance.Expansion1Assets.SubstanceTable;
		}
		Hashtable substanceList = new Hashtable();
		ElementsAudio.Instance.LoadData(AsyncLoadManager<IGlobalAsyncLoader>.AsyncLoader<ElementAudioFileLoader>.Get().entries);
		ElementLoader.Load(ref substanceList, dictionary);
	}

	public static string GetSimpleSoundEventName(string path)
	{
		string value = null;
		if (!simpleSoundEventNames.TryGetValue(path, out value))
		{
			int num = path.LastIndexOf('/');
			value = ((num != -1) ? path.Substring(num + 1) : path);
			simpleSoundEventNames[path] = value;
		}
		return value;
	}

	private static BuildingDef GetDef(IList<BuildingDef> defs, string prefab_id)
	{
		int count = defs.Count;
		for (int i = 0; i < count; i++)
		{
			if (defs[i].PrefabID == prefab_id)
			{
				return defs[i];
			}
		}
		return null;
	}

	public static BuildingDef GetBuildingDef(string prefab_id)
	{
		return GetDef(BuildingDefs, prefab_id);
	}

	public static TintedSprite GetTintedSprite(string name)
	{
		TintedSprite result = null;
		if (TintedSprites != null)
		{
			for (int i = 0; i < TintedSprites.Count; i++)
			{
				if (TintedSprites[i].sprite.name == name)
				{
					result = TintedSprites[i];
					break;
				}
			}
		}
		return result;
	}

	public static Sprite GetSprite(HashedString name)
	{
		Sprite value = null;
		if (Sprites != null)
		{
			Sprites.TryGetValue(name, out value);
		}
		return value;
	}

	public static VideoClip GetVideo(string name)
	{
		return Resources.Load<VideoClip>("video_webm/" + name);
	}

	public static Texture2D GetTexture(string name)
	{
		Texture2D result = null;
		if (Textures != null)
		{
			for (int i = 0; i < Textures.Count; i++)
			{
				if (Textures[i].name == name)
				{
					result = Textures[i];
					break;
				}
			}
		}
		return result;
	}

	public static ComicData GetComic(string id)
	{
		ComicData[] array = instance.comics;
		foreach (ComicData comicData in array)
		{
			if (comicData.name == id)
			{
				return comicData;
			}
		}
		return null;
	}

	public static void AddPrefab(KPrefabID prefab)
	{
		if (prefab == null)
		{
			return;
		}
		prefab.UpdateSaveLoadTag();
		if (PrefabsByTag.ContainsKey(prefab.PrefabTag))
		{
			Tag prefabTag = prefab.PrefabTag;
			Debug.LogWarning("Tried loading prefab with duplicate tag, ignoring: " + prefabTag.ToString());
			return;
		}
		PrefabsByTag[prefab.PrefabTag] = prefab;
		foreach (Tag tag in prefab.Tags)
		{
			if (!PrefabsByAdditionalTags.ContainsKey(tag))
			{
				PrefabsByAdditionalTags[tag] = new List<KPrefabID>();
			}
			PrefabsByAdditionalTags[tag].Add(prefab);
		}
		Prefabs.Add(prefab);
		TryAddCountableTag(prefab);
		if (OnAddPrefab != null)
		{
			OnAddPrefab(prefab);
		}
	}

	public static void RegisterOnAddPrefab(Action<KPrefabID> on_add)
	{
		OnAddPrefab = (Action<KPrefabID>)Delegate.Combine(OnAddPrefab, on_add);
		foreach (KPrefabID prefab in Prefabs)
		{
			on_add(prefab);
		}
	}

	public static void UnregisterOnAddPrefab(Action<KPrefabID> on_add)
	{
		OnAddPrefab = (Action<KPrefabID>)Delegate.Remove(OnAddPrefab, on_add);
	}

	public static void ClearOnAddPrefab()
	{
		OnAddPrefab = null;
	}

	public static GameObject GetPrefab(Tag tag)
	{
		GameObject obj = TryGetPrefab(tag);
		if (obj == null)
		{
			Tag tag2 = tag;
			Debug.LogWarning("Missing prefab: " + tag2.ToString());
		}
		return obj;
	}

	public static GameObject TryGetPrefab(Tag tag)
	{
		KPrefabID value = null;
		PrefabsByTag.TryGetValue(tag, out value);
		if (!(value != null))
		{
			return null;
		}
		return value.gameObject;
	}

	public static List<GameObject> GetPrefabsWithTag(Tag tag)
	{
		List<GameObject> list = new List<GameObject>();
		if (PrefabsByAdditionalTags.ContainsKey(tag))
		{
			for (int i = 0; i < PrefabsByAdditionalTags[tag].Count; i++)
			{
				list.Add(PrefabsByAdditionalTags[tag][i].gameObject);
			}
		}
		return list;
	}

	public static List<GameObject> GetPrefabsWithComponent<Type>()
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < Prefabs.Count; i++)
		{
			if (Prefabs[i].GetComponent<Type>() != null)
			{
				list.Add(Prefabs[i].gameObject);
			}
		}
		return list;
	}

	public static GameObject GetPrefabWithComponent<Type>()
	{
		List<GameObject> prefabsWithComponent = GetPrefabsWithComponent<Type>();
		Debug.Assert(prefabsWithComponent.Count > 0, "There are no prefabs of type " + typeof(Type).Name);
		return prefabsWithComponent[0];
	}

	public static List<Tag> GetPrefabTagsWithComponent<Type>()
	{
		List<Tag> list = new List<Tag>();
		for (int i = 0; i < Prefabs.Count; i++)
		{
			if (Prefabs[i].GetComponent<Type>() != null)
			{
				list.Add(Prefabs[i].PrefabID());
			}
		}
		return list;
	}

	public static Assets GetInstanceEditorOnly()
	{
		Assets[] array = (Assets[])Resources.FindObjectsOfTypeAll(typeof(Assets));
		_ = array?.LongLength;
		return array[0];
	}

	public static TextureAtlas GetTextureAtlas(string name)
	{
		foreach (TextureAtlas textureAtlase in TextureAtlases)
		{
			if (textureAtlase.name == name)
			{
				return textureAtlase;
			}
		}
		return null;
	}

	public static Material GetMaterial(string name)
	{
		foreach (Material material in Materials)
		{
			if (material.name == name)
			{
				return material;
			}
		}
		return null;
	}

	public static BlockTileDecorInfo GetBlockTileDecorInfo(string name)
	{
		foreach (BlockTileDecorInfo blockTileDecorInfo in BlockTileDecorInfos)
		{
			if (blockTileDecorInfo.name == name)
			{
				return blockTileDecorInfo;
			}
		}
		Debug.LogError("Could not find BlockTileDecorInfo named [" + name + "]");
		return null;
	}

	public static KAnimFile GetAnim(HashedString name)
	{
		if (!name.IsValid)
		{
			Debug.LogWarning("Invalid hash name");
			return null;
		}
		KAnimFile value = null;
		AnimTable.TryGetValue(name, out value);
		if (value == null)
		{
			Debug.LogWarning("Missing Anim: [" + name.ToString() + "]. You may have to run Collect Anim on the Assets prefab");
		}
		return value;
	}

	public static bool TryGetAnim(HashedString name, out KAnimFile anim)
	{
		if (!name.IsValid)
		{
			Debug.LogWarning("Invalid hash name");
			anim = null;
			return false;
		}
		AnimTable.TryGetValue(name, out anim);
		return anim != null;
	}

	public void OnAfterDeserialize()
	{
		TintedSpriteAssets = TintedSpriteAssets.Where((TintedSprite x) => x != null && x.sprite != null).ToList();
		TintedSpriteAssets.Sort((TintedSprite a, TintedSprite b) => a.name.CompareTo(b.name));
	}

	public void OnBeforeSerialize()
	{
	}

	public static void AddBuildingDef(BuildingDef def)
	{
		BuildingDefs = BuildingDefs.Where((BuildingDef x) => x.PrefabID != def.PrefabID).ToList();
		BuildingDefs.Add(def);
	}
}
