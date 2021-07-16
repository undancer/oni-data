using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class FixedCapturePoint : GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>
{
	public class Def : BaseDef
	{
		public Func<GameObject, Instance, bool> isCreatureEligibleToBeCapturedCb;

		public Func<Instance, int> getTargetCapturePoint = delegate(Instance smi)
		{
			int num = Grid.PosToCell(smi);
			Navigator component = smi.targetCapturable.GetComponent<Navigator>();
			if (Grid.IsValidCell(num - 1) && component.CanReach(num - 1))
			{
				return num - 1;
			}
			return (Grid.IsValidCell(num + 1) && component.CanReach(num + 1)) ? (num + 1) : num;
		};
	}

	public class OperationalState : State
	{
		public State manual;

		public State automated;
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public new class Instance : GameInstance, ICheckboxControl
	{
		public FixedCapturableMonitor.Instance targetCapturable
		{
			get;
			private set;
		}

		public bool shouldCreatureGoGetCaptured
		{
			get;
			private set;
		}

		string ICheckboxControl.CheckboxTitleKey => UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.TITLE.key.String;

		string ICheckboxControl.CheckboxLabel => UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.AUTOWRANGLE;

		string ICheckboxControl.CheckboxTooltip => UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.AUTOWRANGLE_TOOLTIP;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			Subscribe(-905833192, OnCopySettings);
		}

		private void OnCopySettings(object data)
		{
			GameObject gameObject = (GameObject)data;
			if (!(gameObject == null))
			{
				Instance sMI = gameObject.GetSMI<Instance>();
				if (sMI != null)
				{
					base.sm.automated.Set(base.sm.automated.Get(sMI), this);
				}
			}
		}

		public Chore CreateChore()
		{
			FindFixedCapturable();
			return new FixedCaptureChore(GetComponent<KPrefabID>());
		}

		public bool IsCreatureAvailableForFixedCapture()
		{
			if (!targetCapturable.IsNullOrStopped())
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
				return CanCapturableBeCapturedAtCapturePoint(targetCapturable, this, cavityForCell, num);
			}
			return false;
		}

		public void SetRancherIsAvailableForCapturing()
		{
			shouldCreatureGoGetCaptured = true;
		}

		public void ClearRancherIsAvailableForCapturing()
		{
			shouldCreatureGoGetCaptured = false;
		}

		private static bool CanCapturableBeCapturedAtCapturePoint(FixedCapturableMonitor.Instance capturable, Instance capture_point, CavityInfo capture_cavity_info, int capture_cell)
		{
			if (!capturable.IsRunning())
			{
				return false;
			}
			if (capturable.targetCapturePoint != capture_point && !capturable.targetCapturePoint.IsNullOrStopped())
			{
				return false;
			}
			if (capture_point.def.isCreatureEligibleToBeCapturedCb != null && !capture_point.def.isCreatureEligibleToBeCapturedCb(capturable.gameObject, capture_point))
			{
				return false;
			}
			int cell = Grid.PosToCell(capturable.transform.GetPosition());
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell);
			if (cavityForCell == null || cavityForCell != capture_cavity_info)
			{
				return false;
			}
			if (capturable.HasTag(GameTags.Creatures.Bagged))
			{
				return false;
			}
			if (!capturable.GetComponent<ChoreConsumer>().IsChoreEqualOrAboveCurrentChorePriority<FixedCaptureStates>())
			{
				return false;
			}
			if (capturable.GetComponent<Navigator>().GetNavigationCost(capture_cell) == -1)
			{
				return false;
			}
			TreeFilterable component = capture_point.GetComponent<TreeFilterable>();
			IUserControlledCapacity component2 = capture_point.GetComponent<IUserControlledCapacity>();
			if (component.ContainsTag(capturable.GetComponent<KPrefabID>().PrefabTag) && component2.AmountStored <= component2.UserMaxCapacity)
			{
				return false;
			}
			return true;
		}

		public void FindFixedCapturable()
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
			if (cavityForCell == null)
			{
				ResetCapturePoint();
				return;
			}
			if (!targetCapturable.IsNullOrStopped() && !CanCapturableBeCapturedAtCapturePoint(targetCapturable, this, cavityForCell, num))
			{
				ResetCapturePoint();
			}
			if (!targetCapturable.IsNullOrStopped())
			{
				return;
			}
			foreach (FixedCapturableMonitor.Instance fixedCapturableMonitor in Components.FixedCapturableMonitors)
			{
				if (CanCapturableBeCapturedAtCapturePoint(fixedCapturableMonitor, this, cavityForCell, num))
				{
					targetCapturable = fixedCapturableMonitor;
					if (!targetCapturable.IsNullOrStopped())
					{
						targetCapturable.targetCapturePoint = this;
					}
					break;
				}
			}
		}

		public void ResetCapturePoint()
		{
			Trigger(643180843);
			if (!targetCapturable.IsNullOrStopped())
			{
				targetCapturable.targetCapturePoint = null;
				targetCapturable.Trigger(1034952693);
				targetCapturable = null;
			}
		}

		bool ICheckboxControl.GetCheckboxValue()
		{
			return base.sm.automated.Get(this);
		}

		void ICheckboxControl.SetCheckboxValue(bool value)
		{
			base.sm.automated.Set(value, this);
		}
	}

	private BoolParameter automated;

	public State unoperational;

	public OperationalState operational;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = operational;
		base.serializable = SerializeType.Both_DEPRECATED;
		unoperational.TagTransition(GameTags.Operational, operational);
		operational.DefaultState(operational.manual).TagTransition(GameTags.Operational, unoperational, on_remove: true);
		operational.manual.ParamTransition(automated, operational.automated, GameStateMachine<FixedCapturePoint, Instance, IStateMachineTarget, Def>.IsTrue);
		operational.automated.ParamTransition(automated, operational.manual, GameStateMachine<FixedCapturePoint, Instance, IStateMachineTarget, Def>.IsFalse).ToggleChore((Instance smi) => smi.CreateChore(), unoperational, unoperational).Update("FindFixedCapturable", delegate(Instance smi, float dt)
		{
			smi.FindFixedCapturable();
		}, UpdateRate.SIM_1000ms);
	}
}
