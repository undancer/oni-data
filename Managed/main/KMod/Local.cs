using System.IO;
using Klei;
using STRINGS;
using UnityEngine;

namespace KMod
{
	public class Local : IDistributionPlatform
	{
		private class Header
		{
			public string title
			{
				get;
				set;
			}

			public string description
			{
				get;
				set;
			}
		}

		public string folder
		{
			get;
			private set;
		}

		public Label.DistributionPlatform distribution_platform
		{
			get;
			private set;
		}

		public string GetDirectory()
		{
			return FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), folder));
		}

		private void Subscribe(string id, long timestamp, IFileSource file_source)
		{
			FileHandle filehandle = file_source.GetFileSystem().FindFileHandle(Path.Combine(file_source.GetRoot(), "mod.yaml"));
			Header header = ((filehandle.full_path != null) ? YamlIO.LoadFile<Header>(filehandle) : null);
			if (header == null)
			{
				header = new Header
				{
					title = id,
					description = id
				};
			}
			Label label = default(Label);
			label.id = id;
			label.distribution_platform = distribution_platform;
			label.version = id.GetHashCode();
			label.title = header.title;
			Mod mod = new Mod(label, header.description, file_source, UI.FRONTEND.MODS.TOOLTIPS.MANAGE_LOCAL_MOD, delegate
			{
				Application.OpenURL("file://" + file_source.GetRoot());
			});
			if (file_source.GetType() == typeof(Directory))
			{
				mod.status = Mod.Status.Installed;
			}
			Global.Instance.modManager.Subscribe(mod, this);
		}

		public Local(string folder, Label.DistributionPlatform distribution_platform)
		{
			this.folder = folder;
			this.distribution_platform = distribution_platform;
			DirectoryInfo directoryInfo = new DirectoryInfo(GetDirectory());
			if (directoryInfo.Exists)
			{
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				foreach (DirectoryInfo directoryInfo2 in directories)
				{
					string name = directoryInfo2.Name;
					Subscribe(name, directoryInfo2.LastWriteTime.ToFileTime(), new Directory(directoryInfo2.FullName));
				}
			}
		}
	}
}
