using System;
using Klei.AI;
using KSerialization;
using UnityEngine;

[RequireComponent(typeof(Health))]
[AddComponentMenu("KMonoBehaviour/scripts/OxygenBreather")]
public class OxygenBreather : KMonoBehaviour, ISim200ms
{
	public interface IGasProvider
	{
		void OnSetOxygenBreather(OxygenBreather oxygen_breather);

		void OnClearOxygenBreather(OxygenBreather oxygen_breather);

		bool ConsumeGas(OxygenBreather oxygen_breather, float amount);

		bool ShouldEmitCO2();

		bool ShouldStoreCO2();
	}

	public static CellOffset[] DEFAULT_BREATHABLE_OFFSETS = new CellOffset[6]
	{
		new CellOffset(0, 0),
		new CellOffset(0, 1),
		new CellOffset(1, 1),
		new CellOffset(-1, 1),
		new CellOffset(1, 0),
		new CellOffset(-1, 0)
	};

	public float O2toCO2conversion = 0.5f;

	public float lowOxygenThreshold;

	public float noOxygenThreshold;

	public Vector2 mouthOffset;

	[Serialize]
	public float accumulatedCO2;

	[SerializeField]
	public float minCO2ToEmit = 0.3f;

	private bool hasAir = true;

	private Timer hasAirTimer = new Timer();

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpGet]
	private Facing facing;

	private HandleVector<int>.Handle o2Accumulator = HandleVector<int>.InvalidHandle;

	private HandleVector<int>.Handle co2Accumulator = HandleVector<int>.InvalidHandle;

	private AmountInstance temperature;

	private AttributeInstance airConsumptionRate;

	public CellOffset[] breathableCells;

	public Action<Sim.MassConsumedCallback> onSimConsume;

	private IGasProvider gasProvider;

	private static readonly EventSystem.IntraObjectHandler<OxygenBreather> OnDeadTagAddedDelegate = GameUtil.CreateHasTagHandler(GameTags.Dead, delegate(OxygenBreather component, object data)
	{
		component.OnDeath(data);
	});

	public float CO2EmitRate => Game.Instance.accumulators.GetAverageRate(co2Accumulator);

	public HandleVector<int>.Handle O2Accumulator => o2Accumulator;

	public int mouthCell
	{
		get
		{
			int cell = Grid.PosToCell(this);
			return GetMouthCellAtCell(cell, breathableCells);
		}
	}

	public bool IsUnderLiquid => Grid.Element[mouthCell].IsLiquid;

	public bool IsSuffocating => !hasAir;

	public SimHashes GetBreathableElement => GetBreathableElementAtCell(Grid.PosToCell(this));

	public bool IsBreathableElement => IsBreathableElementAtCell(Grid.PosToCell(this));

	protected override void OnPrefabInit()
	{
		GameUtil.SubscribeToTags(this, OnDeadTagAddedDelegate, triggerImmediately: true);
	}

	public bool IsLowOxygen()
	{
		return GetOxygenPressure(mouthCell) < lowOxygenThreshold;
	}

	protected override void OnSpawn()
	{
		airConsumptionRate = Db.Get().Attributes.AirConsumptionRate.Lookup(this);
		o2Accumulator = Game.Instance.accumulators.Add("O2", this);
		co2Accumulator = Game.Instance.accumulators.Add("CO2", this);
		KSelectable component = GetComponent<KSelectable>();
		component.AddStatusItem(Db.Get().DuplicantStatusItems.BreathingO2, this);
		component.AddStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, this);
		temperature = Db.Get().Amounts.Temperature.Lookup(this);
		NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this);
	}

	protected override void OnCleanUp()
	{
		Game.Instance.accumulators.Remove(o2Accumulator);
		Game.Instance.accumulators.Remove(co2Accumulator);
		SetGasProvider(null);
		base.OnCleanUp();
	}

	public void Consume(Sim.MassConsumedCallback mass_consumed)
	{
		if (onSimConsume != null)
		{
			onSimConsume(mass_consumed);
		}
	}

	public void Sim200ms(float dt)
	{
		if (base.gameObject.HasTag(GameTags.Dead))
		{
			return;
		}
		float num = airConsumptionRate.GetTotalValue() * dt;
		bool flag = gasProvider.ConsumeGas(this, num);
		if (flag)
		{
			if (gasProvider.ShouldEmitCO2())
			{
				float num2 = num * O2toCO2conversion;
				Game.Instance.accumulators.Accumulate(co2Accumulator, num2);
				accumulatedCO2 += num2;
				if (accumulatedCO2 >= minCO2ToEmit)
				{
					accumulatedCO2 -= minCO2ToEmit;
					Vector3 position = base.transform.GetPosition();
					Vector3 position2 = position;
					position2.x += (facing.GetFacing() ? (0f - mouthOffset.x) : mouthOffset.x);
					position2.y += mouthOffset.y;
					position2.z -= 0.5f;
					if (Mathf.FloorToInt(position2.x) != Mathf.FloorToInt(position.x))
					{
						position2.x = Mathf.Floor(position.x) + (facing.GetFacing() ? 0.01f : 0.99f);
					}
					CO2Manager.instance.SpawnBreath(position2, minCO2ToEmit, temperature.value, facing.GetFacing());
				}
			}
			else if (gasProvider.ShouldStoreCO2())
			{
				Equippable equippable = GetComponent<SuitEquipper>().IsWearingAirtightSuit();
				if (equippable != null)
				{
					float num3 = num * O2toCO2conversion;
					Game.Instance.accumulators.Accumulate(co2Accumulator, num3);
					accumulatedCO2 += num3;
					if (accumulatedCO2 >= minCO2ToEmit)
					{
						accumulatedCO2 -= minCO2ToEmit;
						equippable.GetComponent<Storage>().AddGasChunk(SimHashes.CarbonDioxide, minCO2ToEmit, temperature.value, byte.MaxValue, 0, keep_zero_mass: false);
					}
				}
			}
		}
		if (flag != hasAir)
		{
			hasAirTimer.Start();
			if (hasAirTimer.TryStop(2f))
			{
				hasAir = flag;
			}
		}
		else
		{
			hasAirTimer.Stop();
		}
	}

	private void OnDeath(object data)
	{
		base.enabled = false;
		KSelectable component = GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().DuplicantStatusItems.BreathingO2);
		component.RemoveStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2);
	}

	private int GetMouthCellAtCell(int cell, CellOffset[] offsets)
	{
		float num = 0f;
		int result = cell;
		foreach (CellOffset offset in offsets)
		{
			int num2 = Grid.OffsetCell(cell, offset);
			float oxygenPressure = GetOxygenPressure(num2);
			if (oxygenPressure > num && oxygenPressure > noOxygenThreshold)
			{
				num = oxygenPressure;
				result = num2;
			}
		}
		return result;
	}

	public bool IsBreathableElementAtCell(int cell, CellOffset[] offsets = null)
	{
		return GetBreathableElementAtCell(cell, offsets) != SimHashes.Vacuum;
	}

	public SimHashes GetBreathableElementAtCell(int cell, CellOffset[] offsets = null)
	{
		if (offsets == null)
		{
			offsets = breathableCells;
		}
		int mouthCellAtCell = GetMouthCellAtCell(cell, offsets);
		if (!Grid.IsValidCell(mouthCellAtCell))
		{
			return SimHashes.Vacuum;
		}
		Element element = Grid.Element[mouthCellAtCell];
		if (!element.IsGas || !element.HasTag(GameTags.Breathable) || !(Grid.Mass[mouthCellAtCell] > noOxygenThreshold))
		{
			return SimHashes.Vacuum;
		}
		return element.id;
	}

	private float GetOxygenPressure(int cell)
	{
		if (Grid.IsValidCell(cell) && Grid.Element[cell].HasTag(GameTags.Breathable))
		{
			return Grid.Mass[cell];
		}
		return 0f;
	}

	public IGasProvider GetGasProvider()
	{
		return gasProvider;
	}

	public void SetGasProvider(IGasProvider gas_provider)
	{
		if (gasProvider != null)
		{
			gasProvider.OnClearOxygenBreather(this);
		}
		gasProvider = gas_provider;
		if (gasProvider != null)
		{
			gasProvider.OnSetOxygenBreather(this);
		}
	}
}
