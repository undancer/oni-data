using System;
using System.Collections.Generic;
using System.Linq;
using NodeEditorFramework.Utilities;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class NodeEditorInputControls
	{
		[ContextFiller(ContextType.Canvas)]
		private static void FillAddNodes(NodeEditorInputInfo inputInfo, GenericMenu canvasContextMenu)
		{
			NodeEditorState editorState = inputInfo.editorState;
			List<Node> displayedNodes = ((editorState.connectOutput != null) ? NodeTypes.getCompatibleNodes(editorState.connectOutput) : NodeTypes.nodes.Keys.ToList());
			DeCafList(ref displayedNodes, editorState.canvas);
			foreach (Node item in displayedNodes)
			{
				canvasContextMenu.AddItem(new GUIContent("Add " + NodeTypes.nodes[item].adress), on: false, CreateNodeCallback, new NodeEditorInputInfo(item.GetID, editorState));
			}
		}

		private static void DeCafList(ref List<Node> displayedNodes, NodeCanvas canvas)
		{
			for (int i = 0; i < displayedNodes.Count; i++)
			{
				if (!NodeTypes.nodes[displayedNodes[i]].typeOfNodeCanvas.Contains(canvas.GetType()))
				{
					displayedNodes.RemoveAt(i);
					i--;
				}
			}
		}

		private static void CreateNodeCallback(object infoObj)
		{
			if (!(infoObj is NodeEditorInputInfo nodeEditorInputInfo))
			{
				throw new UnityException("Callback Object passed by context is not of type NodeEditorInputInfo!");
			}
			nodeEditorInputInfo.SetAsCurrentEnvironment();
			Node.Create(nodeEditorInputInfo.message, NodeEditor.ScreenToCanvasSpace(nodeEditorInputInfo.inputPos), nodeEditorInputInfo.editorState.connectOutput);
			nodeEditorInputInfo.editorState.connectOutput = null;
			NodeEditor.RepaintClients();
		}

		[ContextEntry(ContextType.Node, "Delete Node")]
		private static void DeleteNode(NodeEditorInputInfo inputInfo)
		{
			inputInfo.SetAsCurrentEnvironment();
			if (inputInfo.editorState.focusedNode != null)
			{
				inputInfo.editorState.focusedNode.Delete();
				inputInfo.inputEvent.Use();
			}
		}

		[ContextEntry(ContextType.Node, "Duplicate Node")]
		private static void DuplicateNode(NodeEditorInputInfo inputInfo)
		{
			inputInfo.SetAsCurrentEnvironment();
			NodeEditorState editorState = inputInfo.editorState;
			if (editorState.focusedNode != null)
			{
				Node focusedNode = Node.Create(editorState.focusedNode.GetID, NodeEditor.ScreenToCanvasSpace(inputInfo.inputPos), editorState.connectOutput);
				editorState.selectedNode = (editorState.focusedNode = focusedNode);
				editorState.connectOutput = null;
				inputInfo.inputEvent.Use();
			}
		}

		[Hotkey(KeyCode.UpArrow, EventType.KeyDown)]
		[Hotkey(KeyCode.LeftArrow, EventType.KeyDown)]
		[Hotkey(KeyCode.RightArrow, EventType.KeyDown)]
		[Hotkey(KeyCode.DownArrow, EventType.KeyDown)]
		private static void KB_MoveNode(NodeEditorInputInfo inputInfo)
		{
			NodeEditorState editorState = inputInfo.editorState;
			if (editorState.selectedNode != null)
			{
				Vector2 position = editorState.selectedNode.rect.position;
				int num = 0;
				num = ((!inputInfo.inputEvent.shift) ? 5 : 10);
				if (inputInfo.inputEvent.keyCode == KeyCode.RightArrow)
				{
					position = new Vector2(position.x + (float)num, position.y);
				}
				else if (inputInfo.inputEvent.keyCode == KeyCode.LeftArrow)
				{
					position = new Vector2(position.x - (float)num, position.y);
				}
				else if (inputInfo.inputEvent.keyCode == KeyCode.DownArrow)
				{
					position = new Vector2(position.x, position.y + (float)num);
				}
				else if (inputInfo.inputEvent.keyCode == KeyCode.UpArrow)
				{
					position = new Vector2(position.x, position.y - (float)num);
				}
				editorState.selectedNode.rect.position = position;
				inputInfo.inputEvent.Use();
			}
			NodeEditor.RepaintClients();
		}

		[EventHandler(EventType.MouseDown, 110)]
		private static void HandleNodeDraggingStart(NodeEditorInputInfo inputInfo)
		{
			if (GUIUtility.hotControl <= 0)
			{
				NodeEditorState editorState = inputInfo.editorState;
				if (inputInfo.inputEvent.button == 0 && editorState.focusedNode != null && editorState.focusedNode == editorState.selectedNode && editorState.focusedNodeKnob == null)
				{
					editorState.dragNode = true;
					editorState.dragStart = inputInfo.inputPos;
					editorState.dragPos = editorState.focusedNode.rect.position;
					editorState.dragOffset = Vector2.zero;
					inputInfo.inputEvent.delta = Vector2.zero;
				}
			}
		}

		[EventHandler(EventType.MouseDrag)]
		private static void HandleNodeDragging(NodeEditorInputInfo inputInfo)
		{
			NodeEditorState editorState = inputInfo.editorState;
			if (editorState.dragNode)
			{
				if (editorState.selectedNode != null && GUIUtility.hotControl == 0)
				{
					editorState.dragOffset = inputInfo.inputPos - editorState.dragStart;
					editorState.selectedNode.rect.position = editorState.dragPos + editorState.dragOffset * editorState.zoom;
					NodeEditorCallbacks.IssueOnMoveNode(editorState.selectedNode);
					NodeEditor.RepaintClients();
				}
				else
				{
					editorState.dragNode = false;
				}
			}
		}

		[EventHandler(EventType.MouseDown)]
		[EventHandler(EventType.MouseUp)]
		private static void HandleNodeDraggingEnd(NodeEditorInputInfo inputInfo)
		{
			inputInfo.editorState.dragNode = false;
		}

		[EventHandler(EventType.MouseDown, 100)]
		private static void HandleWindowPanningStart(NodeEditorInputInfo inputInfo)
		{
			if (GUIUtility.hotControl <= 0)
			{
				NodeEditorState editorState = inputInfo.editorState;
				if ((inputInfo.inputEvent.button == 0 || inputInfo.inputEvent.button == 2) && editorState.focusedNode == null)
				{
					editorState.panWindow = true;
					editorState.dragStart = inputInfo.inputPos;
					editorState.dragOffset = Vector2.zero;
				}
			}
		}

		[EventHandler(EventType.MouseDrag)]
		private static void HandleWindowPanning(NodeEditorInputInfo inputInfo)
		{
			NodeEditorState editorState = inputInfo.editorState;
			if (editorState.panWindow)
			{
				Vector2 dragOffset = editorState.dragOffset;
				editorState.dragOffset = inputInfo.inputPos - editorState.dragStart;
				dragOffset = (editorState.dragOffset - dragOffset) * editorState.zoom;
				editorState.panOffset += dragOffset;
				NodeEditor.RepaintClients();
			}
		}

		[EventHandler(EventType.MouseDown)]
		[EventHandler(EventType.MouseUp)]
		private static void HandleWindowPanningEnd(NodeEditorInputInfo inputInfo)
		{
			inputInfo.editorState.panWindow = false;
		}

		[EventHandler(EventType.MouseDown)]
		private static void HandleConnectionDrawing(NodeEditorInputInfo inputInfo)
		{
			NodeEditorState editorState = inputInfo.editorState;
			if (inputInfo.inputEvent.button != 0 || !(editorState.focusedNodeKnob != null))
			{
				return;
			}
			if (editorState.focusedNodeKnob is NodeOutput)
			{
				editorState.connectOutput = (NodeOutput)editorState.focusedNodeKnob;
				inputInfo.inputEvent.Use();
			}
			else if (editorState.focusedNodeKnob is NodeInput)
			{
				NodeInput nodeInput = (NodeInput)editorState.focusedNodeKnob;
				if (nodeInput.connection != null)
				{
					editorState.connectOutput = nodeInput.connection;
					nodeInput.RemoveConnection();
					inputInfo.inputEvent.Use();
				}
			}
		}

		[EventHandler(EventType.MouseUp)]
		private static void HandleApplyConnection(NodeEditorInputInfo inputInfo)
		{
			NodeEditorState editorState = inputInfo.editorState;
			if (inputInfo.inputEvent.button == 0 && editorState.connectOutput != null && editorState.focusedNode != null && editorState.focusedNodeKnob != null && editorState.focusedNodeKnob is NodeInput)
			{
				(editorState.focusedNodeKnob as NodeInput).TryApplyConnection(editorState.connectOutput);
				inputInfo.inputEvent.Use();
			}
			editorState.connectOutput = null;
		}

		[EventHandler(EventType.ScrollWheel)]
		private static void HandleZooming(NodeEditorInputInfo inputInfo)
		{
			inputInfo.editorState.zoom = (float)Math.Round(Math.Min(2.0, Math.Max(0.6, inputInfo.editorState.zoom + inputInfo.inputEvent.delta.y / 15f)), 2);
			NodeEditor.RepaintClients();
		}

		[Hotkey(KeyCode.N, EventType.KeyDown)]
		private static void HandleStartNavigating(NodeEditorInputInfo inputInfo)
		{
			inputInfo.editorState.navigate = true;
		}

		[Hotkey(KeyCode.N, EventType.KeyUp)]
		private static void HandleEndNavigating(NodeEditorInputInfo inputInfo)
		{
			inputInfo.editorState.navigate = false;
		}

		[Hotkey(KeyCode.LeftControl, EventType.KeyDown, 60)]
		[Hotkey(KeyCode.LeftControl, EventType.KeyUp, 60)]
		private static void HandleNodeSnap(NodeEditorInputInfo inputInfo)
		{
			NodeEditorState editorState = inputInfo.editorState;
			if (editorState.selectedNode != null)
			{
				Vector2 position = editorState.selectedNode.rect.position;
				position = new Vector2(Mathf.RoundToInt(position.x / 10f) * 10, Mathf.RoundToInt(position.y / 10f) * 10);
				editorState.selectedNode.rect.position = position;
				inputInfo.inputEvent.Use();
			}
			NodeEditor.RepaintClients();
		}
	}
}
