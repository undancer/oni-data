using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BuildingLoader")]
public class BuildingLoader : KMonoBehaviour
{
	private GameObject previewTemplate;

	private GameObject constructionTemplate;

	public static BuildingLoader Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		previewTemplate = CreatePreviewTemplate();
		constructionTemplate = CreateConstructionTemplate();
	}

	private GameObject CreateTemplate()
	{
		GameObject gameObject = new GameObject();
		gameObject.SetActive(value: false);
		gameObject.AddOrGet<KPrefabID>();
		gameObject.AddOrGet<KSelectable>();
		gameObject.AddOrGet<StateMachineController>();
		PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
		primaryElement.Mass = 1f;
		primaryElement.Temperature = 293f;
		return gameObject;
	}

	private GameObject CreatePreviewTemplate()
	{
		GameObject gameObject = CreateTemplate();
		gameObject.AddComponent<BuildingPreview>();
		return gameObject;
	}

	private GameObject CreateConstructionTemplate()
	{
		GameObject gameObject = CreateTemplate();
		gameObject.AddOrGet<BuildingUnderConstruction>();
		gameObject.AddOrGet<Constructable>();
		Storage storage = gameObject.AddComponent<Storage>();
		storage.doDiseaseTransfer = false;
		gameObject.AddOrGet<Prioritizable>();
		gameObject.AddOrGet<Notifier>();
		gameObject.AddOrGet<SaveLoadRoot>();
		return gameObject;
	}

	public GameObject CreateBuilding(BuildingDef def, GameObject go, GameObject parent = null)
	{
		go = Object.Instantiate(go);
		go.name = def.PrefabID;
		if (parent != null)
		{
			go.transform.parent = parent.transform;
		}
		go.GetComponent<Building>().Def = def;
		return go;
	}

	private static bool Add2DComponents(BuildingDef def, GameObject go, string initialAnimState = null, bool no_collider = false, int layer = -1)
	{
		bool flag = def.AnimFiles != null && def.AnimFiles.Length != 0;
		if (layer == -1)
		{
			layer = LayerMask.NameToLayer("Default");
		}
		go.layer = layer;
		KBatchedAnimController[] components = go.GetComponents<KBatchedAnimController>();
		if (components.Length > 1)
		{
			for (int i = 2; i < components.Length; i++)
			{
				Object.DestroyImmediate(components[i]);
			}
		}
		if (def.BlockTileAtlas == null)
		{
			KBatchedAnimController kBatchedAnimController = UpdateComponentRequirement<KBatchedAnimController>(go, flag);
			if (kBatchedAnimController != null)
			{
				kBatchedAnimController.AnimFiles = def.AnimFiles;
				if (def.isKAnimTile)
				{
					kBatchedAnimController.initialAnim = null;
				}
				else
				{
					if (def.isUtility && initialAnimState == null)
					{
						initialAnimState = "idle";
					}
					else if (go.GetComponent<Door>() != null)
					{
						initialAnimState = "closed";
					}
					kBatchedAnimController.initialAnim = ((initialAnimState != null) ? initialAnimState : def.DefaultAnimState);
				}
				kBatchedAnimController.SetFGLayer(def.ForegroundLayer);
				kBatchedAnimController.materialType = KAnimBatchGroup.MaterialType.Default;
			}
		}
		KBoxCollider2D kBoxCollider2D = UpdateComponentRequirement<KBoxCollider2D>(go, flag && !no_collider);
		if (kBoxCollider2D != null)
		{
			kBoxCollider2D.offset = new Vector3(0f, 0.5f * (float)def.HeightInCells, 0f);
			kBoxCollider2D.size = new Vector3(def.WidthInCells, def.HeightInCells, 0f);
		}
		if (def.AnimFiles == null)
		{
			Debug.LogError(def.Name + " Def missing anim files");
		}
		return flag;
	}

	private static T UpdateComponentRequirement<T>(GameObject go, bool required) where T : Component
	{
		T val = go.GetComponent(typeof(T)) as T;
		if (!required && (Object)val != (Object)null)
		{
			Object.DestroyImmediate(val, allowDestroyingAssets: true);
			val = null;
		}
		else if (required && (Object)val == (Object)null)
		{
			val = go.AddComponent(typeof(T)) as T;
		}
		return val;
	}

	public static KPrefabID AddID(GameObject go, string str)
	{
		KPrefabID kPrefabID = go.GetComponent<KPrefabID>();
		if (kPrefabID == null)
		{
			kPrefabID = go.AddComponent<KPrefabID>();
		}
		kPrefabID.PrefabTag = new Tag(str);
		kPrefabID.SaveLoadTag = kPrefabID.PrefabTag;
		return kPrefabID;
	}

	public GameObject CreateBuildingUnderConstruction(BuildingDef def)
	{
		GameObject gameObject = CreateBuilding(def, constructionTemplate);
		Object.DontDestroyOnLoad(gameObject);
		KSelectable component = gameObject.GetComponent<KSelectable>();
		component.SetName(def.Name);
		for (int i = 0; i < def.Mass.Length; i++)
		{
			gameObject.GetComponent<PrimaryElement>().MassPerUnit += def.Mass[i];
		}
		KPrefabID kPrefabID = AddID(gameObject, def.PrefabID + "UnderConstruction");
		kPrefabID.AddTag(GameTags.UnderConstruction);
		UpdateComponentRequirement<BuildingCellVisualizer>(gameObject, def.CheckRequiresBuildingCellVisualizer());
		Constructable component2 = gameObject.GetComponent<Constructable>();
		component2.SetWorkTime(def.ConstructionTime);
		if (def.Cancellable)
		{
			gameObject.AddOrGet<Cancellable>();
		}
		Rotatable rotatable = UpdateComponentRequirement<Rotatable>(gameObject, def.PermittedRotations != PermittedRotations.Unrotatable);
		if ((bool)rotatable)
		{
			rotatable.permittedRotations = def.PermittedRotations;
		}
		Add2DComponents(def, gameObject, "place", no_collider: false, kPrefabID.defaultLayer = LayerMask.NameToLayer("Construction"));
		UpdateComponentRequirement<Vent>(gameObject, required: false);
		bool required = def.BuildingComplete.GetComponent<AnimTileable>() != null;
		UpdateComponentRequirement<AnimTileable>(gameObject, required);
		if (def.RequiresPowerInput)
		{
			GeneratedBuildings.RegisterSingleLogicInputPort(gameObject);
		}
		Assets.AddPrefab(kPrefabID);
		gameObject.PreInit();
		GeneratedBuildings.InitializeHighEnergyParticlePorts(gameObject, def);
		GeneratedBuildings.InitializeLogicPorts(gameObject, def);
		return gameObject;
	}

	public GameObject CreateBuildingComplete(GameObject go, BuildingDef def)
	{
		go.name = def.PrefabID + "Complete";
		go.transform.SetPosition(new Vector3(0f, 0f, Grid.GetLayerZ(def.SceneLayer)));
		KSelectable component = go.GetComponent<KSelectable>();
		component.SetName(def.Name);
		PrimaryElement component2 = go.GetComponent<PrimaryElement>();
		component2.MassPerUnit = 0f;
		for (int i = 0; i < def.Mass.Length; i++)
		{
			component2.MassPerUnit += def.Mass[i];
		}
		component2.Temperature = 273.15f;
		BuildingHP buildingHP = go.AddOrGet<BuildingHP>();
		if (def.Invincible)
		{
			buildingHP.invincible = true;
		}
		buildingHP.SetHitPoints(def.HitPoints);
		if (def.Repairable)
		{
			UpdateComponentRequirement<Repairable>(go, required: true);
		}
		int defaultLayer = (go.layer = LayerMask.NameToLayer("Default"));
		Building component3 = go.GetComponent<BuildingComplete>();
		component3.Def = def;
		if (def.InputConduitType != 0 || def.OutputConduitType != 0)
		{
			go.AddComponent<BuildingConduitEndpoints>();
		}
		if (!Add2DComponents(def, go))
		{
			Debug.Log(def.Name + " is not yet a 2d building!");
		}
		UpdateComponentRequirement<EnergyConsumer>(go, def.RequiresPowerInput);
		Rotatable rotatable = UpdateComponentRequirement<Rotatable>(go, def.PermittedRotations != PermittedRotations.Unrotatable);
		if ((bool)rotatable)
		{
			rotatable.permittedRotations = def.PermittedRotations;
		}
		if (def.Breakable)
		{
			go.AddComponent<Breakable>();
		}
		ConduitConsumer conduitConsumer = UpdateComponentRequirement<ConduitConsumer>(go, def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid);
		if (conduitConsumer != null)
		{
			conduitConsumer.SetConduitData(def.InputConduitType);
		}
		bool required = def.RequiresPowerInput || def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid;
		RequireInputs requireInputs = UpdateComponentRequirement<RequireInputs>(go, required);
		if (requireInputs != null)
		{
			requireInputs.SetRequirements(def.RequiresPowerInput, def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid);
		}
		UpdateComponentRequirement<RequireOutputs>(go, def.OutputConduitType != ConduitType.None);
		UpdateComponentRequirement<Operational>(go, !def.isUtility);
		if (def.Floodable)
		{
			go.AddComponent<Floodable>();
		}
		if (def.Disinfectable)
		{
			go.AddOrGet<AutoDisinfectable>();
			go.AddOrGet<Disinfectable>();
		}
		if (def.Overheatable)
		{
			Overheatable overheatable = go.AddComponent<Overheatable>();
			overheatable.baseOverheatTemp = def.OverheatTemperature;
			overheatable.baseFatalTemp = def.FatalHot;
		}
		if (def.Entombable)
		{
			go.AddComponent<Structure>();
		}
		if (def.RequiresPowerInput)
		{
			GeneratedBuildings.RegisterSingleLogicInputPort(go);
			go.AddOrGet<LogicOperationalController>();
		}
		UpdateComponentRequirement<BuildingCellVisualizer>(go, def.CheckRequiresBuildingCellVisualizer());
		if (def.BaseDecor != 0f)
		{
			DecorProvider decorProvider = UpdateComponentRequirement<DecorProvider>(go, required: true);
			decorProvider.baseDecor = def.BaseDecor;
			decorProvider.baseRadius = def.BaseDecorRadius;
		}
		if (def.AttachmentSlotTag != Tag.Invalid)
		{
			AttachableBuilding attachableBuilding = UpdateComponentRequirement<AttachableBuilding>(go, required: true);
			attachableBuilding.attachableToTag = def.AttachmentSlotTag;
		}
		KPrefabID kPrefabID = AddID(go, def.PrefabID);
		kPrefabID.defaultLayer = defaultLayer;
		Assets.AddPrefab(kPrefabID);
		go.PreInit();
		GeneratedBuildings.InitializeHighEnergyParticlePorts(go, def);
		GeneratedBuildings.InitializeLogicPorts(go, def);
		return go;
	}

	public GameObject CreateBuildingPreview(BuildingDef def)
	{
		GameObject gameObject = CreateBuilding(def, previewTemplate);
		Object.DontDestroyOnLoad(gameObject);
		int num = LayerMask.NameToLayer("Place");
		gameObject.transform.SetPosition(new Vector3(0f, 0f, Grid.GetLayerZ(def.SceneLayer)));
		Add2DComponents(def, gameObject, "place", no_collider: true, num);
		KAnimControllerBase component = gameObject.GetComponent<KAnimControllerBase>();
		if (component != null)
		{
			component.fgLayer = Grid.SceneLayer.NoLayer;
		}
		Rotatable rotatable = UpdateComponentRequirement<Rotatable>(gameObject, def.PermittedRotations != PermittedRotations.Unrotatable);
		if ((bool)rotatable)
		{
			rotatable.permittedRotations = def.PermittedRotations;
		}
		KPrefabID kPrefabID = AddID(gameObject, def.PrefabID + "Preview");
		kPrefabID.defaultLayer = num;
		KSelectable component2 = gameObject.GetComponent<KSelectable>();
		component2.SetName(def.Name);
		UpdateComponentRequirement<BuildingCellVisualizer>(gameObject, def.CheckRequiresBuildingCellVisualizer());
		KAnimGraphTileVisualizer component3 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
		if (component3 != null)
		{
			Object.DestroyImmediate(component3);
		}
		if (def.RequiresPowerInput)
		{
			GeneratedBuildings.RegisterSingleLogicInputPort(gameObject);
		}
		gameObject.PreInit();
		GeneratedBuildings.InitializeHighEnergyParticlePorts(gameObject, def);
		Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
		GeneratedBuildings.InitializeLogicPorts(gameObject, def);
		return gameObject;
	}
}
