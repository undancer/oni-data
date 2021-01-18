using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeEditorFramework.Standard
{
	public class RTCanvasCalculator : MonoBehaviour
	{
		public string canvasPath;

		public NodeCanvas canvas
		{
			get;
			private set;
		}

		private void Start()
		{
			LoadCanvas(canvasPath);
		}

		public void AssureCanvas()
		{
			if (canvas == null)
			{
				LoadCanvas(canvasPath);
				if (canvas == null)
				{
					throw new UnityException("No canvas specified to calculate on " + base.name + "!");
				}
			}
		}

		public void LoadCanvas(string path)
		{
			canvasPath = path;
			if (!string.IsNullOrEmpty(canvasPath))
			{
				canvas = NodeEditorSaveManager.LoadNodeCanvas(canvasPath, createWorkingCopy: true);
				CalculateCanvas();
			}
			else
			{
				canvas = null;
			}
		}

		public void CalculateCanvas()
		{
			AssureCanvas();
			NodeEditor.RecalculateAll(canvas);
			DebugOutputResults();
		}

		private void DebugOutputResults()
		{
			AssureCanvas();
			List<Node> outputNodes = getOutputNodes();
			foreach (Node item in outputNodes)
			{
				string text = "(OUT) " + item.name + ": ";
				if (item.Outputs.Count == 0)
				{
					foreach (NodeInput input in item.Inputs)
					{
						text = text + input.typeID + " " + (input.IsValueNull ? "NULL" : input.GetValue().ToString()) + "; ";
					}
				}
				else
				{
					foreach (NodeOutput output in item.Outputs)
					{
						text = text + output.typeID + " " + (output.IsValueNull ? "NULL" : output.GetValue().ToString()) + "; ";
					}
				}
				Debug.Log(text);
			}
		}

		public List<Node> getInputNodes()
		{
			AssureCanvas();
			return canvas.nodes.Where((Node node) => (node.Inputs.Count == 0 && node.Outputs.Count != 0) || node.Inputs.TrueForAll((NodeInput input) => input.connection == null)).ToList();
		}

		public List<Node> getOutputNodes()
		{
			AssureCanvas();
			return canvas.nodes.Where((Node node) => (node.Outputs.Count == 0 && node.Inputs.Count != 0) || node.Outputs.TrueForAll((NodeOutput output) => output.connections.Count == 0)).ToList();
		}
	}
}
