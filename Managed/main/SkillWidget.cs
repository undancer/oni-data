using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

[AddComponentMenu("KMonoBehaviour/scripts/SkillWidget")]
public class SkillWidget : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler
{
	[SerializeField]
	private LocText Name;

	[SerializeField]
	private LocText Description;

	[SerializeField]
	private Image TitleBarBG;

	[SerializeField]
	private SkillsScreen skillsScreen;

	[SerializeField]
	private ToolTip tooltip;

	[SerializeField]
	private RectTransform lines_left;

	[SerializeField]
	public RectTransform lines_right;

	[SerializeField]
	private Color header_color_has_skill;

	[SerializeField]
	private Color header_color_can_assign;

	[SerializeField]
	private Color header_color_disabled;

	[SerializeField]
	private Color line_color_default;

	[SerializeField]
	private Color line_color_active;

	[SerializeField]
	private Image hatImage;

	[SerializeField]
	private GameObject borderHighlight;

	[SerializeField]
	private ToolTip masteryCount;

	[SerializeField]
	private GameObject aptitudeBox;

	[SerializeField]
	private GameObject grantedBox;

	[SerializeField]
	private GameObject traitDisabledIcon;

	public TextStyleSetting TooltipTextStyle_Header;

	public TextStyleSetting TooltipTextStyle_AbilityNegativeModifier;

	private List<SkillWidget> prerequisiteSkillWidgets = new List<SkillWidget>();

	private UILineRenderer[] lines;

	private List<Vector2> linePoints = new List<Vector2>();

	public Material defaultMaterial;

	public Material desaturatedMaterial;

	public string skillID { get; private set; }

	public void Refresh(string skillID)
	{
		Skill skill = Db.Get().Skills.Get(skillID);
		if (skill == null)
		{
			Debug.LogWarning("DbSkills is missing skillId " + skillID);
			return;
		}
		Name.text = skill.Name;
		LocText locText = Name;
		locText.text = locText.text + "\n(" + Db.Get().SkillGroups.Get(skill.skillGroup).Name + ")";
		this.skillID = skillID;
		tooltip.SetSimpleTooltip(SkillTooltip(skill));
		skillsScreen.GetMinionIdentity(skillsScreen.CurrentlySelectedMinion, out var minionIdentity, out var storedMinionIdentity);
		MinionResume minionResume = null;
		if (minionIdentity != null)
		{
			minionResume = minionIdentity.GetComponent<MinionResume>();
			MinionResume.SkillMasteryConditions[] skillMasteryConditions = minionResume.GetSkillMasteryConditions(skillID);
			bool flag = minionResume.CanMasterSkill(skillMasteryConditions);
			if (!(minionResume == null) && (minionResume.HasMasteredSkill(skillID) || flag))
			{
				TitleBarBG.color = (minionResume.HasMasteredSkill(skillID) ? header_color_has_skill : header_color_can_assign);
				hatImage.material = defaultMaterial;
			}
			else
			{
				TitleBarBG.color = header_color_disabled;
				hatImage.material = desaturatedMaterial;
			}
		}
		else if (storedMinionIdentity != null)
		{
			if (storedMinionIdentity.HasMasteredSkill(skillID))
			{
				TitleBarBG.color = header_color_has_skill;
				hatImage.material = defaultMaterial;
			}
			else
			{
				TitleBarBG.color = header_color_disabled;
				hatImage.material = desaturatedMaterial;
			}
		}
		hatImage.sprite = Assets.GetSprite(skill.badge);
		bool active = false;
		bool flag2 = false;
		if (minionResume != null)
		{
			flag2 = minionResume.HasBeenGrantedSkill(skill);
			minionResume.AptitudeBySkillGroup.TryGetValue(skill.skillGroup, out var value);
			active = value > 0f && !flag2;
		}
		aptitudeBox.SetActive(active);
		grantedBox.SetActive(flag2);
		traitDisabledIcon.SetActive(minionResume != null && !minionResume.IsAbleToLearnSkill(skill.Id));
		string text = "";
		List<string> list = new List<string>();
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			MinionResume component = item.GetComponent<MinionResume>();
			if (component != null && component.HasMasteredSkill(skillID))
			{
				list.Add(component.GetProperName());
			}
		}
		foreach (MinionStorage item2 in Components.MinionStorages.Items)
		{
			foreach (MinionStorage.Info item3 in item2.GetStoredMinionInfo())
			{
				if (item3.serializedMinion != null)
				{
					StoredMinionIdentity storedMinionIdentity2 = item3.serializedMinion.Get<StoredMinionIdentity>();
					if (storedMinionIdentity2 != null && storedMinionIdentity2.HasMasteredSkill(skillID))
					{
						list.Add(storedMinionIdentity2.GetProperName());
					}
				}
			}
		}
		masteryCount.gameObject.SetActive(list.Count > 0);
		foreach (string item4 in list)
		{
			text = text + "\n    • " + item4;
		}
		masteryCount.SetSimpleTooltip((list.Count > 0) ? string.Format(UI.ROLES_SCREEN.WIDGET.NUMBER_OF_MASTERS_TOOLTIP, text) : UI.ROLES_SCREEN.WIDGET.NO_MASTERS_TOOLTIP.text);
		masteryCount.GetComponentInChildren<LocText>().text = list.Count.ToString();
	}

	public void RefreshLines()
	{
		prerequisiteSkillWidgets.Clear();
		List<Vector2> list = new List<Vector2>();
		foreach (string priorSkill in Db.Get().Skills.Get(skillID).priorSkills)
		{
			list.Add(skillsScreen.GetSkillWidgetLineTargetPosition(priorSkill));
			prerequisiteSkillWidgets.Add(skillsScreen.GetSkillWidget(priorSkill));
		}
		if (lines != null)
		{
			for (int num = lines.Length - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(lines[num].gameObject);
			}
		}
		linePoints.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			float num2 = lines_left.GetPosition().x - list[i].x - 12f;
			float y = 0f;
			linePoints.Add(new Vector2(0f, y));
			linePoints.Add(new Vector2(0f - num2, y));
			linePoints.Add(new Vector2(0f - num2, y));
			linePoints.Add(new Vector2(0f - num2, 0f - (lines_left.GetPosition().y - list[i].y)));
			linePoints.Add(new Vector2(0f - num2, 0f - (lines_left.GetPosition().y - list[i].y)));
			linePoints.Add(new Vector2(0f - (lines_left.GetPosition().x - list[i].x), 0f - (lines_left.GetPosition().y - list[i].y)));
		}
		lines = new UILineRenderer[linePoints.Count / 2];
		int num3 = 0;
		for (int j = 0; j < linePoints.Count; j += 2)
		{
			GameObject gameObject = new GameObject("Line");
			gameObject.AddComponent<RectTransform>();
			gameObject.transform.SetParent(lines_left.transform);
			gameObject.transform.SetLocalPosition(Vector3.zero);
			gameObject.rectTransform().sizeDelta = Vector2.zero;
			lines[num3] = gameObject.AddComponent<UILineRenderer>();
			lines[num3].color = new Color(0.6509804f, 0.6509804f, 0.6509804f, 1f);
			lines[num3].Points = new Vector2[2]
			{
				linePoints[j],
				linePoints[j + 1]
			};
			num3++;
		}
	}

	public void ToggleBorderHighlight(bool on)
	{
		borderHighlight.SetActive(on);
		if (lines != null)
		{
			UILineRenderer[] array = lines;
			foreach (UILineRenderer obj in array)
			{
				obj.color = (on ? line_color_active : line_color_default);
				obj.LineThickness = (on ? 4 : 2);
				obj.SetAllDirty();
			}
		}
		for (int j = 0; j < prerequisiteSkillWidgets.Count; j++)
		{
			prerequisiteSkillWidgets[j].ToggleBorderHighlight(on);
		}
	}

	public string SkillTooltip(Skill skill)
	{
		return string.Concat("" + SkillPerksString(skill), "\n", DuplicantSkillString(skill));
	}

	public static string SkillPerksString(Skill skill)
	{
		string text = "";
		foreach (SkillPerk perk in skill.perks)
		{
			if (!string.IsNullOrEmpty(text))
			{
				text += "\n";
			}
			text = text + "• " + perk.Name;
		}
		return text;
	}

	public string CriteriaString(Skill skill)
	{
		bool flag = false;
		string text = "";
		text = string.Concat(text, "<b>", UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.TITLE, "</b>\n");
		SkillGroup skillGroup = Db.Get().SkillGroups.Get(skill.skillGroup);
		if (skillGroup != null && skillGroup.relevantAttributes != null)
		{
			foreach (Klei.AI.Attribute relevantAttribute in skillGroup.relevantAttributes)
			{
				if (relevantAttribute != null)
				{
					text = text + "    • " + string.Format(UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.SKILLGROUP_ENABLED.DESCRIPTION, relevantAttribute.Name) + "\n";
					flag = true;
				}
			}
		}
		if (skill.priorSkills.Count > 0)
		{
			flag = true;
			for (int i = 0; i < skill.priorSkills.Count; i++)
			{
				text = text + "    • " + $"{Db.Get().Skills.Get(skill.priorSkills[i]).Name}";
				text += "</color>";
				if (i != skill.priorSkills.Count - 1)
				{
					text += "\n";
				}
			}
		}
		if (!flag)
		{
			text = text + "    • " + string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.NONE, skill.Name);
		}
		return text;
	}

	public string DuplicantSkillString(Skill skill)
	{
		string text = "";
		skillsScreen.GetMinionIdentity(skillsScreen.CurrentlySelectedMinion, out var minionIdentity, out var _);
		if (minionIdentity != null)
		{
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			if (component == null)
			{
				return "";
			}
			LocString cAN_MASTER = UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.CAN_MASTER;
			if (component.HasMasteredSkill(skill.Id))
			{
				if (component.HasBeenGrantedSkill(skill))
				{
					text += "\n";
					cAN_MASTER = UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.SKILL_GRANTED;
					text += string.Format(cAN_MASTER, minionIdentity.GetProperName(), skill.Name);
				}
			}
			else
			{
				MinionResume.SkillMasteryConditions[] skillMasteryConditions = component.GetSkillMasteryConditions(skill.Id);
				if (!component.CanMasterSkill(skillMasteryConditions))
				{
					bool flag = false;
					text += "\n";
					cAN_MASTER = UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.CANNOT_MASTER;
					text += string.Format(cAN_MASTER, minionIdentity.GetProperName(), skill.Name);
					if (Array.Exists(skillMasteryConditions, (MinionResume.SkillMasteryConditions element) => element == MinionResume.SkillMasteryConditions.UnableToLearn))
					{
						flag = true;
						string choreGroupID = Db.Get().SkillGroups.Get(skill.skillGroup).choreGroupID;
						minionIdentity.GetComponent<Traits>().IsChoreGroupDisabled(choreGroupID, out var disablingTrait);
						text += "\n";
						cAN_MASTER = UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.PREVENTED_BY_TRAIT;
						text += string.Format(cAN_MASTER, disablingTrait.Name);
					}
					if (!flag && Array.Exists(skillMasteryConditions, (MinionResume.SkillMasteryConditions element) => element == MinionResume.SkillMasteryConditions.MissingPreviousSkill))
					{
						text += "\n";
						cAN_MASTER = UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.REQUIRES_PREVIOUS_SKILLS;
						text += string.Format(cAN_MASTER);
					}
					if (!flag && Array.Exists(skillMasteryConditions, (MinionResume.SkillMasteryConditions element) => element == MinionResume.SkillMasteryConditions.NeedsSkillPoints))
					{
						text += "\n";
						cAN_MASTER = UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.REQUIRES_MORE_SKILL_POINTS;
						text += string.Format(cAN_MASTER);
					}
				}
				else
				{
					if (Array.Exists(skillMasteryConditions, (MinionResume.SkillMasteryConditions element) => element == MinionResume.SkillMasteryConditions.StressWarning))
					{
						text += "\n";
						cAN_MASTER = UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.STRESS_WARNING_MESSAGE;
						text += string.Format(cAN_MASTER, skill.Name, minionIdentity.GetProperName());
					}
					if (Array.Exists(skillMasteryConditions, (MinionResume.SkillMasteryConditions element) => element == MinionResume.SkillMasteryConditions.SkillAptitude))
					{
						text += "\n";
						cAN_MASTER = UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.SKILL_APTITUDE;
						text += string.Format(cAN_MASTER, minionIdentity.GetProperName(), skill.Name);
					}
				}
			}
		}
		return text;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ToggleBorderHighlight(on: true);
		skillsScreen.HoverSkill(skillID);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ToggleBorderHighlight(on: false);
		skillsScreen.HoverSkill(null);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		skillsScreen.GetMinionIdentity(skillsScreen.CurrentlySelectedMinion, out var minionIdentity, out var _);
		if (minionIdentity != null)
		{
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			if (DebugHandler.InstantBuildMode && component.AvailableSkillpoints < 1)
			{
				component.ForceAddSkillPoint();
			}
			MinionResume.SkillMasteryConditions[] skillMasteryConditions = component.GetSkillMasteryConditions(skillID);
			bool flag = component.CanMasterSkill(skillMasteryConditions);
			if (component != null && !component.HasMasteredSkill(skillID) && flag)
			{
				component.MasterSkill(skillID);
				skillsScreen.RefreshAll();
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		skillsScreen.GetMinionIdentity(skillsScreen.CurrentlySelectedMinion, out var minionIdentity, out var _);
		MinionResume minionResume = null;
		bool flag = false;
		if (minionIdentity != null)
		{
			minionResume = minionIdentity.GetComponent<MinionResume>();
			MinionResume.SkillMasteryConditions[] skillMasteryConditions = minionResume.GetSkillMasteryConditions(skillID);
			flag = minionResume.CanMasterSkill(skillMasteryConditions);
		}
		if (minionResume != null && !minionResume.HasMasteredSkill(skillID) && flag)
		{
			KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Click"));
		}
		else
		{
			KFMOD.PlayUISound(GlobalAssets.GetSound("Negative"));
		}
	}
}
