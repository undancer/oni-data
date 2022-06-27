using Klei.AI;
using STRINGS;
using UnityEngine;

public class HugEggStates : GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>
{
	public class AnimSet
	{
		public string pre;

		public string loop;

		public string pst;
	}

	public class Def : BaseDef
	{
		public float hugTime = 15f;

		public Tag behaviourTag;

		public AnimSet hugAnims = new AnimSet
		{
			pre = "hug_egg_pre",
			loop = "hug_egg_loop",
			pst = "hug_egg_pst"
		};

		public AnimSet incubatorHugAnims = new AnimSet
		{
			pre = "hug_incubator_pre",
			loop = "hug_incubator_loop",
			pst = "hug_incubator_pst"
		};

		public Def(Tag behaviourTag)
		{
			this.behaviourTag = behaviourTag;
		}
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, def.behaviourTag);
		}
	}

	public class HugState : State
	{
		public State pre;

		public State loop;

		public State pst;
	}

	public ApproachSubState<EggIncubator> moving;

	public HugState hug;

	public State behaviourcomplete;

	public TargetParameter target;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = moving;
		root.Enter(SetTarget).Enter(delegate(Instance smi)
		{
			if (!Reserve(smi))
			{
				smi.GoTo(behaviourcomplete);
			}
		}).Exit(Unreserve)
			.ToggleStatusItem(CREATURES.STATUSITEMS.HUGEGG.NAME, CREATURES.STATUSITEMS.HUGEGG.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main)
			.OnTargetLost(target, behaviourcomplete);
		moving.MoveTo(GetClimbableCell, hug, behaviourcomplete);
		hug.DefaultState(hug.pre).Enter(delegate(Instance smi)
		{
			smi.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Front);
		}).Exit(delegate(Instance smi)
		{
			smi.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
		});
		hug.pre.Face(target, 0.5f).PlayAnim((Instance smi) => GetAnims(smi).pre).OnAnimQueueComplete(hug.loop);
		hug.loop.QueueAnim((Instance smi) => GetAnims(smi).loop, loop: true).ScheduleGoTo((Instance smi) => smi.def.hugTime, hug.pst);
		hug.pst.QueueAnim((Instance smi) => GetAnims(smi).pst).Enter(ApplyEffect).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete((Instance smi) => smi.def.behaviourTag);
	}

	private static void SetTarget(Instance smi)
	{
		smi.sm.target.Set(smi.GetSMI<HugMonitor.Instance>().hugTarget, smi);
	}

	private static AnimSet GetAnims(Instance smi)
	{
		if (!(smi.sm.target.Get(smi).GetComponent<EggIncubator>() != null))
		{
			return smi.def.hugAnims;
		}
		return smi.def.incubatorHugAnims;
	}

	private static bool Reserve(Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null && !gameObject.HasTag(GameTags.Creatures.ReservedByCreature))
		{
			gameObject.AddTag(GameTags.Creatures.ReservedByCreature);
			return true;
		}
		return false;
	}

	private static void Unreserve(Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null)
		{
			gameObject.RemoveTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static int GetClimbableCell(Instance smi)
	{
		return Grid.PosToCell(smi.sm.target.Get(smi));
	}

	private static void ApplyEffect(Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null)
		{
			EggIncubator component = gameObject.GetComponent<EggIncubator>();
			if (component != null)
			{
				component.Occupant.GetComponent<Effects>().Add("EggHug", should_save: true);
			}
			else if (gameObject.HasTag(GameTags.Egg))
			{
				gameObject.GetComponent<Effects>().Add("EggHug", should_save: true);
			}
		}
	}
}
