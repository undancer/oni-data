using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamInputInterpreter
{
	private enum EActionSets
	{
		MainGameActionSet
	}

	private enum EAnalogActions_MainGameActionSet
	{
		Camera,
		Cursor
	}

	private enum EDigitalActions_MainGameActionSet
	{
		affirmative_click,
		negative_click,
		camera_home,
		camera_zoom_in_scroll_down,
		camera_zoom_out_scroll_up,
		sim_pause,
		sim_cycle_speed,
		rotate_building,
		copy_building,
		dig_tool,
		cancel_tool,
		deconstruct_tool,
		priority_tool,
		disinfect_tool,
		sweep_tool,
		mop_tool,
		attack_tool,
		wrangle_tool,
		harvest_tool,
		empty_tool,
		build_menu_up,
		build_menu_down,
		build_menu_left,
		build_menu_right,
		pause_menu,
		vitals_menu,
		consumables_menu,
		schedule_menu,
		priorities_menu,
		skills_menu,
		research_menu,
		starmap_menu,
		colony_menu,
		codex_menu,
		oxygen_overlay,
		power_overlay,
		temperature_overlay,
		materials_overlay,
		light_overlay,
		plumbing_overlay,
		ventilation_overlay,
		decor_overlay,
		germs_overlay,
		farming_overlay,
		rooms_overlay,
		exosuits_overlay,
		automation_overlay,
		shipping_overlay,
		radiation_overlay,
		toggle_resources,
		toggle_diagnostics
	}

	private struct AnalogData
	{
		public float x;

		public float y;
	}

	private struct DigitalData
	{
		public bool down;
	}

	private struct ControllerData
	{
		public AnalogData[] analogDatas;

		public DigitalData[] digitalDatas;
	}

	private Vector2 m_ScrollPos;

	private bool m_InputInitialized;

	private int m_nInputs;

	private int activeControllerIndex;

	private Dictionary<Action, EDigitalActions_MainGameActionSet> kleiActionToSteamActionLookup = new Dictionary<Action, EDigitalActions_MainGameActionSet>();

	private int m_numActionSets;

	private int m_numMainGameActionSetAnalogActions;

	private int m_numMainGameActionSetDigitalActions;

	private string[] m_ActionSetNames;

	private string[] m_MainGameActionSetAnalogActionNames;

	private string[] m_MainGameActionSetDigitalActionNames;

	private InputActionSetHandle_t[] m_ActionSetHandles;

	private InputAnalogActionHandle_t[] m_MainGameActionSetAnalogActionHandles;

	private InputDigitalActionHandle_t[] m_MainGameActionSetDigitalActionHandles;

	private InputHandle_t[] m_InputHandles;

	private ControllerData[] controllerDatas = new ControllerData[0];

	public int NumOfISteamInputs => m_nInputs;

	public bool Initialized => m_InputInitialized;

	public void OnEnable()
	{
		m_InputInitialized = DistributionPlatform.Initialized && SteamInput.Init(bExplicitlyCallRunFrame: true);
		if (m_InputInitialized)
		{
			m_InputHandles = new InputHandle_t[16];
			Precache();
		}
	}

	private void OnDisable()
	{
		if (m_InputInitialized)
		{
			SteamInput.Shutdown();
		}
		m_InputInitialized = false;
	}

	public void Precache()
	{
		m_ActionSetNames = Enum.GetNames(typeof(EActionSets));
		m_numActionSets = m_ActionSetNames.Length;
		m_ActionSetHandles = new InputActionSetHandle_t[m_numActionSets];
		for (int i = 0; i < m_numActionSets; i++)
		{
			m_ActionSetHandles[i] = SteamInput.GetActionSetHandle(m_ActionSetNames[i]);
		}
		m_MainGameActionSetAnalogActionNames = Enum.GetNames(typeof(EAnalogActions_MainGameActionSet));
		m_numMainGameActionSetAnalogActions = m_MainGameActionSetAnalogActionNames.Length;
		m_MainGameActionSetAnalogActionHandles = new InputAnalogActionHandle_t[m_numMainGameActionSetAnalogActions];
		for (int j = 0; j < m_numMainGameActionSetAnalogActions; j++)
		{
			m_MainGameActionSetAnalogActionHandles[j] = SteamInput.GetAnalogActionHandle(m_MainGameActionSetAnalogActionNames[j]);
		}
		m_MainGameActionSetDigitalActionNames = Enum.GetNames(typeof(EDigitalActions_MainGameActionSet));
		m_numMainGameActionSetDigitalActions = m_MainGameActionSetDigitalActionNames.Length;
		m_MainGameActionSetDigitalActionHandles = new InputDigitalActionHandle_t[m_numMainGameActionSetDigitalActions];
		for (int k = 0; k < m_numMainGameActionSetDigitalActions; k++)
		{
			m_MainGameActionSetDigitalActionHandles[k] = SteamInput.GetDigitalActionHandle(m_MainGameActionSetDigitalActionNames[k]);
		}
		if (kleiActionToSteamActionLookup.Count < 1)
		{
			kleiActionToSteamActionLookup.Add(Action.MouseLeft, EDigitalActions_MainGameActionSet.affirmative_click);
			kleiActionToSteamActionLookup.Add(Action.MouseRight, EDigitalActions_MainGameActionSet.negative_click);
			kleiActionToSteamActionLookup.Add(Action.CameraHome, EDigitalActions_MainGameActionSet.camera_home);
			kleiActionToSteamActionLookup.Add(Action.ZoomIn, EDigitalActions_MainGameActionSet.camera_zoom_in_scroll_down);
			kleiActionToSteamActionLookup.Add(Action.ZoomOut, EDigitalActions_MainGameActionSet.camera_zoom_out_scroll_up);
			kleiActionToSteamActionLookup.Add(Action.TogglePause, EDigitalActions_MainGameActionSet.sim_pause);
			kleiActionToSteamActionLookup.Add(Action.CycleSpeed, EDigitalActions_MainGameActionSet.sim_cycle_speed);
			kleiActionToSteamActionLookup.Add(Action.RotateBuilding, EDigitalActions_MainGameActionSet.rotate_building);
			kleiActionToSteamActionLookup.Add(Action.CopyBuilding, EDigitalActions_MainGameActionSet.copy_building);
			kleiActionToSteamActionLookup.Add(Action.Dig, EDigitalActions_MainGameActionSet.dig_tool);
			kleiActionToSteamActionLookup.Add(Action.BuildingCancel, EDigitalActions_MainGameActionSet.cancel_tool);
			kleiActionToSteamActionLookup.Add(Action.BuildingDeconstruct, EDigitalActions_MainGameActionSet.deconstruct_tool);
			kleiActionToSteamActionLookup.Add(Action.Prioritize, EDigitalActions_MainGameActionSet.priority_tool);
			kleiActionToSteamActionLookup.Add(Action.Disinfect, EDigitalActions_MainGameActionSet.disinfect_tool);
			kleiActionToSteamActionLookup.Add(Action.Clear, EDigitalActions_MainGameActionSet.sweep_tool);
			kleiActionToSteamActionLookup.Add(Action.Mop, EDigitalActions_MainGameActionSet.mop_tool);
			kleiActionToSteamActionLookup.Add(Action.Attack, EDigitalActions_MainGameActionSet.attack_tool);
			kleiActionToSteamActionLookup.Add(Action.Capture, EDigitalActions_MainGameActionSet.wrangle_tool);
			kleiActionToSteamActionLookup.Add(Action.Harvest, EDigitalActions_MainGameActionSet.harvest_tool);
			kleiActionToSteamActionLookup.Add(Action.EmptyPipe, EDigitalActions_MainGameActionSet.empty_tool);
			kleiActionToSteamActionLookup.Add(Action.Escape, EDigitalActions_MainGameActionSet.pause_menu);
			kleiActionToSteamActionLookup.Add(Action.ManageVitals, EDigitalActions_MainGameActionSet.vitals_menu);
			kleiActionToSteamActionLookup.Add(Action.ManageConsumables, EDigitalActions_MainGameActionSet.consumables_menu);
			kleiActionToSteamActionLookup.Add(Action.ManageSchedule, EDigitalActions_MainGameActionSet.schedule_menu);
			kleiActionToSteamActionLookup.Add(Action.ManagePriorities, EDigitalActions_MainGameActionSet.priorities_menu);
			kleiActionToSteamActionLookup.Add(Action.ManageSkills, EDigitalActions_MainGameActionSet.skills_menu);
			kleiActionToSteamActionLookup.Add(Action.ManageResearch, EDigitalActions_MainGameActionSet.research_menu);
			kleiActionToSteamActionLookup.Add(Action.ManageStarmap, EDigitalActions_MainGameActionSet.starmap_menu);
			kleiActionToSteamActionLookup.Add(Action.ManageReport, EDigitalActions_MainGameActionSet.colony_menu);
			kleiActionToSteamActionLookup.Add(Action.ManageDatabase, EDigitalActions_MainGameActionSet.codex_menu);
			kleiActionToSteamActionLookup.Add(Action.Overlay1, EDigitalActions_MainGameActionSet.oxygen_overlay);
			kleiActionToSteamActionLookup.Add(Action.Overlay2, EDigitalActions_MainGameActionSet.power_overlay);
			kleiActionToSteamActionLookup.Add(Action.Overlay3, EDigitalActions_MainGameActionSet.temperature_overlay);
			kleiActionToSteamActionLookup.Add(Action.Overlay4, EDigitalActions_MainGameActionSet.materials_overlay);
			kleiActionToSteamActionLookup.Add(Action.Overlay5, EDigitalActions_MainGameActionSet.light_overlay);
			kleiActionToSteamActionLookup.Add(Action.Overlay6, EDigitalActions_MainGameActionSet.plumbing_overlay);
			kleiActionToSteamActionLookup.Add(Action.Overlay7, EDigitalActions_MainGameActionSet.ventilation_overlay);
			kleiActionToSteamActionLookup.Add(Action.Overlay8, EDigitalActions_MainGameActionSet.decor_overlay);
			kleiActionToSteamActionLookup.Add(Action.Overlay9, EDigitalActions_MainGameActionSet.germs_overlay);
			kleiActionToSteamActionLookup.Add(Action.Overlay10, EDigitalActions_MainGameActionSet.farming_overlay);
			kleiActionToSteamActionLookup.Add(Action.Overlay11, EDigitalActions_MainGameActionSet.rooms_overlay);
			kleiActionToSteamActionLookup.Add(Action.Overlay12, EDigitalActions_MainGameActionSet.exosuits_overlay);
			kleiActionToSteamActionLookup.Add(Action.Overlay13, EDigitalActions_MainGameActionSet.automation_overlay);
			kleiActionToSteamActionLookup.Add(Action.Overlay14, EDigitalActions_MainGameActionSet.shipping_overlay);
			kleiActionToSteamActionLookup.Add(Action.Overlay15, EDigitalActions_MainGameActionSet.radiation_overlay);
		}
		SteamInput.RunFrame();
		m_nInputs = SteamInput.GetConnectedControllers(m_InputHandles);
		for (int l = 0; l < m_InputHandles.Length; l++)
		{
			SteamInput.ActivateActionSet(m_InputHandles[l], m_ActionSetHandles[0]);
		}
	}

	public void Reset()
	{
		SteamInput.RunFrame();
		m_nInputs = SteamInput.GetConnectedControllers(m_InputHandles);
		for (int i = 0; i < m_InputHandles.Length; i++)
		{
			SteamInput.ActivateActionSet(m_InputHandles[i], m_ActionSetHandles[0]);
		}
	}

	public bool GetSteamInputActionIsDown(Action action)
	{
		bool flag = false;
		for (int i = 0; i < m_nInputs; i++)
		{
			if (kleiActionToSteamActionLookup.TryGetValue(action, out var value))
			{
				flag |= controllerDatas[i].digitalDatas[(int)value].down;
			}
		}
		return flag;
	}

	public Vector2 GetSteamCursorMovement()
	{
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < m_nInputs; i++)
		{
			float x = controllerDatas[i].analogDatas[1].x;
			float y = controllerDatas[i].analogDatas[1].y;
			if (Mathf.Abs(x) > Mathf.Epsilon || Mathf.Abs(y) > Mathf.Epsilon)
			{
				zero += new Vector2(x, 0f - y);
			}
		}
		return zero;
	}

	public Vector2 GetSteamCameraMovement()
	{
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < m_nInputs; i++)
		{
			float x = controllerDatas[i].analogDatas[0].x;
			float y = controllerDatas[i].analogDatas[0].y;
			if (Mathf.Abs(x) > Mathf.Epsilon || Mathf.Abs(y) > Mathf.Epsilon)
			{
				zero += new Vector2(x, y);
			}
		}
		return zero;
	}

	public string GetActionGlyph(Action action)
	{
		int num = -1;
		string text = string.Empty;
		if (kleiActionToSteamActionLookup.TryGetValue(action, out var value))
		{
			InputDigitalActionHandle_t digitalActionHandle = m_MainGameActionSetDigitalActionHandles[(int)value];
			EInputActionOrigin[] array = new EInputActionOrigin[8];
			SteamInput.GetDigitalActionOrigins(m_InputHandles[activeControllerIndex], m_ActionSetHandles[0], digitalActionHandle, array);
			int num2 = 0;
			int num3 = (int)array[0];
			switch (SteamInput.GetInputTypeForHandle(m_InputHandles[activeControllerIndex]))
			{
			case ESteamInputType.k_ESteamInputType_Unknown:
				num2 = 0;
				break;
			case ESteamInputType.k_ESteamInputType_SteamController:
				text = "SteamControllerSheet";
				num2 = 1;
				break;
			case ESteamInputType.k_ESteamInputType_XBox360Controller:
				text = "XB360Sheet";
				num2 = 153;
				break;
			case ESteamInputType.k_ESteamInputType_XBoxOneController:
				text = "XboxOneSheet";
				num2 = 114;
				break;
			case ESteamInputType.k_ESteamInputType_GenericGamepad:
				num2 = 0;
				break;
			case ESteamInputType.k_ESteamInputType_PS4Controller:
				text = "PS4Sheet";
				num2 = 50;
				break;
			case ESteamInputType.k_ESteamInputType_SwitchJoyConPair:
				num2 = 192;
				break;
			case ESteamInputType.k_ESteamInputType_SwitchProController:
				num2 = 192;
				break;
			case ESteamInputType.k_ESteamInputType_PS5Controller:
				num2 = 258;
				break;
			case ESteamInputType.k_ESteamInputType_SteamDeckController:
				text = "SteamDeckSheet";
				num2 = 333;
				break;
			}
			num = num3 - num2;
		}
		if (num >= 0)
		{
			return "<sprite=\"" + text + "\" index=" + num + ">";
		}
		return "";
	}

	public void CheckForControllerChange()
	{
		SteamInput.RunFrame();
		int nInputs = m_nInputs;
		m_nInputs = SteamInput.GetConnectedControllers(m_InputHandles);
		for (int i = 0; i < m_nInputs; i++)
		{
			SteamInput.ActivateActionSet(m_InputHandles[i], m_ActionSetHandles[0]);
		}
		if (nInputs == 0 && m_nInputs != 0)
		{
			Precache();
		}
	}

	public void Update()
	{
		if (!m_InputInitialized)
		{
			return;
		}
		CheckForControllerChange();
		if (controllerDatas.Length != m_nInputs)
		{
			controllerDatas = new ControllerData[m_nInputs];
			for (int i = 0; i < m_nInputs; i++)
			{
				controllerDatas[i].analogDatas = new AnalogData[m_numMainGameActionSetAnalogActions];
				controllerDatas[i].digitalDatas = new DigitalData[m_numMainGameActionSetDigitalActions];
			}
		}
		for (int j = 0; j < m_nInputs; j++)
		{
			for (int k = 0; k < m_numMainGameActionSetDigitalActions; k++)
			{
				bool flag = SteamInput.GetDigitalActionData(m_InputHandles[j], m_MainGameActionSetDigitalActionHandles[k]).bState != 0;
				controllerDatas[j].digitalDatas[k].down = flag;
				if (flag && (!KInputManager.currentControllerIsGamepad || activeControllerIndex != j))
				{
					activeControllerIndex = j;
					KInputManager.currentControllerIsGamepad = true;
					KInputManager.InputChange.Invoke();
				}
			}
			for (int l = 0; l < m_numMainGameActionSetAnalogActions; l++)
			{
				InputAnalogActionData_t analogActionData = SteamInput.GetAnalogActionData(m_InputHandles[j], m_MainGameActionSetAnalogActionHandles[l]);
				controllerDatas[j].analogDatas[l].x = analogActionData.x;
				controllerDatas[j].analogDatas[l].y = analogActionData.y;
				if ((Mathf.Abs(analogActionData.x) > Mathf.Epsilon || Mathf.Abs(analogActionData.y) > Mathf.Epsilon) && (!KInputManager.currentControllerIsGamepad || activeControllerIndex != j))
				{
					activeControllerIndex = j;
					KInputManager.currentControllerIsGamepad = true;
					KInputManager.InputChange.Invoke();
				}
			}
		}
	}
}
