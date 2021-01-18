using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class ResourceTreeLoader<T> : ResourceLoader<T> where T : ResourceTreeNode, new()
{
	public ResourceTreeLoader(TextAsset file)
		: base(file)
	{
	}

	public override void Load(TextAsset file)
	{
		Dictionary<string, ResourceTreeNode> dictionary = new Dictionary<string, ResourceTreeNode>();
		using (XmlReader xmlReader = XmlReader.Create(new StringReader(file.text)))
		{
			while (xmlReader.ReadToFollowing("node"))
			{
				xmlReader.MoveToFirstAttribute();
				string value = xmlReader.Value;
				float nodeX = 0f;
				float nodeY = 0f;
				float num = 40f;
				float num2 = 20f;
				if (xmlReader.ReadToFollowing("Geometry"))
				{
					xmlReader.MoveToAttribute("x");
					nodeX = float.Parse(xmlReader.Value);
					xmlReader.MoveToAttribute("y");
					nodeY = 0f - float.Parse(xmlReader.Value);
					xmlReader.MoveToAttribute("width");
					num = float.Parse(xmlReader.Value);
					xmlReader.MoveToAttribute("height");
					num2 = float.Parse(xmlReader.Value);
				}
				Debug.Assert(num != 0f && num2 != 0f, "Error parsing GRAPHML");
				if (xmlReader.ReadToFollowing("NodeLabel"))
				{
					string text = xmlReader.ReadString();
					T val = new T();
					val.Id = text;
					val.Name = text;
					val.nodeX = nodeX;
					val.nodeY = nodeY;
					val.width = num;
					val.height = num2;
					dictionary[value] = val;
					resources.Add(val);
				}
			}
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(file.text);
		XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectNodes("/graphml/graph/edge");
		foreach (XmlNode item in xmlNodeList)
		{
			ResourceTreeNode value2 = null;
			dictionary.TryGetValue(item.Attributes["source"].Value, out value2);
			ResourceTreeNode value3 = null;
			dictionary.TryGetValue(item.Attributes["target"].Value, out value3);
			if (value2 == null || value3 == null)
			{
				continue;
			}
			value2.references.Add(value3);
			ResourceTreeNode.Edge edge = null;
			XmlNode xmlNode2 = null;
			foreach (XmlNode childNode in item.ChildNodes)
			{
				if (childNode.HasChildNodes)
				{
					xmlNode2 = childNode.FirstChild;
					break;
				}
			}
			string name = xmlNode2.Name;
			ResourceTreeNode.Edge.EdgeType edgeType = (ResourceTreeNode.Edge.EdgeType)Enum.Parse(typeof(ResourceTreeNode.Edge.EdgeType), name);
			edge = new ResourceTreeNode.Edge(value2, value3, edgeType);
			foreach (XmlNode childNode2 in xmlNode2.ChildNodes)
			{
				if (childNode2.Name != "Path")
				{
					continue;
				}
				edge.sourceOffset = new Vector2f(float.Parse(childNode2.Attributes["sx"].Value), 0f - float.Parse(childNode2.Attributes["sy"].Value));
				edge.targetOffset = new Vector2f(float.Parse(childNode2.Attributes["tx"].Value), 0f - float.Parse(childNode2.Attributes["ty"].Value));
				foreach (XmlNode childNode3 in childNode2.ChildNodes)
				{
					Vector2f point = new Vector2f(float.Parse(childNode3.Attributes["x"].Value), 0f - float.Parse(childNode3.Attributes["y"].Value));
					edge.AddToPath(point);
				}
				break;
			}
			value2.edges.Add(edge);
		}
	}
}
