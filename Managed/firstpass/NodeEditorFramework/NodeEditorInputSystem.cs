using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeEditorFramework.Utilities;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class NodeEditorInputSystem
	{
		private static List<KeyValuePair<EventHandlerAttribute, Delegate>> eventHandlers;

		private static List<KeyValuePair<HotkeyAttribute, Delegate>> hotkeyHandlers;

		private static List<KeyValuePair<ContextEntryAttribute, PopupMenu.MenuFunctionData>> contextEntries;

		private static List<KeyValuePair<ContextFillerAttribute, Delegate>> contextFillers;

		private static NodeEditorState unfocusControlsForState;

		public static void SetupInput()
		{
			eventHandlers = new List<KeyValuePair<EventHandlerAttribute, Delegate>>();
			hotkeyHandlers = new List<KeyValuePair<HotkeyAttribute, Delegate>>();
			contextEntries = new List<KeyValuePair<ContextEntryAttribute, PopupMenu.MenuFunctionData>>();
			contextFillers = new List<KeyValuePair<ContextFillerAttribute, Delegate>>();
			foreach (Assembly item in from assembly in AppDomain.CurrentDomain.GetAssemblies()
				where assembly.FullName.Contains("Assembly")
				select assembly)
			{
				Type[] types = item.GetTypes();
				for (int i = 0; i < types.Length; i++)
				{
					MethodInfo[] methods = types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
					foreach (MethodInfo methodInfo in methods)
					{
						Delegate actionDelegate = null;
						object[] customAttributes = methodInfo.GetCustomAttributes(inherit: true);
						foreach (object obj in customAttributes)
						{
							Type type = obj.GetType();
							if (type == typeof(EventHandlerAttribute))
							{
								if (EventHandlerAttribute.AssureValidity(methodInfo, obj as EventHandlerAttribute))
								{
									if ((object)actionDelegate == null)
									{
										actionDelegate = Delegate.CreateDelegate(typeof(Action<NodeEditorInputInfo>), methodInfo);
									}
									eventHandlers.Add(new KeyValuePair<EventHandlerAttribute, Delegate>(obj as EventHandlerAttribute, actionDelegate));
								}
							}
							else if (type == typeof(HotkeyAttribute))
							{
								if (HotkeyAttribute.AssureValidity(methodInfo, obj as HotkeyAttribute))
								{
									if ((object)actionDelegate == null)
									{
										actionDelegate = Delegate.CreateDelegate(typeof(Action<NodeEditorInputInfo>), methodInfo);
									}
									hotkeyHandlers.Add(new KeyValuePair<HotkeyAttribute, Delegate>(obj as HotkeyAttribute, actionDelegate));
								}
							}
							else if (type == typeof(ContextEntryAttribute))
							{
								if (!ContextEntryAttribute.AssureValidity(methodInfo, obj as ContextEntryAttribute))
								{
									continue;
								}
								if ((object)actionDelegate == null)
								{
									actionDelegate = Delegate.CreateDelegate(typeof(Action<NodeEditorInputInfo>), methodInfo);
								}
								PopupMenu.MenuFunctionData value = delegate(object callbackObj)
								{
									if (!(callbackObj is NodeEditorInputInfo))
									{
										throw new UnityException("Callback Object passed by context is not of type NodeEditorMenuCallback!");
									}
									actionDelegate.DynamicInvoke(callbackObj as NodeEditorInputInfo);
								};
								contextEntries.Add(new KeyValuePair<ContextEntryAttribute, PopupMenu.MenuFunctionData>(obj as ContextEntryAttribute, value));
							}
							else if (type == typeof(ContextFillerAttribute) && ContextFillerAttribute.AssureValidity(methodInfo, obj as ContextFillerAttribute))
							{
								Delegate value2 = Delegate.CreateDelegate(typeof(Action<NodeEditorInputInfo, GenericMenu>), methodInfo);
								contextFillers.Add(new KeyValuePair<ContextFillerAttribute, Delegate>(obj as ContextFillerAttribute, value2));
							}
						}
					}
				}
			}
			eventHandlers.Sort((KeyValuePair<EventHandlerAttribute, Delegate> handlerA, KeyValuePair<EventHandlerAttribute, Delegate> handlerB) => handlerA.Key.priority.CompareTo(handlerB.Key.priority));
			hotkeyHandlers.Sort((KeyValuePair<HotkeyAttribute, Delegate> handlerA, KeyValuePair<HotkeyAttribute, Delegate> handlerB) => handlerA.Key.priority.CompareTo(handlerB.Key.priority));
		}

		private static void CallEventHandlers(NodeEditorInputInfo inputInfo, bool late)
		{
			object[] args = new object[1]
			{
				inputInfo
			};
			foreach (KeyValuePair<EventHandlerAttribute, Delegate> eventHandler in eventHandlers)
			{
				if ((!eventHandler.Key.handledEvent.HasValue || eventHandler.Key.handledEvent == inputInfo.inputEvent.type) && (late ? (eventHandler.Key.priority >= 100) : (eventHandler.Key.priority < 100)))
				{
					eventHandler.Value.DynamicInvoke(args);
					if (inputInfo.inputEvent.type == EventType.Used)
					{
						break;
					}
				}
			}
		}

		private static void CallHotkeys(NodeEditorInputInfo inputInfo, KeyCode keyCode, EventModifiers mods)
		{
			object[] args = new object[1]
			{
				inputInfo
			};
			foreach (KeyValuePair<HotkeyAttribute, Delegate> hotkeyHandler in hotkeyHandlers)
			{
				if (hotkeyHandler.Key.handledHotKey == keyCode && (!hotkeyHandler.Key.modifiers.HasValue || hotkeyHandler.Key.modifiers == mods) && (!hotkeyHandler.Key.limitingEventType.HasValue || hotkeyHandler.Key.limitingEventType == inputInfo.inputEvent.type))
				{
					hotkeyHandler.Value.DynamicInvoke(args);
					if (inputInfo.inputEvent.type == EventType.Used)
					{
						break;
					}
				}
			}
		}

		private static void FillContextMenu(NodeEditorInputInfo inputInfo, GenericMenu contextMenu, ContextType contextType)
		{
			foreach (KeyValuePair<ContextEntryAttribute, PopupMenu.MenuFunctionData> contextEntry in contextEntries)
			{
				if (contextEntry.Key.contextType == contextType)
				{
					contextMenu.AddItem(new GUIContent(contextEntry.Key.contextPath), on: false, contextEntry.Value, inputInfo);
				}
			}
			object[] args = new object[2]
			{
				inputInfo,
				contextMenu
			};
			foreach (KeyValuePair<ContextFillerAttribute, Delegate> contextFiller in contextFillers)
			{
				if (contextFiller.Key.contextType == contextType)
				{
					contextFiller.Value.DynamicInvoke(args);
				}
			}
		}

		public static void HandleInputEvents(NodeEditorState state)
		{
			if (!shouldIgnoreInput(state))
			{
				NodeEditorInputInfo inputInfo = new NodeEditorInputInfo(state);
				CallEventHandlers(inputInfo, late: false);
				CallHotkeys(inputInfo, Event.current.keyCode, Event.current.modifiers);
			}
		}

		public static void HandleLateInputEvents(NodeEditorState state)
		{
			if (!shouldIgnoreInput(state))
			{
				CallEventHandlers(new NodeEditorInputInfo(state), late: true);
			}
		}

		internal static bool shouldIgnoreInput(NodeEditorState state)
		{
			if (OverlayGUI.HasPopupControl())
			{
				return true;
			}
			if (!state.canvasRect.Contains(Event.current.mousePosition))
			{
				return true;
			}
			for (int i = 0; i < state.ignoreInput.Count; i++)
			{
				if (state.ignoreInput[i].Contains(Event.current.mousePosition))
				{
					return true;
				}
			}
			return false;
		}

		[EventHandler(-4)]
		private static void HandleFocussing(NodeEditorInputInfo inputInfo)
		{
			NodeEditorState editorState = inputInfo.editorState;
			editorState.focusedNode = NodeEditor.NodeAtPosition(NodeEditor.ScreenToCanvasSpace(inputInfo.inputPos), out editorState.focusedNodeKnob);
			if (unfocusControlsForState == editorState && Event.current.type == EventType.Repaint)
			{
				GUIUtility.hotControl = 0;
				GUIUtility.keyboardControl = 0;
				unfocusControlsForState = null;
			}
		}

		[EventHandler(EventType.MouseDown, -2)]
		private static void HandleSelecting(NodeEditorInputInfo inputInfo)
		{
			NodeEditorState editorState = inputInfo.editorState;
			if (inputInfo.inputEvent.button == 0 && editorState.focusedNode != editorState.selectedNode)
			{
				unfocusControlsForState = editorState;
				editorState.selectedNode = editorState.focusedNode;
				NodeEditor.RepaintClients();
			}
		}

		[EventHandler(EventType.MouseDown, 0)]
		private static void HandleContextClicks(NodeEditorInputInfo inputInfo)
		{
			if (Event.current.button == 1)
			{
				GenericMenu genericMenu = new GenericMenu();
				if (inputInfo.editorState.focusedNode != null)
				{
					FillContextMenu(inputInfo, genericMenu, ContextType.Node);
				}
				else
				{
					FillContextMenu(inputInfo, genericMenu, ContextType.Canvas);
				}
				genericMenu.Show(inputInfo.inputPos);
				Event.current.Use();
			}
		}
	}
}
