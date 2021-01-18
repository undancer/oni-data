using Klei.AI;

namespace Database
{
	public class Attributes : ResourceSet<Attribute>
	{
		public Attribute Construction;

		public Attribute Digging;

		public Attribute Machinery;

		public Attribute Athletics;

		public Attribute Learning;

		public Attribute Cooking;

		public Attribute Caring;

		public Attribute Strength;

		public Attribute Art;

		public Attribute Botanist;

		public Attribute Ranching;

		public Attribute LifeSupport;

		public Attribute Toggle;

		public Attribute PowerTinker;

		public Attribute FarmTinker;

		public Attribute SpaceNavigation;

		public Attribute Immunity;

		public Attribute GermResistance;

		public Attribute Insulation;

		public Attribute ThermalConductivityBarrier;

		public Attribute Decor;

		public Attribute FoodQuality;

		public Attribute ScaldingThreshold;

		public Attribute GeneratorOutput;

		public Attribute MachinerySpeed;

		public Attribute DecorExpectation;

		public Attribute FoodExpectation;

		public Attribute RoomTemperaturePreference;

		public Attribute QualityOfLifeExpectation;

		public Attribute AirConsumptionRate;

		public Attribute MaxUnderwaterTravelCost;

		public Attribute ToiletEfficiency;

		public Attribute Sneezyness;

		public Attribute DiseaseCureSpeed;

		public Attribute DoctoredLevel;

		public Attribute CarryAmount;

		public Attribute QualityOfLife;

		public Attributes(ResourceSet parent)
			: base("Attributes", parent)
		{
			Construction = Add(new Attribute("Construction", is_trainable: true, Attribute.Display.Skill, is_profession: true));
			Construction.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Digging = Add(new Attribute("Digging", is_trainable: true, Attribute.Display.Skill, is_profession: true));
			Digging.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Machinery = Add(new Attribute("Machinery", is_trainable: true, Attribute.Display.Skill, is_profession: true));
			Machinery.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Athletics = Add(new Attribute("Athletics", is_trainable: true, Attribute.Display.Skill, is_profession: true));
			Athletics.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Learning = Add(new Attribute("Learning", is_trainable: true, Attribute.Display.Skill, is_profession: true));
			Learning.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Cooking = Add(new Attribute("Cooking", is_trainable: true, Attribute.Display.Skill, is_profession: true));
			Cooking.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Art = Add(new Attribute("Art", is_trainable: true, Attribute.Display.Skill, is_profession: true));
			Art.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Strength = Add(new Attribute("Strength", is_trainable: true, Attribute.Display.Skill, is_profession: true));
			Strength.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Caring = Add(new Attribute("Caring", is_trainable: true, Attribute.Display.Skill, is_profession: true));
			Caring.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Botanist = Add(new Attribute("Botanist", is_trainable: true, Attribute.Display.Skill, is_profession: true));
			Botanist.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Ranching = Add(new Attribute("Ranching", is_trainable: true, Attribute.Display.Skill, is_profession: true));
			Ranching.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			PowerTinker = Add(new Attribute("PowerTinker", is_trainable: true, Attribute.Display.Normal, is_profession: true));
			PowerTinker.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			FarmTinker = Add(new Attribute("FarmTinker", is_trainable: true, Attribute.Display.Normal, is_profession: true));
			FarmTinker.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			SpaceNavigation = Add(new Attribute("SpaceNavigation", is_trainable: true, Attribute.Display.Normal, is_profession: true));
			SpaceNavigation.SetFormatter(new PercentAttributeFormatter());
			Immunity = Add(new Attribute("Immunity", is_trainable: true, Attribute.Display.Details, is_profession: false));
			Immunity.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			ThermalConductivityBarrier = Add(new Attribute("ThermalConductivityBarrier", is_trainable: false, Attribute.Display.Details, is_profession: false));
			ThermalConductivityBarrier.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Distance, GameUtil.TimeSlice.None));
			Insulation = Add(new Attribute("Insulation", is_trainable: false, Attribute.Display.General, is_profession: true));
			Decor = Add(new Attribute("Decor", is_trainable: false, Attribute.Display.General, is_profession: false));
			Decor.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			FoodQuality = Add(new Attribute("FoodQuality", is_trainable: false, Attribute.Display.General, is_profession: false));
			FoodQuality.SetFormatter(new FoodQualityAttributeFormatter());
			ScaldingThreshold = Add(new Attribute("ScaldingThreshold", is_trainable: false, Attribute.Display.General, is_profession: false));
			ScaldingThreshold.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.None));
			GeneratorOutput = Add(new Attribute("GeneratorOutput", is_trainable: false, Attribute.Display.General, is_profession: false));
			GeneratorOutput.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.None));
			MachinerySpeed = Add(new Attribute("MachinerySpeed", is_trainable: false, Attribute.Display.General, is_profession: false, 1f));
			MachinerySpeed.SetFormatter(new PercentAttributeFormatter());
			DecorExpectation = Add(new Attribute("DecorExpectation", is_trainable: false, Attribute.Display.Expectation, is_profession: false));
			DecorExpectation.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			FoodExpectation = Add(new Attribute("FoodExpectation", is_trainable: false, Attribute.Display.Expectation, is_profession: false));
			FoodExpectation.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			RoomTemperaturePreference = Add(new Attribute("RoomTemperaturePreference", is_trainable: false, Attribute.Display.Normal, is_profession: false));
			RoomTemperaturePreference.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.None));
			QualityOfLifeExpectation = Add(new Attribute("QualityOfLifeExpectation", is_trainable: false, Attribute.Display.Normal, is_profession: false));
			QualityOfLifeExpectation.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			AirConsumptionRate = Add(new Attribute("AirConsumptionRate", is_trainable: false, Attribute.Display.Normal, is_profession: false));
			AirConsumptionRate.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerSecond));
			MaxUnderwaterTravelCost = Add(new Attribute("MaxUnderwaterTravelCost", is_trainable: false, Attribute.Display.Normal, is_profession: false));
			ToiletEfficiency = Add(new Attribute("ToiletEfficiency", is_trainable: false, Attribute.Display.Details, is_profession: false));
			ToiletEfficiency.SetFormatter(new ToPercentAttributeFormatter(1f));
			Sneezyness = Add(new Attribute("Sneezyness", is_trainable: false, Attribute.Display.Details, is_profession: false));
			DiseaseCureSpeed = Add(new Attribute("DiseaseCureSpeed", is_trainable: false, Attribute.Display.Normal, is_profession: false));
			DiseaseCureSpeed.BaseValue = 1f;
			DiseaseCureSpeed.SetFormatter(new ToPercentAttributeFormatter(1f));
			DoctoredLevel = Add(new Attribute("DoctoredLevel", is_trainable: false, Attribute.Display.Never, is_profession: false));
			CarryAmount = Add(new Attribute("CarryAmount", is_trainable: false, Attribute.Display.Details, is_profession: false));
			CarryAmount.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.None));
			QualityOfLife = Add(new Attribute("QualityOfLife", is_trainable: false, Attribute.Display.Details, is_profession: false, 0f, "ui_icon_qualityoflife", "attribute_qualityoflife"));
			QualityOfLife.SetFormatter(new QualityOfLifeAttributeFormatter());
			GermResistance = Add(new Attribute("GermResistance", is_trainable: false, Attribute.Display.Details, is_profession: false, 0f, "ui_icon_immunelevel", "attribute_immunelevel"));
			GermResistance.SetFormatter(new GermResistanceAttributeFormatter());
			LifeSupport = Add(new Attribute("LifeSupport", is_trainable: true, Attribute.Display.Never, is_profession: false));
			LifeSupport.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Toggle = Add(new Attribute("Toggle", is_trainable: true, Attribute.Display.Never, is_profession: false));
			Toggle.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
		}
	}
}
