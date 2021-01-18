using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Staterpillar : KMonoBehaviour
{
	[Serialize]
	private Ref<KPrefabID> generatorRef = new Ref<KPrefabID>();

	private AttributeModifier wildMod = new AttributeModifier(Db.Get().Attributes.GeneratorOutput.Id, -75f, BUILDINGS.PREFABS.STATERPILLARGENERATOR.MODIFIERS.WILD);

	private IList<Tag> generatorElement;

	private BuildingDef generatorDef;

	protected override void OnPrefabInit()
	{
		generatorElement = new List<Tag>
		{
			SimHashes.Creature.CreateTag()
		};
		generatorDef = Assets.GetBuildingDef(StaterpillarGeneratorConfig.ID);
		base.OnPrefabInit();
	}

	public void SpawnGenerator(int targetCell)
	{
		KPrefabID kPrefabID = generatorRef.Get();
		GameObject gameObject = null;
		if (kPrefabID != null)
		{
			gameObject = kPrefabID.gameObject;
		}
		if (!gameObject)
		{
			gameObject = generatorDef.Build(targetCell, Orientation.R180, null, generatorElement, base.gameObject.GetComponent<PrimaryElement>().Temperature);
			gameObject.SetActive(value: true);
			generatorRef = new Ref<KPrefabID>(gameObject.GetComponent<KPrefabID>());
			BuildingCellVisualizer component = gameObject.GetComponent<BuildingCellVisualizer>();
			component.enabled = false;
			StaterpillarGenerator component2 = gameObject.GetComponent<StaterpillarGenerator>();
			component2.enabled = false;
		}
		Attributes attributes = gameObject.gameObject.GetAttributes();
		bool flag = base.gameObject.GetSMI<WildnessMonitor.Instance>().wildness.value > 0f;
		if (flag)
		{
			attributes.Add(wildMod);
		}
		bool flag2 = base.gameObject.GetComponent<Effects>().HasEffect("Unhappy");
		CreatureCalorieMonitor.Instance sMI = base.gameObject.GetSMI<CreatureCalorieMonitor.Instance>();
		if (sMI.IsHungry() || flag2)
		{
			float calories0to = sMI.GetCalories0to1();
			float num = 1f;
			if (calories0to <= 0f)
			{
				num = (flag ? 0.1f : 0f);
			}
			else if (calories0to <= 0.3f)
			{
				num = 0.25f;
			}
			else if (calories0to <= 0.6f)
			{
				num = 0.5f;
			}
			else if (calories0to <= 0.9f)
			{
				num = 0.75f;
			}
			if (flag2)
			{
				num *= 0.2f;
			}
			if (num < 1f)
			{
				AttributeModifier modifier = new AttributeModifier(value: 0f - ((!flag) ? ((1f - num) * 100f) : Mathf.Lerp(0f, 25f, 1f - num)), attribute_id: Db.Get().Attributes.GeneratorOutput.Id, description: BUILDINGS.PREFABS.STATERPILLARGENERATOR.MODIFIERS.HUNGRY);
				attributes.Add(modifier);
			}
		}
	}

	public bool IsConnected()
	{
		KPrefabID generator = GetGenerator();
		Generator component = generator.GetComponent<Generator>();
		if (component.CircuitID == ushort.MaxValue && !component.HasWire)
		{
			return false;
		}
		return true;
	}

	public bool IsNotConnected()
	{
		return !IsConnected();
	}

	public void EnableGenerator()
	{
		KPrefabID generator = GetGenerator();
		StaterpillarGenerator component = generator.GetComponent<StaterpillarGenerator>();
		component.enabled = true;
		BuildingCellVisualizer component2 = generator.GetComponent<BuildingCellVisualizer>();
		component2.enabled = true;
	}

	public void DestroyGenerator()
	{
		KPrefabID generator = GetGenerator();
		if (generator != null)
		{
			generatorRef.Set(null);
			GameScheduler.Instance.ScheduleNextFrame("Destroy Staterpillar Generator", delegate
			{
				Util.KDestroyGameObject(generator.gameObject);
			});
		}
	}

	public KPrefabID GetGenerator()
	{
		return (generatorRef.Get() != null) ? generatorRef.Get() : null;
	}
}
