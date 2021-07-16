using System;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RoverModifiers : Modifiers, ISaveLoadable
{
	private static readonly EventSystem.IntraObjectHandler<RoverModifiers> OnBeginChoreDelegate = new EventSystem.IntraObjectHandler<RoverModifiers>(delegate(RoverModifiers component, object data)
	{
		component.OnBeginChore(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		attributes.Add(Db.Get().Attributes.Construction);
		attributes.Add(Db.Get().Attributes.Digging);
		attributes.Add(Db.Get().Attributes.Strength);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (GetComponent<ChoreConsumer>() != null)
		{
			Subscribe(-1988963660, OnBeginChoreDelegate);
			Vector3 position = base.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
			base.transform.SetPosition(position);
			base.gameObject.layer = LayerMask.NameToLayer("Default");
			SetupDependentAttribute(Db.Get().Attributes.CarryAmount, Db.Get().AttributeConverters.CarryAmountFromStrength);
		}
	}

	private void SetupDependentAttribute(Klei.AI.Attribute targetAttribute, AttributeConverter attributeConverter)
	{
		Klei.AI.Attribute attribute = attributeConverter.attribute;
		AttributeInstance attributeInstance = attribute.Lookup(this);
		AttributeModifier target_modifier = new AttributeModifier(targetAttribute.Id, attributeConverter.Lookup(this).Evaluate(), attribute.Name, is_multiplier: false, uiOnly: false, is_readonly: false);
		this.GetAttributes().Add(target_modifier);
		attributeInstance.OnDirty = (System.Action)Delegate.Combine(attributeInstance.OnDirty, (System.Action)delegate
		{
			target_modifier.SetValue(attributeConverter.Lookup(this).Evaluate());
		});
	}

	private void OnBeginChore(object data)
	{
		Storage component = GetComponent<Storage>();
		if (component != null)
		{
			component.DropAll();
		}
	}
}
