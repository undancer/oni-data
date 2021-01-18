using STRINGS;

public class ProducePowerStates : GameStateMachine<ProducePowerStates, ProducePowerStates.Instance, IStateMachineTarget, ProducePowerStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToProducePower);
		}
	}

	public class SleepStates : State
	{
		public State connected;

		public State noConnection;
	}

	public class HasGeneratorStates : State
	{
		public State moveToSleepLocation;

		public SleepStates sleep;

		public State noConnectionWake;

		public State connectedWake;
	}

	public HasGeneratorStates generator;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = generator.moveToSleepLocation;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.SLEEPING.NAME, CREATURES.STATUSITEMS.SLEEPING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		generator.moveToSleepLocation.MoveTo(delegate(Instance smi)
		{
			ProducePowerMonitor.Instance sMI2 = smi.GetSMI<ProducePowerMonitor.Instance>();
			return sMI2.sm.targetSleepCell.Get(sMI2);
		}, generator.sleep, behaviourcomplete);
		generator.sleep.Enter(delegate(Instance smi)
		{
			if (smi.GetComponent<Staterpillar>().GetGenerator() == null)
			{
				smi.GoTo(behaviourcomplete);
			}
			else
			{
				smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Ceiling);
				smi.GetComponent<Staterpillar>().EnableGenerator();
				if (smi.GetComponent<Staterpillar>().IsConnected())
				{
					smi.GoTo(generator.sleep.connected);
				}
				else
				{
					smi.GoTo(generator.sleep.noConnection);
				}
			}
		}).Exit(delegate(Instance smi)
		{
			ProducePowerMonitor.Instance sMI = smi.GetSMI<ProducePowerMonitor.Instance>();
			sMI.sm.targetSleepCell.Set(Grid.InvalidCell, sMI);
			smi.GetComponent<Staterpillar>().DestroyGenerator();
		});
		generator.sleep.connected.Enter(delegate(Instance smi)
		{
			smi.Get<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.SolidConduitBridges);
		}).EventTransition(GameHashes.NewDay, (Instance smi) => GameClock.Instance, generator.connectedWake).Transition(generator.sleep.noConnection, (Instance smi) => smi.GetComponent<Staterpillar>().IsNotConnected())
			.PlayAnim("sleep_charging_pre")
			.QueueAnim("sleep_charging_loop", loop: true)
			.Exit(delegate(Instance smi)
			{
				smi.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
			});
		generator.sleep.noConnection.PlayAnim("sleep_pre").QueueAnim("sleep_loop", loop: true).ToggleStatusItem(Db.Get().BuildingStatusItems.NoWireConnected)
			.EventTransition(GameHashes.NewDay, (Instance smi) => GameClock.Instance, generator.noConnectionWake)
			.Transition(generator.sleep.connected, (Instance smi) => smi.GetComponent<Staterpillar>().IsConnected());
		generator.connectedWake.QueueAnim("sleep_charging_pst").OnAnimQueueComplete(behaviourcomplete);
		generator.noConnectionWake.QueueAnim("sleep_pst").OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToProducePower);
	}

	public static bool ShouldStop(Instance smi)
	{
		return !GameClock.Instance.IsNighttime();
	}
}
