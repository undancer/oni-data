using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{text}")]
public struct Descriptor
{
	public enum DescriptorType
	{
		Requirement,
		Effect,
		Lifecycle,
		Information,
		DiseaseSource,
		Detail,
		Symptom,
		SymptomAidable
	}

	public string text;

	public string tooltipText;

	public int indent;

	public DescriptorType type;

	public bool onlyForSimpleInfoScreen;

	public Descriptor(string txt, string tooltip, DescriptorType descriptorType = DescriptorType.Effect, bool only_for_simple_info_screen = false)
	{
		indent = 0;
		text = txt;
		tooltipText = tooltip;
		type = descriptorType;
		onlyForSimpleInfoScreen = only_for_simple_info_screen;
	}

	public void SetupDescriptor(string txt, string tooltip, DescriptorType descriptorType = DescriptorType.Effect)
	{
		text = txt;
		tooltipText = tooltip;
		type = descriptorType;
	}

	public Descriptor IncreaseIndent()
	{
		indent++;
		return this;
	}

	public Descriptor DecreaseIndent()
	{
		indent = Mathf.Max(indent - 1, 0);
		return this;
	}

	public string IndentedText()
	{
		string text = this.text;
		for (int i = 0; i < indent; i++)
		{
			text = "    " + text;
		}
		return text;
	}
}
