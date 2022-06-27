namespace Epic.OnlineServices.Sessions
{
	public class UpdateSessionOptions
	{
		public int ApiVersion => 1;

		public SessionModification SessionModificationHandle { get; set; }
	}
}
