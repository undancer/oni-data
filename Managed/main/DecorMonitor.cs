using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class DecorMonitor : GameStateMachine<DecorMonitor, DecorMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		[Serialize]
		private float cycleTotalDecor;

		[Serialize]
		private float yesterdaysTotalDecor;

		private AmountInstance amount;

		private AttributeModifier modifier;

		private List<KeyValuePair<float, string>> effectLookup = new List<KeyValuePair<float, string>>
		{
			new KeyValuePair<float, string>(MAXIMUM_DECOR_VALUE * -0.25f, "DecorMinus1"),
			new KeyValuePair<float, string>(MAXIMUM_DECOR_VALUE * 0f, "Decor0"),
			new KeyValuePair<float, string>(MAXIMUM_DECOR_VALUE * 0.25f, "Decor1"),
			new KeyValuePair<float, string>(MAXIMUM_DECOR_VALUE * 0.5f, "Decor2"),
			new KeyValuePair<float, string>(MAXIMUM_DECOR_VALUE * 0.75f, "Decor3"),
			new KeyValuePair<float, string>(MAXIMUM_DECOR_VALUE, "Decor4"),
			new KeyValuePair<float, string>(float.MaxValue, "Decor5")
		};

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			cycleTotalDecor = 2250f;
			amount = Db.Get().Amounts.Decor.Lookup(base.gameObject);
			modifier = new AttributeModifier(Db.Get().Amounts.Decor.deltaAttribute.Id, 1f, DUPLICANTS.NEEDS.DECOR.OBSERVED_DECOR, is_multiplier: false, uiOnly: false, is_readonly: false);
		}

		public AttributeModifier GetDecorModifier()
		{
			return modifier;
		}

		public void Update(float dt)
		{
			int cell = Grid.PosToCell(base.gameObject);
			if (!Grid.IsValidCell(cell))
			{
				return;
			}
			float decorAtCell = GameUtil.GetDecorAtCell(cell);
			cycleTotalDecor += decorAtCell * dt;
			float value = 0f;
			float num = 4.1666665f;
			if (Mathf.Abs(decorAtCell - amount.value) > 0.5f)
			{
				if (decorAtCell > amount.value)
				{
					value = 3f * num;
				}
				else if (decorAtCell < amount.value)
				{
					value = 0f - num;
				}
			}
			else
			{
				amount.value = decorAtCell;
			}
			modifier.SetValue(value);
		}

		public void OnNewDay()
		{
			yesterdaysTotalDecor = cycleTotalDecor;
			cycleTotalDecor = 0f;
			float totalValue = base.gameObject.GetAttributes().Add(Db.Get().Attributes.DecorExpectation).GetTotalValue();
			float num = yesterdaysTotalDecor / 600f;
			num += totalValue;
			Effects component = base.gameObject.GetComponent<Effects>();
			foreach (KeyValuePair<float, string> item in effectLookup)
			{
				if (num < item.Key)
				{
					component.Add(item.Value, should_save: true);
					break;
				}
			}
		}

		public float GetTodaysAverageDecor()
		{
			return cycleTotalDecor / (GameClock.Instance.GetCurrentCycleAsPercentage() * 600f);
		}

		public float GetYesterdaysAverageDecor()
		{
			return yesterdaysTotalDecor / 600f;
		}
	}

	public static float MAXIMUM_DECOR_VALUE = 120f;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleAttributeModifier("DecorSmoother", (Instance smi) => smi.GetDecorModifier(), (Instance smi) => true).Update("DecorSensing", delegate(Instance smi, float dt)
		{
			smi.Update(dt);
		}).EventHandler(GameHashes.NewDay, (Instance smi) => GameClock.Instance, delegate(Instance smi)
		{
			smi.OnNewDay();
		});
	}
}
