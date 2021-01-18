using System;
using UnityEngine;

public class ButtonLabelColumn : LabelTableColumn
{
	private Action<GameObject> on_click_action;

	private Action<GameObject> on_double_click_action;

	private bool whiteText = false;

	public ButtonLabelColumn(Action<IAssignableIdentity, GameObject> on_load_action, Func<IAssignableIdentity, GameObject, string> get_value_action, Action<GameObject> on_click_action, Action<GameObject> on_double_click_action, Comparison<IAssignableIdentity> sort_comparison, Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip, Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip, bool whiteText = false)
		: base(on_load_action, get_value_action, sort_comparison, on_tooltip, on_sort_tooltip)
	{
		this.on_click_action = on_click_action;
		this.on_double_click_action = on_double_click_action;
		this.whiteText = whiteText;
	}

	public override GameObject GetDefaultWidget(GameObject parent)
	{
		GameObject widget_go = Util.KInstantiateUI(whiteText ? Assets.UIPrefabs.TableScreenWidgets.ButtonLabelWhite : Assets.UIPrefabs.TableScreenWidgets.ButtonLabel, parent, force_active: true);
		if (on_click_action != null)
		{
			widget_go.GetComponent<KButton>().onClick += delegate
			{
				on_click_action(widget_go);
			};
		}
		if (on_double_click_action != null)
		{
			widget_go.GetComponent<KButton>().onDoubleClick += delegate
			{
				on_double_click_action(widget_go);
			};
		}
		return widget_go;
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		return base.GetHeaderWidget(parent);
	}

	public override GameObject GetMinionWidget(GameObject parent)
	{
		GameObject widget_go = Util.KInstantiateUI(whiteText ? Assets.UIPrefabs.TableScreenWidgets.ButtonLabelWhite : Assets.UIPrefabs.TableScreenWidgets.ButtonLabel, parent, force_active: true);
		ToolTip tt = widget_go.GetComponent<ToolTip>();
		tt.OnToolTip = () => GetTooltip(tt);
		if (on_click_action != null)
		{
			widget_go.GetComponent<KButton>().onClick += delegate
			{
				on_click_action(widget_go);
			};
		}
		if (on_double_click_action != null)
		{
			widget_go.GetComponent<KButton>().onDoubleClick += delegate
			{
				on_double_click_action(widget_go);
			};
		}
		return widget_go;
	}
}
