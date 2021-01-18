using System;
using System.Collections.Generic;
using System.Diagnostics;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/RocketModule")]
public class RocketModule : KMonoBehaviour
{
	public LaunchConditionManager conditionManager;

	public Dictionary<ProcessCondition.ProcessConditionType, List<ProcessCondition>> moduleConditions = new Dictionary<ProcessCondition.ProcessConditionType, List<ProcessCondition>>();

	public static Operational.Flag landedFlag = new Operational.Flag("landed", Operational.Flag.Type.Requirement);

	public RocketModulePerformance performanceStats;

	private string rocket_module_bg_base_string = "{0}{1}";

	private string rocket_module_bg_affix = "BG";

	private string rocket_module_bg_anim = "on";

	[SerializeField]
	private KAnimFile bgAnimFile = null;

	protected string parentRocketName = UI.STARMAP.DEFAULT_NAME;

	private static readonly EventSystem.IntraObjectHandler<RocketModule> DEBUG_OnDestroyDelegate = new EventSystem.IntraObjectHandler<RocketModule>(delegate(RocketModule component, object data)
	{
		component.DEBUG_OnDestroy(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RocketModule> OnRocketOnGroundTagDelegate = GameUtil.CreateHasTagHandler(GameTags.RocketOnGround, delegate(RocketModule component, object data)
	{
		component.OnRocketOnGroundTag(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RocketModule> OnRocketNotOnGroundTagDelegate = GameUtil.CreateHasTagHandler(GameTags.RocketNotOnGround, delegate(RocketModule component, object data)
	{
		component.OnRocketNotOnGroundTag(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RocketModule> OnNewConstructionDelegate = new EventSystem.IntraObjectHandler<RocketModule>(delegate(RocketModule component, object data)
	{
		component.OnNewConstruction(data);
	});

	public CraftModuleInterface CraftInterface
	{
		get;
		set;
	}

	public ProcessCondition AddModuleCondition(ProcessCondition.ProcessConditionType conditionType, ProcessCondition condition)
	{
		if (!moduleConditions.ContainsKey(conditionType))
		{
			moduleConditions.Add(conditionType, new List<ProcessCondition>());
		}
		if (!moduleConditions[conditionType].Contains(condition))
		{
			moduleConditions[conditionType].Add(condition);
		}
		return condition;
	}

	public List<ProcessCondition> GetConditionSet(ProcessCondition.ProcessConditionType conditionType)
	{
		List<ProcessCondition> value;
		if (conditionType == ProcessCondition.ProcessConditionType.All)
		{
			value = new List<ProcessCondition>();
			foreach (KeyValuePair<ProcessCondition.ProcessConditionType, List<ProcessCondition>> moduleCondition in moduleConditions)
			{
				value.AddRange(moduleCondition.Value);
			}
		}
		else
		{
			moduleConditions.TryGetValue(conditionType, out value);
		}
		return value;
	}

	public void SetBGKAnim(KAnimFile anim_file)
	{
		bgAnimFile = anim_file;
	}

	private void RegisterWithCraftModuleInterface()
	{
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>());
		foreach (GameObject item in attachedNetwork)
		{
			if (!(item == base.gameObject))
			{
				RocketModule component = item.GetComponent<RocketModule>();
				if (component != null)
				{
					component.CraftInterface.AddModule(this);
					break;
				}
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GameUtil.SubscribeToTags(this, OnRocketOnGroundTagDelegate, triggerImmediately: false);
		GameUtil.SubscribeToTags(this, OnRocketNotOnGroundTagDelegate, triggerImmediately: false);
		Subscribe(2121280625, OnNewConstructionDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (CraftInterface == null && DlcManager.IsExpansion1Active())
		{
			RegisterWithCraftModuleInterface();
		}
		if (!DlcManager.IsExpansion1Active())
		{
			conditionManager = FindLaunchConditionManager();
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(conditionManager);
			if (spacecraftFromLaunchConditionManager != null)
			{
				SetParentRocketName(spacecraftFromLaunchConditionManager.GetRocketName());
			}
			RegisterWithConditionManager();
		}
		KSelectable component = GetComponent<KSelectable>();
		if (component != null)
		{
			component.AddStatusItem(Db.Get().BuildingStatusItems.RocketName, this);
		}
		Subscribe(1502190696, DEBUG_OnDestroyDelegate);
		FixSorting();
		AttachableBuilding component2 = GetComponent<AttachableBuilding>();
		component2.onAttachmentNetworkChanged = (Action<AttachableBuilding>)Delegate.Combine(component2.onAttachmentNetworkChanged, new Action<AttachableBuilding>(OnAttachmentNetworkChanged));
		if (bgAnimFile != null)
		{
			AddBGGantry();
		}
	}

	public void FixSorting()
	{
		int num = 0;
		AttachableBuilding component = GetComponent<AttachableBuilding>();
		while (component != null)
		{
			BuildingAttachPoint attachedTo = component.GetAttachedTo();
			if (attachedTo != null)
			{
				component = attachedTo.GetComponent<AttachableBuilding>();
				num++;
				continue;
			}
			break;
		}
		Vector3 localPosition = base.transform.GetLocalPosition();
		localPosition.z = Grid.GetLayerZ(Grid.SceneLayer.Building) - (float)num * 0.01f;
		base.transform.SetLocalPosition(localPosition);
		KBatchedAnimController component2 = GetComponent<KBatchedAnimController>();
		if (component2.enabled)
		{
			component2.enabled = false;
			component2.enabled = true;
		}
	}

	private void OnAttachmentNetworkChanged(AttachableBuilding ab)
	{
		FixSorting();
	}

	private void AddBGGantry()
	{
		KAnimControllerBase component = GetComponent<KAnimControllerBase>();
		GameObject gameObject = new GameObject();
		gameObject.name = string.Format(rocket_module_bg_base_string, base.name, rocket_module_bg_affix);
		gameObject.SetActive(value: false);
		Vector3 position = component.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.InteriorWall);
		gameObject.transform.SetPosition(position);
		gameObject.transform.parent = base.transform;
		KBatchedAnimController kBatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.AnimFiles = new KAnimFile[1]
		{
			bgAnimFile
		};
		kBatchedAnimController.initialAnim = rocket_module_bg_anim;
		kBatchedAnimController.fgLayer = Grid.SceneLayer.NoLayer;
		kBatchedAnimController.initialMode = KAnim.PlayMode.Paused;
		kBatchedAnimController.FlipX = component.FlipX;
		kBatchedAnimController.FlipY = component.FlipY;
		gameObject.SetActive(value: true);
	}

	private void DEBUG_OnDestroy(object data)
	{
		if (conditionManager != null && !App.IsExiting && !KMonoBehaviour.isLoadingScene)
		{
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(conditionManager);
			conditionManager.DEBUG_TraceModuleDestruction(base.name, (spacecraftFromLaunchConditionManager == null) ? "null spacecraft" : spacecraftFromLaunchConditionManager.state.ToString(), new StackTrace(fNeedFileInfo: true).ToString());
		}
	}

	private void OnRocketOnGroundTag(object data)
	{
		RegisterComponents();
		Operational component = GetComponent<Operational>();
		if (component != null)
		{
			component.SetFlag(landedFlag, value: true);
		}
	}

	private void OnRocketNotOnGroundTag(object data)
	{
		DeregisterComponents();
		Operational component = GetComponent<Operational>();
		if (component != null)
		{
			component.SetFlag(landedFlag, value: false);
		}
	}

	private void OnNewConstruction(object data)
	{
		Constructable constructable = (Constructable)data;
		if (!(constructable == null))
		{
			RocketModule component = constructable.GetComponent<RocketModule>();
			if (!(component == null) && component.CraftInterface != null)
			{
				component.CraftInterface.AddModule(this);
			}
		}
	}

	public void DeregisterComponents()
	{
		KSelectable component = GetComponent<KSelectable>();
		component.IsSelectable = false;
		if (SelectTool.Instance.selected == component)
		{
			SelectTool.Instance.Select(null);
		}
		Deconstructable component2 = GetComponent<Deconstructable>();
		if (component2 != null)
		{
			component2.SetAllowDeconstruction(allow: false);
		}
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		if (handle.IsValid())
		{
			GameComps.StructureTemperatures.Disable(handle);
		}
		FakeFloorAdder component3 = GetComponent<FakeFloorAdder>();
		if (component3 != null)
		{
			component3.SetFloor(active: false);
		}
		AccessControl component4 = GetComponent<AccessControl>();
		if (component4 != null)
		{
			component4.SetRegistered(newRegistered: false);
		}
		ManualDeliveryKG[] components = GetComponents<ManualDeliveryKG>();
		ManualDeliveryKG[] array = components;
		foreach (ManualDeliveryKG manualDeliveryKG in array)
		{
			DebugUtil.DevAssert(!manualDeliveryKG.IsPaused, "RocketModule ManualDeliver chore was already paused, when this rocket lands it will re-enable it.");
			manualDeliveryKG.Pause(pause: true, "Rocket heading to space");
		}
		BuildingConduitEndpoints[] components2 = GetComponents<BuildingConduitEndpoints>();
		BuildingConduitEndpoints[] array2 = components2;
		foreach (BuildingConduitEndpoints buildingConduitEndpoints in array2)
		{
			buildingConduitEndpoints.RemoveEndPoint();
		}
	}

	public void RegisterComponents()
	{
		KSelectable component = GetComponent<KSelectable>();
		component.IsSelectable = true;
		Deconstructable component2 = GetComponent<Deconstructable>();
		if (component2 != null)
		{
			component2.SetAllowDeconstruction(allow: true);
		}
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		if (handle.IsValid())
		{
			GameComps.StructureTemperatures.Enable(handle);
		}
		Storage[] components = GetComponents<Storage>();
		Storage[] array = components;
		foreach (Storage storage in array)
		{
			storage.UpdateStoredItemCachedCells();
		}
		FakeFloorAdder component3 = GetComponent<FakeFloorAdder>();
		if (component3 != null)
		{
			component3.SetFloor(active: true);
		}
		AccessControl component4 = GetComponent<AccessControl>();
		if (component4 != null)
		{
			component4.SetRegistered(newRegistered: true);
		}
		ManualDeliveryKG[] components2 = GetComponents<ManualDeliveryKG>();
		ManualDeliveryKG[] array2 = components2;
		foreach (ManualDeliveryKG manualDeliveryKG in array2)
		{
			manualDeliveryKG.Pause(pause: false, "Landing on world");
		}
		BuildingConduitEndpoints[] components3 = GetComponents<BuildingConduitEndpoints>();
		BuildingConduitEndpoints[] array3 = components3;
		foreach (BuildingConduitEndpoints buildingConduitEndpoints in array3)
		{
			buildingConduitEndpoints.AddEndpoint();
		}
	}

	private void ToggleComponent(Type cmpType, bool enabled)
	{
		MonoBehaviour monoBehaviour = (MonoBehaviour)GetComponent(cmpType);
		if (monoBehaviour != null)
		{
			monoBehaviour.enabled = enabled;
		}
	}

	public void RegisterWithConditionManager()
	{
		Debug.Assert(!DlcManager.IsExpansion1Active());
		if (conditionManager != null)
		{
			conditionManager.RegisterRocketModule(this);
		}
		else
		{
			Debug.LogWarning("Module conditionManager is null");
		}
	}

	protected override void OnCleanUp()
	{
		if (conditionManager != null)
		{
			conditionManager.UnregisterRocketModule(this);
		}
		if (DlcManager.IsExpansion1Active())
		{
			CraftInterface.RemoveModule(this);
		}
		base.OnCleanUp();
	}

	public LaunchConditionManager FindLaunchConditionManager()
	{
		if (DlcManager.IsExpansion1Active())
		{
			return CraftInterface.FindLaunchConditionManager();
		}
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>());
		foreach (GameObject item in attachedNetwork)
		{
			LaunchConditionManager component = item.GetComponent<LaunchConditionManager>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	public void SetParentRocketName(string newName)
	{
		parentRocketName = newName;
		NameDisplayScreen.Instance.UpdateName(base.gameObject);
	}

	public string GetParentRocketName()
	{
		if (CraftInterface != null)
		{
			return CraftInterface.GetComponent<Clustercraft>().Name;
		}
		return parentRocketName;
	}

	public void MoveToSpace()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		Building component = GetComponent<Building>();
		BuildingDef def = component.Def;
		def.UnmarkArea(cell, component.Orientation, component.Def.ObjectLayer, base.gameObject);
		TransformExtensions.SetPosition(position: new Vector3(-1f, -1f, 0f), transform: base.gameObject.transform);
		LogicPorts component2 = GetComponent<LogicPorts>();
		if (component2 != null)
		{
			component2.OnMove();
		}
	}

	public void MoveToPad(int newCell)
	{
		base.gameObject.transform.SetPosition(Grid.CellToPos(newCell, CellAlignment.Bottom, Grid.SceneLayer.Building));
		int cell = Grid.PosToCell(base.transform.GetPosition());
		Building component = GetComponent<Building>();
		component.RefreshCells();
		BuildingDef def = component.Def;
		def.MarkArea(cell, component.Orientation, component.Def.ObjectLayer, base.gameObject);
		LogicPorts component2 = GetComponent<LogicPorts>();
		if (component2 != null)
		{
			component2.OnMove();
		}
	}
}
