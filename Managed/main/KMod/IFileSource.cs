using System.Collections.Generic;
using Klei;

namespace KMod
{
	public interface IFileSource
	{
		string GetRoot();

		bool Exists();

		bool Exists(string relative_path);

		void GetTopLevelItems(List<FileSystemItem> file_system_items, string relative_root = "");

		IFileDirectory GetFileSystem();

		void CopyTo(string path, List<string> extensions = null);

		string Read(string relative_path);
	}
}
