using System.Collections.Generic;
using Database;
using UnityEngine;

public class Tech : Resource
{
	public List<Tech> requiredTech = new List<Tech>();

	public List<Tech> unlockedTech = new List<Tech>();

	public List<TechItem> unlockedItems = new List<TechItem>();

	public List<string> unlockedItemIDs = new List<string>();

	public int tier;

	public Dictionary<string, float> costsByResearchTypeID = new Dictionary<string, float>();

	public string desc;

	public string category;

	public Tag[] tags;

	private ResourceTreeNode node;

	public bool FoundNode => node != null;

	public Vector2 center => node.center;

	public float width => node.width;

	public float height => node.height;

	public List<ResourceTreeNode.Edge> edges => node.edges;

	public Tech(string id, string[] unlockedItemIDs, Techs techs)
		: base(id, techs, Strings.Get("STRINGS.RESEARCH.TECHS." + id.ToUpper() + ".NAME"))
	{
		desc = Strings.Get("STRINGS.RESEARCH.TECHS." + id.ToUpper() + ".DESC");
		foreach (string item in unlockedItemIDs)
		{
			this.unlockedItemIDs.Add(item);
		}
	}

	public void SetNode(ResourceTreeNode node, string categoryID)
	{
		this.node = node;
		category = categoryID;
	}

	public bool CanAfford(ResearchPointInventory pointInventory)
	{
		foreach (KeyValuePair<string, float> item in costsByResearchTypeID)
		{
			if (pointInventory.PointsByTypeID[item.Key] < item.Value)
			{
				return false;
			}
		}
		return true;
	}

	public string CostString(ResearchTypes types)
	{
		string text = "";
		foreach (KeyValuePair<string, float> item in costsByResearchTypeID)
		{
			text += $"{types.GetResearchType(item.Key).name.ToString()}:{item.Value.ToString()}";
			text += "\n";
		}
		return text;
	}

	public bool IsComplete()
	{
		if (Research.Instance != null)
		{
			return Research.Instance.Get(this)?.IsComplete() ?? false;
		}
		return false;
	}

	public bool ArePrerequisitesComplete()
	{
		foreach (Tech item in requiredTech)
		{
			if (!item.IsComplete())
			{
				return false;
			}
		}
		return true;
	}
}
