using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ResearchModule")]
public class ResearchModule : KMonoBehaviour
{
	private static readonly EventSystem.IntraObjectHandler<ResearchModule> OnLaunchDelegate = new EventSystem.IntraObjectHandler<ResearchModule>(delegate(ResearchModule component, object data)
	{
		component.OnLaunch(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ResearchModule> OnLandDelegate = new EventSystem.IntraObjectHandler<ResearchModule>(delegate(ResearchModule component, object data)
	{
		component.OnLand(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop);
		Subscribe(-1277991738, OnLaunchDelegate);
		Subscribe(-887025858, OnLandDelegate);
	}

	public void OnLaunch(object data)
	{
	}

	public void OnLand(object data)
	{
		if (!DlcManager.IsExpansion1Active())
		{
			SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(SpacecraftManager.instance.GetSpacecraftID(GetComponent<RocketModule>().conditionManager.GetComponent<LaunchableRocket>()));
			SpaceDestination.ResearchOpportunity researchOpportunity = spacecraftDestination.TryCompleteResearchOpportunity();
			if (researchOpportunity != null)
			{
				GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab("ResearchDatabank"), base.gameObject.transform.GetPosition(), Grid.SceneLayer.Ore);
				gameObject.SetActive(value: true);
				gameObject.GetComponent<PrimaryElement>().Mass = researchOpportunity.dataValue;
				if (!string.IsNullOrEmpty(researchOpportunity.discoveredRareItem))
				{
					GameObject prefab = Assets.GetPrefab(researchOpportunity.discoveredRareItem);
					if (prefab == null)
					{
						KCrashReporter.Assert(condition: false, "Missing prefab: " + researchOpportunity.discoveredRareItem);
					}
					else
					{
						GameObject gameObject2 = GameUtil.KInstantiate(prefab, base.gameObject.transform.GetPosition(), Grid.SceneLayer.Ore);
						gameObject2.SetActive(value: true);
					}
				}
			}
		}
		GameObject gameObject3 = GameUtil.KInstantiate(Assets.GetPrefab("ResearchDatabank"), base.gameObject.transform.GetPosition(), Grid.SceneLayer.Ore);
		gameObject3.SetActive(value: true);
		gameObject3.GetComponent<PrimaryElement>().Mass = ROCKETRY.DESTINATION_RESEARCH.EVERGREEN;
	}
}
