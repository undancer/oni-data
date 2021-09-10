using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class CargoDropperMinion : GameStateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>
{
	public class Def : BaseDef
	{
		public Vector3 dropOffset;

		public string kAnimName;

		public string animName;

		public Grid.SceneLayer animLayer = Grid.SceneLayer.Move;

		public bool notifyOnJettison;
	}

	public class StatesInstance : GameInstance
	{
		public MinionIdentity escapingMinion;

		public Chore exitAnimChore;

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void JettisonCargo(object data = null)
		{
			Vector3 pos = base.master.transform.GetPosition() + base.def.dropOffset;
			MinionStorage component = GetComponent<MinionStorage>();
			if (!(component != null))
			{
				return;
			}
			List<MinionStorage.Info> storedMinionInfo = component.GetStoredMinionInfo();
			for (int num = storedMinionInfo.Count - 1; num >= 0; num--)
			{
				GameObject gameObject = component.DeserializeMinion(storedMinionInfo[num].id, pos);
				escapingMinion = gameObject.GetComponent<MinionIdentity>();
				gameObject.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
				ChoreProvider component2 = gameObject.GetComponent<ChoreProvider>();
				if (component2 != null)
				{
					exitAnimChore = new EmoteChore(component2, Db.Get().ChoreTypes.EmoteHighPriority, base.def.kAnimName, new HashedString[1] { base.def.animName }, KAnim.PlayMode.Once);
					Vector3 position = gameObject.transform.GetPosition();
					position.z = Grid.GetLayerZ(base.def.animLayer);
					gameObject.transform.SetPosition(position);
				}
				if (base.def.notifyOnJettison)
				{
					gameObject.GetComponent<Notifier>().Add(CreateCrashLandedNotification());
				}
			}
		}

		public bool SyncMinionExitAnimation()
		{
			if (escapingMinion != null && exitAnimChore != null && !exitAnimChore.isComplete)
			{
				KBatchedAnimController component = escapingMinion.GetComponent<KBatchedAnimController>();
				KBatchedAnimController component2 = base.master.GetComponent<KBatchedAnimController>();
				if (component2.CurrentAnim.name == base.def.animName)
				{
					component.SetElapsedTime(component2.GetElapsedTime());
					return true;
				}
			}
			return false;
		}

		public Notification CreateCrashLandedNotification()
		{
			return new Notification(MISC.NOTIFICATIONS.DUPLICANT_CRASH_LANDED.NAME, NotificationType.Bad, (List<Notification> notificationList, object data) => string.Concat(MISC.NOTIFICATIONS.DUPLICANT_CRASH_LANDED.TOOLTIP, notificationList.ReduceMessages(countNames: false)));
		}
	}

	private State notLanded;

	private State landed;

	private State exiting;

	private State complete;

	public BoolParameter hasLanded = new BoolParameter(default_value: false);

	public override void InitializeStates(out BaseState default_state)
	{
		base.serializable = SerializeType.ParamsOnly;
		default_state = notLanded;
		root.ParamTransition(hasLanded, complete, GameStateMachine<CargoDropperMinion, StatesInstance, IStateMachineTarget, Def>.IsTrue);
		notLanded.EventHandlerTransition(GameHashes.JettisonCargo, landed, (StatesInstance smi, object obj) => true);
		landed.Enter(delegate(StatesInstance smi)
		{
			smi.JettisonCargo();
			smi.GoTo(exiting);
		});
		exiting.Update(delegate(StatesInstance smi, float dt)
		{
			if (!smi.SyncMinionExitAnimation())
			{
				smi.GoTo(complete);
			}
		});
		complete.Enter(delegate(StatesInstance smi)
		{
			hasLanded.Set(value: true, smi);
		});
	}
}
