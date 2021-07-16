using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SeedProducer")]
public class SeedProducer : KMonoBehaviour, IGameObjectEffectDescriptor
{
	[Serializable]
	public struct SeedInfo
	{
		public string seedId;

		public ProductionType productionType;

		public int newSeedsProduced;
	}

	public enum ProductionType
	{
		Hidden,
		DigOnly,
		Harvest,
		Fruit,
		Sterile
	}

	public SeedInfo seedInfo;

	private bool droppedSeedAlready;

	private static readonly EventSystem.IntraObjectHandler<SeedProducer> DropSeedDelegate = new EventSystem.IntraObjectHandler<SeedProducer>(delegate(SeedProducer component, object data)
	{
		component.DropSeed(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SeedProducer> CropPickedDelegate = new EventSystem.IntraObjectHandler<SeedProducer>(delegate(SeedProducer component, object data)
	{
		component.CropPicked(data);
	});

	public void Configure(string SeedID, ProductionType productionType, int newSeedsProduced = 1)
	{
		seedInfo.seedId = SeedID;
		seedInfo.productionType = productionType;
		seedInfo.newSeedsProduced = newSeedsProduced;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-216549700, DropSeedDelegate);
		Subscribe(1623392196, DropSeedDelegate);
		Subscribe(-1072826864, CropPickedDelegate);
	}

	private GameObject ProduceSeed(string seedId, int units = 1, bool canMutate = true)
	{
		if (seedId != null && units > 0)
		{
			Vector3 position = base.gameObject.transform.GetPosition() + new Vector3(0f, 0.5f, 0f);
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(new Tag(seedId)), position, Grid.SceneLayer.Ore);
			MutantPlant component = GetComponent<MutantPlant>();
			if (component != null)
			{
				MutantPlant component2 = gameObject.GetComponent<MutantPlant>();
				bool flag = false;
				if (canMutate && component2 != null && component2.IsOriginal)
				{
					flag = RollForMutation();
				}
				if (flag)
				{
					component2.Mutate();
				}
				else
				{
					component.CopyMutationsTo(component2);
				}
			}
			PrimaryElement component3 = base.gameObject.GetComponent<PrimaryElement>();
			PrimaryElement component4 = gameObject.GetComponent<PrimaryElement>();
			component4.Temperature = component3.Temperature;
			component4.Units = units;
			Trigger(472291861, gameObject.GetComponent<PlantableSeed>());
			gameObject.SetActive(value: true);
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, gameObject.GetProperName(), gameObject.transform);
			return gameObject;
		}
		return null;
	}

	public void DropSeed(object data = null)
	{
		if (!droppedSeedAlready)
		{
			GameObject gameObject = ProduceSeed(seedInfo.seedId, 1, canMutate: false);
			Trigger(-1736624145, gameObject.GetComponent<PlantableSeed>());
			droppedSeedAlready = true;
		}
	}

	public void CropDepleted(object data)
	{
		DropSeed();
	}

	public void CropPicked(object data)
	{
		if (seedInfo.productionType == ProductionType.Harvest)
		{
			Worker completed_by = GetComponent<Harvestable>().completed_by;
			float num = 0.1f;
			if (completed_by != null)
			{
				num += completed_by.GetComponent<AttributeConverters>().Get(Db.Get().AttributeConverters.SeedHarvestChance).Evaluate();
			}
			int units = ((UnityEngine.Random.Range(0f, 1f) <= num) ? 1 : 0);
			ProduceSeed(seedInfo.seedId, units);
		}
	}

	public bool RollForMutation()
	{
		AttributeInstance attributeInstance = Db.Get().PlantAttributes.MaxRadiationThreshold.Lookup(this);
		int num = Grid.PosToCell(base.gameObject);
		float num2 = Mathf.Clamp(Grid.IsValidCell(num) ? Grid.Radiation[num] : 0f, 0f, attributeInstance.GetTotalValue()) / attributeInstance.GetTotalValue() * 0.8f;
		return UnityEngine.Random.value < num2;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		_ = Assets.GetPrefab(new Tag(seedInfo.seedId)) != null;
		switch (seedInfo.productionType)
		{
		default:
			return null;
		case ProductionType.DigOnly:
			return null;
		case ProductionType.Harvest:
			list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_PRODUCTION_HARVEST, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_PRODUCTION_HARVEST, Descriptor.DescriptorType.Lifecycle, only_for_simple_info_screen: true));
			list.Add(new Descriptor(string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.BONUS_SEEDS, GameUtil.GetFormattedPercent(10f)), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.BONUS_SEEDS, GameUtil.GetFormattedPercent(10f))));
			break;
		case ProductionType.Fruit:
			list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_PRODUCTION_FRUIT, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_PRODUCTION_DIG_ONLY, Descriptor.DescriptorType.Lifecycle, only_for_simple_info_screen: true));
			break;
		case ProductionType.Sterile:
			list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.MUTANT_STERILE, UI.GAMEOBJECTEFFECTS.TOOLTIPS.MUTANT_STERILE));
			break;
		}
		return list;
	}
}
