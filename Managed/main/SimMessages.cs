using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Database;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;
using STRINGS;

public static class SimMessages
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct AddElementConsumerMessage
	{
		public int cellIdx;

		public int callbackIdx;

		public byte radius;

		public byte configuration;

		public byte elementIdx;

		private byte pad0;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct SetElementConsumerDataMessage
	{
		public int handle;

		public int cell;

		public float consumptionRate;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct RemoveElementConsumerMessage
	{
		public int handle;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct AddElementEmitterMessage
	{
		public float maxPressure;

		public int callbackIdx;

		public int onBlockedCB;

		public int onUnblockedCB;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ModifyElementEmitterMessage
	{
		public int handle;

		public int cellIdx;

		public float emitInterval;

		public float emitMass;

		public float emitTemperature;

		public float maxPressure;

		public int diseaseCount;

		public byte elementIdx;

		public byte maxDepth;

		public byte diseaseIdx;

		private byte pad0;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct RemoveElementEmitterMessage
	{
		public int handle;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct AddRadiationEmitterMessage
	{
		public int callbackIdx;

		public int cell;

		public short emitRadiusX;

		public short emitRadiusY;

		public float emitRads;

		public float emitRate;

		public float emitSpeed;

		public float emitDirection;

		public float emitAngle;

		public int emitType;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ModifyRadiationEmitterMessage
	{
		public int handle;

		public int cell;

		public int callbackIdx;

		public short emitRadiusX;

		public short emitRadiusY;

		public float emitRads;

		public float emitRate;

		public float emitSpeed;

		public float emitDirection;

		public float emitAngle;

		public int emitType;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct RemoveRadiationEmitterMessage
	{
		public int handle;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct AddElementChunkMessage
	{
		public int gameCell;

		public int callbackIdx;

		public float mass;

		public float temperature;

		public float surfaceArea;

		public float thickness;

		public float groundTransferScale;

		public byte elementIdx;

		public byte pad0;

		public byte pad1;

		public byte pad2;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct RemoveElementChunkMessage
	{
		public int handle;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct SetElementChunkDataMessage
	{
		public int handle;

		public float temperature;

		public float heatCapacity;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct MoveElementChunkMessage
	{
		public int handle;

		public int gameCell;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ModifyElementChunkEnergyMessage
	{
		public int handle;

		public float deltaKJ;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ModifyElementChunkAdjusterMessage
	{
		public int handle;

		public float temperature;

		public float heatCapacity;

		public float thermalConductivity;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct AddBuildingHeatExchangeMessage
	{
		public int callbackIdx;

		public byte elemIdx;

		public byte pad0;

		public byte pad1;

		public byte pad2;

		public float mass;

		public float temperature;

		public float thermalConductivity;

		public float overheatTemperature;

		public float operatingKilowatts;

		public int minX;

		public int minY;

		public int maxX;

		public int maxY;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ModifyBuildingHeatExchangeMessage
	{
		public int callbackIdx;

		public byte elemIdx;

		public byte pad0;

		public byte pad1;

		public byte pad2;

		public float mass;

		public float temperature;

		public float thermalConductivity;

		public float overheatTemperature;

		public float operatingKilowatts;

		public int minX;

		public int minY;

		public int maxX;

		public int maxY;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ModifyBuildingEnergyMessage
	{
		public int handle;

		public float deltaKJ;

		public float minTemperature;

		public float maxTemperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct RemoveBuildingHeatExchangeMessage
	{
		public int handle;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct AddDiseaseEmitterMessage
	{
		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ModifyDiseaseEmitterMessage
	{
		public int handle;

		public int gameCell;

		public byte diseaseIdx;

		public byte maxDepth;

		private byte pad0;

		private byte pad1;

		public float emitInterval;

		public int emitCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct RemoveDiseaseEmitterMessage
	{
		public int handle;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct SetSavedOptionsMessage
	{
		public byte clearBits;

		public byte setBits;
	}

	public enum SimSavedOptions : byte
	{
		ENABLE_DIAGONAL_FALLING_SAND = 1
	}

	public struct WorldOffsetData
	{
		public int worldOffsetX;

		public int worldOffsetY;

		public int worldSizeX;

		public int worldSizeY;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct DigMessage
	{
		public int cellIdx;

		public int callbackIdx;

		public bool skipEvent;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct SetCellFloatValueMessage
	{
		public int cellIdx;

		public float value;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct CellPropertiesMessage
	{
		public int cellIdx;

		public byte properties;

		public byte set;

		public byte pad0;

		public byte pad1;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct SetInsulationValueMessage
	{
		public int cellIdx;

		public float value;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ModifyCellMessage
	{
		public int cellIdx;

		public int callbackIdx;

		public float temperature;

		public float mass;

		public int diseaseCount;

		public byte elementIdx;

		public byte replaceType;

		public byte diseaseIdx;

		public byte addSubType;
	}

	public enum ReplaceType
	{
		None,
		Replace,
		ReplaceAndDisplace
	}

	private enum AddSolidMassSubType
	{
		DoVerticalDisplacement,
		OnlyIfSameElement
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct CellDiseaseModification
	{
		public int cellIdx;

		public byte diseaseIdx;

		public byte pad0;

		public byte pad1;

		public byte pad2;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct RadiationParamsModification
	{
		public int RadiationParamsType;

		public float value;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct CellRadiationModification
	{
		public int cellIdx;

		public float radiationDelta;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct MassConsumptionMessage
	{
		public int cellIdx;

		public int callbackIdx;

		public float mass;

		public byte elementIdx;

		public byte radius;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct MassEmissionMessage
	{
		public int cellIdx;

		public int callbackIdx;

		public float mass;

		public float temperature;

		public int diseaseCount;

		public byte elementIdx;

		public byte diseaseIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ConsumeDiseaseMessage
	{
		public int gameCell;

		public int callbackIdx;

		public float percentToConsume;

		public int maxToConsume;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ModifyCellEnergyMessage
	{
		public int cellIdx;

		public float kilojoules;

		public float maxTemperature;

		public int id;
	}

	public enum EnergySourceID
	{
		DebugHeat = 1000,
		DebugCool,
		FierySkin,
		Overheatable,
		LiquidCooledFan,
		ConduitTemperatureManager,
		Excavator,
		HeatBulb,
		WarmBlooded,
		StructureTemperature,
		Burner
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct VisibleCells
	{
		public Vector2I min;

		public Vector2I max;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct WakeCellMessage
	{
		public int gameCell;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ElementInteraction
	{
		public uint interactionType;

		public byte elemIdx1;

		public byte elemIdx2;

		public byte elemResultIdx;

		public byte pad;

		public float minMass;

		public float interactionProbability;

		public float elem1MassDestructionPercent;

		public float elem2MassRequiredMultiplier;

		public float elemResultMassCreationMultiplier;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct CreateElementInteractionsMsg
	{
		public int numInteractions;

		public unsafe ElementInteraction* interactions;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct PipeChange
	{
		public int cell;

		public byte layer;

		public byte pad0;

		public byte pad1;

		public byte pad2;

		public float mass;

		public float temperature;

		public int elementHash;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct CellWorldZoneModification
	{
		public int cell;

		public byte zoneID;
	}

	public const int InvalidCallback = -1;

	public const float STATE_TRANSITION_TEMPERATURE_BUFER = 3f;

	public unsafe static void AddElementConsumer(int gameCell, ElementConsumer.Configuration configuration, SimHashes element, byte radius, int cb_handle)
	{
		Debug.Assert(Grid.IsValidCell(gameCell));
		if (Grid.IsValidCell(gameCell))
		{
			int elementIndex = ElementLoader.GetElementIndex(element);
			AddElementConsumerMessage* ptr = stackalloc AddElementConsumerMessage[1];
			ptr->cellIdx = gameCell;
			ptr->configuration = (byte)configuration;
			ptr->elementIdx = (byte)elementIndex;
			ptr->radius = radius;
			ptr->callbackIdx = cb_handle;
			Sim.SIM_HandleMessage(2024405073, sizeof(AddElementConsumerMessage), (byte*)ptr);
		}
	}

	public unsafe static void SetElementConsumerData(int sim_handle, int cell, float consumptionRate)
	{
		if (Sim.IsValidHandle(sim_handle))
		{
			SetElementConsumerDataMessage* ptr = stackalloc SetElementConsumerDataMessage[1];
			ptr->handle = sim_handle;
			ptr->cell = cell;
			ptr->consumptionRate = consumptionRate;
			Sim.SIM_HandleMessage(1575539738, sizeof(SetElementConsumerDataMessage), (byte*)ptr);
		}
	}

	public unsafe static void RemoveElementConsumer(int cb_handle, int sim_handle)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(condition: false, "Invalid handle");
			return;
		}
		RemoveElementConsumerMessage* ptr = stackalloc RemoveElementConsumerMessage[1];
		ptr->callbackIdx = cb_handle;
		ptr->handle = sim_handle;
		Sim.SIM_HandleMessage(894417742, sizeof(RemoveElementConsumerMessage), (byte*)ptr);
	}

	public unsafe static void AddElementEmitter(float max_pressure, int on_registered, int on_blocked = -1, int on_unblocked = -1)
	{
		AddElementEmitterMessage* ptr = stackalloc AddElementEmitterMessage[1];
		ptr->maxPressure = max_pressure;
		ptr->callbackIdx = on_registered;
		ptr->onBlockedCB = on_blocked;
		ptr->onUnblockedCB = on_unblocked;
		Sim.SIM_HandleMessage(-505471181, sizeof(AddElementEmitterMessage), (byte*)ptr);
	}

	public unsafe static void ModifyElementEmitter(int sim_handle, int game_cell, int max_depth, SimHashes element, float emit_interval, float emit_mass, float emit_temperature, float max_pressure, byte disease_idx, int disease_count)
	{
		Debug.Assert(Grid.IsValidCell(game_cell));
		if (Grid.IsValidCell(game_cell))
		{
			int elementIndex = ElementLoader.GetElementIndex(element);
			ModifyElementEmitterMessage* ptr = stackalloc ModifyElementEmitterMessage[1];
			ptr->handle = sim_handle;
			ptr->cellIdx = game_cell;
			ptr->emitInterval = emit_interval;
			ptr->emitMass = emit_mass;
			ptr->emitTemperature = emit_temperature;
			ptr->maxPressure = max_pressure;
			ptr->elementIdx = (byte)elementIndex;
			ptr->maxDepth = (byte)max_depth;
			ptr->diseaseIdx = disease_idx;
			ptr->diseaseCount = disease_count;
			Sim.SIM_HandleMessage(403589164, sizeof(ModifyElementEmitterMessage), (byte*)ptr);
		}
	}

	public unsafe static void RemoveElementEmitter(int cb_handle, int sim_handle)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(condition: false, "Invalid handle");
			return;
		}
		RemoveElementEmitterMessage* ptr = stackalloc RemoveElementEmitterMessage[1];
		ptr->callbackIdx = cb_handle;
		ptr->handle = sim_handle;
		Sim.SIM_HandleMessage(-1524118282, sizeof(RemoveElementEmitterMessage), (byte*)ptr);
	}

	public unsafe static void AddRadiationEmitter(int on_registered, int game_cell, short emitRadiusX, short emitRadiusY, float emitRads, float emitRate, float emitSpeed, float emitDirection, float emitAngle, RadiationEmitter.RadiationEmitterType emitType)
	{
		AddRadiationEmitterMessage* ptr = stackalloc AddRadiationEmitterMessage[1];
		ptr->callbackIdx = on_registered;
		ptr->cell = game_cell;
		ptr->emitRadiusX = emitRadiusX;
		ptr->emitRadiusY = emitRadiusY;
		ptr->emitRads = emitRads;
		ptr->emitRate = emitRate;
		ptr->emitSpeed = emitSpeed;
		ptr->emitDirection = emitDirection;
		ptr->emitAngle = emitAngle;
		ptr->emitType = (int)emitType;
		Sim.SIM_HandleMessage(-1505895314, sizeof(AddRadiationEmitterMessage), (byte*)ptr);
	}

	public unsafe static void ModifyRadiationEmitter(int sim_handle, int game_cell, short emitRadiusX, short emitRadiusY, float emitRads, float emitRate, float emitSpeed, float emitDirection, float emitAngle, RadiationEmitter.RadiationEmitterType emitType)
	{
		Debug.Assert(Grid.IsValidCell(game_cell));
		if (Grid.IsValidCell(game_cell))
		{
			ModifyRadiationEmitterMessage* ptr = stackalloc ModifyRadiationEmitterMessage[1];
			ptr->handle = sim_handle;
			ptr->cell = game_cell;
			ptr->callbackIdx = -1;
			ptr->emitRadiusX = emitRadiusX;
			ptr->emitRadiusY = emitRadiusY;
			ptr->emitRads = emitRads;
			ptr->emitRate = emitRate;
			ptr->emitSpeed = emitSpeed;
			ptr->emitDirection = emitDirection;
			ptr->emitAngle = emitAngle;
			ptr->emitType = (int)emitType;
			Sim.SIM_HandleMessage(-503965465, sizeof(ModifyRadiationEmitterMessage), (byte*)ptr);
		}
	}

	public unsafe static void RemoveRadiationEmitter(int cb_handle, int sim_handle)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(condition: false, "Invalid handle");
			return;
		}
		RemoveRadiationEmitterMessage* ptr = stackalloc RemoveRadiationEmitterMessage[1];
		ptr->callbackIdx = cb_handle;
		ptr->handle = sim_handle;
		Sim.SIM_HandleMessage(-704259919, sizeof(RemoveRadiationEmitterMessage), (byte*)ptr);
	}

	public unsafe static void AddElementChunk(int gameCell, SimHashes element, float mass, float temperature, float surface_area, float thickness, float ground_transfer_scale, int cb_handle)
	{
		Debug.Assert(Grid.IsValidCell(gameCell));
		if (Grid.IsValidCell(gameCell) && mass * temperature > 0f)
		{
			int elementIndex = ElementLoader.GetElementIndex(element);
			AddElementChunkMessage* ptr = stackalloc AddElementChunkMessage[1];
			ptr->gameCell = gameCell;
			ptr->callbackIdx = cb_handle;
			ptr->mass = mass;
			ptr->temperature = temperature;
			ptr->surfaceArea = surface_area;
			ptr->thickness = thickness;
			ptr->groundTransferScale = ground_transfer_scale;
			ptr->elementIdx = (byte)elementIndex;
			Sim.SIM_HandleMessage(1445724082, sizeof(AddElementChunkMessage), (byte*)ptr);
		}
	}

	public unsafe static void RemoveElementChunk(int sim_handle, int cb_handle)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(condition: false, "Invalid handle");
			return;
		}
		RemoveElementChunkMessage* ptr = stackalloc RemoveElementChunkMessage[1];
		ptr->callbackIdx = cb_handle;
		ptr->handle = sim_handle;
		Sim.SIM_HandleMessage(-912908555, sizeof(RemoveElementChunkMessage), (byte*)ptr);
	}

	public unsafe static void SetElementChunkData(int sim_handle, float temperature, float heat_capacity)
	{
		if (Sim.IsValidHandle(sim_handle))
		{
			SetElementChunkDataMessage* ptr = stackalloc SetElementChunkDataMessage[1];
			ptr->handle = sim_handle;
			ptr->temperature = temperature;
			ptr->heatCapacity = heat_capacity;
			Sim.SIM_HandleMessage(-435115907, sizeof(SetElementChunkDataMessage), (byte*)ptr);
		}
	}

	public unsafe static void MoveElementChunk(int sim_handle, int cell)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(condition: false, "Invalid handle");
			return;
		}
		MoveElementChunkMessage* ptr = stackalloc MoveElementChunkMessage[1];
		ptr->handle = sim_handle;
		ptr->gameCell = cell;
		Sim.SIM_HandleMessage(-374911358, sizeof(MoveElementChunkMessage), (byte*)ptr);
	}

	public unsafe static void ModifyElementChunkEnergy(int sim_handle, float delta_kj)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(condition: false, "Invalid handle");
			return;
		}
		ModifyElementChunkEnergyMessage* ptr = stackalloc ModifyElementChunkEnergyMessage[1];
		ptr->handle = sim_handle;
		ptr->deltaKJ = delta_kj;
		Sim.SIM_HandleMessage(1020555667, sizeof(ModifyElementChunkEnergyMessage), (byte*)ptr);
	}

	public unsafe static void ModifyElementChunkTemperatureAdjuster(int sim_handle, float temperature, float heat_capacity, float thermal_conductivity)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(condition: false, "Invalid handle");
			return;
		}
		ModifyElementChunkAdjusterMessage* ptr = stackalloc ModifyElementChunkAdjusterMessage[1];
		ptr->handle = sim_handle;
		ptr->temperature = temperature;
		ptr->heatCapacity = heat_capacity;
		ptr->thermalConductivity = thermal_conductivity;
		Sim.SIM_HandleMessage(-1387601379, sizeof(ModifyElementChunkAdjusterMessage), (byte*)ptr);
	}

	public unsafe static void AddBuildingHeatExchange(Extents extents, float mass, float temperature, float thermal_conductivity, float operating_kw, byte elem_idx, int callbackIdx = -1)
	{
		if (Grid.IsValidCell(Grid.XYToCell(extents.x, extents.y)))
		{
			int num = Grid.XYToCell(extents.x + extents.width, extents.y + extents.height);
			if (!Grid.IsValidCell(num))
			{
				Debug.LogErrorFormat("Invalid Cell [{0}] Extents [{1},{2}] [{3},{4}]", num, extents.x, extents.y, extents.width, extents.height);
			}
			if (Grid.IsValidCell(num))
			{
				AddBuildingHeatExchangeMessage* ptr = stackalloc AddBuildingHeatExchangeMessage[1];
				ptr->callbackIdx = callbackIdx;
				ptr->elemIdx = elem_idx;
				ptr->mass = mass;
				ptr->temperature = temperature;
				ptr->thermalConductivity = thermal_conductivity;
				ptr->overheatTemperature = float.MaxValue;
				ptr->operatingKilowatts = operating_kw;
				ptr->minX = extents.x;
				ptr->minY = extents.y;
				ptr->maxX = extents.x + extents.width;
				ptr->maxY = extents.y + extents.height;
				Sim.SIM_HandleMessage(1739021608, sizeof(AddBuildingHeatExchangeMessage), (byte*)ptr);
			}
		}
	}

	public unsafe static void ModifyBuildingHeatExchange(int sim_handle, Extents extents, float mass, float temperature, float thermal_conductivity, float overheat_temperature, float operating_kw, byte element_idx)
	{
		int cell = Grid.XYToCell(extents.x, extents.y);
		Debug.Assert(Grid.IsValidCell(cell));
		if (Grid.IsValidCell(cell))
		{
			int cell2 = Grid.XYToCell(extents.x + extents.width, extents.y + extents.height);
			Debug.Assert(Grid.IsValidCell(cell2));
			if (Grid.IsValidCell(cell2))
			{
				ModifyBuildingHeatExchangeMessage* ptr = stackalloc ModifyBuildingHeatExchangeMessage[1];
				ptr->callbackIdx = sim_handle;
				ptr->elemIdx = element_idx;
				ptr->mass = mass;
				ptr->temperature = temperature;
				ptr->thermalConductivity = thermal_conductivity;
				ptr->overheatTemperature = overheat_temperature;
				ptr->operatingKilowatts = operating_kw;
				ptr->minX = extents.x;
				ptr->minY = extents.y;
				ptr->maxX = extents.x + extents.width;
				ptr->maxY = extents.y + extents.height;
				Sim.SIM_HandleMessage(1818001569, sizeof(ModifyBuildingHeatExchangeMessage), (byte*)ptr);
			}
		}
	}

	public unsafe static void RemoveBuildingHeatExchange(int sim_handle, int callbackIdx = -1)
	{
		RemoveBuildingHeatExchangeMessage* ptr = stackalloc RemoveBuildingHeatExchangeMessage[1];
		Debug.Assert(Sim.IsValidHandle(sim_handle));
		ptr->handle = sim_handle;
		ptr->callbackIdx = callbackIdx;
		Sim.SIM_HandleMessage(-456116629, sizeof(RemoveBuildingHeatExchangeMessage), (byte*)ptr);
	}

	public unsafe static void ModifyBuildingEnergy(int sim_handle, float delta_kj, float min_temperature, float max_temperature)
	{
		ModifyBuildingEnergyMessage* ptr = stackalloc ModifyBuildingEnergyMessage[1];
		Debug.Assert(Sim.IsValidHandle(sim_handle));
		ptr->handle = sim_handle;
		ptr->deltaKJ = delta_kj;
		ptr->minTemperature = min_temperature;
		ptr->maxTemperature = max_temperature;
		Sim.SIM_HandleMessage(-1348791658, sizeof(ModifyBuildingEnergyMessage), (byte*)ptr);
	}

	public unsafe static void AddDiseaseEmitter(int callbackIdx)
	{
		AddDiseaseEmitterMessage* ptr = stackalloc AddDiseaseEmitterMessage[1];
		ptr->callbackIdx = callbackIdx;
		Sim.SIM_HandleMessage(1486783027, sizeof(AddDiseaseEmitterMessage), (byte*)ptr);
	}

	public unsafe static void ModifyDiseaseEmitter(int sim_handle, int cell, byte range, byte disease_idx, float emit_interval, int emit_count)
	{
		Debug.Assert(Sim.IsValidHandle(sim_handle));
		ModifyDiseaseEmitterMessage* ptr = stackalloc ModifyDiseaseEmitterMessage[1];
		ptr->handle = sim_handle;
		ptr->gameCell = cell;
		ptr->maxDepth = range;
		ptr->diseaseIdx = disease_idx;
		ptr->emitInterval = emit_interval;
		ptr->emitCount = emit_count;
		Sim.SIM_HandleMessage(-1899123924, sizeof(ModifyDiseaseEmitterMessage), (byte*)ptr);
	}

	public unsafe static void RemoveDiseaseEmitter(int cb_handle, int sim_handle)
	{
		Debug.Assert(Sim.IsValidHandle(sim_handle));
		RemoveDiseaseEmitterMessage* ptr = stackalloc RemoveDiseaseEmitterMessage[1];
		ptr->handle = sim_handle;
		ptr->callbackIdx = cb_handle;
		Sim.SIM_HandleMessage(468135926, sizeof(RemoveDiseaseEmitterMessage), (byte*)ptr);
	}

	public unsafe static void SetSavedOptionValue(SimSavedOptions option, int zero_or_one)
	{
		SetSavedOptionsMessage* ptr = stackalloc SetSavedOptionsMessage[1];
		if (zero_or_one == 0)
		{
			byte* clearBits = &ptr->clearBits;
			*clearBits = (byte)((uint)(*clearBits) | (uint)option);
			ptr->setBits = 0;
		}
		else
		{
			ptr->clearBits = 0;
			byte* setBits = &ptr->setBits;
			*setBits = (byte)((uint)(*setBits) | (uint)option);
		}
		Sim.SIM_HandleMessage(1154135737, sizeof(SetSavedOptionsMessage), (byte*)ptr);
	}

	private static void WriteKleiString(this BinaryWriter writer, string str)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(str);
		writer.Write(bytes.Length);
		if (bytes.Length != 0)
		{
			writer.Write(bytes);
		}
	}

	public unsafe static void CreateSimElementsTable(List<Element> elements)
	{
		MemoryStream memoryStream = new MemoryStream(Marshal.SizeOf(typeof(int)) + Marshal.SizeOf(typeof(Sim.Element)) * elements.Count);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		Debug.Assert(elements.Count < 255, "SimDLL internals assume there are fewer than 255 elements");
		binaryWriter.Write(elements.Count);
		for (int i = 0; i < elements.Count; i++)
		{
			new Sim.Element(elements[i], elements).Write(binaryWriter);
		}
		for (int j = 0; j < elements.Count; j++)
		{
			binaryWriter.WriteKleiString(UI.StripLinkFormatting(elements[j].name));
		}
		byte[] buffer = memoryStream.GetBuffer();
		fixed (byte* msg = buffer)
		{
			Sim.SIM_HandleMessage(1108437482, buffer.Length, msg);
		}
	}

	public unsafe static void CreateDiseaseTable(Diseases diseases)
	{
		MemoryStream memoryStream = new MemoryStream(1024);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(diseases.Count);
		List<Element> elements = ElementLoader.elements;
		binaryWriter.Write(elements.Count);
		for (int i = 0; i < diseases.Count; i++)
		{
			Disease disease = diseases[i];
			binaryWriter.WriteKleiString(UI.StripLinkFormatting(disease.Name));
			binaryWriter.Write(disease.id.GetHashCode());
			binaryWriter.Write(disease.strength);
			disease.temperatureRange.Write(binaryWriter);
			disease.temperatureHalfLives.Write(binaryWriter);
			disease.pressureRange.Write(binaryWriter);
			disease.pressureHalfLives.Write(binaryWriter);
			for (int j = 0; j < elements.Count; j++)
			{
				ElemGrowthInfo elemGrowthInfo = disease.elemGrowthInfo[j];
				elemGrowthInfo.Write(binaryWriter);
			}
		}
		fixed (byte* msg = memoryStream.GetBuffer())
		{
			Sim.SIM_HandleMessage(825301935, (int)memoryStream.Length, msg);
		}
	}

	public unsafe static void DefineWorldOffsets(List<WorldOffsetData> worldOffsets)
	{
		MemoryStream memoryStream = new MemoryStream(1024);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(worldOffsets.Count);
		foreach (WorldOffsetData worldOffset in worldOffsets)
		{
			binaryWriter.Write(worldOffset.worldOffsetX);
			binaryWriter.Write(worldOffset.worldOffsetY);
			binaryWriter.Write(worldOffset.worldSizeX);
			binaryWriter.Write(worldOffset.worldSizeY);
		}
		fixed (byte* msg = memoryStream.GetBuffer())
		{
			Sim.SIM_HandleMessage(-895846551, (int)memoryStream.Length, msg);
		}
	}

	public static void SimDataInitializeFromCells(int width, int height, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, bool headless)
	{
		MemoryStream memoryStream = new MemoryStream(Marshal.SizeOf(typeof(int)) + Marshal.SizeOf(typeof(int)) + Marshal.SizeOf(typeof(Sim.Cell)) * width * height + Marshal.SizeOf(typeof(float)) * width * height + Marshal.SizeOf(typeof(Sim.DiseaseCell)) * width * height);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(width);
		binaryWriter.Write(height);
		bool value = Sim.IsRadiationEnabled();
		binaryWriter.Write(value);
		binaryWriter.Write(headless);
		int num = width * height;
		for (int i = 0; i < num; i++)
		{
			cells[i].Write(binaryWriter);
		}
		for (int j = 0; j < num; j++)
		{
			binaryWriter.Write(bgTemp[j]);
		}
		for (int k = 0; k < num; k++)
		{
			dc[k].Write(binaryWriter);
		}
		byte[] buffer = memoryStream.GetBuffer();
		Sim.HandleMessage(SimMessageHashes.SimData_InitializeFromCells, buffer.Length, buffer);
	}

	public static void SimDataResizeGridAndInitializeVacuumCells(Vector2I grid_size, int width, int height, int x_offset, int y_offset)
	{
		MemoryStream memoryStream = new MemoryStream(Marshal.SizeOf(typeof(int)) + Marshal.SizeOf(typeof(int)));
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(grid_size.x);
		binaryWriter.Write(grid_size.y);
		binaryWriter.Write(width);
		binaryWriter.Write(height);
		binaryWriter.Write(x_offset);
		binaryWriter.Write(y_offset);
		byte[] buffer = memoryStream.GetBuffer();
		Sim.HandleMessage(SimMessageHashes.SimData_ResizeAndInitializeVacuumCells, buffer.Length, buffer);
	}

	public static void SimDataFreeCells(int width, int height, int x_offset, int y_offset)
	{
		MemoryStream memoryStream = new MemoryStream(Marshal.SizeOf(typeof(int)) + Marshal.SizeOf(typeof(int)));
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(width);
		binaryWriter.Write(height);
		binaryWriter.Write(x_offset);
		binaryWriter.Write(y_offset);
		byte[] buffer = memoryStream.GetBuffer();
		Sim.HandleMessage(SimMessageHashes.SimData_FreeCells, buffer.Length, buffer);
	}

	public unsafe static void Dig(int gameCell, int callbackIdx = -1, bool skipEvent = false)
	{
		if (Grid.IsValidCell(gameCell))
		{
			DigMessage* ptr = stackalloc DigMessage[1];
			ptr->cellIdx = gameCell;
			ptr->callbackIdx = callbackIdx;
			ptr->skipEvent = skipEvent;
			Sim.SIM_HandleMessage(833038498, sizeof(DigMessage), (byte*)ptr);
		}
	}

	public unsafe static void SetInsulation(int gameCell, float value)
	{
		if (Grid.IsValidCell(gameCell))
		{
			SetCellFloatValueMessage* ptr = stackalloc SetCellFloatValueMessage[1];
			ptr->cellIdx = gameCell;
			ptr->value = value;
			Sim.SIM_HandleMessage(-898773121, sizeof(SetCellFloatValueMessage), (byte*)ptr);
		}
	}

	public unsafe static void SetStrength(int gameCell, int weight, float strengthMultiplier)
	{
		if (Grid.IsValidCell(gameCell))
		{
			SetCellFloatValueMessage* ptr = stackalloc SetCellFloatValueMessage[1];
			ptr->cellIdx = gameCell;
			int num = (int)(strengthMultiplier * 4f) & 0x7F;
			int num2 = ((weight & 1) << 7) | num;
			ptr->value = (int)(byte)num2;
			Sim.SIM_HandleMessage(1593243982, sizeof(SetCellFloatValueMessage), (byte*)ptr);
		}
	}

	public unsafe static void SetCellProperties(int gameCell, byte properties)
	{
		if (Grid.IsValidCell(gameCell))
		{
			CellPropertiesMessage* ptr = stackalloc CellPropertiesMessage[1];
			ptr->cellIdx = gameCell;
			ptr->properties = properties;
			ptr->set = 1;
			Sim.SIM_HandleMessage(-469311643, sizeof(CellPropertiesMessage), (byte*)ptr);
		}
	}

	public unsafe static void ClearCellProperties(int gameCell, byte properties)
	{
		if (Grid.IsValidCell(gameCell))
		{
			CellPropertiesMessage* ptr = stackalloc CellPropertiesMessage[1];
			ptr->cellIdx = gameCell;
			ptr->properties = properties;
			ptr->set = 0;
			Sim.SIM_HandleMessage(-469311643, sizeof(CellPropertiesMessage), (byte*)ptr);
		}
	}

	public unsafe static void ModifyCell(int gameCell, int elementIdx, float temperature, float mass, byte disease_idx, int disease_count, ReplaceType replace_type = ReplaceType.None, bool do_vertical_solid_displacement = false, int callbackIdx = -1)
	{
		if (Grid.IsValidCell(gameCell))
		{
			Element element = ElementLoader.elements[elementIdx];
			if (element.maxMass == 0f && mass > element.maxMass)
			{
				Debug.LogWarningFormat("Invalid cell modification (mass greater than element maximum): Cell={0}, EIdx={1}, T={2}, M={3}, {4} max mass = {5}", gameCell, elementIdx, temperature, mass, element.id, element.maxMass);
				mass = element.maxMass;
			}
			if (temperature < 0f || temperature > 10000f)
			{
				Debug.LogWarningFormat("Invalid cell modification (temp out of bounds): Cell={0}, EIdx={1}, T={2}, M={3}, {4} default temp = {5}", gameCell, elementIdx, temperature, mass, element.id, element.defaultValues.temperature);
				temperature = element.defaultValues.temperature;
			}
			if (temperature == 0f && mass > 0f)
			{
				Debug.LogWarningFormat("Invalid cell modification (zero temp with non-zero mass): Cell={0}, EIdx={1}, T={2}, M={3}, {4} default temp = {5}", gameCell, elementIdx, temperature, mass, element.id, element.defaultValues.temperature);
				temperature = element.defaultValues.temperature;
			}
			ModifyCellMessage* ptr = stackalloc ModifyCellMessage[1];
			ptr->cellIdx = gameCell;
			ptr->callbackIdx = callbackIdx;
			ptr->temperature = temperature;
			ptr->mass = mass;
			ptr->elementIdx = (byte)elementIdx;
			ptr->replaceType = (byte)replace_type;
			ptr->diseaseIdx = disease_idx;
			ptr->diseaseCount = disease_count;
			ptr->addSubType = (byte)((!do_vertical_solid_displacement) ? 1u : 0u);
			Sim.SIM_HandleMessage(-1252920804, sizeof(ModifyCellMessage), (byte*)ptr);
		}
	}

	public unsafe static void ModifyDiseaseOnCell(int gameCell, byte disease_idx, int disease_delta)
	{
		CellDiseaseModification* ptr = stackalloc CellDiseaseModification[1];
		ptr->cellIdx = gameCell;
		ptr->diseaseIdx = disease_idx;
		ptr->diseaseCount = disease_delta;
		Sim.SIM_HandleMessage(-1853671274, sizeof(CellDiseaseModification), (byte*)ptr);
	}

	public unsafe static void ModifyRadiationOnCell(int gameCell, float radiationDelta, int callbackIdx = -1)
	{
		CellRadiationModification* ptr = stackalloc CellRadiationModification[1];
		ptr->cellIdx = gameCell;
		ptr->radiationDelta = radiationDelta;
		ptr->callbackIdx = callbackIdx;
		Sim.SIM_HandleMessage(-1914877797, sizeof(CellRadiationModification), (byte*)ptr);
	}

	public unsafe static void ModifyRadiationParams(RadiationParams type, float value)
	{
		RadiationParamsModification* ptr = stackalloc RadiationParamsModification[1];
		ptr->RadiationParamsType = (int)type;
		ptr->value = value;
		Sim.SIM_HandleMessage(377112707, sizeof(RadiationParamsModification), (byte*)ptr);
	}

	public static int GetElementIndex(SimHashes element)
	{
		int result = -1;
		List<Element> elements = ElementLoader.elements;
		for (int i = 0; i < elements.Count; i++)
		{
			if (elements[i].id == element)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public unsafe static void ConsumeMass(int gameCell, SimHashes element, float mass, byte radius, int callbackIdx = -1)
	{
		if (Grid.IsValidCell(gameCell))
		{
			int elementIndex = ElementLoader.GetElementIndex(element);
			MassConsumptionMessage* ptr = stackalloc MassConsumptionMessage[1];
			ptr->cellIdx = gameCell;
			ptr->callbackIdx = callbackIdx;
			ptr->mass = mass;
			ptr->elementIdx = (byte)elementIndex;
			ptr->radius = radius;
			Sim.SIM_HandleMessage(1727657959, sizeof(MassConsumptionMessage), (byte*)ptr);
		}
	}

	public unsafe static void EmitMass(int gameCell, byte element_idx, float mass, float temperature, byte disease_idx, int disease_count, int callbackIdx = -1)
	{
		if (Grid.IsValidCell(gameCell))
		{
			MassEmissionMessage* ptr = stackalloc MassEmissionMessage[1];
			ptr->cellIdx = gameCell;
			ptr->callbackIdx = callbackIdx;
			ptr->mass = mass;
			ptr->temperature = temperature;
			ptr->elementIdx = element_idx;
			ptr->diseaseIdx = disease_idx;
			ptr->diseaseCount = disease_count;
			Sim.SIM_HandleMessage(797274363, sizeof(MassEmissionMessage), (byte*)ptr);
		}
	}

	public unsafe static void ConsumeDisease(int game_cell, float percent_to_consume, int max_to_consume, int callback_idx)
	{
		if (Grid.IsValidCell(game_cell))
		{
			ConsumeDiseaseMessage* ptr = stackalloc ConsumeDiseaseMessage[1];
			ptr->callbackIdx = callback_idx;
			ptr->gameCell = game_cell;
			ptr->percentToConsume = percent_to_consume;
			ptr->maxToConsume = max_to_consume;
			Sim.SIM_HandleMessage(-1019841536, sizeof(ConsumeDiseaseMessage), (byte*)ptr);
		}
	}

	public static void AddRemoveSubstance(int gameCell, SimHashes new_element, CellAddRemoveSubstanceEvent ev, float mass, float temperature, byte disease_idx, int disease_count, bool do_vertical_solid_displacement = true, int callbackIdx = -1)
	{
		int elementIndex = GetElementIndex(new_element);
		AddRemoveSubstance(gameCell, elementIndex, ev, mass, temperature, disease_idx, disease_count, do_vertical_solid_displacement, callbackIdx);
	}

	public static void AddRemoveSubstance(int gameCell, int elementIdx, CellAddRemoveSubstanceEvent ev, float mass, float temperature, byte disease_idx, int disease_count, bool do_vertical_solid_displacement = true, int callbackIdx = -1)
	{
		if (elementIdx != -1)
		{
			Element element = ElementLoader.elements[elementIdx];
			float temperature2 = ((temperature != -1f) ? temperature : element.defaultValues.temperature);
			ModifyCell(gameCell, elementIdx, temperature2, mass, disease_idx, disease_count, ReplaceType.None, do_vertical_solid_displacement, callbackIdx);
		}
	}

	public static void ReplaceElement(int gameCell, SimHashes new_element, CellElementEvent ev, float mass, float temperature = -1f, byte diseaseIdx = byte.MaxValue, int diseaseCount = 0, int callbackIdx = -1)
	{
		int elementIndex = GetElementIndex(new_element);
		if (elementIndex != -1)
		{
			Element element = ElementLoader.elements[elementIndex];
			float temperature2 = ((temperature != -1f) ? temperature : element.defaultValues.temperature);
			ModifyCell(gameCell, elementIndex, temperature2, mass, diseaseIdx, diseaseCount, ReplaceType.Replace, do_vertical_solid_displacement: false, callbackIdx);
		}
	}

	public static void ReplaceAndDisplaceElement(int gameCell, SimHashes new_element, CellElementEvent ev, float mass, float temperature = -1f, byte disease_idx = byte.MaxValue, int disease_count = 0, int callbackIdx = -1)
	{
		int elementIndex = GetElementIndex(new_element);
		if (elementIndex != -1)
		{
			Element element = ElementLoader.elements[elementIndex];
			float temperature2 = ((temperature != -1f) ? temperature : element.defaultValues.temperature);
			ModifyCell(gameCell, elementIndex, temperature2, mass, disease_idx, disease_count, ReplaceType.ReplaceAndDisplace, do_vertical_solid_displacement: false, callbackIdx);
		}
	}

	public unsafe static void ModifyEnergy(int gameCell, float kilojoules, float max_temperature, EnergySourceID id)
	{
		if (Grid.IsValidCell(gameCell))
		{
			if (max_temperature <= 0f)
			{
				Debug.LogError("invalid max temperature for cell energy modification");
				return;
			}
			ModifyCellEnergyMessage* ptr = stackalloc ModifyCellEnergyMessage[1];
			ptr->cellIdx = gameCell;
			ptr->kilojoules = kilojoules;
			ptr->maxTemperature = max_temperature;
			ptr->id = (int)id;
			Sim.SIM_HandleMessage(818320644, sizeof(ModifyCellEnergyMessage), (byte*)ptr);
		}
	}

	public static void ModifyMass(int gameCell, float mass, byte disease_idx, int disease_count, CellModifyMassEvent ev, float temperature = -1f, SimHashes element = SimHashes.Vacuum)
	{
		if (element != SimHashes.Vacuum)
		{
			int elementIndex = GetElementIndex(element);
			if (elementIndex != -1)
			{
				if (temperature == -1f)
				{
					temperature = ElementLoader.elements[elementIndex].defaultValues.temperature;
				}
				ModifyCell(gameCell, elementIndex, temperature, mass, disease_idx, disease_count);
			}
		}
		else
		{
			ModifyCell(gameCell, 0, temperature, mass, disease_idx, disease_count);
		}
	}

	public unsafe static void CreateElementInteractions(ElementInteraction[] interactions)
	{
		fixed (ElementInteraction* interactions2 = interactions)
		{
			CreateElementInteractionsMsg* ptr = stackalloc CreateElementInteractionsMsg[1];
			ptr->numInteractions = interactions.Length;
			ptr->interactions = interactions2;
			Sim.SIM_HandleMessage(-930289787, sizeof(CreateElementInteractionsMsg), (byte*)ptr);
		}
	}

	public unsafe static void NewGameFrame(float elapsed_seconds, List<Pair<Vector2I, Vector2I>> activeRegions)
	{
		Debug.Assert(activeRegions.Count > 0, "NewGameFrame cannot be called with zero activeRegions");
		Sim.NewGameFrame* ptr = stackalloc Sim.NewGameFrame[activeRegions.Count];
		Sim.NewGameFrame* ptr2 = ptr;
		foreach (Pair<Vector2I, Vector2I> activeRegion in activeRegions)
		{
			Pair<Vector2I, Vector2I> pair = activeRegion;
			pair.first = new Vector2I(MathUtil.Clamp(0, Grid.WidthInCells - 1, activeRegion.first.x), MathUtil.Clamp(0, Grid.HeightInCells - 1, activeRegion.first.y));
			pair.second = new Vector2I(MathUtil.Clamp(0, Grid.WidthInCells - 1, activeRegion.second.x), MathUtil.Clamp(0, Grid.HeightInCells - 1, activeRegion.second.y));
			ptr2->elapsedSeconds = elapsed_seconds;
			ptr2->minX = pair.first.x;
			ptr2->minY = pair.first.y;
			ptr2->maxX = pair.second.x;
			ptr2->maxY = pair.second.y;
			ptr2++;
		}
		Sim.SIM_HandleMessage(-775326397, sizeof(Sim.NewGameFrame) * activeRegions.Count, (byte*)ptr);
	}

	public unsafe static void SetDebugProperties(Sim.DebugProperties properties)
	{
		Sim.DebugProperties* ptr = stackalloc Sim.DebugProperties[1];
		*ptr = properties;
		ptr->buildingTemperatureScale = properties.buildingTemperatureScale;
		Sim.SIM_HandleMessage(-1683118492, sizeof(Sim.DebugProperties), (byte*)ptr);
	}

	public unsafe static void ModifyCellWorldZone(int cell, byte zone_id)
	{
		CellWorldZoneModification* ptr = stackalloc CellWorldZoneModification[1];
		ptr->cell = cell;
		ptr->zoneID = zone_id;
		Sim.SIM_HandleMessage(-449718014, sizeof(CellWorldZoneModification), (byte*)ptr);
	}
}
