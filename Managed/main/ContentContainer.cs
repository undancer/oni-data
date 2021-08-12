using System.Collections.Generic;
using KSerialization.Converters;
using UnityEngine;

public class ContentContainer
{
	public enum ContentLayout
	{
		Vertical,
		Horizontal,
		Grid,
		GridTwoColumn,
		GridTwoColumnTall
	}

	public GameObject go;

	public List<ICodexWidget> content { get; set; }

	public string lockID { get; set; }

	[StringEnumConverter]
	public ContentLayout contentLayout { get; set; }

	public bool showBeforeGeneratedContent { get; set; }

	public ContentContainer()
	{
		content = new List<ICodexWidget>();
	}

	public ContentContainer(List<ICodexWidget> content, ContentLayout contentLayout)
	{
		this.content = content;
		this.contentLayout = contentLayout;
	}
}
