using System;
using System.Diagnostics;
using System.IO;
using Klei;
using Newtonsoft.Json;

namespace KMod
{
	[JsonObject(MemberSerialization.Fields)]
	[DebuggerDisplay("{title}")]
	public struct Label
	{
		public enum DistributionPlatform
		{
			Local,
			Steam,
			Epic,
			Rail,
			Dev
		}

		public DistributionPlatform distribution_platform;

		public string id;

		public long version;

		public string title;

		[JsonIgnore]
		private string distribution_platform_name => distribution_platform.ToString();

		[JsonIgnore]
		public string install_path => FileSystem.Normalize(Path.Combine(Path.Combine(Manager.GetDirectory(), distribution_platform_name), id));

		[JsonIgnore]
		public System.DateTime time_stamp => System.DateTime.FromFileTimeUtc(version);

		public override string ToString()
		{
			return title;
		}

		public bool Match(Label rhs)
		{
			return id == rhs.id && distribution_platform == rhs.distribution_platform;
		}
	}
}