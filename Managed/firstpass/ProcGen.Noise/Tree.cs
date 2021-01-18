using System.Collections.Generic;
using LibNoiseDotNet.Graphics.Tools.Noise;

namespace ProcGen.Noise
{
	public class Tree
	{
		private Dictionary<string, IModule3D> primitiveLookup = new Dictionary<string, IModule3D>();

		private Dictionary<string, IModule3D> filterLookup = new Dictionary<string, IModule3D>();

		private Dictionary<string, IModule3D> modifierLookup = new Dictionary<string, IModule3D>();

		private Dictionary<string, IModule3D> selectorLookup = new Dictionary<string, IModule3D>();

		private Dictionary<string, IModule3D> transformerLookup = new Dictionary<string, IModule3D>();

		private Dictionary<string, IModule3D> combinerLookup = new Dictionary<string, IModule3D>();

		public SampleSettings settings
		{
			get;
			set;
		}

		public List<NodeLink> links
		{
			get;
			set;
		}

		public Dictionary<string, Primitive> primitives
		{
			get;
			set;
		}

		public Dictionary<string, Filter> filters
		{
			get;
			set;
		}

		public Dictionary<string, Transformer> transformers
		{
			get;
			set;
		}

		public Dictionary<string, Selector> selectors
		{
			get;
			set;
		}

		public Dictionary<string, Modifier> modifiers
		{
			get;
			set;
		}

		public Dictionary<string, Combiner> combiners
		{
			get;
			set;
		}

		public Dictionary<string, FloatList> floats
		{
			get;
			set;
		}

		public Dictionary<string, ControlPointList> controlpoints
		{
			get;
			set;
		}

		public Tree()
		{
			settings = new SampleSettings();
			links = new List<NodeLink>();
			primitives = new Dictionary<string, Primitive>();
			filters = new Dictionary<string, Filter>();
			transformers = new Dictionary<string, Transformer>();
			selectors = new Dictionary<string, Selector>();
			modifiers = new Dictionary<string, Modifier>();
			combiners = new Dictionary<string, Combiner>();
			floats = new Dictionary<string, FloatList>();
			controlpoints = new Dictionary<string, ControlPointList>();
		}

		public void ClearEmptyLists()
		{
			if (links.Count == 0)
			{
				links = null;
			}
			if (primitives.Count == 0)
			{
				primitives = null;
			}
			if (filters.Count == 0)
			{
				filters = null;
			}
			if (transformers.Count == 0)
			{
				transformers = null;
			}
			if (selectors.Count == 0)
			{
				selectors = null;
			}
			if (modifiers.Count == 0)
			{
				modifiers = null;
			}
			if (combiners.Count == 0)
			{
				combiners = null;
			}
			if (floats.Count == 0)
			{
				floats = null;
			}
			if (controlpoints.Count == 0)
			{
				controlpoints = null;
			}
		}

		public void CreateEmptyLists()
		{
			if (links == null)
			{
				links = new List<NodeLink>();
			}
			if (primitives == null)
			{
				primitives = new Dictionary<string, Primitive>();
			}
			if (filters == null)
			{
				filters = new Dictionary<string, Filter>();
			}
			if (transformers == null)
			{
				transformers = new Dictionary<string, Transformer>();
			}
			if (selectors == null)
			{
				selectors = new Dictionary<string, Selector>();
			}
			if (modifiers == null)
			{
				modifiers = new Dictionary<string, Modifier>();
			}
			if (combiners == null)
			{
				combiners = new Dictionary<string, Combiner>();
			}
			if (floats == null)
			{
				floats = new Dictionary<string, FloatList>();
			}
			if (controlpoints == null)
			{
				controlpoints = new Dictionary<string, ControlPointList>();
			}
		}

		private IModule3D GetModuleFromLink(Link link)
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
			case Link.Type.Terminator:
				return null;
			}
			Debug.LogError("Couldnt find link [" + link.name + "] [" + link.type.ToString() + "]");
			return null;
		}

		public IModule3D BuildFinalModule(int globalSeed)
		{
			IModule3D module3D = null;
			primitiveLookup.Clear();
			filterLookup.Clear();
			modifierLookup.Clear();
			selectorLookup.Clear();
			transformerLookup.Clear();
			combinerLookup.Clear();
			foreach (KeyValuePair<string, Primitive> primitive in primitives)
			{
				primitiveLookup.Add(primitive.Key, primitive.Value.CreateModule(globalSeed));
			}
			foreach (KeyValuePair<string, Filter> filter in filters)
			{
				filterLookup.Add(filter.Key, filter.Value.CreateModule());
			}
			foreach (KeyValuePair<string, Modifier> modifier in modifiers)
			{
				modifierLookup.Add(modifier.Key, modifier.Value.CreateModule());
			}
			foreach (KeyValuePair<string, Selector> selector in selectors)
			{
				selectorLookup.Add(selector.Key, selector.Value.CreateModule());
			}
			foreach (KeyValuePair<string, Transformer> transformer in transformers)
			{
				transformerLookup.Add(transformer.Key, transformer.Value.CreateModule());
			}
			foreach (KeyValuePair<string, Combiner> combiner in combiners)
			{
				combinerLookup.Add(combiner.Key, combiner.Value.CreateModule());
			}
			for (int i = 0; i < links.Count; i++)
			{
				NodeLink nodeLink = links[i];
				IModule3D moduleFromLink = GetModuleFromLink(nodeLink.target);
				if (nodeLink.target.type == Link.Type.Terminator)
				{
					module3D = GetModuleFromLink(nodeLink.source0);
					continue;
				}
				IModule3D module3D2 = null;
				IModule3D module3D3 = null;
				IModule3D module3D4 = null;
				IModule3D module3D5 = null;
				switch (nodeLink.target.type)
				{
				case Link.Type.Filter:
					module3D2 = GetModuleFromLink(nodeLink.source0);
					filters[nodeLink.target.name].SetSouces(moduleFromLink, module3D2);
					((FilterModule)moduleFromLink).Primitive3D = module3D2;
					break;
				case Link.Type.Modifier:
				{
					module3D2 = GetModuleFromLink(nodeLink.source0);
					ControlPointList controlPoints = null;
					if (nodeLink.source1 != null && nodeLink.source1.type == Link.Type.ControlPoints && controlpoints.ContainsKey(nodeLink.source1.name))
					{
						controlPoints = controlpoints[nodeLink.source1.name];
					}
					FloatList controlFloats = null;
					if (nodeLink.source1 != null && nodeLink.source1.type == Link.Type.FloatPoints && floats.ContainsKey(nodeLink.source1.name))
					{
						controlFloats = floats[nodeLink.source1.name];
					}
					modifiers[nodeLink.target.name].SetSouces(moduleFromLink, module3D2, controlFloats, controlPoints);
					break;
				}
				case Link.Type.Selector:
					module3D2 = GetModuleFromLink(nodeLink.source0);
					module3D3 = GetModuleFromLink(nodeLink.source1);
					module3D4 = GetModuleFromLink(nodeLink.source2);
					selectors[nodeLink.target.name].SetSouces(moduleFromLink, module3D4, module3D2, module3D3);
					break;
				case Link.Type.Transformer:
					module3D2 = GetModuleFromLink(nodeLink.source0);
					module3D3 = GetModuleFromLink(nodeLink.source1);
					module3D4 = GetModuleFromLink(nodeLink.source2);
					module3D5 = GetModuleFromLink(nodeLink.source3);
					transformers[nodeLink.target.name].SetSouces(moduleFromLink, module3D2, module3D3, module3D4, module3D5);
					break;
				case Link.Type.Combiner:
					module3D2 = GetModuleFromLink(nodeLink.source0);
					module3D3 = GetModuleFromLink(nodeLink.source1);
					combiners[nodeLink.target.name].SetSouces(moduleFromLink, module3D2, module3D3);
					break;
				}
			}
			Debug.Assert(module3D != null, "Missing Terminus module");
			return module3D;
		}

		public string[] GetPrimitiveNames()
		{
			string[] array = new string[primitives.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, Primitive> primitive in primitives)
			{
				array[num++] = primitive.Key;
			}
			return array;
		}

		public string[] GetFilterNames()
		{
			string[] array = new string[filters.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, Filter> filter in filters)
			{
				array[num++] = filter.Key;
			}
			return array;
		}
	}
}
