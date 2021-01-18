using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class PlanterSideScreen : ReceptacleSideScreen
{
	public DescriptorPanel RequirementsDescriptorPanel;

	public DescriptorPanel HarvestDescriptorPanel;

	public DescriptorPanel EffectsDescriptorPanel;

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<PlantablePlot>() != null;
	}

	protected override Sprite GetEntityIcon(Tag prefabTag)
	{
		PlantableSeed component = Assets.GetPrefab(prefabTag).GetComponent<PlantableSeed>();
		if (component != null)
		{
			return base.GetEntityIcon(new Tag(component.PlantID));
		}
		return base.GetEntityIcon(prefabTag);
	}

	protected override void SetResultDescriptions(GameObject seed_or_plant)
	{
		string text = "";
		GameObject gameObject = seed_or_plant;
		PlantableSeed component = seed_or_plant.GetComponent<PlantableSeed>();
		List<Descriptor> list = new List<Descriptor>();
		if (component != null)
		{
			list = component.GetDescriptors(component.gameObject);
			if (targetReceptacle.rotatable != null && targetReceptacle.Direction != component.direction)
			{
				if (component.direction == SingleEntityReceptacle.ReceptacleDirection.Top)
				{
					text += UI.UISIDESCREENS.PLANTERSIDESCREEN.ROTATION_NEED_FLOOR;
				}
				else if (component.direction == SingleEntityReceptacle.ReceptacleDirection.Side)
				{
					text += UI.UISIDESCREENS.PLANTERSIDESCREEN.ROTATION_NEED_WALL;
				}
				else if (component.direction == SingleEntityReceptacle.ReceptacleDirection.Bottom)
				{
					text += UI.UISIDESCREENS.PLANTERSIDESCREEN.ROTATION_NEED_CEILING;
				}
				text += "\n\n";
			}
			gameObject = Assets.GetPrefab(component.PlantID);
			if (!string.IsNullOrEmpty(component.domesticatedDescription))
			{
				text += component.domesticatedDescription;
			}
		}
		else
		{
			InfoDescription component2 = gameObject.GetComponent<InfoDescription>();
			if ((bool)component2)
			{
				text += component2.description;
			}
		}
		descriptionLabel.SetText(text);
		List<Descriptor> plantLifeCycleDescriptors = GameUtil.GetPlantLifeCycleDescriptors(gameObject);
		if (plantLifeCycleDescriptors.Count > 0)
		{
			HarvestDescriptorPanel.SetDescriptors(plantLifeCycleDescriptors);
			HarvestDescriptorPanel.gameObject.SetActive(value: true);
		}
		List<Descriptor> plantRequirementDescriptors = GameUtil.GetPlantRequirementDescriptors(gameObject);
		if (list.Count > 0)
		{
			GameUtil.IndentListOfDescriptors(list);
			plantRequirementDescriptors.InsertRange(plantRequirementDescriptors.Count, list);
		}
		if (plantRequirementDescriptors.Count > 0)
		{
			RequirementsDescriptorPanel.SetDescriptors(plantRequirementDescriptors);
			RequirementsDescriptorPanel.gameObject.SetActive(value: true);
		}
		List<Descriptor> plantEffectDescriptors = GameUtil.GetPlantEffectDescriptors(gameObject);
		if (plantEffectDescriptors.Count > 0)
		{
			EffectsDescriptorPanel.SetDescriptors(plantEffectDescriptors);
			EffectsDescriptorPanel.gameObject.SetActive(value: true);
		}
		else
		{
			EffectsDescriptorPanel.gameObject.SetActive(value: false);
		}
	}

	protected override bool AdditionalCanDepositTest()
	{
		return (targetReceptacle as PlantablePlot).ValidPlant;
	}
}
