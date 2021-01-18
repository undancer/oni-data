using UnityEngine;

namespace Database
{
	public class TechTreeTitles : ResourceSet<TechTreeTitle>
	{
		public TechTreeTitles(ResourceSet parent)
			: base("TreeTitles", parent)
		{
		}

		public void Load(TextAsset tree_file)
		{
			foreach (ResourceTreeNode item in new ResourceTreeLoader<ResourceTreeNode>(tree_file))
			{
				if (string.Equals(item.Id.Substring(0, 1), "_"))
				{
					new TechTreeTitle(item.Id, this, Strings.Get("STRINGS.RESEARCH.TREES.TITLE" + item.Id.ToUpper()), item);
				}
			}
		}
	}
}
