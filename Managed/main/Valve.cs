using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/Valve")]
public class Valve : Workable, ISaveLoadable
{
	[MyCmpReq]
	private ValveBase valveBase;

	[Serialize]
	private float desiredFlow = 0.5f;

	private Chore chore = null;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<Valve> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Valve>(delegate(Valve component, object data)
	{
		component.OnCopySettings(data);
	});

	public float QueuedMaxFlow => (chore != null) ? desiredFlow : (-1f);

	public float DesiredFlow => desiredFlow;

	public float MaxFlow => valveBase.MaxFlow;

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		Valve component = gameObject.GetComponent<Valve>();
		if (component != null)
		{
			ChangeFlow(component.desiredFlow);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
		synchronizeAnims = false;
		valveBase.CurrentFlow = valveBase.MaxFlow;
		desiredFlow = valveBase.MaxFlow;
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		ChangeFlow(desiredFlow);
		base.OnSpawn();
		Prioritizable.AddRef(base.gameObject);
	}

	protected override void OnCleanUp()
	{
		Prioritizable.RemoveRef(base.gameObject);
		base.OnCleanUp();
	}

	public void ChangeFlow(float amount)
	{
		desiredFlow = Mathf.Clamp(amount, 0f, valveBase.MaxFlow);
		KSelectable component = GetComponent<KSelectable>();
		component.ToggleStatusItem(Db.Get().BuildingStatusItems.PumpingLiquidOrGas, desiredFlow >= 0f, valveBase.AccumulatorHandle);
		if (DebugHandler.InstantBuildMode)
		{
			UpdateFlow();
			return;
		}
		if (desiredFlow != valveBase.CurrentFlow)
		{
			if (chore == null)
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.ValveRequest, this);
				component.AddStatusItem(Db.Get().BuildingStatusItems.PendingWork, this);
				chore = new WorkChore<Valve>(Db.Get().ChoreTypes.Toggle, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false);
			}
			return;
		}
		if (chore != null)
		{
			chore.Cancel("desiredFlow == currentFlow");
			chore = null;
		}
		component.RemoveStatusItem(Db.Get().BuildingStatusItems.ValveRequest);
		component.RemoveStatusItem(Db.Get().BuildingStatusItems.PendingWork);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		UpdateFlow();
	}

	public void UpdateFlow()
	{
		valveBase.CurrentFlow = desiredFlow;
		valveBase.UpdateAnim();
		if (chore != null)
		{
			chore.Cancel("forced complete");
		}
		chore = null;
		KSelectable component = GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().BuildingStatusItems.ValveRequest);
		component.RemoveStatusItem(Db.Get().BuildingStatusItems.PendingWork);
	}
}
