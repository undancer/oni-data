using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Exhaust")]
public class Exhaust : KMonoBehaviour, ISim200ms
{
	private delegate void EmitDelegate(int cell, PrimaryElement primary_element);

	[MyCmpGet]
	private Vent vent;

	[MyCmpGet]
	private Storage storage;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private ConduitConsumer consumer;

	[MyCmpGet]
	private PrimaryElement exhaustPE;

	private static readonly Operational.Flag canExhaust = new Operational.Flag("canExhaust", Operational.Flag.Type.Requirement);

	private bool isAnimating;

	private bool recentlyExhausted;

	private const float MinSwitchTime = 1f;

	private float elapsedSwitchTime;

	private static readonly EventSystem.IntraObjectHandler<Exhaust> OnConduitStateChangedDelegate = new EventSystem.IntraObjectHandler<Exhaust>(delegate(Exhaust component, object data)
	{
		component.OnConduitStateChanged(data);
	});

	private static EmitDelegate emit_element = delegate(int cell, PrimaryElement primary_element)
	{
		SimMessages.AddRemoveSubstance(cell, primary_element.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, primary_element.Mass, primary_element.Temperature, primary_element.DiseaseIdx, primary_element.DiseaseCount);
	};

	private static EmitDelegate emit_particle = delegate(int cell, PrimaryElement primary_element)
	{
		FallingWater.instance.AddParticle(cell, (byte)ElementLoader.elements.IndexOf(primary_element.Element), primary_element.Mass, primary_element.Temperature, primary_element.DiseaseIdx, primary_element.DiseaseCount, skip_sound: true, skip_decor: false, debug_track: true);
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-592767678, OnConduitStateChangedDelegate);
		Subscribe(-111137758, OnConduitStateChangedDelegate);
		GetComponent<RequireInputs>().visualizeRequirements = false;
	}

	protected override void OnSpawn()
	{
		OnConduitStateChanged(null);
	}

	private void OnConduitStateChanged(object data)
	{
		operational.SetActive(operational.IsOperational && !vent.IsBlocked);
	}

	private void CalculateDiseaseTransfer(PrimaryElement item1, PrimaryElement item2, float transfer_rate, out int disease_to_item1, out int disease_to_item2)
	{
		disease_to_item1 = (int)((float)item2.DiseaseCount * transfer_rate);
		disease_to_item2 = (int)((float)item1.DiseaseCount * transfer_rate);
	}

	public void Sim200ms(float dt)
	{
		operational.SetFlag(canExhaust, !vent.IsBlocked);
		if (!operational.IsOperational)
		{
			if (isAnimating)
			{
				isAnimating = false;
				recentlyExhausted = false;
				Trigger(-793429877);
			}
			return;
		}
		UpdateEmission();
		elapsedSwitchTime -= dt;
		if (elapsedSwitchTime <= 0f)
		{
			elapsedSwitchTime = 1f;
			if (recentlyExhausted != isAnimating)
			{
				isAnimating = recentlyExhausted;
				Trigger(-793429877);
			}
			recentlyExhausted = false;
		}
	}

	public bool IsAnimating()
	{
		return isAnimating;
	}

	private void UpdateEmission()
	{
		if (consumer.ConsumptionRate == 0f || storage.items.Count == 0)
		{
			return;
		}
		int num = Grid.PosToCell(base.transform.GetPosition());
		if (!Grid.Solid[num])
		{
			switch (consumer.TypeOfConduit)
			{
			case ConduitType.Liquid:
				EmitLiquid(num);
				break;
			case ConduitType.Gas:
				EmitGas(num);
				break;
			}
		}
	}

	private bool EmitCommon(int cell, PrimaryElement primary_element, EmitDelegate emit)
	{
		if (primary_element.Mass <= 0f)
		{
			return false;
		}
		CalculateDiseaseTransfer(exhaustPE, primary_element, 0.05f, out var disease_to_item, out var disease_to_item2);
		primary_element.ModifyDiseaseCount(-disease_to_item, "Exhaust transfer");
		primary_element.AddDisease(exhaustPE.DiseaseIdx, disease_to_item2, "Exhaust transfer");
		exhaustPE.ModifyDiseaseCount(-disease_to_item2, "Exhaust transfer");
		exhaustPE.AddDisease(primary_element.DiseaseIdx, disease_to_item, "Exhaust transfer");
		emit(cell, primary_element);
		if (vent != null)
		{
			vent.UpdateVentedMass(primary_element.ElementID, primary_element.Mass);
		}
		primary_element.KeepZeroMassObject = true;
		primary_element.Mass = 0f;
		primary_element.ModifyDiseaseCount(int.MinValue, "Exhaust.SimUpdate");
		recentlyExhausted = true;
		return true;
	}

	private void EmitLiquid(int cell)
	{
		int num = Grid.CellBelow(cell);
		EmitDelegate emit = ((Grid.IsValidCell(num) && !Grid.Solid[num]) ? emit_particle : emit_element);
		foreach (GameObject item in storage.items)
		{
			PrimaryElement component = item.GetComponent<PrimaryElement>();
			if (component.Element.IsLiquid && EmitCommon(cell, component, emit))
			{
				break;
			}
		}
	}

	private void EmitGas(int cell)
	{
		foreach (GameObject item in storage.items)
		{
			PrimaryElement component = item.GetComponent<PrimaryElement>();
			if (component.Element.IsGas && EmitCommon(cell, component, emit_element))
			{
				break;
			}
		}
	}
}
