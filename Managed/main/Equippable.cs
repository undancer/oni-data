using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Equippable : Assignable, ISaveLoadable, IGameObjectEffectDescriptor, IQuality
{
	private QualityLevel quality;

	[MyCmpAdd]
	private EquippableWorkable equippableWorkable;

	[MyCmpAdd]
	private EquippableFacade facade;

	[MyCmpReq]
	private KSelectable selectable;

	public DefHandle defHandle;

	[Serialize]
	public bool isEquipped;

	private bool destroyed;

	[Serialize]
	public bool unequippable = true;

	[Serialize]
	public bool hideInCodex;

	private static readonly EventSystem.IntraObjectHandler<Equippable> SetDestroyedTrueDelegate = new EventSystem.IntraObjectHandler<Equippable>(delegate(Equippable component, object data)
	{
		component.destroyed = true;
	});

	public EquipmentDef def
	{
		get
		{
			return defHandle.Get<EquipmentDef>();
		}
		set
		{
			defHandle.Set(value);
		}
	}

	public QualityLevel GetQuality()
	{
		return quality;
	}

	public void SetQuality(QualityLevel level)
	{
		quality = level;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (def.AdditionalTags != null)
		{
			Tag[] additionalTags = def.AdditionalTags;
			foreach (Tag tag in additionalTags)
			{
				GetComponent<KPrefabID>().AddTag(tag);
			}
		}
	}

	protected override void OnSpawn()
	{
		if (isEquipped)
		{
			if (assignee != null && assignee is MinionIdentity)
			{
				assignee = (assignee as MinionIdentity).assignableProxy.Get();
				assignee_identityRef.Set(assignee as KMonoBehaviour);
			}
			if (assignee == null && assignee_identityRef.Get() != null)
			{
				assignee = assignee_identityRef.Get().GetComponent<IAssignableIdentity>();
			}
			if (assignee != null)
			{
				assignee.GetSoleOwner().GetComponent<Equipment>().Equip(this);
			}
			else
			{
				Debug.LogWarning("Equippable trying to be equipped to missing prefab");
				isEquipped = false;
			}
		}
		Subscribe(1969584890, SetDestroyedTrueDelegate);
	}

	public KAnimFile GetBuildOverride()
	{
		EquippableFacade component = GetComponent<EquippableFacade>();
		if (component == null || component.BuildOverride == null)
		{
			return def.BuildOverride;
		}
		return Assets.GetAnim(component.BuildOverride);
	}

	public override void Assign(IAssignableIdentity new_assignee)
	{
		if (new_assignee == assignee)
		{
			return;
		}
		if (base.slot != null && new_assignee is MinionIdentity)
		{
			new_assignee = (new_assignee as MinionIdentity).assignableProxy.Get();
		}
		if (base.slot != null && new_assignee is StoredMinionIdentity)
		{
			new_assignee = (new_assignee as StoredMinionIdentity).assignableProxy.Get();
		}
		if (new_assignee is MinionAssignablesProxy)
		{
			AssignableSlotInstance assignableSlotInstance = new_assignee.GetSoleOwner().GetComponent<Equipment>().GetSlot(base.slot);
			if (assignableSlotInstance != null)
			{
				Assignable assignable = assignableSlotInstance.assignable;
				if (assignable != null)
				{
					assignable.Unassign();
				}
			}
		}
		base.Assign(new_assignee);
	}

	public override void Unassign()
	{
		if (isEquipped)
		{
			((assignee is MinionIdentity) ? ((MinionIdentity)assignee).assignableProxy.Get().GetComponent<Equipment>() : ((KMonoBehaviour)assignee).GetComponent<Equipment>()).Unequip(this);
			OnUnequip();
		}
		base.Unassign();
	}

	public void OnEquip(AssignableSlotInstance slot)
	{
		isEquipped = true;
		if (SelectTool.Instance.selected == selectable)
		{
			SelectTool.Instance.Select(null);
		}
		GetComponent<KBatchedAnimController>().enabled = false;
		GetComponent<KSelectable>().IsSelectable = false;
		Effects component = slot.gameObject.GetComponent<MinionAssignablesProxy>().GetTargetGameObject().GetComponent<Effects>();
		if (component != null)
		{
			foreach (Effect effectImmunite in def.EffectImmunites)
			{
				component.AddImmunity(effectImmunite);
			}
		}
		if (def.OnEquipCallBack != null)
		{
			def.OnEquipCallBack(this);
		}
		GetComponent<KPrefabID>().AddTag(GameTags.Equipped);
	}

	public void OnUnequip()
	{
		isEquipped = false;
		if (destroyed)
		{
			return;
		}
		GetComponent<KPrefabID>().RemoveTag(GameTags.Equipped);
		GetComponent<KBatchedAnimController>().enabled = true;
		GetComponent<KSelectable>().IsSelectable = true;
		if (assignee != null)
		{
			Ownables soleOwner = assignee.GetSoleOwner();
			if ((bool)soleOwner)
			{
				GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				if ((bool)targetGameObject)
				{
					Effects component = targetGameObject.GetComponent<Effects>();
					if (component != null)
					{
						foreach (Effect effectImmunite in def.EffectImmunites)
						{
							component.RemoveImmunity(effectImmunite);
						}
					}
				}
			}
		}
		if (def.OnUnequipCallBack != null)
		{
			def.OnUnequipCallBack(this);
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		if (def != null)
		{
			List<Descriptor> equipmentEffects = GameUtil.GetEquipmentEffects(def);
			if (def.additionalDescriptors != null)
			{
				foreach (Descriptor additionalDescriptor in def.additionalDescriptors)
				{
					equipmentEffects.Add(additionalDescriptor);
				}
				return equipmentEffects;
			}
			return equipmentEffects;
		}
		return new List<Descriptor>();
	}
}
