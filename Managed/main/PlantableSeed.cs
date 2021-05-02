using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/PlantableSeed")]
public class PlantableSeed : KMonoBehaviour, IReceptacleDirection, IGameObjectEffectDescriptor
{
	public Tag PlantID;

	public Tag PreviewID;

	[Serialize]
	public float timeUntilSelfPlant = 0f;

	public Tag replantGroundTag;

	public string domesticatedDescription;

	public SingleEntityReceptacle.ReceptacleDirection direction = SingleEntityReceptacle.ReceptacleDirection.Top;

	private static readonly EventSystem.IntraObjectHandler<PlantableSeed> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<PlantableSeed>(delegate(PlantableSeed component, object data)
	{
		component.OnAbsorb(data);
	});

	private static readonly EventSystem.IntraObjectHandler<PlantableSeed> OnSplitFromChunkDelegate = new EventSystem.IntraObjectHandler<PlantableSeed>(delegate(PlantableSeed component, object data)
	{
		component.OnSplitFromChunk(data);
	});

	public SingleEntityReceptacle.ReceptacleDirection Direction => direction;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (GetComponent<MutantPlant>() != null)
		{
			Subscribe(-2064133523, OnAbsorbDelegate);
			Subscribe(1335436905, OnSplitFromChunkDelegate);
		}
		timeUntilSelfPlant = Util.RandomVariance(2400f, 600f);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.PlantableSeeds.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.PlantableSeeds.Remove(this);
		base.OnCleanUp();
	}

	private void OnAbsorb(object data)
	{
		Pickupable pickupable = data as Pickupable;
		Debug.Assert(GetComponent<MutantPlant>().SubSpeciesID == pickupable.GetComponent<MutantPlant>().SubSpeciesID, "Two seeds of different subspecies just absorbed!");
	}

	private void OnSplitFromChunk(object data)
	{
		Pickupable pickupable = data as Pickupable;
		pickupable.GetComponent<MutantPlant>().CopyMutationsTo(GetComponent<MutantPlant>());
	}

	public void TryPlant(bool allow_plant_from_storage = false)
	{
		timeUntilSelfPlant = Util.RandomVariance(2400f, 600f);
		if (!allow_plant_from_storage && base.gameObject.HasTag(GameTags.Stored))
		{
			return;
		}
		int cell = Grid.PosToCell(base.gameObject);
		if (!TestSuitableGround(cell))
		{
			return;
		}
		Vector3 position = Grid.CellToPosCBC(cell, Grid.SceneLayer.BuildingFront);
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(PlantID), position, Grid.SceneLayer.BuildingFront);
		MutantPlant component = gameObject.GetComponent<MutantPlant>();
		if (component != null)
		{
			GetComponent<MutantPlant>().CopyMutationsTo(component);
		}
		gameObject.SetActive(value: true);
		Pickupable component2 = GetComponent<Pickupable>();
		Pickupable pickupable = component2.Take(1f);
		if (pickupable != null)
		{
			Crop component3 = gameObject.GetComponent<Crop>();
			if (component3 != null)
			{
			}
			Util.KDestroyGameObject(pickupable.gameObject);
		}
		else
		{
			KCrashReporter.Assert(condition: false, "Seed has fractional total amount < 1f");
		}
	}

	public bool TestSuitableGround(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		int num = ((Direction != SingleEntityReceptacle.ReceptacleDirection.Bottom) ? Grid.CellBelow(cell) : Grid.CellAbove(cell));
		if (!Grid.IsValidCell(num))
		{
			return false;
		}
		if (Grid.Foundation[num])
		{
			return false;
		}
		if (Grid.Element[num].hardness >= 150)
		{
			return false;
		}
		if (replantGroundTag.IsValid && !Grid.Element[num].HasTag(replantGroundTag))
		{
			return false;
		}
		GameObject prefab = Assets.GetPrefab(PlantID);
		EntombVulnerable component = prefab.GetComponent<EntombVulnerable>();
		if (component != null && !component.IsCellSafe(cell))
		{
			return false;
		}
		DrowningMonitor component2 = prefab.GetComponent<DrowningMonitor>();
		if (component2 != null && !component2.IsCellSafe(cell))
		{
			return false;
		}
		TemperatureVulnerable component3 = prefab.GetComponent<TemperatureVulnerable>();
		if (component3 != null && !component3.IsCellSafe(cell))
		{
			return false;
		}
		UprootedMonitor component4 = prefab.GetComponent<UprootedMonitor>();
		if (component4 != null && !component4.IsSuitableFoundation(cell))
		{
			return false;
		}
		OccupyArea component5 = prefab.GetComponent<OccupyArea>();
		if (component5 != null && !component5.CanOccupyArea(cell, ObjectLayer.Building))
		{
			return false;
		}
		return true;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (direction == SingleEntityReceptacle.ReceptacleDirection.Bottom)
		{
			Descriptor item = new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_REQUIREMENT_CEILING, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_REQUIREMENT_CEILING, Descriptor.DescriptorType.Requirement);
			list.Add(item);
		}
		else if (direction == SingleEntityReceptacle.ReceptacleDirection.Side)
		{
			Descriptor item2 = new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_REQUIREMENT_WALL, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_REQUIREMENT_WALL, Descriptor.DescriptorType.Requirement);
			list.Add(item2);
		}
		return list;
	}
}
