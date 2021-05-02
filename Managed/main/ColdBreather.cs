using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class ColdBreather : StateMachineComponent<ColdBreather.StatesInstance>, IGameObjectEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, ColdBreather, object>.GameInstance
	{
		public StatesInstance(ColdBreather master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, ColdBreather>
	{
		public class AliveStates : PlantAliveSubState
		{
			public State mature;

			public State wilting;
		}

		public State grow;

		public State blocked_from_growing;

		public AliveStates alive;

		public State dead;

		private StatusItem statusItemCooling;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = SerializeType.Both_DEPRECATED;
			default_state = grow;
			statusItemCooling = new StatusItem("cooling", CREATURES.STATUSITEMS.COOLING.NAME, CREATURES.STATUSITEMS.COOLING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			dead.ToggleStatusItem(CREATURES.STATUSITEMS.DEAD.NAME, CREATURES.STATUSITEMS.DEAD.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).Enter(delegate(StatesInstance smi)
			{
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront).SetActive(value: true);
				smi.master.Trigger(1623392196);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, smi.master.DestroySelf);
			});
			blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked).EventTransition(GameHashes.EntombedChanged, alive, (StatesInstance smi) => alive.ForceUpdateStatus(smi.master.gameObject)).EventTransition(GameHashes.TooColdWarning, alive, (StatesInstance smi) => alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.TooHotWarning, alive, (StatesInstance smi) => alive.ForceUpdateStatus(smi.master.gameObject))
				.TagTransition(GameTags.Uprooted, dead);
			grow.Enter(delegate(StatesInstance smi)
			{
				if (smi.master.receptacleMonitor.HasReceptacle() && !alive.ForceUpdateStatus(smi.master.gameObject))
				{
					smi.GoTo(blocked_from_growing);
				}
			}).PlayAnim("grow_seed", KAnim.PlayMode.Once).EventTransition(GameHashes.AnimQueueComplete, alive);
			alive.InitializeStates(masterTarget, dead).DefaultState(alive.mature).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.Exhale();
			});
			alive.mature.EventTransition(GameHashes.Wilt, alive.wilting, (StatesInstance smi) => smi.master.wiltCondition.IsWilting()).PlayAnim("idle", KAnim.PlayMode.Loop).ToggleMainStatusItem(statusItemCooling)
				.Enter(delegate(StatesInstance smi)
				{
					smi.master.elementConsumer.EnableConsumption(enabled: true);
					smi.master.radiationEmitter.SetEmitting(emitting: true);
				})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.elementConsumer.EnableConsumption(enabled: false);
					smi.master.radiationEmitter.SetEmitting(emitting: false);
				});
			alive.wilting.PlayAnim("wilt1").EventTransition(GameHashes.WiltRecover, alive.mature, (StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
		}
	}

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private KAnimControllerBase animController;

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private ElementConsumer elementConsumer;

	[MyCmpReq]
	private RadiationEmitter radiationEmitter;

	[MyCmpReq]
	private ReceptacleMonitor receptacleMonitor;

	private const float EXHALE_PERIOD = 1f;

	public float consumptionRate = 0f;

	public float deltaEmitTemperature = -5f;

	public Vector3 emitOffsetCell = new Vector3(0f, 0f);

	private List<GameObject> gases = new List<GameObject>();

	private Tag lastEmitTag;

	private int nextGasEmitIndex = 0;

	private HandleVector<Game.ComplexCallbackInfo<Sim.MassEmittedCallback>>.Handle simEmitCBHandle = HandleVector<Game.ComplexCallbackInfo<Sim.MassEmittedCallback>>.InvalidHandle;

	private static readonly EventSystem.IntraObjectHandler<ColdBreather> OnReplantedDelegate = new EventSystem.IntraObjectHandler<ColdBreather>(delegate(ColdBreather component, object data)
	{
		component.OnReplanted(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		simEmitCBHandle = Game.Instance.massEmitCallbackManager.Add(OnSimEmittedCallback, this, "ColdBreather");
		base.smi.StartSM();
	}

	protected override void OnPrefabInit()
	{
		elementConsumer.EnableConsumption(enabled: false);
		Subscribe(1309017699, OnReplantedDelegate);
		base.OnPrefabInit();
	}

	private void OnReplanted(object data = null)
	{
		ReceptacleMonitor component = GetComponent<ReceptacleMonitor>();
		if (!(component == null))
		{
			ElementConsumer component2 = GetComponent<ElementConsumer>();
			if (component.Replanted)
			{
				component2.consumptionRate = consumptionRate;
			}
			else
			{
				component2.consumptionRate = consumptionRate * 0.25f;
			}
			radiationEmitter.emitRads = 48f;
			radiationEmitter.Refresh();
		}
	}

	protected override void OnCleanUp()
	{
		Game.Instance.massEmitCallbackManager.Release(simEmitCBHandle, "coldbreather");
		simEmitCBHandle.Clear();
		if ((bool)storage)
		{
			storage.DropAll(vent_gas: true);
		}
		base.OnCleanUp();
	}

	protected void DestroySelf(object callbackParam)
	{
		CreatureHelpers.DeselectCreature(base.gameObject);
		Util.KDestroyGameObject(base.gameObject);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(UI.GAMEOBJECTEFFECTS.COLDBREATHER, UI.GAMEOBJECTEFFECTS.TOOLTIPS.COLDBREATHER)
		};
	}

	private void Exhale()
	{
		if (lastEmitTag != Tag.Invalid)
		{
			return;
		}
		gases.Clear();
		storage.Find(GameTags.Gas, gases);
		if (nextGasEmitIndex >= gases.Count)
		{
			nextGasEmitIndex = 0;
		}
		while (nextGasEmitIndex < gases.Count)
		{
			int index = nextGasEmitIndex++;
			PrimaryElement component = gases[index].GetComponent<PrimaryElement>();
			if (component != null && component.Mass > 0f && simEmitCBHandle.IsValid())
			{
				float temperature = Mathf.Max(component.Element.lowTemp + 5f, component.Temperature + deltaEmitTemperature);
				int gameCell = Grid.PosToCell(base.transform.GetPosition() + emitOffsetCell);
				byte idx = component.Element.idx;
				Game.Instance.massEmitCallbackManager.GetItem(simEmitCBHandle);
				SimMessages.EmitMass(gameCell, idx, component.Mass, temperature, component.DiseaseIdx, component.DiseaseCount, simEmitCBHandle.index);
				lastEmitTag = component.Element.tag;
				break;
			}
		}
	}

	private static void OnSimEmittedCallback(Sim.MassEmittedCallback info, object data)
	{
		((ColdBreather)data).OnSimEmitted(info);
	}

	private void OnSimEmitted(Sim.MassEmittedCallback info)
	{
		if (info.suceeded == 1 && (bool)storage && lastEmitTag.IsValid)
		{
			storage.ConsumeIgnoringDisease(lastEmitTag, info.mass);
		}
		lastEmitTag = Tag.Invalid;
	}
}
