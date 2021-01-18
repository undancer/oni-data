using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Ionic.Zip;

namespace Klei
{
	public class ZipFileDirectory : IFileDirectory
	{
		private string id;

		private string mountPoint;

		private ZipFile zipfile;

		public string MountPoint => mountPoint;

		public string GetID()
		{
			return id;
		}

		public ZipFileDirectory(string id, ZipFile zipfile, string mount_point = "")
		{
			this.id = id;
			mountPoint = FileSystem.Normalize(mount_point);
			this.zipfile = zipfile;
		}

		public ZipFileDirectory(string id, Stream zip_data_stream, string mount_point = "")
			: this(id, ZipFile.Read(zip_data_stream), mount_point)
		{
		}

		public string GetRoot()
		{
			return MountPoint;
		}

		public byte[] ReadBytes(string filename)
		{
			if (mountPoint.Length > 0)
			{
				filename = filename.Substring(mountPoint.Length);
			}
			ZipEntry zipEntry = zipfile[filename];
			if (zipEntry == null)
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			zipEntry.Extract(memoryStream);
			return memoryStream.ToArray();
		}

		public void GetFiles(Regex re, string path, ICollection<string> result)
		{
			if (zipfile.Count <= 0)
			{
				return;
			}
			foreach (ZipEntry entry in zipfile.Entries)
			{
				if (!entry.IsDirectory)
				{
					string text = FileSystem.Normalize(Path.Combine(mountPoint, entry.FileName));
					if (re.IsMatch(text))
					{
						result.Add(text);
					}
				}
			}
		}

		public bool FileExists(string path)
		{
			if (mountPoint.Length > 0)
			{
				path = path.Substring(mountPoint.Length);
			}
			return zipfile.ContainsEntry(path);
		}

		public FileHandle FindFileHandle(string path)
		{
			FileHandle result;
			if (FileExists(path))
			{
				if (mountPoint.Length > 0)
				{
					path = path.Substring(mountPoint.Length);
				}
				result = default(FileHandle);
				result.full_path = FileSystem.Normalize(Path.Combine(mountPoint, path));
				result.source = this;
				return result;
			}
			result = default(FileHandle);
			return result;
		}
	}
}
