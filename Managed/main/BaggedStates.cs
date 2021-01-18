using KSerialization;
using STRINGS;
using UnityEngine;

public class BaggedStates : GameStateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>
{
	public class Def : BaseDef
	{
		public float escapeTime = 300f;
	}

	public new class Instance : GameInstance
	{
		[Serialize]
		public float baggedTime = 0f;

		public static readonly Chore.Precondition IsBagged = new Chore.Precondition
		{
			id = "IsBagged",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.prefabid.HasTag(GameTags.Creatures.Bagged);
			}
		};

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(IsBagged);
		}

		public void UpdateFaller(bool bagged)
		{
			bool flag = bagged && !base.gameObject.HasTag(GameTags.Stored);
			bool flag2 = GameComps.Fallers.Has(base.gameObject);
			if (flag != flag2)
			{
				if (flag)
				{
					GameComps.Fallers.Add(base.gameObject, Vector2.zero);
				}
				else
				{
					GameComps.Fallers.Remove(base.gameObject);
				}
			}
		}
	}

	public State bagged;

	public State escape;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = bagged;
		base.serializable = SerializeType.Both_DEPRECATED;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.BAGGED.NAME, CREATURES.STATUSITEMS.BAGGED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		bagged.Enter(BagStart).ToggleTag(GameTags.Creatures.Deliverable).PlayAnim("trussed", KAnim.PlayMode.Loop)
			.TagTransition(GameTags.Creatures.Bagged, null, on_remove: true)
			.Transition(escape, ShouldEscape, UpdateRate.SIM_4000ms)
			.EventHandler(GameHashes.OnStore, OnStore)
			.Exit(BagEnd);
		escape.Enter(Unbag).PlayAnim("escape").OnAnimQueueComplete(null);
	}

	private static void BagStart(Instance smi)
	{
		if (smi.baggedTime == 0f)
		{
			smi.baggedTime = GameClock.Instance.GetTime();
		}
		smi.UpdateFaller(bagged: true);
	}

	private static void BagEnd(Instance smi)
	{
		smi.baggedTime = 0f;
		smi.UpdateFaller(bagged: false);
	}

	private static void Unbag(Instance smi)
	{
		Baggable component = smi.gameObject.GetComponent<Baggable>();
		if ((bool)component)
		{
			component.Free();
		}
	}

	private static void OnStore(Instance smi)
	{
		smi.UpdateFaller(bagged: true);
	}

	private static bool ShouldEscape(Instance smi)
	{
		if (smi.gameObject.HasTag(GameTags.Stored))
		{
			return false;
		}
		float num = GameClock.Instance.GetTime() - smi.baggedTime;
		if (num < smi.def.escapeTime)
		{
			return false;
		}
		return true;
	}
}
