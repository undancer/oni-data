using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class VignetteManager : GameStateMachine<VignetteManager, VignetteManager.Instance>
{
	public class OnStates : State
	{
		public State yellow;

		public State red;
	}

	public new class Instance : GameInstance
	{
		private static Instance instance;

		private bool isToggled;

		private bool hasTopPriorityChore;

		public Notification redAlertNotification = new Notification(MISC.NOTIFICATIONS.REDALERT.NAME, NotificationType.Bad, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.REDALERT.TOOLTIP, null, expires: false);

		public static void DestroyInstance()
		{
			instance = null;
		}

		public static Instance Get()
		{
			return instance;
		}

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			instance = this;
		}

		public void UpdateState(float dt)
		{
			if (IsRedAlert())
			{
				base.smi.GoTo(base.sm.on.red);
			}
			else if (IsYellowAlert())
			{
				base.smi.GoTo(base.sm.on.yellow);
			}
			else if (!IsOn())
			{
				base.smi.GoTo(base.sm.off);
			}
		}

		public bool IsOn()
		{
			if (!base.sm.isYellowAlert.Get(base.smi))
			{
				return base.sm.isRedAlert.Get(base.smi);
			}
			return true;
		}

		public bool IsRedAlert()
		{
			return base.sm.isRedAlert.Get(base.smi);
		}

		public bool IsYellowAlert()
		{
			return base.sm.isYellowAlert.Get(base.smi);
		}

		public bool IsRedAlertToggledOn()
		{
			return isToggled;
		}

		public void ToggleRedAlert(bool on)
		{
			isToggled = on;
			Refresh();
		}

		public void HasTopPriorityChore(bool on)
		{
			hasTopPriorityChore = on;
			Refresh();
		}

		private void Refresh()
		{
			base.sm.isYellowAlert.Set(hasTopPriorityChore, base.smi);
			base.sm.isRedAlert.Set(isToggled, base.smi);
			base.sm.isOn.Set(hasTopPriorityChore || isToggled, base.smi);
		}
	}

	public State off;

	public OnStates on;

	public BoolParameter isRedAlert = new BoolParameter();

	public BoolParameter isYellowAlert = new BoolParameter();

	public BoolParameter isOn = new BoolParameter();

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		off.ParamTransition(isOn, on, GameStateMachine<VignetteManager, Instance, IStateMachineTarget, object>.IsTrue);
		on.Exit("VignetteOff", delegate
		{
			Vignette.Instance.Reset();
		}).ParamTransition(isRedAlert, on.red, (Instance smi, bool p) => isRedAlert.Get(smi)).ParamTransition(isRedAlert, on.yellow, (Instance smi, bool p) => isYellowAlert.Get(smi) && !isRedAlert.Get(smi))
			.ParamTransition(isYellowAlert, on.yellow, (Instance smi, bool p) => isYellowAlert.Get(smi) && !isRedAlert.Get(smi))
			.ParamTransition(isOn, off, GameStateMachine<VignetteManager, Instance, IStateMachineTarget, object>.IsFalse);
		on.red.Enter("EnterEvent", delegate
		{
			Game.Instance.Trigger(1585324898);
		}).Exit("ExitEvent", delegate
		{
			Game.Instance.Trigger(-1393151672);
		}).Enter("EnableVignette", delegate
		{
			Vignette.Instance.SetColor(new Color(1f, 0f, 0f, 0.3f));
		})
			.Enter("SoundsOnRedAlert", delegate
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_ON"));
			})
			.Exit("SoundsOffRedAlert", delegate
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_OFF"));
			})
			.ToggleLoopingSound(GlobalAssets.GetSound("RedAlert_LP"), null, pause_on_game_pause: true, enable_culling: false)
			.ToggleNotification((Instance smi) => smi.redAlertNotification);
		on.yellow.Enter("EnterEvent", delegate
		{
			Game.Instance.Trigger(-741654735);
		}).Exit("ExitEvent", delegate
		{
			Game.Instance.Trigger(-2062778933);
		}).Enter("EnableVignette", delegate
		{
			Vignette.Instance.SetColor(new Color(1f, 1f, 0f, 0.3f));
		})
			.Enter("SoundsOnYellowAlert", delegate
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("YellowAlert_ON"));
			})
			.Exit("SoundsOffRedAlert", delegate
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("YellowAlert_OFF"));
			})
			.ToggleLoopingSound(GlobalAssets.GetSound("YellowAlert_LP"), null, pause_on_game_pause: true, enable_culling: false);
	}
}
