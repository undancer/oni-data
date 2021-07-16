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

		public string title;

		public long version;

		[JsonIgnore]
		private string distribution_platform_name => distribution_platform.ToString();

		[JsonIgnore]
		public string install_path => FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), distribution_platform_name, id));

		[JsonIgnore]
		public string defaultStaticID => id + "." + distribution_platform;

		public override string ToString()
		{
			return title;
		}

		public bool Match(Label rhs)
		{
			if (id == rhs.id)
			{
				return distribution_platform == rhs.distribution_platform;
			}
			return false;
		}
	}
}
