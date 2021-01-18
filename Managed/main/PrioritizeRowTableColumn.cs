using System;
using UnityEngine;

public class PrioritizeRowTableColumn : TableColumn
{
	public object userData;

	private Action<object, int> onChangePriority;

	private Func<object, int, string> onHoverWidget;

	public PrioritizeRowTableColumn(object user_data, Action<object, int> on_change_priority, Func<object, int, string> on_hover_widget)
		: base(null, null)
	{
		userData = user_data;
		onChangePriority = on_change_priority;
		onHoverWidget = on_hover_widget;
	}

	public override GameObject GetMinionWidget(GameObject parent)
	{
		return GetWidget(parent);
	}

	public override GameObject GetDefaultWidget(GameObject parent)
	{
		return GetWidget(parent);
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		return Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.PrioritizeRowHeaderWidget, parent, force_active: true);
	}

	private GameObject GetWidget(GameObject parent)
	{
		GameObject gameObject = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.PrioritizeRowWidget, parent, force_active: true);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		ConfigureButton(component, "UpButton", 1, gameObject);
		ConfigureButton(component, "DownButton", -1, gameObject);
		return gameObject;
	}

	private void ConfigureButton(HierarchyReferences refs, string ref_id, int delta, GameObject widget_go)
	{
		KButton kButton = refs.GetReference(ref_id) as KButton;
		kButton.onClick += delegate
		{
			onChangePriority(widget_go, delta);
		};
		ToolTip component = kButton.GetComponent<ToolTip>();
		if (component != null)
		{
			component.OnToolTip = () => onHoverWidget(widget_go, delta);
		}
	}
}
