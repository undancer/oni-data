using System;
using System.Collections.Generic;
using Klei;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using ProcGen.Noise;
using UnityEngine;

[NodeCanvasType("Noise Canvas")]
public class NoiseNodeCanvas : NodeCanvas
{
	private NoiseTreeFiles ntf;

	private TerminalNodeEditor terminator;

	private Rect lastRectPos;

	private Dictionary<string, PrimitiveNodeEditor> primitiveLookup = new Dictionary<string, PrimitiveNodeEditor>();

	private Dictionary<string, FilterNodeEditor> filterLookup = new Dictionary<string, FilterNodeEditor>();

	private Dictionary<string, ModifierModuleNodeEditor> modifierLookup = new Dictionary<string, ModifierModuleNodeEditor>();

	private Dictionary<string, SelectorModuleNodeEditor> selectorLookup = new Dictionary<string, SelectorModuleNodeEditor>();

	private Dictionary<string, TransformerNodeEditor> transformerLookup = new Dictionary<string, TransformerNodeEditor>();

	private Dictionary<string, CombinerModuleNodeEditor> combinerLookup = new Dictionary<string, CombinerModuleNodeEditor>();

	private Dictionary<string, FloatPointsNodeEditor> floatlistLookup = new Dictionary<string, FloatPointsNodeEditor>();

	private Dictionary<string, ControlPointsNodeEditor> ctrlpointsLookup = new Dictionary<string, ControlPointsNodeEditor>();

	[SerializeField]
	public SampleSettings settings
	{
		get;
		private set;
	}

	public static NoiseNodeCanvas CreateInstance()
	{
		NoiseNodeCanvas noiseNodeCanvas = ScriptableObject.CreateInstance<NoiseNodeCanvas>();
		noiseNodeCanvas.ntf = YamlIO.LoadFile<NoiseTreeFiles>(NoiseTreeFiles.GetPath());
		return noiseNodeCanvas;
	}

	public override void UpdateSettings(string sceneCanvasName)
	{
		if (settings == null)
		{
			settings = new SampleSettings();
			settings.name = sceneCanvasName;
		}
	}

	public override string DrawAdditionalSettings(string sceneCanvasName)
	{
		return sceneCanvasName;
	}

	public override void BeforeSavingCanvas()
	{
		foreach (BaseNodeEditor node in nodes)
		{
			NoiseBase target = node.GetTarget();
			if (target != null)
			{
				target.pos = new Vector2f(node.rect.position);
			}
		}
	}

	public override void AdditionalSaveMethods(string sceneCanvasName, CompleteLoadCallback onComplete)
	{
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(new GUIContent("Load Yaml", "Loads the Canvas from a Yaml Save File")))
		{
			Load(sceneCanvasName, onComplete);
		}
		if (GUILayout.Button(new GUIContent("Save to Yaml", "Saves the Canvas to a Yaml file"), GUILayout.ExpandWidth(expand: false)))
		{
			BeforeSavingCanvas();
			Tree tree = BuildTreeFromCanvas();
			if (tree != null)
			{
				tree.ClearEmptyLists();
				string treeFilePath = NoiseTreeFiles.GetTreeFilePath(sceneCanvasName);
				YamlIO.Save(tree, treeFilePath);
			}
		}
		GUILayout.EndHorizontal();
		if (ntf == null)
		{
			ntf = YamlIO.LoadFile<NoiseTreeFiles>(NoiseTreeFiles.GetPath());
		}
		if (ntf != null && GUILayout.Button(new GUIContent("Load Tree", "Loads the Canvas from Trees list")))
		{
			GenericMenu genericMenu = new GenericMenu();
			foreach (string tree_file in ntf.tree_files)
			{
				genericMenu.AddItem(new GUIContent(tree_file), on: false, delegate(object fileName)
				{
					Load((string)fileName, onComplete);
				}, tree_file);
			}
			genericMenu.Show(lastRectPos.position);
		}
		if (Event.current.type == EventType.Repaint)
		{
			Rect lastRect = GUILayoutUtility.GetLastRect();
			lastRectPos = new Rect(lastRect.x + 2f, lastRect.yMax + 2f, lastRect.width - 4f, 0f);
		}
	}

	private void UpdateTerminator()
	{
		if (terminator == null)
		{
			foreach (Node node in nodes)
			{
				if (node.GetType() == typeof(TerminalNodeEditor))
				{
					if (terminator == null)
					{
						terminator = node as TerminalNodeEditor;
					}
					else
					{
						node.Delete();
					}
				}
			}
			if (terminator == null)
			{
				terminator = (TerminalNodeEditor)Node.Create("terminalNodeEditor", Vector2.zero);
			}
		}
		Vector2 position = terminator.rect.min + new Vector2(0f, -290f);
		((DisplayNodeEditor)Node.Create("displayNodeEditor", position)).Inputs[0].ApplyConnection(terminator.Outputs[0]);
	}

	private Link GetLink(Node node)
	{
		Link link = new Link();
		Type type = node.GetType();
		if (type == typeof(PrimitiveNodeEditor))
		{
			PrimitiveNodeEditor primitiveNodeEditor = node as PrimitiveNodeEditor;
			Debug.Assert(primitiveNodeEditor.target.name != null && primitiveNodeEditor.target.name != "", "Invalid target name");
			link.name = primitiveNodeEditor.target.name;
			link.type = Link.Type.Primitive;
		}
		else if (type == typeof(FilterNodeEditor))
		{
			FilterNodeEditor filterNodeEditor = node as FilterNodeEditor;
			Debug.Assert(filterNodeEditor.target.name != null && filterNodeEditor.target.name != "", "Invalid target name");
			link.name = filterNodeEditor.target.name;
			link.type = Link.Type.Filter;
		}
		else if (type == typeof(TransformerNodeEditor))
		{
			TransformerNodeEditor transformerNodeEditor = node as TransformerNodeEditor;
			Debug.Assert(transformerNodeEditor.target.name != null && transformerNodeEditor.target.name != "", "Invalid target name");
			link.name = transformerNodeEditor.target.name;
			link.type = Link.Type.Transformer;
		}
		else if (type == typeof(SelectorModuleNodeEditor))
		{
			SelectorModuleNodeEditor selectorModuleNodeEditor = node as SelectorModuleNodeEditor;
			Debug.Assert(selectorModuleNodeEditor.target.name != null && selectorModuleNodeEditor.target.name != "", "Invalid target name");
			link.name = selectorModuleNodeEditor.target.name;
			link.type = Link.Type.Selector;
		}
		else if (type == typeof(ModifierModuleNodeEditor))
		{
			ModifierModuleNodeEditor modifierModuleNodeEditor = node as ModifierModuleNodeEditor;
			Debug.Assert(modifierModuleNodeEditor.target.name != null && modifierModuleNodeEditor.target.name != "", "Invalid target name");
			link.name = modifierModuleNodeEditor.target.name;
			link.type = Link.Type.Modifier;
		}
		else if (type == typeof(CombinerModuleNodeEditor))
		{
			CombinerModuleNodeEditor combinerModuleNodeEditor = node as CombinerModuleNodeEditor;
			Debug.Assert(combinerModuleNodeEditor.target.name != null && combinerModuleNodeEditor.target.name != "", "Invalid target name");
			link.name = combinerModuleNodeEditor.target.name;
			link.type = Link.Type.Combiner;
		}
		else if (type == typeof(FloatPointsNodeEditor))
		{
			FloatPointsNodeEditor floatPointsNodeEditor = node as FloatPointsNodeEditor;
			Debug.Assert(floatPointsNodeEditor.target.name != null && floatPointsNodeEditor.target.name != "", "Invalid target name");
			link.name = floatPointsNodeEditor.target.name;
			link.type = Link.Type.FloatPoints;
		}
		else if (type == typeof(ControlPointsNodeEditor))
		{
			ControlPointsNodeEditor controlPointsNodeEditor = node as ControlPointsNodeEditor;
			Debug.Assert(controlPointsNodeEditor.target.name != null && controlPointsNodeEditor.target.name != "", "Invalid target name");
			link.name = controlPointsNodeEditor.target.name;
			link.type = Link.Type.ControlPoints;
		}
		else if (type == typeof(TerminalNodeEditor))
		{
			link.name = "TERMINATOR";
			link.type = Link.Type.Terminator;
		}
		return link;
	}

	public Tree BuildTreeFromCanvas()
	{
		Tree tree = new Tree();
		tree.settings = settings;
		foreach (Node node in nodes)
		{
			Type type = node.GetType();
			if (type == typeof(PrimitiveNodeEditor))
			{
				PrimitiveNodeEditor primitiveNodeEditor = node as PrimitiveNodeEditor;
				if (primitiveNodeEditor.target.name == null || primitiveNodeEditor.target.name == "" || tree.primitives.ContainsKey(primitiveNodeEditor.target.name))
				{
					primitiveNodeEditor.target.name = "Primitive" + tree.primitives.Count;
				}
				tree.primitives.Add(primitiveNodeEditor.target.name, primitiveNodeEditor.target);
			}
			else if (type == typeof(FilterNodeEditor))
			{
				FilterNodeEditor filterNodeEditor = node as FilterNodeEditor;
				if (filterNodeEditor.target.name == null || filterNodeEditor.target.name == "" || tree.filters.ContainsKey(filterNodeEditor.target.name))
				{
					filterNodeEditor.target.name = "Filter" + tree.filters.Count;
				}
				tree.filters.Add(filterNodeEditor.target.name, filterNodeEditor.target);
			}
			else if (type == typeof(TransformerNodeEditor))
			{
				TransformerNodeEditor transformerNodeEditor = node as TransformerNodeEditor;
				if (transformerNodeEditor.target.name == null || transformerNodeEditor.target.name == "" || tree.transformers.ContainsKey(transformerNodeEditor.target.name))
				{
					transformerNodeEditor.target.name = "Transformer" + tree.transformers.Count;
				}
				tree.transformers.Add(transformerNodeEditor.target.name, transformerNodeEditor.target);
			}
			else if (type == typeof(SelectorModuleNodeEditor))
			{
				SelectorModuleNodeEditor selectorModuleNodeEditor = node as SelectorModuleNodeEditor;
				if (selectorModuleNodeEditor.target.name == null || selectorModuleNodeEditor.target.name == "" || tree.selectors.ContainsKey(selectorModuleNodeEditor.target.name))
				{
					selectorModuleNodeEditor.target.name = "Selector" + tree.selectors.Count;
				}
				tree.selectors.Add(selectorModuleNodeEditor.target.name, selectorModuleNodeEditor.target);
			}
			else if (type == typeof(ModifierModuleNodeEditor))
			{
				ModifierModuleNodeEditor modifierModuleNodeEditor = node as ModifierModuleNodeEditor;
				if (modifierModuleNodeEditor.target.name == null || modifierModuleNodeEditor.target.name == "" || tree.modifiers.ContainsKey(modifierModuleNodeEditor.target.name))
				{
					modifierModuleNodeEditor.target.name = "Modifier" + tree.modifiers.Count;
				}
				tree.modifiers.Add(modifierModuleNodeEditor.target.name, modifierModuleNodeEditor.target);
			}
			else if (type == typeof(CombinerModuleNodeEditor))
			{
				CombinerModuleNodeEditor combinerModuleNodeEditor = node as CombinerModuleNodeEditor;
				if (combinerModuleNodeEditor.target.name == null || combinerModuleNodeEditor.target.name == "" || tree.combiners.ContainsKey(combinerModuleNodeEditor.target.name))
				{
					combinerModuleNodeEditor.target.name = "Combiner" + tree.combiners.Count;
				}
				tree.combiners.Add(combinerModuleNodeEditor.target.name, combinerModuleNodeEditor.target);
			}
			else if (type == typeof(FloatPointsNodeEditor))
			{
				FloatPointsNodeEditor floatPointsNodeEditor = node as FloatPointsNodeEditor;
				if (floatPointsNodeEditor.target.name == null || floatPointsNodeEditor.target.name == "" || tree.floats.ContainsKey(floatPointsNodeEditor.target.name))
				{
					floatPointsNodeEditor.target.name = "Terrace Control" + tree.combiners.Count;
				}
				tree.floats.Add(floatPointsNodeEditor.target.name, floatPointsNodeEditor.target);
			}
			else if (type == typeof(ControlPointsNodeEditor))
			{
				ControlPointsNodeEditor controlPointsNodeEditor = node as ControlPointsNodeEditor;
				if (controlPointsNodeEditor.target.name == null || controlPointsNodeEditor.target.name == "" || tree.controlpoints.ContainsKey(controlPointsNodeEditor.target.name))
				{
					controlPointsNodeEditor.target.name = "Curve Control" + tree.combiners.Count;
				}
				tree.controlpoints.Add(controlPointsNodeEditor.target.name, controlPointsNodeEditor.target);
			}
			else if (type == typeof(TerminalNodeEditor) && terminator == null)
			{
				terminator = node as TerminalNodeEditor;
			}
		}
		foreach (Node node2 in nodes)
		{
			Type type2 = node2.GetType();
			if (type2 == typeof(FilterNodeEditor))
			{
				FilterNodeEditor filterNodeEditor2 = node2 as FilterNodeEditor;
				NodeLink nodeLink = new NodeLink();
				nodeLink.target = GetLink(node2);
				if (filterNodeEditor2.Inputs[0] != null && filterNodeEditor2.Inputs[0].connection != null)
				{
					nodeLink.source0 = GetLink(filterNodeEditor2.Inputs[0].connection.body);
				}
				tree.links.Add(nodeLink);
			}
			else if (type2 == typeof(TransformerNodeEditor))
			{
				TransformerNodeEditor transformerNodeEditor2 = node2 as TransformerNodeEditor;
				NodeLink nodeLink2 = new NodeLink();
				nodeLink2.target = GetLink(node2);
				if (transformerNodeEditor2.Inputs[0] != null && transformerNodeEditor2.Inputs[0].connection != null)
				{
					nodeLink2.source0 = GetLink(transformerNodeEditor2.Inputs[0].connection.body);
				}
				if (transformerNodeEditor2.Inputs[1] != null && transformerNodeEditor2.Inputs[1].connection != null)
				{
					nodeLink2.source1 = GetLink(transformerNodeEditor2.Inputs[1].connection.body);
				}
				if (transformerNodeEditor2.Inputs[2] != null && transformerNodeEditor2.Inputs[2].connection != null)
				{
					nodeLink2.source2 = GetLink(transformerNodeEditor2.Inputs[2].connection.body);
				}
				if (transformerNodeEditor2.Inputs[3] != null && transformerNodeEditor2.Inputs[3].connection != null)
				{
					nodeLink2.source3 = GetLink(transformerNodeEditor2.Inputs[3].connection.body);
				}
				tree.links.Add(nodeLink2);
			}
			else if (type2 == typeof(SelectorModuleNodeEditor))
			{
				SelectorModuleNodeEditor selectorModuleNodeEditor2 = node2 as SelectorModuleNodeEditor;
				NodeLink nodeLink3 = new NodeLink();
				nodeLink3.target = GetLink(node2);
				if (selectorModuleNodeEditor2.Inputs[0] != null && selectorModuleNodeEditor2.Inputs[0].connection != null)
				{
					nodeLink3.source0 = GetLink(selectorModuleNodeEditor2.Inputs[0].connection.body);
				}
				if (selectorModuleNodeEditor2.Inputs[1] != null && selectorModuleNodeEditor2.Inputs[1].connection != null)
				{
					nodeLink3.source1 = GetLink(selectorModuleNodeEditor2.Inputs[1].connection.body);
				}
				if (selectorModuleNodeEditor2.Inputs[2] != null && selectorModuleNodeEditor2.Inputs[2].connection != null)
				{
					nodeLink3.source2 = GetLink(selectorModuleNodeEditor2.Inputs[2].connection.body);
				}
				tree.links.Add(nodeLink3);
			}
			else if (type2 == typeof(ModifierModuleNodeEditor))
			{
				ModifierModuleNodeEditor modifierModuleNodeEditor2 = node2 as ModifierModuleNodeEditor;
				NodeLink nodeLink4 = new NodeLink();
				nodeLink4.target = GetLink(node2);
				if (modifierModuleNodeEditor2.Inputs[0] != null && modifierModuleNodeEditor2.Inputs[0].connection != null)
				{
					nodeLink4.source0 = GetLink(modifierModuleNodeEditor2.Inputs[0].connection.body);
				}
				if (modifierModuleNodeEditor2.Inputs[1] != null && modifierModuleNodeEditor2.Inputs[1].connection != null)
				{
					nodeLink4.source1 = GetLink(modifierModuleNodeEditor2.Inputs[1].connection.body);
				}
				if (modifierModuleNodeEditor2.Inputs[2] != null && modifierModuleNodeEditor2.Inputs[2].connection != null)
				{
					nodeLink4.source2 = GetLink(modifierModuleNodeEditor2.Inputs[2].connection.body);
				}
				tree.links.Add(nodeLink4);
			}
			else if (type2 == typeof(CombinerModuleNodeEditor))
			{
				CombinerModuleNodeEditor combinerModuleNodeEditor2 = node2 as CombinerModuleNodeEditor;
				NodeLink nodeLink5 = new NodeLink();
				nodeLink5.target = GetLink(node2);
				if (combinerModuleNodeEditor2.Inputs[0] != null && combinerModuleNodeEditor2.Inputs[0].connection != null)
				{
					nodeLink5.source0 = GetLink(combinerModuleNodeEditor2.Inputs[0].connection.body);
				}
				if (combinerModuleNodeEditor2.Inputs[1] != null && combinerModuleNodeEditor2.Inputs[1].connection != null)
				{
					nodeLink5.source1 = GetLink(combinerModuleNodeEditor2.Inputs[1].connection.body);
				}
				tree.links.Add(nodeLink5);
			}
			else if (type2 == typeof(TerminalNodeEditor))
			{
				TerminalNodeEditor terminalNodeEditor = node2 as TerminalNodeEditor;
				NodeLink nodeLink6 = new NodeLink();
				nodeLink6.target = GetLink(node2);
				if (terminalNodeEditor.Inputs[0] != null && terminalNodeEditor.Inputs[0].connection != null)
				{
					nodeLink6.source0 = GetLink(terminalNodeEditor.Inputs[0].connection.body);
				}
				tree.links.Add(nodeLink6);
			}
		}
		return tree;
	}

	private NodeCanvas Load(string name, CompleteLoadCallback onComplete)
	{
		NodeCanvas nodeCanvas = null;
		Tree tree = YamlIO.LoadFile<Tree>(NoiseTreeFiles.GetTreeFilePath(name));
		if (tree != null)
		{
			if (tree.settings.name == null || tree.settings.name == "")
			{
				tree.settings.name = name;
			}
			nodeCanvas = PopulateNoiseNodeEditor(tree);
		}
		onComplete(name, nodeCanvas);
		return nodeCanvas;
	}

	private Node GetNodeFromLink(Link link)
	{
		if (link == null)
		{
			return null;
		}
		switch (link.type)
		{
		case Link.Type.Primitive:
			if (primitiveLookup.ContainsKey(link.name))
			{
				return primitiveLookup[link.name];
			}
			Debug.LogError("Couldnt find [" + link.name + "] in primitives");
			break;
		case Link.Type.Filter:
			if (filterLookup.ContainsKey(link.name))
			{
				return filterLookup[link.name];
			}
			Debug.LogError("Couldnt find [" + link.name + "] in filters");
			break;
		case Link.Type.Modifier:
			if (modifierLookup.ContainsKey(link.name))
			{
				return modifierLookup[link.name];
			}
			Debug.LogError("Couldnt find [" + link.name + "] in modifiers");
			break;
		case Link.Type.Selector:
			if (selectorLookup.ContainsKey(link.name))
			{
				return selectorLookup[link.name];
			}
			Debug.LogError("Couldnt find [" + link.name + "] in selectors");
			break;
		case Link.Type.Transformer:
			if (transformerLookup.ContainsKey(link.name))
			{
				return transformerLookup[link.name];
			}
			Debug.LogError("Couldnt find [" + link.name + "] in transformers");
			break;
		case Link.Type.Combiner:
			if (combinerLookup.ContainsKey(link.name))
			{
				return combinerLookup[link.name];
			}
			Debug.LogError("Couldnt find [" + link.name + "] in combiners");
			break;
		case Link.Type.FloatPoints:
			if (floatlistLookup.ContainsKey(link.name))
			{
				return floatlistLookup[link.name];
			}
			Debug.LogError("Couldnt find [" + link.name + "] in float points");
			break;
		case Link.Type.ControlPoints:
			if (ctrlpointsLookup.ContainsKey(link.name))
			{
				return ctrlpointsLookup[link.name];
			}
			Debug.LogError("Couldnt find [" + link.name + "] in control points");
			break;
		case Link.Type.Terminator:
			if (terminator == null)
			{
				terminator = (TerminalNodeEditor)Node.Create("terminalNodeEditor", Vector2.zero);
				terminator.name = link.name;
			}
			return terminator;
		}
		Debug.LogError("Couldnt find link [" + link.name + "] [" + link.type.ToString() + "]");
		return null;
	}

	private static NoiseNodeCanvas PopulateNoiseNodeEditor(Tree tree)
	{
		NodeCanvas nodeCanvas = (NodeEditor.curNodeCanvas = CreateInstance());
		((NoiseNodeCanvas)nodeCanvas).Populate(tree);
		return (NoiseNodeCanvas)nodeCanvas;
	}

	private void Populate(Tree tree)
	{
		settings = tree.settings;
		primitiveLookup.Clear();
		foreach (KeyValuePair<string, Primitive> primitive in tree.primitives)
		{
			PrimitiveNodeEditor primitiveNodeEditor = (PrimitiveNodeEditor)Node.Create("primitiveNodeEditor", primitive.Value.pos);
			primitiveNodeEditor.name = primitive.Key;
			primitiveNodeEditor.target = primitive.Value;
			primitiveLookup.Add(primitive.Key, primitiveNodeEditor);
		}
		filterLookup.Clear();
		foreach (KeyValuePair<string, Filter> filter in tree.filters)
		{
			FilterNodeEditor filterNodeEditor = (FilterNodeEditor)Node.Create("filterNodeEditor", filter.Value.pos);
			filterNodeEditor.name = filter.Key;
			filterNodeEditor.target = filter.Value;
			filterLookup.Add(filter.Key, filterNodeEditor);
		}
		modifierLookup.Clear();
		foreach (KeyValuePair<string, ProcGen.Noise.Modifier> modifier in tree.modifiers)
		{
			ModifierModuleNodeEditor modifierModuleNodeEditor = (ModifierModuleNodeEditor)Node.Create("modifierModuleNodeEditor", modifier.Value.pos);
			modifierModuleNodeEditor.name = modifier.Key;
			modifierModuleNodeEditor.target = modifier.Value;
			modifierLookup.Add(modifier.Key, modifierModuleNodeEditor);
		}
		selectorLookup.Clear();
		foreach (KeyValuePair<string, Selector> selector in tree.selectors)
		{
			SelectorModuleNodeEditor selectorModuleNodeEditor = (SelectorModuleNodeEditor)Node.Create("selectorModuleNodeEditor", selector.Value.pos);
			selectorModuleNodeEditor.name = selector.Key;
			selectorModuleNodeEditor.target = selector.Value;
			selectorLookup.Add(selector.Key, selectorModuleNodeEditor);
		}
		transformerLookup.Clear();
		foreach (KeyValuePair<string, Transformer> transformer in tree.transformers)
		{
			TransformerNodeEditor transformerNodeEditor = (TransformerNodeEditor)Node.Create("transformerNodeEditor", transformer.Value.pos);
			transformerNodeEditor.name = transformer.Key;
			transformerNodeEditor.target = transformer.Value;
			transformerLookup.Add(transformer.Key, transformerNodeEditor);
		}
		combinerLookup.Clear();
		foreach (KeyValuePair<string, Combiner> combiner in tree.combiners)
		{
			CombinerModuleNodeEditor combinerModuleNodeEditor = (CombinerModuleNodeEditor)Node.Create("combinerModuleNodeEditor", combiner.Value.pos);
			combinerModuleNodeEditor.name = combiner.Key;
			combinerModuleNodeEditor.target = combiner.Value;
			combinerLookup.Add(combiner.Key, combinerModuleNodeEditor);
		}
		floatlistLookup.Clear();
		foreach (KeyValuePair<string, FloatList> @float in tree.floats)
		{
			FloatPointsNodeEditor floatPointsNodeEditor = (FloatPointsNodeEditor)Node.Create("floatPointsNodeEditor", @float.Value.pos);
			floatPointsNodeEditor.name = @float.Key;
			floatPointsNodeEditor.target = @float.Value;
			floatlistLookup.Add(@float.Key, floatPointsNodeEditor);
		}
		ctrlpointsLookup.Clear();
		foreach (KeyValuePair<string, ControlPointList> controlpoint in tree.controlpoints)
		{
			ControlPointsNodeEditor controlPointsNodeEditor = (ControlPointsNodeEditor)Node.Create("controlPointsNodeEditor", controlpoint.Value.pos);
			controlPointsNodeEditor.name = controlpoint.Key;
			controlPointsNodeEditor.target = controlpoint.Value;
			ctrlpointsLookup.Add(controlpoint.Key, controlPointsNodeEditor);
		}
		for (int i = 0; i < tree.links.Count; i++)
		{
			NodeLink nodeLink = tree.links[i];
			Node nodeFromLink = GetNodeFromLink(nodeLink.target);
			Node node = null;
			Node node2 = null;
			Node node3 = null;
			Node node4 = null;
			switch (nodeLink.target.type)
			{
			case Link.Type.Filter:
			case Link.Type.Terminator:
				node = GetNodeFromLink(nodeLink.source0);
				break;
			case Link.Type.Combiner:
				node = GetNodeFromLink(nodeLink.source0);
				node2 = GetNodeFromLink(nodeLink.source1);
				break;
			case Link.Type.Selector:
			case Link.Type.Modifier:
				node = GetNodeFromLink(nodeLink.source0);
				node2 = GetNodeFromLink(nodeLink.source1);
				node3 = GetNodeFromLink(nodeLink.source2);
				break;
			case Link.Type.Transformer:
				node = GetNodeFromLink(nodeLink.source0);
				node2 = GetNodeFromLink(nodeLink.source1);
				node3 = GetNodeFromLink(nodeLink.source2);
				node4 = GetNodeFromLink(nodeLink.source3);
				break;
			}
			if (node != null)
			{
				if (nodeFromLink.Inputs.Count == 0)
				{
					Debug.LogError(string.Concat("Target [", nodeFromLink.name, "][", nodeLink.target.type, "] doesnt have any inputs"));
				}
				if (node.Outputs.Count == 0)
				{
					Debug.LogError(string.Concat("Source [", node.name, "][", nodeLink.source0.type, "] doesnt have any outputs"));
				}
				nodeFromLink.Inputs[0].ApplyConnection(node.Outputs[0]);
			}
			if (node2 != null)
			{
				nodeFromLink.Inputs[1].ApplyConnection(node2.Outputs[0]);
			}
			if (node3 != null)
			{
				nodeFromLink.Inputs[2].ApplyConnection(node3.Outputs[0]);
			}
			if (node4 != null)
			{
				nodeFromLink.Inputs[3].ApplyConnection(node4.Outputs[0]);
			}
		}
		UpdateTerminator();
	}
}
