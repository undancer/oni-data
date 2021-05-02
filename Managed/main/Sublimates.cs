using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Sublimates")]
public class Sublimates : KMonoBehaviour, ISim200ms
{
	[Serializable]
	public struct Info
	{
		public float sublimationRate;

		public float minSublimationAmount;

		public float maxDestinationMass;

		public float massPower;

		public byte diseaseIdx;

		public int diseaseCount;

		[HashedEnum]
		public SimHashes sublimatedElement;

		public Info(float rate, float min_amount, float max_destination_mass, float mass_power, SimHashes element, byte disease_idx = byte.MaxValue, int disease_count = 0)
		{
			sublimationRate = rate;
			minSublimationAmount = min_amount;
			maxDestinationMass = max_destination_mass;
			massPower = mass_power;
			sublimatedElement = element;
			diseaseIdx = disease_idx;
			diseaseCount = disease_count;
		}
	}

	private enum EmitState
	{
		Emitting,
		BlockedOnPressure,
		BlockedOnTemperature
	}

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpReq]
	private KSelectable selectable;

	[SerializeField]
	public SpawnFXHashes spawnFXHash = SpawnFXHashes.None;

	[SerializeField]
	public Info info;

	[Serialize]
	private float sublimatedMass;

	private HandleVector<int>.Handle flowAccumulator = HandleVector<int>.InvalidHandle;

	private EmitState lastEmitState = (EmitState)(-1);

	private static readonly EventSystem.IntraObjectHandler<Sublimates> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<Sublimates>(delegate(Sublimates component, object data)
	{
		component.OnAbsorb(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Sublimates> OnSplitFromChunkDelegate = new EventSystem.IntraObjectHandler<Sublimates>(delegate(Sublimates component, object data)
	{
		component.OnSplitFromChunk(data);
	});

	public float Temperature => primaryElement.Temperature;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-2064133523, OnAbsorbDelegate);
		Subscribe(1335436905, OnSplitFromChunkDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		flowAccumulator = Game.Instance.accumulators.Add("EmittedMass", this);
		RefreshStatusItem(EmitState.Emitting);
	}

	protected override void OnCleanUp()
	{
		flowAccumulator = Game.Instance.accumulators.Remove(flowAccumulator);
		base.OnCleanUp();
	}

	private void OnAbsorb(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if (pickupable != null)
		{
			Sublimates component = pickupable.GetComponent<Sublimates>();
			if (component != null)
			{
				sublimatedMass += component.sublimatedMass;
			}
		}
	}

	private void OnSplitFromChunk(object data)
	{
		Pickupable pickupable = data as Pickupable;
		PrimaryElement component = pickupable.GetComponent<PrimaryElement>();
		Sublimates component2 = pickupable.GetComponent<Sublimates>();
		if (!(component2 == null))
		{
			float mass = primaryElement.Mass;
			float mass2 = component.Mass;
			float num = mass / (mass2 + mass);
			sublimatedMass = component2.sublimatedMass * num;
			float num2 = 1f - num;
			component2.sublimatedMass *= num2;
		}
	}

	public void Sim200ms(float dt)
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		if (!Grid.IsValidCell(num) || this.HasTag(GameTags.Sealed))
		{
			return;
		}
		Element element = ElementLoader.FindElementByHash(info.sublimatedElement);
		if (primaryElement.Temperature <= element.lowTemp)
		{
			RefreshStatusItem(EmitState.BlockedOnTemperature);
			return;
		}
		float num2 = Grid.Mass[num];
		if (num2 < info.maxDestinationMass)
		{
			float mass = primaryElement.Mass;
			if (mass > 0f)
			{
				float num3 = Mathf.Pow(mass, info.massPower);
				float num4 = Mathf.Max(info.sublimationRate, info.sublimationRate * num3);
				num4 *= dt;
				num4 = Mathf.Min(num4, mass);
				sublimatedMass += num4;
				mass -= num4;
				if (sublimatedMass > info.minSublimationAmount)
				{
					float num5 = sublimatedMass / primaryElement.Mass;
					byte b = byte.MaxValue;
					int num6 = 0;
					if (info.diseaseIdx == byte.MaxValue)
					{
						b = primaryElement.DiseaseIdx;
						num6 = (int)((float)primaryElement.DiseaseCount * num5);
						primaryElement.ModifyDiseaseCount(-num6, "Sublimates.SimUpdate");
					}
					else
					{
						float num7 = sublimatedMass / info.sublimationRate;
						b = info.diseaseIdx;
						num6 = (int)((float)info.diseaseCount * num7);
					}
					float num8 = Mathf.Min(sublimatedMass, info.maxDestinationMass - num2);
					if (num8 > 0f)
					{
						Emit(num, num8, primaryElement.Temperature, b, num6);
						sublimatedMass = Mathf.Max(0f, sublimatedMass - num8);
						primaryElement.Mass = Mathf.Max(0f, primaryElement.Mass - num8);
						UpdateStorage();
						RefreshStatusItem(EmitState.Emitting);
					}
					else
					{
						RefreshStatusItem(EmitState.BlockedOnPressure);
					}
				}
			}
			else if (sublimatedMass > 0f)
			{
				float num9 = Mathf.Min(sublimatedMass, info.maxDestinationMass - num2);
				if (num9 > 0f)
				{
					Emit(num, num9, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
					sublimatedMass = Mathf.Max(0f, sublimatedMass - num9);
					primaryElement.Mass = Mathf.Max(0f, primaryElement.Mass - num9);
					UpdateStorage();
					RefreshStatusItem(EmitState.Emitting);
				}
				else
				{
					RefreshStatusItem(EmitState.BlockedOnPressure);
				}
			}
			else if (!primaryElement.KeepZeroMassObject)
			{
				Util.KDestroyGameObject(base.gameObject);
			}
		}
		else
		{
			RefreshStatusItem(EmitState.BlockedOnPressure);
		}
	}

	private void UpdateStorage()
	{
		Pickupable component = GetComponent<Pickupable>();
		if (component != null && component.storage != null)
		{
			component.storage.Trigger(-1697596308, base.gameObject);
		}
	}

	private void Emit(int cell, float mass, float temperature, byte disease_idx, int disease_count)
	{
		SimMessages.AddRemoveSubstance(cell, info.sublimatedElement, CellEventLogger.Instance.SublimatesEmit, mass, temperature, disease_idx, disease_count);
		Game.Instance.accumulators.Accumulate(flowAccumulator, mass);
		if (spawnFXHash != 0)
		{
			Vector3 position = base.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
			Game.Instance.SpawnFX(spawnFXHash, base.transform.GetPosition(), 0f);
		}
	}

	public float AvgFlowRate()
	{
		return Game.Instance.accumulators.GetAverageRate(flowAccumulator);
	}

	private void RefreshStatusItem(EmitState newEmitState)
	{
		if (newEmitState == lastEmitState)
		{
			return;
		}
		switch (newEmitState)
		{
		case EmitState.Emitting:
			if (info.sublimatedElement == SimHashes.Oxygen)
			{
				selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.EmittingOxygenAvg, this);
			}
			else
			{
				selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.EmittingGasAvg, this);
			}
			break;
		case EmitState.BlockedOnPressure:
			selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.EmittingBlockedHighPressure, this);
			break;
		case EmitState.BlockedOnTemperature:
			selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.EmittingBlockedLowTemperature, this);
			break;
		}
		lastEmitState = newEmitState;
	}
}
