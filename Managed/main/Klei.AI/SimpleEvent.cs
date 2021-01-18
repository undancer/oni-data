using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class SimpleEvent : GameplayEvent<SimpleEvent.StatesInstance>
	{
		public class States : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, SimpleEvent>
		{
			public State ending;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = root;
				ending.ReturnSuccess();
			}

			public override GameplayEventPopupData GenerateEventPopupData(StatesInstance smi)
			{
				GameplayEventPopupData gameplayEventPopupData = new GameplayEventPopupData(smi.gameplayEvent);
				gameplayEventPopupData.minions = smi.minions;
				GameplayEventPopupData.PopupOption popupOption = gameplayEventPopupData.AddOption(smi.gameplayEvent.popupButtonText);
				popupOption.callback = delegate
				{
					if (smi.callback != null)
					{
						smi.callback();
					}
					smi.StopSM("SimpleEvent Finished");
				};
				popupOption.tooltip = smi.gameplayEvent.popupButtonTooltip;
				if (smi.textParameters != null)
				{
					foreach (Tuple<string, string> textParameter in smi.textParameters)
					{
						gameplayEventPopupData.SetTextParameter(textParameter.first, textParameter.second);
					}
				}
				return gameplayEventPopupData;
			}
		}

		public class StatesInstance : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, SimpleEvent>.GameplayEventStateMachineInstance
		{
			public GameObject[] minions;

			public List<Tuple<string, string>> textParameters;

			public System.Action callback;

			public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, SimpleEvent simpleEvent)
				: base(master, eventInstance, simpleEvent)
			{
			}

			public void SetTextParameter(string key, string value)
			{
				if (textParameters == null)
				{
					textParameters = new List<Tuple<string, string>>();
				}
				textParameters.Add(new Tuple<string, string>(key, value));
			}

			public void ShowEventPopup(object buttonCallBackData = null)
			{
				GameplayEventInstance.ShowEventPopup(base.smi.sm.GenerateEventPopupData(base.smi));
			}
		}

		private string popupButtonText;

		private string popupButtonTooltip;

		public SimpleEvent(string id, string title, string description, string buttonText = null, string buttonTooltip = null)
			: base(id, 0, 0)
		{
			popupTitle = title;
			popupDescription = description;
			popupButtonText = buttonText;
			popupButtonTooltip = buttonTooltip;
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new StatesInstance(manager, eventInstance, this);
		}
	}
}
