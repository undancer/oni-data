using System.Collections.Generic;
using UnityEngine;

public class RocketModuleCluster : RocketModule
{
	public RocketModulePerformance performanceStats;

	private static readonly EventSystem.IntraObjectHandler<RocketModuleCluster> OnNewConstructionDelegate = new EventSystem.IntraObjectHandler<RocketModuleCluster>(delegate(RocketModuleCluster component, object data)
	{
		component.OnNewConstruction(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RocketModuleCluster> OnLaunchConditionChangedDelegate = new EventSystem.IntraObjectHandler<RocketModuleCluster>(delegate(RocketModuleCluster component, object data)
	{
		component.OnLaunchConditionChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RocketModuleCluster> OnLandDelegate = new EventSystem.IntraObjectHandler<RocketModuleCluster>(delegate(RocketModuleCluster component, object data)
	{
		component.OnLand(data);
	});

	private CraftModuleInterface _craftInterface;

	public CraftModuleInterface CraftInterface
	{
		get
		{
			return _craftInterface;
		}
		set
		{
			_craftInterface = value;
			if (_craftInterface != null)
			{
				base.name = base.name + ": " + GetParentRocketName();
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(2121280625, OnNewConstructionDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (CraftInterface == null && DlcManager.FeatureClusterSpaceEnabled())
		{
			RegisterWithCraftModuleInterface();
		}
		if (GetComponent<RocketEngine>() == null && GetComponent<RocketEngineCluster>() == null && GetComponent<BuildingUnderConstruction>() == null)
		{
			Subscribe(1655598572, OnLaunchConditionChangedDelegate);
			Subscribe(-887025858, OnLandDelegate);
		}
	}

	protected void OnNewConstruction(object data)
	{
		Constructable constructable = (Constructable)data;
		if (!(constructable == null))
		{
			RocketModuleCluster component = constructable.GetComponent<RocketModuleCluster>();
			if (!(component == null) && component.CraftInterface != null)
			{
				component.CraftInterface.AddModule(this);
			}
		}
	}

	private void RegisterWithCraftModuleInterface()
	{
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>());
		foreach (GameObject item in attachedNetwork)
		{
			if (!(item == base.gameObject))
			{
				RocketModuleCluster component = item.GetComponent<RocketModuleCluster>();
				if (component != null)
				{
					component.CraftInterface.AddModule(this);
					break;
				}
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		CraftInterface.RemoveModule(this);
	}

	public override LaunchConditionManager FindLaunchConditionManager()
	{
		return CraftInterface.FindLaunchConditionManager();
	}

	public override string GetParentRocketName()
	{
		if (CraftInterface != null)
		{
			return CraftInterface.GetComponent<Clustercraft>().Name;
		}
		return parentRocketName;
	}

	private void OnLaunchConditionChanged(object data)
	{
		UpdateAnimations();
	}

	private void OnLand(object data)
	{
		UpdateAnimations();
	}

	protected void UpdateAnimations()
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		Clustercraft clustercraft = ((CraftInterface == null) ? null : CraftInterface.GetComponent<Clustercraft>());
		if (clustercraft != null && clustercraft.Status == Clustercraft.CraftStatus.Launching && component.HasAnimation("launch"))
		{
			component.ClearQueue();
			if (component.HasAnimation("launch_pre"))
			{
				component.Play("launch_pre");
			}
			component.Queue("launch", KAnim.PlayMode.Loop);
		}
		else if (CraftInterface != null && CraftInterface.CheckPreppedForLaunch())
		{
			component.initialAnim = "ready_to_launch";
			component.Play("pre_ready_to_launch");
			component.Queue("ready_to_launch", KAnim.PlayMode.Loop);
		}
		else
		{
			component.initialAnim = "grounded";
			component.Play("pst_ready_to_launch");
			component.Queue("grounded", KAnim.PlayMode.Loop);
		}
	}
}
