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

	private Dictionary<Action, EDigitalActions_MainGameActionSet> kleiActionToSteamDigitalActionLookup = new Dictionary<Action, EDigitalActions_MainGameActionSet>();

	private Dictionary<Action, EAnalogActions_MainGameActionSet> kleiActionToSteamAnalogActionLookup = new Dictionary<Action, EAnalogActions_MainGameActionSet>();

	private Sprite[] spritesCache;

	private Sprite[] errorSpriteCache;

	private ESteamInputType currentControllerType;

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
		if (kleiActionToSteamDigitalActionLookup.Count < 1)
		{
			kleiActionToSteamDigitalActionLookup.Add(Action.MouseLeft, EDigitalActions_MainGameActionSet.affirmative_click);
			kleiActionToSteamDigitalActionLookup.Add(Action.MouseRight, EDigitalActions_MainGameActionSet.negative_click);
			kleiActionToSteamDigitalActionLookup.Add(Action.CameraHome, EDigitalActions_MainGameActionSet.camera_home);
			kleiActionToSteamDigitalActionLookup.Add(Action.ZoomIn, EDigitalActions_MainGameActionSet.camera_zoom_in_scroll_down);
			kleiActionToSteamDigitalActionLookup.Add(Action.ZoomOut, EDigitalActions_MainGameActionSet.camera_zoom_out_scroll_up);
			kleiActionToSteamDigitalActionLookup.Add(Action.TogglePause, EDigitalActions_MainGameActionSet.sim_pause);
			kleiActionToSteamDigitalActionLookup.Add(Action.CycleSpeed, EDigitalActions_MainGameActionSet.sim_cycle_speed);
			kleiActionToSteamDigitalActionLookup.Add(Action.RotateBuilding, EDigitalActions_MainGameActionSet.rotate_building);
			kleiActionToSteamDigitalActionLookup.Add(Action.CopyBuilding, EDigitalActions_MainGameActionSet.copy_building);
			kleiActionToSteamDigitalActionLookup.Add(Action.Dig, EDigitalActions_MainGameActionSet.dig_tool);
			kleiActionToSteamDigitalActionLookup.Add(Action.BuildingCancel, EDigitalActions_MainGameActionSet.cancel_tool);
			kleiActionToSteamDigitalActionLookup.Add(Action.BuildingDeconstruct, EDigitalActions_MainGameActionSet.deconstruct_tool);
			kleiActionToSteamDigitalActionLookup.Add(Action.Prioritize, EDigitalActions_MainGameActionSet.priority_tool);
			kleiActionToSteamDigitalActionLookup.Add(Action.Disinfect, EDigitalActions_MainGameActionSet.disinfect_tool);
			kleiActionToSteamDigitalActionLookup.Add(Action.Clear, EDigitalActions_MainGameActionSet.sweep_tool);
			kleiActionToSteamDigitalActionLookup.Add(Action.Mop, EDigitalActions_MainGameActionSet.mop_tool);
			kleiActionToSteamDigitalActionLookup.Add(Action.Attack, EDigitalActions_MainGameActionSet.attack_tool);
			kleiActionToSteamDigitalActionLookup.Add(Action.Capture, EDigitalActions_MainGameActionSet.wrangle_tool);
			kleiActionToSteamDigitalActionLookup.Add(Action.Harvest, EDigitalActions_MainGameActionSet.harvest_tool);
			kleiActionToSteamDigitalActionLookup.Add(Action.EmptyPipe, EDigitalActions_MainGameActionSet.empty_tool);
			kleiActionToSteamDigitalActionLookup.Add(Action.Escape, EDigitalActions_MainGameActionSet.pause_menu);
			kleiActionToSteamDigitalActionLookup.Add(Action.ManageVitals, EDigitalActions_MainGameActionSet.vitals_menu);
			kleiActionToSteamDigitalActionLookup.Add(Action.ManageConsumables, EDigitalActions_MainGameActionSet.consumables_menu);
			kleiActionToSteamDigitalActionLookup.Add(Action.ManageSchedule, EDigitalActions_MainGameActionSet.schedule_menu);
			kleiActionToSteamDigitalActionLookup.Add(Action.ManagePriorities, EDigitalActions_MainGameActionSet.priorities_menu);
			kleiActionToSteamDigitalActionLookup.Add(Action.ManageSkills, EDigitalActions_MainGameActionSet.skills_menu);
			kleiActionToSteamDigitalActionLookup.Add(Action.ManageResearch, EDigitalActions_MainGameActionSet.research_menu);
			kleiActionToSteamDigitalActionLookup.Add(Action.ManageStarmap, EDigitalActions_MainGameActionSet.starmap_menu);
			kleiActionToSteamDigitalActionLookup.Add(Action.ManageReport, EDigitalActions_MainGameActionSet.colony_menu);
			kleiActionToSteamDigitalActionLookup.Add(Action.ManageDatabase, EDigitalActions_MainGameActionSet.codex_menu);
			kleiActionToSteamDigitalActionLookup.Add(Action.Overlay1, EDigitalActions_MainGameActionSet.oxygen_overlay);
			kleiActionToSteamDigitalActionLookup.Add(Action.Overlay2, EDigitalActions_MainGameActionSet.power_overlay);
			kleiActionToSteamDigitalActionLookup.Add(Action.Overlay3, EDigitalActions_MainGameActionSet.temperature_overlay);
			kleiActionToSteamDigitalActionLookup.Add(Action.Overlay4, EDigitalActions_MainGameActionSet.materials_overlay);
			kleiActionToSteamDigitalActionLookup.Add(Action.Overlay5, EDigitalActions_MainGameActionSet.light_overlay);
			kleiActionToSteamDigitalActionLookup.Add(Action.Overlay6, EDigitalActions_MainGameActionSet.plumbing_overlay);
			kleiActionToSteamDigitalActionLookup.Add(Action.Overlay7, EDigitalActions_MainGameActionSet.ventilation_overlay);
			kleiActionToSteamDigitalActionLookup.Add(Action.Overlay8, EDigitalActions_MainGameActionSet.decor_overlay);
			kleiActionToSteamDigitalActionLookup.Add(Action.Overlay9, EDigitalActions_MainGameActionSet.germs_overlay);
			kleiActionToSteamDigitalActionLookup.Add(Action.Overlay10, EDigitalActions_MainGameActionSet.farming_overlay);
			kleiActionToSteamDigitalActionLookup.Add(Action.Overlay11, EDigitalActions_MainGameActionSet.rooms_overlay);
			kleiActionToSteamDigitalActionLookup.Add(Action.Overlay12, EDigitalActions_MainGameActionSet.exosuits_overlay);
			kleiActionToSteamDigitalActionLookup.Add(Action.Overlay13, EDigitalActions_MainGameActionSet.automation_overlay);
			kleiActionToSteamDigitalActionLookup.Add(Action.Overlay14, EDigitalActions_MainGameActionSet.shipping_overlay);
			kleiActionToSteamDigitalActionLookup.Add(Action.Overlay15, EDigitalActions_MainGameActionSet.radiation_overlay);
		}
		if (kleiActionToSteamAnalogActionLookup.Count < 1)
		{
			kleiActionToSteamAnalogActionLookup.Add(Action.AnalogCamera, EAnalogActions_MainGameActionSet.Camera);
			kleiActionToSteamAnalogActionLookup.Add(Action.AnalogCursor, EAnalogActions_MainGameActionSet.Cursor);
		}
		errorSpriteCache = Resources.LoadAll<Sprite>("Sprite Assets/ErrorSheet");
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
			if (kleiActionToSteamDigitalActionLookup.TryGetValue(action, out var value))
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
		int finalIndex = -1;
		int offset = 0;
		string spritesheetName = string.Empty;
		EAnalogActions_MainGameActionSet value2;
		if (kleiActionToSteamDigitalActionLookup.TryGetValue(action, out var value))
		{
			InputDigitalActionHandle_t digitalActionHandle = m_MainGameActionSetDigitalActionHandles[(int)value];
			EInputActionOrigin[] array = new EInputActionOrigin[8];
			SteamInput.GetDigitalActionOrigins(m_InputHandles[activeControllerIndex], m_ActionSetHandles[0], digitalActionHandle, array);
			EInputActionOrigin num = array[0];
			GetControllerTypeForGlyphLookup(ref offset, ref spritesheetName);
			finalIndex = (int)(num - offset);
		}
		else if (kleiActionToSteamAnalogActionLookup.TryGetValue(action, out value2))
		{
			InputAnalogActionHandle_t analogActionHandle = m_MainGameActionSetAnalogActionHandles[(int)value2];
			EInputActionOrigin[] array2 = new EInputActionOrigin[8];
			SteamInput.GetAnalogActionOrigins(m_InputHandles[activeControllerIndex], m_ActionSetHandles[0], analogActionHandle, array2);
			EInputActionOrigin num2 = array2[0];
			GetControllerTypeForGlyphLookup(ref offset, ref spritesheetName);
			finalIndex = (int)(num2 - offset);
		}
		return GetFinalGlyphString(finalIndex, spritesheetName);
	}

	public Sprite GetActionSprite(Action action, bool ShowEmptyOnError = false)
	{
		int num = -1;
		int offset = 0;
		string spritesheetName = string.Empty;
		bool flag = false;
		if (SteamInput.GetInputTypeForHandle(m_InputHandles[activeControllerIndex]) != currentControllerType)
		{
			currentControllerType = SteamInput.GetInputTypeForHandle(m_InputHandles[activeControllerIndex]);
			flag = true;
		}
		EAnalogActions_MainGameActionSet value2;
		if (kleiActionToSteamDigitalActionLookup.TryGetValue(action, out var value))
		{
			InputDigitalActionHandle_t digitalActionHandle = m_MainGameActionSetDigitalActionHandles[(int)value];
			EInputActionOrigin[] array = new EInputActionOrigin[8];
			SteamInput.GetDigitalActionOrigins(m_InputHandles[activeControllerIndex], m_ActionSetHandles[0], digitalActionHandle, array);
			EInputActionOrigin num2 = array[0];
			GetControllerTypeForGlyphLookup(ref offset, ref spritesheetName);
			num = (int)(num2 - offset);
		}
		else if (kleiActionToSteamAnalogActionLookup.TryGetValue(action, out value2))
		{
			InputAnalogActionHandle_t analogActionHandle = m_MainGameActionSetAnalogActionHandles[(int)value2];
			EInputActionOrigin[] array2 = new EInputActionOrigin[8];
			SteamInput.GetAnalogActionOrigins(m_InputHandles[activeControllerIndex], m_ActionSetHandles[0], analogActionHandle, array2);
			EInputActionOrigin num3 = array2[0];
			GetControllerTypeForGlyphLookup(ref offset, ref spritesheetName);
			num = (int)(num3 - offset);
		}
		if (num >= 0)
		{
			if (flag || spritesCache == null)
			{
				spritesCache = Resources.LoadAll<Sprite>("Sprite Assets/" + spritesheetName);
			}
			return spritesCache[num];
		}
		if (!ShowEmptyOnError)
		{
			return errorSpriteCache[0];
		}
		return errorSpriteCache[1];
	}

	private void GetControllerTypeForGlyphLookup(ref int offset, ref string spritesheetName)
	{
		currentControllerType = SteamInput.GetInputTypeForHandle(m_InputHandles[activeControllerIndex]);
		switch (currentControllerType)
		{
		case ESteamInputType.k_ESteamInputType_Unknown:
			offset = 0;
			break;
		case ESteamInputType.k_ESteamInputType_SteamController:
			spritesheetName = "SteamControllerSheet";
			offset = 1;
			break;
		case ESteamInputType.k_ESteamInputType_XBox360Controller:
			spritesheetName = "XB360Sheet";
			offset = 153;
			break;
		case ESteamInputType.k_ESteamInputType_XBoxOneController:
			spritesheetName = "XboxOneSheet";
			offset = 114;
			break;
		case ESteamInputType.k_ESteamInputType_GenericGamepad:
			offset = 0;
			break;
		case ESteamInputType.k_ESteamInputType_PS4Controller:
			spritesheetName = "PS4Sheet";
			offset = 50;
			break;
		case ESteamInputType.k_ESteamInputType_SwitchJoyConPair:
			offset = 192;
			break;
		case ESteamInputType.k_ESteamInputType_SwitchProController:
			offset = 192;
			break;
		case ESteamInputType.k_ESteamInputType_PS5Controller:
			offset = 258;
			break;
		case ESteamInputType.k_ESteamInputType_SteamDeckController:
			spritesheetName = "SteamDeckSheet";
			offset = 333;
			break;
		case ESteamInputType.k_ESteamInputType_AppleMFiController:
		case ESteamInputType.k_ESteamInputType_AndroidController:
		case ESteamInputType.k_ESteamInputType_SwitchJoyConSingle:
		case ESteamInputType.k_ESteamInputType_MobileTouch:
		case ESteamInputType.k_ESteamInputType_PS3Controller:
			break;
		}
	}

	private string GetFinalGlyphString(int finalIndex, string spriteAssetSet)
	{
		if (finalIndex < 0)
		{
			return string.Empty;
		}
		return "<sprite=\"" + spriteAssetSet + "\" index=" + finalIndex + ">";
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
