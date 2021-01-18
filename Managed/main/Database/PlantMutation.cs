using Klei.AI;

namespace Database
{
	public class PlantMutation : Resource
	{
		public bool fertilization;

		public bool irrigation;

		public Trait trait;

		public PlantMutation(string id, string name, string desc, string attributeId, float delta, bool positiveTrait, bool fertilization, bool irrigation, ResourceSet parent)
			: base(id, parent)
		{
			trait = Db.Get().CreateTrait(id, name, desc, null, should_save: true, null, positiveTrait, is_valid_starter_trait: false);
			trait.Add(new AttributeModifier(attributeId, delta, name));
			this.fertilization = fertilization;
			this.irrigation = irrigation;
		}
	}
}
