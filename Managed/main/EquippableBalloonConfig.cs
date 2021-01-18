using System.Collections.Generic;
using Klei.AI;
using TUNING;
using UnityEngine;

public class EquippableBalloonConfig : IEquipmentConfig
{
	public const string ID = "EquippableBalloon";

	private BalloonFX.Instance fx;

	public EquipmentDef CreateEquipmentDef()
	{
		List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("EquippableBalloon", EQUIPMENT.TOYS.SLOT, SimHashes.Carbon, EQUIPMENT.TOYS.BALLOON_MASS, EQUIPMENT.VESTS.COOL_VEST_ICON0, null, null, 0, attributeModifiers, null, IsBody: false, EntityTemplates.CollisionShape.RECTANGLE, 0.75f, 0.4f);
		equipmentDef.OnEquipCallBack = OnEquipBalloon;
		equipmentDef.OnUnequipCallBack = OnUnequipBalloon;
		return equipmentDef;
	}

	private void OnEquipBalloon(Equippable eq)
	{
		if (!(eq != null) || eq.assignee == null)
		{
			return;
		}
		Ownables soleOwner = eq.assignee.GetSoleOwner();
		if (!(soleOwner == null))
		{
			MinionAssignablesProxy component = soleOwner.GetComponent<MinionAssignablesProxy>();
			Effects component2 = (component.target as KMonoBehaviour).GetComponent<Effects>();
			if (component2 != null)
			{
				component2.Add("HasBalloon", should_save: false);
				fx = new BalloonFX.Instance((component.target as KMonoBehaviour).GetComponent<KMonoBehaviour>());
				fx.StartSM();
			}
		}
	}

	private void OnUnequipBalloon(Equippable eq)
	{
		if (eq != null && eq.assignee != null)
		{
			Ownables soleOwner = eq.assignee.GetSoleOwner();
			if (soleOwner == null)
			{
				return;
			}
			MinionAssignablesProxy component = soleOwner.GetComponent<MinionAssignablesProxy>();
			if (component.target != null)
			{
				Effects component2 = (component.target as KMonoBehaviour).GetComponent<Effects>();
				if (component2 != null)
				{
					component2.Remove("HasBalloon");
				}
			}
		}
		if (fx != null)
		{
			fx.StopSM("Unequipped");
		}
		Util.KDestroyGameObject(eq.gameObject);
	}

	public void DoPostConfigure(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddTag(GameTags.Clothes);
		Equippable equippable = go.GetComponent<Equippable>();
		if (equippable == null)
		{
			equippable = go.AddComponent<Equippable>();
		}
		equippable.hideInCodex = true;
		equippable.unequippable = false;
		go.AddOrGet<EquippableBalloon>();
	}
}
