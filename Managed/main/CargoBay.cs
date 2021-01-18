using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CargoBay")]
public class CargoBay : KMonoBehaviour
{
	public enum CargoType
	{
		solids,
		liquids,
		gasses,
		entities
	}

	public Storage storage;

	private MeterController meter;

	public CargoType storageType;

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

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop);
		Subscribe(-1056989049, OnLaunchDelegate);
		Subscribe(238242047, OnLandDelegate);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
		meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
		Subscribe(-1697596308, delegate
		{
			meter.SetPositionPercent(storage.MassStored() / storage.Capacity());
		});
	}

	private void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo button = new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.EMPTYSTORAGE.NAME, delegate
		{
			storage.DropAll();
		}, Action.NumActions, null, null, null, UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP);
		Game.Instance.userMenu.AddButton(base.gameObject, button);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public void SpawnResources(object data)
	{
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(SpacecraftManager.instance.GetSpacecraftID(GetComponent<RocketModule>().conditionManager.GetComponent<LaunchableRocket>()));
		int rootCell = Grid.PosToCell(base.gameObject);
		foreach (KeyValuePair<SimHashes, float> item in spacecraftDestination.GetMissionResourceResult(storage.RemainingCapacity(), storageType == CargoType.solids, storageType == CargoType.liquids, storageType == CargoType.gasses))
		{
			Element element = ElementLoader.FindElementByHash(item.Key);
			if (storageType == CargoType.solids && element.IsSolid)
			{
				GameObject gameObject = Scenario.SpawnPrefab(rootCell, 0, 0, element.tag.Name);
				gameObject.GetComponent<PrimaryElement>().Mass = item.Value;
				gameObject.GetComponent<PrimaryElement>().Temperature = ElementLoader.FindElementByHash(item.Key).defaultValues.temperature;
				gameObject.SetActive(value: true);
				storage.Store(gameObject);
			}
			else if (storageType == CargoType.liquids && element.IsLiquid)
			{
				storage.AddLiquid(item.Key, item.Value, ElementLoader.FindElementByHash(item.Key).defaultValues.temperature, byte.MaxValue, 0);
			}
			else if (storageType == CargoType.gasses && element.IsGas)
			{
				storage.AddGasChunk(item.Key, item.Value, ElementLoader.FindElementByHash(item.Key).defaultValues.temperature, byte.MaxValue, 0, keep_zero_mass: false);
			}
		}
		if (storageType != CargoType.entities)
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
		ReserveResources();
		ConduitDispenser component = GetComponent<ConduitDispenser>();
		if (component != null)
		{
			component.conduitType = ConduitType.None;
		}
	}

	private void ReserveResources()
	{
		int spacecraftID = SpacecraftManager.instance.GetSpacecraftID(GetComponent<RocketModule>().conditionManager.GetComponent<LaunchableRocket>());
		SpacecraftManager.instance.GetSpacecraftDestination(spacecraftID).UpdateRemainingResources(this);
	}

	public void OnLand(object data)
	{
		SpawnResources(data);
		ConduitDispenser component = GetComponent<ConduitDispenser>();
		if (component != null)
		{
			switch (storageType)
			{
			case CargoType.gasses:
				component.conduitType = ConduitType.Gas;
				break;
			case CargoType.liquids:
				component.conduitType = ConduitType.Liquid;
				break;
			default:
				component.conduitType = ConduitType.None;
				break;
			}
		}
	}
}
