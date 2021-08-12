using UnityEngine;
using UnityEngine.UI;

public class KChildFitter : MonoBehaviour
{
	public bool fitWidth;

	public bool fitHeight;

	public float HeightPadding;

	public float WidthPadding;

	public float WidthScale = 1f;

	public float HeightScale = 1f;

	public LayoutElement overrideLayoutElement;

	private RectTransform rect_transform;

	private VerticalLayoutGroup VLG;

	private HorizontalLayoutGroup HLG;

	private GridLayoutGroup GLG;

	public bool findTotalBounds = true;

	public bool includeLayoutGroupPadding = true;

	private void Awake()
	{
		rect_transform = GetComponent<RectTransform>();
		VLG = GetComponent<VerticalLayoutGroup>();
		HLG = GetComponent<HorizontalLayoutGroup>();
		GLG = GetComponent<GridLayoutGroup>();
		if (overrideLayoutElement == null)
		{
			overrideLayoutElement = GetComponent<LayoutElement>();
		}
	}

	private void LateUpdate()
	{
		FitSize();
	}

	public Vector2 GetPositionRelativeToTopLeftPivot(RectTransform element)
	{
		Vector2 zero = Vector2.zero;
		zero.x = element.anchoredPosition.x - element.sizeDelta.x * element.pivot.x;
		zero.y = element.anchoredPosition.y + element.sizeDelta.y * (1f - element.pivot.y);
		return zero;
	}

	public void FitSize()
	{
		if (!fitWidth && !fitHeight)
		{
			return;
		}
		Vector2 sizeDelta = rect_transform.sizeDelta;
		if (fitWidth)
		{
			sizeDelta.x = 0f;
		}
		if (fitHeight)
		{
			sizeDelta.y = 0f;
		}
		float num = float.NegativeInfinity;
		float num2 = float.PositiveInfinity;
		float num3 = float.PositiveInfinity;
		float num4 = float.NegativeInfinity;
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			LayoutElement component = child.gameObject.GetComponent<LayoutElement>();
			if ((!(component == null) && component.ignoreLayout) || !child.gameObject.activeSelf)
			{
				continue;
			}
			RectTransform rectTransform = child as RectTransform;
			if (fitWidth)
			{
				if (findTotalBounds)
				{
					float num5 = GetPositionRelativeToTopLeftPivot(rectTransform).x + rectTransform.sizeDelta.x;
					if (num5 > num4)
					{
						num4 = num5;
					}
					float x = GetPositionRelativeToTopLeftPivot(rectTransform).x;
					if (x < num3)
					{
						num3 = x;
					}
					sizeDelta.x = Mathf.Abs(num4 - num3);
					if (includeLayoutGroupPadding)
					{
						sizeDelta.x += ((VLG != null) ? (VLG.padding.left + VLG.padding.right) : 0);
						sizeDelta.x += ((HLG != null) ? (HLG.padding.left + HLG.padding.right) : 0);
						sizeDelta.x += ((GLG != null) ? (GLG.padding.left + GLG.padding.right) : 0);
					}
				}
				else
				{
					sizeDelta.x += rectTransform.sizeDelta.x;
					if ((bool)HLG)
					{
						sizeDelta.x += HLG.spacing;
					}
				}
			}
			if (!fitHeight)
			{
				continue;
			}
			if (findTotalBounds)
			{
				if (GetPositionRelativeToTopLeftPivot(rectTransform).y > num)
				{
					num = GetPositionRelativeToTopLeftPivot(rectTransform).y;
				}
				if (GetPositionRelativeToTopLeftPivot(rectTransform).y - rectTransform.sizeDelta.y < num2)
				{
					num2 = GetPositionRelativeToTopLeftPivot(rectTransform).y - rectTransform.sizeDelta.y;
				}
				sizeDelta.y = Mathf.Abs(num - num2);
				if (includeLayoutGroupPadding)
				{
					sizeDelta.y += ((VLG != null) ? (VLG.padding.bottom + VLG.padding.top) : 0);
					sizeDelta.y += ((HLG != null) ? (HLG.padding.bottom + HLG.padding.top) : 0);
					sizeDelta.y += ((GLG != null) ? (GLG.padding.bottom + GLG.padding.top) : 0);
				}
			}
			else
			{
				sizeDelta.y += rectTransform.sizeDelta.y;
				if ((bool)VLG)
				{
					sizeDelta.y += VLG.spacing;
				}
			}
		}
		Vector2 vector = new Vector2(WidthPadding, HeightPadding);
		if (!fitWidth)
		{
			WidthPadding = 0f;
		}
		if (!fitHeight)
		{
			HeightPadding = 0f;
		}
		if (overrideLayoutElement != null)
		{
			if (fitWidth && overrideLayoutElement.minWidth != (sizeDelta.x + vector.x) * WidthScale)
			{
				overrideLayoutElement.minWidth = (sizeDelta.x + vector.x) * WidthScale;
			}
			if (fitHeight && overrideLayoutElement.minHeight != (sizeDelta.y + vector.y) * HeightScale)
			{
				overrideLayoutElement.minHeight = (sizeDelta.y + vector.y) * HeightScale;
			}
		}
		Vector2 vector2 = new Vector2(WidthScale * (sizeDelta.x + vector.x), HeightScale * (sizeDelta.y + vector.y));
		if (!(rect_transform.sizeDelta != vector2))
		{
			return;
		}
		rect_transform.sizeDelta = vector2;
		if (base.transform.parent != null)
		{
			KChildFitter component2 = base.transform.parent.GetComponent<KChildFitter>();
			if (component2 != null)
			{
				component2.FitSize();
			}
		}
	}
}
