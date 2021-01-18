using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeEditorFramework
{
	[NodeCanvasType("Default")]
	public class NodeCanvas : ScriptableObject
	{
		public delegate void CompleteLoadCallback(string fileName, NodeCanvas canvas);

		public List<Node> nodes = new List<Node>();

		public NodeEditorState[] editorStates = new NodeEditorState[0];

		public bool livesInScene;

		public void Validate()
		{
			if (nodes == null)
			{
				Debug.LogWarning("NodeCanvas '" + base.name + "' nodes were erased and set to null! Automatically fixed!");
				nodes = new List<Node>();
			}
			for (int i = 0; i < nodes.Count; i++)
			{
				Node node = nodes[i];
				if (node == null)
				{
					Debug.LogWarning("NodeCanvas '" + base.name + "' contained broken (null) nodes! Automatically fixed!");
					nodes.RemoveAt(i);
					i--;
					continue;
				}
				for (int j = 0; j < node.Inputs.Count; j++)
				{
					NodeInput nodeInput = node.Inputs[j];
					if (nodeInput == null)
					{
						Debug.LogWarning("NodeCanvas '" + base.name + "' Node '" + node.name + "' contained broken (null) NodeKnobs! Automatically fixed!");
						node.Inputs.RemoveAt(j);
						j--;
					}
					else if (nodeInput.connection != null && nodeInput.connection.body == null)
					{
						nodeInput.connection = null;
					}
				}
				for (int k = 0; k < node.Outputs.Count; k++)
				{
					NodeOutput nodeOutput = node.Outputs[k];
					if (nodeOutput == null)
					{
						Debug.LogWarning("NodeCanvas '" + base.name + "' Node '" + node.name + "' contained broken (null) NodeKnobs! Automatically fixed!");
						node.Outputs.RemoveAt(k);
						k--;
						continue;
					}
					for (int l = 0; l < nodeOutput.connections.Count; l++)
					{
						NodeInput nodeInput2 = nodeOutput.connections[l];
						if (nodeInput2 == null || nodeInput2.body == null)
						{
							nodeOutput.connections.RemoveAt(l);
							l--;
						}
					}
				}
				for (int m = 0; m < node.nodeKnobs.Count; m++)
				{
					NodeKnob nodeKnob = node.nodeKnobs[m];
					if (nodeKnob == null)
					{
						Debug.LogWarning("NodeCanvas '" + base.name + "' Node '" + node.name + "' contained broken (null) NodeKnobs! Automatically fixed!");
						node.nodeKnobs.RemoveAt(m);
						m--;
					}
					else if (nodeKnob is NodeInput)
					{
						NodeInput nodeInput3 = nodeKnob as NodeInput;
						if (nodeInput3.connection != null && nodeInput3.connection.body == null)
						{
							nodeInput3.connection = null;
						}
					}
					else
					{
						if (!(nodeKnob is NodeOutput))
						{
							continue;
						}
						NodeOutput nodeOutput2 = nodeKnob as NodeOutput;
						for (int n = 0; n < nodeOutput2.connections.Count; n++)
						{
							NodeInput nodeInput4 = nodeOutput2.connections[n];
							if (nodeInput4 == null || nodeInput4.body == null)
							{
								nodeOutput2.connections.RemoveAt(n);
								n--;
							}
						}
					}
				}
			}
			if (editorStates == null)
			{
				Debug.LogWarning("NodeCanvas '" + base.name + "' editorStates were erased! Automatically fixed!");
				editorStates = new NodeEditorState[0];
			}
			editorStates = editorStates.Where((NodeEditorState state) => state != null).ToArray();
			NodeEditorState[] array = editorStates;
			foreach (NodeEditorState nodeEditorState in array)
			{
				if (!nodes.Contains(nodeEditorState.selectedNode))
				{
					nodeEditorState.selectedNode = null;
				}
			}
		}

		public virtual void BeforeSavingCanvas()
		{
		}

		public virtual void AdditionalSaveMethods(string sceneCanvasName, CompleteLoadCallback onComplete)
		{
		}

		public virtual string DrawAdditionalSettings(string sceneCanvasName)
		{
			return sceneCanvasName;
		}

		public virtual void UpdateSettings(string sceneCanvasName)
		{
		}
	}
}
