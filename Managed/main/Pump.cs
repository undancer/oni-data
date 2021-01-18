using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Pump")]
public class Pump : KMonoBehaviour, ISim1000ms
{
	public static readonly Operational.Flag PumpableFlag = new Operational.Flag("vent", Operational.Flag.Type.Requirement);

	[MyCmpReq]
	private Operational operational;

	[MyCmpGet]
	private KSelectable selectable;

	[MyCmpGet]
	private ElementConsumer consumer;

	[MyCmpGet]
	private ConduitDispenser dispenser;

	[MyCmpGet]
	private Storage storage;

	private const float OperationalUpdateInterval = 1f;

	private float elapsedTime = 0f;

	private bool pumpable = false;

	private Guid conduitBlockedStatusGuid;

	private Guid noElementStatusGuid;

	public ConduitType conduitType => dispenser.conduitType;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		consumer.EnableConsumption(enabled: false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		elapsedTime = 0f;
		pumpable = UpdateOperational();
		dispenser.GetConduitManager().AddConduitUpdater(OnConduitUpdate, ConduitFlowPriority.LastPostUpdate);
	}

	protected override void OnCleanUp()
	{
		dispenser.GetConduitManager().RemoveConduitUpdater(OnConduitUpdate);
		base.OnCleanUp();
	}

	public void Sim1000ms(float dt)
	{
		elapsedTime += dt;
		if (elapsedTime >= 1f)
		{
			pumpable = UpdateOperational();
			elapsedTime = 0f;
		}
		if (operational.IsOperational && pumpable)
		{
			operational.SetActive(value: true);
		}
		else
		{
			operational.SetActive(value: false);
		}
	}

	private bool UpdateOperational()
	{
		Element.State state = Element.State.Vacuum;
		switch (dispenser.conduitType)
		{
		case ConduitType.Gas:
			state = Element.State.Gas;
			break;
		case ConduitType.Liquid:
			state = Element.State.Liquid;
			break;
		}
		bool flag = IsPumpable(state, consumer.consumptionRadius);
		StatusItem status_item = ((state == Element.State.Gas) ? Db.Get().BuildingStatusItems.NoGasElementToPump : Db.Get().BuildingStatusItems.NoLiquidElementToPump);
		noElementStatusGuid = selectable.ToggleStatusItem(status_item, noElementStatusGuid, !flag);
		operational.SetFlag(PumpableFlag, !storage.IsFull() && flag);
		return flag;
	}

	private bool IsPumpable(Element.State expected_state, int radius)
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		for (int i = 0; i < consumer.consumptionRadius; i++)
		{
			for (int j = 0; j < consumer.consumptionRadius; j++)
			{
				int num2 = num + j + Grid.WidthInCells * i;
				if (Grid.Element[num2].IsState(expected_state))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void OnConduitUpdate(float dt)
	{
		conduitBlockedStatusGuid = selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.ConduitBlocked, conduitBlockedStatusGuid, dispenser.blocked);
	}
}
