using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ToolMenu : KScreen
{
	public class ToolInfo
	{
		public string text;

		public string icon;

		public Action hotkey;

		public string toolName;

		public ToolCollection collection;

		public string tooltip;

		public KToggle toggle;

		public Action<object> onSelectCallback;

		public object toolData;

		public ToolInfo(string text, string icon_name, Action hotkey, string ToolName, ToolCollection toolCollection, string tooltip = "", Action<object> onSelectCallback = null, object toolData = null)
		{
			this.text = text;
			icon = icon_name;
			this.hotkey = hotkey;
			toolName = ToolName;
			collection = toolCollection;
			toolCollection.tools.Add(this);
			this.tooltip = tooltip;
			this.onSelectCallback = onSelectCallback;
			this.toolData = toolData;
		}
	}

	public class ToolCollection
	{
		public string text;

		public string icon;

		public string tooltip;

		public bool useInfoMenu;

		public bool largeIcon;

		public GameObject toggle;

		public List<ToolInfo> tools = new List<ToolInfo>();

		public GameObject UIMenuDisplay;

		public GameObject MaskContainer;

		public Action hotkey;

		public ToolCollection(string text, string icon_name, string tooltip = "", bool useInfoMenu = false, Action hotkey = Action.NumActions, bool largeIcon = false)
		{
			this.text = text;
			icon = icon_name;
			this.tooltip = tooltip;
			this.useInfoMenu = useInfoMenu;
			this.hotkey = hotkey;
			this.largeIcon = largeIcon;
		}
	}

	public struct CellColorData
	{
		public int cell;

		public Color color;

		public CellColorData(int cell, Color color)
		{
			this.cell = cell;
			this.color = color;
		}
	}

	public static ToolMenu Instance;

	public GameObject Prefab_collectionContainer;

	public GameObject Prefab_collectionContainerWindow;

	public PriorityScreen Prefab_priorityScreen;

	public GameObject toolIconPrefab;

	public GameObject toolIconLargePrefab;

	public GameObject sandboxToolIconPrefab;

	public GameObject collectionIconPrefab;

	public GameObject prefabToolRow;

	public GameObject largeToolSet;

	public GameObject smallToolSet;

	public GameObject smallToolBottomRow;

	public GameObject smallToolTopRow;

	public GameObject sandboxToolSet;

	private PriorityScreen priorityScreen;

	public ToolParameterMenu toolParameterMenu;

	public GameObject sandboxToolParameterMenu;

	private GameObject toolEffectDisplayPlane;

	private Texture2D toolEffectDisplayPlaneTexture;

	public Material toolEffectDisplayMaterial;

	private byte[] toolEffectDisplayBytes;

	private List<List<ToolCollection>> rows = new List<List<ToolCollection>>();

	public List<ToolCollection> basicTools = new List<ToolCollection>();

	public List<ToolCollection> sandboxTools = new List<ToolCollection>();

	public ToolCollection currentlySelectedCollection;

	public ToolInfo currentlySelectedTool;

	public InterfaceTool activeTool;

	private Coroutine activeOpenAnimationRoutine;

	private Coroutine activeCloseAnimationRoutine;

	private HashSet<Action> boundRootActions = new HashSet<Action>();

	private HashSet<Action> boundSubgroupActions = new HashSet<Action>();

	[SerializeField]
	public TextStyleSetting ToggleToolTipTextStyleSetting;

	[SerializeField]
	public TextStyleSetting CategoryLabelTextStyle_LeftAlign;

	private int smallCollectionMax = 5;

	private HashSet<CellColorData> colors = new HashSet<CellColorData>();

	public PriorityScreen PriorityScreen => priorityScreen;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	public override float GetSortKey()
	{
		return 5f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		Game.Instance.Subscribe(1798162660, OnOverlayChanged);
		priorityScreen = Util.KInstantiateUI<PriorityScreen>(Prefab_priorityScreen.gameObject, base.gameObject);
		priorityScreen.InstantiateButtons(OnPriorityClicked, playSelectionSound: false);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Game.Instance.Unsubscribe(1798162660, OnOverlayChanged);
	}

	private void OnOverlayChanged(object overlay_data)
	{
		HashedString hashedString = (HashedString)overlay_data;
		if (PlayerController.Instance.ActiveTool != null && PlayerController.Instance.ActiveTool.ViewMode != OverlayModes.None.ID && PlayerController.Instance.ActiveTool.ViewMode != hashedString)
		{
			ChooseCollection(null);
			ChooseTool(null);
		}
	}

	protected override void OnSpawn()
	{
		activateOnSpawn = true;
		base.OnSpawn();
		CreateSandBoxTools();
		CreateBasicTools();
		rows.Add(sandboxTools);
		rows.Add(basicTools);
		rows.ForEach(delegate(List<ToolCollection> row)
		{
			InstantiateCollectionsUI(row);
		});
		rows.ForEach(delegate(List<ToolCollection> row)
		{
			BuildRowToggles(row);
		});
		rows.ForEach(delegate(List<ToolCollection> row)
		{
			BuildToolToggles(row);
		});
		ChooseCollection(null);
		priorityScreen.gameObject.SetActive(value: false);
		ToggleSandboxUI();
		Game.Instance.Subscribe(-1948169901, ToggleSandboxUI);
		ResetToolDisplayPlane();
	}

	private void ResetToolDisplayPlane()
	{
		toolEffectDisplayPlane = CreateToolDisplayPlane("Overlay", World.Instance.transform);
		toolEffectDisplayPlaneTexture = CreatePlaneTexture(out toolEffectDisplayBytes, Grid.WidthInCells, Grid.HeightInCells);
		toolEffectDisplayPlane.GetComponent<Renderer>().sharedMaterial = toolEffectDisplayMaterial;
		toolEffectDisplayPlane.GetComponent<Renderer>().sharedMaterial.mainTexture = toolEffectDisplayPlaneTexture;
		toolEffectDisplayPlane.transform.SetLocalPosition(new Vector3(Grid.WidthInMeters / 2f, Grid.HeightInMeters / 2f, -6f));
		RefreshToolDisplayPlaneColor();
	}

	private GameObject CreateToolDisplayPlane(string layer, Transform parent)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
		gameObject.name = "toolEffectDisplayPlane";
		gameObject.SetLayerRecursively(LayerMask.NameToLayer(layer));
		UnityEngine.Object.Destroy(gameObject.GetComponent<Collider>());
		if (parent != null)
		{
			gameObject.transform.SetParent(parent);
		}
		gameObject.transform.SetPosition(Vector3.zero);
		gameObject.transform.localScale = new Vector3(Grid.WidthInMeters / -10f, 1f, Grid.HeightInMeters / -10f);
		gameObject.transform.eulerAngles = new Vector3(270f, 0f, 0f);
		gameObject.GetComponent<MeshRenderer>().reflectionProbeUsage = ReflectionProbeUsage.Off;
		return gameObject;
	}

	private Texture2D CreatePlaneTexture(out byte[] textureBytes, int width, int height)
	{
		textureBytes = new byte[width * height * 4];
		return new Texture2D(width, height, TextureUtil.TextureFormatToGraphicsFormat(TextureFormat.RGBA32), TextureCreationFlags.None)
		{
			name = "toolEffectDisplayPlane",
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
	}

	private void Update()
	{
		RefreshToolDisplayPlaneColor();
	}

	private void RefreshToolDisplayPlaneColor()
	{
		if (PlayerController.Instance.ActiveTool == null || PlayerController.Instance.ActiveTool == SelectTool.Instance)
		{
			toolEffectDisplayPlane.SetActive(value: false);
			return;
		}
		PlayerController.Instance.ActiveTool.GetOverlayColorData(out colors);
		Array.Clear(toolEffectDisplayBytes, 0, toolEffectDisplayBytes.Length);
		if (colors != null)
		{
			foreach (CellColorData color in colors)
			{
				if (Grid.IsValidCell(color.cell))
				{
					int num = color.cell * 4;
					if (num >= 0)
					{
						toolEffectDisplayBytes[num] = (byte)(Mathf.Min(color.color.r, 1f) * 255f);
						toolEffectDisplayBytes[num + 1] = (byte)(Mathf.Min(color.color.g, 1f) * 255f);
						toolEffectDisplayBytes[num + 2] = (byte)(Mathf.Min(color.color.b, 1f) * 255f);
						toolEffectDisplayBytes[num + 3] = (byte)(Mathf.Min(color.color.a, 1f) * 255f);
					}
				}
			}
		}
		if (!toolEffectDisplayPlane.activeSelf)
		{
			toolEffectDisplayPlane.SetActive(value: true);
		}
		toolEffectDisplayPlaneTexture.LoadRawTextureData(toolEffectDisplayBytes);
		toolEffectDisplayPlaneTexture.Apply();
	}

	public void ToggleSandboxUI(object data = null)
	{
		ClearSelection();
		PlayerController.Instance.ActivateTool(SelectTool.Instance);
		sandboxTools[0].toggle.transform.parent.transform.parent.gameObject.SetActive(Game.Instance.SandboxModeActive);
	}

	public static ToolCollection CreateToolCollection(LocString collection_name, string icon_name, Action hotkey, string tool_name, LocString tooltip, bool largeIcon)
	{
		ToolCollection toolCollection = new ToolCollection(collection_name, icon_name, "", useInfoMenu: false, Action.NumActions, largeIcon);
		new ToolInfo(collection_name, icon_name, hotkey, tool_name, toolCollection, tooltip);
		return toolCollection;
	}

	private void CreateSandBoxTools()
	{
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.BRUSH.NAME, "brush", Action.SandboxBrush, "SandboxBrushTool", UI.SANDBOXTOOLS.SETTINGS.BRUSH.TOOLTIP, largeIcon: false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.SPRINKLE.NAME, "sprinkle", Action.SandboxSprinkle, "SandboxSprinkleTool", UI.SANDBOXTOOLS.SETTINGS.SPRINKLE.TOOLTIP, largeIcon: false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.FLOOD.NAME, "flood", Action.SandboxFlood, "SandboxFloodTool", UI.SANDBOXTOOLS.SETTINGS.FLOOD.TOOLTIP, largeIcon: false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.SAMPLE.NAME, "sample", Action.SandboxSample, "SandboxSampleTool", UI.SANDBOXTOOLS.SETTINGS.SAMPLE.TOOLTIP, largeIcon: false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.HEATGUN.NAME, "temperature", Action.SandboxHeatGun, "SandboxHeatTool", UI.SANDBOXTOOLS.SETTINGS.HEATGUN.TOOLTIP, largeIcon: false));
		if (DlcManager.FeatureRadiationEnabled())
		{
			sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.RADSTOOL.NAME, "radiation", Action.SandboxRadsTool, "SandboxRadsTool", UI.SANDBOXTOOLS.SETTINGS.RADSTOOL.TOOLTIP, largeIcon: false));
		}
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.SPAWNER.NAME, "spawn", Action.SandboxSpawnEntity, "SandboxSpawnerTool", UI.SANDBOXTOOLS.SETTINGS.SPAWNER.TOOLTIP, largeIcon: false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.CLEAR_FLOOR.NAME, "clear_floor", Action.SandboxClearFloor, "SandboxClearFloorTool", UI.SANDBOXTOOLS.SETTINGS.CLEAR_FLOOR.TOOLTIP, largeIcon: false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.DESTROY.NAME, "destroy", Action.SandboxDestroy, "SandboxDestroyerTool", UI.SANDBOXTOOLS.SETTINGS.DESTROY.TOOLTIP, largeIcon: false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.FOW.NAME, "reveal", Action.SandboxReveal, "SandboxFOWTool", UI.SANDBOXTOOLS.SETTINGS.FOW.TOOLTIP, largeIcon: false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.CRITTER.NAME, "critter", Action.SandboxCritterTool, "SandboxCritterTool", UI.SANDBOXTOOLS.SETTINGS.CRITTER.TOOLTIP, largeIcon: false));
	}

	private void CreateBasicTools()
	{
		basicTools.Add(CreateToolCollection(UI.TOOLS.DIG.NAME, "icon_action_dig", Action.Dig, "DigTool", UI.TOOLTIPS.DIGBUTTON, largeIcon: true));
		basicTools.Add(CreateToolCollection(UI.TOOLS.CANCEL.NAME, "icon_action_cancel", Action.BuildingCancel, "CancelTool", UI.TOOLTIPS.CANCELBUTTON, largeIcon: true));
		basicTools.Add(CreateToolCollection(UI.TOOLS.DECONSTRUCT.NAME, "icon_action_deconstruct", Action.BuildingDeconstruct, "DeconstructTool", UI.TOOLTIPS.DECONSTRUCTBUTTON, largeIcon: true));
		basicTools.Add(CreateToolCollection(UI.TOOLS.PRIORITIZE.NAME, "icon_action_prioritize", Action.Prioritize, "PrioritizeTool", UI.TOOLTIPS.PRIORITIZEBUTTON, largeIcon: true));
		basicTools.Add(CreateToolCollection(UI.TOOLS.DISINFECT.NAME, "icon_action_disinfect", Action.Disinfect, "DisinfectTool", UI.TOOLTIPS.DISINFECTBUTTON, largeIcon: false));
		basicTools.Add(CreateToolCollection(UI.TOOLS.MARKFORSTORAGE.NAME, "icon_action_store", Action.Clear, "ClearTool", UI.TOOLTIPS.CLEARBUTTON, largeIcon: false));
		basicTools.Add(CreateToolCollection(UI.TOOLS.ATTACK.NAME, "icon_action_attack", Action.Attack, "AttackTool", UI.TOOLTIPS.ATTACKBUTTON, largeIcon: false));
		basicTools.Add(CreateToolCollection(UI.TOOLS.MOP.NAME, "icon_action_mop", Action.Mop, "MopTool", UI.TOOLTIPS.MOPBUTTON, largeIcon: false));
		basicTools.Add(CreateToolCollection(UI.TOOLS.CAPTURE.NAME, "icon_action_capture", Action.Capture, "CaptureTool", UI.TOOLTIPS.CAPTUREBUTTON, largeIcon: false));
		basicTools.Add(CreateToolCollection(UI.TOOLS.HARVEST.NAME, "icon_action_harvest", Action.Harvest, "HarvestTool", UI.TOOLTIPS.HARVESTBUTTON, largeIcon: false));
		basicTools.Add(CreateToolCollection(UI.TOOLS.EMPTY_PIPE.NAME, "icon_action_empty_pipes", Action.EmptyPipe, "EmptyPipeTool", UI.TOOLS.EMPTY_PIPE.TOOLTIP, largeIcon: false));
	}

	private void InstantiateCollectionsUI(IList<ToolCollection> collections)
	{
		GameObject parent = Util.KInstantiateUI(prefabToolRow, base.gameObject, force_active: true);
		GameObject gameObject = Util.KInstantiateUI(largeToolSet, parent, force_active: true);
		GameObject parent2 = Util.KInstantiateUI(smallToolSet, parent, force_active: true);
		GameObject gameObject2 = Util.KInstantiateUI(smallToolBottomRow, parent2, force_active: true);
		GameObject gameObject3 = Util.KInstantiateUI(smallToolTopRow, parent2, force_active: true);
		GameObject gameObject4 = Util.KInstantiateUI(sandboxToolSet, parent, force_active: true);
		bool flag = true;
		for (int i = 0; i < collections.Count; i++)
		{
			GameObject parent3;
			if (collections == sandboxTools)
			{
				parent3 = gameObject4;
			}
			else if (collections[i].largeIcon)
			{
				parent3 = gameObject;
			}
			else
			{
				parent3 = (flag ? gameObject3 : gameObject2);
				flag = !flag;
			}
			ToolCollection tc = collections[i];
			tc.toggle = Util.KInstantiateUI((collections[i].tools.Count > 1) ? collectionIconPrefab : ((collections == sandboxTools) ? sandboxToolIconPrefab : (collections[i].largeIcon ? toolIconLargePrefab : toolIconPrefab)), parent3, force_active: true);
			KToggle component = tc.toggle.GetComponent<KToggle>();
			component.soundPlayer.Enabled = false;
			component.onClick += delegate
			{
				if (currentlySelectedCollection == tc && tc.tools.Count >= 1)
				{
					KMonoBehaviour.PlaySound(GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound()));
				}
				ChooseCollection(tc);
			};
			if (tc.tools == null)
			{
				continue;
			}
			GameObject gameObject5 = null;
			if (tc.tools.Count < smallCollectionMax)
			{
				gameObject5 = Util.KInstantiateUI(Prefab_collectionContainer, parent3, force_active: true);
				gameObject5.transform.SetSiblingIndex(gameObject5.transform.GetSiblingIndex() - 1);
				gameObject5.transform.localScale = Vector3.one;
				gameObject5.rectTransform().sizeDelta = new Vector2(tc.tools.Count * 75, 50f);
				tc.MaskContainer = gameObject5.GetComponentInChildren<Mask>().gameObject;
				gameObject5.SetActive(value: false);
			}
			else
			{
				gameObject5 = Util.KInstantiateUI(Prefab_collectionContainerWindow, parent3, force_active: true);
				gameObject5.transform.localScale = Vector3.one;
				gameObject5.GetComponentInChildren<LocText>().SetText(tc.text.ToUpper());
				tc.MaskContainer = gameObject5.GetComponentInChildren<GridLayoutGroup>().gameObject;
				gameObject5.SetActive(value: false);
			}
			tc.UIMenuDisplay = gameObject5;
			for (int j = 0; j < tc.tools.Count; j++)
			{
				ToolInfo ti = tc.tools[j];
				GameObject gameObject6 = Util.KInstantiateUI((collections == sandboxTools) ? sandboxToolIconPrefab : (collections[i].largeIcon ? toolIconLargePrefab : toolIconPrefab), tc.MaskContainer, force_active: true);
				gameObject6.name = ti.text;
				ti.toggle = gameObject6.GetComponent<KToggle>();
				if (ti.collection.tools.Count > 1)
				{
					RectTransform rectTransform = null;
					rectTransform = ti.toggle.gameObject.GetComponentInChildren<SetTextStyleSetting>().rectTransform();
					if (gameObject6.name.Length > 12)
					{
						rectTransform.GetComponent<SetTextStyleSetting>().SetStyle(CategoryLabelTextStyle_LeftAlign);
						rectTransform.anchoredPosition = new Vector2(16f, rectTransform.anchoredPosition.y);
					}
				}
				ti.toggle.onClick += delegate
				{
					ChooseTool(ti);
				};
				tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapse(delegate
				{
					SetToggleState(tc.toggle.GetComponent<KToggle>(), state: false);
					tc.UIMenuDisplay.SetActive(value: false);
				});
			}
		}
		if (gameObject.transform.childCount == 0)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		if (gameObject2.transform.childCount == 0 && gameObject3.transform.childCount == 0)
		{
			UnityEngine.Object.Destroy(parent2);
		}
		if (gameObject4.transform.childCount == 0)
		{
			UnityEngine.Object.Destroy(gameObject4);
		}
	}

	private void ChooseTool(ToolInfo tool)
	{
		if (currentlySelectedTool == tool)
		{
			return;
		}
		if (currentlySelectedTool != tool)
		{
			currentlySelectedTool = tool;
			if (currentlySelectedTool != null && currentlySelectedTool.onSelectCallback != null)
			{
				currentlySelectedTool.onSelectCallback(currentlySelectedTool);
			}
		}
		if (currentlySelectedTool != null)
		{
			currentlySelectedCollection = currentlySelectedTool.collection;
			InterfaceTool[] tools = PlayerController.Instance.tools;
			foreach (InterfaceTool interfaceTool in tools)
			{
				if (currentlySelectedTool.toolName == interfaceTool.name)
				{
					UISounds.PlaySound(UISounds.Sound.ClickObject);
					activeTool = interfaceTool;
					PlayerController.Instance.ActivateTool(interfaceTool);
					break;
				}
			}
		}
		else
		{
			PlayerController.Instance.ActivateTool(SelectTool.Instance);
		}
		rows.ForEach(delegate(List<ToolCollection> row)
		{
			RefreshRowDisplay(row);
		});
	}

	private void RefreshRowDisplay(IList<ToolCollection> row)
	{
		for (int i = 0; i < row.Count; i++)
		{
			ToolCollection tc = row[i];
			if (currentlySelectedTool != null && currentlySelectedTool.collection == tc)
			{
				if (!tc.UIMenuDisplay.activeSelf || tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapsing)
				{
					if (tc.tools.Count > 1)
					{
						tc.UIMenuDisplay.SetActive(value: true);
						if (tc.tools.Count < smallCollectionMax)
						{
							float speedScale = Mathf.Clamp(1f - (float)tc.tools.Count * 0.15f, 0.5f, 1f);
							tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().speedScale = speedScale;
						}
						tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Expand(delegate
						{
							SetToggleState(tc.toggle.GetComponent<KToggle>(), state: true);
						});
					}
					else
					{
						currentlySelectedTool = tc.tools[0];
					}
				}
			}
			else if (tc.UIMenuDisplay.activeSelf && !tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapsing && tc.tools.Count > 0)
			{
				tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapse(delegate
				{
					SetToggleState(tc.toggle.GetComponent<KToggle>(), state: false);
					tc.UIMenuDisplay.SetActive(value: false);
				});
			}
			for (int j = 0; j < tc.tools.Count; j++)
			{
				if (tc.tools[j] == currentlySelectedTool)
				{
					SetToggleState(tc.tools[j].toggle, state: true);
				}
				else
				{
					SetToggleState(tc.tools[j].toggle, state: false);
				}
			}
		}
	}

	public void TurnLargeCollectionOff()
	{
		if (currentlySelectedCollection != null && currentlySelectedCollection.tools.Count > smallCollectionMax)
		{
			ChooseCollection(null);
		}
	}

	private void ChooseCollection(ToolCollection collection, bool autoSelectTool = true)
	{
		if (collection == currentlySelectedCollection)
		{
			if (collection != null && collection.tools.Count > 1)
			{
				currentlySelectedCollection = null;
				if (currentlySelectedTool != null)
				{
					ChooseTool(null);
				}
			}
			else if (currentlySelectedTool != null && currentlySelectedCollection.tools.Contains(currentlySelectedTool) && currentlySelectedCollection.tools.Count == 1)
			{
				currentlySelectedCollection = null;
				ChooseTool(null);
			}
		}
		else
		{
			currentlySelectedCollection = collection;
		}
		rows.ForEach(delegate(List<ToolCollection> row)
		{
			OpenOrCloseCollectionsInRow(row);
		});
	}

	private void OpenOrCloseCollectionsInRow(IList<ToolCollection> row, bool autoSelectTool = true)
	{
		for (int i = 0; i < row.Count; i++)
		{
			ToolCollection tc = row[i];
			if (currentlySelectedCollection == tc)
			{
				if ((currentlySelectedCollection.tools != null && currentlySelectedCollection.tools.Count == 1) || autoSelectTool)
				{
					ChooseTool(currentlySelectedCollection.tools[0]);
				}
			}
			else if (tc.UIMenuDisplay.activeSelf && !tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapsing)
			{
				tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapse(delegate
				{
					SetToggleState(tc.toggle.GetComponent<KToggle>(), state: false);
					tc.UIMenuDisplay.SetActive(value: false);
				});
			}
			SetToggleState(tc.toggle.GetComponent<KToggle>(), currentlySelectedCollection == tc);
		}
	}

	private void SetToggleState(KToggle toggle, bool state)
	{
		if (state)
		{
			toggle.Select();
			toggle.isOn = true;
		}
		else
		{
			toggle.Deselect();
			toggle.isOn = false;
		}
	}

	public void ClearSelection()
	{
		if (currentlySelectedCollection != null)
		{
			ChooseCollection(null);
		}
		if (currentlySelectedTool != null)
		{
			ChooseTool(null);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (e.IsAction(Action.ToggleSandboxTools))
			{
				if (Application.isEditor)
				{
					DebugUtil.LogArgs("Force-enabling sandbox mode because we're in editor.");
					SaveGame.Instance.sandboxEnabled = true;
				}
				if (SaveGame.Instance.sandboxEnabled)
				{
					Game.Instance.SandboxModeActive = !Game.Instance.SandboxModeActive;
				}
			}
			foreach (List<ToolCollection> row in rows)
			{
				if (row == sandboxTools && !Game.Instance.SandboxModeActive)
				{
					continue;
				}
				for (int i = 0; i < row.Count; i++)
				{
					Action toolHotkey = row[i].hotkey;
					if (toolHotkey != Action.NumActions && e.IsAction(toolHotkey) && (currentlySelectedCollection == null || (currentlySelectedCollection != null && currentlySelectedCollection.tools.Find((ToolInfo t) => GameInputMapping.CompareActionKeyCodes(t.hotkey, toolHotkey)) == null)))
					{
						if (currentlySelectedCollection != row[i])
						{
							ChooseCollection(row[i], autoSelectTool: false);
							ChooseTool(row[i].tools[0]);
						}
						else if (currentlySelectedCollection.tools.Count > 1)
						{
							e.Consumed = true;
							ChooseCollection(null);
							ChooseTool(null);
							string sound = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound());
							if (sound != null)
							{
								KMonoBehaviour.PlaySound(sound);
							}
						}
						break;
					}
					for (int j = 0; j < row[i].tools.Count; j++)
					{
						if ((currentlySelectedCollection != null || row[i].tools.Count != 1) && currentlySelectedCollection != row[i] && (currentlySelectedCollection == null || currentlySelectedCollection.tools.Count != 1 || row[i].tools.Count != 1))
						{
							continue;
						}
						Action hotkey = row[i].tools[j].hotkey;
						if (e.IsAction(hotkey) && e.TryConsume(hotkey))
						{
							if (row[i].tools.Count == 1 && currentlySelectedCollection != row[i])
							{
								ChooseCollection(row[i], autoSelectTool: false);
							}
							else if (currentlySelectedTool != row[i].tools[j])
							{
								ChooseTool(row[i].tools[j]);
							}
						}
						else if (GameInputMapping.CompareActionKeyCodes(e.GetAction(), hotkey))
						{
							e.Consumed = true;
						}
					}
				}
			}
			if ((currentlySelectedTool != null || currentlySelectedCollection != null) && !e.Consumed)
			{
				if (e.TryConsume(Action.Escape))
				{
					string sound2 = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound());
					if (sound2 != null)
					{
						KMonoBehaviour.PlaySound(sound2);
					}
					if (currentlySelectedCollection != null)
					{
						ChooseCollection(null);
					}
					if (currentlySelectedTool != null)
					{
						ChooseTool(null);
					}
					SelectTool.Instance.Activate();
				}
			}
			else if (!PlayerController.Instance.IsUsingDefaultTool() && !e.Consumed && e.TryConsume(Action.Escape))
			{
				SelectTool.Instance.Activate();
			}
		}
		base.OnKeyDown(e);
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if ((currentlySelectedTool != null || currentlySelectedCollection != null) && !e.Consumed)
			{
				if (PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
				{
					string sound = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound());
					if (sound != null)
					{
						KMonoBehaviour.PlaySound(sound);
					}
					if (currentlySelectedCollection != null)
					{
						ChooseCollection(null);
					}
					if (currentlySelectedTool != null)
					{
						ChooseTool(null);
					}
					SelectTool.Instance.Activate();
				}
			}
			else if (!PlayerController.Instance.IsUsingDefaultTool() && !e.Consumed && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
			{
				SelectTool.Instance.Activate();
				string sound2 = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound());
				if (sound2 != null)
				{
					KMonoBehaviour.PlaySound(sound2);
				}
			}
		}
		base.OnKeyUp(e);
	}

	protected void BuildRowToggles(IList<ToolCollection> row)
	{
		for (int i = 0; i < row.Count; i++)
		{
			ToolCollection toolCollection = row[i];
			if (toolCollection.toggle == null)
			{
				continue;
			}
			GameObject toggle = toolCollection.toggle;
			Sprite sprite = Assets.GetSprite(toolCollection.icon);
			if (sprite != null)
			{
				toggle.transform.Find("FG").GetComponent<Image>().sprite = sprite;
			}
			Transform transform = toggle.transform.Find("Text");
			if (transform != null)
			{
				LocText component = transform.GetComponent<LocText>();
				if (component != null)
				{
					component.text = toolCollection.text;
				}
			}
			ToolTip component2 = toggle.GetComponent<ToolTip>();
			if (!component2)
			{
				continue;
			}
			if (row[i].tools.Count == 1)
			{
				string newString = GameUtil.ReplaceHotkeyString(row[i].tools[0].tooltip, row[i].tools[0].hotkey);
				component2.AddMultiStringTooltip(newString, ToggleToolTipTextStyleSetting);
				continue;
			}
			string text = row[i].tooltip;
			if (row[i].hotkey != Action.NumActions)
			{
				text = GameUtil.ReplaceHotkeyString(text, row[i].hotkey);
			}
			component2.AddMultiStringTooltip(text, ToggleToolTipTextStyleSetting);
		}
	}

	protected void BuildToolToggles(IList<ToolCollection> row)
	{
		for (int i = 0; i < row.Count; i++)
		{
			ToolCollection toolCollection = row[i];
			if (toolCollection.toggle == null)
			{
				continue;
			}
			for (int j = 0; j < toolCollection.tools.Count; j++)
			{
				GameObject gameObject = toolCollection.tools[j].toggle.gameObject;
				Sprite sprite = Assets.GetSprite(toolCollection.icon);
				if (sprite != null)
				{
					gameObject.transform.Find("FG").GetComponent<Image>().sprite = sprite;
				}
				Transform transform = gameObject.transform.Find("Text");
				if (transform != null)
				{
					LocText component = transform.GetComponent<LocText>();
					if (component != null)
					{
						component.text = toolCollection.tools[j].text;
					}
				}
				ToolTip component2 = gameObject.GetComponent<ToolTip>();
				if ((bool)component2)
				{
					string newString = ((toolCollection.tools.Count > 1) ? GameUtil.ReplaceHotkeyString(toolCollection.tools[j].tooltip, toolCollection.hotkey, toolCollection.tools[j].hotkey) : GameUtil.ReplaceHotkeyString(toolCollection.tools[j].tooltip, toolCollection.tools[j].hotkey));
					component2.AddMultiStringTooltip(newString, ToggleToolTipTextStyleSetting);
				}
			}
		}
	}

	public bool HasUniqueKeyBindings()
	{
		bool result = true;
		boundRootActions.Clear();
		foreach (List<ToolCollection> row in rows)
		{
			foreach (ToolCollection item in row)
			{
				if (boundRootActions.Contains(item.hotkey))
				{
					result = false;
					break;
				}
				boundRootActions.Add(item.hotkey);
				boundSubgroupActions.Clear();
				foreach (ToolInfo tool in item.tools)
				{
					if (boundSubgroupActions.Contains(tool.hotkey))
					{
						result = false;
						break;
					}
					boundSubgroupActions.Add(tool.hotkey);
				}
			}
		}
		return result;
	}

	private void OnPriorityClicked(PrioritySetting priority)
	{
		priorityScreen.SetScreenPriority(priority);
	}
}
