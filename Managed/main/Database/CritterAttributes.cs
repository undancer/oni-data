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
			Happiness = Add(new Attribute("Happiness", is_trainable: false, Attribute.Display.General, is_profession: false));
			Metabolism = Add(new Attribute("Metabolism", is_trainable: false, Attribute.Display.Details, is_profession: false));
			Metabolism.SetFormatter(new ToPercentAttributeFormatter(100f));
		}
	}
}
