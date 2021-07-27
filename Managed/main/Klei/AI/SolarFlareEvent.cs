using STRINGS;

namespace Klei.AI
{
	public class SolarFlareEvent : GameplayEvent<SolarFlareEvent.StatesInstance>
	{
		public class StatesInstance : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, SolarFlareEvent>.GameplayEventStateMachineInstance
		{
			public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, SolarFlareEvent solarFlareEvent)
				: base(master, eventInstance, solarFlareEvent)
			{
			}
		}

		public class States : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, SolarFlareEvent>
		{
			public State idle;

			public State start;

			public State finished;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = idle;
				base.serializable = SerializeType.Both_DEPRECATED;
				idle.DoNothing();
				start.ScheduleGoTo(7f, finished);
				finished.ReturnSuccess();
			}

			public override GameplayEventPopupData GenerateEventPopupData(StatesInstance smi)
			{
				return new GameplayEventPopupData(smi.gameplayEvent)
				{
					location = GAMEPLAY_EVENTS.LOCATIONS.SUN,
					whenDescription = GAMEPLAY_EVENTS.TIMES.NOW
				};
			}
		}

		public const string ID = "SolarFlareEvent";

		public const float DURATION = 7f;

		public SolarFlareEvent()
			: base("SolarFlareEvent", 0, 0)
		{
			popupTitle = GAMEPLAY_EVENTS.EVENT_TYPES.SOLAR_FLARE.NAME;
			popupDescription = GAMEPLAY_EVENTS.EVENT_TYPES.SOLAR_FLARE.DESCRIPTION;
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new StatesInstance(manager, eventInstance, this);
		}
	}
}
