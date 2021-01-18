using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NodeEditorFramework.Utilities;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class NodeEditorSaveManager
	{
		private static GameObject sceneSaveHolder;

		private static void FetchSceneSaveHolder()
		{
			if (sceneSaveHolder == null)
			{
				sceneSaveHolder = GameObject.Find("NodeEditor_SceneSaveHolder");
				if (sceneSaveHolder == null)
				{
					sceneSaveHolder = new GameObject("NodeEditor_SceneSaveHolder");
				}
				sceneSaveHolder.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
			}
		}

		public static string[] GetSceneSaves()
		{
			FetchSceneSaveHolder();
			return (from save in sceneSaveHolder.GetComponents<NodeCanvasSceneSave>()
				select save.savedNodeCanvas.name).ToArray();
		}

		private static NodeCanvasSceneSave FindSceneSave(string saveName)
		{
			FetchSceneSaveHolder();
			return sceneSaveHolder.GetComponents<NodeCanvasSceneSave>().ToList().Find((NodeCanvasSceneSave save) => save.savedNodeCanvas.name == saveName);
		}

		public static void SaveSceneNodeCanvas(string saveName, ref NodeCanvas nodeCanvas, bool createWorkingCopy)
		{
			if (string.IsNullOrEmpty(saveName))
			{
				Debug.LogError("Cannot save Canvas to scene: No save name specified!");
				return;
			}
			nodeCanvas.livesInScene = true;
			nodeCanvas.name = saveName;
			NodeCanvasSceneSave nodeCanvasSceneSave = FindSceneSave(saveName);
			if (nodeCanvasSceneSave == null)
			{
				nodeCanvasSceneSave = sceneSaveHolder.AddComponent<NodeCanvasSceneSave>();
			}
			nodeCanvasSceneSave.savedNodeCanvas = nodeCanvas;
			if (createWorkingCopy)
			{
				nodeCanvasSceneSave.savedNodeCanvas = CreateWorkingCopy(nodeCanvasSceneSave.savedNodeCanvas, editorStates: true);
				Compress(ref nodeCanvasSceneSave.savedNodeCanvas);
			}
		}

		public static NodeCanvas LoadSceneNodeCanvas(string saveName, bool createWorkingCopy)
		{
			if (string.IsNullOrEmpty(saveName))
			{
				Debug.LogError("Cannot load Canvas from scene: No save name specified!");
				return null;
			}
			NodeCanvasSceneSave nodeCanvasSceneSave = FindSceneSave(saveName);
			if (nodeCanvasSceneSave == null)
			{
				return null;
			}
			NodeCanvas nodeCanvas = nodeCanvasSceneSave.savedNodeCanvas;
			nodeCanvas.livesInScene = true;
			if (createWorkingCopy)
			{
				nodeCanvas = CreateWorkingCopy(nodeCanvas, editorStates: true);
			}
			Uncompress(ref nodeCanvas);
			return nodeCanvas;
		}

		public static void SaveNodeCanvas(string path, NodeCanvas nodeCanvas, bool createWorkingCopy)
		{
			throw new NotImplementedException();
		}

		public static NodeCanvas LoadNodeCanvas(string path, bool createWorkingCopy)
		{
			if (!File.Exists(path))
			{
				throw new UnityException("Cannot Load NodeCanvas: File '" + path + "' deos not exist!");
			}
			NodeCanvas nodeCanvas = ResourceManager.LoadResource<NodeCanvas>(path);
			if (nodeCanvas == null)
			{
				throw new UnityException("Cannot Load NodeCanvas: The file at the specified path '" + path + "' is no valid save file as it does not contain a NodeCanvas!");
			}
			if (createWorkingCopy)
			{
				nodeCanvas = CreateWorkingCopy(nodeCanvas, editorStates: true);
			}
			else
			{
				nodeCanvas.Validate();
			}
			Uncompress(ref nodeCanvas);
			NodeEditorCallbacks.IssueOnLoadCanvas(nodeCanvas);
			return nodeCanvas;
		}

		public static void Compress(ref NodeCanvas nodeCanvas)
		{
		}

		public static void Uncompress(ref NodeCanvas nodeCanvas)
		{
			for (int i = 0; i < nodeCanvas.nodes.Count; i++)
			{
				Node node = nodeCanvas.nodes[i];
				if (node.Inputs != null && node.Inputs.Count != 0 && node.Outputs != null && node.Outputs.Count != 0)
				{
					continue;
				}
				node.Inputs = new List<NodeInput>();
				node.Outputs = new List<NodeOutput>();
				for (int j = 0; j < node.nodeKnobs.Count; j++)
				{
					NodeKnob nodeKnob = node.nodeKnobs[j];
					if (nodeKnob is NodeInput)
					{
						node.Inputs.Add(nodeKnob as NodeInput);
					}
					else if (nodeKnob is NodeOutput)
					{
						node.Outputs.Add(nodeKnob as NodeOutput);
					}
				}
			}
		}

		public static NodeCanvas CreateWorkingCopy(NodeCanvas nodeCanvas, bool editorStates)
		{
			nodeCanvas.Validate();
			nodeCanvas = Clone(nodeCanvas);
			List<ScriptableObject> allSOs = new List<ScriptableObject>();
			List<ScriptableObject> clonedSOs = new List<ScriptableObject>();
			for (int i = 0; i < nodeCanvas.nodes.Count; i++)
			{
				Node node = nodeCanvas.nodes[i];
				node.CheckNodeKnobMigration();
				Node node2 = AddClonedSO(allSOs, clonedSOs, node);
				AddClonedSOs(allSOs, clonedSOs, node2.GetScriptableObjects());
				foreach (NodeKnob nodeKnob3 in node2.nodeKnobs)
				{
					AddClonedSO(allSOs, clonedSOs, nodeKnob3);
					AddClonedSOs(allSOs, clonedSOs, nodeKnob3.GetScriptableObjects());
				}
			}
			for (int j = 0; j < nodeCanvas.nodes.Count; j++)
			{
				Node initialSO = nodeCanvas.nodes[j];
				Node node4 = (nodeCanvas.nodes[j] = ReplaceSO(allSOs, clonedSOs, initialSO));
				Node node5 = node4;
				node5.CopyScriptableObjects((ScriptableObject so) => ReplaceSO(allSOs, clonedSOs, so));
				for (int k = 0; k < node5.nodeKnobs.Count; k++)
				{
					NodeKnob nodeKnob2 = (node5.nodeKnobs[k] = ReplaceSO(allSOs, clonedSOs, node5.nodeKnobs[k]));
					nodeKnob2.body = node5;
					nodeKnob2.CopyScriptableObjects((ScriptableObject so) => ReplaceSO(allSOs, clonedSOs, so));
				}
				for (int l = 0; l < node5.Inputs.Count; l++)
				{
					NodeInput nodeInput2 = (node5.Inputs[l] = ReplaceSO(allSOs, clonedSOs, node5.Inputs[l]));
					nodeInput2.body = node5;
				}
				for (int m = 0; m < node5.Outputs.Count; m++)
				{
					NodeOutput nodeOutput2 = (node5.Outputs[m] = ReplaceSO(allSOs, clonedSOs, node5.Outputs[m]));
					nodeOutput2.body = node5;
				}
			}
			if (editorStates)
			{
				nodeCanvas.editorStates = CreateWorkingCopy(nodeCanvas.editorStates, nodeCanvas);
				NodeEditorState[] editorStates2 = nodeCanvas.editorStates;
				foreach (NodeEditorState nodeEditorState in editorStates2)
				{
					nodeEditorState.selectedNode = ReplaceSO(allSOs, clonedSOs, nodeEditorState.selectedNode);
				}
			}
			else
			{
				NodeEditorState[] editorStates2 = nodeCanvas.editorStates;
				for (int n = 0; n < editorStates2.Length; n++)
				{
					editorStates2[n].selectedNode = null;
				}
			}
			return nodeCanvas;
		}

		private static NodeEditorState[] CreateWorkingCopy(NodeEditorState[] editorStates, NodeCanvas associatedNodeCanvas)
		{
			if (editorStates == null)
			{
				return new NodeEditorState[0];
			}
			editorStates = (NodeEditorState[])editorStates.Clone();
			for (int i = 0; i < editorStates.Length; i++)
			{
				if (!(editorStates[i] == null))
				{
					NodeEditorState nodeEditorState = (editorStates[i] = Clone(editorStates[i]));
					if (nodeEditorState == null)
					{
						Debug.LogError("Failed to create a working copy for an NodeEditorState during the loading process of " + associatedNodeCanvas.name + "!");
					}
					else
					{
						nodeEditorState.canvas = associatedNodeCanvas;
					}
				}
			}
			associatedNodeCanvas.editorStates = editorStates;
			return editorStates;
		}

		private static T Clone<T>(T SO) where T : ScriptableObject
		{
			string name = SO.name;
			SO = UnityEngine.Object.Instantiate(SO);
			SO.name = name;
			return SO;
		}

		private static void AddClonedSOs(List<ScriptableObject> scriptableObjects, List<ScriptableObject> clonedScriptableObjects, ScriptableObject[] initialSOs)
		{
			scriptableObjects.AddRange(initialSOs);
			clonedScriptableObjects.AddRange(initialSOs.Select((ScriptableObject so) => Clone(so)));
		}

		private static T AddClonedSO<T>(List<ScriptableObject> scriptableObjects, List<ScriptableObject> clonedScriptableObjects, T initialSO) where T : ScriptableObject
		{
			if ((UnityEngine.Object)initialSO == (UnityEngine.Object)null)
			{
				return null;
			}
			scriptableObjects.Add(initialSO);
			T val = Clone(initialSO);
			clonedScriptableObjects.Add(val);
			return val;
		}

		private static T ReplaceSO<T>(List<ScriptableObject> scriptableObjects, List<ScriptableObject> clonedScriptableObjects, T initialSO) where T : ScriptableObject
		{
			if ((UnityEngine.Object)initialSO == (UnityEngine.Object)null)
			{
				return null;
			}
			int num = scriptableObjects.IndexOf(initialSO);
			if (num == -1)
			{
				Debug.LogError("GetWorkingCopy: ScriptableObject " + initialSO.name + " was not copied before! It will be null!");
			}
			if (num != -1)
			{
				return (T)clonedScriptableObjects[num];
			}
			return null;
		}

		public static NodeEditorState ExtractEditorState(NodeCanvas canvas, string stateName)
		{
			NodeEditorState nodeEditorState = null;
			if (canvas.editorStates.Length != 0)
			{
				nodeEditorState = canvas.editorStates.First((NodeEditorState s) => s.name == stateName);
				if (nodeEditorState == null)
				{
					nodeEditorState = canvas.editorStates[0];
				}
			}
			if (nodeEditorState == null)
			{
				nodeEditorState = ScriptableObject.CreateInstance<NodeEditorState>();
				nodeEditorState.canvas = canvas;
				canvas.editorStates = new NodeEditorState[1]
				{
					nodeEditorState
				};
			}
			nodeEditorState.name = stateName;
			return nodeEditorState;
		}
	}
}
