using System;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class SatelliteCrashEvent : GameplayEvent<SatelliteCrashEvent.StatesInstance>
	{
		public class StatesInstance : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, SatelliteCrashEvent>.GameplayEventStateMachineInstance
		{
			public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, SatelliteCrashEvent crashEvent)
				: base(master, eventInstance, crashEvent)
			{
			}

			public Notification Plan()
			{
				GameObject spawn = Util.KInstantiate(position: new Vector3(Grid.WidthInCells / 2 + UnityEngine.Random.Range(-Grid.WidthInCells / 3, Grid.WidthInCells / 3), Grid.HeightInCells - 1, Grid.GetLayerZ(Grid.SceneLayer.FXFront)), original: Assets.GetPrefab(SatelliteCometConfig.ID));
				spawn.SetActive(value: true);
				Notification notification = GameplayEventInstance.CreateStandardEventNotification(base.smi.sm.GenerateEventPopupData(base.smi));
				notification.clickFocus = spawn.transform;
				Comet component = spawn.GetComponent<Comet>();
				component.OnImpact = (System.Action)Delegate.Combine(component.OnImpact, (System.Action)delegate
				{
					GameObject gameObject = new GameObject();
					gameObject.transform.position = spawn.transform.position;
					notification.clickFocus = gameObject.transform;
					GridVisibility.Reveal(Grid.PosToXY(gameObject.transform.position).x, Grid.PosToXY(gameObject.transform.position).y, 6, 4f);
				});
				return notification;
			}
		}

		public class States : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, SatelliteCrashEvent>
		{
			public State notify;

			public State ending;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = notify;
				notify.ToggleNotification((StatesInstance smi) => smi.Plan());
				ending.ReturnSuccess();
			}

			public override GameplayEventPopupData GenerateEventPopupData(StatesInstance smi)
			{
				GameplayEventPopupData gameplayEventPopupData = new GameplayEventPopupData(smi.gameplayEvent);
				gameplayEventPopupData.location = GAMEPLAY_EVENTS.LOCATIONS.SURFACE;
				gameplayEventPopupData.whenDescription = GAMEPLAY_EVENTS.TIMES.NOW;
				gameplayEventPopupData.AddDefaultOption(delegate
				{
					smi.GoTo(smi.sm.ending);
				});
				return gameplayEventPopupData;
			}
		}

		public SatelliteCrashEvent()
			: base("SatelliteCrash", 0, 0)
		{
			popupTitle = GAMEPLAY_EVENTS.EVENT_TYPES.SATELLITE_CRASH.NAME;
			popupDescription = GAMEPLAY_EVENTS.EVENT_TYPES.SATELLITE_CRASH.DESCRIPTION;
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new StatesInstance(manager, eventInstance, this);
		}
	}
}
