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

	public void Init()
	{
		lineRenderer = base.gameObject.GetComponentInChildren<UILineRenderer>();
		base.gameObject.SetActive(value: true);
	}

	public void Init(List<Vector2> nodes, Color color)
	{
		m_nodes = nodes;
		m_color = color;
		lineRenderer = base.gameObject.GetComponentInChildren<UILineRenderer>();
		UpdateColor();
		UpdateRenderer();
		base.gameObject.SetActive(value: true);
	}

	public void SetColor(Color color)
	{
		m_color = color;
		UpdateColor();
	}

	private void UpdateColor()
	{
		lineRenderer.color = m_color;
		pathStart.color = m_color;
		pathEnd.color = m_color;
	}

	public void SetPoints(List<Vector2> points)
	{
		m_nodes = points;
		UpdateRenderer();
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
			Vector2 vector2 = lineRenderer.Points[lineRenderer.Points.Length - 2];
			pathEnd.transform.localPosition = vector;
			Vector2 vector3 = vector - vector2;
			pathEnd.transform.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
			pathEnd.gameObject.SetActive(value: true);
		}
		else
		{
			pathStart.gameObject.SetActive(value: false);
			pathEnd.gameObject.SetActive(value: false);
		}
	}

	public float GetRotationForNextSegment()
	{
		if (m_nodes.Count > 1)
		{
			Vector2 vector = m_nodes[0];
			Vector2 to = m_nodes[1] - vector;
			return Vector2.SignedAngle(Vector2.up, to);
		}
		return 0f;
	}
}
