using System.Xml.Linq;

namespace Satsuma.IO.GraphML
{
	public abstract class GraphMLProperty
	{
		public string Name
		{
			get;
			set;
		}

		public PropertyDomain Domain
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		protected GraphMLProperty()
		{
			Domain = PropertyDomain.All;
		}

		protected static string DomainToGraphML(PropertyDomain domain)
		{
			return domain switch
			{
				PropertyDomain.Node => "node", 
				PropertyDomain.Arc => "arc", 
				PropertyDomain.Graph => "graph", 
				_ => "all", 
			};
		}

		protected static PropertyDomain ParseDomain(string s)
		{
			return s switch
			{
				"node" => PropertyDomain.Node, 
				"edge" => PropertyDomain.Arc, 
				"graph" => PropertyDomain.Graph, 
				_ => PropertyDomain.All, 
			};
		}

		protected virtual void LoadFromKeyElement(XElement xKey)
		{
			Name = xKey.Attribute("attr.name")?.Value;
			Domain = ParseDomain(xKey.Attribute("for").Value);
			Id = xKey.Attribute("id").Value;
			XElement x = Utils.ElementLocal(xKey, "default");
			ReadData(x, null);
		}

		public virtual XElement GetKeyElement()
		{
			XElement xElement = new XElement(GraphMLFormat.xmlns + "key");
			xElement.SetAttributeValue("attr.name", Name);
			xElement.SetAttributeValue("for", DomainToGraphML(Domain));
			xElement.SetAttributeValue("id", Id);
			XElement xElement2 = WriteData(null);
			if (xElement2 != null)
			{
				xElement2.Name = GraphMLFormat.xmlns + "default";
				xElement.Add(xElement2);
			}
			return xElement;
		}

		public abstract void ReadData(XElement x, object key);

		public abstract XElement WriteData(object key);
	}
}
