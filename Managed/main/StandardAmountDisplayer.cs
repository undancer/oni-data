using Klei.AI;
using STRINGS;
using UnityEngine;

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
			return formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None, instance.gameObject);
		}
		return $"{formatter.GetFormattedValue(instance.value)} / {formatter.GetFormattedValue(instance.GetMax())}";
	}

	public virtual string GetDescription(Amount master, AmountInstance instance)
	{
		return $"{master.Name}: {GetValueString(master, instance)}";
	}

	public virtual string GetTooltip(Amount master, AmountInstance instance)
	{
		string str = "";
		str = ((master.description.IndexOf("{1}") <= -1) ? (str + string.Format(master.description, formatter.GetFormattedValue(instance.value))) : (str + string.Format(master.description, formatter.GetFormattedValue(instance.value), GameUtil.GetIdentityDescriptor(instance.gameObject))));
		str += "\n\n";
		if (formatter.DeltaTimeSlice == GameUtil.TimeSlice.PerCycle)
		{
			str += string.Format(UI.CHANGEPERCYCLE, formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerCycle));
		}
		else if (formatter.DeltaTimeSlice == GameUtil.TimeSlice.PerSecond)
		{
			str += string.Format(UI.CHANGEPERSECOND, formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerSecond));
		}
		for (int i = 0; i != instance.deltaAttribute.Modifiers.Count; i++)
		{
			AttributeModifier attributeModifier = instance.deltaAttribute.Modifiers[i];
			str = str + "\n" + string.Format(UI.MODIFIER_ITEM_TEMPLATE, attributeModifier.GetDescription(), formatter.GetFormattedModifier(attributeModifier, instance.gameObject));
		}
		return str;
	}

	public string GetFormattedAttribute(AttributeInstance instance)
	{
		return formatter.GetFormattedAttribute(instance);
	}

	public string GetFormattedModifier(AttributeModifier modifier, GameObject parent_instance)
	{
		return formatter.GetFormattedModifier(modifier, parent_instance);
	}

	public string GetFormattedValue(float value, GameUtil.TimeSlice time_slice, GameObject parent_instance)
	{
		return formatter.GetFormattedValue(value, time_slice, parent_instance);
	}
}
