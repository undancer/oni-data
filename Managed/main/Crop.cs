using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Crop")]
public class Crop : KMonoBehaviour, IGameObjectEffectDescriptor
{
	[Serializable]
	public struct CropVal
	{
		public string cropId;

		public float cropDuration;

		public int numProduced;

		public bool renewable;

		public CropVal(string crop_id, float crop_duration, int num_produced = 1, bool renewable = true)
		{
			cropId = crop_id;
			cropDuration = crop_duration;
			numProduced = num_produced;
			this.renewable = renewable;
		}
	}

	[MyCmpReq]
	private KSelectable selectable;

	public CropVal cropVal;

	private AttributeInstance yield;

	public string domesticatedDesc = "";

	private Storage planterStorage;

	private static readonly EventSystem.IntraObjectHandler<Crop> OnHarvestDelegate = new EventSystem.IntraObjectHandler<Crop>(delegate(Crop component, object data)
	{
		component.OnHarvest(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Crop> OnSeedDroppedDelegate = new EventSystem.IntraObjectHandler<Crop>(delegate(Crop component, object data)
	{
		component.OnSeedDropped(data);
	});

	public string cropId => cropVal.cropId;

	public Storage PlanterStorage
	{
		get
		{
			return planterStorage;
		}
		set
		{
			planterStorage = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Crops.Add(this);
		yield = this.GetAttributes().Add(Db.Get().PlantAttributes.YieldAmount);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(1272413801, OnHarvestDelegate);
		Subscribe(-1736624145, OnSeedDroppedDelegate);
	}

	public void Configure(CropVal cropval)
	{
		cropVal = cropval;
	}

	public bool CanGrow()
	{
		return cropVal.renewable;
	}

	public void SpawnConfiguredFruit(object callbackParam)
	{
		if (!(this == null))
		{
			CropVal cropVal = this.cropVal;
			if (!string.IsNullOrEmpty(cropVal.cropId))
			{
				SpawnSomeFruit(cropVal.cropId, yield.GetTotalValue());
				Trigger(-1072826864, this);
			}
		}
	}

	public void SpawnSomeFruit(Tag cropID, float amount)
	{
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(cropID), base.transform.GetPosition() + new Vector3(0f, 0.75f, 0f), Grid.SceneLayer.Ore);
		if (gameObject != null)
		{
			gameObject.SetActive(value: true);
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			component.Units = amount;
			component.Temperature = base.gameObject.GetComponent<PrimaryElement>().Temperature;
			Trigger(35625290, gameObject);
			Edible component2 = gameObject.GetComponent<Edible>();
			if ((bool)component2)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component2.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.HARVESTED, "{0}", component2.GetProperName()), UI.ENDOFDAYREPORT.NOTES.HARVESTED_CONTEXT);
			}
		}
		else
		{
			DebugUtil.LogErrorArgs(base.gameObject, "tried to spawn an invalid crop prefab:", cropID);
		}
	}

	protected override void OnCleanUp()
	{
		Components.Crops.Remove(this);
		base.OnCleanUp();
	}

	private void OnHarvest(object obj)
	{
	}

	public void OnSeedDropped(object data)
	{
	}

	public List<Descriptor> RequirementDescriptors(GameObject go)
	{
		return new List<Descriptor>();
	}

	public List<Descriptor> InformationDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Tag tag = new Tag(cropVal.cropId);
		GameObject prefab = Assets.GetPrefab(tag);
		Edible component = prefab.GetComponent<Edible>();
		Klei.AI.Attribute yieldAmount = Db.Get().PlantAttributes.YieldAmount;
		float preModifiedAttributeValue = go.GetComponent<Modifiers>().GetPreModifiedAttributeValue(yieldAmount);
		if (component != null)
		{
			DebugUtil.Assert(GameTags.DisplayAsCalories.Contains(tag), "Trying to display crop info for an edible fruit which isn't displayed as calories!", tag.ToString());
			float caloriesPerUnit = component.FoodInfo.CaloriesPerUnit;
			float calories = caloriesPerUnit * preModifiedAttributeValue;
			string formattedCalories = GameUtil.GetFormattedCalories(calories);
			Descriptor item = new Descriptor(string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD, prefab.GetProperName(), formattedCalories), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD, "", GameUtil.GetFormattedCalories(caloriesPerUnit), GameUtil.GetFormattedCalories(calories)));
			list.Add(item);
		}
		else
		{
			string formattedCalories = ((!GameTags.DisplayAsUnits.Contains(tag)) ? GameUtil.GetFormattedMass(cropVal.numProduced) : GameUtil.GetFormattedUnits(cropVal.numProduced, GameUtil.TimeSlice.None, displaySuffix: false));
			Descriptor item2 = new Descriptor(string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD_NONFOOD, prefab.GetProperName(), formattedCalories), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD_NONFOOD, formattedCalories));
			list.Add(item2);
		}
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor item in RequirementDescriptors(go))
		{
			list.Add(item);
		}
		foreach (Descriptor item2 in InformationDescriptors(go))
		{
			list.Add(item2);
		}
		return list;
	}
}
