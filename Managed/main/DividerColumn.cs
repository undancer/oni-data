using System;
using UnityEngine;

public class DividerColumn : TableColumn
{
	public DividerColumn(Func<bool> revealed = null, string scrollerID = "")
		: base(delegate(IAssignableIdentity minion, GameObject widget_go)
		{
			if (revealed != null)
			{
				if (revealed())
				{
					if (!widget_go.activeSelf)
					{
						widget_go.SetActive(value: true);
					}
				}
				else if (widget_go.activeSelf)
				{
					widget_go.SetActive(value: false);
				}
			}
			else
			{
				widget_go.SetActive(value: true);
			}
		}, null, null, null, revealed, should_refresh_columns: false, scrollerID)
	{
	}

	public override GameObject GetDefaultWidget(GameObject parent)
	{
		return Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.Spacer, parent, force_active: true);
	}

	public override GameObject GetMinionWidget(GameObject parent)
	{
		return Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.Spacer, parent, force_active: true);
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		return Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.Spacer, parent, force_active: true);
	}
}
