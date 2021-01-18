using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConditionListSideScreen : SideScreenContent
{
	public GameObject rowPrefab;

	public GameObject rowContainer;

	[Tooltip("This list is indexed by the ProcessCondition.Status enum")]
	public static Color readyColor = Color.black;

	public static Color failedColor = Color.red;

	public static Color warningColor = new Color(1f, 0.3529412f, 0f, 1f);

	private IProcessConditionSet targetConditionSet;

	private Dictionary<ProcessCondition, GameObject> rows = new Dictionary<ProcessCondition, GameObject>();

	public override bool IsValidForTarget(GameObject target)
	{
		return false;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		if (target != null)
		{
			targetConditionSet = target.GetComponent<IProcessConditionSet>();
		}
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			Refresh();
		}
	}

	private void Refresh()
	{
		bool flag = false;
		List<ProcessCondition> conditionSet = targetConditionSet.GetConditionSet(ProcessCondition.ProcessConditionType.All);
		foreach (ProcessCondition item in conditionSet)
		{
			if (!rows.ContainsKey(item))
			{
				flag = true;
				break;
			}
		}
		foreach (KeyValuePair<ProcessCondition, GameObject> row in rows)
		{
			if (!conditionSet.Contains(row.Key))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			Rebuild();
		}
		foreach (KeyValuePair<ProcessCondition, GameObject> row2 in rows)
		{
			SetRowState(row2.Value, row2.Key);
		}
	}

	public static void SetRowState(GameObject row, ProcessCondition condition)
	{
		HierarchyReferences component = row.GetComponent<HierarchyReferences>();
		ProcessCondition.Status status = condition.EvaluateCondition();
		component.GetReference<LocText>("Label").text = condition.GetStatusMessage(status);
		switch (status)
		{
		case ProcessCondition.Status.Failure:
			component.GetReference<LocText>("Label").color = failedColor;
			component.GetReference<Image>("Box").color = failedColor;
			break;
		case ProcessCondition.Status.Warning:
			component.GetReference<LocText>("Label").color = warningColor;
			component.GetReference<Image>("Box").color = warningColor;
			break;
		case ProcessCondition.Status.Ready:
			component.GetReference<LocText>("Label").color = readyColor;
			component.GetReference<Image>("Box").color = readyColor;
			break;
		}
		component.GetReference<Image>("Check").gameObject.SetActive(status == ProcessCondition.Status.Ready);
		component.GetReference<Image>("Dash").gameObject.SetActive(value: false);
		row.GetComponent<ToolTip>().SetSimpleTooltip(condition.GetStatusTooltip(status));
	}

	private void Rebuild()
	{
		ClearRows();
		BuildRows();
	}

	private void ClearRows()
	{
		foreach (KeyValuePair<ProcessCondition, GameObject> row in rows)
		{
			Util.KDestroyGameObject(row.Value);
		}
		rows.Clear();
	}

	private void BuildRows()
	{
		foreach (ProcessCondition item in targetConditionSet.GetConditionSet(ProcessCondition.ProcessConditionType.All))
		{
			if (item.ShowInUI())
			{
				GameObject value = Util.KInstantiateUI(rowPrefab, rowContainer, force_active: true);
				rows.Add(item, value);
			}
		}
	}
}
