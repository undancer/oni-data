using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class Attribute : Resource
	{
		public enum Display
		{
			Normal,
			Skill,
			Expectation,
			General,
			Details,
			Never
		}

		private static readonly StandardAttributeFormatter defaultFormatter = new StandardAttributeFormatter(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.None);

		public string Description;

		public float BaseValue;

		public Display ShowInUI;

		public bool IsTrainable;

		public bool IsProfession;

		public string ProfessionName;

		public List<AttributeConverter> converters = new List<AttributeConverter>();

		public string uiSprite;

		public string thoughtSprite;

		public string uiFullColourSprite;

		public IAttributeFormatter formatter;

		public Attribute(string id, bool is_trainable, Display show_in_ui, bool is_profession, float base_value = 0f, string uiSprite = null, string thoughtSprite = null, string uiFullColourSprite = null)
			: base(id)
		{
			string str = "STRINGS.DUPLICANTS.ATTRIBUTES." + id.ToUpper();
			Name = Strings.Get(new StringKey(str + ".NAME"));
			ProfessionName = Strings.Get(new StringKey(str + ".NAME"));
			Description = Strings.Get(new StringKey(str + ".DESC"));
			IsTrainable = is_trainable;
			IsProfession = is_profession;
			ShowInUI = show_in_ui;
			BaseValue = base_value;
			formatter = defaultFormatter;
			this.uiSprite = uiSprite;
			this.thoughtSprite = thoughtSprite;
			this.uiFullColourSprite = uiFullColourSprite;
		}

		public Attribute(string id, string name, string profession_name, string attribute_description, float base_value, Display show_in_ui, bool is_trainable, string uiSprite = null, string thoughtSprite = null, string uiFullColourSprite = null)
			: base(id, name)
		{
			Description = attribute_description;
			ProfessionName = profession_name;
			BaseValue = base_value;
			ShowInUI = show_in_ui;
			IsTrainable = is_trainable;
			this.uiSprite = uiSprite;
			this.thoughtSprite = thoughtSprite;
			this.uiFullColourSprite = uiFullColourSprite;
			if (ProfessionName == "")
			{
				ProfessionName = null;
			}
		}

		public void SetFormatter(IAttributeFormatter formatter)
		{
			this.formatter = formatter;
		}

		public AttributeInstance Lookup(Component cmp)
		{
			return Lookup(cmp.gameObject);
		}

		public AttributeInstance Lookup(GameObject go)
		{
			return go.GetAttributes()?.Get(this);
		}

		public string GetDescription(AttributeInstance instance)
		{
			return instance.GetDescription();
		}

		public string GetTooltip(AttributeInstance instance)
		{
			return formatter.GetTooltip(this, instance);
		}
	}
}
