using Database;
using UnityEngine;

[RequireComponent(typeof(SymbolOverrideController))]
public class UIDupeSymbolOverride : MonoBehaviour
{
	public KAnimFile head_default_anim;

	public KAnimFile head_swap_anim;

	public KAnimFile body_swap_anim;

	private KBatchedAnimController animController;

	private AccessorySlots slots;

	private SymbolOverrideController symbolOverrideController;

	public void Apply(MinionIdentity minionIdentity)
	{
		if (slots == null)
		{
			slots = new AccessorySlots(null, head_default_anim, head_swap_anim, body_swap_anim);
		}
		if (symbolOverrideController == null)
		{
			symbolOverrideController = GetComponent<SymbolOverrideController>();
		}
		if (animController == null)
		{
			animController = GetComponent<KBatchedAnimController>();
		}
		Personality personality = null;
		foreach (Personality resource in Db.Get().Personalities.resources)
		{
			if (resource.Name.ToUpper() == minionIdentity.nameStringKey)
			{
				personality = resource;
				break;
			}
		}
		DebugUtil.DevAssert(personality != null, "Personality is not found");
		KCompBuilder.BodyData bodyData = MinionStartingStats.CreateBodyData(personality);
		symbolOverrideController.RemoveAllSymbolOverrides();
		SetAccessory(animController, slots.Hair.Lookup(bodyData.hair));
		SetAccessory(animController, slots.HatHair.Lookup("hat_" + HashCache.Get().Get(bodyData.hair)));
		SetAccessory(animController, slots.Eyes.Lookup(bodyData.eyes));
		SetAccessory(animController, slots.HeadShape.Lookup(bodyData.headShape));
		SetAccessory(animController, slots.Mouth.Lookup(bodyData.mouth));
		SetAccessory(animController, slots.Body.Lookup(bodyData.body));
		SetAccessory(animController, slots.Arm.Lookup(bodyData.arms));
	}

	private KAnimHashedString SetAccessory(KBatchedAnimController minion, Accessory accessory)
	{
		if (accessory != null)
		{
			symbolOverrideController.TryRemoveSymbolOverride(accessory.slot.targetSymbolId);
			symbolOverrideController.AddSymbolOverride(accessory.slot.targetSymbolId, accessory.symbol);
			minion.SetSymbolVisiblity(accessory.slot.targetSymbolId, is_visible: true);
			return accessory.slot.targetSymbolId;
		}
		return HashedString.Invalid;
	}
}
