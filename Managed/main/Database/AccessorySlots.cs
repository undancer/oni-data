namespace Database
{
	public class AccessorySlots : ResourceSet<AccessorySlot>
	{
		public AccessorySlot Eyes;

		public AccessorySlot Hair;

		public AccessorySlot HeadShape;

		public AccessorySlot Mouth;

		public AccessorySlot Body;

		public AccessorySlot Arm;

		public AccessorySlot Hat;

		public AccessorySlot HatHair;

		public AccessorySlot HairAlways;

		public AccessorySlot HeadEffects;

		public AccessorySlots(ResourceSet parent)
			: base("AccessorySlots", parent)
		{
			parent = Db.Get().Accessories;
			KAnimFile anim = Assets.GetAnim("head_swap_kanim");
			KAnimFile anim2 = Assets.GetAnim("body_comp_default_kanim");
			KAnimFile anim3 = Assets.GetAnim("body_swap_kanim");
			KAnimFile anim4 = Assets.GetAnim("hair_swap_kanim");
			KAnimFile anim5 = Assets.GetAnim("hat_swap_kanim");
			Eyes = new AccessorySlot("Eyes", this, anim);
			Hair = new AccessorySlot("Hair", this, anim4);
			HeadShape = new AccessorySlot("HeadShape", this, anim);
			Mouth = new AccessorySlot("Mouth", this, anim);
			Hat = new AccessorySlot("Hat", this, anim5);
			HatHair = new AccessorySlot("Hat_Hair", this, anim4);
			HairAlways = new AccessorySlot("Hair_Always", this, anim4, "hair");
			HeadEffects = new AccessorySlot("HeadFX", this, anim);
			Body = new AccessorySlot("Body", this, anim3);
			Arm = new AccessorySlot("Arm", this, anim3);
			foreach (AccessorySlot resource in resources)
			{
				resource.AddAccessories(anim2, parent);
			}
		}
	}
}
