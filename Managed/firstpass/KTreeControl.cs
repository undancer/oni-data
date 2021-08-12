using System;
using System.Collections.Generic;
using UnityEngine;

public class KTreeControl : MonoBehaviour
{
	public class UserItem
	{
		public string text;

		public object userData;

		public IList<UserItem> children;
	}

	[SerializeField]
	private KTreeItem treeItemPrefab;

	[NonSerialized]
	public KTreeItem root;

	public void SetUserItemRoot(UserItem rootItem)
	{
		if (root != null)
		{
			UnityEngine.Object.Destroy(root);
		}
		root = CreateItem(rootItem);
		root.transform.SetParent(base.transform, worldPositionStays: false);
	}

	private KTreeItem CreateItem(UserItem userItem)
	{
		KTreeItem kTreeItem = UnityEngine.Object.Instantiate(treeItemPrefab);
		kTreeItem.text = userItem.text;
		kTreeItem.userData = userItem.userData;
		kTreeItem.onOpenChanged += OnOpenChanged;
		kTreeItem.onCheckChanged += OnCheckChanged;
		if (userItem.children != null)
		{
			for (int i = 0; i < userItem.children.Count; i++)
			{
				KTreeItem child = CreateItem(userItem.children[i]);
				kTreeItem.AddChild(child);
			}
		}
		return kTreeItem;
	}

	private void OnOpenChanged(KTreeItem item, bool value)
	{
	}

	private void OnCheckChanged(KTreeItem item, bool isChecked)
	{
		if (item.parent != null)
		{
			bool flag = true;
			foreach (KTreeItem child in item.parent.children)
			{
				if (!child.checkboxChecked)
				{
					flag = false;
					break;
				}
			}
			item.parent.checkboxChecked = flag;
			ChangeChecks(item.parent, flag);
		}
		if (item.children == null)
		{
			return;
		}
		foreach (KTreeItem child2 in item.children)
		{
			child2.checkboxChecked = isChecked;
			OnCheckChanged(child2, isChecked);
		}
	}

	private void ChangeChecks(KTreeItem item, bool isChecked)
	{
		if (!(item.parent != null))
		{
			return;
		}
		bool flag = true;
		foreach (KTreeItem child in item.parent.children)
		{
			if (!child.checkboxChecked)
			{
				flag = false;
				break;
			}
		}
		item.parent.checkboxChecked = flag;
		ChangeChecks(item.parent, flag);
	}
}
