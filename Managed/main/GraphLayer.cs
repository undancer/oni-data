using UnityEngine;

[RequireComponent(typeof(GraphBase))]
[AddComponentMenu("KMonoBehaviour/scripts/GraphLayer")]
public class GraphLayer : KMonoBehaviour
{
	[MyCmpReq]
	protected GraphBase graph_base;

	public GraphBase graph
	{
		get
		{
			if (graph_base == null)
			{
				graph_base = GetComponent<GraphBase>();
			}
			return graph_base;
		}
	}
}
