public class RoomTypeCategory : Resource
{
	public string colorName { get; private set; }

	public RoomTypeCategory(string id, string name, string colorName)
		: base(id, name)
	{
		this.colorName = colorName;
	}
}
