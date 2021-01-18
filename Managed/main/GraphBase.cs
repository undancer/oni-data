using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;

[AddComponentMenu("KMonoBehaviour/scripts/GraphBase")]
public class GraphBase : KMonoBehaviour
{
	[Header("Axis")]
	public GraphAxis axis_x;

	public GraphAxis axis_y;

	[Header("References")]
	public GameObject prefab_guide_x;

	public GameObject prefab_guide_y;

	public GameObject prefab_guide_horizontal_label;

	public GameObject prefab_guide_vertical_label;

	public GameObject guides_x;

	public GameObject guides_y;

	public LocText label_title;

	public LocText label_x;

	public LocText label_y;

	public string graphName;

	protected List<GameObject> horizontalGuides = new List<GameObject>();

	protected List<GameObject> verticalGuides = new List<GameObject>();

	private const int points_per_guide_line = 2;

	public Vector2 GetRelativePosition(Vector2 absolute_point)
	{
		Vector2 zero = Vector2.zero;
		float num = Mathf.Max(1f, axis_x.max_value - axis_x.min_value);
		float num2 = absolute_point.x - axis_x.min_value;
		zero.x = num2 / num;
		float num3 = Mathf.Max(1f, axis_y.max_value - axis_y.min_value);
		float num4 = absolute_point.y - axis_y.min_value;
		zero.y = num4 / num3;
		return zero;
	}

	public Vector2 GetRelativeSize(Vector2 absolute_size)
	{
		return GetRelativePosition(absolute_size);
	}

	public void ClearGuides()
	{
		ClearVerticalGuides();
		ClearHorizontalGuides();
	}

	public void ClearHorizontalGuides()
	{
		foreach (GameObject horizontalGuide in horizontalGuides)
		{
			if (horizontalGuide != null)
			{
				Object.DestroyImmediate(horizontalGuide);
			}
		}
		horizontalGuides.Clear();
	}

	public void ClearVerticalGuides()
	{
		foreach (GameObject verticalGuide in verticalGuides)
		{
			if (verticalGuide != null)
			{
				Object.DestroyImmediate(verticalGuide);
			}
		}
		verticalGuides.Clear();
	}

	public void RefreshGuides()
	{
		ClearGuides();
		RefreshHorizontalGuides();
		RefreshVerticalGuides();
	}

	public void RefreshHorizontalGuides()
	{
		if (!(prefab_guide_x != null))
		{
			return;
		}
		GameObject gameObject = Util.KInstantiateUI(prefab_guide_x, guides_x, force_active: true);
		gameObject.name = "guides_horizontal";
		Vector2[] array = new Vector2[2 * (int)(axis_y.range / axis_y.guide_frequency)];
		for (int i = 0; i < array.Length; i += 2)
		{
			Vector2 absolute_point = new Vector2(axis_x.min_value, (float)i * (axis_y.guide_frequency / 2f));
			array[i] = GetRelativePosition(absolute_point);
			Vector2 absolute_point2 = new Vector2(axis_x.max_value, (float)i * (axis_y.guide_frequency / 2f));
			array[i + 1] = GetRelativePosition(absolute_point2);
			if (prefab_guide_horizontal_label != null)
			{
				GameObject gameObject2 = Util.KInstantiateUI(prefab_guide_horizontal_label, gameObject, force_active: true);
				gameObject2.GetComponent<LocText>().alignment = TextAlignmentOptions.MidlineLeft;
				gameObject2.GetComponent<LocText>().text = ((int)axis_y.guide_frequency * (i / 2)).ToString();
				gameObject2.rectTransform().SetLocalPosition(new Vector2(8f, (float)i * (base.gameObject.rectTransform().rect.height / (float)array.Length)) - base.gameObject.rectTransform().rect.size / 2f);
			}
		}
		gameObject.GetComponent<UILineRenderer>().Points = array;
		horizontalGuides.Add(gameObject);
	}

	public void RefreshVerticalGuides()
	{
		if (!(prefab_guide_y != null))
		{
			return;
		}
		GameObject gameObject = Util.KInstantiateUI(prefab_guide_y, guides_y, force_active: true);
		gameObject.name = "guides_vertical";
		Vector2[] array = new Vector2[2 * (int)(axis_x.range / axis_x.guide_frequency)];
		for (int i = 0; i < array.Length; i += 2)
		{
			Vector2 absolute_point = new Vector2((float)i * (axis_x.guide_frequency / 2f), axis_y.min_value);
			array[i] = GetRelativePosition(absolute_point);
			Vector2 absolute_point2 = new Vector2((float)i * (axis_x.guide_frequency / 2f), axis_y.max_value);
			array[i + 1] = GetRelativePosition(absolute_point2);
			if (prefab_guide_vertical_label != null)
			{
				GameObject gameObject2 = Util.KInstantiateUI(prefab_guide_vertical_label, gameObject, force_active: true);
				gameObject2.GetComponent<LocText>().alignment = TextAlignmentOptions.Bottom;
				gameObject2.GetComponent<LocText>().text = ((int)axis_x.guide_frequency * (i / 2)).ToString();
				gameObject2.rectTransform().SetLocalPosition(new Vector2((float)i * (base.gameObject.rectTransform().rect.width / (float)array.Length), 4f) - base.gameObject.rectTransform().rect.size / 2f);
			}
		}
		gameObject.GetComponent<UILineRenderer>().Points = array;
		verticalGuides.Add(gameObject);
	}
}
