using System;
using System.Collections.Generic;
using Klei.AI;
using Klei.CustomSettings;
using KSerialization;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class QualityOfLifeNeed : Need, ISim4000ms
{
	private AttributeInstance qolAttribute;

	public bool skipUpdate;

	[Serialize]
	private List<bool> breakBlocks;

	private static readonly EventSystem.IntraObjectHandler<QualityOfLifeNeed> OnScheduleBlocksTickDelegate = new EventSystem.IntraObjectHandler<QualityOfLifeNeed>(delegate(QualityOfLifeNeed component, object data)
	{
		component.OnScheduleBlocksTick(data);
	});

	private static List<string> breakLengthEffects = new List<string> { "Break1", "Break2", "Break3", "Break4", "Break5" };

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		breakBlocks = new List<bool>(24);
		Attributes attributes = base.gameObject.GetAttributes();
		expectationAttribute = attributes.Add(Db.Get().Attributes.QualityOfLifeExpectation);
		base.Name = DUPLICANTS.NEEDS.QUALITYOFLIFE.NAME;
		base.ExpectationTooltip = string.Format(DUPLICANTS.NEEDS.QUALITYOFLIFE.EXPECTATION_TOOLTIP, Db.Get().Attributes.QualityOfLifeExpectation.Lookup(this).GetTotalValue());
		stressBonus = new ModifierType
		{
			modifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0f, DUPLICANTS.NEEDS.QUALITYOFLIFE.GOOD_MODIFIER, is_multiplier: false, uiOnly: false, is_readonly: false)
		};
		stressNeutral = new ModifierType
		{
			modifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -0.008333334f, DUPLICANTS.NEEDS.QUALITYOFLIFE.NEUTRAL_MODIFIER)
		};
		stressPenalty = new ModifierType
		{
			modifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0f, DUPLICANTS.NEEDS.QUALITYOFLIFE.BAD_MODIFIER, is_multiplier: false, uiOnly: false, is_readonly: false),
			statusItem = Db.Get().DuplicantStatusItems.PoorQualityOfLife
		};
		qolAttribute = Db.Get().Attributes.QualityOfLife.Lookup(base.gameObject);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		while (breakBlocks.Count < 24)
		{
			breakBlocks.Add(item: false);
		}
		while (breakBlocks.Count > 24)
		{
			breakBlocks.RemoveAt(breakBlocks.Count - 1);
		}
		Subscribe(1714332666, OnScheduleBlocksTickDelegate);
	}

	public void Sim4000ms(float dt)
	{
		if (skipUpdate)
		{
			return;
		}
		float num = 0.004166667f;
		float b = 0.041666668f;
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Morale);
		if (currentQualitySetting.id == "Disabled")
		{
			SetModifier(null);
			return;
		}
		if (currentQualitySetting.id == "Easy")
		{
			num = 0.0033333334f;
			b = 0.016666668f;
		}
		else if (currentQualitySetting.id == "Hard")
		{
			num = 0.008333334f;
			b = 0.05f;
		}
		else if (currentQualitySetting.id == "VeryHard")
		{
			num = 0.016666668f;
			b = 0.083333336f;
		}
		float totalValue = qolAttribute.GetTotalValue();
		float totalValue2 = expectationAttribute.GetTotalValue();
		float num2 = totalValue2 - totalValue;
		if (totalValue < totalValue2)
		{
			stressPenalty.modifier.SetValue(Mathf.Min(num2 * num, b));
			SetModifier(stressPenalty);
		}
		else if (totalValue > totalValue2)
		{
			stressBonus.modifier.SetValue(Mathf.Max((0f - num2) * -0.016666668f, -71f / (678f * (float)Math.PI)));
			SetModifier(stressBonus);
		}
		else
		{
			SetModifier(stressNeutral);
		}
	}

	private void OnScheduleBlocksTick(object data)
	{
		Schedule obj = (Schedule)data;
		ScheduleBlock block = obj.GetBlock(Schedule.GetLastBlockIdx());
		ScheduleBlock block2 = obj.GetBlock(Schedule.GetBlockIdx());
		bool flag = block.IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);
		bool flag2 = block2.IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);
		breakBlocks[Schedule.GetLastBlockIdx()] = flag;
		if (!flag || flag2)
		{
			return;
		}
		int num = 0;
		foreach (bool breakBlock in breakBlocks)
		{
			if (breakBlock)
			{
				num++;
			}
		}
		ApplyBreakBonus(num);
	}

	private void ApplyBreakBonus(int numBlocks)
	{
		string breakBonus = GetBreakBonus(numBlocks);
		if (breakBonus != null)
		{
			GetComponent<Effects>().Add(breakBonus, should_save: true);
		}
	}

	public static string GetBreakBonus(int numBlocks)
	{
		int num = numBlocks - 1;
		if (num >= breakLengthEffects.Count)
		{
			return breakLengthEffects[breakLengthEffects.Count - 1];
		}
		if (num >= 0)
		{
			return breakLengthEffects[num];
		}
		return null;
	}
}
