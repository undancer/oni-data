using UnityEngine;

public class FullBodyUIMinionWidget : KMonoBehaviour
{
	[SerializeField]
	private GameObject duplicantAnimAnchor;

	private KBatchedAnimController animController;

	public const float UI_MINION_PORTRAIT_ANIM_SCALE = 0.38f;

	private Tuple<KAnimFileData, int> buildOverrideData;

	protected override void OnSpawn()
	{
		TrySpawnDisplayMinion();
	}

	private void TrySpawnDisplayMinion()
	{
		if (animController == null)
		{
			animController = Util.KInstantiateUI(Assets.GetPrefab(new Tag("FullMinionUIPortrait")), duplicantAnimAnchor.gameObject).GetComponent<KBatchedAnimController>();
			animController.gameObject.SetActive(value: true);
			animController.animScale = 0.38f;
		}
	}

	public void SetPortraitAnimator(IAssignableIdentity assignableIdentity)
	{
		if (assignableIdentity == null || assignableIdentity.IsNull())
		{
			if (Components.MinionIdentities.Count <= 0)
			{
				return;
			}
			assignableIdentity = Components.MinionIdentities[0];
			if (assignableIdentity == null || assignableIdentity.IsNull())
			{
				return;
			}
		}
		TrySpawnDisplayMinion();
		string value = "";
		Accessorizer component = animController.GetComponent<Accessorizer>();
		for (int num = component.GetAccessories().Count - 1; num >= 0; num--)
		{
			component.RemoveAccessory(component.GetAccessories()[num].Get());
		}
		GetMinionIdentity(assignableIdentity, out var minionIdentity, out var storedMinionIdentity);
		Accessorizer accessorizer = null;
		if (minionIdentity != null)
		{
			accessorizer = minionIdentity.GetComponent<Accessorizer>();
			foreach (ResourceRef<Accessory> accessory in accessorizer.GetAccessories())
			{
				component.AddAccessory(accessory.Get());
			}
			value = minionIdentity.GetComponent<MinionResume>().CurrentHat;
		}
		else if (storedMinionIdentity != null)
		{
			foreach (ResourceRef<Accessory> accessory2 in storedMinionIdentity.accessories)
			{
				component.AddAccessory(accessory2.Get());
			}
			value = storedMinionIdentity.currentHat;
		}
		animController.Queue("idle_default", KAnim.PlayMode.Loop);
		AccessorySlot hat = Db.Get().AccessorySlots.Hat;
		animController.SetSymbolVisiblity(hat.targetSymbolId, (!string.IsNullOrEmpty(value)) ? true : false);
		animController.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, string.IsNullOrEmpty(value) ? true : false);
		animController.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, (!string.IsNullOrEmpty(value)) ? true : false);
		KAnim.Build.Symbol source_symbol = null;
		KAnim.Build.Symbol source_symbol2 = null;
		if ((bool)accessorizer)
		{
			source_symbol = accessorizer.GetAccessory(Db.Get().AccessorySlots.Hair).symbol;
			source_symbol2 = Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(accessorizer.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol;
		}
		else if (storedMinionIdentity != null)
		{
			source_symbol = storedMinionIdentity.GetAccessory(Db.Get().AccessorySlots.Hair).symbol;
			source_symbol2 = Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(storedMinionIdentity.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol;
		}
		SymbolOverrideController component2 = animController.GetComponent<SymbolOverrideController>();
		component2.AddSymbolOverride(Db.Get().AccessorySlots.HairAlways.targetSymbolId, source_symbol, 1);
		component2.AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, source_symbol2, 1);
		UpdateClothingOverride(component2, minionIdentity, storedMinionIdentity);
	}

	private void UpdateClothingOverride(SymbolOverrideController symbolOverrideController, MinionIdentity identity, StoredMinionIdentity storedMinionIdentity)
	{
		Equipment equipment = null;
		if (identity != null)
		{
			equipment = identity.assignableProxy.Get().GetComponent<Equipment>();
		}
		else if (storedMinionIdentity != null)
		{
			equipment = storedMinionIdentity.assignableProxy.Get().GetComponent<Equipment>();
		}
		if (buildOverrideData != null)
		{
			symbolOverrideController.RemoveBuildOverride(buildOverrideData.first, buildOverrideData.second);
			buildOverrideData = null;
		}
		AssignableSlotInstance slot = equipment.GetSlot(Db.Get().AssignableSlots.Outfit);
		if (slot.assignable != null)
		{
			Equippable component = slot.assignable.GetComponent<Equippable>();
			if (component != null)
			{
				buildOverrideData = new Tuple<KAnimFileData, int>(component.GetBuildOverride().GetData(), component.def.BuildOverridePriority);
				symbolOverrideController.AddBuildOverride(buildOverrideData.first, component.def.BuildOverridePriority);
			}
		}
	}

	public void UpdateClothingOverride(KAnimFileData clothingData)
	{
		SymbolOverrideController component = animController.GetComponent<SymbolOverrideController>();
		if (buildOverrideData != null)
		{
			component.RemoveBuildOverride(buildOverrideData.first, buildOverrideData.second);
			buildOverrideData = null;
		}
		buildOverrideData = new Tuple<KAnimFileData, int>(clothingData, 4);
		component.AddBuildOverride(buildOverrideData.first, buildOverrideData.second);
	}

	private void GetMinionIdentity(IAssignableIdentity assignableIdentity, out MinionIdentity minionIdentity, out StoredMinionIdentity storedMinionIdentity)
	{
		if (assignableIdentity is MinionAssignablesProxy)
		{
			minionIdentity = ((MinionAssignablesProxy)assignableIdentity).GetTargetGameObject().GetComponent<MinionIdentity>();
			storedMinionIdentity = ((MinionAssignablesProxy)assignableIdentity).GetTargetGameObject().GetComponent<StoredMinionIdentity>();
		}
		else
		{
			minionIdentity = assignableIdentity as MinionIdentity;
			storedMinionIdentity = assignableIdentity as StoredMinionIdentity;
		}
	}
}
