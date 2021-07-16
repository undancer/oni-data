using System.Collections.Generic;

public class Bee : KMonoBehaviour
{
	public float radiationOutputAmount;

	private Dictionary<HashedString, float> radiationModifiers = new Dictionary<HashedString, float>();

	private float unhappyRadiationMod = 0.1f;

	private float awakeRadiationMod;

	private HashedString unhappyRadiationModKey = "UNHAPPY";

	private HashedString awakeRadiationModKey = "AWAKE";

	private static readonly EventSystem.IntraObjectHandler<Bee> OnAttackDelegate = new EventSystem.IntraObjectHandler<Bee>(delegate(Bee component, object data)
	{
		component.OnAttack(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Bee> OnSleepDelegate = new EventSystem.IntraObjectHandler<Bee>(delegate(Bee component, object data)
	{
		component.StartSleep();
	});

	private static readonly EventSystem.IntraObjectHandler<Bee> OnWakeUpDelegate = new EventSystem.IntraObjectHandler<Bee>(delegate(Bee component, object data)
	{
		component.StopSleep();
	});

	private static readonly EventSystem.IntraObjectHandler<Bee> OnDeathDelegate = new EventSystem.IntraObjectHandler<Bee>(delegate(Bee component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Bee> OnHappyDelegate = new EventSystem.IntraObjectHandler<Bee>(delegate(Bee component, object data)
	{
		component.RemoveRadiationMod(component.unhappyRadiationModKey);
	});

	private static readonly EventSystem.IntraObjectHandler<Bee> OnUnhappyDelegate = new EventSystem.IntraObjectHandler<Bee>(delegate(Bee component, object data)
	{
		component.AddRadiationModifier(component.unhappyRadiationModKey, component.unhappyRadiationMod);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-739654666, OnAttackDelegate);
		Subscribe(-1283701846, OnSleepDelegate);
		Subscribe(-2090444759, OnWakeUpDelegate);
		Subscribe(1623392196, OnDeathDelegate);
		Subscribe(1890751808, OnHappyDelegate);
		Subscribe(-647798969, OnUnhappyDelegate);
		GetComponent<KBatchedAnimController>().SetSymbolVisiblity("tag", is_visible: false);
		GetComponent<KBatchedAnimController>().SetSymbolVisiblity("snapto_tag", is_visible: false);
		StopSleep();
	}

	private void OnDeath(object data)
	{
		PrimaryElement component = GetComponent<PrimaryElement>();
		Storage component2 = GetComponent<Storage>();
		component2.AddOre(disease_idx: Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.id), element: SimHashes.NuclearWaste, mass: BeeTuning.WASTE_DROPPED_ON_DEATH, temperature: component.Temperature, disease_count: BeeTuning.GERMS_DROPPED_ON_DEATH);
		component2.DropAll(base.transform.position, vent_gas: true, dump_liquid: true);
	}

	private void StartSleep()
	{
		RemoveRadiationMod(awakeRadiationModKey);
		GetComponent<ElementConsumer>().EnableConsumption(enabled: true);
	}

	private void StopSleep()
	{
		AddRadiationModifier(awakeRadiationModKey, awakeRadiationMod);
		GetComponent<ElementConsumer>().EnableConsumption(enabled: false);
	}

	private void AddRadiationModifier(HashedString name, float mod)
	{
		radiationModifiers.Add(name, mod);
		RefreshRadiationOutput();
	}

	private void RemoveRadiationMod(HashedString name)
	{
		radiationModifiers.Remove(name);
		RefreshRadiationOutput();
	}

	public void RefreshRadiationOutput()
	{
		float num = radiationOutputAmount;
		foreach (KeyValuePair<HashedString, float> radiationModifier in radiationModifiers)
		{
			num *= radiationModifier.Value;
		}
		RadiationEmitter component = GetComponent<RadiationEmitter>();
		component.emitRads = num;
		component.Refresh();
	}

	private void OnAttack(object data)
	{
		if ((Tag)data == GameTags.Creatures.Attack)
		{
			GetComponent<Health>().Damage(GetComponent<Health>().hitPoints);
		}
	}

	public KPrefabID FindHiveInRoom()
	{
		List<BeeHive.StatesInstance> list = new List<BeeHive.StatesInstance>();
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		foreach (BeeHive.StatesInstance item in Components.BeeHives.Items)
		{
			if (Game.Instance.roomProber.GetRoomOfGameObject(item.gameObject) == roomOfGameObject)
			{
				list.Add(item);
			}
		}
		int num = int.MaxValue;
		KPrefabID result = null;
		foreach (BeeHive.StatesInstance item2 in list)
		{
			int navigationCost = base.gameObject.GetComponent<Navigator>().GetNavigationCost(Grid.PosToCell(item2.transform.GetLocalPosition()));
			if (navigationCost < num)
			{
				num = navigationCost;
				result = item2.GetComponent<KPrefabID>();
			}
		}
		return result;
	}
}
