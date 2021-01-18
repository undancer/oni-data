using System.IO;
using NodeEditorFramework.Utilities;
using UnityEngine;

namespace NodeEditorFramework.Standard
{
	public class RuntimeNodeEditor : MonoBehaviour
	{
		public string canvasPath;

		public NodeCanvas canvas;

		private NodeEditorState state;

		public bool screenSize = false;

		private Rect canvasRect;

		public Rect specifiedRootRect;

		public Rect specifiedCanvasRect;

		private string sceneCanvasName = "";

		private Vector2 loadScenePos;

		public void Start()
		{
			NodeEditor.checkInit(GUIFunction: false);
			NodeEditor.initiated = false;
			LoadNodeCanvas(canvasPath);
			FPSCounter.Create();
		}

		public void Update()
		{
			NodeEditor.Update();
			FPSCounter.Update();
		}

		public void OnGUI()
		{
			if (!(canvas != null))
			{
				return;
			}
			if (state == null)
			{
				NewEditorState();
			}
			NodeEditor.checkInit(GUIFunction: true);
			if (NodeEditor.InitiationError)
			{
				GUILayout.Label("Initiation failed! Check console for more information!");
				return;
			}
			try
			{
				if (!screenSize && specifiedRootRect.max != specifiedRootRect.min)
				{
					GUI.BeginGroup(specifiedRootRect, NodeEditorGUI.nodeSkin.box);
				}
				NodeEditorGUI.StartNodeGUI();
				canvasRect = (screenSize ? new Rect(0f, 0f, Screen.width, Screen.height) : specifiedCanvasRect);
				canvasRect.width -= 200f;
				state.canvasRect = canvasRect;
				NodeEditor.DrawCanvas(canvas, state);
				GUILayout.BeginArea(new Rect(canvasRect.x + state.canvasRect.width, state.canvasRect.y, 200f, state.canvasRect.height), NodeEditorGUI.nodeSkin.box);
				SideGUI();
				GUILayout.EndArea();
				NodeEditorGUI.EndNodeGUI();
				if (!screenSize && specifiedRootRect.max != specifiedRootRect.min)
				{
					GUI.EndGroup();
				}
			}
			catch (UnityException exception)
			{
				NewNodeCanvas();
				NodeEditor.ReInit(GUIFunction: true);
				Debug.LogError("Unloaded Canvas due to exception in Draw!");
				Debug.LogException(exception);
			}
		}

		public void SideGUI()
		{
			GUILayout.Label(new GUIContent("Node Editor (" + canvas.name + ")", "The currently opened canvas in the Node Editor"));
			screenSize = GUILayout.Toggle(screenSize, "Adapt to Screen");
			GUILayout.Label("FPS: " + FPSCounter.currentFPS);
			GUILayout.Label(new GUIContent("Node Editor (" + canvas.name + ")"), NodeEditorGUI.nodeLabelBold);
			if (GUILayout.Button(new GUIContent("New Canvas", "Loads an empty Canvas")))
			{
				NewNodeCanvas();
			}
			GUILayout.Space(6f);
			GUILayout.BeginHorizontal();
			sceneCanvasName = GUILayout.TextField(sceneCanvasName, GUILayout.ExpandWidth(expand: true));
			if (GUILayout.Button(new GUIContent("Save to Scene", "Saves the Canvas to the Scene"), GUILayout.ExpandWidth(expand: false)))
			{
				SaveSceneNodeCanvas(sceneCanvasName);
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button(new GUIContent("Load from Scene", "Loads the Canvas from the Scene")))
			{
				GenericMenu genericMenu = new GenericMenu();
				string[] sceneSaves = NodeEditorSaveManager.GetSceneSaves();
				foreach (string text in sceneSaves)
				{
					genericMenu.AddItem(new GUIContent(text), on: false, LoadSceneCanvasCallback, text);
				}
				genericMenu.Show(loadScenePos);
			}
			if (Event.current.type == EventType.Repaint)
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();
				loadScenePos = new Vector2(lastRect.x + 2f, lastRect.yMax + 2f);
			}
			GUILayout.Space(6f);
			if (GUILayout.Button(new GUIContent("Recalculate All", "Initiates complete recalculate. Usually does not need to be triggered manually.")))
			{
				NodeEditor.RecalculateAll(canvas);
			}
			if (GUILayout.Button("Force Re-Init"))
			{
				NodeEditor.ReInit(GUIFunction: true);
			}
			NodeEditorGUI.knobSize = RTEditorGUI.IntSlider(new GUIContent("Handle Size", "The size of the Node Input/Output handles"), NodeEditorGUI.knobSize, 12, 20);
			state.zoom = RTEditorGUI.Slider(new GUIContent("Zoom", "Use the Mousewheel. Seriously."), state.zoom, 0.6f, 2f);
		}

		private void LoadSceneCanvasCallback(object save)
		{
			LoadSceneNodeCanvas((string)save);
		}

		public void SaveSceneNodeCanvas(string path)
		{
			canvas.editorStates = new NodeEditorState[1]
			{
				state
			};
			NodeEditorSaveManager.SaveSceneNodeCanvas(path, ref canvas, createWorkingCopy: true);
		}

		public void LoadSceneNodeCanvas(string path)
		{
			if ((canvas = NodeEditorSaveManager.LoadSceneNodeCanvas(path, createWorkingCopy: true)) == null)
			{
				NewNodeCanvas();
				return;
			}
			state = NodeEditorSaveManager.ExtractEditorState(canvas, "MainEditorState");
			NodeEditor.RecalculateAll(canvas);
		}

		public void LoadNodeCanvas(string path)
		{
			if (!File.Exists(path) || (canvas = NodeEditorSaveManager.LoadNodeCanvas(path, createWorkingCopy: true)) == null)
			{
				NewNodeCanvas();
				return;
			}
			state = NodeEditorSaveManager.ExtractEditorState(canvas, "MainEditorState");
			NodeEditor.RecalculateAll(canvas);
		}

		public void NewNodeCanvas()
		{
			canvas = ScriptableObject.CreateInstance<NodeCanvas>();
			canvas.name = "New Canvas";
			NewEditorState();
		}

		private void NewEditorState()
		{
			state = ScriptableObject.CreateInstance<NodeEditorState>();
			state.canvas = canvas;
			state.name = "MainEditorState";
			canvas.editorStates = new NodeEditorState[1]
			{
				state
			};
		}
	}
}
