using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClusterMapPathDrawer : MonoBehaviour
{
	public ClusterMapPath pathPrefab;

	public Transform pathContainer;

	public ClusterMapPath AddPath()
	{
		ClusterMapPath clusterMapPath = Object.Instantiate(pathPrefab, pathContainer);
		clusterMapPath.Init();
		return clusterMapPath;
	}

	public static List<Vector2> GetDrawPathList(Vector2 startLocation, List<AxialI> pathPoints)
	{
		List<Vector2> list = new List<Vector2>();
		list.Add(startLocation);
		list.AddRange(pathPoints.Select((AxialI point) => point.ToWorld2D()));
		return list;
	}
}
