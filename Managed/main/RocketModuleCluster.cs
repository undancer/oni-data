using System.Collections.Generic;
using UnityEngine;

public class RocketModuleCluster : RocketModule
{
	public RocketModulePerformance performanceStats;

	private static readonly EventSystem.IntraObjectHandler<RocketModuleCluster> OnNewConstructionDelegate = new EventSystem.IntraObjectHandler<RocketModuleCluster>(delegate(RocketModuleCluster component, object data)
	{
		component.OnNewConstruction(data);
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
		if (CraftInterface == null && DlcManager.IsExpansion1Active())
		{
			RegisterWithCraftModuleInterface();
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
}
