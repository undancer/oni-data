using Klei.AI;
using STRINGS;

public class StandardAmountDisplayer : IAmountDisplayer
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

	public StandardAmountDisplayer(GameUtil.UnitClass unitClass, GameUtil.TimeSlice deltaTimeSlice, StandardAttributeFormatter formatter = null)
	{
		if (formatter != null)
		{
			this.formatter = formatter;
		}
		else
		{
			this.formatter = new StandardAttributeFormatter(unitClass, deltaTimeSlice);
		}
	}

	public virtual string GetValueString(Amount master, AmountInstance instance)
	{
		if (!master.showMax)
		{
			return formatter.GetFormattedValue(instance.value);
		}
		return $"{formatter.GetFormattedValue(instance.value)} / {formatter.GetFormattedValue(instance.GetMax())}";
	}

	public virtual string GetDescription(Amount master, AmountInstance instance)
	{
		return $"{master.Name}: {GetValueString(master, instance)}";
	}

	public virtual string GetTooltip(Amount master, AmountInstance instance)
	{
		string text = "";
		text = ((master.description.IndexOf("{1}") <= -1) ? (text + string.Format(master.description, formatter.GetFormattedValue(instance.value))) : (text + string.Format(master.description, formatter.GetFormattedValue(instance.value), GameUtil.GetIdentityDescriptor(instance.gameObject))));
		text += "\n\n";
		if (formatter.DeltaTimeSlice == GameUtil.TimeSlice.PerCycle)
		{
			text += string.Format(UI.CHANGEPERCYCLE, formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerCycle));
		}
		else if (formatter.DeltaTimeSlice == GameUtil.TimeSlice.PerSecond)
		{
			text += string.Format(UI.CHANGEPERSECOND, formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerSecond));
		}
		for (int i = 0; i != instance.deltaAttribute.Modifiers.Count; i++)
		{
			AttributeModifier attributeModifier = instance.deltaAttribute.Modifiers[i];
			text = text + "\n" + string.Format(UI.MODIFIER_ITEM_TEMPLATE, attributeModifier.GetDescription(), formatter.GetFormattedModifier(attributeModifier));
		}
		return text;
	}

	public string GetFormattedAttribute(AttributeInstance instance)
	{
		return formatter.GetFormattedAttribute(instance);
	}

	public string GetFormattedModifier(AttributeModifier modifier)
	{
		return formatter.GetFormattedModifier(modifier);
	}

	public string GetFormattedValue(float value, GameUtil.TimeSlice time_slice)
	{
		return formatter.GetFormattedValue(value, time_slice);
	}
}
