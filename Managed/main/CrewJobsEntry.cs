using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class CrewJobsEntry : CrewListEntry
{
	[Serializable]
	public struct PriorityButton
	{
		public Button button;

		public GameObject ToggleIcon;

		public ChoreGroup choreGroup;

		public ToolTip tooltip;

		public Image border;

		public Image background;

		public Color baseBorderColor;

		public Color baseBackgroundColor;
	}

	public GameObject Prefab_JobPriorityButton;

	public GameObject Prefab_JobPriorityButtonAllTasks;

	private List<PriorityButton> PriorityButtons = new List<PriorityButton>();

	private PriorityButton AllTasksButton;

	public TextStyleSetting TooltipTextStyle_Title;

	public TextStyleSetting TooltipTextStyle_Ability;

	public TextStyleSetting TooltipTextStyle_AbilityPositiveModifier;

	public TextStyleSetting TooltipTextStyle_AbilityNegativeModifier;

	private bool dirty;

	private CrewJobsScreen.everyoneToggleState rowToggleState;

	public ChoreConsumer consumer { get; private set; }

	public override void Populate(MinionIdentity _identity)
	{
		base.Populate(_identity);
		consumer = _identity.GetComponent<ChoreConsumer>();
		ChoreConsumer choreConsumer = consumer;
		choreConsumer.choreRulesChanged = (System.Action)Delegate.Combine(choreConsumer.choreRulesChanged, new System.Action(Dirty));
		foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
		{
			CreateChoreButton(resource);
		}
		CreateAllTaskButton();
		dirty = true;
	}

	private void CreateChoreButton(ChoreGroup chore_group)
	{
		GameObject gameObject = Util.KInstantiateUI(Prefab_JobPriorityButton, base.transform.gameObject);
		gameObject.GetComponent<OverviewColumnIdentity>().columnID = chore_group.Id;
		gameObject.GetComponent<OverviewColumnIdentity>().Column_DisplayName = chore_group.Name;
		PriorityButton priorityButton = default(PriorityButton);
		priorityButton.button = gameObject.GetComponent<Button>();
		priorityButton.border = gameObject.transform.GetChild(1).GetComponent<Image>();
		priorityButton.baseBorderColor = priorityButton.border.color;
		priorityButton.background = gameObject.transform.GetChild(0).GetComponent<Image>();
		priorityButton.baseBackgroundColor = priorityButton.background.color;
		priorityButton.choreGroup = chore_group;
		priorityButton.ToggleIcon = gameObject.transform.GetChild(2).gameObject;
		priorityButton.tooltip = gameObject.GetComponent<ToolTip>();
		priorityButton.tooltip.OnToolTip = () => OnPriorityButtonTooltip(priorityButton);
		priorityButton.button.onClick.AddListener(delegate
		{
			OnPriorityPress(chore_group);
		});
		PriorityButtons.Add(priorityButton);
	}

	private void CreateAllTaskButton()
	{
		GameObject gameObject = Util.KInstantiateUI(Prefab_JobPriorityButtonAllTasks, base.transform.gameObject);
		gameObject.GetComponent<OverviewColumnIdentity>().columnID = "AllTasks";
		gameObject.GetComponent<OverviewColumnIdentity>().Column_DisplayName = "";
		Button b = gameObject.GetComponent<Button>();
		b.onClick.AddListener(delegate
		{
			ToggleTasksAll(b);
		});
		PriorityButton allTasksButton = default(PriorityButton);
		allTasksButton.button = gameObject.GetComponent<Button>();
		allTasksButton.border = gameObject.transform.GetChild(1).GetComponent<Image>();
		allTasksButton.baseBorderColor = allTasksButton.border.color;
		allTasksButton.background = gameObject.transform.GetChild(0).GetComponent<Image>();
		allTasksButton.baseBackgroundColor = allTasksButton.background.color;
		allTasksButton.ToggleIcon = gameObject.transform.GetChild(2).gameObject;
		allTasksButton.tooltip = gameObject.GetComponent<ToolTip>();
		AllTasksButton = allTasksButton;
	}

	private void ToggleTasksAll(Button button)
	{
		bool flag = rowToggleState != CrewJobsScreen.everyoneToggleState.on;
		string text = "HUD_Click_Deselect";
		if (flag)
		{
			text = "HUD_Click";
		}
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound(text));
		foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
		{
			consumer.SetPermittedByUser(resource, flag);
		}
	}

	private void OnPriorityPress(ChoreGroup chore_group)
	{
		int num = (consumer.IsPermittedByUser(chore_group) ? 1 : 0);
		string text = "HUD_Click";
		if (num != 0)
		{
			text = "HUD_Click_Deselect";
		}
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound(text));
		consumer.SetPermittedByUser(chore_group, !consumer.IsPermittedByUser(chore_group));
	}

	private void Refresh(object data = null)
	{
		if (identity == null)
		{
			dirty = false;
		}
		else
		{
			if (!dirty)
			{
				return;
			}
			Attributes attributes = identity.GetAttributes();
			foreach (PriorityButton priorityButton in PriorityButtons)
			{
				bool flag = consumer.IsPermittedByUser(priorityButton.choreGroup);
				if (priorityButton.ToggleIcon.activeSelf != flag)
				{
					priorityButton.ToggleIcon.SetActive(flag);
				}
				float num = 0f;
				num = Mathf.Min(attributes.Get(priorityButton.choreGroup.attribute).GetTotalValue() / 10f, 1f);
				Color baseBorderColor = priorityButton.baseBorderColor;
				baseBorderColor.r = Mathf.Lerp(priorityButton.baseBorderColor.r, 0.72156864f, num);
				baseBorderColor.g = Mathf.Lerp(priorityButton.baseBorderColor.g, 0.44313726f, num);
				baseBorderColor.b = Mathf.Lerp(priorityButton.baseBorderColor.b, 0.5803922f, num);
				if (priorityButton.border.color != baseBorderColor)
				{
					priorityButton.border.color = baseBorderColor;
				}
				Color color = priorityButton.baseBackgroundColor;
				color.a = Mathf.Lerp(0f, 1f, num);
				bool flag2 = consumer.IsPermittedByTraits(priorityButton.choreGroup);
				if (!flag2)
				{
					color = Color.clear;
					priorityButton.border.color = Color.clear;
					priorityButton.ToggleIcon.SetActive(value: false);
				}
				priorityButton.button.interactable = flag2;
				if (priorityButton.background.color != color)
				{
					priorityButton.background.color = color;
				}
			}
			int num2 = 0;
			int num3 = 0;
			foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
			{
				if (consumer.IsPermittedByTraits(resource))
				{
					num3++;
					if (consumer.IsPermittedByUser(resource))
					{
						num2++;
					}
				}
			}
			if (num2 == 0)
			{
				rowToggleState = CrewJobsScreen.everyoneToggleState.off;
			}
			else if (num2 < num3)
			{
				rowToggleState = CrewJobsScreen.everyoneToggleState.mixed;
			}
			else
			{
				rowToggleState = CrewJobsScreen.everyoneToggleState.on;
			}
			ImageToggleState component = AllTasksButton.ToggleIcon.GetComponent<ImageToggleState>();
			switch (rowToggleState)
			{
			case CrewJobsScreen.everyoneToggleState.mixed:
				component.SetInactive();
				break;
			case CrewJobsScreen.everyoneToggleState.on:
				component.SetActive();
				break;
			case CrewJobsScreen.everyoneToggleState.off:
				component.SetDisabled();
				break;
			}
			dirty = false;
		}
	}

	private string OnPriorityButtonTooltip(PriorityButton b)
	{
		b.tooltip.ClearMultiStringTooltip();
		if (identity != null)
		{
			Attributes attributes = identity.GetAttributes();
			if (attributes != null)
			{
				if (!consumer.IsPermittedByTraits(b.choreGroup))
				{
					string newString = string.Format(UI.TOOLTIPS.JOBSSCREEN_CANNOTPERFORMTASK, consumer.GetComponent<MinionIdentity>().GetProperName());
					b.tooltip.AddMultiStringTooltip(newString, TooltipTextStyle_AbilityNegativeModifier);
					return "";
				}
				b.tooltip.AddMultiStringTooltip(UI.TOOLTIPS.JOBSSCREEN_RELEVANT_ATTRIBUTES, TooltipTextStyle_Ability);
				Klei.AI.Attribute attribute = b.choreGroup.attribute;
				AttributeInstance attributeInstance = attributes.Get(attribute);
				float totalValue = attributeInstance.GetTotalValue();
				TextStyleSetting styleSetting = TooltipTextStyle_Ability;
				if (totalValue > 0f)
				{
					styleSetting = TooltipTextStyle_AbilityPositiveModifier;
				}
				else if (totalValue < 0f)
				{
					styleSetting = TooltipTextStyle_AbilityNegativeModifier;
				}
				b.tooltip.AddMultiStringTooltip(attribute.Name + " " + attributeInstance.GetTotalValue(), styleSetting);
			}
		}
		return "";
	}

	private void LateUpdate()
	{
		Refresh();
	}

	private void OnLevelUp(object data)
	{
		Dirty();
	}

	private void Dirty()
	{
		dirty = true;
		CrewJobsScreen.Instance.Dirty();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (consumer != null)
		{
			ChoreConsumer choreConsumer = consumer;
			choreConsumer.choreRulesChanged = (System.Action)Delegate.Remove(choreConsumer.choreRulesChanged, new System.Action(Dirty));
		}
	}
}
