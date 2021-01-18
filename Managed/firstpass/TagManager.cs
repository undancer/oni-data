#define UNITY_ASSERTIONS
using System.Collections.Generic;
using UnityEngine.Assertions;

public class TagManager
{
	private static Dictionary<Tag, string> ProperNames = new Dictionary<Tag, string>();

	private static Dictionary<Tag, string> ProperNamesNoLinks = new Dictionary<Tag, string>();

	public static readonly Tag Invalid = default(Tag);

	public static Tag Create(string tag_string)
	{
		Tag tag = default(Tag);
		tag.Name = tag_string;
		if (!ProperNames.ContainsKey(tag))
		{
			ProperNames[tag] = "";
			ProperNamesNoLinks[tag] = "";
		}
		return tag;
	}

	public static Tag Create(string tag_string, string proper_name)
	{
		Tag tag = Create(tag_string);
		if (string.IsNullOrEmpty(proper_name))
		{
			DebugUtil.Assert(test: false, "Attempting to set proper name for tag: " + tag_string + "to null or empty.");
		}
		ProperNames[tag] = proper_name;
		ProperNamesNoLinks[tag] = StripLinkFormatting(proper_name);
		return tag;
	}

	public static Tag[] Create(IList<string> strings)
	{
		Assert.IsTrue(strings != null && strings.Count > 0);
		Tag[] array = new Tag[strings.Count];
		for (int i = 0; i < strings.Count; i++)
		{
			array[i] = Create(strings[i]);
		}
		return array;
	}

	public static void FillMissingProperNames()
	{
		foreach (Tag item in new List<Tag>(ProperNames.Keys))
		{
			if (string.IsNullOrEmpty(ProperNames[item]))
			{
				ProperNames[item] = TagDescriptions.GetDescription(item.Name);
				ProperNamesNoLinks[item] = StripLinkFormatting(ProperNames[item]);
			}
		}
	}

	public static string GetProperName(Tag tag, bool stripLink = false)
	{
		string value = null;
		if (stripLink && ProperNamesNoLinks.TryGetValue(tag, out value))
		{
			return value;
		}
		if (!stripLink && ProperNames.TryGetValue(tag, out value))
		{
			return value;
		}
		return tag.Name;
	}

	public static string StripLinkFormatting(string text)
	{
		string text2 = text;
		try
		{
			while (text2.Contains("<link="))
			{
				int num = text2.IndexOf("</link>");
				if (num > -1)
				{
					text2 = text2.Remove(num, 7);
				}
				else
				{
					Debug.LogWarningFormat("String has no closing link tag: {0}");
				}
				int num2 = text2.IndexOf("<link=");
				if (num2 != -1)
				{
					text2 = text2.Remove(num2, 7);
				}
				else
				{
					Debug.LogWarningFormat("String has no open link tag: {0}");
				}
				int num3 = text2.IndexOf("\">");
				if (num3 != -1)
				{
					text2 = text2.Remove(num2, num3 - num2 + 2);
				}
				else
				{
					Debug.LogWarningFormat("String has no open link tag: {0}");
				}
			}
		}
		catch
		{
			Debug.Log("STRIP LINK FORMATTING FAILED ON: " + text);
			text2 = text;
		}
		return text2;
	}
}
