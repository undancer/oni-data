using System.Collections.Generic;
using KSerialization;
using UnityEngine;

namespace ProcGen.Map
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class MapGraph : Graph<Cell, Edge>
	{
		[Serialize]
		public List<Corner> cornerList;

		public List<Corner> corners => cornerList;

		public MapGraph(int seed)
			: base(seed)
		{
			cornerList = new List<Corner>();
		}

		public Edge GetEdge(Cell site0, Cell site1)
		{
			return GetArc(site0, site1);
		}

		public Edge AddEdge(Cell site0, Cell site1, Corner corner0, Corner corner1)
		{
			Edge edge = AddArc(site0, site1, "Edge");
			edge.SetCorners(corner0, corner1);
			return edge;
		}

		public Edge AddOrGetEdge(Cell site0, Cell site1, Corner corner0, Corner corner1)
		{
			Edge arc = GetArc(site0, site1);
			if (arc != null)
			{
				return arc;
			}
			arc = AddArc(site0, site1, "Edge");
			arc.SetCorners(corner0, corner1);
			return arc;
		}

		public Corner AddOrGetCorner(Vector2 position)
		{
			Corner corner = cornerList.Find(delegate(Corner c)
			{
				Vector2 vector = c.position - position;
				return vector.x < 1f && vector.x > -1f && vector.y < 1f && vector.y > -1f;
			});
			if (corner == null)
			{
				corner = new Corner(position);
				cornerList.Add(corner);
			}
			return corner;
		}

		public List<Edge> GetEdgesWithTag(Tag tag)
		{
			return GetArcsWithTag(tag);
		}

		public void ClearEdgesAndCorners()
		{
			foreach (Edge arc in arcList)
			{
				base.baseGraph.DeleteArc(arc.arc);
			}
			arcList.Clear();
			cornerList.Clear();
		}

		public void ClearTags()
		{
			foreach (Cell node in nodeList)
			{
				node.tags.Clear();
			}
			foreach (Edge arc in arcList)
			{
				arc.tags.Clear();
			}
		}

		public void Validate()
		{
			for (int i = 0; i < nodeList.Count; i++)
			{
				for (int j = 0; j < nodeList.Count; j++)
				{
					if (j != i)
					{
						if (nodeList[i] == nodeList[j])
						{
							Debug.LogError("Duplicate cell (instance)");
							return;
						}
						if (nodeList[i].position == nodeList[j].position)
						{
							Debug.LogError("Duplicate cell (position)");
							return;
						}
						if (nodeList[i].node == nodeList[j].node)
						{
							Debug.LogError("Duplicate cell (node)");
							return;
						}
					}
				}
			}
			for (int k = 0; k < cornerList.Count; k++)
			{
				for (int l = 0; l < cornerList.Count; l++)
				{
					if (l != k)
					{
						if (cornerList[k] == cornerList[l])
						{
							Debug.LogError("Duplicate corner (instance)");
							return;
						}
						if (cornerList[k].position == cornerList[l].position)
						{
							Debug.LogError("Duplicate corner (position)");
							return;
						}
					}
				}
			}
			for (int m = 0; m < arcList.Count; m++)
			{
				for (int n = 0; n < arcList.Count; n++)
				{
					if (n != m)
					{
						Edge edge = arcList[m];
						Edge edge2 = arcList[n];
						if (edge == edge2)
						{
							Debug.LogError("Duplicate edge (instance)");
							return;
						}
						if (edge.arc == edge2.arc)
						{
							Debug.LogError("Duplicate EDGE [" + edge.arc.ToString() + "] & [" + edge2.arc.ToString() + "] - (ARC)");
							return;
						}
						if (edge.corner0 == edge2.corner0 && edge.corner1 == edge2.corner1)
						{
							Debug.LogError("Duplicate edge (corner same order)");
							return;
						}
						if (edge.corner0 == edge2.corner1 && edge.corner1 == edge2.corner0)
						{
							Debug.LogError("Duplicate edge (corner different order)");
							return;
						}
						List<Cell> nodes = GetNodes(edge);
						List<Cell> nodes2 = GetNodes(edge2);
						if (nodes[0] == nodes2[0] && nodes[1] == nodes2[1])
						{
							Debug.LogError("Duplicate edge (site same order)");
							return;
						}
						if (nodes[0] == nodes2[1] && nodes[1] == nodes2[0])
						{
							Debug.LogError("Duplicate Edge (site differnt order)");
							return;
						}
						if (nodes[0].node == nodes2[0].node && nodes[1].node == nodes2[1].node)
						{
							Debug.LogError("Duplicate edge (site node same order)");
							return;
						}
						if (nodes[0].node == nodes2[1].node && nodes[1].node == nodes2[0].node)
						{
							Debug.LogError("Duplicate edge (site node differnt order)");
							return;
						}
					}
				}
			}
		}
	}
}
