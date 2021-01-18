using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Klei
{
	public class AliasDirectory : IFileDirectory
	{
		private string id;

		private string root;

		private string prefix;

		public string GetID()
		{
			return id;
		}

		public AliasDirectory(string id, string actual_location, string path_prefix)
		{
			this.id = id;
			actual_location = FileSystem.Normalize(actual_location);
			path_prefix = FileSystem.Normalize(path_prefix);
			root = actual_location;
			prefix = path_prefix;
		}

		private string GetActualPath(string filename)
		{
			if (filename.StartsWith(prefix))
			{
				string str = filename.Substring(prefix.Length);
				return FileSystem.Normalize(root + str);
			}
			return filename;
		}

		private string GetVirtualPath(string filename)
		{
			if (filename.StartsWith(root))
			{
				string str = filename.Substring(root.Length);
				return FileSystem.Normalize(prefix + str);
			}
			return filename;
		}

		public string GetRoot()
		{
			return root;
		}

		public byte[] ReadBytes(string src_filename)
		{
			string actualPath = GetActualPath(src_filename);
			if (!File.Exists(actualPath))
			{
				return null;
			}
			byte[] array = null;
			return File.ReadAllBytes(actualPath);
		}

		public void GetFiles(Regex re, string src_path, ICollection<string> result)
		{
			string actualPath = GetActualPath(src_path);
			if (!Directory.Exists(actualPath))
			{
				return;
			}
			string[] files = Directory.GetFiles(actualPath);
			string[] array = files;
			foreach (string filename in array)
			{
				string filename2 = FileSystem.Normalize(filename);
				string virtualPath = GetVirtualPath(filename2);
				if (re.IsMatch(virtualPath))
				{
					result.Add(virtualPath);
				}
			}
		}

		public bool FileExists(string path)
		{
			return File.Exists(GetActualPath(path));
		}

		public FileHandle FindFileHandle(string path)
		{
			FileHandle result;
			if (FileExists(path))
			{
				path = GetVirtualPath(FileSystem.Normalize(path));
				result = default(FileHandle);
				result.full_path = path;
				result.source = this;
				return result;
			}
			result = default(FileHandle);
			return result;
		}
	}
}
