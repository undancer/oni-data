using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/MinionTodoChoreEntry")]
public class MinionTodoChoreEntry : KMonoBehaviour
{
	public Image icon;

	public Image priorityIcon;

	public LocText priorityLabel;

	public LocText label;

	public LocText subLabel;

	public LocText moreLabel;

	public List<Sprite> prioritySprites;

	[SerializeField]
	private ColorStyleSetting buttonColorSettingCurrent;

	[SerializeField]
	private ColorStyleSetting buttonColorSettingStandard;

	private Chore targetChore;

	private IStateMachineTarget lastChoreTarget;

	private PrioritySetting lastPrioritySetting;

	public void SetMoreAmount(int amount)
	{
		if (amount == 0)
		{
			moreLabel.gameObject.SetActive(value: false);
		}
		else
		{
			moreLabel.text = string.Format(UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TRUNCATED_CHORES, amount);
		}
	}

	public void Apply(Chore.Precondition.Context context)
	{
		ChoreConsumer consumer = context.consumerState.consumer;
		if (targetChore == context.chore && context.chore.target == lastChoreTarget && context.chore.masterPriority == lastPrioritySetting)
		{
			return;
		}
		targetChore = context.chore;
		lastChoreTarget = context.chore.target;
		lastPrioritySetting = context.chore.masterPriority;
		string choreName = GameUtil.GetChoreName(context.chore, context.data);
		string text = GameUtil.ChoreGroupsForChoreType(context.chore.choreType);
		string text2 = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.CHORE_TARGET;
		text2 = text2.Replace("{Target}", (context.chore.target.gameObject == consumer.gameObject) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.SELF_LABEL.text : context.chore.target.gameObject.GetProperName());
		if (text != null)
		{
			text2 = text2.Replace("{Groups}", text);
		}
		string text3 = ((context.chore.masterPriority.priority_class == PriorityScreen.PriorityClass.basic) ? context.chore.masterPriority.priority_value.ToString() : "");
		Sprite sprite = ((context.chore.masterPriority.priority_class == PriorityScreen.PriorityClass.basic) ? prioritySprites[context.chore.masterPriority.priority_value - 1] : null);
		ChoreGroup choreGroup = BestPriorityGroup(context, consumer);
		icon.sprite = ((choreGroup != null) ? Assets.GetSprite(choreGroup.sprite) : null);
		label.SetText(choreName);
		subLabel.SetText(text2);
		priorityLabel.SetText(text3);
		priorityIcon.sprite = sprite;
		moreLabel.text = "";
		GetComponent<ToolTip>().SetSimpleTooltip(TooltipForChore(context, consumer));
		KButton componentInChildren = GetComponentInChildren<KButton>();
		componentInChildren.ClearOnClick();
		if (componentInChildren.bgImage != null)
		{
			componentInChildren.bgImage.colorStyleSetting = ((context.chore.driver == consumer.choreDriver) ? buttonColorSettingCurrent : buttonColorSettingStandard);
			componentInChildren.bgImage.ApplyColorStyleSetting();
		}
		_ = context.chore.target.gameObject;
		componentInChildren.ClearOnPointerEvents();
		componentInChildren.GetComponentInChildren<KButton>().onClick += delegate
		{
			if (context.chore != null && !context.chore.target.isNull)
			{
				Vector3 pos = new Vector3(context.chore.target.gameObject.transform.position.x, context.chore.target.gameObject.transform.position.y + 1f, CameraController.Instance.transform.position.z);
				CameraController.Instance.SetTargetPos(pos, 10f, playSound: true);
			}
		};
	}

	private static ChoreGroup BestPriorityGroup(Chore.Precondition.Context context, ChoreConsumer choreConsumer)
	{
		ChoreGroup choreGroup = null;
		if (context.chore.choreType.groups.Length != 0)
		{
			choreGroup = context.chore.choreType.groups[0];
			for (int i = 1; i < context.chore.choreType.groups.Length; i++)
			{
				if (choreConsumer.GetPersonalPriority(choreGroup) < choreConsumer.GetPersonalPriority(context.chore.choreType.groups[i]))
				{
					choreGroup = context.chore.choreType.groups[i];
				}
			}
		}
		return choreGroup;
	}

	private static string TooltipForChore(Chore.Precondition.Context context, ChoreConsumer choreConsumer)
	{
		bool flag = context.chore.masterPriority.priority_class == PriorityScreen.PriorityClass.basic || context.chore.masterPriority.priority_class == PriorityScreen.PriorityClass.high;
		string text = context.chore.masterPriority.priority_class switch
		{
			PriorityScreen.PriorityClass.idle => UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_IDLE, 
			PriorityScreen.PriorityClass.personalNeeds => UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_PERSONAL, 
			PriorityScreen.PriorityClass.topPriority => UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_EMERGENCY, 
			PriorityScreen.PriorityClass.compulsory => UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_COMPULSORY, 
			_ => UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_NORMAL, 
		};
		float num = 0f;
		int num2 = (int)context.chore.masterPriority.priority_class * 100;
		num += (float)num2;
		int num3 = (flag ? choreConsumer.GetPersonalPriority(context.chore.choreType) : 0);
		num += (float)(num3 * 10);
		int num4 = (flag ? context.chore.masterPriority.priority_value : 0);
		num += (float)num4;
		float num5 = (float)context.priority / 10000f;
		num += num5;
		text = text.Replace("{Description}", (context.chore.driver == choreConsumer.choreDriver) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_DESC_ACTIVE : UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_DESC_INACTIVE);
		text = text.Replace("{IdleDescription}", (context.chore.driver == choreConsumer.choreDriver) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_IDLEDESC_ACTIVE : UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_IDLEDESC_INACTIVE);
		string newValue = GameUtil.ChoreGroupsForChoreType(context.chore.choreType);
		ChoreGroup choreGroup = BestPriorityGroup(context, choreConsumer);
		text = text.Replace("{Name}", choreConsumer.name);
		text = text.Replace("{Errand}", GameUtil.GetChoreName(context.chore, context.data));
		text = text.Replace("{Groups}", newValue);
		text = text.Replace("{BestGroup}", (choreGroup != null) ? choreGroup.Name : context.chore.choreType.Name);
		text = text.Replace("{ClassPriority}", num2.ToString());
		text = text.Replace("{PersonalPriority}", JobsTableScreen.priorityInfo[num3].name.text);
		text = text.Replace("{PersonalPriorityValue}", (num3 * 10).ToString());
		text = text.Replace("{Building}", context.chore.gameObject.GetProperName());
		text = text.Replace("{BuildingPriority}", num4.ToString());
		text = text.Replace("{TypePriority}", num5.ToString());
		return text.Replace("{TotalPriority}", num.ToString());
	}
}
