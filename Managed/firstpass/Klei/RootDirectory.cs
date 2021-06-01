using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Klei
{
	public class RootDirectory : IFileDirectory
	{
		private string id = "StandardFS";

		public string GetID()
		{
			return id;
		}

		public string GetRoot()
		{
			return "";
		}

		public byte[] ReadBytes(string filename)
		{
			byte[] array = null;
			return File.ReadAllBytes(filename);
		}

		public string ReadText(string filename)
		{
			byte[] bytes = ReadBytes(filename);
			return Encoding.UTF8.GetString(bytes);
		}

		public void GetFiles(Regex re, string path, ICollection<string> result)
		{
			if (!Directory.Exists(path))
			{
				return;
			}
			string[] files = Directory.GetFiles(path);
			string[] array = files;
			foreach (string filename in array)
			{
				string text = FileSystem.Normalize(filename);
				if (re.IsMatch(text))
				{
					result.Add(text);
				}
			}
		}

		public bool FileExists(string path)
		{
			return File.Exists(path);
		}

		public FileHandle FindFileHandle(string path)
		{
			FileHandle result;
			if (FileExists(path))
			{
				result = default(FileHandle);
				result.full_path = FileSystem.Normalize(path);
				result.source = this;
				return result;
			}
			result = default(FileHandle);
			return result;
		}
	}
}