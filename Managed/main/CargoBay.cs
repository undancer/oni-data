using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CargoBay")]
public class CargoBay : KMonoBehaviour
{
	public enum CargoType
	{
		Solids,
		Liquids,
		Gasses,
		Entities
	}

	public Storage storage;

	private MeterController meter;

	public CargoType storageType = CargoType.Solids;

	public static Dictionary<ConduitType, CargoType> ElementToCargoMap = new Dictionary<ConduitType, CargoType>
	{
		{
			ConduitType.Solid,
			CargoType.Solids
		},
		{
			ConduitType.Liquid,
			CargoType.Liquids
		},
		{
			ConduitType.Gas,
			CargoType.Gasses
		}
	};

	private static readonly EventSystem.IntraObjectHandler<CargoBay> OnLaunchDelegate = new EventSystem.IntraObjectHandler<CargoBay>(delegate(CargoBay component, object data)
	{
		component.OnLaunch(data);
	});

	private static readonly EventSystem.IntraObjectHandler<CargoBay> OnLandDelegate = new EventSystem.IntraObjectHandler<CargoBay>(delegate(CargoBay component, object data)
	{
		component.OnLand(data);
	});

	private static readonly EventSystem.IntraObjectHandler<CargoBay> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<CargoBay>(delegate(CargoBay component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<CargoBay> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<CargoBay>(delegate(CargoBay component, object data)
	{
		component.OnStorageChange(data);
	});

	private static StatusItem connectedPortStatus;

	private static StatusItem connectedWrongPortStatus;

	private static StatusItem connectedNoPortStatus;

	private Guid connectedConduitPortStatusItem;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (connectedPortStatus == null)
		{
			connectedPortStatus = new StatusItem("CONNECTED_ROCKET_PORT", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: true, OverlayModes.None.ID);
			connectedWrongPortStatus = new StatusItem("CONNECTED_ROCKET_WRONG_PORT", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: true, OverlayModes.None.ID);
			connectedNoPortStatus = new StatusItem("CONNECTED_ROCKET_NO_PORT", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.Bad, allow_multiples: true, OverlayModes.None.ID);
		}
		GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop);
		LaunchPad currentPad = GetComponent<RocketModule>().CraftInterface.CurrentPad;
		if (currentPad != null)
		{
			OnLaunchpadChainChanged(null);
			GetComponent<RocketModule>().CraftInterface.CurrentPad.Subscribe(-1009905786, OnLaunchpadChainChanged);
		}
		Subscribe(-1277991738, OnLaunchDelegate);
		Subscribe(-887025858, OnLandDelegate);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
		meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
		OnStorageChange(null);
		Subscribe(-1697596308, OnStorageChangeDelegate);
		UpdateStatusItems();
	}

	private void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo button = new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.EMPTYSTORAGE.NAME, delegate
		{
			storage.DropAll();
		}, Action.NumActions, null, null, null, UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP);
		Game.Instance.userMenu.AddButton(base.gameObject, button);
	}

	private void OnStorageChange(object data)
	{
		meter.SetPositionPercent(storage.MassStored() / storage.Capacity());
	}

	protected override void OnCleanUp()
	{
		LaunchPad currentPad = GetComponent<RocketModule>().CraftInterface.CurrentPad;
		if (currentPad != null)
		{
			currentPad.Unsubscribe(-1009905786, OnLaunchpadChainChanged);
		}
		base.OnCleanUp();
	}

	public void SpawnResources(object data)
	{
		if (DlcManager.IsExpansion1Active())
		{
			return;
		}
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(SpacecraftManager.instance.GetSpacecraftID(GetComponent<RocketModule>().conditionManager.GetComponent<LaunchableRocket>()));
		int rootCell = Grid.PosToCell(base.gameObject);
		foreach (KeyValuePair<SimHashes, float> item in spacecraftDestination.GetMissionResourceResult(storage.RemainingCapacity(), storageType == CargoType.Solids, storageType == CargoType.Liquids, storageType == CargoType.Gasses))
		{
			Element element = ElementLoader.FindElementByHash(item.Key);
			if (storageType == CargoType.Solids && element.IsSolid)
			{
				GameObject gameObject = Scenario.SpawnPrefab(rootCell, 0, 0, element.tag.Name);
				gameObject.GetComponent<PrimaryElement>().Mass = item.Value;
				gameObject.GetComponent<PrimaryElement>().Temperature = ElementLoader.FindElementByHash(item.Key).defaultValues.temperature;
				gameObject.SetActive(value: true);
				storage.Store(gameObject);
			}
			else if (storageType == CargoType.Liquids && element.IsLiquid)
			{
				storage.AddLiquid(item.Key, item.Value, ElementLoader.FindElementByHash(item.Key).defaultValues.temperature, byte.MaxValue, 0);
			}
			else if (storageType == CargoType.Gasses && element.IsGas)
			{
				storage.AddGasChunk(item.Key, item.Value, ElementLoader.FindElementByHash(item.Key).defaultValues.temperature, byte.MaxValue, 0, keep_zero_mass: false);
			}
		}
		if (storageType != CargoType.Entities)
		{
			return;
		}
		foreach (KeyValuePair<Tag, int> item2 in spacecraftDestination.GetMissionEntityResult())
		{
			GameObject prefab = Assets.GetPrefab(item2.Key);
			if (prefab == null)
			{
				KCrashReporter.Assert(condition: false, "Missing prefab: " + item2.Key.Name);
				continue;
			}
			for (int i = 0; i < item2.Value; i++)
			{
				GameObject gameObject2 = Util.KInstantiate(prefab, base.transform.position);
				gameObject2.SetActive(value: true);
				storage.Store(gameObject2);
				Baggable component = gameObject2.GetComponent<Baggable>();
				if (component != null)
				{
					component.SetWrangled();
				}
			}
		}
	}

	public void OnLaunch(object data)
	{
		if (!DlcManager.IsExpansion1Active())
		{
			ReserveResources();
		}
		ConduitDispenser component = GetComponent<ConduitDispenser>();
		if (component != null)
		{
			component.conduitType = ConduitType.None;
		}
		GetComponent<RocketModule>().CraftInterface.CurrentPad.Unsubscribe(-1009905786, OnLaunchpadChainChanged);
	}

	private void ReserveResources()
	{
		int spacecraftID = SpacecraftManager.instance.GetSpacecraftID(GetComponent<RocketModule>().conditionManager.GetComponent<LaunchableRocket>());
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(spacecraftID);
		spacecraftDestination.UpdateRemainingResources(this);
	}

	public void OnLand(object data)
	{
		SpawnResources(data);
		ConduitDispenser component = GetComponent<ConduitDispenser>();
		if (component != null)
		{
			switch (storageType)
			{
			case CargoType.Gasses:
				component.conduitType = ConduitType.Gas;
				break;
			case CargoType.Liquids:
				component.conduitType = ConduitType.Liquid;
				break;
			default:
				component.conduitType = ConduitType.None;
				break;
			}
		}
		GetComponent<RocketModule>().CraftInterface.CurrentPad.Subscribe(-1009905786, OnLaunchpadChainChanged);
		UpdateStatusItems();
	}

	private void OnLaunchpadChainChanged(object data)
	{
		UpdateStatusItems();
	}

	private void UpdateStatusItems()
	{
		bool flag = false;
		bool flag2 = false;
		IEnumerable<GameObject> connectedConduitPorts = GetConnectedConduitPorts();
		if (connectedConduitPorts != null)
		{
			foreach (GameObject item in connectedConduitPorts)
			{
				IConduitDispenser component = item.GetComponent<IConduitDispenser>();
				if (component != null)
				{
					flag = true;
					if (ElementToCargoMap[component.ConduitType] == storageType)
					{
						flag2 = true;
						break;
					}
				}
			}
		}
		KSelectable component2 = GetComponent<KSelectable>();
		if (flag2)
		{
			connectedConduitPortStatusItem = component2.ReplaceStatusItem(connectedConduitPortStatusItem, connectedPortStatus, this);
		}
		else if (flag)
		{
			connectedConduitPortStatusItem = component2.ReplaceStatusItem(connectedConduitPortStatusItem, connectedWrongPortStatus, this);
		}
		else
		{
			connectedConduitPortStatusItem = component2.ReplaceStatusItem(connectedConduitPortStatusItem, connectedNoPortStatus, this);
		}
	}

	private IEnumerable<GameObject> GetConnectedConduitPorts()
	{
		LaunchPad currentPad = GetComponent<RocketModule>().CraftInterface.CurrentPad;
		if (currentPad == null)
		{
			return null;
		}
		return currentPad.GetSMI<ChainedBuilding.StatesInstance>()?.GetLinkedBuildings();
	}
}
