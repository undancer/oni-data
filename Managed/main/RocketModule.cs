using System;
using System.Collections.Generic;
using System.Diagnostics;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/RocketModule")]
public class RocketModule : KMonoBehaviour
{
	protected bool isSuspended;

	public LaunchConditionManager conditionManager;

	public List<RocketLaunchCondition> launchConditions = new List<RocketLaunchCondition>();

	public List<RocketFlightCondition> flightConditions = new List<RocketFlightCondition>();

	private string rocket_module_bg_base_string = "{0}{1}";

	private string rocket_module_bg_affix = "BG";

	private string rocket_module_bg_anim = "on";

	[SerializeField]
	private KAnimFile bgAnimFile;

	protected string parentRocketName = UI.STARMAP.DEFAULT_NAME;

	private static readonly EventSystem.IntraObjectHandler<RocketModule> OnLaunchDelegate = new EventSystem.IntraObjectHandler<RocketModule>(delegate(RocketModule component, object data)
	{
		component.OnLaunch(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RocketModule> OnLandDelegate = new EventSystem.IntraObjectHandler<RocketModule>(delegate(RocketModule component, object data)
	{
		component.OnLand(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RocketModule> DEBUG_OnDestroyDelegate = new EventSystem.IntraObjectHandler<RocketModule>(delegate(RocketModule component, object data)
	{
		component.DEBUG_OnDestroy(data);
	});

	public RocketLaunchCondition AddLaunchCondition(RocketLaunchCondition condition)
	{
		if (!launchConditions.Contains(condition))
		{
			launchConditions.Add(condition);
		}
		return condition;
	}

	public RocketFlightCondition AddFlightCondition(RocketFlightCondition condition)
	{
		if (!flightConditions.Contains(condition))
		{
			flightConditions.Add(condition);
		}
		return condition;
	}

	public void SetBGKAnim(KAnimFile anim_file)
	{
		bgAnimFile = anim_file;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		conditionManager = FindLaunchConditionManager();
		Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(conditionManager);
		if (spacecraftFromLaunchConditionManager != null)
		{
			SetParentRocketName(spacecraftFromLaunchConditionManager.GetRocketName());
		}
		RegisterWithConditionManager();
		KSelectable component = GetComponent<KSelectable>();
		if (component != null)
		{
			component.AddStatusItem(Db.Get().BuildingStatusItems.RocketName, this);
		}
		if (conditionManager != null && conditionManager.GetComponent<KPrefabID>().HasTag(GameTags.RocketNotOnGround))
		{
			OnLaunch(null);
		}
		Subscribe(-1056989049, OnLaunchDelegate);
		Subscribe(238242047, OnLandDelegate);
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
			if (!(attachedTo != null))
			{
				break;
			}
			component = attachedTo.GetComponent<AttachableBuilding>();
			num++;
		}
		Vector3 localPosition = base.transform.GetLocalPosition();
		localPosition.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingFront) - (float)num * 0.01f;
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

	public void OnConditionManagerTagsChanged(object data)
	{
		if (conditionManager.GetComponent<KPrefabID>().HasTag(GameTags.RocketNotOnGround))
		{
			OnLaunch(null);
		}
	}

	private void OnLaunch(object data)
	{
		KSelectable component = GetComponent<KSelectable>();
		component.IsSelectable = false;
		if (SelectTool.Instance.selected == component)
		{
			SelectTool.Instance.Select(null);
		}
		ConduitConsumer component2 = GetComponent<ConduitConsumer>();
		if ((bool)component2)
		{
			ConduitType conduitType = component2.conduitType;
			if ((uint)(conduitType - 1) <= 1u)
			{
				component2.consumptionRate = 0f;
			}
		}
		Deconstructable component3 = GetComponent<Deconstructable>();
		if (component3 != null)
		{
			component3.SetAllowDeconstruction(allow: false);
		}
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		if (handle.IsValid())
		{
			GameComps.StructureTemperatures.Disable(handle);
		}
		ManualDeliveryKG[] components = GetComponents<ManualDeliveryKG>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Pause(pause: true, "Rocket in space");
		}
		ToggleComponent(typeof(ElementConsumer), enabled: false);
		ToggleComponent(typeof(ElementConverter), enabled: false);
		ToggleComponent(typeof(ConduitDispenser), enabled: false);
		ToggleComponent(typeof(SolidConduitDispenser), enabled: false);
		ToggleComponent(typeof(EnergyConsumer), enabled: false);
	}

	private void OnLand(object data)
	{
		GetComponent<KSelectable>().IsSelectable = true;
		ConduitConsumer component = GetComponent<ConduitConsumer>();
		if ((bool)component)
		{
			switch (component.conduitType)
			{
			case ConduitType.Gas:
				GetComponent<ConduitConsumer>().consumptionRate = 1f;
				break;
			case ConduitType.Liquid:
				GetComponent<ConduitConsumer>().consumptionRate = 10f;
				break;
			}
		}
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
		ManualDeliveryKG[] components = GetComponents<ManualDeliveryKG>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Pause(pause: false, "landed");
		}
		ToggleComponent(typeof(ElementConsumer), enabled: true);
		ToggleComponent(typeof(ElementConverter), enabled: true);
		ToggleComponent(typeof(ConduitDispenser), enabled: true);
		ToggleComponent(typeof(SolidConduitDispenser), enabled: true);
		ToggleComponent(typeof(EnergyConsumer), enabled: true);
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
		base.OnCleanUp();
	}

	public virtual void OnSuspend(object data)
	{
		isSuspended = true;
	}

	public bool IsSuspended()
	{
		return isSuspended;
	}

	public LaunchConditionManager FindLaunchConditionManager()
	{
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>()))
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
		return parentRocketName;
	}
}
