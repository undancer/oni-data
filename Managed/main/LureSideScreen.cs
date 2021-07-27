using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class LureSideScreen : SideScreenContent
{
	protected CreatureLure target_lure;

	public GameObject prefab_toggle;

	public GameObject toggle_container;

	public Dictionary<Tag, MultiToggle> toggles_by_tag = new Dictionary<Tag, MultiToggle>();

	private Dictionary<Tag, string> baitAttractionStrings = new Dictionary<Tag, string>
	{
		{
			GameTags.SlimeMold,
			CREATURES.SPECIES.PUFT.NAME
		},
		{
			GameTags.Phosphorite,
			CREATURES.SPECIES.LIGHTBUG.NAME
		}
	};

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<CreatureLure>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		target_lure = target.GetComponent<CreatureLure>();
		foreach (Tag bait in target_lure.baitTypes)
		{
			Tag key = bait;
			if (!toggles_by_tag.ContainsKey(bait))
			{
				GameObject obj = Util.KInstantiateUI(prefab_toggle, toggle_container, force_active: true);
				Image reference = obj.GetComponent<HierarchyReferences>().GetReference<Image>("FGImage");
				obj.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").text = ElementLoader.GetElement(bait).name;
				reference.sprite = Def.GetUISpriteFromMultiObjectAnim(ElementLoader.GetElement(bait).substance.anim);
				MultiToggle component = obj.GetComponent<MultiToggle>();
				toggles_by_tag.Add(key, component);
			}
			toggles_by_tag[bait].onClick = delegate
			{
				Tag tag = bait;
				SelectToggle(tag);
			};
		}
		RefreshToggles();
	}

	public void SelectToggle(Tag tag)
	{
		if (target_lure.activeBaitSetting != tag)
		{
			target_lure.ChangeBaitSetting(tag);
		}
		else
		{
			target_lure.ChangeBaitSetting(Tag.Invalid);
		}
		RefreshToggles();
	}

	private void RefreshToggles()
	{
		foreach (KeyValuePair<Tag, MultiToggle> item in toggles_by_tag)
		{
			if (target_lure.activeBaitSetting == item.Key)
			{
				item.Value.ChangeState(2);
			}
			else
			{
				item.Value.ChangeState(1);
			}
			item.Value.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.UISIDESCREENS.LURE.ATTRACTS, ElementLoader.GetElement(item.Key).name, baitAttractionStrings[item.Key]));
		}
	}
}
