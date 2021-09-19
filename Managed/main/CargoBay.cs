using System.Collections.Generic;
using KSerialization;
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

	[Serialize]
	public float reservedResources;

	public CargoType storageType;

	public static Dictionary<Element.State, CargoType> ElementStateToCargoTypes = new Dictionary<Element.State, CargoType>
	{
		{
			Element.State.Gas,
			CargoType.Gasses
		},
		{
			Element.State.Liquid,
			CargoType.Liquids
		},
		{
			Element.State.Solid,
			CargoType.Solids
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

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop);
		Subscribe(-1277991738, OnLaunchDelegate);
		Subscribe(-887025858, OnLandDelegate);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
		meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
		OnStorageChange(null);
		Subscribe(-1697596308, OnStorageChangeDelegate);
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

	public void SpawnResources(object data)
	{
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			return;
		}
		ILaunchableRocket component = GetComponent<RocketModule>().conditionManager.GetComponent<ILaunchableRocket>();
		if (component.registerType == LaunchableRocketRegisterType.Clustercraft)
		{
			return;
		}
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(SpacecraftManager.instance.GetSpacecraftID(component));
		int rootCell = Grid.PosToCell(base.gameObject);
		foreach (KeyValuePair<SimHashes, float> item in spacecraftDestination.GetMissionResourceResult(storage.RemainingCapacity(), reservedResources, storageType == CargoType.Solids, storageType == CargoType.Liquids, storageType == CargoType.Gasses))
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
				Baggable component2 = gameObject2.GetComponent<Baggable>();
				if (component2 != null)
				{
					component2.SetWrangled();
				}
			}
		}
	}

	public void OnLaunch(object data)
	{
		ReserveResources();
		ConduitDispenser component = GetComponent<ConduitDispenser>();
		if (component != null)
		{
			component.conduitType = ConduitType.None;
		}
	}

	private void ReserveResources()
	{
		if (!DlcManager.FeatureClusterSpaceEnabled())
		{
			ILaunchableRocket component = GetComponent<RocketModule>().conditionManager.GetComponent<ILaunchableRocket>();
			if (component.registerType != LaunchableRocketRegisterType.Clustercraft)
			{
				int spacecraftID = SpacecraftManager.instance.GetSpacecraftID(component);
				SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(spacecraftID);
				reservedResources = spacecraftDestination.ReserveResources(this);
			}
		}
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
	}
}
