using System.IO;
using Klei;

namespace KMod
{
	public class KModUtil
	{
		public static KModHeader GetHeader(IFileSource file_source, string defaultStaticID, string defaultTitle, string defaultDescription)
		{
			string filename = Path.Combine(file_source.GetRoot(), "mod.yaml");
			FileHandle filehandle = file_source.GetFileSystem().FindFileHandle(filename);
			KModHeader kModHeader = ((filehandle.full_path != null) ? YamlIO.LoadFile<KModHeader>(filehandle) : null);
			if (kModHeader == null)
			{
				kModHeader = new KModHeader
				{
					title = defaultTitle,
					description = defaultDescription,
					staticID = defaultStaticID
				};
			}
			if (string.IsNullOrEmpty(kModHeader.staticID))
			{
				kModHeader.staticID = defaultStaticID;
			}
			if (kModHeader.title == null)
			{
				kModHeader.title = defaultTitle;
			}
			if (kModHeader.description == null)
			{
				kModHeader.description = defaultDescription;
			}
			return kModHeader;
		}
	}
}
