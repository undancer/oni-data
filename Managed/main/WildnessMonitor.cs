using Klei.AI;
using UnityEngine;

public class WildnessMonitor : GameStateMachine<WildnessMonitor, WildnessMonitor.Instance, IStateMachineTarget, WildnessMonitor.Def>
{
	public class Def : BaseDef
	{
		public Effect wildEffect;

		public Effect tameEffect;

		public override void Configure(GameObject prefab)
		{
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Wildness.Id);
		}
	}

	public new class Instance : GameInstance
	{
		public AmountInstance wildness;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			wildness = Db.Get().Amounts.Wildness.Lookup(base.gameObject);
			wildness.value = wildness.GetMax();
		}
	}

	public State wild;

	public State tame;

	private static readonly KAnimHashedString[] DOMESTICATION_SYMBOLS = new KAnimHashedString[2] { "tag", "snapto_tag" };

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = tame;
		base.serializable = SerializeType.Both_DEPRECATED;
		wild.Enter(RefreshAmounts).Enter(HideDomesticationSymbol).Transition(tame, (Instance smi) => !IsWild(smi), UpdateRate.SIM_1000ms)
			.ToggleEffect((Instance smi) => smi.def.wildEffect)
			.ToggleTag(GameTags.Creatures.Wild);
		tame.Enter(RefreshAmounts).Enter(ShowDomesticationSymbol).Transition(wild, IsWild, UpdateRate.SIM_1000ms)
			.ToggleEffect((Instance smi) => smi.def.tameEffect)
			.Enter(delegate(Instance smi)
			{
				SaveGame.Instance.GetComponent<ColonyAchievementTracker>().LogCritterTamed(smi.PrefabID());
			});
	}

	private static void HideDomesticationSymbol(Instance smi)
	{
		KAnimHashedString[] dOMESTICATION_SYMBOLS = DOMESTICATION_SYMBOLS;
		foreach (KAnimHashedString symbol in dOMESTICATION_SYMBOLS)
		{
			smi.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(symbol, is_visible: false);
		}
	}

	private static void ShowDomesticationSymbol(Instance smi)
	{
		KAnimHashedString[] dOMESTICATION_SYMBOLS = DOMESTICATION_SYMBOLS;
		foreach (KAnimHashedString symbol in dOMESTICATION_SYMBOLS)
		{
			smi.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(symbol, is_visible: true);
		}
	}

	private static bool IsWild(Instance smi)
	{
		return smi.wildness.value > 0f;
	}

	private static void RefreshAmounts(Instance smi)
	{
		bool flag = IsWild(smi);
		smi.wildness.hide = !flag;
		AttributeInstance attributeInstance = Db.Get().CritterAttributes.Happiness.Lookup(smi.gameObject);
		if (attributeInstance != null)
		{
			attributeInstance.hide = flag;
		}
		AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(smi.gameObject);
		if (amountInstance != null)
		{
			amountInstance.hide = flag;
		}
		AmountInstance amountInstance2 = Db.Get().Amounts.Temperature.Lookup(smi.gameObject);
		if (amountInstance2 != null)
		{
			amountInstance2.hide = flag;
		}
		AmountInstance amountInstance3 = Db.Get().Amounts.Fertility.Lookup(smi.gameObject);
		if (amountInstance3 != null)
		{
			amountInstance3.hide = flag;
		}
	}
}
