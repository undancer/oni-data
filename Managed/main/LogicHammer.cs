using FMOD.Studio;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicHammer : Switch
{
	protected KBatchedAnimController animController;

	private static readonly EventSystem.IntraObjectHandler<LogicHammer> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicHammer>(delegate(LogicHammer component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<LogicHammer> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<LogicHammer>(delegate(LogicHammer component, object data)
	{
		component.OnOperationalChanged(data);
	});

	public static readonly HashedString PORT_ID = new HashedString("LogicHammerInput");

	private static string PARAMETER_NAME = "hammerObjectCount";

	private static string SOUND_EVENT_PREFIX = "Hammer_strike_";

	private static string DEFAULT_NO_SOUND_EVENT = "Hammer_strike_default";

	[MyCmpGet]
	private Operational operational;

	private int resonator_cell;

	private CellOffset target_offset = new CellOffset(-1, 0);

	private Rotatable rotatable;

	private int logic_value;

	private bool wasOn;

	protected static readonly HashedString[] ON_HIT_ANIMS = new HashedString[1] { "on_hit" };

	protected static readonly HashedString[] ON_MISS_ANIMS = new HashedString[1] { "on_miss" };

	protected static readonly HashedString[] OFF_ANIMS = new HashedString[2] { "off_pre", "off" };

	protected override void OnSpawn()
	{
		base.OnSpawn();
		animController = GetComponent<KBatchedAnimController>();
		switchedOn = false;
		UpdateVisualState(connected: false);
		rotatable = GetComponent<Rotatable>();
		CellOffset rotatedCellOffset = rotatable.GetRotatedCellOffset(target_offset);
		int cell = Grid.PosToCell(base.transform.GetPosition());
		resonator_cell = Grid.OffsetCell(cell, rotatedCellOffset);
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		Subscribe(-592767678, OnOperationalChangedDelegate);
		base.OnToggle += OnSwitchToggled;
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		bool connected = false;
		if (operational.IsOperational && toggled_on)
		{
			connected = TriggerAudio();
			operational.SetActive(value: true);
		}
		else
		{
			operational.SetActive(value: false);
		}
		UpdateVisualState(connected);
	}

	private void OnOperationalChanged(object data)
	{
		if (operational.IsOperational)
		{
			SetState(LogicCircuitNetwork.IsBitActive(0, logic_value));
		}
		else
		{
			UpdateVisualState(connected: false);
		}
	}

	private bool TriggerAudio()
	{
		if (!wasOn && switchedOn)
		{
			string text = null;
			if (!Grid.IsValidCell(resonator_cell))
			{
				return false;
			}
			float num = float.NaN;
			GameObject gameObject = Grid.Objects[resonator_cell, 1];
			if (gameObject == null)
			{
				gameObject = Grid.Objects[resonator_cell, 30];
				if (gameObject == null)
				{
					gameObject = Grid.Objects[resonator_cell, 26];
					if (gameObject != null)
					{
						Wire component = gameObject.GetComponent<Wire>();
						if (component != null)
						{
							ElectricalUtilityNetwork electricalUtilityNetwork = (ElectricalUtilityNetwork)Game.Instance.electricalConduitSystem.GetNetworkForCell(component.GetNetworkCell());
							if (electricalUtilityNetwork != null)
							{
								num = electricalUtilityNetwork.allWires.Count;
							}
						}
					}
					else
					{
						gameObject = Grid.Objects[resonator_cell, 31];
						if (gameObject != null)
						{
							if (gameObject.GetComponent<LogicWire>() != null)
							{
								LogicCircuitNetwork networkForCell = Game.Instance.logicCircuitManager.GetNetworkForCell(resonator_cell);
								if (networkForCell != null)
								{
									num = networkForCell.WireCount;
								}
							}
						}
						else
						{
							gameObject = Grid.Objects[resonator_cell, 12];
							if (gameObject != null)
							{
								Conduit component2 = gameObject.GetComponent<Conduit>();
								FlowUtilityNetwork flowUtilityNetwork = (FlowUtilityNetwork)Conduit.GetNetworkManager(ConduitType.Gas).GetNetworkForCell(component2.GetNetworkCell());
								if (flowUtilityNetwork != null)
								{
									num = flowUtilityNetwork.conduitCount;
								}
							}
							else
							{
								gameObject = Grid.Objects[resonator_cell, 16];
								if (gameObject != null)
								{
									Conduit component3 = gameObject.GetComponent<Conduit>();
									FlowUtilityNetwork flowUtilityNetwork2 = (FlowUtilityNetwork)Conduit.GetNetworkManager(ConduitType.Liquid).GetNetworkForCell(component3.GetNetworkCell());
									if (flowUtilityNetwork2 != null)
									{
										num = flowUtilityNetwork2.conduitCount;
									}
								}
								else
								{
									gameObject = Grid.Objects[resonator_cell, 20];
									_ = gameObject != null;
								}
							}
						}
					}
				}
			}
			if (gameObject != null)
			{
				Building component4 = gameObject.GetComponent<BuildingComplete>();
				if (component4 != null)
				{
					text = component4.Def.PrefabID;
				}
			}
			if (text != null)
			{
				string text2 = StringFormatter.Combine(SOUND_EVENT_PREFIX, text);
				text2 = GlobalAssets.GetSound(text2, force_no_warning: true);
				if (text2 == null)
				{
					text2 = GlobalAssets.GetSound(DEFAULT_NO_SOUND_EVENT);
				}
				Vector3 position = base.transform.position;
				position.z = 0f;
				EventInstance instance = KFMOD.BeginOneShot(text2, position);
				if (!float.IsNaN(num))
				{
					instance.setParameterByName(PARAMETER_NAME, num);
				}
				KFMOD.EndOneShot(instance);
				return true;
			}
			return false;
		}
		return false;
	}

	private void UpdateVisualState(bool connected, bool force = false)
	{
		if (!(wasOn != switchedOn || force))
		{
			return;
		}
		wasOn = switchedOn;
		if (switchedOn)
		{
			if (connected)
			{
				animController.Play(ON_HIT_ANIMS);
			}
			else
			{
				animController.Play(ON_MISS_ANIMS);
			}
		}
		else
		{
			animController.Play(OFF_ANIMS);
		}
	}

	private void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == PORT_ID)
		{
			SetState(LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue));
			logic_value = logicValueChanged.newValue;
		}
	}
}
