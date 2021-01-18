using KSerialization;
using STRINGS;
using UnityEngine;

public class OilEater : StateMachineComponent<OilEater.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, OilEater, object>.GameInstance
	{
		public StatesInstance(OilEater master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, OilEater>
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

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = grow;
			dead.ToggleStatusItem(CREATURES.STATUSITEMS.DEAD.NAME, CREATURES.STATUSITEMS.DEAD.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).Enter(delegate(StatesInstance smi)
			{
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront).SetActive(value: true);
				smi.master.Trigger(1623392196);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, delegate(object data)
				{
					GameObject obj = (GameObject)data;
					CreatureHelpers.DeselectCreature(obj);
					Util.KDestroyGameObject(obj);
				}, smi.master.gameObject);
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
			alive.InitializeStates(masterTarget, dead).DefaultState(alive.mature).Update("Alive", delegate(StatesInstance smi, float dt)
			{
				smi.master.Exhaust(dt);
			});
			alive.mature.EventTransition(GameHashes.Wilt, alive.wilting, (StatesInstance smi) => smi.master.wiltCondition.IsWilting()).PlayAnim("idle", KAnim.PlayMode.Loop);
			alive.wilting.PlayAnim("wilt1").EventTransition(GameHashes.WiltRecover, alive.mature, (StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
		}
	}

	private const SimHashes srcElement = SimHashes.CrudeOil;

	private const SimHashes emitElement = SimHashes.CarbonDioxide;

	public float emitRate = 1f;

	public float minEmitMass;

	public Vector3 emitOffset = Vector3.zero;

	[Serialize]
	private float emittedMass;

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private ReceptacleMonitor receptacleMonitor;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void Exhaust(float dt)
	{
		if (!base.smi.master.wiltCondition.IsWilting())
		{
			emittedMass += dt * emitRate;
			if (emittedMass >= minEmitMass)
			{
				SimMessages.AddRemoveSubstance(Grid.PosToCell(base.transform.GetPosition() + emitOffset), temperature: GetComponent<PrimaryElement>().Temperature, new_element: SimHashes.CarbonDioxide, ev: CellEventLogger.Instance.ElementEmitted, mass: emittedMass, disease_idx: byte.MaxValue, disease_count: 0);
				emittedMass = 0f;
			}
		}
	}
}
