using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Klei;
using KMod;
using KSerialization;
using STRINGS;
using UnityEngine;
using UnityEngine.U2D;

public class Global : MonoBehaviour
{
	public SpriteAtlas[] forcedAtlasInitializationList;

	public GameObject modErrorsPrefab;

	public GameObject globalCanvas;

	private GameInputManager mInputManager;

	private AnimEventManager mAnimEventManager;

	public KMod.Manager modManager;

	private bool gotKleiUserID = false;

	public Thread mainThread;

	private bool updated_with_initialized_distribution_platform = false;

	public static readonly string LanguageModKey = "LanguageMod";

	public static readonly string LanguageCodeKey = "LanguageCode";

	public static Global Instance
	{
		get;
		private set;
	}

	public static BindingEntry[] GenerateDefaultBindings(bool hotKeyBuildMenuPermitted = true)
	{
		List<BindingEntry> list = new List<BindingEntry>
		{
			new BindingEntry(null, GamepadButton.NumButtons, KKeyCode.Escape, Modifier.None, Action.Escape, rebindable: false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.W, Modifier.None, Action.PanUp),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.S, Modifier.None, Action.PanDown),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.A, Modifier.None, Action.PanLeft),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.D, Modifier.None, Action.PanRight),
			new BindingEntry("Tool", GamepadButton.NumButtons, KKeyCode.O, Modifier.None, Action.RotateBuilding),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.L, Modifier.None, Action.ManagePriorities),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.F, Modifier.None, Action.ManageConsumables),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.V, Modifier.None, Action.ManageVitals),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.R, Modifier.None, Action.ManageResearch),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.E, Modifier.None, Action.ManageReport),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.U, Modifier.None, Action.ManageDatabase),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.J, Modifier.None, Action.ManageSkills),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.Period, Modifier.None, Action.ManageSchedule),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.Z, Modifier.None, Action.ManageStarmap),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.G, Modifier.None, Action.Dig),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.M, Modifier.None, Action.Mop),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.K, Modifier.None, Action.Clear),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.I, Modifier.None, Action.Disinfect),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.T, Modifier.None, Action.Attack),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.N, Modifier.None, Action.Capture),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Y, Modifier.None, Action.Harvest),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Insert, Modifier.None, Action.EmptyPipe),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.P, Modifier.None, Action.Prioritize),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.S, Modifier.Alt, Action.ToggleScreenshotMode),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.C, Modifier.None, Action.BuildingCancel),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.X, Modifier.None, Action.BuildingDeconstruct),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Tab, Modifier.None, Action.CycleSpeed),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.H, Modifier.None, Action.CameraHome),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Mouse0, Modifier.None, Action.MouseLeft, rebindable: false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Mouse0, Modifier.Shift, Action.ShiftMouseLeft, rebindable: false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Mouse1, Modifier.None, Action.MouseRight, rebindable: false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Mouse2, Modifier.None, Action.MouseMiddle, rebindable: false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.None, Action.Plan1),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.None, Action.Plan2),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.None, Action.Plan3),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.None, Action.Plan4),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.None, Action.Plan5),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha6, Modifier.None, Action.Plan6),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha7, Modifier.None, Action.Plan7),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha8, Modifier.None, Action.Plan8),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.None, Action.Plan9),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.None, Action.Plan10),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Minus, Modifier.None, Action.Plan11),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Equals, Modifier.None, Action.Plan12),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Minus, Modifier.Shift, Action.Plan13),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Equals, Modifier.Shift, Action.Plan14),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.B, Modifier.None, Action.CopyBuilding),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.MouseScrollUp, Modifier.None, Action.ZoomIn),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.MouseScrollDown, Modifier.None, Action.ZoomOut),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F1, Modifier.None, Action.Overlay1),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F2, Modifier.None, Action.Overlay2),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F3, Modifier.None, Action.Overlay3),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F4, Modifier.None, Action.Overlay4),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F5, Modifier.None, Action.Overlay5),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F6, Modifier.None, Action.Overlay6),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F7, Modifier.None, Action.Overlay7),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F8, Modifier.None, Action.Overlay8),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F9, Modifier.None, Action.Overlay9),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F10, Modifier.None, Action.Overlay10),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F11, Modifier.None, Action.Overlay11),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F1, Modifier.Shift, Action.Overlay12),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F2, Modifier.Shift, Action.Overlay13),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F3, Modifier.Shift, Action.Overlay14),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F4, Modifier.Shift, Action.Overlay15),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.KeypadPlus, Modifier.None, Action.SpeedUp),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.KeypadMinus, Modifier.None, Action.SlowDown),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Space, Modifier.None, Action.TogglePause),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.Ctrl, Action.SetUserNav1),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.Ctrl, Action.SetUserNav2),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.Ctrl, Action.SetUserNav3),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.Ctrl, Action.SetUserNav4),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.Ctrl, Action.SetUserNav5),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha6, Modifier.Ctrl, Action.SetUserNav6),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha7, Modifier.Ctrl, Action.SetUserNav7),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha8, Modifier.Ctrl, Action.SetUserNav8),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.Ctrl, Action.SetUserNav9),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.Ctrl, Action.SetUserNav10),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.Shift, Action.GotoUserNav1),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.Shift, Action.GotoUserNav2),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.Shift, Action.GotoUserNav3),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.Shift, Action.GotoUserNav4),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.Shift, Action.GotoUserNav5),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha6, Modifier.Shift, Action.GotoUserNav6),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha7, Modifier.Shift, Action.GotoUserNav7),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha8, Modifier.Shift, Action.GotoUserNav8),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.Shift, Action.GotoUserNav9),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.Shift, Action.GotoUserNav10),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.C, Modifier.None, Action.CinemaCamEnable, rebindable: true, ignore_root_conflicts: true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.A, Modifier.None, Action.CinemaPanLeft, rebindable: true, ignore_root_conflicts: true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.D, Modifier.None, Action.CinemaPanRight, rebindable: true, ignore_root_conflicts: true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.W, Modifier.None, Action.CinemaPanUp, rebindable: true, ignore_root_conflicts: true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.S, Modifier.None, Action.CinemaPanDown, rebindable: true, ignore_root_conflicts: true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.I, Modifier.None, Action.CinemaZoomIn, rebindable: true, ignore_root_conflicts: true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.O, Modifier.None, Action.CinemaZoomOut, rebindable: true, ignore_root_conflicts: true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.Z, Modifier.None, Action.CinemaZoomSpeedPlus, rebindable: true, ignore_root_conflicts: true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.Z, Modifier.Shift, Action.CinemaZoomSpeedMinus, rebindable: true, ignore_root_conflicts: true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.P, Modifier.None, Action.CinemaUnpauseOnMove, rebindable: true, ignore_root_conflicts: true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.T, Modifier.None, Action.CinemaToggleLock, rebindable: true, ignore_root_conflicts: true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.E, Modifier.None, Action.CinemaToggleEasing, rebindable: true, ignore_root_conflicts: true),
			new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.Slash, Modifier.None, Action.ToggleOpen),
			new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.Return, Modifier.None, Action.ToggleEnabled),
			new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.Backslash, Modifier.None, Action.BuildingUtility1),
			new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.LeftBracket, Modifier.None, Action.BuildingUtility2),
			new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.RightBracket, Modifier.None, Action.BuildingUtility3),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.LeftAlt, Modifier.Alt, Action.AlternateView),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.RightAlt, Modifier.Alt, Action.AlternateView),
			new BindingEntry("Tool", GamepadButton.NumButtons, KKeyCode.LeftShift, Modifier.Shift, Action.DragStraight),
			new BindingEntry("Tool", GamepadButton.NumButtons, KKeyCode.RightShift, Modifier.Shift, Action.DragStraight),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.T, Modifier.Ctrl, Action.DebugFocus),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.U, Modifier.Ctrl, Action.DebugUltraTestMode),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F1, Modifier.Alt, Action.DebugToggleUI),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F3, Modifier.Alt, Action.DebugCollectGarbage),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F7, Modifier.Alt, Action.DebugInvincible),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, Modifier.Alt, Action.DebugForceLightEverywhere),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, Modifier.Shift, Action.DebugElementTest),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F12, Modifier.Shift, Action.DebugTileTest),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.N, Modifier.Alt, Action.DebugRefreshNavCell),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Q, Modifier.Ctrl, Action.DebugGotoTarget),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.S, Modifier.Ctrl, Action.DebugSelectMaterial),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.M, Modifier.Ctrl, Action.DebugToggleMusic),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F, Modifier.Ctrl, Action.DebugToggleClusterFX),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Backspace, Modifier.None, Action.DebugToggle),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Backspace, Modifier.Ctrl, Action.DebugToggleFastWorkers),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Q, Modifier.Alt, Action.DebugTeleport),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F2, Modifier.Alt, Action.DebugSpawnMinionAtmoSuit),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F2, Modifier.Ctrl, Action.DebugSpawnMinion),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F3, Modifier.Ctrl, Action.DebugPlace),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F4, Modifier.Ctrl, Action.DebugInstantBuildMode),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F5, Modifier.Ctrl, Action.DebugSlowTestMode),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F6, Modifier.Ctrl, Action.DebugDig),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F8, Modifier.Ctrl, Action.DebugExplosion),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F9, Modifier.Ctrl, Action.DebugDiscoverAllElements),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.T, Modifier.Alt, Action.DebugToggleSelectInEditor),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.P, Modifier.Alt, Action.DebugPathFinding),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Z, Modifier.Alt, Action.DebugSuperSpeed),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Equals, Modifier.Alt, Action.DebugGameStep),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Minus, Modifier.Alt, Action.DebugSimStep),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.X, Modifier.Alt, Action.DebugNotification),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.C, Modifier.Alt, Action.DebugNotificationMessage),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.BackQuote, Modifier.None, Action.ToggleProfiler),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.BackQuote, Modifier.Alt, Action.ToggleChromeProfiler),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F1, Modifier.Ctrl, Action.DebugDumpSceneParitionerLeakData),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F12, Modifier.Ctrl, Action.DebugTriggerException),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F12, (Modifier)6, Action.DebugTriggerError),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, Modifier.Ctrl, Action.DebugDumpGCRoots),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, (Modifier)3, Action.DebugDumpGarbageReferences),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F11, Modifier.Ctrl, Action.DebugDumpEventData),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F7, (Modifier)3, Action.DebugCrashSim),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.Alt, Action.DebugNextCall),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.Alt, Action.SreenShot1x),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.Alt, Action.SreenShot2x),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.Alt, Action.SreenShot8x),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.Alt, Action.SreenShot32x),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.Alt, Action.DebugLockCursor),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.Alt, Action.DebugTogglePersonalPriorityComparison),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Return, Modifier.None, Action.DialogSubmit, rebindable: false),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.A, Modifier.None, Action.BuildMenuKeyA, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.B, Modifier.None, Action.BuildMenuKeyB, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.C, Modifier.None, Action.BuildMenuKeyC, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.D, Modifier.None, Action.BuildMenuKeyD, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.E, Modifier.None, Action.BuildMenuKeyE, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.F, Modifier.None, Action.BuildMenuKeyF, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.G, Modifier.None, Action.BuildMenuKeyG, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.H, Modifier.None, Action.BuildMenuKeyH, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.I, Modifier.None, Action.BuildMenuKeyI, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.J, Modifier.None, Action.BuildMenuKeyJ, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.K, Modifier.None, Action.BuildMenuKeyK, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.L, Modifier.None, Action.BuildMenuKeyL, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.M, Modifier.None, Action.BuildMenuKeyM, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.N, Modifier.None, Action.BuildMenuKeyN, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.O, Modifier.None, Action.BuildMenuKeyO, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.P, Modifier.None, Action.BuildMenuKeyP, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.Q, Modifier.None, Action.BuildMenuKeyQ, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.R, Modifier.None, Action.BuildMenuKeyR, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.S, Modifier.None, Action.BuildMenuKeyS, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.T, Modifier.None, Action.BuildMenuKeyT, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.U, Modifier.None, Action.BuildMenuKeyU, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.V, Modifier.None, Action.BuildMenuKeyV, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.W, Modifier.None, Action.BuildMenuKeyW, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.X, Modifier.None, Action.BuildMenuKeyX, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.Y, Modifier.None, Action.BuildMenuKeyY, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.Z, Modifier.None, Action.BuildMenuKeyZ, rebindable: false, ignore_root_conflicts: true),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.B, Modifier.Shift, Action.SandboxBrush),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.N, Modifier.Shift, Action.SandboxSprinkle),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.F, Modifier.Shift, Action.SandboxFlood),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.K, Modifier.Shift, Action.SandboxSample),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.H, Modifier.Shift, Action.SandboxHeatGun),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.J, Modifier.Shift, Action.SandboxRadsTool),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.C, Modifier.Shift, Action.SandboxClearFloor),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.X, Modifier.Shift, Action.SandboxDestroy),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.E, Modifier.Shift, Action.SandboxSpawnEntity),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.S, Modifier.Shift, Action.ToggleSandboxTools),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.R, Modifier.Shift, Action.SandboxReveal),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.Z, Modifier.Shift, Action.SandboxCritterTool),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.Mouse0, Modifier.Ctrl, Action.SandboxCopyElement),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.Backtick, Action.SwitchActiveWorld1),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.Backtick, Action.SwitchActiveWorld2),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.Backtick, Action.SwitchActiveWorld3),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.Backtick, Action.SwitchActiveWorld4),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.Backtick, Action.SwitchActiveWorld5),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha6, Modifier.Backtick, Action.SwitchActiveWorld6),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha7, Modifier.Backtick, Action.SwitchActiveWorld7),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha8, Modifier.Backtick, Action.SwitchActiveWorld8),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.Backtick, Action.SwitchActiveWorld9),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.Backtick, Action.SwitchActiveWorld10)
		};
		IList<BuildMenu.DisplayInfo> list2 = (IList<BuildMenu.DisplayInfo>)BuildMenu.OrderedBuildings.data;
		if (BuildMenu.UseHotkeyBuildMenu() && hotKeyBuildMenuPermitted)
		{
			foreach (BuildMenu.DisplayInfo item in list2)
			{
				AddBindings(HashedString.Invalid, item, list);
			}
		}
		return list.ToArray();
	}

	private static void AddBindings(HashedString parent_category, BuildMenu.DisplayInfo display_info, List<BindingEntry> bindings)
	{
		if (display_info.data == null)
		{
			return;
		}
		Type type = display_info.data.GetType();
		if (typeof(IList<BuildMenu.DisplayInfo>).IsAssignableFrom(type))
		{
			IList<BuildMenu.DisplayInfo> list = (IList<BuildMenu.DisplayInfo>)display_info.data;
			foreach (BuildMenu.DisplayInfo item2 in list)
			{
				AddBindings(display_info.category, item2, bindings);
			}
		}
		else if (typeof(IList<BuildMenu.BuildingInfo>).IsAssignableFrom(type))
		{
			string str = HashCache.Get().Get(parent_category);
			TextInfo textInfo = new CultureInfo("en-US", useUserOverride: false).TextInfo;
			string group = textInfo.ToTitleCase(str) + " Menu";
			BindingEntry item = new BindingEntry(group, GamepadButton.NumButtons, display_info.keyCode, Modifier.None, display_info.hotkey, rebindable: true, ignore_root_conflicts: true);
			bindings.Add(item);
		}
	}

	private void Awake()
	{
		KCrashReporter crash_reporter = GetComponent<KCrashReporter>();
		if ((crash_reporter != null) & (SceneInitializerLoader.ReportDeferredError == null))
		{
			SceneInitializerLoader.ReportDeferredError = delegate(SceneInitializerLoader.DeferredError deferred_error)
			{
				crash_reporter.ShowDialog(deferred_error.msg, deferred_error.stack_trace);
			};
		}
		globalCanvas = GameObject.Find("Canvas");
		UnityEngine.Object.DontDestroyOnLoad(globalCanvas.gameObject);
		OutputSystemInfo();
		Debug.Assert(Instance == null);
		Instance = this;
		Debug.Log("Initializing at " + System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"));
		Debug.Log("Save path: " + Util.RootFolder());
		MyCmp.Init();
		MySmi.Init();
		if (forcedAtlasInitializationList != null)
		{
			SpriteAtlas[] array = forcedAtlasInitializationList;
			foreach (SpriteAtlas spriteAtlas in array)
			{
				int spriteCount = spriteAtlas.spriteCount;
				Sprite[] array2 = new Sprite[spriteCount];
				spriteAtlas.GetSprites(array2);
				Sprite[] array3 = array2;
				foreach (Sprite sprite in array3)
				{
					Texture2D texture = sprite.texture;
					if (texture != null)
					{
						texture.filterMode = FilterMode.Bilinear;
						texture.anisoLevel = 4;
						texture.mipMapBias = 0f;
					}
				}
			}
		}
		FileSystem.Initialize();
		Singleton<StateMachineUpdater>.CreateInstance();
		Singleton<StateMachineManager>.CreateInstance();
		Localization.RegisterForTranslation(typeof(UI));
		modManager = new KMod.Manager();
		modManager.Load(Content.DLL);
		modManager.Load(Content.Strings);
		KSerialization.Manager.Initialize();
		mInputManager = new GameInputManager(GenerateDefaultBindings());
		Audio.Get();
		KAnimBatchManager.CreateInstance();
		Singleton<SoundEventVolumeCache>.CreateInstance();
		mAnimEventManager = new AnimEventManager();
		Singleton<KBatchedAnimUpdater>.CreateInstance();
		DistributionPlatform.Initialize();
		Localization.Initialize();
		modManager.Load(Content.Translation);
		modManager.distribution_platforms.Add(new Local("Local", Label.DistributionPlatform.Local));
		modManager.distribution_platforms.Add(new Local("Dev", Label.DistributionPlatform.Dev));
		mainThread = Thread.CurrentThread;
		KProfiler.main_thread = Thread.CurrentThread;
		RestoreLegacyMetricsSetting();
		TestDataLocations();
		DistributionPlatform.onExitRequest += OnExitRequest;
		DistributionPlatform.onDlcAuthenticationFailed += OnDlcAuthenticationFailed;
		if (DistributionPlatform.Initialized)
		{
			if (!KPrivacyPrefs.instance.disableDataCollection)
			{
				Debug.Log(string.Concat("Logged into ", DistributionPlatform.Inst.Name, " with ID:", DistributionPlatform.Inst.LocalUser.Id, ", NAME:", DistributionPlatform.Inst.LocalUser.Name));
				ThreadedHttps<KleiAccount>.Instance.AuthenticateUser(OnGetUserIdKey);
			}
		}
		else
		{
			Debug.LogWarning("Can't init " + DistributionPlatform.Inst.Name + " distribution platform...");
			OnGetUserIdKey();
		}
		modManager.Load(Content.LayerableFiles);
		GlobalResources.Instance();
	}

	private void OnExitRequest()
	{
		bool flag = true;
		if (Game.Instance != null)
		{
			string filename = SaveLoader.GetActiveSaveFilePath();
			if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
			{
				flag = false;
				GameObject gameObject = KScreenManager.AddChild(globalCanvas, ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject);
				KScreen component = gameObject.GetComponent<KScreen>();
				component.Activate();
				ConfirmDialogScreen component2 = component.GetComponent<ConfirmDialogScreen>();
				component2.PopupConfirmDialog(string.Format(UI.FRONTEND.RAILFORCEQUIT.SAVE_EXIT, Path.GetFileNameWithoutExtension(filename)), delegate
				{
					SaveLoader.Instance.Save(filename);
					App.Quit();
				}, delegate
				{
					App.Quit();
				});
			}
		}
		if (flag)
		{
			GameObject gameObject2 = KScreenManager.AddChild(globalCanvas, ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject);
			KScreen component3 = gameObject2.GetComponent<KScreen>();
			component3.Activate();
			ConfirmDialogScreen component4 = component3.GetComponent<ConfirmDialogScreen>();
			component4.PopupConfirmDialog(UI.FRONTEND.RAILFORCEQUIT.WARN_EXIT, delegate
			{
				App.Quit();
			}, null);
		}
	}

	private void OnDlcAuthenticationFailed()
	{
		GameObject gameObject = KScreenManager.AddChild(globalCanvas, ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject);
		KScreen component = gameObject.GetComponent<KScreen>();
		component.Activate();
		ConfirmDialogScreen component2 = component.GetComponent<ConfirmDialogScreen>();
		component2.deactivateOnCancelAction = false;
		component2.PopupConfirmDialog(UI.FRONTEND.RAILFORCEQUIT.DLC_NOT_PURCHASED, delegate
		{
			App.Quit();
		}, null);
	}

	private void RestoreLegacyMetricsSetting()
	{
		if (KPlayerPrefs.GetInt("ENABLE_METRICS", 1) == 0)
		{
			KPlayerPrefs.DeleteKey("ENABLE_METRICS");
			KPlayerPrefs.Save();
			KPrivacyPrefs.instance.disableDataCollection = true;
			KPrivacyPrefs.Save();
		}
	}

	private void TestDataLocations()
	{
	}

	public GameInputManager GetInputManager()
	{
		return mInputManager;
	}

	public AnimEventManager GetAnimEventManager()
	{
		if (App.IsExiting)
		{
			return null;
		}
		return mAnimEventManager;
	}

	private void OnApplicationFocus(bool focus)
	{
		if (mInputManager != null)
		{
			mInputManager.OnApplicationFocus(focus);
		}
	}

	private void OnGetUserIdKey()
	{
		gotKleiUserID = true;
	}

	private void Update()
	{
		mInputManager.Update();
		if (mAnimEventManager != null)
		{
			mAnimEventManager.Update();
		}
		if (DistributionPlatform.Initialized && !updated_with_initialized_distribution_platform)
		{
			updated_with_initialized_distribution_platform = true;
			SteamUGCService.Initialize();
			Steam steam = new Steam();
			SteamUGCService.Instance.AddClient(steam);
			modManager.distribution_platforms.Add(steam);
			SteamAchievementService.Initialize();
		}
		if (gotKleiUserID)
		{
			gotKleiUserID = false;
			ThreadedHttps<KleiMetrics>.Instance.SetCallBacks(SetONIStaticSessionVariables, SetONIDynamicSessionVariables);
			ThreadedHttps<KleiMetrics>.Instance.StartSession();
		}
		ThreadedHttps<KleiMetrics>.Instance.SetLastUserAction(KInputManager.lastUserActionTicks);
		Localization.VerifyTranslationModSubscription(globalCanvas);
	}

	private void SetONIStaticSessionVariables()
	{
		ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable("Branch", "release");
		ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable("Build", 466654u);
		if (KPlayerPrefs.HasKey(UnitConfigurationScreen.MassUnitKey))
		{
			ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(UnitConfigurationScreen.MassUnitKey, ((GameUtil.MassUnit)KPlayerPrefs.GetInt(UnitConfigurationScreen.MassUnitKey)).ToString());
		}
		if (KPlayerPrefs.HasKey(UnitConfigurationScreen.TemperatureUnitKey))
		{
			ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(UnitConfigurationScreen.TemperatureUnitKey, ((GameUtil.TemperatureUnit)KPlayerPrefs.GetInt(UnitConfigurationScreen.TemperatureUnitKey)).ToString());
		}
		Localization.SelectedLanguageType selectedLanguageType = Localization.GetSelectedLanguageType();
		ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(LanguageCodeKey, Localization.GetCurrentLanguageCode());
		if (selectedLanguageType == Localization.SelectedLanguageType.UGC)
		{
			ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(LanguageModKey, LanguageOptionsScreen.GetSavedLanguageMod());
		}
	}

	private void SetONIDynamicSessionVariables(Dictionary<string, object> data)
	{
		if (Game.Instance != null && GameClock.Instance != null)
		{
			data.Add("GameTimeSeconds", (int)GameClock.Instance.GetTime());
			data.Add("WasDebugEverUsed", Game.Instance.debugWasUsed);
			data.Add("IsSandboxEnabled", SaveGame.Instance.sandboxEnabled);
		}
	}

	private void LateUpdate()
	{
		Singleton<KBatchedAnimUpdater>.Instance.LateUpdate();
	}

	private void OnDestroy()
	{
		if (modManager != null)
		{
			modManager.Shutdown();
		}
		Instance = null;
		if (mAnimEventManager != null)
		{
			mAnimEventManager.FreeResources();
		}
		Singleton<KBatchedAnimUpdater>.DestroyInstance();
	}

	private void OnApplicationQuit()
	{
		KGlobalAnimParser.DestroyInstance();
		ThreadedHttps<KleiMetrics>.Instance.EndSession();
	}

	private void OutputSystemInfo()
	{
		try
		{
			Console.WriteLine("SYSTEM INFO:");
			Dictionary<string, object> hardwareStats = KleiMetrics.GetHardwareStats();
			foreach (KeyValuePair<string, object> item in hardwareStats)
			{
				try
				{
					Console.WriteLine($"    {item.Key.ToString()}={item.Value.ToString()}");
				}
				catch
				{
				}
			}
			Console.WriteLine(string.Format("    {0}={1}", "System Language", Application.systemLanguage.ToString()));
		}
		catch
		{
		}
	}
}
