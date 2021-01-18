using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[AddComponentMenu("KMonoBehaviour/Plugins/KRectStretcher")]
public class KRectStretcher : KMonoBehaviour
{
	public enum ParentSizeReferenceValue
	{
		SizeDelta,
		RectDimensions
	}

	public enum aspectFitOption
	{
		WidthDictatesHeight,
		HeightDictatesWidth,
		EnvelopeParent
	}

	private RectTransform rect;

	private DrivenRectTransformTracker rectTracker;

	public bool StretchX;

	public bool StretchY;

	public float XStretchFactor = 1f;

	public float YStretchFactor = 1f;

	public ParentSizeReferenceValue SizeReferenceMethod;

	public Vector2 Padding;

	public bool lerpToSize;

	public float lerpTime = 1f;

	public LayoutElement OverrideLayoutElement;

	public bool PreserveAspectRatio;

	public float aspectRatioToPreserve = 1f;

	public aspectFitOption AspectFitOption;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		rectTracker = default(DrivenRectTransformTracker);
		UpdateStretching();
	}

	private void Update()
	{
		if (base.transform.parent.hasChanged || (OverrideLayoutElement != null && OverrideLayoutElement.transform.hasChanged))
		{
			UpdateStretching();
		}
	}

	public void UpdateStretching()
	{
		if (rect == null)
		{
			rect = GetComponent<RectTransform>();
		}
		if (rect == null || (base.transform.parent == null && OverrideLayoutElement == null))
		{
			return;
		}
		RectTransform rectTransform = base.transform.parent.rectTransform();
		Vector3 vector = Vector3.zero;
		if (SizeReferenceMethod == ParentSizeReferenceValue.SizeDelta)
		{
			vector = rectTransform.sizeDelta;
		}
		else if (SizeReferenceMethod == ParentSizeReferenceValue.RectDimensions)
		{
			vector = rectTransform.rect.size;
		}
		Vector2 vector2 = Vector2.zero;
		if (!PreserveAspectRatio)
		{
			vector2 = new Vector2(StretchX ? vector.x : rect.sizeDelta.x, StretchY ? vector.y : rect.sizeDelta.y);
		}
		else
		{
			switch (AspectFitOption)
			{
			case aspectFitOption.WidthDictatesHeight:
				vector2 = new Vector2(StretchX ? vector.x : rect.sizeDelta.x, StretchY ? (vector.x / aspectRatioToPreserve) : rect.sizeDelta.y);
				break;
			case aspectFitOption.HeightDictatesWidth:
				vector2 = new Vector2(StretchX ? (vector.y * aspectRatioToPreserve) : rect.sizeDelta.x, StretchY ? vector.y : rect.sizeDelta.y);
				break;
			case aspectFitOption.EnvelopeParent:
				vector2 = ((!(rectTransform.sizeDelta.x / rectTransform.sizeDelta.y > aspectRatioToPreserve)) ? new Vector2(StretchX ? (vector.y * aspectRatioToPreserve) : rect.sizeDelta.x, StretchY ? vector.y : rect.sizeDelta.y) : new Vector2(StretchX ? vector.x : rect.sizeDelta.x, StretchY ? (vector.x / aspectRatioToPreserve) : rect.sizeDelta.y));
				break;
			}
		}
		if (StretchX)
		{
			vector2.x *= XStretchFactor;
		}
		if (StretchY)
		{
			vector2.y *= YStretchFactor;
		}
		if (StretchX)
		{
			vector2.x += Padding.x;
		}
		if (StretchY)
		{
			vector2.y += Padding.y;
		}
		if (rect.sizeDelta != vector2)
		{
			if (lerpToSize)
			{
				if (OverrideLayoutElement != null)
				{
					if (StretchX)
					{
						OverrideLayoutElement.minWidth = Mathf.Lerp(OverrideLayoutElement.minWidth, vector2.x, Time.unscaledDeltaTime * lerpTime);
					}
					if (StretchY)
					{
						OverrideLayoutElement.minHeight = Mathf.Lerp(OverrideLayoutElement.minHeight, vector2.y, Time.unscaledDeltaTime * lerpTime);
					}
				}
				else
				{
					rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, vector2, lerpTime * Time.unscaledDeltaTime);
				}
			}
			else
			{
				if (OverrideLayoutElement != null)
				{
					if (StretchX)
					{
						OverrideLayoutElement.minWidth = vector2.x;
					}
					if (StretchY)
					{
						OverrideLayoutElement.minHeight = vector2.y;
					}
				}
				rect.sizeDelta = vector2;
			}
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			KRectStretcher component = base.transform.GetChild(i).GetComponent<KRectStretcher>();
			if ((bool)component)
			{
				component.UpdateStretching();
			}
		}
		rectTracker.Clear();
		if (StretchX)
		{
			rectTracker.Add(this, rect, DrivenTransformProperties.SizeDeltaX);
		}
		if (StretchY)
		{
			rectTracker.Add(this, rect, DrivenTransformProperties.SizeDeltaY);
		}
	}
}
