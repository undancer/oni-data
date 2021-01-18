using System.Diagnostics;
using UnityEngine;

namespace Database
{
	[DebuggerDisplay("{Id}")]
	public class AsteroidType : Resource
	{
		public string animName;

		public AsteroidType(string id, ResourceSet parent, string animName)
			: base(id, parent)
		{
			this.animName = animName;
		}

		public Sprite GetUISprite()
		{
			KAnimFile anim = Assets.GetAnim(animName);
			return Def.GetUISpriteFromMultiObjectAnim(anim);
		}
	}
}
