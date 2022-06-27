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

		private bool isModded;

		public string GetID()
		{
			return id;
		}

		public AliasDirectory(string id, string actual_location, string path_prefix, bool isModded = false)
		{
			this.id = id;
			actual_location = FileSystem.Normalize(actual_location);
			path_prefix = FileSystem.Normalize(path_prefix);
			this.isModded = isModded;
			root = actual_location;
			prefix = path_prefix;
		}

		private string GetActualPath(string filename)
		{
			if (filename.StartsWith(prefix))
			{
				string text = filename.Substring(prefix.Length);
				return FileSystem.Normalize(root + text);
			}
			return filename;
		}

		private string GetVirtualPath(string filename)
		{
			if (filename.StartsWith(root))
			{
				string text = filename.Substring(root.Length);
				return FileSystem.Normalize(prefix + text);
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
			for (int i = 0; i < files.Length; i++)
			{
				string filename = FileSystem.Normalize(files[i]);
				string virtualPath = GetVirtualPath(filename);
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
			if (FileExists(path))
			{
				path = GetVirtualPath(FileSystem.Normalize(path));
				FileHandle result = default(FileHandle);
				result.full_path = path;
				result.source = this;
				return result;
			}
			return default(FileHandle);
		}

		public bool IsModded()
		{
			return isModded;
		}
	}
}
