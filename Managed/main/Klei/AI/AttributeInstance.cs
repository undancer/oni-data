using System;
using System.Collections.Generic;
using System.Diagnostics;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{Attribute.Id}")]
	public class AttributeInstance : ModifierInstance<Attribute>
	{
		public Attribute Attribute;

		public System.Action OnDirty;

		public ArrayRef<AttributeModifier> Modifiers;

		public bool hide;

		public string Id => Attribute.Id;

		public string Name => Attribute.Name;

		public string Description => Attribute.Description;

		public float GetBaseValue()
		{
			return Attribute.BaseValue;
		}

		public float GetTotalDisplayValue()
		{
			float num = Attribute.BaseValue;
			float num2 = 0f;
			for (int i = 0; i != Modifiers.Count; i++)
			{
				AttributeModifier attributeModifier = Modifiers[i];
				if (!attributeModifier.IsMultiplier)
				{
					num += attributeModifier.Value;
				}
				else
				{
					num2 += attributeModifier.Value;
				}
			}
			if (num2 != 0f)
			{
				num += Mathf.Abs(num) * num2;
			}
			return num;
		}

		public float GetTotalValue()
		{
			float num = Attribute.BaseValue;
			float num2 = 0f;
			for (int i = 0; i != Modifiers.Count; i++)
			{
				AttributeModifier attributeModifier = Modifiers[i];
				if (!attributeModifier.UIOnly)
				{
					if (!attributeModifier.IsMultiplier)
					{
						num += attributeModifier.Value;
					}
					else
					{
						num2 += attributeModifier.Value;
					}
				}
			}
			if (num2 != 0f)
			{
				num += Mathf.Abs(num) * num2;
			}
			return num;
		}

		public static float GetTotalDisplayValue(Attribute attribute, List<AttributeModifier> modifiers)
		{
			float num = attribute.BaseValue;
			float num2 = 0f;
			for (int i = 0; i != modifiers.Count; i++)
			{
				AttributeModifier attributeModifier = modifiers[i];
				if (!attributeModifier.IsMultiplier)
				{
					num += attributeModifier.Value;
				}
				else
				{
					num2 += attributeModifier.Value;
				}
			}
			if (num2 != 0f)
			{
				num += Mathf.Abs(num) * num2;
			}
			return num;
		}

		public static float GetTotalValue(Attribute attribute, List<AttributeModifier> modifiers)
		{
			float num = attribute.BaseValue;
			float num2 = 0f;
			for (int i = 0; i != modifiers.Count; i++)
			{
				AttributeModifier attributeModifier = modifiers[i];
				if (!attributeModifier.UIOnly)
				{
					if (!attributeModifier.IsMultiplier)
					{
						num += attributeModifier.Value;
					}
					else
					{
						num2 += attributeModifier.Value;
					}
				}
			}
			if (num2 != 0f)
			{
				num += Mathf.Abs(num) * num2;
			}
			return num;
		}

		public float GetModifierContribution(AttributeModifier testModifier)
		{
			if (!testModifier.IsMultiplier)
			{
				return testModifier.Value;
			}
			float num = Attribute.BaseValue;
			for (int i = 0; i != Modifiers.Count; i++)
			{
				AttributeModifier attributeModifier = Modifiers[i];
				if (!attributeModifier.IsMultiplier)
				{
					num += attributeModifier.Value;
				}
			}
			return num * testModifier.Value;
		}

		public AttributeInstance(GameObject game_object, Attribute attribute)
			: base(game_object, attribute)
		{
			DebugUtil.Assert(attribute != null);
			Attribute = attribute;
		}

		public void Add(AttributeModifier modifier)
		{
			Modifiers.Add(modifier);
			if (OnDirty != null)
			{
				OnDirty();
			}
		}

		public void Remove(AttributeModifier modifier)
		{
			for (int i = 0; i < Modifiers.Count; i++)
			{
				if (Modifiers[i] == modifier)
				{
					Modifiers.RemoveAt(i);
					if (OnDirty != null)
					{
						OnDirty();
					}
					break;
				}
			}
		}

		public void ClearModifiers()
		{
			if (Modifiers.Count > 0)
			{
				Modifiers.Clear();
				if (OnDirty != null)
				{
					OnDirty();
				}
			}
		}

		public string GetDescription()
		{
			return string.Format(DUPLICANTS.ATTRIBUTES.VALUE, Name, GetFormattedValue());
		}

		public string GetFormattedValue()
		{
			return Attribute.formatter.GetFormattedAttribute(this);
		}

		public string GetAttributeValueTooltip()
		{
			return Attribute.GetTooltip(this);
		}
	}
}
