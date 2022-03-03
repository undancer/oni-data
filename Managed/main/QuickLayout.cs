using UnityEngine;

public class QuickLayout : KMonoBehaviour
{
	private enum LayoutDirection
	{
		TopToBottom,
		BottomToTop,
		LeftToRight,
		RightToLeft
	}

	[Header("Configuration")]
	[SerializeField]
	private int elementSize;

	[SerializeField]
	private int spacing;

	[SerializeField]
	private LayoutDirection layoutDirection;

	[SerializeField]
	private Vector2 offset;

	[SerializeField]
	private RectTransform driveParentRectSize;

	private int _elementSize;

	private int _spacing;

	private LayoutDirection _layoutDirection;

	private Vector2 _offset;

	private int oldActiveChildCount;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ForceUpdate();
	}

	private void OnEnable()
	{
		ForceUpdate();
	}

	private void LateUpdate()
	{
		Run();
	}

	public void ForceUpdate()
	{
		Run(forceUpdate: true);
	}

	private void Run(bool forceUpdate = false)
	{
		forceUpdate = forceUpdate || _elementSize != elementSize;
		forceUpdate = forceUpdate || _spacing != spacing;
		forceUpdate = forceUpdate || _layoutDirection != layoutDirection;
		forceUpdate = forceUpdate || _offset != offset;
		if (forceUpdate)
		{
			_elementSize = elementSize;
			_spacing = spacing;
			_layoutDirection = layoutDirection;
			_offset = offset;
		}
		int num = 0;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			if (base.transform.GetChild(i).gameObject.activeInHierarchy)
			{
				num++;
			}
		}
		if (num != oldActiveChildCount || forceUpdate)
		{
			Layout();
			oldActiveChildCount = num;
		}
	}

	public void Layout()
	{
		Vector3 vector = _offset;
		bool flag = false;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			if (base.transform.GetChild(i).gameObject.activeInHierarchy)
			{
				flag = true;
				base.transform.GetChild(i).rectTransform().anchoredPosition = vector;
				vector += (Vector3)((_elementSize + _spacing) * GetDirectionVector());
			}
		}
		if (!(driveParentRectSize != null))
		{
			return;
		}
		if (!flag)
		{
			if (_layoutDirection == LayoutDirection.BottomToTop || _layoutDirection == LayoutDirection.TopToBottom)
			{
				driveParentRectSize.sizeDelta = new Vector2(Mathf.Abs(driveParentRectSize.sizeDelta.x), 0f);
			}
			else if (_layoutDirection == LayoutDirection.LeftToRight || _layoutDirection == LayoutDirection.LeftToRight)
			{
				driveParentRectSize.sizeDelta = new Vector2(0f, Mathf.Abs(driveParentRectSize.sizeDelta.y));
			}
		}
		else if (_layoutDirection == LayoutDirection.BottomToTop || _layoutDirection == LayoutDirection.TopToBottom)
		{
			driveParentRectSize.sizeDelta = new Vector2(driveParentRectSize.sizeDelta.x, Mathf.Abs(vector.y));
		}
		else if (_layoutDirection == LayoutDirection.LeftToRight || _layoutDirection == LayoutDirection.LeftToRight)
		{
			driveParentRectSize.sizeDelta = new Vector2(Mathf.Abs(vector.x), driveParentRectSize.sizeDelta.y);
		}
	}

	private Vector2 GetDirectionVector()
	{
		Vector2 result = Vector3.zero;
		switch (_layoutDirection)
		{
		case LayoutDirection.TopToBottom:
			result = Vector2.down;
			break;
		case LayoutDirection.BottomToTop:
			result = Vector2.up;
			break;
		case LayoutDirection.LeftToRight:
			result = Vector2.right;
			break;
		case LayoutDirection.RightToLeft:
			result = Vector2.left;
			break;
		}
		return result;
	}
}
