namespace rail
{
	public class AsyncUpdateMetadataResult : EventBase
	{
		public EnumRailSpaceWorkType type;

		public SpaceWorkID id = new SpaceWorkID();
	}
}
