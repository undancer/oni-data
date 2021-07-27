using System;

namespace YamlDotNet.Core
{
	[Serializable]
	public class Version
	{
		public int Major { get; private set; }

		public int Minor { get; private set; }

		public Version(int major, int minor)
		{
			Major = major;
			Minor = minor;
		}

		public override bool Equals(object obj)
		{
			Version version = obj as Version;
			if (version != null && Major == version.Major)
			{
				return Minor == version.Minor;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Major.GetHashCode() ^ Minor.GetHashCode();
		}
	}
}
