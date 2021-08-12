using Klei.AI;
using UnityEngine;

public class Hit
{
	private AttackProperties properties;

	private GameObject target;

	public Hit(AttackProperties properties, GameObject target)
	{
		this.properties = properties;
		this.target = target;
		DeliverHit();
	}

	private float rollDamage()
	{
		return Mathf.RoundToInt(Random.Range(properties.base_damage_min, properties.base_damage_max));
	}

	private void DeliverHit()
	{
		Health component = target.GetComponent<Health>();
		if (!component)
		{
			return;
		}
		target.Trigger(-787691065, properties.attacker.GetComponent<FactionAlignment>());
		float num = rollDamage();
		AttackableBase component2 = target.GetComponent<AttackableBase>();
		num *= 1f + component2.GetDamageMultiplier();
		component.Damage(num);
		if (properties.effects == null)
		{
			return;
		}
		Effects component3 = target.GetComponent<Effects>();
		if (!component3)
		{
			return;
		}
		foreach (AttackEffect effect in properties.effects)
		{
			if (Random.Range(0f, 100f) < effect.effectProbability * 100f)
			{
				component3.Add(effect.effectID, should_save: true);
			}
		}
	}
}
