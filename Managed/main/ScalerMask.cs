using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ScalerMask")]
public class ScalerMask : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public RectTransform SourceTransform;

	private RectTransform _thisTransform;

	private LayoutElement _thisLayoutElement;

	public GameObject hoverIndicator;

	public bool hoverLock = false;

	private bool grandparentIsHovered = false;

	private bool isHovered = false;

	private bool queuedSizeUpdate = true;

	public float topPadding = 0f;

	public float bottomPadding = 0f;

	private RectTransform ThisTransform
	{
		get
		{
			if (_thisTransform == null)
			{
				_thisTransform = GetComponent<RectTransform>();
			}
			return _thisTransform;
		}
	}

	private LayoutElement ThisLayoutElement
	{
		get
		{
			if (_thisLayoutElement == null)
			{
				_thisLayoutElement = GetComponent<LayoutElement>();
			}
			return _thisLayoutElement;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		DetailsScreen componentInParent = GetComponentInParent<DetailsScreen>();
		if ((bool)componentInParent)
		{
			componentInParent.pointerEnterActions = (KScreen.PointerEnterActions)Delegate.Combine(componentInParent.pointerEnterActions, new KScreen.PointerEnterActions(OnPointerEnterGrandparent));
			componentInParent.pointerExitActions = (KScreen.PointerExitActions)Delegate.Combine(componentInParent.pointerExitActions, new KScreen.PointerExitActions(OnPointerExitGrandparent));
		}
	}

	protected override void OnCleanUp()
	{
		DetailsScreen componentInParent = GetComponentInParent<DetailsScreen>();
		if ((bool)componentInParent)
		{
			componentInParent.pointerEnterActions = (KScreen.PointerEnterActions)Delegate.Remove(componentInParent.pointerEnterActions, new KScreen.PointerEnterActions(OnPointerEnterGrandparent));
			componentInParent.pointerExitActions = (KScreen.PointerExitActions)Delegate.Remove(componentInParent.pointerExitActions, new KScreen.PointerExitActions(OnPointerExitGrandparent));
		}
		base.OnCleanUp();
	}

	private void Update()
	{
		if (SourceTransform != null)
		{
			SourceTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ThisTransform.rect.width);
		}
		if (SourceTransform != null && (!hoverLock || !grandparentIsHovered || isHovered || queuedSizeUpdate))
		{
			ThisLayoutElement.minHeight = SourceTransform.rect.height + topPadding + bottomPadding;
			SourceTransform.anchoredPosition = new Vector2(0f, 0f - topPadding);
			queuedSizeUpdate = false;
		}
		if (hoverIndicator != null)
		{
			if (SourceTransform != null && SourceTransform.rect.height > ThisTransform.rect.height)
			{
				hoverIndicator.SetActive(value: true);
			}
			else
			{
				hoverIndicator.SetActive(value: false);
			}
		}
	}

	public void UpdateSize()
	{
		queuedSizeUpdate = true;
	}

	public void OnPointerEnterGrandparent(PointerEventData eventData)
	{
		grandparentIsHovered = true;
	}

	public void OnPointerExitGrandparent(PointerEventData eventData)
	{
		grandparentIsHovered = false;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isHovered = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHovered = false;
	}
}
