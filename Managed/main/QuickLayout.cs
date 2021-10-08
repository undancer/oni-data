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

	private int _elementSize;

	private int _spacing;

	private LayoutDirection _layoutDirection;

	private Vector2 _offset;

	private int oldActiveChildCount;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Run();
	}

	private void Update()
	{
		Run();
	}

	private void Run()
	{
		bool flag = false || _elementSize != elementSize || _spacing != spacing || _layoutDirection != layoutDirection || _offset != offset;
		if (flag)
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
		if (num != oldActiveChildCount || flag)
		{
			Layout();
			oldActiveChildCount = num;
		}
	}

	public void Layout()
	{
		Vector3 vector = _offset;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			if (base.transform.GetChild(i).gameObject.activeInHierarchy)
			{
				base.transform.GetChild(i).rectTransform().anchoredPosition = vector;
				vector += (Vector3)((_elementSize + _spacing) * GetDirectionVector());
			}
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
