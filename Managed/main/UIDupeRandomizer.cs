using System;
using System.Collections.Generic;
using Database;
using UnityEngine;

public class UIDupeRandomizer : MonoBehaviour
{
	[Serializable]
	public struct AnimChoice
	{
		public string anim_name;

		public List<KBatchedAnimController> minions;

		public float minSecondsBetweenAction;

		public float maxSecondsBetweenAction;

		public float lastWaitTime;

		public KAnimFile curBody;
	}

	public KAnimFile head_default_anim;

	public KAnimFile head_swap_anim;

	public KAnimFile body_swap_anim;

	public bool applyHat = true;

	public bool applySuit = true;

	public AnimChoice[] anims;

	private AccessorySlots slots;

	protected virtual void Start()
	{
		slots = new AccessorySlots(null, head_default_anim, head_swap_anim, body_swap_anim);
		for (int i = 0; i < anims.Length; i++)
		{
			anims[i].curBody = null;
			GetNewBody(i);
		}
	}

	protected void GetNewBody(int minion_idx)
	{
		int idx = UnityEngine.Random.Range(0, Db.Get().Personalities.Count);
		Personality personality = Db.Get().Personalities[idx];
		foreach (KBatchedAnimController minion in anims[minion_idx].minions)
		{
			Apply(minion, personality);
		}
	}

	private void Apply(KBatchedAnimController dupe, Personality personality)
	{
		KCompBuilder.BodyData bodyData = MinionStartingStats.CreateBodyData(personality);
		SymbolOverrideController component = dupe.GetComponent<SymbolOverrideController>();
		component.RemoveAllSymbolOverrides();
		AddAccessory(dupe, slots.Hair.Lookup(bodyData.hair));
		AddAccessory(dupe, slots.HatHair.Lookup("hat_" + HashCache.Get().Get(bodyData.hair)));
		AddAccessory(dupe, slots.Eyes.Lookup(bodyData.eyes));
		AddAccessory(dupe, slots.HeadShape.Lookup(bodyData.headShape));
		AddAccessory(dupe, slots.Mouth.Lookup(bodyData.mouth));
		AddAccessory(dupe, slots.Body.Lookup(bodyData.body));
		AddAccessory(dupe, slots.Arm.Lookup(bodyData.arms));
		if (applySuit && UnityEngine.Random.value < 0.15f)
		{
			component.AddBuildOverride(Assets.GetAnim("body_oxygen_kanim").GetData(), 6);
			component.AddBuildOverride(Assets.GetAnim("helm_oxygen_kanim").GetData(), 6);
			dupe.SetSymbolVisiblity("snapto_neck", is_visible: true);
		}
		else
		{
			dupe.SetSymbolVisiblity("snapto_neck", is_visible: false);
		}
		if (applyHat && UnityEngine.Random.value < 0.5f)
		{
			List<string> list = new List<string>();
			foreach (Skill resource in Db.Get().Skills.resources)
			{
				list.Add(resource.hat);
			}
			string id = list[UnityEngine.Random.Range(0, list.Count)];
			AddAccessory(dupe, slots.Hat.Lookup(id));
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, is_visible: false);
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, is_visible: true);
		}
		else
		{
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, is_visible: true);
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, is_visible: false);
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.Hat.targetSymbolId, is_visible: false);
		}
	}

	public static KAnimHashedString AddAccessory(KBatchedAnimController minion, Accessory accessory)
	{
		if (accessory != null)
		{
			SymbolOverrideController component = minion.GetComponent<SymbolOverrideController>();
			DebugUtil.Assert(component != null, minion.name + " is missing symbol override controller");
			component.TryRemoveSymbolOverride(accessory.slot.targetSymbolId);
			component.AddSymbolOverride(accessory.slot.targetSymbolId, accessory.symbol);
			minion.SetSymbolVisiblity(accessory.slot.targetSymbolId, is_visible: true);
			return accessory.slot.targetSymbolId;
		}
		return HashedString.Invalid;
	}

	public KAnimHashedString AddRandomAccessory(KBatchedAnimController minion, List<Accessory> choices)
	{
		Accessory accessory = choices[UnityEngine.Random.Range(1, choices.Count)];
		return AddAccessory(minion, accessory);
	}

	protected virtual void Update()
	{
	}
}
