using System;

namespace YamlDotNet.Samples.Helpers
{
	internal class SampleAttribute : Attribute
	{
		private string title;

		public string DisplayName { get; private set; }

		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				title = value;
				DisplayName = "Sample: " + value;
			}
		}

		public string Description { get; set; }
	}
}
