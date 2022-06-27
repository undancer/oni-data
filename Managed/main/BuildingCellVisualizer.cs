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
	private enum Ports
	{
		PowerIn = 1,
		PowerOut = 2,
		GasIn = 4,
		GasOut = 8,
		LiquidIn = 0x10,
		LiquidOut = 0x20,
		SolidIn = 0x40,
		SolidOut = 0x80,
		HighEnergyParticle = 0x100
	}

	private BuildingCellVisualizerResources resources;

	[MyCmpReq]
	private Building building;

	[SerializeField]
	public static Color32 secondOutputColour = new Color(0.9843137f, 0.6901961f, 0.23137255f);

	[SerializeField]
	public static Color32 secondInputColour = new Color(0.9843137f, 0.6901961f, 0.23137255f);

	private const Ports POWER_PORTS = Ports.PowerIn | Ports.PowerOut;

	private const Ports GAS_PORTS = Ports.GasIn | Ports.GasOut;

	private const Ports LIQUID_PORTS = Ports.LiquidIn | Ports.LiquidOut;

	private const Ports SOLID_PORTS = Ports.SolidIn | Ports.SolidOut;

	private const Ports MATTER_PORTS = Ports.GasIn | Ports.GasOut | Ports.LiquidIn | Ports.LiquidOut | Ports.SolidIn | Ports.SolidOut;

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

	public bool RequiresUtilityConnection => (ports & (Ports.GasIn | Ports.GasOut | Ports.LiquidIn | Ports.LiquidOut | Ports.SolidIn | Ports.SolidOut)) != 0;

	public bool RequiresHighEnergyParticles => (ports & Ports.HighEnergyParticle) != 0;

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
		if (def.CheckRequiresHighEnergyParticleInput())
		{
			ports |= Ports.HighEnergyParticle;
		}
		if (def.CheckRequiresHighEnergyParticleOutput())
		{
			ports |= Ports.HighEnergyParticle;
		}
		DiseaseVisualization.Info info = Assets.instance.DiseaseVisualization.GetInfo(def.DiseaseCellVisName);
		if (info.name != null)
		{
			diseaseSourceSprite = Assets.instance.DiseaseVisualization.overlaySprite;
			diseaseSourceColour = GlobalAssets.Instance.colorSet.GetColorByName(info.overlayColourName);
		}
		ISecondaryInput[] components = def.BuildingComplete.GetComponents<ISecondaryInput>();
		foreach (ISecondaryInput secondaryInput in components)
		{
			if (secondaryInput != null)
			{
				if (secondaryInput.HasSecondaryConduitType(ConduitType.Gas))
				{
					secondary_ports |= Ports.GasIn;
				}
				if (secondaryInput.HasSecondaryConduitType(ConduitType.Liquid))
				{
					secondary_ports |= Ports.LiquidIn;
				}
				if (secondaryInput.HasSecondaryConduitType(ConduitType.Solid))
				{
					secondary_ports |= Ports.SolidIn;
				}
			}
		}
		ISecondaryOutput[] components2 = def.BuildingComplete.GetComponents<ISecondaryOutput>();
		foreach (ISecondaryOutput secondaryOutput in components2)
		{
			if (secondaryOutput != null)
			{
				if (secondaryOutput.HasSecondaryConduitType(ConduitType.Gas))
				{
					secondary_ports |= Ports.GasOut;
				}
				if (secondaryOutput.HasSecondaryConduitType(ConduitType.Liquid))
				{
					secondary_ports |= Ports.LiquidOut;
				}
				if (secondaryOutput.HasSecondaryConduitType(ConduitType.Solid))
				{
					secondary_ports |= Ports.SolidOut;
				}
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
		if (base.gameObject.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
		{
			DisableIcons();
		}
		else if (mode == OverlayModes.Power.ID)
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
					Color32 color2 = ((component != null && !component.IsEnabled) ? Color.gray : color);
					Sprite icon_img2 = ((!flag && circuitID2 != 65535) ? resources.electricityConnectedIcon : resources.electricityInputIcon);
					DrawUtilityIcon(powerOutputCell, icon_img2, ref outputVisualizer, color2, GetWireColor(powerOutputCell), 1f);
				}
				return;
			}
			bool flag2 = true;
			Switch component2 = GetComponent<Switch>();
			if (component2 != null)
			{
				int cell = Grid.PosToCell(base.transform.GetPosition());
				Color32 color3 = (component2.IsHandlerOn() ? resources.switchColor : resources.switchOffColor);
				DrawUtilityIcon(cell, resources.switchIcon, ref outputVisualizer, color3, Color.white, 1f);
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
					ISecondaryInput[] components = building.GetComponents<ISecondaryInput>();
					CellOffset cellOffset = CellOffset.none;
					ISecondaryInput[] array = components;
					for (int i = 0; i < array.Length; i++)
					{
						cellOffset = array[i].GetSecondaryConduitOffset(ConduitType.Gas);
						if (cellOffset != CellOffset.none)
						{
							break;
						}
					}
					Color tint4 = secondInputColour;
					if ((ports & Ports.GasIn) == 0)
					{
						bool num3 = null != Grid.Objects[Grid.OffsetCell(Grid.PosToCell(building.transform.GetPosition()), cellOffset), 12];
						BuildingCellVisualizerResources.ConnectedDisconnectedColours input2 = resources.gasIOColours.input;
						tint4 = (num3 ? input2.connected : input2.disconnected);
					}
					int visualizerCell = GetVisualizerCell(building, cellOffset);
					DrawUtilityIcon(visualizerCell, resources.gasInputIcon, ref secondaryInputVisualizer, tint4, Color.white);
				}
				if ((secondary_ports & Ports.GasOut) == 0)
				{
					return;
				}
				ISecondaryOutput[] components2 = building.GetComponents<ISecondaryOutput>();
				CellOffset cellOffset2 = CellOffset.none;
				ISecondaryOutput[] array2 = components2;
				for (int i = 0; i < array2.Length; i++)
				{
					cellOffset2 = array2[i].GetSecondaryConduitOffset(ConduitType.Gas);
					if (cellOffset2 != CellOffset.none)
					{
						break;
					}
				}
				Color tint5 = secondOutputColour;
				if ((ports & Ports.GasOut) == 0)
				{
					bool num4 = null != Grid.Objects[Grid.OffsetCell(Grid.PosToCell(building.transform.GetPosition()), cellOffset2), 12];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output2 = resources.gasIOColours.output;
					tint5 = (num4 ? output2.connected : output2.disconnected);
				}
				int visualizerCell2 = GetVisualizerCell(building, cellOffset2);
				DrawUtilityIcon(visualizerCell2, resources.gasOutputIcon, ref secondaryOutputVisualizer, tint5, Color.white);
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
					bool num5 = null != Grid.Objects[building.GetUtilityInputCell(), 16];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input3 = resources.liquidIOColours.input;
					Color tint6 = (num5 ? input3.connected : input3.disconnected);
					DrawUtilityIcon(building.GetUtilityInputCell(), resources.liquidInputIcon, ref inputVisualizer, tint6);
				}
				if ((ports & Ports.LiquidOut) != 0)
				{
					bool num6 = null != Grid.Objects[building.GetUtilityOutputCell(), 16];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output3 = resources.liquidIOColours.output;
					Color tint7 = (num6 ? output3.connected : output3.disconnected);
					DrawUtilityIcon(building.GetUtilityOutputCell(), resources.liquidOutputIcon, ref outputVisualizer, tint7);
				}
				if ((secondary_ports & Ports.LiquidIn) != 0)
				{
					ISecondaryInput[] components3 = building.GetComponents<ISecondaryInput>();
					CellOffset cellOffset3 = CellOffset.none;
					ISecondaryInput[] array = components3;
					for (int i = 0; i < array.Length; i++)
					{
						cellOffset3 = array[i].GetSecondaryConduitOffset(ConduitType.Liquid);
						if (cellOffset3 != CellOffset.none)
						{
							break;
						}
					}
					Color tint8 = secondInputColour;
					if ((ports & Ports.LiquidIn) == 0)
					{
						bool num7 = null != Grid.Objects[Grid.OffsetCell(Grid.PosToCell(building.transform.GetPosition()), cellOffset3), 16];
						BuildingCellVisualizerResources.ConnectedDisconnectedColours input4 = resources.liquidIOColours.input;
						tint8 = (num7 ? input4.connected : input4.disconnected);
					}
					int visualizerCell3 = GetVisualizerCell(building, cellOffset3);
					DrawUtilityIcon(visualizerCell3, resources.liquidInputIcon, ref secondaryInputVisualizer, tint8, Color.white);
				}
				if ((secondary_ports & Ports.LiquidOut) == 0)
				{
					return;
				}
				ISecondaryOutput[] components4 = building.GetComponents<ISecondaryOutput>();
				CellOffset cellOffset4 = CellOffset.none;
				ISecondaryOutput[] array2 = components4;
				for (int i = 0; i < array2.Length; i++)
				{
					cellOffset4 = array2[i].GetSecondaryConduitOffset(ConduitType.Liquid);
					if (cellOffset4 != CellOffset.none)
					{
						break;
					}
				}
				Color tint9 = secondOutputColour;
				if ((ports & Ports.LiquidOut) == 0)
				{
					bool num8 = null != Grid.Objects[Grid.OffsetCell(Grid.PosToCell(building.transform.GetPosition()), cellOffset4), 16];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output4 = resources.liquidIOColours.output;
					tint9 = (num8 ? output4.connected : output4.disconnected);
				}
				int visualizerCell4 = GetVisualizerCell(building, cellOffset4);
				DrawUtilityIcon(visualizerCell4, resources.liquidOutputIcon, ref secondaryOutputVisualizer, tint9, Color.white);
			}
			else
			{
				DisableIcons();
			}
		}
		else if (mode == OverlayModes.SolidConveyor.ID)
		{
			if (RequiresSolid || (secondary_ports & (Ports.SolidIn | Ports.SolidOut)) != 0)
			{
				if ((ports & Ports.SolidIn) != 0)
				{
					bool num9 = null != Grid.Objects[building.GetUtilityInputCell(), 20];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input5 = resources.liquidIOColours.input;
					Color tint10 = (num9 ? input5.connected : input5.disconnected);
					DrawUtilityIcon(building.GetUtilityInputCell(), resources.liquidInputIcon, ref inputVisualizer, tint10);
				}
				if ((ports & Ports.SolidOut) != 0)
				{
					bool num10 = null != Grid.Objects[building.GetUtilityOutputCell(), 20];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output5 = resources.liquidIOColours.output;
					Color tint11 = (num10 ? output5.connected : output5.disconnected);
					DrawUtilityIcon(building.GetUtilityOutputCell(), resources.liquidOutputIcon, ref outputVisualizer, tint11);
				}
				if ((secondary_ports & Ports.SolidIn) != 0)
				{
					ISecondaryInput[] components5 = building.GetComponents<ISecondaryInput>();
					CellOffset cellOffset5 = CellOffset.none;
					ISecondaryInput[] array = components5;
					for (int i = 0; i < array.Length; i++)
					{
						cellOffset5 = array[i].GetSecondaryConduitOffset(ConduitType.Solid);
						if (cellOffset5 != CellOffset.none)
						{
							break;
						}
					}
					Color tint12 = secondInputColour;
					if ((ports & Ports.SolidIn) == 0)
					{
						bool num11 = null != Grid.Objects[Grid.OffsetCell(Grid.PosToCell(building.transform.GetPosition()), cellOffset5), 20];
						BuildingCellVisualizerResources.ConnectedDisconnectedColours input6 = resources.liquidIOColours.input;
						tint12 = (num11 ? input6.connected : input6.disconnected);
					}
					int visualizerCell5 = GetVisualizerCell(building, cellOffset5);
					DrawUtilityIcon(visualizerCell5, resources.liquidInputIcon, ref secondaryInputVisualizer, tint12, Color.white);
				}
				if ((secondary_ports & Ports.SolidOut) == 0)
				{
					return;
				}
				ISecondaryOutput[] components6 = building.GetComponents<ISecondaryOutput>();
				CellOffset cellOffset6 = CellOffset.none;
				ISecondaryOutput[] array2 = components6;
				for (int i = 0; i < array2.Length; i++)
				{
					cellOffset6 = array2[i].GetSecondaryConduitOffset(ConduitType.Solid);
					if (cellOffset6 != CellOffset.none)
					{
						break;
					}
				}
				Color tint13 = secondOutputColour;
				if ((ports & Ports.SolidOut) == 0)
				{
					bool num12 = null != Grid.Objects[Grid.OffsetCell(Grid.PosToCell(building.transform.GetPosition()), cellOffset6), 20];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output6 = resources.liquidIOColours.output;
					tint13 = (num12 ? output6.connected : output6.disconnected);
				}
				int visualizerCell6 = GetVisualizerCell(building, cellOffset6);
				DrawUtilityIcon(visualizerCell6, resources.liquidOutputIcon, ref secondaryOutputVisualizer, tint13, Color.white);
			}
			else
			{
				DisableIcons();
			}
		}
		else if (mode == OverlayModes.Disease.ID)
		{
			if (diseaseSourceSprite != null)
			{
				int utilityOutputCell = building.GetUtilityOutputCell();
				DrawUtilityIcon(utilityOutputCell, diseaseSourceSprite, ref inputVisualizer, diseaseSourceColour);
			}
		}
		else
		{
			if (!(mode == OverlayModes.Radiation.ID) || !RequiresHighEnergyParticles)
			{
				return;
			}
			int num13 = 3;
			if (building.Def.UseHighEnergyParticleInputPort)
			{
				int highEnergyParticleInputCell = building.GetHighEnergyParticleInputCell();
				DrawUtilityIcon(highEnergyParticleInputCell, resources.highEnergyParticleInputIcon, ref inputVisualizer, resources.highEnergyParticleInputColour, Color.white, num13, hideBG: true);
			}
			if (building.Def.UseHighEnergyParticleOutputPort)
			{
				int highEnergyParticleOutputCell = building.GetHighEnergyParticleOutputCell();
				IHighEnergyParticleDirection component4 = building.GetComponent<IHighEnergyParticleDirection>();
				Sprite icon_img3 = resources.highEnergyParticleOutputIcons[0];
				if (component4 != null)
				{
					int directionIndex = EightDirectionUtil.GetDirectionIndex(component4.Direction);
					icon_img3 = resources.highEnergyParticleOutputIcons[directionIndex];
				}
				DrawUtilityIcon(highEnergyParticleOutputCell, icon_img3, ref outputVisualizer, resources.highEnergyParticleOutputColour, Color.white, num13, hideBG: true);
			}
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
