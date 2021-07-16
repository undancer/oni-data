using System.Collections.Generic;
using System.Diagnostics;

public class CellEventLogger : EventLogger<CellEventInstance, CellEvent>
{
	public static CellEventLogger Instance;

	public CellSolidEvent SimMessagesSolid;

	public CellSolidEvent SimCellOccupierDestroy;

	public CellSolidEvent SimCellOccupierForceSolid;

	public CellSolidEvent SimCellOccupierSolidChanged;

	public CellElementEvent DoorOpen;

	public CellElementEvent DoorClose;

	public CellElementEvent Excavator;

	public CellElementEvent DebugTool;

	public CellElementEvent SandBoxTool;

	public CellElementEvent TemplateLoader;

	public CellElementEvent Scenario;

	public CellElementEvent SimCellOccupierOnSpawn;

	public CellElementEvent SimCellOccupierDestroySelf;

	public CellElementEvent WorldGapManager;

	public CellElementEvent ReceiveElementChanged;

	public CellElementEvent ObjectSetSimOnSpawn;

	public CellElementEvent DecompositionDirtyWater;

	public CellElementEvent LaunchpadDesolidify;

	public CellCallbackEvent SendCallback;

	public CellCallbackEvent ReceiveCallback;

	public CellDigEvent Dig;

	public CellAddRemoveSubstanceEvent WorldDamageDelayedSpawnFX;

	public CellAddRemoveSubstanceEvent SublimatesEmit;

	public CellAddRemoveSubstanceEvent OxygenModifierSimUpdate;

	public CellAddRemoveSubstanceEvent LiquidChunkOnStore;

	public CellAddRemoveSubstanceEvent FallingWaterAddToSim;

	public CellAddRemoveSubstanceEvent ExploderOnSpawn;

	public CellAddRemoveSubstanceEvent ExhaustSimUpdate;

	public CellAddRemoveSubstanceEvent ElementConsumerSimUpdate;

	public CellAddRemoveSubstanceEvent ElementChunkTransition;

	public CellAddRemoveSubstanceEvent OxyrockEmit;

	public CellAddRemoveSubstanceEvent BleachstoneEmit;

	public CellAddRemoveSubstanceEvent UnstableGround;

	public CellAddRemoveSubstanceEvent ConduitFlowEmptyConduit;

	public CellAddRemoveSubstanceEvent ConduitConsumerWrongElement;

	public CellAddRemoveSubstanceEvent OverheatableMeltingDown;

	public CellAddRemoveSubstanceEvent FabricatorProduceMelted;

	public CellAddRemoveSubstanceEvent PumpSimUpdate;

	public CellAddRemoveSubstanceEvent WallPumpSimUpdate;

	public CellAddRemoveSubstanceEvent Vomit;

	public CellAddRemoveSubstanceEvent Tears;

	public CellAddRemoveSubstanceEvent Pee;

	public CellAddRemoveSubstanceEvent AlgaeHabitat;

	public CellAddRemoveSubstanceEvent CO2FilterOxygen;

	public CellAddRemoveSubstanceEvent ToiletEmit;

	public CellAddRemoveSubstanceEvent ElementEmitted;

	public CellAddRemoveSubstanceEvent Mop;

	public CellAddRemoveSubstanceEvent OreMelted;

	public CellAddRemoveSubstanceEvent ConstructTile;

	public CellAddRemoveSubstanceEvent Dumpable;

	public CellAddRemoveSubstanceEvent Cough;

	public CellAddRemoveSubstanceEvent Meteor;

	public CellModifyMassEvent CO2ManagerFixedUpdate;

	public CellModifyMassEvent EnvironmentConsumerFixedUpdate;

	public CellModifyMassEvent ExcavatorShockwave;

	public CellModifyMassEvent OxygenBreatherSimUpdate;

	public CellModifyMassEvent CO2ScrubberSimUpdate;

	public CellModifyMassEvent RiverSourceSimUpdate;

	public CellModifyMassEvent RiverTerminusSimUpdate;

	public CellModifyMassEvent DebugToolModifyMass;

	public CellModifyMassEvent EnergyGeneratorModifyMass;

	public CellSolidFilterEvent SolidFilterEvent;

	public Dictionary<int, int> CallbackToCellMap = new Dictionary<int, int>();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	[Conditional("ENABLE_CELL_EVENT_LOGGER")]
	public void LogCallbackSend(int cell, int callback_id)
	{
		if (callback_id != -1)
		{
			CallbackToCellMap[callback_id] = cell;
		}
	}

	[Conditional("ENABLE_CELL_EVENT_LOGGER")]
	public void LogCallbackReceive(int callback_id)
	{
		int value = Grid.InvalidCell;
		CallbackToCellMap.TryGetValue(callback_id, out value);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		SimMessagesSolid = AddEvent(new CellSolidEvent("SimMessageSolid", "Sim Message", is_send: false)) as CellSolidEvent;
		SimCellOccupierDestroy = AddEvent(new CellSolidEvent("SimCellOccupierClearSolid", "Sim Cell Occupier Destroy", is_send: false)) as CellSolidEvent;
		SimCellOccupierForceSolid = AddEvent(new CellSolidEvent("SimCellOccupierForceSolid", "Sim Cell Occupier Force Solid", is_send: false)) as CellSolidEvent;
		SimCellOccupierSolidChanged = AddEvent(new CellSolidEvent("SimCellOccupierSolidChanged", "Sim Cell Occupier Solid Changed", is_send: false)) as CellSolidEvent;
		DoorOpen = AddEvent(new CellElementEvent("DoorOpen", "Door Open", is_send: true)) as CellElementEvent;
		DoorClose = AddEvent(new CellElementEvent("DoorClose", "Door Close", is_send: true)) as CellElementEvent;
		Excavator = AddEvent(new CellElementEvent("Excavator", "Excavator", is_send: true)) as CellElementEvent;
		DebugTool = AddEvent(new CellElementEvent("DebugTool", "Debug Tool", is_send: true)) as CellElementEvent;
		SandBoxTool = AddEvent(new CellElementEvent("SandBoxTool", "Sandbox Tool", is_send: true)) as CellElementEvent;
		TemplateLoader = AddEvent(new CellElementEvent("TemplateLoader", "Template Loader", is_send: true)) as CellElementEvent;
		Scenario = AddEvent(new CellElementEvent("Scenario", "Scenario", is_send: true)) as CellElementEvent;
		SimCellOccupierOnSpawn = AddEvent(new CellElementEvent("SimCellOccupierOnSpawn", "Sim Cell Occupier OnSpawn", is_send: true)) as CellElementEvent;
		SimCellOccupierDestroySelf = AddEvent(new CellElementEvent("SimCellOccupierDestroySelf", "Sim Cell Occupier Destroy Self", is_send: true)) as CellElementEvent;
		WorldGapManager = AddEvent(new CellElementEvent("WorldGapManager", "World Gap Manager", is_send: true)) as CellElementEvent;
		ReceiveElementChanged = AddEvent(new CellElementEvent("ReceiveElementChanged", "Sim Message", is_send: false, enable_logging: false)) as CellElementEvent;
		ObjectSetSimOnSpawn = AddEvent(new CellElementEvent("ObjectSetSimOnSpawn", "Object set sim on spawn", is_send: true)) as CellElementEvent;
		DecompositionDirtyWater = AddEvent(new CellElementEvent("DecompositionDirtyWater", "Decomposition dirty water", is_send: true)) as CellElementEvent;
		SendCallback = AddEvent(new CellCallbackEvent("SendCallback", is_send: true)) as CellCallbackEvent;
		ReceiveCallback = AddEvent(new CellCallbackEvent("ReceiveCallback", is_send: false)) as CellCallbackEvent;
		Dig = AddEvent(new CellDigEvent()) as CellDigEvent;
		WorldDamageDelayedSpawnFX = AddEvent(new CellAddRemoveSubstanceEvent("WorldDamageDelayedSpawnFX", "World Damage Delayed Spawn FX")) as CellAddRemoveSubstanceEvent;
		OxygenModifierSimUpdate = AddEvent(new CellAddRemoveSubstanceEvent("OxygenModifierSimUpdate", "Oxygen Modifier SimUpdate")) as CellAddRemoveSubstanceEvent;
		LiquidChunkOnStore = AddEvent(new CellAddRemoveSubstanceEvent("LiquidChunkOnStore", "Liquid Chunk On Store")) as CellAddRemoveSubstanceEvent;
		FallingWaterAddToSim = AddEvent(new CellAddRemoveSubstanceEvent("FallingWaterAddToSim", "Falling Water Add To Sim")) as CellAddRemoveSubstanceEvent;
		ExploderOnSpawn = AddEvent(new CellAddRemoveSubstanceEvent("ExploderOnSpawn", "Exploder OnSpawn")) as CellAddRemoveSubstanceEvent;
		ExhaustSimUpdate = AddEvent(new CellAddRemoveSubstanceEvent("ExhaustSimUpdate", "Exhaust SimUpdate")) as CellAddRemoveSubstanceEvent;
		ElementConsumerSimUpdate = AddEvent(new CellAddRemoveSubstanceEvent("ElementConsumerSimUpdate", "Element Consumer SimUpdate")) as CellAddRemoveSubstanceEvent;
		SublimatesEmit = AddEvent(new CellAddRemoveSubstanceEvent("SublimatesEmit", "Sublimates Emit")) as CellAddRemoveSubstanceEvent;
		Mop = AddEvent(new CellAddRemoveSubstanceEvent("Mop", "Mop")) as CellAddRemoveSubstanceEvent;
		OreMelted = AddEvent(new CellAddRemoveSubstanceEvent("OreMelted", "Ore Melted")) as CellAddRemoveSubstanceEvent;
		ConstructTile = AddEvent(new CellAddRemoveSubstanceEvent("ConstructTile", "ConstructTile")) as CellAddRemoveSubstanceEvent;
		Dumpable = AddEvent(new CellAddRemoveSubstanceEvent("Dympable", "Dumpable")) as CellAddRemoveSubstanceEvent;
		Cough = AddEvent(new CellAddRemoveSubstanceEvent("Cough", "Cough")) as CellAddRemoveSubstanceEvent;
		Meteor = AddEvent(new CellAddRemoveSubstanceEvent("Meteor", "Meteor")) as CellAddRemoveSubstanceEvent;
		ElementChunkTransition = AddEvent(new CellAddRemoveSubstanceEvent("ElementChunkTransition", "Element Chunk Transition")) as CellAddRemoveSubstanceEvent;
		OxyrockEmit = AddEvent(new CellAddRemoveSubstanceEvent("OxyrockEmit", "Oxyrock Emit")) as CellAddRemoveSubstanceEvent;
		BleachstoneEmit = AddEvent(new CellAddRemoveSubstanceEvent("BleachstoneEmit", "Bleachstone Emit")) as CellAddRemoveSubstanceEvent;
		UnstableGround = AddEvent(new CellAddRemoveSubstanceEvent("UnstableGround", "Unstable Ground")) as CellAddRemoveSubstanceEvent;
		ConduitFlowEmptyConduit = AddEvent(new CellAddRemoveSubstanceEvent("ConduitFlowEmptyConduit", "Conduit Flow Empty Conduit")) as CellAddRemoveSubstanceEvent;
		ConduitConsumerWrongElement = AddEvent(new CellAddRemoveSubstanceEvent("ConduitConsumerWrongElement", "Conduit Consumer Wrong Element")) as CellAddRemoveSubstanceEvent;
		OverheatableMeltingDown = AddEvent(new CellAddRemoveSubstanceEvent("OverheatableMeltingDown", "Overheatable MeltingDown")) as CellAddRemoveSubstanceEvent;
		FabricatorProduceMelted = AddEvent(new CellAddRemoveSubstanceEvent("FabricatorProduceMelted", "Fabricator Produce Melted")) as CellAddRemoveSubstanceEvent;
		PumpSimUpdate = AddEvent(new CellAddRemoveSubstanceEvent("PumpSimUpdate", "Pump SimUpdate")) as CellAddRemoveSubstanceEvent;
		WallPumpSimUpdate = AddEvent(new CellAddRemoveSubstanceEvent("WallPumpSimUpdate", "Wall Pump SimUpdate")) as CellAddRemoveSubstanceEvent;
		Vomit = AddEvent(new CellAddRemoveSubstanceEvent("Vomit", "Vomit")) as CellAddRemoveSubstanceEvent;
		Tears = AddEvent(new CellAddRemoveSubstanceEvent("Tears", "Tears")) as CellAddRemoveSubstanceEvent;
		Pee = AddEvent(new CellAddRemoveSubstanceEvent("Pee", "Pee")) as CellAddRemoveSubstanceEvent;
		AlgaeHabitat = AddEvent(new CellAddRemoveSubstanceEvent("AlgaeHabitat", "AlgaeHabitat")) as CellAddRemoveSubstanceEvent;
		CO2FilterOxygen = AddEvent(new CellAddRemoveSubstanceEvent("CO2FilterOxygen", "CO2FilterOxygen")) as CellAddRemoveSubstanceEvent;
		ToiletEmit = AddEvent(new CellAddRemoveSubstanceEvent("ToiletEmit", "ToiletEmit")) as CellAddRemoveSubstanceEvent;
		ElementEmitted = AddEvent(new CellAddRemoveSubstanceEvent("ElementEmitted", "Element Emitted")) as CellAddRemoveSubstanceEvent;
		CO2ManagerFixedUpdate = AddEvent(new CellModifyMassEvent("CO2ManagerFixedUpdate", "CO2Manager FixedUpdate")) as CellModifyMassEvent;
		EnvironmentConsumerFixedUpdate = AddEvent(new CellModifyMassEvent("EnvironmentConsumerFixedUpdate", "EnvironmentConsumer FixedUpdate")) as CellModifyMassEvent;
		ExcavatorShockwave = AddEvent(new CellModifyMassEvent("ExcavatorShockwave", "Excavator Shockwave")) as CellModifyMassEvent;
		OxygenBreatherSimUpdate = AddEvent(new CellModifyMassEvent("OxygenBreatherSimUpdate", "Oxygen Breather SimUpdate")) as CellModifyMassEvent;
		CO2ScrubberSimUpdate = AddEvent(new CellModifyMassEvent("CO2ScrubberSimUpdate", "CO2Scrubber SimUpdate")) as CellModifyMassEvent;
		RiverSourceSimUpdate = AddEvent(new CellModifyMassEvent("RiverSourceSimUpdate", "RiverSource SimUpdate")) as CellModifyMassEvent;
		RiverTerminusSimUpdate = AddEvent(new CellModifyMassEvent("RiverTerminusSimUpdate", "RiverTerminus SimUpdate")) as CellModifyMassEvent;
		DebugToolModifyMass = AddEvent(new CellModifyMassEvent("DebugToolModifyMass", "DebugTool ModifyMass")) as CellModifyMassEvent;
		EnergyGeneratorModifyMass = AddEvent(new CellModifyMassEvent("EnergyGeneratorModifyMass", "EnergyGenerator ModifyMass")) as CellModifyMassEvent;
		SolidFilterEvent = AddEvent(new CellSolidFilterEvent("SolidFilterEvent")) as CellSolidFilterEvent;
	}
}
