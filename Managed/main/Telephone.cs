using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Telephone : StateMachineComponent<Telephone.StatesInstance>, IGameObjectEffectDescriptor
{
	public class States : GameStateMachine<States, StatesInstance, Telephone>
	{
		public class ReadyStates : State
		{
			public class CallingStates : State
			{
				public State dial;

				public State animHack;

				public State pre;

				public State wait;
			}

			public class TalkingStates : State
			{
				public State babbling;

				public State chatting;
			}

			public State idle;

			public State ringing;

			public State answer;

			public State speaker;

			public State hangup;

			public CallingStates calling;

			public TalkingStates talking;
		}

		private State unoperational;

		private ReadyStates ready;

		private static StatusItem partyLine;

		private static StatusItem babbling;

		public override void InitializeStates(out BaseState default_state)
		{
			CreateStatusItems();
			default_state = unoperational;
			unoperational.PlayAnim("off").TagTransition(GameTags.Operational, ready);
			ready.TagTransition(GameTags.Operational, unoperational, on_remove: true).DefaultState(ready.idle).ToggleRecurringChore(CreateChore)
				.Enter(delegate(StatesInstance smi)
				{
					foreach (Telephone item in Components.Telephones.Items)
					{
						if (item.isInUse)
						{
							smi.GoTo(ready.speaker);
						}
					}
				});
			ready.idle.WorkableStartTransition((StatesInstance smi) => smi.master.GetComponent<TelephoneCallerWorkable>(), ready.calling.dial).TagTransition(GameTags.TelephoneRinging, ready.ringing).PlayAnim("off");
			ready.calling.ScheduleGoTo(15f, ready.talking.babbling);
			ready.calling.dial.PlayAnim("on_pre").OnAnimQueueComplete(ready.calling.animHack);
			ready.calling.animHack.ScheduleActionNextFrame("animHack_delay", delegate(StatesInstance smi)
			{
				smi.GoTo(ready.calling.pre);
			});
			ready.calling.pre.PlayAnim("on").Enter(delegate(StatesInstance smi)
			{
				RingAllTelephones(smi);
			}).OnAnimQueueComplete(ready.calling.wait);
			ready.calling.wait.PlayAnim("on", KAnim.PlayMode.Loop).Transition(ready.talking.chatting, (StatesInstance smi) => smi.CallAnswered(), UpdateRate.SIM_4000ms);
			ready.ringing.PlayAnim("on_receiving", KAnim.PlayMode.Loop).Transition(ready.answer, (StatesInstance smi) => smi.GetComponent<Telephone>().isInUse, UpdateRate.SIM_33ms).TagTransition(GameTags.TelephoneRinging, ready.speaker, on_remove: true)
				.ScheduleGoTo(15f, ready.speaker)
				.Exit(delegate(StatesInstance smi)
				{
					smi.GetComponent<Telephone>().RemoveTag(GameTags.TelephoneRinging);
				});
			ready.answer.PlayAnim("on_pre_loop_receiving").OnAnimQueueComplete(ready.talking.chatting);
			ready.talking.ScheduleGoTo(25f, ready.hangup).Enter(delegate(StatesInstance smi)
			{
				UpdatePartyLine(smi);
			});
			ready.talking.babbling.PlayAnim("on_loop", KAnim.PlayMode.Loop).Transition(ready.talking.chatting, (StatesInstance smi) => smi.CallAnswered(), UpdateRate.SIM_33ms).ToggleStatusItem(babbling);
			ready.talking.chatting.PlayAnim("on_loop_pre").QueueAnim("on_loop", loop: true).Transition(ready.talking.babbling, (StatesInstance smi) => !smi.CallAnswered(), UpdateRate.SIM_33ms)
				.ToggleStatusItem(partyLine);
			ready.speaker.PlayAnim("on_loop_nobody", KAnim.PlayMode.Loop).Transition(ready, (StatesInstance smi) => !smi.CallAnswered(), UpdateRate.SIM_4000ms).Transition(ready.answer, (StatesInstance smi) => smi.GetComponent<Telephone>().isInUse, UpdateRate.SIM_33ms);
			ready.hangup.OnAnimQueueComplete(ready);
		}

		private Chore CreateChore(StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<TelephoneCallerWorkable>();
			WorkChore<TelephoneCallerWorkable> workChore = new WorkChore<TelephoneCallerWorkable>(Db.Get().ChoreTypes.Relax, component, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, Db.Get().ScheduleBlockTypes.Recreation, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.high);
			workChore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, component);
			return workChore;
		}

		public void UpdatePartyLine(StatesInstance smi)
		{
			int myWorldId = smi.GetMyWorldId();
			bool flag = false;
			foreach (Telephone item in Components.Telephones.Items)
			{
				item.RemoveTag(GameTags.TelephoneRinging);
				if (item.isInUse && myWorldId != item.GetMyWorldId())
				{
					flag = true;
					item.AddTag(GameTags.LongDistanceCall);
				}
			}
			Telephone component = smi.GetComponent<Telephone>();
			component.RemoveTag(GameTags.TelephoneRinging);
			if (flag)
			{
				component.AddTag(GameTags.LongDistanceCall);
			}
		}

		public void RingAllTelephones(StatesInstance smi)
		{
			Telephone component = smi.master.GetComponent<Telephone>();
			foreach (Telephone item in Components.Telephones.Items)
			{
				if (component != item && item.GetComponent<Operational>().IsOperational)
				{
					TelephoneCallerWorkable component2 = item.GetComponent<TelephoneCallerWorkable>();
					if (component2 != null && component2.worker == null)
					{
						item.AddTag(GameTags.TelephoneRinging);
					}
				}
			}
		}

		private static void CreateStatusItems()
		{
			if (partyLine == null)
			{
				partyLine = new StatusItem("PartyLine", BUILDING.STATUSITEMS.TELEPHONE.CONVERSATION.TALKING_TO, "", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
				partyLine.resolveStringCallback = delegate(string str, object obj)
				{
					Telephone component2 = ((StatesInstance)obj).GetComponent<Telephone>();
					int num = 0;
					foreach (Telephone item in Components.Telephones.Items)
					{
						if (item.isInUse && item != component2)
						{
							num++;
							if (num == 1)
							{
								str = str.Replace("{Asteroid}", item.GetMyWorld().GetProperName());
								str = str.Replace("{Duplicant}", item.GetComponent<TelephoneCallerWorkable>().worker.GetProperName());
							}
						}
					}
					if (num > 1)
					{
						str = string.Format(BUILDING.STATUSITEMS.TELEPHONE.CONVERSATION.TALKING_TO_NUM, num);
					}
					return str;
				};
				partyLine.resolveTooltipCallback = delegate(string str, object obj)
				{
					Telephone component = ((StatesInstance)obj).GetComponent<Telephone>();
					foreach (Telephone item2 in Components.Telephones.Items)
					{
						if (item2.isInUse && item2 != component)
						{
							string text = BUILDING.STATUSITEMS.TELEPHONE.CONVERSATION.TALKING_TO;
							text = text.Replace("{Duplicant}", item2.GetComponent<TelephoneCallerWorkable>().worker.GetProperName());
							text = text.Replace("{Asteroid}", item2.GetMyWorld().GetProperName());
							str = str + text + "\n";
						}
					}
					return str;
				};
			}
			if (babbling == null)
			{
				babbling = new StatusItem("Babbling", BUILDING.STATUSITEMS.TELEPHONE.BABBLE.NAME, BUILDING.STATUSITEMS.TELEPHONE.BABBLE.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
				babbling.resolveTooltipCallback = delegate(string str, object obj)
				{
					StatesInstance statesInstance = (StatesInstance)obj;
					str = str.Replace("{Duplicant}", statesInstance.GetComponent<TelephoneCallerWorkable>().worker.GetProperName());
					return str;
				};
			}
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, Telephone, object>.GameInstance
	{
		public StatesInstance(Telephone smi)
			: base(smi)
		{
		}

		public bool CallAnswered()
		{
			foreach (Telephone item in Components.Telephones.Items)
			{
				if (item.isInUse && item != base.smi.GetComponent<Telephone>())
				{
					item.wasAnswered = true;
					return true;
				}
			}
			return false;
		}

		public bool CallEnded()
		{
			foreach (Telephone item in Components.Telephones.Items)
			{
				if (item.isInUse)
				{
					return false;
				}
			}
			return true;
		}
	}

	public string babbleEffect;

	public string chatEffect;

	public string longDistanceEffect;

	public string trackingEffect;

	public bool isInUse;

	public bool wasAnswered;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		Components.Telephones.Add(this);
		GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule);
		});
	}

	protected override void OnCleanUp()
	{
		Components.Telephones.Remove(this);
		base.OnCleanUp();
	}

	public void AddModifierDescriptions(List<Descriptor> descs, string effect_id)
	{
		Effect effect = Db.Get().effects.Get(effect_id);
		string text;
		string text2;
		if (effect.Id == babbleEffect)
		{
			text = BUILDINGS.PREFABS.TELEPHONE.EFFECT_BABBLE;
			text2 = BUILDINGS.PREFABS.TELEPHONE.EFFECT_BABBLE_TOOLTIP;
		}
		else if (effect.Id == chatEffect)
		{
			text = BUILDINGS.PREFABS.TELEPHONE.EFFECT_CHAT;
			text2 = BUILDINGS.PREFABS.TELEPHONE.EFFECT_CHAT_TOOLTIP;
		}
		else
		{
			text = BUILDINGS.PREFABS.TELEPHONE.EFFECT_LONG_DISTANCE;
			text2 = BUILDINGS.PREFABS.TELEPHONE.EFFECT_LONG_DISTANCE_TOOLTIP;
		}
		foreach (AttributeModifier selfModifier in effect.SelfModifiers)
		{
			Descriptor item = new Descriptor(text.Replace("{attrib}", Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + selfModifier.AttributeId.ToUpper() + ".NAME")).Replace("{amount}", selfModifier.GetFormattedString()), text2.Replace("{attrib}", Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + selfModifier.AttributeId.ToUpper() + ".NAME")).Replace("{amount}", selfModifier.GetFormattedString()));
			item.IncreaseIndent();
			descs.Add(item);
		}
	}

	List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION);
		list.Add(item);
		AddModifierDescriptions(list, babbleEffect);
		AddModifierDescriptions(list, chatEffect);
		AddModifierDescriptions(list, longDistanceEffect);
		return list;
	}

	public void HangUp()
	{
		isInUse = false;
		wasAnswered = false;
		this.RemoveTag(GameTags.LongDistanceCall);
	}
}
