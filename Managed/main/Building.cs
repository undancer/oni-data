using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Database;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Building")]
public class Building : KMonoBehaviour, IGameObjectEffectDescriptor, IUniformGridObject, IApproachable
{
	public BuildingDef Def;

	[MyCmpGet]
	private Rotatable rotatable;

	[MyCmpAdd]
	private StateMachineController stateMachineController;

	private int[] placementCells;

	private Extents extents;

	private static StatusItem deprecatedBuildingStatusItem;

	private HandleVector<int>.Handle scenePartitionerEntry;

	public Orientation Orientation
	{
		get
		{
			if (!(rotatable != null))
			{
				return Orientation.Neutral;
			}
			return rotatable.GetOrientation();
		}
	}

	public int[] PlacementCells
	{
		get
		{
			if (placementCells == null)
			{
				RefreshCells();
			}
			return placementCells;
		}
	}

	public Extents GetExtents()
	{
		if (extents.width == 0 || extents.height == 0)
		{
			RefreshCells();
		}
		return extents;
	}

	public Extents GetValidPlacementExtents()
	{
		Extents result = GetExtents();
		result.x--;
		result.y--;
		result.width += 2;
		result.height += 2;
		return result;
	}

	public void RefreshCells()
	{
		placementCells = new int[Def.PlacementOffsets.Length];
		int num = Grid.PosToCell(this);
		if (num < 0)
		{
			extents.x = -1;
			extents.y = -1;
			extents.width = Def.WidthInCells;
			extents.height = Def.HeightInCells;
			return;
		}
		Orientation orientation = Orientation;
		for (int i = 0; i < Def.PlacementOffsets.Length; i++)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(Def.PlacementOffsets[i], orientation);
			int num2 = Grid.OffsetCell(num, rotatedCellOffset);
			placementCells[i] = num2;
		}
		int x = 0;
		int y = 0;
		Grid.CellToXY(placementCells[0], out x, out y);
		int num3 = x;
		int num4 = y;
		int[] array = placementCells;
		foreach (int cell in array)
		{
			int x2 = 0;
			int y2 = 0;
			Grid.CellToXY(cell, out x2, out y2);
			x = Math.Min(x, x2);
			y = Math.Min(y, y2);
			num3 = Math.Max(num3, x2);
			num4 = Math.Max(num4, y2);
		}
		extents.x = x;
		extents.y = y;
		extents.width = num3 - x + 1;
		extents.height = num4 - y + 1;
	}

	[OnDeserialized]
	internal void OnDeserialized()
	{
		PrimaryElement component = GetComponent<PrimaryElement>();
		if (component != null && component.Temperature == 0f)
		{
			if (component.Element == null)
			{
				DeserializeWarnings.Instance.PrimaryElementHasNoElement.Warn(base.name + " primary element has no element.", base.gameObject);
			}
			else if (!(this is BuildingUnderConstruction))
			{
				DeserializeWarnings.Instance.BuildingTemeperatureIsZeroKelvin.Warn(base.name + " is at zero degrees kelvin. Resetting temperature.");
				component.Temperature = component.Element.defaultValues.temperature;
			}
		}
	}

	protected override void OnSpawn()
	{
		if (Def == null)
		{
			Debug.LogError("Missing building definition on object " + base.name);
		}
		KSelectable component = GetComponent<KSelectable>();
		if (component != null)
		{
			component.SetName(Def.Name);
			component.SetStatusIndicatorOffset(new Vector3(0f, -0.35f, 0f));
		}
		Prioritizable component2 = GetComponent<Prioritizable>();
		if (component2 != null)
		{
			component2.iconOffset.y = 0.3f;
		}
		if (GetComponent<KPrefabID>().HasTag(RoomConstraints.ConstraintTags.IndustrialMachinery))
		{
			scenePartitionerEntry = GameScenePartitioner.Instance.Add(base.name, base.gameObject, GetExtents(), GameScenePartitioner.Instance.industrialBuildings, null);
		}
		if (Def.Deprecated && GetComponent<KSelectable>() != null)
		{
			KSelectable component3 = GetComponent<KSelectable>();
			deprecatedBuildingStatusItem = new StatusItem("BUILDING_DEPRECATED", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			component3.AddStatusItem(deprecatedBuildingStatusItem);
		}
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref scenePartitionerEntry);
		base.OnCleanUp();
	}

	public virtual void UpdatePosition(int cell)
	{
		GameScenePartitioner.Instance.UpdatePosition(scenePartitionerEntry, cell);
	}

	protected void RegisterBlockTileRenderer()
	{
		if (Def.BlockTileAtlas != null)
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			if (component != null)
			{
				SimHashes visualizationElementID = GetVisualizationElementID(component);
				int cell = Grid.PosToCell(base.transform.GetPosition());
				Constructable component2 = GetComponent<Constructable>();
				bool isReplacement = component2 != null && component2.IsReplacementTile;
				World.Instance.blockTileRenderer.AddBlock(base.gameObject.layer, Def, isReplacement, visualizationElementID, cell);
			}
		}
	}

	public CellOffset GetRotatedOffset(CellOffset offset)
	{
		if (!(rotatable != null))
		{
			return offset;
		}
		return rotatable.GetRotatedCellOffset(offset);
	}

	private int GetBottomLeftCell()
	{
		return Grid.PosToCell(base.transform.GetPosition());
	}

	public int GetPowerInputCell()
	{
		CellOffset rotatedOffset = GetRotatedOffset(Def.PowerInputOffset);
		return Grid.OffsetCell(GetBottomLeftCell(), rotatedOffset);
	}

	public int GetPowerOutputCell()
	{
		CellOffset rotatedOffset = GetRotatedOffset(Def.PowerOutputOffset);
		return Grid.OffsetCell(GetBottomLeftCell(), rotatedOffset);
	}

	public int GetUtilityInputCell()
	{
		CellOffset rotatedOffset = GetRotatedOffset(Def.UtilityInputOffset);
		return Grid.OffsetCell(GetBottomLeftCell(), rotatedOffset);
	}

	public int GetHighEnergyParticleInputCell()
	{
		CellOffset rotatedOffset = GetRotatedOffset(Def.HighEnergyParticleInputOffset);
		return Grid.OffsetCell(GetBottomLeftCell(), rotatedOffset);
	}

	public int GetHighEnergyParticleOutputCell()
	{
		CellOffset rotatedOffset = GetRotatedOffset(Def.HighEnergyParticleOutputOffset);
		return Grid.OffsetCell(GetBottomLeftCell(), rotatedOffset);
	}

	public int GetUtilityOutputCell()
	{
		CellOffset rotatedOffset = GetRotatedOffset(Def.UtilityOutputOffset);
		return Grid.OffsetCell(GetBottomLeftCell(), rotatedOffset);
	}

	public CellOffset GetUtilityInputOffset()
	{
		return GetRotatedOffset(Def.UtilityInputOffset);
	}

	public CellOffset GetUtilityOutputOffset()
	{
		return GetRotatedOffset(Def.UtilityOutputOffset);
	}

	public CellOffset GetHighEnergyParticleInputOffset()
	{
		return GetRotatedOffset(Def.HighEnergyParticleInputOffset);
	}

	public CellOffset GetHighEnergyParticleOutputOffset()
	{
		return GetRotatedOffset(Def.HighEnergyParticleOutputOffset);
	}

	protected void UnregisterBlockTileRenderer()
	{
		if (Def.BlockTileAtlas != null)
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			if (component != null)
			{
				SimHashes visualizationElementID = GetVisualizationElementID(component);
				int cell = Grid.PosToCell(base.transform.GetPosition());
				Constructable component2 = GetComponent<Constructable>();
				bool isReplacement = component2 != null && component2.IsReplacementTile;
				World.Instance.blockTileRenderer.RemoveBlock(Def, isReplacement, visualizationElementID, cell);
			}
		}
	}

	private SimHashes GetVisualizationElementID(PrimaryElement pe)
	{
		if (!(this is BuildingComplete))
		{
			return SimHashes.Void;
		}
		return pe.ElementID;
	}

	public void RunOnArea(Action<int> callback)
	{
		Def.RunOnArea(Grid.PosToCell(this), Orientation, callback);
	}

	public List<Descriptor> RequirementDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		BuildingComplete component = def.BuildingComplete.GetComponent<BuildingComplete>();
		if (def.RequiresPowerInput)
		{
			float wattsNeededWhenActive = component.GetComponent<IEnergyConsumer>().WattsNeededWhenActive;
			if (wattsNeededWhenActive > 0f)
			{
				string formattedWattage = GameUtil.GetFormattedWattage(wattsNeededWhenActive);
				Descriptor item = new Descriptor(string.Format(UI.BUILDINGEFFECTS.REQUIRESPOWER, formattedWattage), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESPOWER, formattedWattage), Descriptor.DescriptorType.Requirement);
				list.Add(item);
			}
		}
		if (def.InputConduitType == ConduitType.Liquid)
		{
			Descriptor item2 = default(Descriptor);
			item2.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESLIQUIDINPUT, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESLIQUIDINPUT, Descriptor.DescriptorType.Requirement);
			list.Add(item2);
		}
		else if (def.InputConduitType == ConduitType.Gas)
		{
			Descriptor item3 = default(Descriptor);
			item3.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESGASINPUT, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESGASINPUT, Descriptor.DescriptorType.Requirement);
			list.Add(item3);
		}
		if (def.OutputConduitType == ConduitType.Liquid)
		{
			Descriptor item4 = default(Descriptor);
			item4.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESLIQUIDOUTPUT, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESLIQUIDOUTPUT, Descriptor.DescriptorType.Requirement);
			list.Add(item4);
		}
		else if (def.OutputConduitType == ConduitType.Gas)
		{
			Descriptor item5 = default(Descriptor);
			item5.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESGASOUTPUT, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESGASOUTPUT, Descriptor.DescriptorType.Requirement);
			list.Add(item5);
		}
		if (component.isManuallyOperated)
		{
			Descriptor item6 = default(Descriptor);
			item6.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESMANUALOPERATION, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESMANUALOPERATION, Descriptor.DescriptorType.Requirement);
			list.Add(item6);
		}
		if (component.isArtable)
		{
			Descriptor item7 = default(Descriptor);
			item7.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESCREATIVITY, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESCREATIVITY, Descriptor.DescriptorType.Requirement);
			list.Add(item7);
		}
		if (def.BuildingUnderConstruction != null)
		{
			Constructable component2 = def.BuildingUnderConstruction.GetComponent<Constructable>();
			if (component2 != null && component2.requiredSkillPerk != HashedString.Invalid)
			{
				StringBuilder stringBuilder = new StringBuilder();
				List<Skill> skillsWithPerk = Db.Get().Skills.GetSkillsWithPerk(component2.requiredSkillPerk);
				for (int i = 0; i < skillsWithPerk.Count; i++)
				{
					Skill skill = skillsWithPerk[i];
					stringBuilder.Append(skill.Name);
					if (i != skillsWithPerk.Count - 1)
					{
						stringBuilder.Append(", ");
					}
				}
				string replacement = stringBuilder.ToString();
				list.Add(new Descriptor(UI.BUILD_REQUIRES_SKILL.Replace("{Skill}", replacement), UI.BUILD_REQUIRES_SKILL_TOOLTIP.Replace("{Skill}", replacement), Descriptor.DescriptorType.Requirement));
			}
		}
		return list;
	}

	public List<Descriptor> EffectDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (def.EffectDescription != null)
		{
			list.AddRange(def.EffectDescription);
		}
		if (def.GeneratorWattageRating > 0f && GetComponent<Battery>() == null)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ENERGYGENERATED, GameUtil.GetFormattedWattage(def.GeneratorWattageRating)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ENERGYGENERATED, GameUtil.GetFormattedWattage(def.GeneratorWattageRating)));
			list.Add(item);
		}
		if (def.ExhaustKilowattsWhenActive > 0f || def.SelfHeatKilowattsWhenActive > 0f)
		{
			Descriptor item2 = default(Descriptor);
			string formattedHeatEnergy = GameUtil.GetFormattedHeatEnergy((def.ExhaustKilowattsWhenActive + def.SelfHeatKilowattsWhenActive) * 1000f);
			item2.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.HEATGENERATED, formattedHeatEnergy), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED, formattedHeatEnergy));
			list.Add(item2);
		}
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor item in RequirementDescriptors(Def))
		{
			list.Add(item);
		}
		foreach (Descriptor item2 in EffectDescriptors(Def))
		{
			list.Add(item2);
		}
		return list;
	}

	public override Vector2 PosMin()
	{
		Extents extents = GetExtents();
		return new Vector2(extents.x, extents.y);
	}

	public override Vector2 PosMax()
	{
		Extents extents = GetExtents();
		return new Vector2(extents.x + extents.width, extents.y + extents.height);
	}

	public CellOffset[] GetOffsets()
	{
		return OffsetGroups.Use;
	}

	public int GetCell()
	{
		return Grid.PosToCell(this);
	}
}
