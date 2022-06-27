using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Weapon")]
public class Weapon : KMonoBehaviour
{
	[MyCmpReq]
	private FactionAlignment alignment;

	public AttackProperties properties;

	public void Configure(float base_damage_min, float base_damage_max, AttackProperties.DamageType attackType = AttackProperties.DamageType.Standard, AttackProperties.TargetType targetType = AttackProperties.TargetType.Single, int maxHits = 1, float aoeRadius = 0f)
	{
		properties = new AttackProperties();
		properties.base_damage_min = base_damage_min;
		properties.base_damage_max = base_damage_max;
		properties.maxHits = maxHits;
		properties.damageType = attackType;
		properties.aoe_radius = aoeRadius;
		properties.attacker = this;
	}

	public void AddEffect(string effectID = "WasAttacked", float probability = 1f)
	{
		if (properties.effects == null)
		{
			properties.effects = new List<AttackEffect>();
		}
		properties.effects.Add(new AttackEffect(effectID, probability));
	}

	public int AttackArea(Vector3 centerPoint)
	{
		Vector3 a = centerPoint;
		Vector3 zero = Vector3.zero;
		alignment = GetComponent<FactionAlignment>();
		if (alignment == null)
		{
			return 0;
		}
		List<GameObject> list = new List<GameObject>();
		foreach (Health item in Components.Health.Items)
		{
			if (item.gameObject == base.gameObject || item.IsDefeated())
			{
				continue;
			}
			FactionAlignment component = item.GetComponent<FactionAlignment>();
			if (!(component == null) && component.IsAlignmentActive() && FactionManager.Instance.GetDisposition(alignment.Alignment, component.Alignment) == FactionManager.Disposition.Attack)
			{
				zero = item.transform.GetPosition();
				zero.z = a.z;
				if (Vector3.Distance(a, zero) <= properties.aoe_radius)
				{
					list.Add(item.gameObject);
				}
			}
		}
		AttackTargets(list.ToArray());
		return list.Count;
	}

	public void AttackTarget(GameObject target)
	{
		AttackTargets(new GameObject[1] { target });
	}

	public void AttackTargets(GameObject[] targets)
	{
		if (properties == null)
		{
			Debug.LogWarning($"Attack properties not configured. {base.gameObject.name} cannot attack with weapon.");
		}
		else
		{
			new Attack(properties, targets);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		properties.attacker = this;
	}
}
