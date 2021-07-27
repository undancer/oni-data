namespace Epic.OnlineServices.Lobby
{
	public class AttributeData
	{
		public int ApiVersion => 1;

		public string Key { get; set; }

		public AttributeDataValue Value { get; set; }
	}
}
