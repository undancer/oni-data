using STRINGS;
using UnityEngine;

public class JungleGasPlant : StateMachineComponent<JungleGasPlant.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, JungleGasPlant, object>.GameInstance
	{
		public StatesInstance(JungleGasPlant master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, JungleGasPlant>
	{
		public class AliveStates : PlantAliveSubState
		{
			public State seed_grow;

			public State idle;

			public WiltingState wilting;

			public GrownState grown;

			public State destroy;
		}

		public class GrownState : State
		{
			public State pre;

			public State idle;
		}

		public class WiltingState : State
		{
			public State pre;

			public State idle;

			public State pst;
		}

		public State blocked_from_growing;

		public AliveStates alive;

		public State dead;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = alive.seed_grow;
			base.serializable = true;
			root.Enter(delegate(StatesInstance smi)
			{
				if (smi.master.rm.Replanted && !alive.ForceUpdateStatus(smi.master.gameObject))
				{
					smi.GoTo(blocked_from_growing);
				}
				else
				{
					smi.GoTo(alive.seed_grow);
				}
			});
			dead.ToggleStatusItem(CREATURES.STATUSITEMS.DEAD.NAME, CREATURES.STATUSITEMS.DEAD.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).Enter(delegate(StatesInstance smi)
			{
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront).SetActive(value: true);
				smi.master.Trigger(1623392196);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, smi.master.DestroySelf);
			});
			blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked).TagTransition(GameTags.Entombed, alive.seed_grow, on_remove: true).EventTransition(GameHashes.TooColdWarning, alive.seed_grow)
				.EventTransition(GameHashes.TooHotWarning, alive.seed_grow)
				.TagTransition(GameTags.Uprooted, dead);
			alive.InitializeStates(masterTarget, dead);
			alive.seed_grow.QueueAnim("seed_grow").EventTransition(GameHashes.AnimQueueComplete, alive.idle).EventTransition(GameHashes.Wilt, alive.wilting, (StatesInstance smi) => smi.master.wiltCondition.IsWilting());
			alive.idle.EventTransition(GameHashes.Wilt, alive.wilting, (StatesInstance smi) => smi.master.wiltCondition.IsWilting()).EventTransition(GameHashes.Grow, alive.grown, (StatesInstance smi) => smi.master.growing.IsGrown()).PlayAnim("idle_loop", KAnim.PlayMode.Loop);
			alive.grown.DefaultState(alive.grown.pre).EventTransition(GameHashes.Wilt, alive.wilting, (StatesInstance smi) => smi.master.wiltCondition.IsWilting()).Enter(delegate(StatesInstance smi)
			{
				smi.master.elementEmitter.SetEmitting(emitting: true);
			})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.elementEmitter.SetEmitting(emitting: false);
				});
			alive.grown.pre.PlayAnim("grow", KAnim.PlayMode.Once).OnAnimQueueComplete(alive.grown.idle);
			alive.grown.idle.PlayAnim("idle_bloom_loop", KAnim.PlayMode.Loop);
			alive.wilting.pre.DefaultState(alive.wilting.pre).PlayAnim("wilt_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(alive.wilting.idle)
				.EventTransition(GameHashes.WiltRecover, alive.wilting.pst, (StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
			alive.wilting.idle.PlayAnim("idle_wilt_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.WiltRecover, alive.wilting.pst, (StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
			alive.wilting.pst.PlayAnim("wilt_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(alive.idle);
		}
	}

	[MyCmpReq]
	private ReceptacleMonitor rm;

	[MyCmpReq]
	private Growing growing;

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private ElementEmitter elementEmitter;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	protected void DestroySelf(object callbackParam)
	{
		CreatureHelpers.DeselectCreature(base.gameObject);
		Util.KDestroyGameObject(base.gameObject);
	}
}
