using System.Collections.Generic;

public class Bee : KMonoBehaviour
{
	public float radiationAmount;

	private static readonly EventSystem.IntraObjectHandler<Bee> OnSpawnedFromLarvaDelegate = new EventSystem.IntraObjectHandler<Bee>(delegate(Bee component, object data)
	{
		component.OnSpawnedFromLarva(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Bee> EnterHiveDelegate = new EventSystem.IntraObjectHandler<Bee>(delegate(Bee component, object data)
	{
		component.TurnOffRadiation(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Bee> ExitHiveDelegate = new EventSystem.IntraObjectHandler<Bee>(delegate(Bee component, object data)
	{
		component.TurnOnRadiation(data);
	});

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

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-2027483228, OnSpawnedFromLarvaDelegate);
		Subscribe(-2099923209, EnterHiveDelegate);
		Subscribe(-1220248099, ExitHiveDelegate);
		Subscribe(-739654666, OnAttackDelegate);
		Subscribe(-1283701846, OnSleepDelegate);
		Subscribe(-2090444759, OnWakeUpDelegate);
	}

	protected override void OnCleanUp()
	{
	}

	private void OnSpawnedFromLarva(object data)
	{
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
		List<BeeHive> list = new List<BeeHive>();
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		foreach (BeeHive item in Components.BeeHives.Items)
		{
			if (Game.Instance.roomProber.GetRoomOfGameObject(item.gameObject) == roomOfGameObject)
			{
				list.Add(item);
			}
		}
		int num = int.MaxValue;
		KPrefabID result = null;
		foreach (BeeHive item2 in list)
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
