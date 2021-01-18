using System;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("KMonoBehaviour/Plugins/HoverCallback")]
public class HoverCallback : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Action<bool> OnHover;

	public void OnPointerEnter(PointerEventData data)
	{
		if (OnHover != null)
		{
			OnHover(obj: true);
		}
	}

	public void OnPointerExit(PointerEventData data)
	{
		if (OnHover != null)
		{
			OnHover(obj: false);
		}
	}
}
