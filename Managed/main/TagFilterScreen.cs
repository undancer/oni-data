using System.Collections.Generic;
using UnityEngine;

public class TagFilterScreen : SideScreenContent
{
	public class TagEntry
	{
		public string name;

		public Tag tag;

		public TagEntry[] children;
	}

	[SerializeField]
	private KTreeControl treeControl;

	private KTreeControl.UserItem rootItem;

	private TagEntry rootTag = defaultRootTag;

	private List<Tag> acceptedTags = new List<Tag>();

	private TreeFilterable targetFilterable;

	public static TagEntry defaultRootTag = new TagEntry
	{
		name = "All",
		tag = default(Tag),
		children = new TagEntry[0]
	};

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<TreeFilterable>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		if (target == null)
		{
			Debug.LogError("The target object provided was null");
			return;
		}
		targetFilterable = target.GetComponent<TreeFilterable>();
		if (targetFilterable == null)
		{
			Debug.LogError("The target provided does not have a Tree Filterable component");
		}
		else if (targetFilterable.showUserMenu)
		{
			Filter(targetFilterable.AcceptedTags);
			Activate();
		}
	}

	protected override void OnActivate()
	{
		rootItem = BuildDisplay(rootTag);
		treeControl.SetUserItemRoot(rootItem);
		treeControl.root.opened = true;
		Filter(treeControl.root, acceptedTags, parentEnabled: false);
	}

	public static List<Tag> GetAllTags()
	{
		List<Tag> list = new List<Tag>();
		TagEntry[] children = defaultRootTag.children;
		foreach (TagEntry tagEntry in children)
		{
			if (tagEntry.tag.IsValid)
			{
				list.Add(tagEntry.tag);
			}
		}
		return list;
	}

	private KTreeControl.UserItem BuildDisplay(TagEntry root)
	{
		KTreeControl.UserItem userItem = null;
		if (root.name != null && root.name != "")
		{
			userItem = new KTreeControl.UserItem
			{
				text = root.name,
				userData = root.tag
			};
			List<KTreeControl.UserItem> list = new List<KTreeControl.UserItem>();
			if (root.children != null)
			{
				TagEntry[] children = root.children;
				foreach (TagEntry root2 in children)
				{
					list.Add(BuildDisplay(root2));
				}
			}
			userItem.children = list;
		}
		return userItem;
	}

	private static KTreeControl.UserItem CreateTree(string tree_name, Tag tree_tag, IList<Element> items)
	{
		KTreeControl.UserItem userItem = new KTreeControl.UserItem
		{
			text = tree_name,
			userData = tree_tag,
			children = new List<KTreeControl.UserItem>()
		};
		foreach (Element item2 in items)
		{
			KTreeControl.UserItem item = new KTreeControl.UserItem
			{
				text = item2.name,
				userData = GameTagExtensions.Create(item2.id)
			};
			userItem.children.Add(item);
		}
		return userItem;
	}

	public void SetRootTag(TagEntry root_tag)
	{
		rootTag = root_tag;
	}

	public void Filter(List<Tag> acceptedTags)
	{
		this.acceptedTags = acceptedTags;
	}

	private void Filter(KTreeItem root, List<Tag> acceptedTags, bool parentEnabled)
	{
		root.checkboxChecked = parentEnabled || (root.userData != null && acceptedTags.Contains((Tag)root.userData));
		foreach (KTreeItem child in root.children)
		{
			Filter(child, acceptedTags, root.checkboxChecked);
		}
		if (root.checkboxChecked || root.children.Count <= 0)
		{
			return;
		}
		bool checkboxChecked = true;
		foreach (KTreeItem child2 in root.children)
		{
			if (!child2.checkboxChecked)
			{
				checkboxChecked = false;
				break;
			}
		}
		root.checkboxChecked = checkboxChecked;
	}

	private void AddEnabledTags(KTreeItem root, List<Tag> tags)
	{
		bool flag = false;
		if (root.userData != null)
		{
			Tag item = (Tag)root.userData;
			if (item.IsValid && root.checkboxChecked)
			{
				flag = true;
				tags.Add(item);
			}
		}
		if (flag)
		{
			return;
		}
		foreach (KTreeItem child in root.children)
		{
			AddEnabledTags(child, tags);
		}
	}

	private void UpdateFilters()
	{
		if (targetFilterable == null)
		{
			Debug.LogError("Cannot update the filters on a null target.");
			return;
		}
		List<Tag> list = new List<Tag>();
		AddEnabledTags(treeControl.root, list);
		targetFilterable.UpdateFilters(list);
	}
}
