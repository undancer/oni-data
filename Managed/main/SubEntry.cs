using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SubEntry
{
	public ContentContainer lockedContentContainer;

	public Color iconColor = Color.white;

	public List<ContentContainer> contentContainers
	{
		get;
		set;
	}

	public string parentEntryID
	{
		get;
		set;
	}

	public string id
	{
		get;
		set;
	}

	public string name
	{
		get;
		set;
	}

	public string title
	{
		get;
		set;
	}

	public string subtitle
	{
		get;
		set;
	}

	public Sprite icon
	{
		get;
		set;
	}

	public int layoutPriority
	{
		get;
		set;
	}

	public bool disabled
	{
		get;
		set;
	}

	public string lockID
	{
		get;
		set;
	}

	public string[] dlcIds
	{
		get;
		set;
	}

	public string sortString
	{
		get;
		set;
	}

	public bool showBeforeGeneratedCategoryLinks
	{
		get;
		set;
	}

	public SubEntry()
	{
	}

	public SubEntry(string id, string parentEntryID, List<ContentContainer> contentContainers, string name)
	{
		this.id = id;
		this.parentEntryID = parentEntryID;
		this.name = name;
		this.contentContainers = contentContainers;
		if (!string.IsNullOrEmpty(lockID))
		{
			foreach (ContentContainer contentContainer in contentContainers)
			{
				contentContainer.lockID = lockID;
			}
		}
		if (string.IsNullOrEmpty(sortString))
		{
			if (!string.IsNullOrEmpty(title))
			{
				sortString = UI.StripLinkFormatting(title);
			}
			else
			{
				sortString = UI.StripLinkFormatting(name);
			}
		}
	}

	public string[] GetDlcIds()
	{
		return dlcIds;
	}
}
