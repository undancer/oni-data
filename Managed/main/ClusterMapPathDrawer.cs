using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClusterMapPathDrawer : MonoBehaviour
{
	private List<ClusterMapPath> m_activePaths = new List<ClusterMapPath>();

	private Dictionary<GameObject, ClusterMapPath> m_pathsByRotateParent = new Dictionary<GameObject, ClusterMapPath>();

	public ClusterMapPath pathPrefab;

	public Transform pathContainer;

	public void AddPath(Vector2 start_location, List<AxialI> points, Color color, GameObject parent, bool rotateTransform)
	{
		List<Vector2> list = new List<Vector2>();
		list.Add(start_location);
		list.AddRange(points.Select((AxialI point) => point.ToWorld2D()));
		ClusterMapPath clusterMapPath = Object.Instantiate(pathPrefab, parent.transform);
		clusterMapPath.transform.SetAsFirstSibling();
		clusterMapPath.transform.SetPositionAndRotation(pathContainer.position, Quaternion.identity);
		clusterMapPath.Init(list, color);
		m_activePaths.Add(clusterMapPath);
		if (rotateTransform)
		{
			m_pathsByRotateParent[parent] = clusterMapPath;
		}
	}

	public ClusterMapPath GetPathForRotateParent(GameObject parent)
	{
		ClusterMapPath value = null;
		m_pathsByRotateParent.TryGetValue(parent, out value);
		return value;
	}

	public void RefreshVisiblePaths()
	{
		foreach (ClusterMapPath activePath in m_activePaths)
		{
			activePath.gameObject.SetActive(value: true);
		}
	}

	public void ClearPaths()
	{
		foreach (ClusterMapPath activePath in m_activePaths)
		{
			Util.KDestroyGameObject(activePath);
		}
		m_activePaths.Clear();
		m_pathsByRotateParent.Clear();
	}
}
