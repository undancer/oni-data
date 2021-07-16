using System.IO;
using Klei;
using STRINGS;
using UnityEngine;

namespace KMod
{
	public class Local : IDistributionPlatform
	{
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

		private void Subscribe(string directoryName, long timestamp, IFileSource file_source)
		{
			Label label = default(Label);
			label.id = directoryName;
			label.distribution_platform = distribution_platform;
			label.version = directoryName.GetHashCode();
			label.title = directoryName;
			Label label2 = label;
			KModHeader header = KModUtil.GetHeader(file_source, label2.defaultStaticID, directoryName, directoryName);
			label2.title = header.title;
			Mod mod = new Mod(label2, header.staticID, header.description, file_source, UI.FRONTEND.MODS.TOOLTIPS.MANAGE_LOCAL_MOD, delegate
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
