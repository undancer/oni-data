using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Reactor : StateMachineComponent<Reactor.StatesInstance>, IGameObjectEffectDescriptor, ISingleSliderControl, ISliderControl
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Reactor, object>.GameInstance
	{
		public StatesInstance(Reactor smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Reactor>
	{
		public class ReactingStates : State
		{
			public class VentingStates : State
			{
				public State ventIssue;

				public State vent;
			}

			public State reacting;

			public VentingStates venting;
		}

		public Signal doVent;

		public BoolParameter canVent = new BoolParameter(default_value: true);

		public State off;

		public ReactingStates on;

		public State meltdown;

		public State dead;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			root.EventHandler(GameHashes.OnStorageChange, delegate(StatesInstance smi)
			{
				PrimaryElement storedCoolant = smi.master.GetStoredCoolant();
				if (!storedCoolant)
				{
					smi.master.waterMeter.SetPositionPercent(0f);
				}
				else
				{
					smi.master.waterMeter.SetPositionPercent(storedCoolant.Mass / 60f);
				}
			});
			off.PlayAnim("off").Enter(delegate(StatesInstance smi)
			{
				smi.master.radEmitter.SetEmitting(emitting: false);
				smi.master.SetEmitRads(0f);
			}).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.TransferFuel();
				smi.master.TransferCoolant();
				if (smi.master.CanStartReaction())
				{
					smi.GoTo(on);
				}
			}, UpdateRate.SIM_1000ms);
			on.Enter(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: true);
				smi.master.SetEmitRads(35f);
				smi.master.radEmitter.SetEmitting(emitting: true);
			}).EventTransition(GameHashes.OperationalChanged, off, (StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.TransferFuel();
				smi.master.TransferCoolant();
				smi.master.UpdateEmitRate();
				smi.master.React(dt);
				smi.master.UpdateCoolantStatus();
				smi.master.UpdateVentStatus();
				smi.master.DumpSpentFuel();
				PrimaryElement activeCoolant2 = smi.master.GetActiveCoolant();
				if (activeCoolant2 != null)
				{
					smi.master.Cool(dt);
				}
				PrimaryElement activeFuel = smi.master.GetActiveFuel();
				if (activeFuel != null)
				{
					float num2 = 300f;
					float num3 = Mathf.Max(activeFuel.Temperature - 673.15f, 0f) + 50f;
					smi.master.temperatureMeter.SetPositionPercent(num3 / num2);
					if (activeFuel.Temperature >= 973.15f)
					{
						smi.GoTo(meltdown);
					}
				}
				else
				{
					smi.GoTo(off);
					smi.master.temperatureMeter.SetPositionPercent(0f);
				}
			})
				.DefaultState(on.reacting);
			on.reacting.PlayAnim("working_loop", KAnim.PlayMode.Loop).OnSignal(doVent, on.venting);
			on.venting.ParamTransition(canVent, on.venting.vent, GameStateMachine<States, StatesInstance, Reactor, object>.IsTrue).ParamTransition(canVent, on.venting.ventIssue, GameStateMachine<States, StatesInstance, Reactor, object>.IsFalse);
			on.venting.ventIssue.PlayAnim("venting_issue", KAnim.PlayMode.Loop).ParamTransition(canVent, on.venting.vent, GameStateMachine<States, StatesInstance, Reactor, object>.IsTrue);
			on.venting.vent.PlayAnim("venting").Enter(delegate(StatesInstance smi)
			{
				PrimaryElement activeCoolant = smi.master.GetActiveCoolant();
				activeCoolant.GetComponent<Dumpable>().Dump(smi.master.transform.GetPosition() + smi.master.dumpOffset);
			}).OnAnimQueueComplete(on);
			meltdown.PlayAnim("on").ScheduleGoTo(40f, dead).Enter(delegate(StatesInstance smi)
			{
				smi.master.radEmitter.SetEmitting(emitting: true);
				smi.master.radEmitter.emitRate = 0.5f;
				smi.master.SetEmitRads(100f);
				smi.master.temperatureMeter.SetPositionPercent(1f);
			})
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.master.timeSinceMeltdownEmit += dt;
					if (smi.master.timeSinceMeltdownEmit > 0.5f)
					{
						smi.master.timeSinceMeltdownEmit -= 0.5f;
						for (int i = 0; i < 3; i++)
						{
							GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(NuclearWasteCometConfig.ID), smi.master.transform.position + Vector3.up * 2f, Quaternion.identity);
							gameObject.SetActive(value: true);
							Comet component = gameObject.GetComponent<Comet>();
							int num = 270;
							while (num > 225 && num < 335)
							{
								num = UnityEngine.Random.Range(0, 360);
							}
							float f = (float)num * (float)Math.PI / 180f;
							component.Velocity = new Vector2((0f - Mathf.Cos(f)) * 20f, Mathf.Sin(f) * 20f);
							KBatchedAnimController component2 = component.GetComponent<KBatchedAnimController>();
							component2.Rotation = (float)(-num) - 90f;
							SimMessages.AddRemoveSubstance(Grid.PosToCell(smi.master.transform.position + Vector3.up * 3f), SimHashes.NuclearWaste, CellEventLogger.Instance.ElementEmitted, 1000f * smi.master.timeSinceMeltdownEmit, 500f, Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.Id), Mathf.RoundToInt(1000000f * smi.master.timeSinceMeltdownEmit));
							SimMessages.AddRemoveSubstance(Grid.PosToCell(smi.master.transform.position + Vector3.up * 3f + Vector3.right * 2f), SimHashes.NuclearWaste, CellEventLogger.Instance.ElementEmitted, 1000f * smi.master.timeSinceMeltdownEmit, 500f, Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.Id), Mathf.RoundToInt(1000000f * smi.master.timeSinceMeltdownEmit));
							SimMessages.AddRemoveSubstance(Grid.PosToCell(smi.master.transform.position + Vector3.up * 3f + Vector3.right * 4f), SimHashes.NuclearWaste, CellEventLogger.Instance.ElementEmitted, 1000f * smi.master.timeSinceMeltdownEmit, 500f, Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.Id), Mathf.RoundToInt(1000000f * smi.master.timeSinceMeltdownEmit));
						}
					}
				});
			dead.PlayAnim("on").Enter(delegate(StatesInstance smi)
			{
				smi.master.radEmitter.SetEmitting(emitting: false);
				smi.master.temperatureMeter.SetPositionPercent(1f);
			});
		}
	}

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private RadiationEmitter radEmitter;

	[MyCmpGet]
	private ManualDeliveryKG fuelDelivery;

	private MeterController temperatureMeter;

	private MeterController waterMeter;

	private Storage supplyStorage;

	private Storage reactionStorage;

	private Storage wasteStorage;

	private Tag fuelTag = SimHashes.EnrichedUranium.CreateTag();

	private Tag coolantTag = SimHashes.Water.CreateTag();

	private Vector3 dumpOffset = new Vector3(0f, 5f, 0f);

	private float spentFuel = 0f;

	private float timeSinceMeltdownEmit = 0f;

	[Serialize]
	private float reactionMassTarget = 5f;

	private float ReactionMassTarget
	{
		get
		{
			return reactionMassTarget;
		}
		set
		{
			fuelDelivery.capacity = value * 2f;
			fuelDelivery.refillMass = value * 0.2f;
			fuelDelivery.minimumMass = value * 0.2f;
			reactionMassTarget = value;
		}
	}

	public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.NUCLEAR_REACTOR_SIDE_SCREEN.TITLE";

	public string SliderUnits => UI.UNITSUFFIXES.MASS.KILOGRAM;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Storage[] components = GetComponents<Storage>();
		supplyStorage = components[0];
		reactionStorage = components[1];
		wasteStorage = components[2];
		CreateMeters();
		base.smi.StartSM();
	}

	public void SetStorages(Storage supply, Storage reaction, Storage waste)
	{
		supplyStorage = supply;
		reactionStorage = reaction;
		wasteStorage = waste;
	}

	private void CreateMeters()
	{
		temperatureMeter = new MeterController(GetComponent<KBatchedAnimController>(), "temperature_meter_target", "meter_temperature", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "temperature_meter_target");
		waterMeter = new MeterController(GetComponent<KBatchedAnimController>(), "water_meter_target", "meter_water", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "water_meter_target");
	}

	private void TransferFuel()
	{
		PrimaryElement activeFuel = GetActiveFuel();
		PrimaryElement storedFuel = GetStoredFuel();
		float num = ((activeFuel != null) ? activeFuel.Mass : 0f);
		float num2 = ((storedFuel != null) ? storedFuel.Mass : 0f);
		float b = ReactionMassTarget - num;
		b = Mathf.Min(num2, b);
		if (b > 0.5f || b == num2)
		{
			supplyStorage.Transfer(reactionStorage, fuelTag, b, block_events: false, hide_popups: true);
		}
	}

	private void TransferCoolant()
	{
		PrimaryElement activeCoolant = GetActiveCoolant();
		PrimaryElement storedCoolant = GetStoredCoolant();
		float num = ((activeCoolant != null) ? activeCoolant.Mass : 0f);
		float num2 = ((storedCoolant != null) ? storedCoolant.Mass : 0f);
		float b = 30f - num;
		b = Mathf.Min(num2, b);
		if (b > 3f || b == num2)
		{
			supplyStorage.Transfer(reactionStorage, coolantTag, b, block_events: false, hide_popups: true);
		}
	}

	private PrimaryElement GetStoredFuel()
	{
		GameObject gameObject = supplyStorage.FindFirst(fuelTag);
		if ((bool)gameObject && (bool)gameObject.GetComponent<PrimaryElement>())
		{
			return gameObject.GetComponent<PrimaryElement>();
		}
		return null;
	}

	private PrimaryElement GetActiveFuel()
	{
		GameObject gameObject = reactionStorage.FindFirst(fuelTag);
		if ((bool)gameObject && (bool)gameObject.GetComponent<PrimaryElement>())
		{
			return gameObject.GetComponent<PrimaryElement>();
		}
		return null;
	}

	private PrimaryElement GetStoredCoolant()
	{
		GameObject gameObject = supplyStorage.FindFirst(coolantTag);
		if ((bool)gameObject && (bool)gameObject.GetComponent<PrimaryElement>())
		{
			return gameObject.GetComponent<PrimaryElement>();
		}
		return null;
	}

	private PrimaryElement GetActiveCoolant()
	{
		GameObject gameObject = reactionStorage.FindFirst(coolantTag);
		if ((bool)gameObject && (bool)gameObject.GetComponent<PrimaryElement>())
		{
			return gameObject.GetComponent<PrimaryElement>();
		}
		return null;
	}

	private bool CanStartReaction()
	{
		PrimaryElement activeCoolant = GetActiveCoolant();
		PrimaryElement activeFuel = GetActiveFuel();
		if ((bool)activeCoolant && (bool)activeFuel && activeCoolant.Mass > 3f && activeFuel.Mass > 0.5f)
		{
			return true;
		}
		return false;
	}

	private void Cool(float dt)
	{
		PrimaryElement activeFuel = GetActiveFuel();
		if (activeFuel == null)
		{
			return;
		}
		PrimaryElement activeCoolant = GetActiveCoolant();
		if (!(activeCoolant == null))
		{
			GameUtil.ForceConduction(activeFuel, activeCoolant, dt);
			if (activeCoolant.Temperature > 673.15f)
			{
				base.smi.sm.doVent.Trigger(base.smi);
			}
		}
	}

	private void React(float dt)
	{
		PrimaryElement activeFuel = GetActiveFuel();
		if (activeFuel != null)
		{
			float num = GameUtil.EnergyToTemperatureDelta(-10f * dt * activeFuel.Mass, activeFuel);
			activeFuel.Temperature += num;
			spentFuel += dt * 0.016666668f;
		}
	}

	private void UpdateEmitRate()
	{
		float num = 0.1f;
		float num2 = ((base.smi.master.GetActiveFuel() != null) ? base.smi.master.GetActiveFuel().Mass : 0f);
		num = Mathf.Lerp(6f, 3f, num2 / 10f);
		base.smi.master.radEmitter.emitRate = num;
		base.smi.master.radEmitter.Refresh();
	}

	private void SetEmitRads(float rads)
	{
		base.smi.master.radEmitter.emitRads = rads;
		base.smi.master.radEmitter.Refresh();
	}

	private bool ReadyToCool()
	{
		return GetActiveCoolant().Mass > 0f;
	}

	private void DumpSpentFuel()
	{
		PrimaryElement activeFuel = GetActiveFuel();
		if (activeFuel != null)
		{
			if (spentFuel <= 0f)
			{
				Debug.Assert(condition: true, "Trying to spawn zero spent fuel?");
			}
			GameObject go = ElementLoader.FindElementByHash(SimHashes.NuclearWaste).substance.SpawnResource(base.transform.position, spentFuel, activeFuel.Temperature, Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.id), Mathf.RoundToInt(spentFuel * 10000f));
			wasteStorage.Store(go, hide_popups: true);
			if (wasteStorage.MassStored() >= 100f)
			{
				wasteStorage.DropAll(vent_gas: true, dump_liquid: true);
			}
			if (spentFuel >= activeFuel.Mass)
			{
				Util.KDestroyGameObject(activeFuel.gameObject);
				spentFuel = 0f;
			}
			else
			{
				activeFuel.Mass -= spentFuel;
				spentFuel = 0f;
			}
		}
	}

	private void UpdateVentStatus()
	{
		KSelectable component = GetComponent<KSelectable>();
		if (ClearToVent())
		{
			if (component.HasStatusItem(Db.Get().BuildingStatusItems.GasVentOverPressure))
			{
				base.smi.sm.canVent.Set(value: true, base.smi);
				component.RemoveStatusItem(Db.Get().BuildingStatusItems.GasVentOverPressure);
			}
		}
		else if (!component.HasStatusItem(Db.Get().BuildingStatusItems.GasVentOverPressure))
		{
			base.smi.sm.canVent.Set(value: false, base.smi);
			component.AddStatusItem(Db.Get().BuildingStatusItems.GasVentOverPressure);
		}
	}

	private void UpdateCoolantStatus()
	{
		KSelectable component = GetComponent<KSelectable>();
		if (GetStoredCoolant() != null)
		{
			if (component.HasStatusItem(Db.Get().BuildingStatusItems.NoCoolant))
			{
				component.RemoveStatusItem(Db.Get().BuildingStatusItems.NoCoolant);
			}
		}
		else if (!component.HasStatusItem(Db.Get().BuildingStatusItems.NoCoolant))
		{
			component.AddStatusItem(Db.Get().BuildingStatusItems.NoCoolant);
		}
	}

	private bool ClearToVent()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		cell = Grid.CellAbove(cell);
		if (GameUtil.FloodFillCheck((int c, Reactor o) => Grid.Mass[c] < 20f, this, cell, 5, stop_at_solid: false, stop_at_liquid: false))
		{
			return true;
		}
		return false;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>();
	}

	public int SliderDecimalPlaces(int index)
	{
		return 0;
	}

	public float GetSliderMin(int index)
	{
		return 1f;
	}

	public float GetSliderMax(int index)
	{
		return 10f;
	}

	public float GetSliderValue(int index)
	{
		return ReactionMassTarget;
	}

	public void SetSliderValue(float value, int index)
	{
		ReactionMassTarget = value;
	}

	string ISliderControl.GetSliderTooltip()
	{
		ManualDeliveryKG component = GetComponent<ManualDeliveryKG>();
		return string.Format(Strings.Get("STRINGS.UI.UISIDESCREENS.NUCLEAR_REACTOR_SIDE_SCREEN.TOOLTIP"), ReactionMassTarget, UI.UNITSUFFIXES.MASS.KILOGRAM, component.requestedItemTag.ProperName());
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.NUCLEAR_REACTOR_SIDE_SCREEN.TOOLTIP";
	}
}
