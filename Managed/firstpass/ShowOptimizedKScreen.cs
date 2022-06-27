using UnityEngine;

public class ShowOptimizedKScreen : KScreen
{
	public override void Show(bool show = true)
	{
		mouseOver = false;
		Canvas[] componentsInChildren = GetComponentsInChildren<Canvas>(includeInactive: true);
		foreach (Canvas canvas in componentsInChildren)
		{
			if (canvas.enabled != show)
			{
				canvas.enabled = show;
			}
		}
		CanvasGroup component = GetComponent<CanvasGroup>();
		if (component != null)
		{
			component.interactable = show;
			component.blocksRaycasts = show;
			component.ignoreParentGroups = true;
		}
		isHiddenButActive = !show;
		OnShow(show);
	}
}
