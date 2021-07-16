using System;
using System.Collections.Generic;
using Klei.AI;

public class EquipmentDef : Def
{
	public string Id;

	public string Slot;

	public string FabricatorId;

	public float FabricationTime;

	public string RecipeTechUnlock;

	public SimHashes OutputElement;

	public Dictionary<string, float> InputElementMassMap;

	public float Mass;

	public KAnimFile Anim;

	public string SnapOn;

	public string SnapOn1;

	public KAnimFile BuildOverride;

	public int BuildOverridePriority;

	public bool IsBody;

	public List<AttributeModifier> AttributeModifiers;

	public string RecipeDescription;

	public List<Effect> EffectImmunites = new List<Effect>();

	public Action<Equippable> OnEquipCallBack;

	public Action<Equippable> OnUnequipCallBack;

	public EntityTemplates.CollisionShape CollisionShape;

	public float width;

	public float height = 0.325f;

	public Tag[] AdditionalTags;

	public string wornID;

	public List<Descriptor> additionalDescriptors = new List<Descriptor>();

	public override string Name => Strings.Get("STRINGS.EQUIPMENT.PREFABS." + Id.ToUpper() + ".NAME");

	public string GenericName => Strings.Get("STRINGS.EQUIPMENT.PREFABS." + Id.ToUpper() + ".GENERICNAME");

	public string WornName => Strings.Get("STRINGS.EQUIPMENT.PREFABS." + Id.ToUpper() + ".WORN_NAME");

	public string WornDesc => Strings.Get("STRINGS.EQUIPMENT.PREFABS." + Id.ToUpper() + ".WORN_DESC");
}
