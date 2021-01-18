using Klei.AI;

namespace Database
{
	public class Amounts : ResourceSet<Amount>
	{
		public Amount Stamina;

		public Amount Calories;

		public Amount ImmuneLevel;

		public Amount ExternalTemperature;

		public Amount Breath;

		public Amount Stress;

		public Amount Toxicity;

		public Amount Bladder;

		public Amount Decor;

		public Amount Temperature;

		public Amount HitPoints;

		public Amount AirPressure;

		public Amount Maturity;

		public Amount OldAge;

		public Amount Age;

		public Amount Fertilization;

		public Amount Illumination;

		public Amount Irrigation;

		public Amount CreatureCalories;

		public Amount Fertility;

		public Amount Viability;

		public Amount Wildness;

		public Amount Incubation;

		public Amount ScaleGrowth;

		public Amount InternalBattery;

		public Amount Rot;

		public void Load()
		{
			Stamina = CreateAmount("Stamina", 0f, 100f, show_max: false, Units.Flat, 0.35f, show_in_ui: true, "STRINGS.DUPLICANTS.STATS", "ui_icon_stamina", "attribute_stamina");
			Stamina.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle));
			Calories = CreateAmount("Calories", 0f, 0f, show_max: false, Units.Flat, 4000f, show_in_ui: true, "STRINGS.DUPLICANTS.STATS", "ui_icon_calories", "attribute_calories");
			Calories.SetDisplayer(new CaloriesDisplayer());
			Temperature = CreateAmount("Temperature", 0f, 10000f, show_max: false, Units.Kelvin, 0.5f, show_in_ui: true, "STRINGS.DUPLICANTS.STATS", "ui_icon_temperature");
			Temperature.SetDisplayer(new DuplicantTemperatureDeltaAsEnergyAmountDisplayer(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.PerSecond));
			ExternalTemperature = CreateAmount("ExternalTemperature", 0f, 10000f, show_max: false, Units.Kelvin, 0.5f, show_in_ui: true, "STRINGS.DUPLICANTS.STATS");
			ExternalTemperature.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.PerSecond));
			Breath = CreateAmount("Breath", 0f, 100f, show_max: false, Units.Flat, 0.5f, show_in_ui: true, "STRINGS.DUPLICANTS.STATS", "ui_icon_breath");
			Breath.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond));
			Stress = CreateAmount("Stress", 0f, 100f, show_max: false, Units.Flat, 0.5f, show_in_ui: true, "STRINGS.DUPLICANTS.STATS", "ui_icon_stress", "attribute_stress");
			Stress.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			Toxicity = CreateAmount("Toxicity", 0f, 100f, show_max: true, Units.Flat, 0.5f, show_in_ui: true, "STRINGS.DUPLICANTS.STATS");
			Toxicity.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle));
			Bladder = CreateAmount("Bladder", 0f, 100f, show_max: false, Units.Flat, 0.5f, show_in_ui: true, "STRINGS.DUPLICANTS.STATS", "ui_icon_bladder");
			Bladder.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle));
			Decor = CreateAmount("Decor", -1000f, 1000f, show_max: false, Units.Flat, 0.016666668f, show_in_ui: true, "STRINGS.DUPLICANTS.STATS", "ui_icon_decor");
			Decor.SetDisplayer(new DecorDisplayer());
			Maturity = CreateAmount("Maturity", 0f, 0f, show_max: true, Units.Flat, 0.0009166667f, show_in_ui: true, "STRINGS.CREATURES.STATS", "ui_icon_maturity");
			Maturity.SetDisplayer(new MaturityDisplayer());
			OldAge = CreateAmount("OldAge", 0f, 0f, show_max: false, Units.Flat, 0f, show_in_ui: false, "STRINGS.CREATURES.STATS");
			Fertilization = CreateAmount("Fertilization", 0f, 100f, show_max: true, Units.Flat, 0.1675f, show_in_ui: true, "STRINGS.CREATURES.STATS");
			Fertilization.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond));
			Fertility = CreateAmount("Fertility", 0f, 100f, show_max: true, Units.Flat, 0.008375f, show_in_ui: true, "STRINGS.CREATURES.STATS", "ui_icon_fertility");
			Fertility.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			Wildness = CreateAmount("Wildness", 0f, 100f, show_max: true, Units.Flat, 0.1675f, show_in_ui: true, "STRINGS.CREATURES.STATS", "ui_icon_wildness");
			Wildness.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			Incubation = CreateAmount("Incubation", 0f, 100f, show_max: true, Units.Flat, 0.01675f, show_in_ui: true, "STRINGS.CREATURES.STATS", "ui_icon_incubation");
			Incubation.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			Viability = CreateAmount("Viability", 0f, 100f, show_max: true, Units.Flat, 0.1675f, show_in_ui: true, "STRINGS.CREATURES.STATS");
			Viability.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			Age = CreateAmount("Age", 0f, 0f, show_max: true, Units.Flat, 0.1675f, show_in_ui: true, "STRINGS.CREATURES.STATS", "ui_icon_age");
			Age.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.PerCycle));
			Irrigation = CreateAmount("Irrigation", 0f, 1f, show_max: true, Units.Flat, 0.1675f, show_in_ui: true, "STRINGS.CREATURES.STATS");
			Irrigation.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond));
			HitPoints = CreateAmount("HitPoints", 0f, 0f, show_max: true, Units.Flat, 0.1675f, show_in_ui: true, "STRINGS.DUPLICANTS.STATS", "ui_icon_hitpoints", "attribute_hitpoints");
			HitPoints.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.PerCycle));
			ImmuneLevel = CreateAmount("ImmuneLevel", 0f, 100f, show_max: true, Units.Flat, 0.1675f, show_in_ui: true, "STRINGS.DUPLICANTS.STATS", "ui_icon_immunelevel", "attribute_immunelevel");
			ImmuneLevel.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			Rot = CreateAmount("Rot", 0f, 0f, show_max: false, Units.Flat, 0f, show_in_ui: true, "STRINGS.CREATURES.STATS");
			Rot.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			AirPressure = CreateAmount("AirPressure", 0f, 1E+09f, show_max: false, Units.Flat, 0f, show_in_ui: true, "STRINGS.CREATURES.STATS");
			AirPressure.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerSecond));
			Illumination = CreateAmount("Illumination", 0f, 1f, show_max: false, Units.Flat, 0f, show_in_ui: true, "STRINGS.CREATURES.STATS");
			Illumination.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.None));
			ScaleGrowth = CreateAmount("ScaleGrowth", 0f, 100f, show_max: true, Units.Flat, 0.1675f, show_in_ui: true, "STRINGS.CREATURES.STATS", "ui_icon_scale_growth");
			ScaleGrowth.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			InternalBattery = CreateAmount("InternalBattery", 0f, 0f, show_max: false, Units.Flat, 4000f, show_in_ui: true, "STRINGS.ROBOTS.STATS.", "ui_icon_stress", "attribute_stress");
			InternalBattery.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.None));
		}

		public Amount CreateAmount(string id, float min, float max, bool show_max, Units units, float delta_threshold, bool show_in_ui, string string_root, string uiSprite = null, string thoughtSprite = null)
		{
			string text = Strings.Get(string.Format("{1}.{0}.NAME", id.ToUpper(), string_root.ToUpper()));
			string description = Strings.Get(string.Format("{1}.{0}.TOOLTIP", id.ToUpper(), string_root.ToUpper()));
			Attribute attribute = new Attribute(id + "Min", "Minimum" + text, "", "", min, Attribute.Display.Normal, is_trainable: false);
			Attribute attribute2 = new Attribute(id + "Max", "Maximum" + text, "", "", max, Attribute.Display.Normal, is_trainable: false);
			string text2 = id + "Delta";
			string name = Strings.Get($"STRINGS.DUPLICANTS.ATTRIBUTES.{text2.ToUpper()}.NAME");
			string attribute_description = Strings.Get($"STRINGS.DUPLICANTS.ATTRIBUTES.{text2.ToUpper()}.DESC");
			Attribute attribute3 = new Attribute(text2, name, "", attribute_description, 0f, Attribute.Display.Normal, is_trainable: false);
			Amount amount = new Amount(id, text, description, attribute, attribute2, attribute3, show_max, units, delta_threshold, show_in_ui, uiSprite, thoughtSprite);
			Db.Get().Attributes.Add(attribute);
			Db.Get().Attributes.Add(attribute2);
			Db.Get().Attributes.Add(attribute3);
			Add(amount);
			return amount;
		}
	}
}
