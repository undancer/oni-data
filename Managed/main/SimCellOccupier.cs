using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/SimCellOccupier")]
public class SimCellOccupier : KMonoBehaviour, IGameObjectEffectDescriptor
{
	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[SerializeField]
	public bool doReplaceElement = true;

	[SerializeField]
	public bool setGasImpermeable = false;

	[SerializeField]
	public bool setLiquidImpermeable = false;

	[SerializeField]
	public bool setTransparent = false;

	[SerializeField]
	public bool setOpaque = false;

	[SerializeField]
	public bool notifyOnMelt = false;

	[SerializeField]
	public float strengthMultiplier = 1f;

	[SerializeField]
	public float movementSpeedMultiplier = 1f;

	private bool isReady;

	private bool callDestroy = true;

	private static readonly EventSystem.IntraObjectHandler<SimCellOccupier> OnBuildingRepairedDelegate = new EventSystem.IntraObjectHandler<SimCellOccupier>(delegate(SimCellOccupier component, object data)
	{
		component.OnBuildingRepaired(data);
	});

	public bool IsVisuallySolid => doReplaceElement;

	protected override void OnPrefabInit()
	{
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal);
	}

	protected override void OnSpawn()
	{
		HandleVector<Game.CallbackInfo>.Handle callbackHandle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(OnModifyComplete));
		int num = building.Def.PlacementOffsets.Length;
		float mass_per_cell = primaryElement.Mass / (float)num;
		building.RunOnArea(delegate(int offset_cell)
		{
			if (doReplaceElement)
			{
				SimMessages.ReplaceAndDisplaceElement(offset_cell, primaryElement.ElementID, CellEventLogger.Instance.SimCellOccupierOnSpawn, mass_per_cell, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, callbackHandle.index);
				callbackHandle = HandleVector<Game.CallbackInfo>.InvalidHandle;
				SimMessages.SetStrength(offset_cell, 0, strengthMultiplier);
				Game.Instance.RemoveSolidChangedFilter(offset_cell);
			}
			else
			{
				if (SaveGame.Instance.sandboxEnabled && Grid.Element[offset_cell].IsSolid)
				{
					SimMessages.Dig(offset_cell);
				}
				ForceSetGameCellData(offset_cell);
				Game.Instance.AddSolidChangedFilter(offset_cell);
			}
			Sim.Cell.Properties simCellProperties = GetSimCellProperties();
			SimMessages.SetCellProperties(offset_cell, (byte)simCellProperties);
			Grid.RenderedByWorld[offset_cell] = false;
			Game.Instance.GetComponent<EntombedItemVisualizer>().ForceClear(offset_cell);
		});
		Subscribe(-1699355994, OnBuildingRepairedDelegate);
	}

	protected override void OnCleanUp()
	{
		if (callDestroy)
		{
			DestroySelf(null);
		}
	}

	private Sim.Cell.Properties GetSimCellProperties()
	{
		Sim.Cell.Properties properties = Sim.Cell.Properties.SolidImpermeable;
		if (setGasImpermeable)
		{
			properties |= Sim.Cell.Properties.GasImpermeable;
		}
		if (setLiquidImpermeable)
		{
			properties |= Sim.Cell.Properties.LiquidImpermeable;
		}
		if (setTransparent)
		{
			properties |= Sim.Cell.Properties.Transparent;
		}
		if (setOpaque)
		{
			properties |= Sim.Cell.Properties.Opaque;
		}
		if (notifyOnMelt)
		{
			properties |= Sim.Cell.Properties.NotifyOnMelt;
		}
		return properties;
	}

	public void DestroySelf(System.Action onComplete)
	{
		callDestroy = false;
		for (int i = 0; i < building.PlacementCells.Length; i++)
		{
			int num = building.PlacementCells[i];
			Game.Instance.RemoveSolidChangedFilter(num);
			Sim.Cell.Properties simCellProperties = GetSimCellProperties();
			SimMessages.ClearCellProperties(num, (byte)simCellProperties);
			if (doReplaceElement && Grid.Element[num].id == primaryElement.ElementID)
			{
				HandleVector<int>.Handle handle = GameComps.DiseaseContainers.GetHandle(base.gameObject);
				if (handle.IsValid())
				{
					DiseaseHeader header = GameComps.DiseaseContainers.GetHeader(handle);
					header.diseaseIdx = Grid.DiseaseIdx[num];
					header.diseaseCount = Grid.DiseaseCount[num];
					GameComps.DiseaseContainers.SetHeader(handle, header);
				}
				if (onComplete != null)
				{
					SimMessages.ReplaceElement(callbackIdx: Game.Instance.callbackManager.Add(new Game.CallbackInfo(onComplete)).index, gameCell: num, new_element: SimHashes.Vacuum, ev: CellEventLogger.Instance.SimCellOccupierDestroySelf, mass: 0f);
				}
				else
				{
					SimMessages.ReplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierDestroySelf, 0f);
				}
				SimMessages.SetStrength(num, 1, 1f);
			}
			else
			{
				Grid.SetSolid(num, solid: false, CellEventLogger.Instance.SimCellOccupierDestroy);
				onComplete.Signal();
				World.Instance.OnSolidChanged(num);
				GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
			}
		}
	}

	public bool IsReady()
	{
		return isReady;
	}

	private void OnModifyComplete()
	{
		if (!(this == null) && !(base.gameObject == null))
		{
			isReady = true;
			GetComponent<PrimaryElement>().SetUseSimDiseaseInfo(use: true);
			Vector2I vector2I = Grid.PosToXY(base.transform.GetPosition());
			GameScenePartitioner.Instance.TriggerEvent(vector2I.x, vector2I.y, 1, 1, GameScenePartitioner.Instance.solidChangedLayer, null);
		}
	}

	private void ForceSetGameCellData(int cell)
	{
		bool solid = !Grid.DupePassable[cell];
		Grid.SetSolid(cell, solid, CellEventLogger.Instance.SimCellOccupierForceSolid);
		Pathfinding.Instance.AddDirtyNavGridCell(cell);
		GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.solidChangedLayer, null);
		Grid.Damage[cell] = 0f;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = null;
		if (movementSpeedMultiplier != 1f)
		{
			list = new List<Descriptor>();
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.DUPLICANTMOVEMENTBOOST, GameUtil.AddPositiveSign(GameUtil.GetFormattedPercent(movementSpeedMultiplier * 100f - 100f), movementSpeedMultiplier - 1f >= 0f)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.DUPLICANTMOVEMENTBOOST, GameUtil.GetFormattedPercent(movementSpeedMultiplier * 100f - 100f)));
			list.Add(item);
		}
		return list;
	}

	private void OnBuildingRepaired(object data)
	{
		BuildingHP buildingHP = (BuildingHP)data;
		float damage = 1f - (float)buildingHP.HitPoints / (float)buildingHP.MaxHitPoints;
		building.RunOnArea(delegate(int offset_cell)
		{
			WorldDamage.Instance.RestoreDamageToValue(offset_cell, damage);
		});
	}
}
