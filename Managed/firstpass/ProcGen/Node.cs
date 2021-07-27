using KSerialization;
using Satsuma;
using UnityEngine;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Node
	{
		private bool nodeSet;

		[Serialize]
		public TagSet tags = new TagSet();

		[Serialize]
		public Tag templateTag = Tag.Invalid;

		[Serialize]
		public TagSet featureSpecificTags = new TagSet();

		[Serialize]
		public TagSet biomeSpecificTags = new TagSet();

		internal Satsuma.Node node { get; private set; }

		[Serialize]
		public string type { get; private set; }

		[Serialize]
		public Vector2 position { get; private set; }

		public void SetNode(Satsuma.Node node)
		{
			Debug.Assert(!nodeSet, "Tried initializing a Node twice, that ain't gonna work.");
			this.node = node;
			nodeSet = true;
		}

		public void SetType(string newtype)
		{
			type = newtype;
		}

		public string GetSubworld()
		{
			foreach (Tag tag in tags)
			{
				if (tag.Name.Contains("subworlds/"))
				{
					return tag.Name;
				}
			}
			return "MISSING";
		}

		public string GetBiome()
		{
			foreach (Tag tag in tags)
			{
				if (tag.Name.Contains("biomes/"))
				{
					return tag.Name;
				}
			}
			return "MISSING";
		}

		public string GetFeature()
		{
			foreach (Tag tag in tags)
			{
				if (tag.Name.Contains("features/"))
				{
					return tag.Name;
				}
			}
			return null;
		}

		public void SetPosition(Vector2 newPos)
		{
			position = newPos;
		}

		public Node()
		{
		}

		public Node(string type)
		{
			this.type = type;
		}

		public Node(Node other)
		{
			position = other.position;
			node = other.node;
			type = other.type;
			tags = new TagSet(other.tags);
			featureSpecificTags = new TagSet(other.featureSpecificTags);
			biomeSpecificTags = new TagSet(other.biomeSpecificTags);
		}

		public Node(Satsuma.Node node, string type, Vector2 position = default(Vector2))
		{
			this.node = node;
			this.type = type;
			this.position = position;
		}
	}
}
