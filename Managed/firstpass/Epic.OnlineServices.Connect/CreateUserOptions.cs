namespace Epic.OnlineServices.Connect
{
	public class CreateUserOptions
	{
		public int ApiVersion => 1;

		public ContinuanceToken ContinuanceToken { get; set; }
	}
}
