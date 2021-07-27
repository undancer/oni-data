using System.Collections.Generic;
using KSerialization;
using UnityEngine;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptOut)]
	public struct Segment
	{
		public Vector2 e0;

		public Vector2 e1;

		public Segment(Vector2 e0, Vector2 e1)
		{
			this.e0 = e0;
			this.e1 = e1;
		}

		public List<Segment> Stagger(SeededRandom rnd, float maxDistance = 10f, float staggerRange = 3f)
		{
			List<Segment> list = new List<Segment>();
			Vector2 vector = e1 - e0;
			Vector2 vector2 = e0;
			Vector2 vector3 = e1;
			float num = vector.magnitude / maxDistance;
			Vector2 normalized = new Vector2(0f - vector.y, vector.x).normalized;
			for (int i = 0; (float)i < num; i++)
			{
				vector3 = e0 + vector * (1f / num) * i + normalized * rnd.RandomRange(0f - staggerRange, staggerRange);
				list.Add(new Segment(vector2, vector3));
				vector2 = vector3;
			}
			list.Add(new Segment(vector3, e1));
			return list;
		}
	}
}
