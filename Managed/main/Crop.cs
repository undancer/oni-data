using System;
using System.Collections.Generic;
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

	public void SpawnFruit(object callbackParam)
	{
		if (this == null)
		{
			return;
		}
		CropVal cropVal = this.cropVal;
		if (string.IsNullOrEmpty(cropVal.cropId))
		{
			return;
		}
		GameObject gameObject = Scenario.SpawnPrefab(Grid.PosToCell(base.gameObject), 0, 0, cropVal.cropId);
		if (gameObject != null)
		{
			float y = 0.75f;
			gameObject.transform.SetPosition(gameObject.transform.GetPosition() + new Vector3(0f, y, 0f));
			gameObject.SetActive(value: true);
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			component.Units = cropVal.numProduced;
			component.Temperature = base.gameObject.GetComponent<PrimaryElement>().Temperature;
			Edible component2 = gameObject.GetComponent<Edible>();
			if ((bool)component2)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component2.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.HARVESTED, "{0}", component2.GetProperName()), UI.ENDOFDAYREPORT.NOTES.HARVESTED_CONTEXT);
			}
		}
		else
		{
			DebugUtil.LogErrorArgs(base.gameObject, "tried to spawn an invalid crop prefab:", cropVal.cropId);
		}
		Trigger(-1072826864);
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
		float num = 0f;
		string arg = "";
		if (component != null)
		{
			num = component.FoodInfo.CaloriesPerUnit;
		}
		float calories = num * (float)cropVal.numProduced;
		InfoDescription component2 = prefab.GetComponent<InfoDescription>();
		if ((bool)component2)
		{
			arg = component2.description;
		}
		string arg2 = (GameTags.DisplayAsCalories.Contains(tag) ? GameUtil.GetFormattedCalories(calories) : ((!GameTags.DisplayAsUnits.Contains(tag)) ? GameUtil.GetFormattedMass(cropVal.numProduced) : GameUtil.GetFormattedUnits(cropVal.numProduced, GameUtil.TimeSlice.None, displaySuffix: false)));
		LocString yIELD = UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD;
		Descriptor item = new Descriptor(string.Format(yIELD, prefab.GetProperName(), arg2), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD, arg, GameUtil.GetFormattedCalories(num), GameUtil.GetFormattedCalories(calories)));
		list.Add(item);
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
