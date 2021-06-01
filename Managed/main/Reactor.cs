using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Reactor : StateMachineComponent<Reactor.StatesInstance>, IGameObjectEffectDescriptor
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

			public State pre;

			public State reacting;

			public State pst;

			public VentingStates venting;
		}

		public class MeltdownStates : State
		{
			public State almost_pre;

			public State almost_loop;

			public State pre;

			public State loop;
		}

		public Signal doVent;

		public BoolParameter canVent = new BoolParameter(default_value: true);

		public BoolParameter reactionUnderway = new BoolParameter();

		public FloatParameter meltdownMassRemaining = new FloatParameter(0f);

		public FloatParameter timeSinceMeltdown = new FloatParameter(0f);

		public BoolParameter meltingDown = new BoolParameter(default_value: false);

		public BoolParameter melted = new BoolParameter(default_value: false);

		public State off;

		public State off_pre;

		public ReactingStates on;

		public MeltdownStates meltdown;

		public State dead;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = SerializeType.ParamsOnly;
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
					smi.master.waterMeter.SetPositionPercent(storedCoolant.Mass / 90f);
				}
			});
			off_pre.QueueAnim("working_pst").OnAnimQueueComplete(off);
			off.PlayAnim("off").Enter(delegate(StatesInstance smi)
			{
				smi.master.radEmitter.SetEmitting(emitting: false);
				smi.master.SetEmitRads(0f);
			}).ParamTransition(reactionUnderway, on, GameStateMachine<States, StatesInstance, Reactor, object>.IsTrue)
				.ParamTransition(melted, dead, GameStateMachine<States, StatesInstance, Reactor, object>.IsTrue)
				.ParamTransition(meltingDown, meltdown, GameStateMachine<States, StatesInstance, Reactor, object>.IsTrue)
				.Update(delegate(StatesInstance smi, float dt)
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
				smi.sm.reactionUnderway.Set(value: true, smi);
				smi.master.operational.SetActive(value: true);
				smi.master.SetEmitRads(105f);
				smi.master.radEmitter.SetEmitting(emitting: true);
			}).Exit(delegate(StatesInstance smi)
			{
				smi.sm.reactionUnderway.Set(value: false, smi);
			}).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.TransferFuel();
				smi.master.TransferCoolant();
				smi.master.React(dt);
				smi.master.UpdateCoolantStatus();
				smi.master.UpdateVentStatus();
				smi.master.DumpSpentFuel();
				if (!smi.master.fuelDeliveryEnabled)
				{
					smi.master.refuelStausHandle = smi.master.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ReactorRefuelDisabled);
				}
				else
				{
					smi.master.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ReactorRefuelDisabled);
					smi.master.refuelStausHandle = Guid.Empty;
				}
				PrimaryElement activeCoolant2 = smi.master.GetActiveCoolant();
				if (activeCoolant2 != null)
				{
					smi.master.Cool(dt);
				}
				PrimaryElement activeFuel = smi.master.GetActiveFuel();
				if (activeFuel != null)
				{
					smi.master.temperatureMeter.SetPositionPercent(Mathf.Clamp01(activeFuel.Temperature / 3000f) / meterFrameScaleHack);
					if (activeFuel.Temperature >= 3000f)
					{
						smi.sm.meltdownMassRemaining.Set(10f + smi.master.supplyStorage.MassStored() + smi.master.reactionStorage.MassStored() + smi.master.wasteStorage.MassStored(), smi);
						smi.master.supplyStorage.ConsumeAllIgnoringDisease();
						smi.master.reactionStorage.ConsumeAllIgnoringDisease();
						smi.master.wasteStorage.ConsumeAllIgnoringDisease();
						smi.GoTo(meltdown.pre);
					}
					else if (activeFuel.Mass <= 0.25f)
					{
						smi.GoTo(off_pre);
						smi.master.temperatureMeter.SetPositionPercent(0f);
					}
				}
				else
				{
					smi.GoTo(off_pre);
					smi.master.temperatureMeter.SetPositionPercent(0f);
				}
			})
				.DefaultState(on.pre);
			on.pre.PlayAnim("working_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(on.reacting).OnSignal(doVent, on.venting);
			on.reacting.PlayAnim("working_loop", KAnim.PlayMode.Loop).OnSignal(doVent, on.venting);
			on.venting.ParamTransition(canVent, on.venting.vent, GameStateMachine<States, StatesInstance, Reactor, object>.IsTrue).ParamTransition(canVent, on.venting.ventIssue, GameStateMachine<States, StatesInstance, Reactor, object>.IsFalse);
			on.venting.ventIssue.PlayAnim("venting_issue", KAnim.PlayMode.Loop).ParamTransition(canVent, on.venting.vent, GameStateMachine<States, StatesInstance, Reactor, object>.IsTrue);
			on.venting.vent.PlayAnim("venting").Enter(delegate(StatesInstance smi)
			{
				PrimaryElement activeCoolant = smi.master.GetActiveCoolant();
				if (activeCoolant != null)
				{
					activeCoolant.GetComponent<Dumpable>().Dump(Grid.CellToPos(smi.master.GetVentCell()));
				}
			}).OnAnimQueueComplete(on.reacting);
			meltdown.ToggleStatusItem(Db.Get().BuildingStatusItems.ReactorMeltdown).ToggleNotification((StatesInstance smi) => smi.master.CreateMeltdownNotification()).ParamTransition(meltdownMassRemaining, dead, (StatesInstance smi, float p) => p <= 0f)
				.ToggleTag(GameTags.DeadReactor)
				.DefaultState(meltdown.loop);
			meltdown.pre.PlayAnim("almost_meltdown_pre", KAnim.PlayMode.Once).QueueAnim("almost_meltdown_loop").QueueAnim("meltdown_pre")
				.OnAnimQueueComplete(meltdown.loop);
			meltdown.loop.PlayAnim("meltdown_loop", KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
			{
				smi.master.radEmitter.SetEmitting(emitting: true);
				smi.master.SetEmitRads(210f);
				smi.master.temperatureMeter.SetPositionPercent(1f / meterFrameScaleHack);
				smi.master.UpdateCoolantStatus();
				if (meltingDown.Get(smi))
				{
					MusicManager.instance.PlaySong(MELTDOWN_STINGER);
					MusicManager.instance.StopDynamicMusic();
				}
				else
				{
					MusicManager.instance.PlaySong(MELTDOWN_STINGER);
					MusicManager.instance.SetSongParameter(MELTDOWN_STINGER, "Music_PlayStinger", 1f);
					MusicManager.instance.StopDynamicMusic();
				}
				meltingDown.Set(value: true, smi);
			}).Exit(delegate(StatesInstance smi)
			{
				meltingDown.Set(value: false, smi);
				MusicManager.instance.SetSongParameter(MELTDOWN_STINGER, "Music_NuclearMeltdownActive", 0f);
			})
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.master.timeSinceMeltdownEmit += dt;
					float num = 0.5f;
					float b = 5f;
					if (smi.master.timeSinceMeltdownEmit > num && smi.sm.meltdownMassRemaining.Get(smi) > 0f)
					{
						smi.master.timeSinceMeltdownEmit -= num;
						float num2 = Mathf.Min(smi.sm.meltdownMassRemaining.Get(smi), b);
						smi.sm.meltdownMassRemaining.Delta(0f - num2, smi);
						for (int i = 0; i < 3; i++)
						{
							if (num2 >= NuclearWasteCometConfig.MASS)
							{
								GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(NuclearWasteCometConfig.ID), smi.master.transform.position + Vector3.up * 2f, Quaternion.identity);
								gameObject.SetActive(value: true);
								Comet component = gameObject.GetComponent<Comet>();
								component.ignoreObstacleForDamage.Set(smi.master.gameObject.GetComponent<KPrefabID>());
								component.addTiles = 1;
								int num3 = 270;
								while (num3 > 225 && num3 < 335)
								{
									num3 = UnityEngine.Random.Range(0, 360);
								}
								float f = (float)num3 * (float)Math.PI / 180f;
								component.Velocity = new Vector2((0f - Mathf.Cos(f)) * 20f, Mathf.Sin(f) * 20f);
								KBatchedAnimController component2 = component.GetComponent<KBatchedAnimController>();
								component2.Rotation = (float)(-num3) - 90f;
								num2 -= NuclearWasteCometConfig.MASS;
							}
						}
						for (int j = 0; j < 3; j++)
						{
							if (num2 >= 0.001f)
							{
								SimMessages.AddRemoveSubstance(Grid.PosToCell(smi.master.transform.position + Vector3.up * 3f + Vector3.right * j * 2f), SimHashes.NuclearWaste, CellEventLogger.Instance.ElementEmitted, num2 / 3f, 3000f, Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.Id), Mathf.RoundToInt(499.99997f * (num2 / 3f)));
							}
						}
					}
				});
			dead.PlayAnim("dead").ToggleTag(GameTags.DeadReactor).Enter(delegate(StatesInstance smi)
			{
				smi.master.temperatureMeter.SetPositionPercent(1f / meterFrameScaleHack);
				smi.master.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.DeadReactorCoolingOff, smi);
				melted.Set(value: true, smi);
			})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.DeadReactorCoolingOff);
				})
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.sm.timeSinceMeltdown.Delta(dt, smi);
					smi.master.radEmitter.emitRads = Mathf.Lerp(210f, 0f, smi.sm.timeSinceMeltdown.Get(smi) / 3000f);
					smi.master.radEmitter.Refresh();
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

	private Tag coolantTag = GameTags.AnyWater;

	private Vector3 dumpOffset = new Vector3(0f, 5f, 0f);

	public static string MELTDOWN_STINGER = "Stinger_Loop_NuclearMeltdown";

	private static float meterFrameScaleHack = 3f;

	[Serialize]
	private float spentFuel = 0f;

	private float timeSinceMeltdownEmit = 0f;

	private const float reactorMeltDownBonusMassAmount = 10f;

	[MyCmpGet]
	private LogicPorts logicPorts;

	private LogicEventHandler fuelControlPort;

	private bool fuelDeliveryEnabled = true;

	public Guid refuelStausHandle;

	private float reactionMassTarget = 60f;

	private int[] ventCells = null;

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

	public float FuelTemperature
	{
		get
		{
			if (reactionStorage.items.Count > 0)
			{
				return reactionStorage.items[0].GetComponent<PrimaryElement>().Temperature;
			}
			return -1f;
		}
	}

	public float ReserveCoolantMass
	{
		get
		{
			PrimaryElement storedCoolant = GetStoredCoolant();
			return (storedCoolant == null) ? 0f : storedCoolant.Mass;
		}
	}

	public bool On => base.smi.IsInsideState(base.smi.sm.on);

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.NuclearReactors.Add(this);
		Storage[] components = GetComponents<Storage>();
		supplyStorage = components[0];
		reactionStorage = components[1];
		wasteStorage = components[2];
		CreateMeters();
		base.smi.StartSM();
		fuelDelivery = GetComponent<ManualDeliveryKG>();
		CheckLogicInputValueChanged(onLoad: true);
	}

	protected override void OnCleanUp()
	{
		Components.NuclearReactors.Remove(this);
		base.OnCleanUp();
	}

	private void Update()
	{
		CheckLogicInputValueChanged();
	}

	public Notification CreateMeltdownNotification()
	{
		KSelectable component = GetComponent<KSelectable>();
		return new Notification(MISC.NOTIFICATIONS.REACTORMELTDOWN.NAME, NotificationType.Bad, (List<Notification> notificationList, object data) => string.Concat(MISC.NOTIFICATIONS.REACTORMELTDOWN.TOOLTIP, notificationList.ReduceMessages(countNames: false)), "/t• " + component.GetProperName(), expires: false);
	}

	public void SetStorages(Storage supply, Storage reaction, Storage waste)
	{
		supplyStorage = supply;
		reactionStorage = reaction;
		wasteStorage = waste;
	}

	private void CheckLogicInputValueChanged(bool onLoad = false)
	{
		int num = 1;
		if (logicPorts.IsPortConnected("CONTROL_FUEL_DELIVERY"))
		{
			num = logicPorts.GetInputValue("CONTROL_FUEL_DELIVERY");
		}
		if (num == 0 && (fuelDeliveryEnabled || onLoad))
		{
			fuelDelivery.refillMass = -1f;
			fuelDeliveryEnabled = false;
		}
		else if (num == 1 && (!fuelDeliveryEnabled || onLoad))
		{
			fuelDelivery.refillMass = reactionMassTarget * 0.2f;
			fuelDeliveryEnabled = true;
		}
	}

	private void OnLogicConnectionChanged(int value, bool connection)
	{
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
		float a = ((storedCoolant != null) ? storedCoolant.Mass : 0f);
		float b = 30f - num;
		b = Mathf.Min(a, b);
		if (b > 0f)
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
		if ((bool)activeCoolant && (bool)activeFuel && activeCoolant.Mass >= 30f && activeFuel.Mass >= 0.5f)
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
			GameUtil.ForceConduction(activeFuel, activeCoolant, dt * 5f);
			if (activeCoolant.Temperature > 673.15f)
			{
				base.smi.sm.doVent.Trigger(base.smi);
			}
		}
	}

	private void React(float dt)
	{
		PrimaryElement activeFuel = GetActiveFuel();
		if (activeFuel != null && activeFuel.Mass >= 0.25f)
		{
			float num = GameUtil.EnergyToTemperatureDelta(-100f * dt * activeFuel.Mass, activeFuel);
			activeFuel.Temperature += num;
			spentFuel += dt * 0.016666668f;
		}
	}

	private void SetEmitRads(float rads)
	{
		base.smi.master.radEmitter.emitRads = rads;
		base.smi.master.radEmitter.Refresh();
	}

	private bool ReadyToCool()
	{
		PrimaryElement activeCoolant = GetActiveCoolant();
		return activeCoolant != null && activeCoolant.Mass > 0f;
	}

	private void DumpSpentFuel()
	{
		PrimaryElement activeFuel = GetActiveFuel();
		if (activeFuel != null && !(spentFuel <= 0f))
		{
			float num = spentFuel * 100f;
			if (num > 0f)
			{
				GameObject go = ElementLoader.FindElementByHash(SimHashes.NuclearWaste).substance.SpawnResource(base.transform.position, num, activeFuel.Temperature, Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.id), Mathf.RoundToInt(num * 499.99997f));
				go.AddTag(GameTags.Stored);
				wasteStorage.Store(go, hide_popups: true);
			}
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
		if (GetStoredCoolant() != null || base.smi.GetCurrentState() == base.smi.sm.meltdown || base.smi.GetCurrentState() == base.smi.sm.dead)
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

	private void InitVentCells()
	{
		if (ventCells == null)
		{
			ventCells = new int[10]
			{
				Grid.PosToCell(base.transform.GetPosition() + base.smi.master.dumpOffset + Vector3.zero),
				Grid.PosToCell(base.transform.GetPosition() + base.smi.master.dumpOffset + Vector3.right),
				Grid.PosToCell(base.transform.GetPosition() + base.smi.master.dumpOffset + Vector3.left),
				Grid.PosToCell(base.transform.GetPosition() + base.smi.master.dumpOffset + Vector3.right + Vector3.right),
				Grid.PosToCell(base.transform.GetPosition() + base.smi.master.dumpOffset + Vector3.left + Vector3.left),
				Grid.PosToCell(base.transform.GetPosition() + base.smi.master.dumpOffset + Vector3.down),
				Grid.PosToCell(base.transform.GetPosition() + base.smi.master.dumpOffset + Vector3.down + Vector3.right),
				Grid.PosToCell(base.transform.GetPosition() + base.smi.master.dumpOffset + Vector3.down + Vector3.left),
				Grid.PosToCell(base.transform.GetPosition() + base.smi.master.dumpOffset + Vector3.down + Vector3.right + Vector3.right),
				Grid.PosToCell(base.transform.GetPosition() + base.smi.master.dumpOffset + Vector3.down + Vector3.left + Vector3.left)
			};
		}
	}

	public int GetVentCell()
	{
		InitVentCells();
		for (int i = 0; i < ventCells.Length; i++)
		{
			if (Grid.Mass[ventCells[i]] < 150f && !Grid.Solid[ventCells[i]])
			{
				return ventCells[i];
			}
		}
		return -1;
	}

	private bool ClearToVent()
	{
		InitVentCells();
		for (int i = 0; i < ventCells.Length; i++)
		{
			if (Grid.Mass[ventCells[i]] < 150f && !Grid.Solid[ventCells[i]])
			{
				return true;
			}
		}
		return false;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>();
	}
}
