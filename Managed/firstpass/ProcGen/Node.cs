using KSerialization;
using Satsuma;
using UnityEngine;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Node
	{
		[Serialize]
		public TagSet tags = new TagSet();

		public TagSet featureSpecificTags = new TagSet();

		public TagSet biomeSpecificTags = new TagSet();

		public Satsuma.Node node
		{
			get;
			private set;
		}

		[Serialize]
		public string type
		{
			get;
			private set;
		}

		[Serialize]
		public Vector2 position
		{
			get;
			private set;
		}

		public void SetType(string newtype)
		{
			type = newtype;
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

		public Node(Satsuma.Node node, string type)
		{
			this.node = node;
			this.type = type;
		}
	}
}
