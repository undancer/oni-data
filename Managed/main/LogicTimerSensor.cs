using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicTimerSensor : Switch, ISaveLoadable, ISim33ms
{
	[Serialize]
	public float onDuration = 10f;

	[Serialize]
	public float offDuration = 10f;

	[Serialize]
	public bool displayCyclesMode;

	private bool wasOn;

	[SerializeField]
	[Serialize]
	public float timeElapsedInCurrentState;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicTimerSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicTimerSensor>(delegate(LogicTimerSensor component, object data)
	{
		component.OnCopySettings(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		LogicTimerSensor component = ((GameObject)data).GetComponent<LogicTimerSensor>();
		if (component != null)
		{
			onDuration = component.onDuration;
			offDuration = component.offDuration;
			timeElapsedInCurrentState = component.timeElapsedInCurrentState;
			displayCyclesMode = component.displayCyclesMode;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += OnSwitchToggled;
		UpdateLogicCircuit();
		UpdateVisualState(force: true);
		wasOn = switchedOn;
	}

	public void Sim33ms(float dt)
	{
		if (onDuration == 0f && offDuration == 0f)
		{
			return;
		}
		timeElapsedInCurrentState += dt;
		bool flag = base.IsSwitchedOn;
		if (flag)
		{
			if (timeElapsedInCurrentState >= onDuration)
			{
				flag = false;
				timeElapsedInCurrentState -= onDuration;
			}
		}
		else if (timeElapsedInCurrentState >= offDuration)
		{
			flag = true;
			timeElapsedInCurrentState -= offDuration;
		}
		SetState(flag);
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		UpdateLogicCircuit();
		UpdateVisualState();
	}

	private void UpdateLogicCircuit()
	{
		GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, switchedOn ? 1 : 0);
	}

	private void UpdateVisualState(bool force = false)
	{
		if (wasOn != switchedOn || force)
		{
			wasOn = switchedOn;
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			component.Play(switchedOn ? "on_pre" : "on_pst");
			component.Queue(switchedOn ? "on" : "off");
		}
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem status_item = (switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive);
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
	}

	public void ResetTimer()
	{
		SetState(on: true);
		OnSwitchToggled(toggled_on: true);
		timeElapsedInCurrentState = 0f;
	}
}
