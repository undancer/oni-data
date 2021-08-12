using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeEditorFramework.Utilities;
using UnityEngine;

namespace NodeEditorFramework
{
	public class NodeCanvasManager
	{
		public static Dictionary<Type, NodeCanvasTypeData> TypeOfCanvases;

		private static Action<Type> _callBack;

		public static void GetAllCanvasTypes()
		{
			TypeOfCanvases = new Dictionary<Type, NodeCanvasTypeData>();
			foreach (Assembly item in from assembly in AppDomain.CurrentDomain.GetAssemblies()
				where assembly.FullName.Contains("Assembly")
				select assembly)
			{
				foreach (Type item2 in from T in item.GetTypes()
					where T.IsClass && !T.IsAbstract && T.GetCustomAttributes(typeof(NodeCanvasTypeAttribute), inherit: false).Length != 0
					select T)
				{
					NodeCanvasTypeAttribute nodeCanvasTypeAttribute = item2.GetCustomAttributes(typeof(NodeCanvasTypeAttribute), inherit: false)[0] as NodeCanvasTypeAttribute;
					TypeOfCanvases.Add(item2, new NodeCanvasTypeData
					{
						CanvasType = item2,
						DisplayString = nodeCanvasTypeAttribute.Name
					});
				}
			}
		}

		private static void CreateNewCanvas(object userdata)
		{
			NodeCanvasTypeData nodeCanvasTypeData = (NodeCanvasTypeData)userdata;
			_callBack(nodeCanvasTypeData.CanvasType);
		}

		public static void PopulateMenu(ref GenericMenu menu, Action<Type> newNodeCanvas)
		{
			_callBack = newNodeCanvas;
			foreach (KeyValuePair<Type, NodeCanvasTypeData> typeOfCanvase in TypeOfCanvases)
			{
				menu.AddItem(new GUIContent(typeOfCanvase.Value.DisplayString), on: false, CreateNewCanvas, typeOfCanvase.Value);
			}
		}
	}
}
