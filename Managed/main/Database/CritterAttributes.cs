using Klei.AI;

namespace Database
{
	public class CritterAttributes : ResourceSet<Attribute>
	{
		public Attribute Happiness;

		public Attribute Metabolism;

		public CritterAttributes(ResourceSet parent)
			: base("CritterAttributes", parent)
		{
			Happiness = Add(new Attribute("Happiness", Strings.Get("STRINGS.CREATURES.STATS.HAPPINESS.NAME"), "", Strings.Get("STRINGS.CREATURES.STATS.HAPPINESS.TOOLTIP"), 0f, Attribute.Display.General, is_trainable: false, "ui_icon_happiness"));
			Happiness.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Metabolism = Add(new Attribute("Metabolism", is_trainable: false, Attribute.Display.Details, is_profession: false));
			Metabolism.SetFormatter(new ToPercentAttributeFormatter(100f));
		}
	}
}
