public class PlantMutation : Resource
{
	public string description;

	public PlantMutation(string id, ResourceSet parent, string name, string description)
		: base(id, parent, name)
	{
		this.description = description;
	}
}
