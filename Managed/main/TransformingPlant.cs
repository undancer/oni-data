using System;
using Klei.AI;
using UnityEngine;

public class TransformingPlant : KMonoBehaviour
{
	public string transformPlantId;

	public Func<object, bool> eventDataCondition;

	public bool useGrowthTimeRatio = false;

	public bool keepPlantablePlotStorage = true;

	public string fxKAnim;

	public string fxAnim;

	private static readonly EventSystem.IntraObjectHandler<TransformingPlant> OnTransformationEventDelegate = new EventSystem.IntraObjectHandler<TransformingPlant>(delegate(TransformingPlant component, object data)
	{
		component.DoPlantTransform(data);
	});

	public void SubscribeToTransformEvent(GameHashes eventHash)
	{
		Subscribe((int)eventHash, OnTransformationEventDelegate);
	}

	public void UnsubscribeToTransformEvent(GameHashes eventHash)
	{
		Unsubscribe((int)eventHash, OnTransformationEventDelegate);
	}

	private void DoPlantTransform(object data)
	{
		if (eventDataCondition != null && !eventDataCondition(data))
		{
			return;
		}
		GameObject prefab = Assets.GetPrefab(transformPlantId.ToTag());
		GameObject gameObject = GameUtil.KInstantiate(prefab, Grid.SceneLayer.BuildingBack);
		gameObject.transform.SetPosition(base.transform.GetPosition());
		MutantPlant component = GetComponent<MutantPlant>();
		MutantPlant component2 = gameObject.GetComponent<MutantPlant>();
		if (component != null && gameObject != null)
		{
			gameObject.GetComponent<MutantPlant>().SetSubSpecies(GetComponent<MutantPlant>().subspeciesID);
		}
		gameObject.SetActive(value: true);
		Growing component3 = GetComponent<Growing>();
		Growing component4 = gameObject.GetComponent<Growing>();
		if (component3 != null && component4 != null)
		{
			float num = component3.PercentGrown();
			if (useGrowthTimeRatio)
			{
				float num2 = component3.growthTime / component4.growthTime;
				num = Mathf.Clamp01(num * num2);
			}
			component4.OverrideMaturityLevel(num);
		}
		PrimaryElement component5 = gameObject.GetComponent<PrimaryElement>();
		PrimaryElement component6 = GetComponent<PrimaryElement>();
		component5.Temperature = component6.Temperature;
		component5.AddDisease(component6.DiseaseIdx, component6.DiseaseCount, "TransformedPlant");
		gameObject.GetComponent<Effects>().CopyEffects(GetComponent<Effects>());
		gameObject.GetComponent<PlantRadiationMonitor>().totalRadiationExposure = GetComponent<PlantRadiationMonitor>().totalRadiationExposure;
		HarvestDesignatable component7 = GetComponent<HarvestDesignatable>();
		HarvestDesignatable component8 = gameObject.GetComponent<HarvestDesignatable>();
		if (component7 != null && component8 != null)
		{
			component8.SetHarvestWhenReady(component7.HarvestWhenReady);
		}
		PlantablePlot receptacle = GetComponent<ReceptacleMonitor>().GetReceptacle();
		if (receptacle != null)
		{
			receptacle.ReplacePlant(gameObject, keepPlantablePlotStorage);
		}
		Util.KDestroyGameObject(base.gameObject);
		if (fxKAnim != null)
		{
			KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect(fxKAnim, gameObject.transform.position, null, update_looping_sounds_position: false, Grid.SceneLayer.FXFront);
			kBatchedAnimController.Play(fxAnim);
			kBatchedAnimController.destroyOnAnimComplete = true;
		}
	}
}
