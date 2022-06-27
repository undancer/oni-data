using Klei.AI;
using STRINGS;

public class OvercrowdingMonitor : GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>
{
	public class Def : BaseDef
	{
		public int spaceRequiredPerCreature;
	}

	public new class Instance : GameInstance
	{
		public CavityInfo cavity;

		public bool isBaby;

		public bool isFish;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			BabyMonitor.Def def2 = master.gameObject.GetDef<BabyMonitor.Def>();
			isBaby = def2 != null;
			FishOvercrowdingMonitor.Def def3 = master.gameObject.GetDef<FishOvercrowdingMonitor.Def>();
			isFish = def3 != null;
			UpdateState(this, 0f);
		}

		protected override void OnCleanUp()
		{
			KPrefabID component = base.master.GetComponent<KPrefabID>();
			if (cavity != null)
			{
				if (HasTag(GameTags.Egg))
				{
					cavity.RemoveFromCavity(component, cavity.eggs);
				}
				else
				{
					cavity.RemoveFromCavity(component, cavity.creatures);
				}
			}
		}

		public void RoomRefreshUpdateCavity()
		{
			UpdateState(this, 0f);
		}
	}

	public const float OVERCROWDED_FERTILITY_DEBUFF = -1f;

	public static Effect futureOvercrowdedEffect;

	public static Effect overcrowdedEffect;

	public static Effect fishOvercrowdedEffect;

	public static Effect stuckEffect;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.Update(UpdateState, UpdateRate.SIM_1000ms, load_balance: true);
		futureOvercrowdedEffect = new Effect("FutureOvercrowded", CREATURES.MODIFIERS.FUTURE_OVERCROWDED.NAME, CREATURES.MODIFIERS.FUTURE_OVERCROWDED.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: true);
		futureOvercrowdedEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, -1f, CREATURES.MODIFIERS.FUTURE_OVERCROWDED.NAME, is_multiplier: true));
		overcrowdedEffect = new Effect("Overcrowded", CREATURES.MODIFIERS.OVERCROWDED.NAME, CREATURES.MODIFIERS.OVERCROWDED.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: true);
		overcrowdedEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -5f, CREATURES.MODIFIERS.OVERCROWDED.NAME));
		fishOvercrowdedEffect = new Effect("Overcrowded", CREATURES.MODIFIERS.OVERCROWDED.NAME, CREATURES.MODIFIERS.OVERCROWDED.FISHTOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: true);
		fishOvercrowdedEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -5f, CREATURES.MODIFIERS.OVERCROWDED.NAME));
		stuckEffect = new Effect("Confined", CREATURES.MODIFIERS.CONFINED.NAME, CREATURES.MODIFIERS.CONFINED.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: true);
		stuckEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -10f, CREATURES.MODIFIERS.CONFINED.NAME));
	}

	private static bool IsConfined(Instance smi)
	{
		if (smi.HasTag(GameTags.Creatures.Burrowed))
		{
			return false;
		}
		if (smi.HasTag(GameTags.Creatures.Digger))
		{
			return false;
		}
		if (smi.cavity == null)
		{
			return true;
		}
		if (smi.cavity.numCells < smi.def.spaceRequiredPerCreature)
		{
			return true;
		}
		return false;
	}

	private static bool IsFutureOvercrowded(Instance smi)
	{
		if (smi.def.spaceRequiredPerCreature == 0)
		{
			return false;
		}
		if (smi.cavity != null)
		{
			int num = smi.cavity.creatures.Count + smi.cavity.eggs.Count;
			if (num == 0 || smi.cavity.eggs.Count == 0)
			{
				return false;
			}
			return smi.cavity.numCells / num < smi.def.spaceRequiredPerCreature;
		}
		return false;
	}

	private static bool IsOvercrowded(Instance smi)
	{
		if (smi.def.spaceRequiredPerCreature == 0)
		{
			return false;
		}
		FishOvercrowdingMonitor.Instance sMI = smi.GetSMI<FishOvercrowdingMonitor.Instance>();
		if (sMI != null)
		{
			int fishCount = sMI.fishCount;
			if (fishCount > 0)
			{
				return sMI.cellCount / fishCount < smi.def.spaceRequiredPerCreature;
			}
			return !Grid.IsLiquid(Grid.PosToCell(smi));
		}
		if (smi.cavity != null && smi.cavity.creatures.Count > 1)
		{
			return smi.cavity.numCells / smi.cavity.creatures.Count < smi.def.spaceRequiredPerCreature;
		}
		return false;
	}

	private static void UpdateState(Instance smi, float dt)
	{
		UpdateCavity(smi, dt);
		bool flag = IsConfined(smi);
		bool flag2 = IsOvercrowded(smi);
		bool flag3 = !smi.isBaby && IsFutureOvercrowded(smi);
		KPrefabID component = smi.gameObject.GetComponent<KPrefabID>();
		component.SetTag(GameTags.Creatures.Confined, flag);
		component.SetTag(GameTags.Creatures.Overcrowded, flag2);
		component.SetTag(GameTags.Creatures.Expecting, flag3);
		SetEffect(smi, stuckEffect, flag);
		Effect effect = (smi.isFish ? fishOvercrowdedEffect : overcrowdedEffect);
		SetEffect(smi, effect, !flag && flag2);
		SetEffect(smi, futureOvercrowdedEffect, !flag && flag3);
	}

	private static void SetEffect(Instance smi, Effect effect, bool set)
	{
		Effects component = smi.GetComponent<Effects>();
		if (set)
		{
			component.Add(effect, should_save: false);
		}
		else
		{
			component.Remove(effect);
		}
	}

	private static void UpdateCavity(Instance smi, float dt)
	{
		CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(smi));
		if (cavityForCell == smi.cavity)
		{
			return;
		}
		KPrefabID component = smi.GetComponent<KPrefabID>();
		if (smi.cavity != null)
		{
			if (smi.HasTag(GameTags.Egg))
			{
				smi.cavity.RemoveFromCavity(component, smi.cavity.eggs);
			}
			else
			{
				smi.cavity.RemoveFromCavity(component, smi.cavity.creatures);
			}
			Game.Instance.roomProber.UpdateRoom(cavityForCell);
		}
		smi.cavity = cavityForCell;
		if (smi.cavity != null)
		{
			if (smi.HasTag(GameTags.Egg))
			{
				smi.cavity.eggs.Add(component);
			}
			else
			{
				smi.cavity.creatures.Add(component);
			}
			Game.Instance.roomProber.UpdateRoom(smi.cavity);
		}
	}
}
