using System.Collections.Generic;
using System.Linq;
using ProcGen;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ClusterMapPath : MonoBehaviour
{
	private List<Vector2> m_nodes;

	private Color m_color;

	public UILineRenderer lineRenderer;

	public Image pathStart;

	public Image pathEnd;

	public void Init(List<Vector2> nodes, Color color)
	{
		m_nodes = nodes;
		m_color = color;
		lineRenderer = base.gameObject.GetComponentInChildren<UILineRenderer>();
		UpdateColor();
		UpdateRenderer();
		base.gameObject.SetActive(value: true);
	}

	private void UpdateColor()
	{
		lineRenderer.color = m_color;
		pathStart.color = m_color;
		pathEnd.color = m_color;
	}

	private void UpdateRenderer()
	{
		HashSet<Vector2> pointsOnCatmullRomSpline = ProcGen.Util.GetPointsOnCatmullRomSpline(m_nodes, 10);
		lineRenderer.Points = pointsOnCatmullRomSpline.ToArray();
		if (lineRenderer.Points.Length > 1)
		{
			pathStart.transform.localPosition = lineRenderer.Points[0];
			pathStart.gameObject.SetActive(value: true);
			Vector2 vector = lineRenderer.Points[lineRenderer.Points.Length - 1];
			Vector2 b = lineRenderer.Points[lineRenderer.Points.Length - 2];
			pathEnd.transform.localPosition = vector;
			Vector2 v = vector - b;
			pathEnd.transform.rotation = Quaternion.LookRotation(Vector3.forward, v);
			pathEnd.gameObject.SetActive(value: true);
		}
		else
		{
			pathStart.gameObject.SetActive(value: false);
			pathEnd.gameObject.SetActive(value: false);
		}
	}

	public void RotateTransformAlongPath(Transform transform)
	{
		if (lineRenderer.Points.Length > 1)
		{
			Vector2 b = lineRenderer.Points[0];
			Vector2 a = lineRenderer.Points[1];
			Vector2 v = a - b;
			transform.rotation = Quaternion.LookRotation(Vector3.forward, v);
		}
		else
		{
			transform.rotation = Quaternion.identity;
		}
	}
}
