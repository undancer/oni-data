using STRINGS;

namespace Klei.AI
{
	public class EclipseEvent : GameplayEvent<EclipseEvent.StatesInstance>
	{
		public class StatesInstance : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, EclipseEvent>.GameplayEventStateMachineInstance
		{
			public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, EclipseEvent eclipseEvent)
				: base(master, eventInstance, eclipseEvent)
			{
			}
		}

		public class States : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, EclipseEvent>
		{
			public State planning;

			public State eclipse;

			public State finished;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = planning;
				base.serializable = SerializeType.Both_DEPRECATED;
				planning.GoTo(eclipse);
				eclipse.ToggleNotification((StatesInstance smi) => GameplayEventInstance.CreateStandardEventNotification(GenerateEventPopupData(smi))).Enter(delegate
				{
					TimeOfDay.Instance.SetEclipse(eclipse: true);
				}).Exit(delegate
				{
					TimeOfDay.Instance.SetEclipse(eclipse: false);
				})
					.ScheduleGoTo(30f, finished);
				finished.ReturnSuccess();
			}

			public override GameplayEventPopupData GenerateEventPopupData(StatesInstance smi)
			{
				GameplayEventPopupData gameplayEventPopupData = new GameplayEventPopupData(smi.gameplayEvent);
				gameplayEventPopupData.location = GAMEPLAY_EVENTS.LOCATIONS.SUN;
				gameplayEventPopupData.whenDescription = GAMEPLAY_EVENTS.TIMES.NOW;
				return gameplayEventPopupData;
			}
		}

		public const string ID = "EclipseEvent";

		public const float duration = 30f;

		public EclipseEvent()
			: base("EclipseEvent", 0, 0)
		{
			popupTitle = GAMEPLAY_EVENTS.EVENT_TYPES.ECLIPSE.NAME;
			popupDescription = GAMEPLAY_EVENTS.EVENT_TYPES.ECLIPSE.DESCRIPTION;
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new StatesInstance(manager, eventInstance, this);
		}
	}
}
