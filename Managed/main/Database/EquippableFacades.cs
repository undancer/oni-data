using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klei;

namespace Database
{
	public class EquippableFacades : ResourceSet<EquippableFacadeResource>
	{
		public EquippableFacades(ResourceSet parent)
			: base("EquippableFacades", parent)
		{
			Initialize();
			Load();
		}

		public void Load()
		{
			ListPool<YamlIO.Error, EquippableFacadeResource>.PooledList errors = ListPool<YamlIO.Error, EquippableFacadeResource>.Allocate();
			List<FileHandle> list = new List<FileHandle>();
			FileSystem.GetFiles(FileSystem.Normalize(Path.Combine(Db.GetPath("", "equippablefacades"))), "*.yaml", list);
			foreach (FileHandle item in list)
			{
				EquippableFacadeInfo equippableFacadeInfo = YamlIO.LoadFile<EquippableFacadeInfo>(item, delegate(YamlIO.Error error, bool force_log_as_warning)
				{
					errors.Add(error);
				});
				if (equippableFacadeInfo.defID == null)
				{
					continue;
				}
				foreach (EquippableFacadeInfo.equippable equippable in equippableFacadeInfo.equippables)
				{
					resources.Add(new EquippableFacadeResource(equippable.name, equippable.buildoverride, equippableFacadeInfo.defID, equippable.animfile));
				}
			}
			resources = resources.Distinct().ToList();
		}
	}
}
