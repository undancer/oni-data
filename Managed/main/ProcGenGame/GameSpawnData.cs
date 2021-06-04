using System.Collections.Generic;
using Klei.CustomSettings;
using KSerialization;
using TemplateClasses;

namespace ProcGenGame
{
	[SerializationConfig(MemberSerialization.OptOut)]
	public class GameSpawnData
	{
		public Vector2I baseStartPos;

		public List<Prefab> buildings = new List<Prefab>();

		public List<Prefab> pickupables = new List<Prefab>();

		public List<Prefab> elementalOres = new List<Prefab>();

		public List<Prefab> otherEntities = new List<Prefab>();

		public List<KeyValuePair<Vector2I, bool>> preventFoWReveal = new List<KeyValuePair<Vector2I, bool>>();

		public void AddRange(IEnumerable<KeyValuePair<int, string>> newItems)
		{
			foreach (KeyValuePair<int, string> newItem in newItems)
			{
				Vector2I vector2I = Grid.CellToXY(newItem.Key);
				Prefab item = new Prefab(newItem.Value, Prefab.Type.Other, vector2I.x, vector2I.y, (SimHashes)0);
				otherEntities.Add(item);
			}
		}

		public void AddTemplate(TemplateContainer template, Vector2I position, ref Dictionary<int, int> claimedCells)
		{
			int cell = Grid.XYToCell(position.x, position.y);
			bool flag = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Teleporters).id == "Enabled";
			if (template.buildings != null)
			{
				foreach (Prefab building in template.buildings)
				{
					if (!claimedCells.ContainsKey(Grid.OffsetCell(cell, building.location_x, building.location_y)) && (flag || !IsWarpTeleporter(building)))
					{
						buildings.Add(building.Clone(position));
					}
				}
			}
			if (template.pickupables != null)
			{
				foreach (Prefab pickupable in template.pickupables)
				{
					if (!claimedCells.ContainsKey(Grid.OffsetCell(cell, pickupable.location_x, pickupable.location_y)))
					{
						pickupables.Add(pickupable.Clone(position));
					}
				}
			}
			if (template.elementalOres != null)
			{
				foreach (Prefab elementalOre in template.elementalOres)
				{
					if (!claimedCells.ContainsKey(Grid.OffsetCell(cell, elementalOre.location_x, elementalOre.location_y)))
					{
						elementalOres.Add(elementalOre.Clone(position));
					}
				}
			}
			if (template.otherEntities != null)
			{
				foreach (Prefab otherEntity in template.otherEntities)
				{
					if (!claimedCells.ContainsKey(Grid.OffsetCell(cell, otherEntity.location_x, otherEntity.location_y)) && (flag || !IsWarpTeleporter(otherEntity)))
					{
						otherEntities.Add(otherEntity.Clone(position));
					}
				}
			}
			if (template.cells == null)
			{
				return;
			}
			for (int i = 0; i < template.cells.Count; i++)
			{
				int key = Grid.XYToCell(position.x + template.cells[i].location_x, position.y + template.cells[i].location_y);
				if (!claimedCells.ContainsKey(key))
				{
					claimedCells[key] = 1;
					preventFoWReveal.Add(new KeyValuePair<Vector2I, bool>(new Vector2I(position.x + template.cells[i].location_x, position.y + template.cells[i].location_y), template.cells[i].preventFoWReveal));
				}
				else
				{
					claimedCells[key]++;
				}
			}
		}

		private bool IsWarpTeleporter(Prefab prefab)
		{
			return prefab.id == "WarpPortal" || prefab.id == WarpReceiverConfig.ID || prefab.id == "WarpConduitSender" || prefab.id == "WarpConduitReceiver";
		}
	}
}
