using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/BuildingCellVisualizer")]
public class BuildingCellVisualizer : KMonoBehaviour
{
	[Flags]
	private enum Ports : byte
	{
		PowerIn = 0x1,
		PowerOut = 0x2,
		GasIn = 0x4,
		GasOut = 0x8,
		LiquidIn = 0x10,
		LiquidOut = 0x20,
		SolidIn = 0x40,
		SolidOut = 0x80
	}

	private BuildingCellVisualizerResources resources;

	[MyCmpReq]
	private Building building;

	[SerializeField]
	public static Color32 secondOutputColour = new Color(251f / 255f, 176f / 255f, 59f / 255f);

	[SerializeField]
	public static Color32 secondInputColour = new Color(251f / 255f, 176f / 255f, 59f / 255f);

	private const Ports POWER_PORTS = Ports.PowerIn | Ports.PowerOut;

	private const Ports GAS_PORTS = Ports.GasIn | Ports.GasOut;

	private const Ports LIQUID_PORTS = Ports.LiquidIn | Ports.LiquidOut;

	private const Ports SOLID_PORTS = Ports.SolidIn | Ports.SolidOut;

	private const Ports MATTER_PORTS = ~(Ports.PowerIn | Ports.PowerOut);

	private Ports ports;

	private Ports secondary_ports;

	private Sprite diseaseSourceSprite;

	private Color32 diseaseSourceColour;

	private GameObject inputVisualizer;

	private GameObject outputVisualizer;

	private GameObject secondaryInputVisualizer;

	private GameObject secondaryOutputVisualizer;

	private bool enableRaycast;

	private Dictionary<GameObject, Image> icons;

	public bool RequiresPowerInput => (ports & Ports.PowerIn) != 0;

	public bool RequiresPowerOutput => (ports & Ports.PowerOut) != 0;

	public bool RequiresPower => (ports & (Ports.PowerIn | Ports.PowerOut)) != 0;

	public bool RequiresGas => (ports & (Ports.GasIn | Ports.GasOut)) != 0;

	public bool RequiresLiquid => (ports & (Ports.LiquidIn | Ports.LiquidOut)) != 0;

	public bool RequiresSolid => (ports & (Ports.SolidIn | Ports.SolidOut)) != 0;

	public bool RequiresUtilityConnection => (ports & ~(Ports.PowerIn | Ports.PowerOut)) != 0;

	public void ConnectedEventWithDelay(float delay, int connectionCount, int cell, string soundName)
	{
		StartCoroutine(ConnectedDelay(delay, connectionCount, cell, soundName));
	}

	private IEnumerator ConnectedDelay(float delay, int connectionCount, int cell, string soundName)
	{
		float startTime = Time.realtimeSinceStartup;
		float currentTime = startTime;
		while (currentTime < startTime + delay)
		{
			currentTime += Time.unscaledDeltaTime;
			yield return new WaitForEndOfFrame();
		}
		ConnectedEvent(cell);
		string sound = GlobalAssets.GetSound(soundName);
		if (sound != null)
		{
			Vector3 position = base.transform.GetPosition();
			position.z = 0f;
			EventInstance instance = SoundEvent.BeginOneShot(sound, position);
			instance.setParameterByName("connectedCount", connectionCount);
			SoundEvent.EndOneShot(instance);
		}
	}

	public void ConnectedEvent(int cell)
	{
		GameObject gameObject = null;
		if (inputVisualizer != null && Grid.PosToCell(inputVisualizer) == cell)
		{
			gameObject = inputVisualizer;
		}
		else if (outputVisualizer != null && Grid.PosToCell(outputVisualizer) == cell)
		{
			gameObject = outputVisualizer;
		}
		else if (secondaryInputVisualizer != null && Grid.PosToCell(secondaryInputVisualizer) == cell)
		{
			gameObject = secondaryInputVisualizer;
		}
		else if (secondaryOutputVisualizer != null && Grid.PosToCell(secondaryOutputVisualizer) == cell)
		{
			gameObject = secondaryOutputVisualizer;
		}
		if (!(gameObject == null))
		{
			SizePulse pulse = gameObject.gameObject.AddComponent<SizePulse>();
			pulse.speed = 20f;
			pulse.multiplier = 0.75f;
			pulse.updateWhenPaused = true;
			SizePulse sizePulse = pulse;
			sizePulse.onComplete = (System.Action)Delegate.Combine(sizePulse.onComplete, (System.Action)delegate
			{
				UnityEngine.Object.Destroy(pulse);
			});
		}
	}

	private void MapBuilding()
	{
		BuildingDef def = building.Def;
		if (def.CheckRequiresPowerInput())
		{
			ports |= Ports.PowerIn;
		}
		if (def.CheckRequiresPowerOutput())
		{
			ports |= Ports.PowerOut;
		}
		if (def.CheckRequiresGasInput())
		{
			ports |= Ports.GasIn;
		}
		if (def.CheckRequiresGasOutput())
		{
			ports |= Ports.GasOut;
		}
		if (def.CheckRequiresLiquidInput())
		{
			ports |= Ports.LiquidIn;
		}
		if (def.CheckRequiresLiquidOutput())
		{
			ports |= Ports.LiquidOut;
		}
		if (def.CheckRequiresSolidInput())
		{
			ports |= Ports.SolidIn;
		}
		if (def.CheckRequiresSolidOutput())
		{
			ports |= Ports.SolidOut;
		}
		DiseaseVisualization.Info info = Assets.instance.DiseaseVisualization.GetInfo(def.DiseaseCellVisName);
		if (info.name != null)
		{
			diseaseSourceSprite = Assets.instance.DiseaseVisualization.overlaySprite;
			diseaseSourceColour = GlobalAssets.Instance.colorSet.GetColorByName(info.overlayColourName);
		}
		ISecondaryInput component = def.BuildingComplete.GetComponent<ISecondaryInput>();
		if (component != null)
		{
			switch (component.GetSecondaryConduitType())
			{
			case ConduitType.Gas:
				secondary_ports |= Ports.GasIn;
				break;
			case ConduitType.Liquid:
				secondary_ports |= Ports.LiquidIn;
				break;
			case ConduitType.Solid:
				secondary_ports |= Ports.SolidIn;
				break;
			}
		}
		ISecondaryOutput component2 = def.BuildingComplete.GetComponent<ISecondaryOutput>();
		if (component2 != null)
		{
			switch (component2.GetSecondaryConduitType())
			{
			case ConduitType.Gas:
				secondary_ports |= Ports.GasOut;
				break;
			case ConduitType.Liquid:
				secondary_ports |= Ports.LiquidOut;
				break;
			case ConduitType.Solid:
				secondary_ports |= Ports.SolidOut;
				break;
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (inputVisualizer != null)
		{
			UnityEngine.Object.Destroy(inputVisualizer);
		}
		if (outputVisualizer != null)
		{
			UnityEngine.Object.Destroy(outputVisualizer);
		}
		if (secondaryInputVisualizer != null)
		{
			UnityEngine.Object.Destroy(secondaryInputVisualizer);
		}
		if (secondaryOutputVisualizer != null)
		{
			UnityEngine.Object.Destroy(secondaryOutputVisualizer);
		}
	}

	private Color GetWireColor(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, 26];
		if (gameObject == null)
		{
			return Color.white;
		}
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		if (!(component != null))
		{
			return Color.white;
		}
		return component.TintColour;
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if (resources == null)
		{
			resources = BuildingCellVisualizerResources.Instance();
		}
		if (icons == null)
		{
			icons = new Dictionary<GameObject, Image>();
		}
		enableRaycast = building as BuildingComplete != null;
		MapBuilding();
		Components.BuildingCellVisualizers.Add(this);
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		Components.BuildingCellVisualizers.Remove(this);
	}

	public void DrawIcons(HashedString mode)
	{
		if (mode == OverlayModes.Power.ID)
		{
			if (RequiresPower)
			{
				bool flag = building as BuildingPreview != null;
				BuildingEnabledButton component = building.GetComponent<BuildingEnabledButton>();
				int powerInputCell = building.GetPowerInputCell();
				if (RequiresPowerInput)
				{
					int circuitID = Game.Instance.circuitManager.GetCircuitID(powerInputCell);
					Color tint = ((component != null && !component.IsEnabled) ? Color.gray : Color.white);
					Sprite icon_img = ((!flag && circuitID != 65535) ? resources.electricityConnectedIcon : resources.electricityInputIcon);
					DrawUtilityIcon(powerInputCell, icon_img, ref inputVisualizer, tint, GetWireColor(powerInputCell), 1f);
				}
				if (RequiresPowerOutput)
				{
					int powerOutputCell = building.GetPowerOutputCell();
					int circuitID2 = Game.Instance.circuitManager.GetCircuitID(powerOutputCell);
					Color color = (building.Def.UseWhitePowerOutputConnectorColour ? Color.white : resources.electricityOutputColor);
					Color32 c = ((component != null && !component.IsEnabled) ? Color.gray : color);
					Sprite icon_img2 = ((!flag && circuitID2 != 65535) ? resources.electricityConnectedIcon : resources.electricityInputIcon);
					DrawUtilityIcon(powerOutputCell, icon_img2, ref outputVisualizer, c, GetWireColor(powerOutputCell), 1f);
				}
				return;
			}
			bool flag2 = true;
			Switch component2 = GetComponent<Switch>();
			if (component2 != null)
			{
				int cell = Grid.PosToCell(base.transform.GetPosition());
				Color32 c2 = (component2.IsHandlerOn() ? resources.switchColor : resources.switchOffColor);
				DrawUtilityIcon(cell, resources.switchIcon, ref outputVisualizer, c2, Color.white, 1f);
				flag2 = false;
			}
			else
			{
				WireUtilityNetworkLink component3 = GetComponent<WireUtilityNetworkLink>();
				if (component3 != null)
				{
					component3.GetCells(out var linked_cell, out var linked_cell2);
					DrawUtilityIcon(linked_cell, (Game.Instance.circuitManager.GetCircuitID(linked_cell) == ushort.MaxValue) ? resources.electricityBridgeIcon : resources.electricityConnectedIcon, ref inputVisualizer, resources.electricityInputColor, Color.white, 1f);
					DrawUtilityIcon(linked_cell2, (Game.Instance.circuitManager.GetCircuitID(linked_cell2) == ushort.MaxValue) ? resources.electricityBridgeIcon : resources.electricityConnectedIcon, ref outputVisualizer, resources.electricityInputColor, Color.white, 1f);
					flag2 = false;
				}
			}
			if (flag2)
			{
				DisableIcons();
			}
		}
		else if (mode == OverlayModes.GasConduits.ID)
		{
			if (RequiresGas || (secondary_ports & (Ports.GasIn | Ports.GasOut)) != 0)
			{
				if ((ports & Ports.GasIn) != 0)
				{
					bool num = null != Grid.Objects[building.GetUtilityInputCell(), 12];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input = resources.gasIOColours.input;
					Color tint2 = (num ? input.connected : input.disconnected);
					DrawUtilityIcon(building.GetUtilityInputCell(), resources.gasInputIcon, ref inputVisualizer, tint2);
				}
				if ((ports & Ports.GasOut) != 0)
				{
					bool num2 = null != Grid.Objects[building.GetUtilityOutputCell(), 12];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output = resources.gasIOColours.output;
					Color tint3 = (num2 ? output.connected : output.disconnected);
					DrawUtilityIcon(building.GetUtilityOutputCell(), resources.gasOutputIcon, ref outputVisualizer, tint3);
				}
				if ((secondary_ports & Ports.GasIn) != 0)
				{
					CellOffset secondaryConduitOffset = building.GetComponent<ISecondaryInput>().GetSecondaryConduitOffset();
					int visualizerCell = GetVisualizerCell(building, secondaryConduitOffset);
					DrawUtilityIcon(visualizerCell, resources.gasInputIcon, ref secondaryInputVisualizer, secondInputColour, Color.white);
				}
				if ((secondary_ports & Ports.GasOut) != 0)
				{
					CellOffset secondaryConduitOffset2 = building.GetComponent<ISecondaryOutput>().GetSecondaryConduitOffset();
					int visualizerCell2 = GetVisualizerCell(building, secondaryConduitOffset2);
					DrawUtilityIcon(visualizerCell2, resources.gasOutputIcon, ref secondaryOutputVisualizer, secondOutputColour, Color.white);
				}
			}
			else
			{
				DisableIcons();
			}
		}
		else if (mode == OverlayModes.LiquidConduits.ID)
		{
			if (RequiresLiquid || (secondary_ports & (Ports.LiquidIn | Ports.LiquidOut)) != 0)
			{
				if ((ports & Ports.LiquidIn) != 0)
				{
					bool num3 = null != Grid.Objects[building.GetUtilityInputCell(), 16];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input2 = resources.liquidIOColours.input;
					Color tint4 = (num3 ? input2.connected : input2.disconnected);
					DrawUtilityIcon(building.GetUtilityInputCell(), resources.liquidInputIcon, ref inputVisualizer, tint4);
				}
				if ((ports & Ports.LiquidOut) != 0)
				{
					bool num4 = null != Grid.Objects[building.GetUtilityOutputCell(), 16];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output2 = resources.liquidIOColours.output;
					Color tint5 = (num4 ? output2.connected : output2.disconnected);
					DrawUtilityIcon(building.GetUtilityOutputCell(), resources.liquidOutputIcon, ref outputVisualizer, tint5);
				}
				if ((secondary_ports & Ports.LiquidIn) != 0)
				{
					CellOffset secondaryConduitOffset3 = building.GetComponent<ISecondaryInput>().GetSecondaryConduitOffset();
					int visualizerCell3 = GetVisualizerCell(building, secondaryConduitOffset3);
					DrawUtilityIcon(visualizerCell3, resources.liquidInputIcon, ref secondaryInputVisualizer, secondInputColour, Color.white);
				}
				if ((secondary_ports & Ports.LiquidOut) != 0)
				{
					CellOffset secondaryConduitOffset4 = building.GetComponent<ISecondaryOutput>().GetSecondaryConduitOffset();
					int visualizerCell4 = GetVisualizerCell(building, secondaryConduitOffset4);
					DrawUtilityIcon(visualizerCell4, resources.liquidOutputIcon, ref secondaryOutputVisualizer, secondOutputColour, Color.white);
				}
			}
			else
			{
				DisableIcons();
			}
		}
		else if (mode == OverlayModes.SolidConveyor.ID)
		{
			if (RequiresSolid)
			{
				if ((ports & Ports.SolidIn) != 0)
				{
					bool num5 = null != Grid.Objects[building.GetUtilityInputCell(), 20];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input3 = resources.liquidIOColours.input;
					Color tint6 = (num5 ? input3.connected : input3.disconnected);
					DrawUtilityIcon(building.GetUtilityInputCell(), resources.liquidInputIcon, ref inputVisualizer, tint6);
				}
				if ((ports & Ports.SolidOut) != 0)
				{
					bool num6 = null != Grid.Objects[building.GetUtilityOutputCell(), 20];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output3 = resources.liquidIOColours.output;
					Color tint7 = (num6 ? output3.connected : output3.disconnected);
					DrawUtilityIcon(building.GetUtilityOutputCell(), resources.liquidOutputIcon, ref outputVisualizer, tint7);
				}
				if ((secondary_ports & Ports.SolidIn) != 0)
				{
					CellOffset secondaryConduitOffset5 = building.GetComponent<ISecondaryInput>().GetSecondaryConduitOffset();
					int visualizerCell5 = GetVisualizerCell(building, secondaryConduitOffset5);
					DrawUtilityIcon(visualizerCell5, resources.liquidInputIcon, ref secondaryInputVisualizer, secondInputColour, Color.white);
				}
				if ((secondary_ports & Ports.SolidOut) != 0)
				{
					CellOffset secondaryConduitOffset6 = building.GetComponent<ISecondaryOutput>().GetSecondaryConduitOffset();
					int visualizerCell6 = GetVisualizerCell(building, secondaryConduitOffset6);
					DrawUtilityIcon(visualizerCell6, resources.liquidOutputIcon, ref secondaryOutputVisualizer, secondOutputColour, Color.white);
				}
			}
			else
			{
				DisableIcons();
			}
		}
		else if (mode == OverlayModes.Disease.ID && diseaseSourceSprite != null)
		{
			int utilityOutputCell = building.GetUtilityOutputCell();
			DrawUtilityIcon(utilityOutputCell, diseaseSourceSprite, ref inputVisualizer, diseaseSourceColour);
		}
	}

	private int GetVisualizerCell(Building building, CellOffset offset)
	{
		CellOffset rotatedOffset = building.GetRotatedOffset(offset);
		return Grid.OffsetCell(building.GetCell(), rotatedOffset);
	}

	public void DisableIcons()
	{
		if (inputVisualizer != null && inputVisualizer.activeInHierarchy)
		{
			inputVisualizer.SetActive(value: false);
		}
		if (outputVisualizer != null && outputVisualizer.activeInHierarchy)
		{
			outputVisualizer.SetActive(value: false);
		}
		if (secondaryInputVisualizer != null && secondaryInputVisualizer.activeInHierarchy)
		{
			secondaryInputVisualizer.SetActive(value: false);
		}
		if (secondaryOutputVisualizer != null && secondaryOutputVisualizer.activeInHierarchy)
		{
			secondaryOutputVisualizer.SetActive(value: false);
		}
	}

	private void DrawUtilityIcon(int cell, Sprite icon_img, ref GameObject visualizerObj)
	{
		DrawUtilityIcon(cell, icon_img, ref visualizerObj, Color.white, Color.white);
	}

	private void DrawUtilityIcon(int cell, Sprite icon_img, ref GameObject visualizerObj, Color tint)
	{
		DrawUtilityIcon(cell, icon_img, ref visualizerObj, tint, Color.white);
	}

	private void DrawUtilityIcon(int cell, Sprite icon_img, ref GameObject visualizerObj, Color tint, Color connectorColor, float scaleMultiplier = 1.5f, bool hideBG = false)
	{
		Vector3 position = Grid.CellToPosCCC(cell, Grid.SceneLayer.Building);
		if (visualizerObj == null)
		{
			visualizerObj = Util.KInstantiate(Assets.UIPrefabs.ResourceVisualizer, GameScreenManager.Instance.worldSpaceCanvas);
			visualizerObj.transform.SetAsFirstSibling();
			icons.Add(visualizerObj, visualizerObj.transform.GetChild(0).GetComponent<Image>());
		}
		if (!visualizerObj.gameObject.activeInHierarchy)
		{
			visualizerObj.gameObject.SetActive(value: true);
		}
		visualizerObj.GetComponent<Image>().enabled = !hideBG;
		icons[visualizerObj].raycastTarget = enableRaycast;
		icons[visualizerObj].sprite = icon_img;
		visualizerObj.transform.GetChild(0).gameObject.GetComponent<Image>().color = tint;
		visualizerObj.transform.SetPosition(position);
		if (visualizerObj.GetComponent<SizePulse>() == null)
		{
			visualizerObj.transform.localScale = Vector3.one * scaleMultiplier;
		}
	}

	public Image GetOutputIcon()
	{
		if (!(outputVisualizer == null))
		{
			return outputVisualizer.transform.GetChild(0).GetComponent<Image>();
		}
		return null;
	}

	public Image GetInputIcon()
	{
		if (!(inputVisualizer == null))
		{
			return inputVisualizer.transform.GetChild(0).GetComponent<Image>();
		}
		return null;
	}
}
