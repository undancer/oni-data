using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MicrobeMusher : ComplexFabricator
{
	[SerializeField]
	public Vector3 mushbarSpawnOffset = Vector3.right;

	private static readonly KAnimHashedString meterRationHash = new KAnimHashedString("meter_ration");

	private static readonly KAnimHashedString canHash = new KAnimHashedString("can");

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		choreType = Db.Get().ChoreTypes.Cook;
		fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("WaterFetchingTutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater);
		});
		workable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Mushing;
		workable.AttributeConverter = Db.Get().AttributeConverters.CookingSpeed;
		workable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		workable.SkillExperienceSkillGroup = Db.Get().SkillGroups.Cooking.Id;
		workable.SkillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		workable.meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, "meter_target", "meter_ration");
		workable.meter.meterController.SetSymbolVisiblity(canHash, is_visible: false);
		workable.meter.meterController.SetSymbolVisiblity(meterRationHash, is_visible: false);
	}

	protected override List<GameObject> SpawnOrderProduct(ComplexRecipe recipe)
	{
		List<GameObject> list = base.SpawnOrderProduct(recipe);
		foreach (GameObject item in list)
		{
			PrimaryElement component = item.GetComponent<PrimaryElement>();
			if (component != null)
			{
				if (item.PrefabID() == "MushBar")
				{
					byte index = Db.Get().Diseases.GetIndex("FoodPoisoning");
					component.AddDisease(index, 1000, "Made of mud");
				}
				if (item.GetComponent<PrimaryElement>().DiseaseCount > 0)
				{
					Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_DiseaseCooking);
				}
			}
		}
		return list;
	}
}
