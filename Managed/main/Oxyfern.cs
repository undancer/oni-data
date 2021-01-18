using STRINGS;
using UnityEngine;

public class Oxyfern : StateMachineComponent<Oxyfern.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Oxyfern, object>.GameInstance
	{
		public StatesInstance(Oxyfern master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Oxyfern>
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
			}).PlayAnim("grow_pst", KAnim.PlayMode.Once).EventTransition(GameHashes.AnimQueueComplete, alive);
			alive.InitializeStates(masterTarget, dead).DefaultState(alive.mature);
			alive.mature.EventTransition(GameHashes.Wilt, alive.wilting, (StatesInstance smi) => smi.master.wiltCondition.IsWilting()).PlayAnim("idle_full", KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
			{
				smi.master.elementConsumer.EnableConsumption(enabled: true);
			})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.elementConsumer.EnableConsumption(enabled: false);
				});
			alive.wilting.PlayAnim("wilt3").EventTransition(GameHashes.WiltRecover, alive.mature, (StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
		}
	}

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private ElementConsumer elementConsumer;

	[MyCmpReq]
	private ElementConverter elementConverter;

	[MyCmpReq]
	private ReceptacleMonitor receptacleMonitor;

	private static readonly EventSystem.IntraObjectHandler<Oxyfern> OnUprootedDelegate = new EventSystem.IntraObjectHandler<Oxyfern>(delegate(Oxyfern component, object data)
	{
		component.OnUprooted(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Oxyfern> OnReplantedDelegate = new EventSystem.IntraObjectHandler<Oxyfern>(delegate(Oxyfern component, object data)
	{
		component.OnReplanted(data);
	});

	protected void DestroySelf(object callbackParam)
	{
		CreatureHelpers.DeselectCreature(base.gameObject);
		Util.KDestroyGameObject(base.gameObject);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-216549700, OnUprootedDelegate);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (Tutorial.Instance.oxygenGenerators.Contains(base.gameObject))
		{
			Tutorial.Instance.oxygenGenerators.Remove(base.gameObject);
		}
	}

	protected override void OnPrefabInit()
	{
		Subscribe(1309017699, OnReplantedDelegate);
		base.OnPrefabInit();
	}

	private void OnUprooted(object data = null)
	{
		GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), base.gameObject.transform.GetPosition(), Grid.SceneLayer.FXFront).SetActive(value: true);
		base.gameObject.Trigger(1623392196);
		base.gameObject.GetComponent<KBatchedAnimController>().StopAndClear();
		Object.Destroy(base.gameObject.GetComponent<KBatchedAnimController>());
		Util.KDestroyGameObject(base.gameObject);
	}

	private void OnReplanted(object data = null)
	{
		SetConsumptionRate();
		if (receptacleMonitor.Replanted)
		{
			Tutorial.Instance.oxygenGenerators.Add(base.gameObject);
		}
	}

	public void SetConsumptionRate()
	{
		if (receptacleMonitor.Replanted)
		{
			elementConsumer.consumptionRate = 0.00062500004f;
		}
		else
		{
			elementConsumer.consumptionRate = 0.00015625001f;
		}
	}
}
