namespace Epic.OnlineServices.Lobby
{
	public class LobbyDetailsCopyMemberAttributeByIndexOptions
	{
		public int ApiVersion => 1;

		public ProductUserId TargetUserId
		{
			get;
			set;
		}

		public uint AttrIndex
		{
			get;
			set;
		}
	}
}
