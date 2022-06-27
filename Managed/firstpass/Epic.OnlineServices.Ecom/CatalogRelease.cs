namespace Epic.OnlineServices.Ecom
{
	public class CatalogRelease
	{
		public int ApiVersion => 1;

		public string[] CompatibleAppIds { get; set; }

		public string[] CompatiblePlatforms { get; set; }

		public string ReleaseNote { get; set; }
	}
}
