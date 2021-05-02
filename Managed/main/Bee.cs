using System.Collections.Generic;

public class Bee : KMonoBehaviour
{
	public float radiationAmount;

	private static readonly EventSystem.IntraObjectHandler<Bee> OnAttackDelegate = new EventSystem.IntraObjectHandler<Bee>(delegate(Bee component, object data)
	{
		component.OnAttack(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Bee> OnSleepDelegate = new EventSystem.IntraObjectHandler<Bee>(delegate(Bee component, object data)
	{
		component.StartSleep(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Bee> OnWakeUpDelegate = new EventSystem.IntraObjectHandler<Bee>(delegate(Bee component, object data)
	{
		component.StopSleep(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Bee> OnDeathDelegate = new EventSystem.IntraObjectHandler<Bee>(delegate(Bee component, object data)
	{
		component.OnDeath(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-739654666, OnAttackDelegate);
		Subscribe(-1283701846, OnSleepDelegate);
		Subscribe(-2090444759, OnWakeUpDelegate);
		Subscribe(1623392196, OnDeathDelegate);
		GetComponent<KBatchedAnimController>().SetSymbolVisiblity("tag", is_visible: false);
		GetComponent<KBatchedAnimController>().SetSymbolVisiblity("snapto_tag", is_visible: false);
	}

	private void OnDeath(object data)
	{
		PrimaryElement component = GetComponent<PrimaryElement>();
		Storage component2 = GetComponent<Storage>();
		byte index = Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.id);
		component2.AddOre(SimHashes.NuclearWaste, BeeTuning.WASTE_DROPPED_ON_DEATH, component.Temperature, index, BeeTuning.GERMS_DROPPED_ON_DEATH);
		component2.DropAll(base.transform.position, vent_gas: true, dump_liquid: true);
	}

	private void TurnOffRadiation(object data)
	{
		RadiationEmitter component = GetComponent<RadiationEmitter>();
		component.emitRads = 0f;
		component.Refresh();
	}

	private void TurnOnRadiation(object data)
	{
		RadiationEmitter component = GetComponent<RadiationEmitter>();
		component.emitRads = radiationAmount;
		component.Refresh();
	}

	private void StartSleep(object data)
	{
		TurnOnRadiation(data);
		GetComponent<ElementConsumer>().EnableConsumption(enabled: true);
	}

	private void StopSleep(object data)
	{
		TurnOffRadiation(data);
		GetComponent<ElementConsumer>().EnableConsumption(enabled: false);
	}

	private void OnAttack(object data)
	{
		Tag a = (Tag)data;
		if (a == GameTags.Creatures.Attack)
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
