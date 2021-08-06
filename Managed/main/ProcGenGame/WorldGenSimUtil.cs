using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using KSerialization;
using ProcGen;
using STRINGS;
using TemplateClasses;

namespace ProcGenGame
{
	public static class WorldGenSimUtil
	{
		private const int STEPS = 500;

		public unsafe static bool DoSettleSim(WorldGenSettings settings, ref Sim.Cell[] cells, ref float[] bgTemp, ref Sim.DiseaseCell[] dcs, WorldGen.OfflineCallbackFunction updateProgressFn, Data data, List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets, Action<OfflineWorldGen.ErrorInfo> error_cb, int baseId)
		{
			Sim.SIM_Initialize(Sim.DLL_MessageHandler);
			SimMessages.CreateSimElementsTable(ElementLoader.elements);
			SimMessages.CreateDiseaseTable(WorldGen.diseaseStats);
			SimMessages.SimDataInitializeFromCells(Grid.WidthInCells, Grid.HeightInCells, cells, bgTemp, dcs, headless: true);
			updateProgressFn(UI.WORLDGEN.SETTLESIM.key, 0f, WorldGenProgressStages.Stages.SettleSim);
			Sim.Start();
			byte[] array = new byte[Grid.CellCount];
			for (int i = 0; i < Grid.CellCount; i++)
			{
				array[i] = byte.MaxValue;
			}
			Vector2I a = new Vector2I(0, 0);
			Vector2I size = data.world.size;
			List<Game.SimActiveRegion> list = new List<Game.SimActiveRegion>();
			Game.SimActiveRegion simActiveRegion = new Game.SimActiveRegion();
			simActiveRegion.region = new Pair<Vector2I, Vector2I>(a, size);
			list.Add(simActiveRegion);
			for (int j = 0; j < 500; j++)
			{
				if (j == 498)
				{
					HashSet<int> hashSet = new HashSet<int>();
					foreach (KeyValuePair<Vector2I, TemplateContainer> templateSpawnTarget in templateSpawnTargets)
					{
						if (templateSpawnTarget.Value.cells == null)
						{
							continue;
						}
						for (int k = 0; k < templateSpawnTarget.Value.cells.Count; k++)
						{
							Cell cell = templateSpawnTarget.Value.cells[k];
							int num = Grid.OffsetCell(Grid.XYToCell(templateSpawnTarget.Key.x, templateSpawnTarget.Key.y), cell.location_x, cell.location_y);
							if (Grid.IsValidCell(num) && !hashSet.Contains(num))
							{
								hashSet.Add(num);
								byte elementIdx = (byte)ElementLoader.GetElementIndex(cell.element);
								float temperature = cell.temperature;
								float mass = cell.mass;
								byte index = WorldGen.diseaseStats.GetIndex(cell.diseaseName);
								int diseaseCount = cell.diseaseCount;
								SimMessages.ModifyCell(num, elementIdx, temperature, mass, index, diseaseCount, SimMessages.ReplaceType.Replace);
							}
						}
					}
				}
				SimMessages.NewGameFrame(0.2f, list);
				IntPtr intPtr = Sim.HandleMessage(SimMessageHashes.PrepareGameData, array.Length, array);
				updateProgressFn(UI.WORLDGEN.SETTLESIM.key, (float)j / 500f, WorldGenProgressStages.Stages.SettleSim);
				if (intPtr == IntPtr.Zero)
				{
					DebugUtil.LogWarningArgs("Unexpected");
					continue;
				}
				Sim.GameDataUpdate* ptr = (Sim.GameDataUpdate*)(void*)intPtr;
				Grid.elementIdx = ptr->elementIdx;
				Grid.temperature = ptr->temperature;
				Grid.mass = ptr->mass;
				Grid.radiation = ptr->radiation;
				Grid.properties = ptr->properties;
				Grid.strengthInfo = ptr->strengthInfo;
				Grid.insulation = ptr->insulation;
				Grid.diseaseIdx = ptr->diseaseIdx;
				Grid.diseaseCount = ptr->diseaseCount;
				Grid.AccumulatedFlowValues = ptr->accumulatedFlow;
				Grid.exposedToSunlight = (byte*)(void*)ptr->propertyTextureExposedToSunlight;
				for (int l = 0; l < ptr->numSubstanceChangeInfo; l++)
				{
					Sim.SubstanceChangeInfo substanceChangeInfo = ptr->substanceChangeInfo[l];
					int cellIdx = substanceChangeInfo.cellIdx;
					cells[cellIdx].elementIdx = ptr->elementIdx[cellIdx];
					cells[cellIdx].insulation = ptr->insulation[cellIdx];
					cells[cellIdx].properties = ptr->properties[cellIdx];
					cells[cellIdx].temperature = ptr->temperature[cellIdx];
					cells[cellIdx].mass = ptr->mass[cellIdx];
					cells[cellIdx].strengthInfo = ptr->strengthInfo[cellIdx];
					dcs[cellIdx].diseaseIdx = ptr->diseaseIdx[cellIdx];
					dcs[cellIdx].elementCount = ptr->diseaseCount[cellIdx];
					Grid.Element[cellIdx] = ElementLoader.elements[substanceChangeInfo.newElemIdx];
				}
				for (int m = 0; m < ptr->numSolidInfo; m++)
				{
					Sim.SolidInfo solidInfo = ptr->solidInfo[m];
					Grid.SetSolid(solid: solidInfo.isSolid != 0, cell: solidInfo.cellIdx, ev: null);
				}
			}
			bool result = SaveSim(settings, data, baseId, error_cb);
			Sim.Shutdown();
			return result;
		}

		private static bool SaveSim(WorldGenSettings settings, Data data, int baseId, Action<OfflineWorldGen.ErrorInfo> error_cb)
		{
			try
			{
				Manager.Clear();
				SimSaveFileStructure simSaveFileStructure = new SimSaveFileStructure();
				for (int i = 0; i < data.overworldCells.Count; i++)
				{
					simSaveFileStructure.worldDetail.overworldCells.Add(new WorldDetailSave.OverworldCell(SettingsCache.GetCachedSubWorld(data.overworldCells[i].node.type).zoneType, data.overworldCells[i]));
				}
				simSaveFileStructure.worldDetail.globalWorldSeed = data.globalWorldSeed;
				simSaveFileStructure.worldDetail.globalWorldLayoutSeed = data.globalWorldLayoutSeed;
				simSaveFileStructure.worldDetail.globalTerrainSeed = data.globalTerrainSeed;
				simSaveFileStructure.worldDetail.globalNoiseSeed = data.globalNoiseSeed;
				simSaveFileStructure.WidthInCells = Grid.WidthInCells;
				simSaveFileStructure.HeightInCells = Grid.HeightInCells;
				simSaveFileStructure.x = data.world.offset.x;
				simSaveFileStructure.y = data.world.offset.y;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (BinaryWriter writer = new BinaryWriter(memoryStream))
					{
						Sim.Save(writer, simSaveFileStructure.x, simSaveFileStructure.y);
					}
					simSaveFileStructure.Sim = memoryStream.ToArray();
				}
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					using (BinaryWriter writer2 = new BinaryWriter(memoryStream2))
					{
						try
						{
							Serializer.Serialize(simSaveFileStructure, writer2);
						}
						catch (Exception ex)
						{
							DebugUtil.LogErrorArgs("Couldn't serialize", ex.Message, ex.StackTrace);
						}
					}
					using BinaryWriter binaryWriter = new BinaryWriter(File.Open(WorldGen.GetSIMSaveFilename(baseId), FileMode.Create));
					Manager.SerializeDirectory(binaryWriter);
					binaryWriter.Write(memoryStream2.ToArray());
				}
				return true;
			}
			catch (Exception ex2)
			{
				error_cb(new OfflineWorldGen.ErrorInfo
				{
					errorDesc = string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, WorldGen.GetSIMSaveFilename(baseId)),
					exception = ex2
				});
				DebugUtil.LogErrorArgs("Couldn't write", ex2.Message, ex2.StackTrace);
				return false;
			}
		}

		public static void LoadSim(int baseCount, List<SimSaveFileStructure> loadedWorlds)
		{
			for (int i = 0; i != baseCount; i++)
			{
				SimSaveFileStructure simSaveFileStructure = new SimSaveFileStructure();
				try
				{
					FastReader reader = new FastReader(File.ReadAllBytes(WorldGen.GetSIMSaveFilename(i)));
					Manager.DeserializeDirectory(reader);
					Deserializer.Deserialize(simSaveFileStructure, reader);
				}
				catch (Exception ex)
				{
					DebugUtil.LogErrorArgs("LoadSim Error!\n", ex.Message, ex.StackTrace);
					return;
				}
				if (simSaveFileStructure.worldDetail == null)
				{
					Debug.LogError("Detail is null for world " + i);
				}
				else
				{
					loadedWorlds.Add(simSaveFileStructure);
				}
			}
		}
	}
}
