using System;
using UnityEngine;

public class TechItem : Resource
{
	public string description;

	public Func<string, bool, Sprite> getUISprite;

	public string parentTechId;

	public string[] dlcIds;

	public Tech ParentTech => Db.Get().Techs.Get(parentTechId);

	public TechItem(string id, ResourceSet parent, string name, string description, Func<string, bool, Sprite> getUISprite, string parentTechId, string[] dlcIds)
		: base(id, parent, name)
	{
		this.description = description;
		this.getUISprite = getUISprite;
		this.parentTechId = parentTechId;
		this.dlcIds = dlcIds;
	}

	public Sprite UISprite()
	{
		return getUISprite("ui", arg2: false);
	}

	public bool IsComplete()
	{
		return ParentTech.IsComplete();
	}
}
