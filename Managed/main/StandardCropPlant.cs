using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class StandardCropPlant : StateMachineComponent<StandardCropPlant.StatesInstance>
{
	public class AnimSet
	{
		public string grow;

		public string grow_pst;

		public string idle_full;

		public string wilt_base;

		public string harvest;
	}

	public class States : GameStateMachine<States, StatesInstance, StandardCropPlant>
	{
		public class AliveStates : PlantAliveSubState
		{
			public State idle;

			public State pre_fruiting;

			public State fruiting_lost;

			public State barren;

			public FruitingState fruiting;

			public State wilting;

			public State destroy;

			public State harvest;

			public State sleeping;
		}

		public class FruitingState : State
		{
			public State fruiting_idle;

			public State fruiting_old;
		}

		public AliveStates alive;

		public State dead;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = true;
			default_state = alive;
			dead.ToggleStatusItem(CREATURES.STATUSITEMS.DEAD.NAME, CREATURES.STATUSITEMS.DEAD.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).Enter(delegate(StatesInstance smi)
			{
				if (smi.master.rm.Replanted && !smi.master.GetComponent<KPrefabID>().HasTag(GameTags.Uprooted))
				{
					Notifier notifier = smi.master.gameObject.AddOrGet<Notifier>();
					Notification notification = smi.master.CreateDeathNotification();
					notifier.Add(notification);
				}
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront).SetActive(value: true);
				Harvestable component = smi.master.GetComponent<Harvestable>();
				if (component != null && component.CanBeHarvested && GameScheduler.Instance != null)
				{
					GameScheduler.Instance.Schedule("SpawnFruit", 0.2f, smi.master.crop.SpawnFruit);
				}
				smi.master.Trigger(1623392196);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, smi.master.DestroySelf);
			});
			alive.InitializeStates(masterTarget, dead).DefaultState(alive.idle).ToggleComponent<Growing>();
			alive.idle.EventTransition(GameHashes.Wilt, alive.wilting, (StatesInstance smi) => smi.master.wiltCondition.IsWilting()).EventTransition(GameHashes.Grow, alive.pre_fruiting, (StatesInstance smi) => smi.master.growing.ReachedNextHarvest()).EventTransition(GameHashes.CropSleep, alive.sleeping, IsSleeping)
				.PlayAnim((StatesInstance smi) => smi.master.anims.grow, KAnim.PlayMode.Paused)
				.Enter(RefreshPositionPercent)
				.Update(RefreshPositionPercent, UpdateRate.SIM_4000ms)
				.EventHandler(GameHashes.ConsumePlant, RefreshPositionPercent);
			alive.pre_fruiting.PlayAnim((StatesInstance smi) => smi.master.anims.grow_pst, KAnim.PlayMode.Once).TriggerOnEnter(GameHashes.BurstEmitDisease).EventTransition(GameHashes.AnimQueueComplete, alive.fruiting);
			alive.fruiting_lost.Enter(delegate(StatesInstance smi)
			{
				if (smi.master.harvestable != null)
				{
					smi.master.harvestable.SetCanBeHarvested(state: false);
				}
			}).GoTo(alive.idle);
			alive.wilting.PlayAnim(GetWiltAnim, KAnim.PlayMode.Loop).EventTransition(GameHashes.WiltRecover, alive.idle, (StatesInstance smi) => !smi.master.wiltCondition.IsWilting()).EventTransition(GameHashes.Harvest, alive.harvest);
			alive.sleeping.PlayAnim((StatesInstance smi) => smi.master.anims.grow, KAnim.PlayMode.Once).EventTransition(GameHashes.CropWakeUp, alive.idle, GameStateMachine<States, StatesInstance, StandardCropPlant, object>.Not(IsSleeping)).EventTransition(GameHashes.Harvest, alive.harvest)
				.EventTransition(GameHashes.Wilt, alive.wilting);
			alive.fruiting.DefaultState(alive.fruiting.fruiting_idle).EventTransition(GameHashes.Wilt, alive.wilting).EventTransition(GameHashes.Harvest, alive.harvest)
				.EventTransition(GameHashes.Grow, alive.fruiting_lost, (StatesInstance smi) => !smi.master.growing.ReachedNextHarvest());
			alive.fruiting.fruiting_idle.PlayAnim((StatesInstance smi) => smi.master.anims.idle_full, KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
			{
				if (smi.master.harvestable != null)
				{
					smi.master.harvestable.SetCanBeHarvested(state: true);
				}
			}).Transition(alive.fruiting.fruiting_old, IsOld, UpdateRate.SIM_4000ms);
			alive.fruiting.fruiting_old.PlayAnim(GetWiltAnim, KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
			{
				if (smi.master.harvestable != null)
				{
					smi.master.harvestable.SetCanBeHarvested(state: true);
				}
			}).Transition(alive.fruiting.fruiting_idle, GameStateMachine<States, StatesInstance, StandardCropPlant, object>.Not(IsOld), UpdateRate.SIM_4000ms);
			alive.harvest.PlayAnim((StatesInstance smi) => smi.master.anims.harvest, KAnim.PlayMode.Once).Enter(delegate(StatesInstance smi)
			{
				if (GameScheduler.Instance != null && smi.master != null)
				{
					GameScheduler.Instance.Schedule("SpawnFruit", 0.2f, smi.master.crop.SpawnFruit);
				}
				if (smi.master.harvestable != null)
				{
					smi.master.harvestable.SetCanBeHarvested(state: false);
				}
			}).OnAnimQueueComplete(alive.idle);
		}

		private static string GetWiltAnim(StatesInstance smi)
		{
			float num = smi.master.growing.PercentOfCurrentHarvest();
			string str = ((num < 0.75f) ? "1" : ((!(num < 1f)) ? "3" : "2"));
			return smi.master.anims.wilt_base + str;
		}

		private static void RefreshPositionPercent(StatesInstance smi, float dt)
		{
			smi.master.RefreshPositionPercent();
		}

		private static void RefreshPositionPercent(StatesInstance smi)
		{
			smi.master.RefreshPositionPercent();
		}

		public bool IsOld(StatesInstance smi)
		{
			return smi.master.growing.PercentOldAge() > 0.5f;
		}

		public bool IsSleeping(StatesInstance smi)
		{
			return smi.master.GetSMI<CropSleepingMonitor.Instance>()?.IsSleeping() ?? false;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, StandardCropPlant, object>.GameInstance
	{
		public StatesInstance(StandardCropPlant master)
			: base(master)
		{
		}
	}

	[MyCmpReq]
	private Crop crop;

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private ReceptacleMonitor rm;

	[MyCmpReq]
	private Growing growing;

	[MyCmpReq]
	private KAnimControllerBase animController;

	[MyCmpGet]
	private Harvestable harvestable;

	public static AnimSet defaultAnimSet = new AnimSet
	{
		grow = "grow",
		grow_pst = "grow_pst",
		idle_full = "idle_full",
		wilt_base = "wilt",
		harvest = "harvest"
	};

	public AnimSet anims = defaultAnimSet;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.Get<KBatchedAnimController>().randomiseLoopedOffset = true;
		base.smi.StartSM();
	}

	protected void DestroySelf(object callbackParam)
	{
		CreatureHelpers.DeselectCreature(base.gameObject);
		Util.KDestroyGameObject(base.gameObject);
	}

	public Notification CreateDeathNotification()
	{
		return new Notification(CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION, NotificationType.Bad, HashedString.Invalid, (List<Notification> notificationList, object data) => string.Concat(CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP, notificationList.ReduceMessages(countNames: false)), "/t• " + base.gameObject.GetProperName());
	}

	public void RefreshPositionPercent()
	{
		animController.SetPositionPercent(growing.PercentOfCurrentHarvest());
	}

	private static string ToolTipResolver(List<Notification> notificationList, object data)
	{
		string text = "";
		for (int i = 0; i < notificationList.Count; i++)
		{
			Notification notification = notificationList[i];
			text += (string)notification.tooltipData;
			if (i < notificationList.Count - 1)
			{
				text += "\n";
			}
		}
		return string.Format(CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP, text);
	}
}
