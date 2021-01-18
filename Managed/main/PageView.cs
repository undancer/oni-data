using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/PageView")]
public class PageView : KMonoBehaviour
{
	[SerializeField]
	private MultiToggle nextButton;

	[SerializeField]
	private MultiToggle prevButton;

	[SerializeField]
	private LocText pageLabel;

	[SerializeField]
	private int childrenPerPage = 8;

	private int currentPage = 0;

	private int oldChildCount = 0;

	public Action<int> OnChangePage;

	public int ChildrenPerPage => childrenPerPage;

	private int pageCount
	{
		get
		{
			int num = base.transform.childCount / childrenPerPage;
			if (base.transform.childCount % childrenPerPage != 0)
			{
				num++;
			}
			return num;
		}
	}

	private void Update()
	{
		if (oldChildCount != base.transform.childCount)
		{
			oldChildCount = base.transform.childCount;
			RefreshPage();
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MultiToggle multiToggle = nextButton;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			currentPage = (currentPage + 1) % pageCount;
			if (OnChangePage != null)
			{
				OnChangePage(currentPage);
			}
			RefreshPage();
		});
		MultiToggle multiToggle2 = prevButton;
		multiToggle2.onClick = (System.Action)Delegate.Combine(multiToggle2.onClick, (System.Action)delegate
		{
			currentPage--;
			if (currentPage < 0)
			{
				currentPage += pageCount;
			}
			if (OnChangePage != null)
			{
				OnChangePage(currentPage);
			}
			RefreshPage();
		});
	}

	private void RefreshPage()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			if (i < currentPage * childrenPerPage)
			{
				base.transform.GetChild(i).gameObject.SetActive(value: false);
			}
			else if (i >= currentPage * childrenPerPage + childrenPerPage)
			{
				base.transform.GetChild(i).gameObject.SetActive(value: false);
			}
			else
			{
				base.transform.GetChild(i).gameObject.SetActive(value: true);
			}
		}
		pageLabel.SetText(currentPage % pageCount + 1 + "/" + pageCount);
	}
}
