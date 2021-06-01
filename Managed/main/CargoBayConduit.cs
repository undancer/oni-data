using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CargoBay")]
public class CargoBayConduit : KMonoBehaviour
{
	public static Dictionary<ConduitType, CargoBay.CargoType> ElementToCargoMap = new Dictionary<ConduitType, CargoBay.CargoType>
	{
		{
			ConduitType.Solid,
			CargoBay.CargoType.Solids
		},
		{
			ConduitType.Liquid,
			CargoBay.CargoType.Liquids
		},
		{
			ConduitType.Gas,
			CargoBay.CargoType.Gasses
		}
	};

	private static readonly EventSystem.IntraObjectHandler<CargoBayConduit> OnLaunchDelegate = new EventSystem.IntraObjectHandler<CargoBayConduit>(delegate(CargoBayConduit component, object data)
	{
		component.OnLaunch(data);
	});

	private static readonly EventSystem.IntraObjectHandler<CargoBayConduit> OnLandDelegate = new EventSystem.IntraObjectHandler<CargoBayConduit>(delegate(CargoBayConduit component, object data)
	{
		component.OnLand(data);
	});

	private static StatusItem connectedPortStatus;

	private static StatusItem connectedWrongPortStatus;

	private static StatusItem connectedNoPortStatus;

	private CargoBay.CargoType storageType;

	private Guid connectedConduitPortStatusItem;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (connectedPortStatus == null)
		{
			connectedPortStatus = new StatusItem("CONNECTED_ROCKET_PORT", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: true, OverlayModes.None.ID);
			connectedWrongPortStatus = new StatusItem("CONNECTED_ROCKET_WRONG_PORT", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: true, OverlayModes.None.ID);
			connectedNoPortStatus = new StatusItem("CONNECTED_ROCKET_NO_PORT", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.Bad, allow_multiples: true, OverlayModes.None.ID);
		}
		LaunchPad currentPad = GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad;
		if (currentPad != null)
		{
			OnLaunchpadChainChanged(null);
			GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad.Subscribe(-1009905786, OnLaunchpadChainChanged);
		}
		Subscribe(-1277991738, OnLaunchDelegate);
		Subscribe(-887025858, OnLandDelegate);
		storageType = GetComponent<CargoBay>().storageType;
		UpdateStatusItems();
	}

	protected override void OnCleanUp()
	{
		LaunchPad currentPad = GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad;
		if (currentPad != null)
		{
			currentPad.Unsubscribe(-1009905786, OnLaunchpadChainChanged);
		}
		base.OnCleanUp();
	}

	public void OnLaunch(object data)
	{
		ConduitDispenser component = GetComponent<ConduitDispenser>();
		if (component != null)
		{
			component.conduitType = ConduitType.None;
		}
		GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad.Unsubscribe(-1009905786, OnLaunchpadChainChanged);
	}

	public void OnLand(object data)
	{
		ConduitDispenser component = GetComponent<ConduitDispenser>();
		if (component != null)
		{
			switch (storageType)
			{
			case CargoBay.CargoType.Gasses:
				component.conduitType = ConduitType.Gas;
				break;
			case CargoBay.CargoType.Liquids:
				component.conduitType = ConduitType.Liquid;
				break;
			default:
				component.conduitType = ConduitType.None;
				break;
			}
		}
		GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad.Subscribe(-1009905786, OnLaunchpadChainChanged);
		UpdateStatusItems();
	}

	private void OnLaunchpadChainChanged(object data)
	{
		UpdateStatusItems();
	}

	private void UpdateStatusItems()
	{
		HasMatchingConduitPort(out var hasMatch, out var hasAny);
		KSelectable component = GetComponent<KSelectable>();
		if (hasMatch)
		{
			connectedConduitPortStatusItem = component.ReplaceStatusItem(connectedConduitPortStatusItem, connectedPortStatus, this);
		}
		else if (hasAny)
		{
			connectedConduitPortStatusItem = component.ReplaceStatusItem(connectedConduitPortStatusItem, connectedWrongPortStatus, this);
		}
		else
		{
			connectedConduitPortStatusItem = component.ReplaceStatusItem(connectedConduitPortStatusItem, connectedNoPortStatus, this);
		}
	}

	private void HasMatchingConduitPort(out bool hasMatch, out bool hasAny)
	{
		hasMatch = false;
		hasAny = false;
		LaunchPad currentPad = GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad;
		if (currentPad == null)
		{
			return;
		}
		ChainedBuilding.StatesInstance sMI = currentPad.GetSMI<ChainedBuilding.StatesInstance>();
		if (sMI == null)
		{
			return;
		}
		HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
		sMI.GetLinkedBuildings(ref chain);
		foreach (ChainedBuilding.StatesInstance item in chain)
		{
			IConduitDispenser component = item.GetComponent<IConduitDispenser>();
			if (component != null)
			{
				hasAny = true;
				if (ElementToCargoMap[component.ConduitType] == storageType)
				{
					hasMatch = true;
					break;
				}
			}
		}
		chain.Recycle();
	}
}
