using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

[Serializable]
[AddComponentMenu("KMonoBehaviour/scripts/GraphedLine")]
public class GraphedLine : KMonoBehaviour
{
	public UILineRenderer line_renderer;

	public LineLayer layer;

	private Vector2[] points = new Vector2[0];

	[SerializeField]
	private GameObject highlightPoint;

	public int PointCount => points.Length;

	public void SetPoints(Vector2[] points)
	{
		this.points = points;
		UpdatePoints();
	}

	private void UpdatePoints()
	{
		Vector2[] array = new Vector2[points.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = layer.graph.GetRelativePosition(points[i]);
		}
		line_renderer.Points = array;
	}

	public Vector2 GetClosestDataToPointOnXAxis(Vector2 toPoint)
	{
		float num = toPoint.x / layer.graph.rectTransform().sizeDelta.x;
		float num2 = layer.graph.axis_x.min_value + layer.graph.axis_x.range * num;
		Vector2 result = Vector2.zero;
		Vector2[] array = points;
		for (int i = 0; i < array.Length; i++)
		{
			Vector2 vector = array[i];
			if (Mathf.Abs(vector.x - num2) < Mathf.Abs(result.x - num2))
			{
				result = vector;
			}
		}
		return result;
	}

	public void HidePointHighlight()
	{
		highlightPoint.SetActive(value: false);
	}

	public void SetPointHighlight(Vector2 point)
	{
		highlightPoint.SetActive(value: true);
		Vector2 relativePosition = layer.graph.GetRelativePosition(point);
		highlightPoint.rectTransform().SetLocalPosition(new Vector2(relativePosition.x * layer.graph.rectTransform().sizeDelta.x - layer.graph.rectTransform().sizeDelta.x / 2f, relativePosition.y * layer.graph.rectTransform().sizeDelta.y - layer.graph.rectTransform().sizeDelta.y / 2f));
		ToolTip component = layer.graph.GetComponent<ToolTip>();
		component.ClearMultiStringTooltip();
		component.tooltipPositionOffset = new Vector2(highlightPoint.rectTransform().localPosition.x, layer.graph.rectTransform().rect.height / 2f - 12f);
		component.SetSimpleTooltip(layer.graph.axis_x.name + " " + point.x + ", " + Mathf.RoundToInt(point.y) + " " + layer.graph.axis_y.name);
		ToolTipScreen.Instance.SetToolTip(component);
	}
}
