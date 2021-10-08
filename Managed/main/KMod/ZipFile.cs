using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using Klei;
using UnityEngine;

namespace KMod
{
	internal struct ZipFile : IFileSource
	{
		private string filename;

		private Ionic.Zip.ZipFile zipfile;

		private ZipFileDirectory file_system;

		public ZipFile(string filename)
		{
			this.filename = filename;
			zipfile = Ionic.Zip.ZipFile.Read(filename);
			file_system = new ZipFileDirectory(zipfile.Name, zipfile, Application.streamingAssetsPath, isModded: true);
		}

		public string GetRoot()
		{
			return filename;
		}

		public bool Exists()
		{
			return File.Exists(GetRoot());
		}

		public bool Exists(string relative_path)
		{
			if (!Exists())
			{
				return false;
			}
			foreach (ZipEntry item in zipfile)
			{
				if (FileSystem.Normalize(item.FileName).StartsWith(relative_path))
				{
					return true;
				}
			}
			return false;
		}

		public void GetTopLevelItems(List<FileSystemItem> file_system_items, string relative_root)
		{
			HashSetPool<string, ZipFile>.PooledHashSet pooledHashSet = HashSetPool<string, ZipFile>.Allocate();
			string[] array;
			if (!string.IsNullOrEmpty(relative_root))
			{
				relative_root = relative_root ?? "";
				relative_root = FileSystem.Normalize(relative_root);
				array = relative_root.Split('/');
			}
			else
			{
				array = new string[0];
			}
			foreach (ZipEntry item in zipfile)
			{
				List<string> list = (from part in FileSystem.Normalize(item.FileName).Split('/')
					where !string.IsNullOrEmpty(part)
					select part).ToList();
				if (!IsSharedRoot(array, list))
				{
					continue;
				}
				list = list.GetRange(array.Length, list.Count - array.Length);
				if (list.Count != 0)
				{
					string text = list[0];
					if (pooledHashSet.Add(text))
					{
						file_system_items.Add(new FileSystemItem
						{
							name = text,
							type = ((1 >= list.Count) ? FileSystemItem.ItemType.File : FileSystemItem.ItemType.Directory)
						});
					}
				}
			}
			pooledHashSet.Recycle();
		}

		private bool IsSharedRoot(string[] root_path, List<string> check_path)
		{
			for (int i = 0; i < root_path.Length; i++)
			{
				if (i >= check_path.Count || root_path[i] != check_path[i])
				{
					return false;
				}
			}
			return true;
		}

		public IFileDirectory GetFileSystem()
		{
			return file_system;
		}

		public void CopyTo(string path, List<string> extensions = null)
		{
			foreach (ZipEntry entry in zipfile.Entries)
			{
				bool flag = extensions == null || extensions.Count == 0;
				if (extensions != null)
				{
					foreach (string extension in extensions)
					{
						if (entry.FileName.ToLower().EndsWith(extension))
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					continue;
				}
				string path2 = FileSystem.Normalize(Path.Combine(path, entry.FileName));
				string directoryName = Path.GetDirectoryName(path2);
				if (!string.IsNullOrEmpty(directoryName) && !FileUtil.CreateDirectory(directoryName))
				{
					continue;
				}
				using MemoryStream memoryStream = new MemoryStream((int)entry.UncompressedSize);
				entry.Extract(memoryStream);
				using FileStream fileStream = FileUtil.Create(path2);
				fileStream.Write(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length);
			}
		}

		public string Read(string relative_path)
		{
			ICollection<ZipEntry> collection = zipfile.SelectEntries(relative_path);
			if (collection.Count == 0)
			{
				return string.Empty;
			}
			using (IEnumerator<ZipEntry> enumerator = collection.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ZipEntry current = enumerator.Current;
					using MemoryStream memoryStream = new MemoryStream((int)current.UncompressedSize);
					current.Extract(memoryStream);
					return Encoding.UTF8.GetString(memoryStream.GetBuffer());
				}
			}
			return string.Empty;
		}
	}
}
