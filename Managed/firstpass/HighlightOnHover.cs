using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("KMonoBehaviour/Plugins/HighlightOnHover")]
public class HighlightOnHover : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public KImage image;

	public void OnPointerEnter(PointerEventData data)
	{
		image.ColorState = KImage.ColorSelector.Hover;
	}

	public void OnPointerExit(PointerEventData data)
	{
		image.ColorState = KImage.ColorSelector.Inactive;
	}
}
