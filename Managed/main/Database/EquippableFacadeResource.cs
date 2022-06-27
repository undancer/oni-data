using UnityEngine;

namespace Database
{
	public class EquippableFacadeResource : Resource
	{
		public string BuildOverride { get; private set; }

		public string DefID { get; private set; }

		public KAnimFile AnimFile { get; private set; }

		public EquippableFacadeResource(string id, string buildOverride, string defID, string animFile)
			: base(id)
		{
			DefID = defID;
			BuildOverride = buildOverride;
			AnimFile = Assets.GetAnim(animFile);
		}

		public Tuple<Sprite, Color> GetUISprite()
		{
			Sprite uISpriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(AnimFile);
			return new Tuple<Sprite, Color>(uISpriteFromMultiObjectAnim, (uISpriteFromMultiObjectAnim != null) ? Color.white : Color.clear);
		}
	}
}
