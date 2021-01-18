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
		public unsafe static bool DoSettleSim(WorldGenSettings settings, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dcs, WorldGen.OfflineCallbackFunction updateProgressFn, Data data, List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets, Action<OfflineWorldGen.ErrorInfo> error_cb, Action<Sim.Cell[], float[], Sim.DiseaseCell[]> onSettleComplete)
		{
			Sim.SIM_Initialize(Sim.DLL_MessageHandler);
			SimMessages.CreateSimElementsTable(ElementLoader.elements);
			SimMessages.CreateWorldGenHACKDiseaseTable(WorldGen.diseaseIds);
			Sim.DiseaseCell[] dc = new Sim.DiseaseCell[dcs.Length];
			SimMessages.SimDataInitializeFromCells(Grid.WidthInCells, Grid.HeightInCells, cells, bgTemp, dc);
			int num = 500;
			updateProgressFn(UI.WORLDGEN.SETTLESIM.key, 0f, WorldGenProgressStages.Stages.SettleSim);
			Vector2I min = new Vector2I(0, 0);
			Vector2I max = new Vector2I(Grid.WidthInCells, Grid.HeightInCells);
			byte[] bytes = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(memoryStream))
				{
					try
					{
						Sim.Save(writer);
					}
					catch (Exception ex)
					{
						string message = ex.Message;
						string stackTrace = ex.StackTrace;
						WorldGenLogger.LogException(message, stackTrace);
						return updateProgressFn(new StringKey("Exception in Sim Save"), -1f, WorldGenProgressStages.Stages.Failure);
					}
				}
				bytes = memoryStream.ToArray();
			}
			if (Sim.Load(new FastReader(bytes)) != 0)
			{
				updateProgressFn(UI.WORLDGEN.FAILED.key, -1f, WorldGenProgressStages.Stages.Failure);
				return true;
			}
			byte[] array = new byte[Grid.CellCount];
			for (int i = 0; i < Grid.CellCount; i++)
			{
				array[i] = byte.MaxValue;
			}
			for (int j = 0; j < num; j++)
			{
				SimMessages.NewGameFrame(0.2f, min, max);
				IntPtr intPtr = Sim.HandleMessage(SimMessageHashes.PrepareGameData, array.Length, array);
				updateProgressFn(UI.WORLDGEN.SETTLESIM.key, (float)j / (float)num * 100f, WorldGenProgressStages.Stages.SettleSim);
				if (intPtr == IntPtr.Zero)
				{
					continue;
				}
				Sim.GameDataUpdate* ptr = (Sim.GameDataUpdate*)(void*)intPtr;
				for (int k = 0; k < ptr->numSubstanceChangeInfo; k++)
				{
					int cellIdx = ptr->substanceChangeInfo[k].cellIdx;
					cells[cellIdx].elementIdx = ptr->elementIdx[cellIdx];
					cells[cellIdx].insulation = ptr->insulation[cellIdx];
					cells[cellIdx].properties = ptr->properties[cellIdx];
					cells[cellIdx].temperature = ptr->temperature[cellIdx];
					cells[cellIdx].mass = ptr->mass[cellIdx];
					cells[cellIdx].strengthInfo = ptr->strengthInfo[cellIdx];
				}
				Cell templateCellData;
				foreach (KeyValuePair<Vector2I, TemplateContainer> templateSpawnTarget in templateSpawnTargets)
				{
					for (int l = 0; l < templateSpawnTarget.Value.cells.Count; l++)
					{
						templateCellData = templateSpawnTarget.Value.cells[l];
						int num2 = Grid.OffsetCell(Grid.XYToCell(templateSpawnTarget.Key.x, templateSpawnTarget.Key.y), templateCellData.location_x, templateCellData.location_y);
						if (Grid.IsValidCell(num2))
						{
							cells[num2].elementIdx = (byte)ElementLoader.GetElementIndex(templateCellData.element);
							cells[num2].temperature = templateCellData.temperature;
							cells[num2].mass = templateCellData.mass;
							dcs[num2].diseaseIdx = (byte)WorldGen.diseaseIds.FindIndex((string name) => name == templateCellData.diseaseName);
							dcs[num2].elementCount = templateCellData.diseaseCount;
						}
					}
				}
			}
			for (int m = 0; m < Grid.CellCount; m++)
			{
				int callbackIdx = ((m == Grid.CellCount - 1) ? 2147481337 : (-1));
				SimMessages.ModifyCell(m, cells[m].elementIdx, cells[m].temperature, cells[m].mass, dcs[m].diseaseIdx, dcs[m].elementCount, SimMessages.ReplaceType.Replace, do_vertical_solid_displacement: false, callbackIdx);
			}
			bool flag = false;
			while (!flag)
			{
				SimMessages.NewGameFrame(0.2f, min, max);
				IntPtr intPtr2 = Sim.HandleMessage(SimMessageHashes.PrepareGameData, array.Length, array);
				if (intPtr2 == IntPtr.Zero)
				{
					continue;
				}
				Sim.GameDataUpdate* ptr2 = (Sim.GameDataUpdate*)(void*)intPtr2;
				for (int n = 0; n < ptr2->numCallbackInfo; n++)
				{
					if (ptr2->callbackInfo[n].callbackIdx == 2147481337)
					{
						flag = true;
						break;
					}
				}
			}
			Sim.HandleMessage(SimMessageHashes.SettleWorldGen, 0, null);
			bool result = SaveSim(settings, data, error_cb);
			onSettleComplete(cells, bgTemp, dcs);
			Sim.Shutdown();
			return result;
		}

		private static bool SaveSim(WorldGenSettings settings, Data data, Action<OfflineWorldGen.ErrorInfo> error_cb)
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
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (BinaryWriter writer = new BinaryWriter(memoryStream))
					{
						Sim.Save(writer);
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
					using BinaryWriter binaryWriter = new BinaryWriter(File.Open(WorldGen.SIM_SAVE_FILENAME, FileMode.Create));
					Manager.SerializeDirectory(binaryWriter);
					binaryWriter.Write(memoryStream2.ToArray());
				}
				return true;
			}
			catch (Exception ex2)
			{
				error_cb(new OfflineWorldGen.ErrorInfo
				{
					errorDesc = string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, WorldGen.SIM_SAVE_FILENAME),
					exception = ex2
				});
				DebugUtil.LogErrorArgs("Couldn't write", ex2.Message, ex2.StackTrace);
				return false;
			}
		}
	}
}
