using System;
using System.Diagnostics;

namespace Klei.AI
{
	[DebuggerDisplay("{AttributeId}")]
	public class AttributeModifier
	{
		public string Description;

		public Func<string> DescriptionCB;

		public string AttributeId { get; private set; }

		public float Value { get; private set; }

		public bool IsMultiplier { get; private set; }

		public bool UIOnly { get; private set; }

		public bool IsReadonly { get; private set; }

		public AttributeModifier(string attribute_id, float value, string description = null, bool is_multiplier = false, bool uiOnly = false, bool is_readonly = true)
		{
			AttributeId = attribute_id;
			Value = value;
			Description = ((description == null) ? attribute_id : description);
			DescriptionCB = null;
			IsMultiplier = is_multiplier;
			UIOnly = uiOnly;
			IsReadonly = is_readonly;
		}

		public AttributeModifier(string attribute_id, float value, Func<string> description_cb, bool is_multiplier = false, bool uiOnly = false)
		{
			AttributeId = attribute_id;
			Value = value;
			DescriptionCB = description_cb;
			Description = null;
			IsMultiplier = is_multiplier;
			UIOnly = uiOnly;
			if (description_cb == null)
			{
				Debug.LogWarning("AttributeModifier being constructed without a description callback: " + attribute_id);
			}
		}

		public void SetValue(float value)
		{
			Value = value;
		}

		public string GetDescription()
		{
			if (DescriptionCB == null)
			{
				return Description;
			}
			return DescriptionCB();
		}

		public string GetFormattedString()
		{
			IAttributeFormatter attributeFormatter = null;
			Attribute attribute = Db.Get().Attributes.TryGet(AttributeId);
			if (!IsMultiplier)
			{
				if (attribute != null)
				{
					attributeFormatter = attribute.formatter;
				}
				else
				{
					attribute = Db.Get().BuildingAttributes.TryGet(AttributeId);
					if (attribute != null)
					{
						attributeFormatter = attribute.formatter;
					}
					else
					{
						attribute = Db.Get().PlantAttributes.TryGet(AttributeId);
						if (attribute != null)
						{
							attributeFormatter = attribute.formatter;
						}
					}
				}
			}
			string text = "";
			text = ((attributeFormatter != null) ? attributeFormatter.GetFormattedModifier(this) : ((!IsMultiplier) ? (text + GameUtil.GetFormattedSimple(Value)) : (text + GameUtil.GetFormattedPercent(Value * 100f))));
			if (text != null && text.Length > 0 && text[0] != '-')
			{
				text = GameUtil.AddPositiveSign(text, Value > 0f);
			}
			return text;
		}

		public AttributeModifier Clone()
		{
			return new AttributeModifier(AttributeId, Value, Description);
		}
	}
}
