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

		private bool isModded;

		public string MountPoint => mountPoint;

		public string GetID()
		{
			return id;
		}

		public ZipFileDirectory(string id, ZipFile zipfile, string mount_point = "", bool isModded = false)
		{
			this.id = id;
			this.isModded = isModded;
			mountPoint = FileSystem.Normalize(mount_point);
			this.zipfile = zipfile;
		}

		public ZipFileDirectory(string id, Stream zip_data_stream, string mount_point = "", bool isModded = false)
			: this(id, ZipFile.Read(zip_data_stream), mount_point, isModded)
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
				if (mountPoint.Length > path.Length)
				{
					Debug.LogError("Tried finding an invalid path inside a matching mount point!\n" + path + "\n" + mountPoint);
				}
				path = path.Substring(mountPoint.Length);
			}
			return zipfile.ContainsEntry(path);
		}

		public FileHandle FindFileHandle(string path)
		{
			if (FileExists(path))
			{
				if (mountPoint.Length > 0)
				{
					if (mountPoint.Length > path.Length)
					{
						Debug.LogError("Tried finding an invalid path inside a matching mount point!\n" + path + "\n" + mountPoint);
					}
					path = path.Substring(mountPoint.Length);
				}
				FileHandle result = default(FileHandle);
				result.full_path = FileSystem.Normalize(Path.Combine(mountPoint, path));
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
