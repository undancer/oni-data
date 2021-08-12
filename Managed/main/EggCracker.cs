using System;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EggCracker")]
public class EggCracker : KMonoBehaviour
{
	[MyCmpReq]
	private ComplexFabricator refinery;

	[MyCmpReq]
	private ComplexFabricatorWorkable workable;

	private KBatchedAnimTracker tracker;

	private GameObject display_egg;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		refinery.choreType = Db.Get().ChoreTypes.Cook;
		refinery.fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;
		workable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
		workable.AttributeConverter = Db.Get().AttributeConverters.CookingSpeed;
		workable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		workable.SkillExperienceSkillGroup = Db.Get().SkillGroups.Cooking.Id;
		workable.SkillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		ComplexFabricatorWorkable complexFabricatorWorkable = workable;
		complexFabricatorWorkable.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(complexFabricatorWorkable.OnWorkableEventCB, new Action<Workable.WorkableEvent>(OnWorkableEvent));
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		UnityEngine.Object.Destroy(tracker);
		tracker = null;
	}

	private void OnWorkableEvent(Workable.WorkableEvent e)
	{
		switch (e)
		{
		case Workable.WorkableEvent.WorkStarted:
		{
			ComplexRecipe currentWorkingOrder = refinery.CurrentWorkingOrder;
			if (currentWorkingOrder != null)
			{
				ComplexRecipe.RecipeElement[] ingredients = currentWorkingOrder.ingredients;
				if (ingredients.Length != 0)
				{
					ComplexRecipe.RecipeElement recipeElement = ingredients[0];
					display_egg = refinery.buildStorage.FindFirst(recipeElement.material);
					PositionActiveEgg();
				}
			}
			break;
		}
		case Workable.WorkableEvent.WorkCompleted:
			if ((bool)display_egg)
			{
				display_egg.GetComponent<KBatchedAnimController>().Play("hatching_pst");
			}
			break;
		case Workable.WorkableEvent.WorkStopped:
			UnityEngine.Object.Destroy(tracker);
			tracker = null;
			display_egg = null;
			break;
		}
	}

	private void PositionActiveEgg()
	{
		if ((bool)display_egg)
		{
			KBatchedAnimController component = display_egg.GetComponent<KBatchedAnimController>();
			component.enabled = true;
			component.SetSceneLayer(Grid.SceneLayer.BuildingUse);
			KSelectable component2 = display_egg.GetComponent<KSelectable>();
			if (component2 != null)
			{
				component2.enabled = true;
			}
			tracker = display_egg.AddComponent<KBatchedAnimTracker>();
			tracker.symbol = "snapto_egg";
		}
	}
}
