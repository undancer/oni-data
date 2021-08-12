using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicElementSensor : Switch, ISaveLoadable, ISim200ms
{
	private bool wasOn;

	public Element.State desiredState = Element.State.Gas;

	private const int WINDOW_SIZE = 8;

	private bool[] samples = new bool[8];

	private int sampleIdx;

	private byte desiredElementIdx = byte.MaxValue;

	private static readonly EventSystem.IntraObjectHandler<LogicElementSensor> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<LogicElementSensor>(delegate(LogicElementSensor component, object data)
	{
		component.OnOperationalChanged(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GetComponent<Filterable>().onFilterChanged += OnElementSelected;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += OnSwitchToggled;
		UpdateLogicCircuit();
		UpdateVisualState(force: true);
		wasOn = switchedOn;
		Subscribe(-592767678, OnOperationalChangedDelegate);
	}

	public void Sim200ms(float dt)
	{
		int i = Grid.PosToCell(this);
		if (sampleIdx < 8)
		{
			samples[sampleIdx] = Grid.ElementIdx[i] == desiredElementIdx;
			sampleIdx++;
			return;
		}
		sampleIdx = 0;
		bool flag = true;
		bool[] array = samples;
		for (int j = 0; j < array.Length; j++)
		{
			flag = array[j] && flag;
		}
		if (base.IsSwitchedOn != flag)
		{
			Toggle();
		}
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		UpdateLogicCircuit();
		UpdateVisualState();
	}

	private void UpdateLogicCircuit()
	{
		bool flag = switchedOn && GetComponent<Operational>().IsOperational;
		GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, flag ? 1 : 0);
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

	private void OnElementSelected(Tag element_tag)
	{
		if (element_tag.IsValid)
		{
			Element element = ElementLoader.GetElement(element_tag);
			bool on = true;
			if (element != null)
			{
				desiredElementIdx = (byte)ElementLoader.GetElementIndex(element.id);
				on = element.id == SimHashes.Void || element.id == SimHashes.Vacuum;
			}
			GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, on);
		}
	}

	private void OnOperationalChanged(object data)
	{
		UpdateLogicCircuit();
		UpdateVisualState();
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem status_item = (switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive);
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
	}
}
