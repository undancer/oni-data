using Klei.AI;

namespace Database
{
	public class BuildingAttributes : ResourceSet<Attribute>
	{
		public Attribute Decor;

		public Attribute DecorRadius;

		public Attribute NoisePollution;

		public Attribute NoisePollutionRadius;

		public Attribute Hygiene;

		public Attribute Comfort;

		public Attribute OverheatTemperature;

		public Attribute FatalTemperature;

		public BuildingAttributes(ResourceSet parent)
			: base("BuildingAttributes", parent)
		{
			Decor = Add(new Attribute("Decor", is_trainable: true, Attribute.Display.General, is_profession: false));
			DecorRadius = Add(new Attribute("DecorRadius", is_trainable: true, Attribute.Display.General, is_profession: false));
			NoisePollution = Add(new Attribute("NoisePollution", is_trainable: true, Attribute.Display.General, is_profession: false));
			NoisePollutionRadius = Add(new Attribute("NoisePollutionRadius", is_trainable: true, Attribute.Display.General, is_profession: false));
			Hygiene = Add(new Attribute("Hygiene", is_trainable: true, Attribute.Display.General, is_profession: false));
			Comfort = Add(new Attribute("Comfort", is_trainable: true, Attribute.Display.General, is_profession: false));
			OverheatTemperature = Add(new Attribute("OverheatTemperature", is_trainable: true, Attribute.Display.General, is_profession: false));
			OverheatTemperature.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.ModifyOnly));
			FatalTemperature = Add(new Attribute("FatalTemperature", is_trainable: true, Attribute.Display.General, is_profession: false));
			FatalTemperature.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.ModifyOnly));
		}
	}
}
