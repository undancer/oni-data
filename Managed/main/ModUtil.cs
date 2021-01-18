using System;
using System.Collections.Generic;
using System.IO;
using KMod;
using TUNING;
using UnityEngine;

public static class ModUtil
{
	public static void AddBuildingToPlanScreen(HashedString category, string building_id)
	{
		int num = BUILDINGS.PLANORDER.FindIndex((PlanScreen.PlanInfo x) => x.category == category);
		if (num >= 0)
		{
			(BUILDINGS.PLANORDER[num].data as IList<string>).Add(building_id);
		}
	}

	public static void AddBuildingToHotkeyBuildMenu(HashedString category, string building_id, Action hotkey)
	{
		BuildMenu.DisplayInfo info = BuildMenu.OrderedBuildings.GetInfo(category);
		if (!(info.category != category))
		{
			(info.data as IList<BuildMenu.BuildingInfo>).Add(new BuildMenu.BuildingInfo(building_id, hotkey));
		}
	}

	public static KAnimFile AddKAnimMod(string name, KAnimFile.Mod anim_mod)
	{
		KAnimFile kAnimFile = ScriptableObject.CreateInstance<KAnimFile>();
		kAnimFile.mod = anim_mod;
		kAnimFile.name = name;
		AnimCommandFile animCommandFile = new AnimCommandFile();
		KAnimGroupFile.GroupFile groupFile = new KAnimGroupFile.GroupFile();
		groupFile.groupID = animCommandFile.GetGroupName(kAnimFile);
		groupFile.commandDirectory = "assets/" + name;
		animCommandFile.AddGroupFile(groupFile);
		if (KAnimGroupFile.GetGroupFile().AddAnimMod(groupFile, animCommandFile, kAnimFile) == KAnimGroupFile.AddModResult.Added)
		{
			Assets.ModLoadedKAnims.Add(kAnimFile);
		}
		return kAnimFile;
	}

	public static KAnimFile AddKAnim(string name, TextAsset anim_file, TextAsset build_file, IList<Texture2D> textures)
	{
		KAnimFile kAnimFile = ScriptableObject.CreateInstance<KAnimFile>();
		kAnimFile.Initialize(anim_file, build_file, textures);
		kAnimFile.name = name;
		AnimCommandFile animCommandFile = new AnimCommandFile();
		KAnimGroupFile.GroupFile groupFile = new KAnimGroupFile.GroupFile();
		groupFile.groupID = animCommandFile.GetGroupName(kAnimFile);
		groupFile.commandDirectory = "assets/" + name;
		animCommandFile.AddGroupFile(groupFile);
		KAnimGroupFile.GetGroupFile().AddAnimFile(groupFile, animCommandFile, kAnimFile);
		Assets.ModLoadedKAnims.Add(kAnimFile);
		return kAnimFile;
	}

	public static KAnimFile AddKAnim(string name, TextAsset anim_file, TextAsset build_file, Texture2D texture)
	{
		List<Texture2D> list = new List<Texture2D>();
		list.Add(texture);
		return AddKAnim(name, anim_file, build_file, list);
	}

	public static Substance CreateSubstance(string name, Element.State state, KAnimFile kanim, Material material, Color32 colour, Color32 ui_colour, Color32 conduit_colour)
	{
		return new Substance
		{
			name = name,
			nameTag = TagManager.Create(name),
			elementID = (SimHashes)Hash.SDBMLower(name),
			anim = kanim,
			colour = colour,
			uiColour = ui_colour,
			conduitColour = conduit_colour,
			material = material,
			renderedByWorld = ((state & Element.State.Solid) == Element.State.Solid)
		};
	}

	public static void RegisterForTranslation(Type locstring_tree_root)
	{
		Localization.RegisterForTranslation(locstring_tree_root);
		Localization.GenerateStringsTemplate(locstring_tree_root, Path.Combine(Manager.GetDirectory(), "strings_templates"));
	}
}
