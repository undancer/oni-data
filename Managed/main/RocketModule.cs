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

	public bool operationalLandedRequired = true;

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

	private static readonly EventSystem.IntraObjectHandler<RocketModule> OnLaunchConditionChangedDelegate = new EventSystem.IntraObjectHandler<RocketModule>(delegate(RocketModule component, object data)
	{
		component.OnLaunchConditionChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RocketModule> OnLandDelegate = new EventSystem.IntraObjectHandler<RocketModule>(delegate(RocketModule component, object data)
	{
		component.OnLand(data);
	});

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
		List<ProcessCondition> list = new List<ProcessCondition>();
		if (conditionType == ProcessCondition.ProcessConditionType.All)
		{
			foreach (KeyValuePair<ProcessCondition.ProcessConditionType, List<ProcessCondition>> moduleCondition in moduleConditions)
			{
				list.AddRange(moduleCondition.Value);
			}
		}
		else if (moduleConditions.ContainsKey(conditionType))
		{
			list = moduleConditions[conditionType];
		}
		return list;
	}

	public void SetBGKAnim(KAnimFile anim_file)
	{
		bgAnimFile = anim_file;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GameUtil.SubscribeToTags(this, OnRocketOnGroundTagDelegate, triggerImmediately: false);
		GameUtil.SubscribeToTags(this, OnRocketNotOnGroundTagDelegate, triggerImmediately: false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
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
		if (GetComponent<RocketEngine>() == null && GetComponent<RocketEngineCluster>() == null && GetComponent<BuildingUnderConstruction>() == null)
		{
			Subscribe(1655598572, OnLaunchConditionChangedDelegate);
			Subscribe(-887025858, OnLandDelegate);
		}
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
		if (operationalLandedRequired && component != null)
		{
			component.SetFlag(landedFlag, value: true);
		}
	}

	private void OnRocketNotOnGroundTag(object data)
	{
		DeregisterComponents();
		Operational component = GetComponent<Operational>();
		if (operationalLandedRequired && component != null)
		{
			component.SetFlag(landedFlag, value: false);
		}
	}

	private void OnLaunchConditionChanged(object data)
	{
		UpdateAnimations();
	}

	private void OnLand(object data)
	{
		UpdateAnimations();
	}

	private void UpdateAnimations()
	{
		RocketModuleCluster component = GetComponent<RocketModuleCluster>();
		KBatchedAnimController component2 = GetComponent<KBatchedAnimController>();
		CraftModuleInterface craftInterface = component.CraftInterface;
		if (craftInterface.CheckPreppedForLaunch())
		{
			component2.initialAnim = "ready_to_launch";
			component2.Play("pre_ready_to_launch");
			component2.Queue("ready_to_launch", KAnim.PlayMode.Loop);
		}
		else if (craftInterface.CurrentPad == null)
		{
			component2.Play("launch", KAnim.PlayMode.Loop);
		}
		else
		{
			component2.initialAnim = "grounded";
			component2.Play("pst_ready_to_launch");
			component2.Queue("grounded", KAnim.PlayMode.Loop);
		}
	}

	public void DeregisterComponents()
	{
		int cell = Grid.PosToCell(this);
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
		ReorderableBuilding component5 = GetComponent<ReorderableBuilding>();
		if (component5 != null)
		{
			component5.ShowReorderArm(show: false);
		}
		BuildingComplete component6 = GetComponent<BuildingComplete>();
		if (component6 != null)
		{
			component6.UpdatePosition(cell);
		}
		Workable component7 = GetComponent<Workable>();
		if (component7 != null)
		{
			component7.RefreshReachability();
		}
		Structure component8 = GetComponent<Structure>();
		if (component8 != null)
		{
			component8.UpdatePosition(cell);
		}
		WireUtilitySemiVirtualNetworkLink component9 = GetComponent<WireUtilitySemiVirtualNetworkLink>();
		if (component9 != null)
		{
			component9.SetLinkConnected(connect: false);
		}
	}

	public void RegisterComponents()
	{
		int cell = Grid.PosToCell(this);
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
		ReorderableBuilding component5 = GetComponent<ReorderableBuilding>();
		if (component5 != null)
		{
			component5.ShowReorderArm(show: true);
		}
		BuildingComplete component6 = GetComponent<BuildingComplete>();
		if (component6 != null)
		{
			component6.UpdatePosition(cell);
		}
		Workable component7 = GetComponent<Workable>();
		if (component7 != null)
		{
			component7.RefreshReachability();
		}
		Structure component8 = GetComponent<Structure>();
		if (component8 != null)
		{
			component8.UpdatePosition(cell);
		}
		WireUtilitySemiVirtualNetworkLink component9 = GetComponent<WireUtilitySemiVirtualNetworkLink>();
		if (component9 != null)
		{
			component9.SetLinkConnected(connect: true);
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
	}

	protected override void OnCleanUp()
	{
		if (conditionManager != null)
		{
			conditionManager.UnregisterRocketModule(this);
		}
		base.OnCleanUp();
	}

	public virtual LaunchConditionManager FindLaunchConditionManager()
	{
		if (!DlcManager.IsExpansion1Active())
		{
			List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>());
			foreach (GameObject item in attachedNetwork)
			{
				LaunchConditionManager component = item.GetComponent<LaunchConditionManager>();
				if (component != null)
				{
					return component;
				}
			}
		}
		return null;
	}

	public void SetParentRocketName(string newName)
	{
		parentRocketName = newName;
		NameDisplayScreen.Instance.UpdateName(base.gameObject);
	}

	public virtual string GetParentRocketName()
	{
		return parentRocketName;
	}

	public void MoveToSpace()
	{
		Prioritizable component = GetComponent<Prioritizable>();
		if (component != null && component.GetMyWorld() != null)
		{
			component.GetMyWorld().RemoveTopPriorityPrioritizable(component);
		}
		int cell = Grid.PosToCell(base.transform.GetPosition());
		Building component2 = GetComponent<Building>();
		BuildingDef def = component2.Def;
		def.UnmarkArea(cell, component2.Orientation, component2.Def.ObjectLayer, base.gameObject);
		TransformExtensions.SetPosition(position: new Vector3(-1f, -1f, 0f), transform: base.gameObject.transform);
		LogicPorts component3 = GetComponent<LogicPorts>();
		if (component3 != null)
		{
			component3.OnMove();
		}
		KSelectable component4 = GetComponent<KSelectable>();
		component4.ToggleStatusItem(Db.Get().BuildingStatusItems.Entombed, on: false, this);
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
		Prioritizable component3 = GetComponent<Prioritizable>();
		if (component3 != null && component3.IsTopPriority())
		{
			component3.GetMyWorld().AddTopPriorityPrioritizable(component3);
		}
	}
}
