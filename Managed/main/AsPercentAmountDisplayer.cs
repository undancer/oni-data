using Klei.AI;
using STRINGS;

public class AsPercentAmountDisplayer : IAmountDisplayer
{
	protected StandardAttributeFormatter formatter;

	public IAttributeFormatter Formatter => formatter;

	public GameUtil.TimeSlice DeltaTimeSlice
	{
		get
		{
			return formatter.DeltaTimeSlice;
		}
		set
		{
			formatter.DeltaTimeSlice = value;
		}
	}

	public AsPercentAmountDisplayer(GameUtil.TimeSlice deltaTimeSlice)
	{
		formatter = new StandardAttributeFormatter(GameUtil.UnitClass.Percent, deltaTimeSlice);
	}

	public string GetValueString(Amount master, AmountInstance instance)
	{
		return formatter.GetFormattedValue(ToPercent(instance.value, instance));
	}

	public virtual string GetDescription(Amount master, AmountInstance instance)
	{
		return $"{master.Name}: {formatter.GetFormattedValue(ToPercent(instance.value, instance))}";
	}

	public virtual string GetTooltipDescription(Amount master, AmountInstance instance)
	{
		return string.Format(master.description, formatter.GetFormattedValue(instance.value));
	}

	public virtual string GetTooltip(Amount master, AmountInstance instance)
	{
		string str = string.Format(master.description, formatter.GetFormattedValue(instance.value));
		str += "\n\n";
		str = ((formatter.DeltaTimeSlice != GameUtil.TimeSlice.PerCycle) ? (str + string.Format(UI.CHANGEPERSECOND, formatter.GetFormattedValue(ToPercent(instance.deltaAttribute.GetTotalDisplayValue(), instance), GameUtil.TimeSlice.PerSecond))) : (str + string.Format(UI.CHANGEPERCYCLE, formatter.GetFormattedValue(ToPercent(instance.deltaAttribute.GetTotalDisplayValue(), instance), GameUtil.TimeSlice.PerCycle))));
		for (int i = 0; i != instance.deltaAttribute.Modifiers.Count; i++)
		{
			AttributeModifier attributeModifier = instance.deltaAttribute.Modifiers[i];
			float modifierContribution = instance.deltaAttribute.GetModifierContribution(attributeModifier);
			str = str + "\n" + string.Format(UI.MODIFIER_ITEM_TEMPLATE, attributeModifier.GetDescription(), formatter.GetFormattedValue(ToPercent(modifierContribution, instance), formatter.DeltaTimeSlice));
		}
		return str;
	}

	public string GetFormattedAttribute(AttributeInstance instance)
	{
		return formatter.GetFormattedAttribute(instance);
	}

	public string GetFormattedModifier(AttributeModifier modifier)
	{
		if (modifier.IsMultiplier)
		{
			return GameUtil.GetFormattedPercent(modifier.Value * 100f);
		}
		return formatter.GetFormattedModifier(modifier);
	}

	public string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice)
	{
		return formatter.GetFormattedValue(value, timeSlice);
	}

	protected float ToPercent(float value, AmountInstance instance)
	{
		return 100f * value / instance.GetMax();
	}
}
