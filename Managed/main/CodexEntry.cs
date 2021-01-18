using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class CodexEntry
{
	public EntryDevLog log = new EntryDevLog();

	private List<ContentContainer> _contentContainers = new List<ContentContainer>();

	private string _id;

	private string _parentId;

	private string _category;

	private string _title;

	private string _name;

	private string _subtitle;

	private List<SubEntry> _subEntries = new List<SubEntry>();

	private Sprite _icon;

	private Color _iconColor = Color.white;

	private string _iconPrefabID;

	private bool _disabled;

	private bool _searchOnly;

	private int _customContentLength;

	private string _sortString;

	private bool _showBeforeGeneratedCategoryLinks;

	public List<ContentContainer> contentContainers
	{
		get
		{
			return _contentContainers;
		}
		private set
		{
			_contentContainers = value;
		}
	}

	public string id
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	public string parentId
	{
		get
		{
			return _parentId;
		}
		set
		{
			_parentId = value;
		}
	}

	public string category
	{
		get
		{
			return _category;
		}
		set
		{
			_category = value;
		}
	}

	public string title
	{
		get
		{
			return _title;
		}
		set
		{
			_title = value;
		}
	}

	public string name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public string subtitle
	{
		get
		{
			return _subtitle;
		}
		set
		{
			_subtitle = value;
		}
	}

	public List<SubEntry> subEntries
	{
		get
		{
			return _subEntries;
		}
		set
		{
			_subEntries = value;
		}
	}

	public Sprite icon
	{
		get
		{
			return _icon;
		}
		set
		{
			_icon = value;
		}
	}

	public Color iconColor
	{
		get
		{
			return _iconColor;
		}
		set
		{
			_iconColor = value;
		}
	}

	public string iconPrefabID
	{
		get
		{
			return _iconPrefabID;
		}
		set
		{
			_iconPrefabID = value;
		}
	}

	public bool disabled
	{
		get
		{
			return _disabled;
		}
		set
		{
			_disabled = value;
		}
	}

	public bool searchOnly
	{
		get
		{
			return _searchOnly;
		}
		set
		{
			_searchOnly = value;
		}
	}

	public int customContentLength
	{
		get
		{
			return _customContentLength;
		}
		set
		{
			_customContentLength = value;
		}
	}

	public string sortString
	{
		get
		{
			return _sortString;
		}
		set
		{
			_sortString = value;
		}
	}

	public bool showBeforeGeneratedCategoryLinks
	{
		get
		{
			return _showBeforeGeneratedCategoryLinks;
		}
		set
		{
			_showBeforeGeneratedCategoryLinks = value;
		}
	}

	public CodexEntry()
	{
	}

	public CodexEntry(string category, List<ContentContainer> contentContainers, string name)
	{
		this.category = category;
		this.name = name;
		this.contentContainers = contentContainers;
		if (string.IsNullOrEmpty(sortString))
		{
			sortString = UI.StripLinkFormatting(name);
		}
	}

	public CodexEntry(string category, string titleKey, List<ContentContainer> contentContainers)
	{
		this.category = category;
		title = titleKey;
		this.contentContainers = contentContainers;
		if (string.IsNullOrEmpty(sortString))
		{
			sortString = UI.StripLinkFormatting(title);
		}
	}

	public static List<string> ContentContainerDebug(List<ContentContainer> _contentContainers)
	{
		List<string> list = new List<string>();
		foreach (ContentContainer _contentContainer in _contentContainers)
		{
			if (_contentContainer != null)
			{
				string text = "<b>" + _contentContainer.contentLayout.ToString() + " container: " + ((_contentContainer.content != null) ? _contentContainer.content.Count : 0) + " items</b>";
				if (_contentContainer.content != null)
				{
					text += "\n";
					for (int i = 0; i < _contentContainer.content.Count; i++)
					{
						text = text + "    • " + _contentContainer.content[i].ToString() + ": " + GetContentWidgetDebugString(_contentContainer.content[i]) + "\n";
					}
				}
				list.Add(text);
			}
			else
			{
				list.Add("null container");
			}
		}
		return list;
	}

	private static string GetContentWidgetDebugString(ICodexWidget widget)
	{
		CodexText codexText = widget as CodexText;
		if (codexText != null)
		{
			return codexText.text;
		}
		CodexLabelWithIcon codexLabelWithIcon = widget as CodexLabelWithIcon;
		if (codexLabelWithIcon != null)
		{
			return codexLabelWithIcon.label.text + " / " + codexLabelWithIcon.icon.spriteName;
		}
		CodexImage codexImage = widget as CodexImage;
		if (codexImage != null)
		{
			return codexImage.spriteName;
		}
		CodexVideo codexVideo = widget as CodexVideo;
		if (codexVideo != null)
		{
			return codexVideo.name;
		}
		CodexIndentedLabelWithIcon codexIndentedLabelWithIcon = widget as CodexIndentedLabelWithIcon;
		if (codexIndentedLabelWithIcon != null)
		{
			return codexIndentedLabelWithIcon.label.text + " / " + codexIndentedLabelWithIcon.icon.spriteName;
		}
		return "";
	}

	public void CreateContentContainerCollection()
	{
		contentContainers = new List<ContentContainer>();
	}

	public void InsertContentContainer(int index, ContentContainer container)
	{
		contentContainers.Insert(index, container);
	}

	public void RemoveContentContainerAt(int index)
	{
		contentContainers.RemoveAt(index);
	}

	public void AddContentContainer(ContentContainer container)
	{
		contentContainers.Add(container);
	}

	public void AddContentContainerRange(IEnumerable<ContentContainer> containers)
	{
		contentContainers.AddRange(containers);
	}

	public void RemoveContentContainer(ContentContainer container)
	{
		contentContainers.Remove(container);
	}

	public ICodexWidget GetFirstWidget()
	{
		for (int i = 0; i < contentContainers.Count; i++)
		{
			if (contentContainers[i].content == null)
			{
				continue;
			}
			for (int j = 0; j < contentContainers[i].content.Count; j++)
			{
				if (contentContainers[i].content[j] != null)
				{
					return contentContainers[i].content[j];
				}
			}
		}
		return null;
	}
}
