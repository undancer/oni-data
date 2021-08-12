using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/MessStation")]
public class MessStation : Workable, IGameObjectEffectDescriptor
{
	public class MessStationSM : GameStateMachine<MessStationSM, MessStationSM.Instance, MessStation>
	{
		public class SaltState : State
		{
			public State none;

			public State salty;
		}

		public new class Instance : GameInstance
		{
			private Storage saltStorage;

			private Assignable assigned;

			public bool HasSalt => saltStorage.Has(TableSaltConfig.ID.ToTag());

			public Instance(MessStation master)
				: base(master)
			{
				saltStorage = master.GetComponent<Storage>();
				assigned = master.GetComponent<Assignable>();
			}

			public bool IsEating()
			{
				if (assigned == null || assigned.assignee == null)
				{
					return false;
				}
				Ownables soleOwner = assigned.assignee.GetSoleOwner();
				if (soleOwner == null)
				{
					return false;
				}
				GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				if (targetGameObject == null)
				{
					return false;
				}
				ChoreDriver component = targetGameObject.GetComponent<ChoreDriver>();
				if (component != null && component.HasChore())
				{
					return component.GetCurrentChore().choreType.urge == Db.Get().Urges.Eat;
				}
				return false;
			}
		}

		public SaltState salt;

		public State eating;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = salt.none;
			salt.none.Transition(salt.salty, (Instance smi) => smi.HasSalt).PlayAnim("off");
			salt.salty.Transition(salt.none, (Instance smi) => !smi.HasSalt).PlayAnim("salt").EventTransition(GameHashes.EatStart, eating);
			eating.Transition(salt.salty, (Instance smi) => smi.HasSalt && !smi.IsEating()).Transition(salt.none, (Instance smi) => !smi.HasSalt && !smi.IsEating()).PlayAnim("off");
		}
	}

	private MessStationSM.Instance smi;

	public bool HasSalt => smi.HasSalt;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_use_machine_kanim") };
	}

	protected override void OnCompleteWork(Worker worker)
	{
		worker.workable.GetComponent<Edible>().CompleteWork(worker);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		smi = new MessStationSM.Instance(this);
		smi.StartSM();
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (go.GetComponent<Storage>().Has(TableSaltConfig.ID.ToTag()))
		{
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.MESS_TABLE_SALT, TableSaltTuning.MORALE_MODIFIER), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.MESS_TABLE_SALT, TableSaltTuning.MORALE_MODIFIER)));
		}
		return list;
	}
}
