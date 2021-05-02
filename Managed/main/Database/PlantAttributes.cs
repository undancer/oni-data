using Klei.AI;

namespace Database
{
	public class PlantAttributes : ResourceSet<Attribute>
	{
		public Attribute WiltTempRangeMod;

		public Attribute YieldAmount;

		public Attribute HarvestTime;

		public Attribute DecorBonus;

		public Attribute MinLightLux;

		public Attribute FertilizerUsageMod;

		public Attribute MinRadiationThreshold;

		public Attribute MaxRadiationThreshold;

		public PlantAttributes(ResourceSet parent)
			: base("PlantAttributes", parent)
		{
			WiltTempRangeMod = Add(new Attribute("WiltTempRangeMod", is_trainable: false, Attribute.Display.Normal, is_profession: false, 1f));
			WiltTempRangeMod.SetFormatter(new PercentAttributeFormatter());
			YieldAmount = Add(new Attribute("YieldAmount", is_trainable: false, Attribute.Display.Normal, is_profession: false));
			YieldAmount.SetFormatter(new PercentAttributeFormatter());
			HarvestTime = Add(new Attribute("HarvestTime", is_trainable: false, Attribute.Display.Normal, is_profession: false));
			HarvestTime.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Time, GameUtil.TimeSlice.None));
			DecorBonus = Add(new Attribute("DecorBonus", is_trainable: false, Attribute.Display.Normal, is_profession: false));
			DecorBonus.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			MinLightLux = Add(new Attribute("MinLightLux", is_trainable: false, Attribute.Display.Normal, is_profession: false));
			MinLightLux.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Lux, GameUtil.TimeSlice.None));
			FertilizerUsageMod = Add(new Attribute("FertilizerUsageMod", is_trainable: false, Attribute.Display.Normal, is_profession: false, 1f));
			FertilizerUsageMod.SetFormatter(new PercentAttributeFormatter());
			MinRadiationThreshold = Add(new Attribute("MinRadiationThreshold", is_trainable: false, Attribute.Display.Normal, is_profession: false));
			MinRadiationThreshold.SetFormatter(new RadsPerCycleAttributeFormatter());
			MaxRadiationThreshold = Add(new Attribute("MaxRadiationThreshold", is_trainable: false, Attribute.Display.Normal, is_profession: false));
			MaxRadiationThreshold.SetFormatter(new RadsPerCycleAttributeFormatter());
		}
	}
}
