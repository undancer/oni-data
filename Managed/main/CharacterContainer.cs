using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Database;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.UI;

public class CharacterContainer : KScreen, ITelepadDeliverableContainer
{
	[Serializable]
	public struct ProfessionIcon
	{
		public string professionName;

		public Sprite iconImg;
	}

	[SerializeField]
	private GameObject contentBody;

	[SerializeField]
	private LocText characterName;

	[SerializeField]
	private EditableTitleBar characterNameTitle;

	[SerializeField]
	private LocText characterJob;

	public GameObject selectedBorder;

	[SerializeField]
	private Image titleBar;

	[SerializeField]
	private Color selectedTitleColor;

	[SerializeField]
	private Color deselectedTitleColor;

	[SerializeField]
	private KButton reshuffleButton;

	private KBatchedAnimController animController;

	[SerializeField]
	private GameObject iconGroup;

	private List<GameObject> iconGroups;

	[SerializeField]
	private LocText goodTrait;

	[SerializeField]
	private LocText badTrait;

	[SerializeField]
	private GameObject aptitudeEntry;

	[SerializeField]
	private Transform aptitudeLabel;

	[SerializeField]
	private Transform attributeLabelAptitude;

	[SerializeField]
	private Transform attributeLabelTrait;

	[SerializeField]
	private LocText expectationRight;

	private List<LocText> expectationLabels;

	[SerializeField]
	private DropDown archetypeDropDown;

	[SerializeField]
	private Image selectedArchetypeIcon;

	[SerializeField]
	private Sprite noArchetypeIcon;

	[SerializeField]
	private Sprite dropdownArrowIcon;

	private string guaranteedAptitudeID;

	private List<GameObject> aptitudeEntries;

	private List<GameObject> traitEntries;

	[SerializeField]
	private LocText description;

	[SerializeField]
	private KToggle selectButton;

	[SerializeField]
	private KBatchedAnimController fxAnim;

	private MinionStartingStats stats;

	private CharacterSelectionController controller;

	private static List<CharacterContainer> containers;

	private KAnimFile idle_anim;

	[HideInInspector]
	public bool addMinionToIdentityList = true;

	[SerializeField]
	private Sprite enabledSpr;

	private static readonly HashedString[] idleAnims = new HashedString[7]
	{
		"anim_idle_healthy_kanim",
		"anim_idle_susceptible_kanim",
		"anim_idle_keener_kanim",
		"anim_idle_coaster_kanim",
		"anim_idle_fastfeet_kanim",
		"anim_idle_breatherdeep_kanim",
		"anim_idle_breathershallow_kanim"
	};

	public float baseCharacterScale = 0.38f;

	public MinionStartingStats Stats => stats;

	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Initialize();
		characterNameTitle.OnStartedEditing += OnStartedEditing;
		characterNameTitle.OnNameChanged += OnNameChanged;
		reshuffleButton.onClick += delegate
		{
			Reshuffle(is_starter: true);
		};
		List<IListableOption> list = new List<IListableOption>();
		List<SkillGroup> list2 = new List<SkillGroup>(Db.Get().SkillGroups.resources);
		foreach (SkillGroup item in list2)
		{
			list.Add(item);
		}
		archetypeDropDown.Initialize(list, OnArchetypeEntryClick, archetypeDropDownSort, archetypeDropEntryRefreshAction, displaySelectedValueWhenClosed: false);
		archetypeDropDown.CustomizeEmptyRow(Strings.Get("STRINGS.UI.CHARACTERCONTAINER_NOARCHETYPESELECTED"), noArchetypeIcon);
		StartCoroutine(DelayedGeneration());
	}

	public void ForceStopEditingTitle()
	{
		characterNameTitle.ForceStopEditing();
	}

	public override float GetSortKey()
	{
		return 50f;
	}

	private IEnumerator DelayedGeneration()
	{
		yield return new WaitForEndOfFrame();
		GenerateCharacter(controller.IsStarterMinion);
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (animController != null)
		{
			animController.gameObject.DeleteObject();
			animController = null;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (controller != null)
		{
			CharacterSelectionController characterSelectionController = controller;
			characterSelectionController.OnLimitReachedEvent = (System.Action)Delegate.Remove(characterSelectionController.OnLimitReachedEvent, new System.Action(OnCharacterSelectionLimitReached));
			CharacterSelectionController characterSelectionController2 = controller;
			characterSelectionController2.OnLimitUnreachedEvent = (System.Action)Delegate.Remove(characterSelectionController2.OnLimitUnreachedEvent, new System.Action(OnCharacterSelectionLimitUnReached));
			CharacterSelectionController characterSelectionController3 = controller;
			characterSelectionController3.OnReshuffleEvent = (Action<bool>)Delegate.Remove(characterSelectionController3.OnReshuffleEvent, new Action<bool>(Reshuffle));
		}
	}

	private void Initialize()
	{
		iconGroups = new List<GameObject>();
		traitEntries = new List<GameObject>();
		expectationLabels = new List<LocText>();
		aptitudeEntries = new List<GameObject>();
		if (containers == null)
		{
			containers = new List<CharacterContainer>();
		}
		containers.Add(this);
	}

	private void OnNameChanged(string newName)
	{
		stats.Name = newName;
		stats.personality.Name = newName;
		description.text = stats.personality.description;
	}

	private void OnStartedEditing()
	{
		KScreenManager.Instance.RefreshStack();
	}

	private void GenerateCharacter(bool is_starter, string guaranteedAptitudeID = null)
	{
		int num = 0;
		do
		{
			stats = new MinionStartingStats(is_starter, guaranteedAptitudeID);
			num++;
		}
		while (IsCharacterRedundant() && num < 20);
		if (animController != null)
		{
			UnityEngine.Object.Destroy(animController.gameObject);
			animController = null;
		}
		SetAnimator();
		SetInfoText();
		StartCoroutine(SetAttributes());
		selectButton.ClearOnClick();
		if (!controller.IsStarterMinion)
		{
			selectButton.enabled = true;
			selectButton.onClick += delegate
			{
				SelectDeliverable();
			};
		}
	}

	private void SetAnimator()
	{
		if (animController == null)
		{
			animController = Util.KInstantiateUI(Assets.GetPrefab(new Tag("MinionSelectPreview")), contentBody.gameObject).GetComponent<KBatchedAnimController>();
			animController.gameObject.SetActive(value: true);
			animController.animScale = baseCharacterScale;
		}
		stats.ApplyTraits(animController.gameObject);
		stats.ApplyRace(animController.gameObject);
		stats.ApplyAccessories(animController.gameObject);
		stats.ApplyExperience(animController.gameObject);
		HashedString name = idleAnims[UnityEngine.Random.Range(0, idleAnims.Length)];
		idle_anim = Assets.GetAnim(name);
		if (idle_anim != null)
		{
			animController.AddAnimOverrides(idle_anim);
		}
		HashedString name2 = new HashedString("crewSelect_fx_kanim");
		KAnimFile anim = Assets.GetAnim(name2);
		if (anim != null)
		{
			animController.AddAnimOverrides(anim);
		}
		animController.Queue("idle_default", KAnim.PlayMode.Loop);
	}

	private void SetInfoText()
	{
		traitEntries.ForEach(delegate(GameObject tl)
		{
			UnityEngine.Object.Destroy(tl.gameObject);
		});
		traitEntries.Clear();
		characterNameTitle.SetTitle(stats.Name);
		for (int i = 1; i < stats.Traits.Count; i++)
		{
			Trait trait = stats.Traits[i];
			LocText locText = (trait.PositiveTrait ? goodTrait : badTrait);
			LocText locText2 = Util.KInstantiateUI<LocText>(locText.gameObject, locText.transform.parent.gameObject);
			locText2.gameObject.SetActive(value: true);
			locText2.text = stats.Traits[i].Name;
			locText2.color = (trait.PositiveTrait ? Constants.POSITIVE_COLOR : Constants.NEGATIVE_COLOR);
			locText2.GetComponent<ToolTip>().SetSimpleTooltip(trait.GetTooltip());
			for (int j = 0; j < trait.SelfModifiers.Count; j++)
			{
				GameObject gameObject = Util.KInstantiateUI(attributeLabelTrait.gameObject, locText.transform.parent.gameObject);
				gameObject.SetActive(value: true);
				LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
				string format = ((trait.SelfModifiers[j].Value > 0f) ? UI.CHARACTERCONTAINER_ATTRIBUTEMODIFIER_INCREASED : UI.CHARACTERCONTAINER_ATTRIBUTEMODIFIER_DECREASED);
				componentInChildren.text = string.Format(format, Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + trait.SelfModifiers[j].AttributeId.ToUpper() + ".NAME"));
				if (trait.SelfModifiers[j].AttributeId == "GermResistance")
				{
				}
				Klei.AI.Attribute attribute = Db.Get().Attributes.Get(trait.SelfModifiers[j].AttributeId);
				string text = attribute.Description;
				text = string.Concat(text, "\n\n", Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + trait.SelfModifiers[j].AttributeId.ToUpper() + ".NAME"), ": ", trait.SelfModifiers[j].GetFormattedString());
				List<AttributeConverter> convertersForAttribute = Db.Get().AttributeConverters.GetConvertersForAttribute(attribute);
				for (int k = 0; k < convertersForAttribute.Count; k++)
				{
					string text2 = convertersForAttribute[k].DescriptionFromAttribute(convertersForAttribute[k].multiplier * trait.SelfModifiers[j].Value, null);
					if (text2 != "")
					{
						text = text + "\n    • " + text2;
					}
				}
				componentInChildren.GetComponent<ToolTip>().SetSimpleTooltip(text);
				traitEntries.Add(gameObject);
			}
			if (trait.disabledChoreGroups != null)
			{
				GameObject gameObject2 = Util.KInstantiateUI(attributeLabelTrait.gameObject, locText.transform.parent.gameObject);
				gameObject2.SetActive(value: true);
				LocText componentInChildren2 = gameObject2.GetComponentInChildren<LocText>();
				componentInChildren2.text = trait.GetDisabledChoresString(list_entry: false);
				string text3 = "";
				string text4 = "";
				for (int l = 0; l < trait.disabledChoreGroups.Length; l++)
				{
					if (l > 0)
					{
						text3 += ", ";
						text4 += "\n";
					}
					text3 += trait.disabledChoreGroups[l].Name;
					text4 += trait.disabledChoreGroups[l].description;
				}
				componentInChildren2.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(DUPLICANTS.TRAITS.CANNOT_DO_TASK_TOOLTIP, text3, text4));
				traitEntries.Add(gameObject2);
			}
			if (trait.ignoredEffects != null && trait.ignoredEffects.Length != 0)
			{
				GameObject gameObject3 = Util.KInstantiateUI(attributeLabelTrait.gameObject, locText.transform.parent.gameObject);
				gameObject3.SetActive(value: true);
				LocText componentInChildren3 = gameObject3.GetComponentInChildren<LocText>();
				componentInChildren3.text = trait.GetIgnoredEffectsString(list_entry: false);
				string text5 = "";
				string text6 = "";
				for (int m = 0; m < trait.ignoredEffects.Length; m++)
				{
					if (m > 0)
					{
						text5 += ", ";
						text6 += "\n";
					}
					text5 += Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + trait.ignoredEffects[m].ToUpper() + ".NAME");
					text6 += Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + trait.ignoredEffects[m].ToUpper() + ".CAUSE");
				}
				componentInChildren3.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(DUPLICANTS.TRAITS.IGNORED_EFFECTS_TOOLTIP, text5, text6));
				traitEntries.Add(gameObject3);
			}
			if (Strings.TryGet("STRINGS.DUPLICANTS.TRAITS." + trait.Id.ToUpper() + ".SHORT_DESC", out var result))
			{
				GameObject gameObject4 = Util.KInstantiateUI(attributeLabelTrait.gameObject, locText.transform.parent.gameObject);
				gameObject4.SetActive(value: true);
				LocText componentInChildren4 = gameObject4.GetComponentInChildren<LocText>();
				componentInChildren4.text = result.String;
				componentInChildren4.GetComponent<ToolTip>().SetSimpleTooltip(Strings.Get("STRINGS.DUPLICANTS.TRAITS." + trait.Id.ToUpper() + ".SHORT_DESC_TOOLTIP"));
				traitEntries.Add(gameObject4);
			}
			traitEntries.Add(locText2.gameObject);
		}
		aptitudeEntries.ForEach(delegate(GameObject al)
		{
			UnityEngine.Object.Destroy(al.gameObject);
		});
		aptitudeEntries.Clear();
		expectationLabels.ForEach(delegate(LocText el)
		{
			UnityEngine.Object.Destroy(el.gameObject);
		});
		expectationLabels.Clear();
		foreach (KeyValuePair<SkillGroup, float> skillAptitude in stats.skillAptitudes)
		{
			if (skillAptitude.Value == 0f)
			{
				continue;
			}
			SkillGroup skillGroup = Db.Get().SkillGroups.Get(skillAptitude.Key.IdHash);
			if (skillGroup == null)
			{
				Debug.LogWarningFormat("Role group not found for aptitude: {0}", skillAptitude.Key);
				continue;
			}
			GameObject gameObject5 = Util.KInstantiateUI(aptitudeEntry.gameObject, aptitudeEntry.transform.parent.gameObject);
			LocText locText3 = Util.KInstantiateUI<LocText>(aptitudeLabel.gameObject, gameObject5);
			locText3.gameObject.SetActive(value: true);
			locText3.text = skillGroup.Name;
			string text7 = "";
			if (skillGroup.choreGroupID != "")
			{
				ChoreGroup choreGroup = Db.Get().ChoreGroups.Get(skillGroup.choreGroupID);
				text7 = string.Format(DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION_CHOREGROUP, skillGroup.Name, DUPLICANTSTATS.APTITUDE_BONUS, choreGroup.description);
			}
			else
			{
				text7 = string.Format(DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION, skillGroup.Name, DUPLICANTSTATS.APTITUDE_BONUS);
			}
			locText3.GetComponent<ToolTip>().SetSimpleTooltip(text7);
			float num = stats.StartingLevels[skillAptitude.Key.relevantAttributes[0].Id];
			LocText locText4 = Util.KInstantiateUI<LocText>(attributeLabelAptitude.gameObject, gameObject5);
			locText4.gameObject.SetActive(value: true);
			locText4.text = "+" + num + " " + skillAptitude.Key.relevantAttributes[0].Name;
			string text8 = skillAptitude.Key.relevantAttributes[0].Description;
			text8 = text8 + "\n\n" + skillAptitude.Key.relevantAttributes[0].Name + ": +" + num;
			List<AttributeConverter> convertersForAttribute2 = Db.Get().AttributeConverters.GetConvertersForAttribute(skillAptitude.Key.relevantAttributes[0]);
			for (int n = 0; n < convertersForAttribute2.Count; n++)
			{
				text8 = text8 + "\n    • " + convertersForAttribute2[n].DescriptionFromAttribute(convertersForAttribute2[n].multiplier * num, null);
			}
			locText4.GetComponent<ToolTip>().SetSimpleTooltip(text8);
			gameObject5.gameObject.SetActive(value: true);
			aptitudeEntries.Add(gameObject5);
		}
		if (stats.stressTrait != null)
		{
			LocText locText5 = Util.KInstantiateUI<LocText>(expectationRight.gameObject, expectationRight.transform.parent.gameObject);
			locText5.gameObject.SetActive(value: true);
			locText5.text = string.Format(UI.CHARACTERCONTAINER_STRESSTRAIT, stats.stressTrait.Name);
			locText5.GetComponent<ToolTip>().SetSimpleTooltip(stats.stressTrait.GetTooltip());
			expectationLabels.Add(locText5);
		}
		if (stats.joyTrait != null)
		{
			LocText locText6 = Util.KInstantiateUI<LocText>(expectationRight.gameObject, expectationRight.transform.parent.gameObject);
			locText6.gameObject.SetActive(value: true);
			locText6.text = string.Format(UI.CHARACTERCONTAINER_JOYTRAIT, stats.joyTrait.Name);
			locText6.GetComponent<ToolTip>().SetSimpleTooltip(stats.joyTrait.GetTooltip());
			expectationLabels.Add(locText6);
		}
		if (stats.congenitaltrait != null)
		{
			LocText locText7 = Util.KInstantiateUI<LocText>(expectationRight.gameObject, expectationRight.transform.parent.gameObject);
			locText7.gameObject.SetActive(value: true);
			locText7.text = string.Format(UI.CHARACTERCONTAINER_CONGENITALTRAIT, stats.congenitaltrait.Name);
			locText7.GetComponent<ToolTip>().SetSimpleTooltip(stats.congenitaltrait.GetTooltip());
			expectationLabels.Add(locText7);
		}
		description.text = stats.personality.description;
	}

	private IEnumerator SetAttributes()
	{
		yield return null;
		iconGroups.ForEach(delegate(GameObject icg)
		{
			UnityEngine.Object.Destroy(icg);
		});
		iconGroups.Clear();
		Klei.AI.Attributes attr = animController.gameObject.GetAttributes();
		List<AttributeInstance> attributes = new List<AttributeInstance>(attr.AttributeTable);
		attributes.RemoveAll((AttributeInstance at) => at.Attribute.ShowInUI != Klei.AI.Attribute.Display.Skill);
		attributes = attributes.OrderBy((AttributeInstance at) => at.Name).ToList();
		for (int i = 0; i < attributes.Count; i++)
		{
			GameObject newIconGroup = Util.KInstantiateUI(iconGroup.gameObject, iconGroup.transform.parent.gameObject);
			LocText label = newIconGroup.GetComponentInChildren<LocText>();
			newIconGroup.SetActive(value: true);
			float totalValue = attributes[i].GetTotalValue();
			if (totalValue > 0f)
			{
				label.color = Constants.POSITIVE_COLOR;
			}
			else if (totalValue == 0f)
			{
				label.color = Constants.NEUTRAL_COLOR;
			}
			else
			{
				label.color = Constants.NEGATIVE_COLOR;
			}
			label.text = string.Format(UI.CHARACTERCONTAINER_SKILL_VALUE, GameUtil.AddPositiveSign(totalValue.ToString(), totalValue > 0f), attributes[i].Name);
			AttributeInstance attribute = attributes[i];
			string tooltip = attribute.Description;
			if (attribute.Attribute.converters.Count > 0)
			{
				tooltip += "\n";
				foreach (AttributeConverter converter in attribute.Attribute.converters)
				{
					AttributeConverterInstance converter_instance = animController.gameObject.GetComponent<Klei.AI.AttributeConverters>().GetConverter(converter.Id);
					string instance_details = converter_instance.DescriptionFromAttribute(converter_instance.Evaluate(), converter_instance.gameObject);
					if (instance_details != null)
					{
						tooltip = tooltip + "\n" + instance_details;
					}
				}
			}
			newIconGroup.GetComponent<ToolTip>().SetSimpleTooltip(tooltip);
			iconGroups.Add(newIconGroup);
		}
	}

	public void SelectDeliverable()
	{
		if (controller != null)
		{
			controller.AddDeliverable(stats);
		}
		if (MusicManager.instance.SongIsPlaying("Music_SelectDuplicant"))
		{
			MusicManager.instance.SetSongParameter("Music_SelectDuplicant", "songSection", 1f);
		}
		selectButton.GetComponent<ImageToggleState>().SetActive();
		selectButton.ClearOnClick();
		selectButton.onClick += delegate
		{
			DeselectDeliverable();
			if (MusicManager.instance.SongIsPlaying("Music_SelectDuplicant"))
			{
				MusicManager.instance.SetSongParameter("Music_SelectDuplicant", "songSection", 0f);
			}
		};
		selectedBorder.SetActive(value: true);
		titleBar.color = selectedTitleColor;
		animController.Play("cheer_pre");
		animController.Play("cheer_loop", KAnim.PlayMode.Loop);
	}

	public void DeselectDeliverable()
	{
		if (controller != null)
		{
			controller.RemoveDeliverable(stats);
		}
		selectButton.GetComponent<ImageToggleState>().SetInactive();
		selectButton.Deselect();
		selectButton.ClearOnClick();
		selectButton.onClick += delegate
		{
			SelectDeliverable();
		};
		selectedBorder.SetActive(value: false);
		titleBar.color = deselectedTitleColor;
		animController.Queue("cheer_pst");
		animController.Queue("idle_default", KAnim.PlayMode.Loop);
	}

	private void OnReplacedEvent(ITelepadDeliverable deliverable)
	{
		if (deliverable == stats)
		{
			DeselectDeliverable();
		}
	}

	private void OnCharacterSelectionLimitReached()
	{
		if (!(controller != null) || !controller.IsSelected(stats))
		{
			selectButton.ClearOnClick();
			if (controller.AllowsReplacing)
			{
				selectButton.onClick += ReplaceCharacterSelection;
			}
			else
			{
				selectButton.onClick += CantSelectCharacter;
			}
		}
	}

	private void CantSelectCharacter()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
	}

	private void ReplaceCharacterSelection()
	{
		if (!(controller == null))
		{
			controller.RemoveLast();
			SelectDeliverable();
		}
	}

	private void OnCharacterSelectionLimitUnReached()
	{
		if (!(controller != null) || !controller.IsSelected(stats))
		{
			selectButton.ClearOnClick();
			selectButton.onClick += delegate
			{
				SelectDeliverable();
			};
		}
	}

	public void SetReshufflingState(bool enable)
	{
		reshuffleButton.gameObject.SetActive(enable);
		archetypeDropDown.gameObject.SetActive(enable);
	}

	private void Reshuffle(bool is_starter)
	{
		if (controller != null && controller.IsSelected(stats))
		{
			DeselectDeliverable();
		}
		if (fxAnim != null)
		{
			fxAnim.Play("loop");
		}
		GenerateCharacter(is_starter, guaranteedAptitudeID);
	}

	public void SetController(CharacterSelectionController csc)
	{
		if (!(csc == controller))
		{
			controller = csc;
			CharacterSelectionController characterSelectionController = controller;
			characterSelectionController.OnLimitReachedEvent = (System.Action)Delegate.Combine(characterSelectionController.OnLimitReachedEvent, new System.Action(OnCharacterSelectionLimitReached));
			CharacterSelectionController characterSelectionController2 = controller;
			characterSelectionController2.OnLimitUnreachedEvent = (System.Action)Delegate.Combine(characterSelectionController2.OnLimitUnreachedEvent, new System.Action(OnCharacterSelectionLimitUnReached));
			CharacterSelectionController characterSelectionController3 = controller;
			characterSelectionController3.OnReshuffleEvent = (Action<bool>)Delegate.Combine(characterSelectionController3.OnReshuffleEvent, new Action<bool>(Reshuffle));
			CharacterSelectionController characterSelectionController4 = controller;
			characterSelectionController4.OnReplacedEvent = (Action<ITelepadDeliverable>)Delegate.Combine(characterSelectionController4.OnReplacedEvent, new Action<ITelepadDeliverable>(OnReplacedEvent));
		}
	}

	public void DisableSelectButton()
	{
		selectButton.soundPlayer.AcceptClickCondition = () => false;
		selectButton.GetComponent<ImageToggleState>().SetDisabled();
		selectButton.soundPlayer.Enabled = false;
	}

	private bool IsCharacterRedundant()
	{
		return containers.Find((CharacterContainer c) => c != null && c.stats != null && c != this && c.stats.Name == stats.Name && c.stats.IsValid) != null || Components.LiveMinionIdentities.Items.Any((MinionIdentity id) => id.GetProperName() == stats.Name);
	}

	public string GetValueColor(bool isPositive)
	{
		return isPositive ? "<color=green>" : "<color=#ff2222ff>";
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.IsAction(Action.Escape))
		{
			characterNameTitle.ForceStopEditing();
			controller.OnPressBack();
		}
		e.Consumed = true;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		e.Consumed = true;
	}

	protected override void OnCmpEnable()
	{
		base.OnActivate();
		if (stats != null)
		{
			SetAnimator();
		}
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		characterNameTitle.ForceStopEditing();
	}

	private void OnArchetypeEntryClick(IListableOption skill, object data)
	{
		if (skill != null)
		{
			SkillGroup skillGroup = skill as SkillGroup;
			guaranteedAptitudeID = skillGroup.Id;
			selectedArchetypeIcon.sprite = Assets.GetSprite(skillGroup.archetypeIcon);
			Reshuffle(is_starter: true);
		}
		else
		{
			guaranteedAptitudeID = null;
			selectedArchetypeIcon.sprite = dropdownArrowIcon;
			Reshuffle(is_starter: true);
		}
	}

	private int archetypeDropDownSort(IListableOption a, IListableOption b, object targetData)
	{
		if (b.Equals("Random"))
		{
			return -1;
		}
		return b.GetProperName().CompareTo(a.GetProperName());
	}

	private void archetypeDropEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		if (entry.entryData != null)
		{
			SkillGroup skillGroup = entry.entryData as SkillGroup;
			entry.image.sprite = Assets.GetSprite(skillGroup.archetypeIcon);
		}
	}
}
