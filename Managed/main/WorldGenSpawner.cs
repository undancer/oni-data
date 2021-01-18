using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using ProcGen;
using ProcGenGame;
using TemplateClasses;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/WorldGenSpawner")]
public class WorldGenSpawner : KMonoBehaviour
{
	private class Spawnable
	{
		public delegate GameObject PlaceEntityFn(Prefab prefab, int root_cell);

		private HandleVector<int>.Handle fogOfWarPartitionerEntry;

		private HandleVector<int>.Handle solidChangedPartitionerEntry;

		public Prefab spawnInfo
		{
			get;
			private set;
		}

		public bool isSpawned
		{
			get;
			private set;
		}

		public int cell
		{
			get;
			private set;
		}

		public Spawnable(Prefab spawn_info)
		{
			spawnInfo = spawn_info;
			int num = Grid.XYToCell(spawnInfo.location_x, spawnInfo.location_y);
			GameObject prefab = Assets.GetPrefab(spawn_info.id);
			if (prefab != null)
			{
				WorldSpawnableMonitor.Def def = prefab.GetDef<WorldSpawnableMonitor.Def>();
				if (def != null && def.adjustSpawnLocationCb != null)
				{
					num = def.adjustSpawnLocationCb(num);
				}
			}
			cell = num;
			Debug.Assert(Grid.IsValidCell(cell));
			if (Grid.Spawnable[cell] > 0)
			{
				TrySpawn();
			}
			else
			{
				fogOfWarPartitionerEntry = GameScenePartitioner.Instance.Add("WorldGenSpawner.OnReveal", this, cell, GameScenePartitioner.Instance.fogOfWarChangedLayer, OnReveal);
			}
		}

		private void OnReveal(object data)
		{
			if (Grid.Spawnable[cell] > 0)
			{
				TrySpawn();
			}
		}

		private void OnSolidChanged(object data)
		{
			if (!Grid.Solid[cell])
			{
				GameScenePartitioner.Instance.Free(ref solidChangedPartitionerEntry);
				Game.Instance.GetComponent<EntombedItemVisualizer>().RemoveItem(cell);
				Spawn();
			}
		}

		public void FreeResources()
		{
			if (solidChangedPartitionerEntry.IsValid())
			{
				GameScenePartitioner.Instance.Free(ref solidChangedPartitionerEntry);
				if (Game.Instance != null)
				{
					Game.Instance.GetComponent<EntombedItemVisualizer>().RemoveItem(cell);
				}
			}
			GameScenePartitioner.Instance.Free(ref fogOfWarPartitionerEntry);
			isSpawned = true;
		}

		public void TrySpawn()
		{
			if (isSpawned || solidChangedPartitionerEntry.IsValid())
			{
				return;
			}
			WorldContainer world = ClusterManager.Instance.GetWorld(Grid.WorldIdx[cell]);
			bool flag = world != null && world.IsDiscovered;
			GameObject prefab = Assets.GetPrefab(GetPrefabTag());
			if (prefab != null)
			{
				if (flag | prefab.HasTag(GameTags.WarpTech))
				{
					GameScenePartitioner.Instance.Free(ref fogOfWarPartitionerEntry);
					bool flag2 = false;
					if (prefab.GetComponent<Pickupable>() != null && !prefab.HasTag(GameTags.Creatures.Digger))
					{
						flag2 = true;
					}
					else if (prefab.GetDef<BurrowMonitor.Def>() != null)
					{
						flag2 = true;
					}
					if (flag2 && Grid.Solid[cell])
					{
						solidChangedPartitionerEntry = GameScenePartitioner.Instance.Add("WorldGenSpawner.OnSolidChanged", this, cell, GameScenePartitioner.Instance.solidChangedLayer, OnSolidChanged);
						Game.Instance.GetComponent<EntombedItemVisualizer>().AddItem(cell);
					}
					else
					{
						Spawn();
					}
				}
			}
			else if (flag)
			{
				GameScenePartitioner.Instance.Free(ref fogOfWarPartitionerEntry);
				Spawn();
			}
		}

		private Tag GetPrefabTag()
		{
			Mob mob = SettingsCache.mobs.GetMob(spawnInfo.id);
			if (mob != null && mob.prefabName != null)
			{
				return new Tag(mob.prefabName);
			}
			return new Tag(spawnInfo.id);
		}

		private void Spawn()
		{
			isSpawned = true;
			GameObject gameObject = GetSpawnableCallback(spawnInfo.type)(spawnInfo, 0);
			if (gameObject != null && (bool)gameObject)
			{
				gameObject.SetActive(value: true);
				gameObject.Trigger(1119167081);
			}
			FreeResources();
		}

		public static PlaceEntityFn GetSpawnableCallback(Prefab.Type type)
		{
			return type switch
			{
				Prefab.Type.Building => TemplateLoader.PlaceBuilding, 
				Prefab.Type.Ore => TemplateLoader.PlaceElementalOres, 
				Prefab.Type.Other => TemplateLoader.PlaceOtherEntities, 
				Prefab.Type.Pickupable => TemplateLoader.PlacePickupables, 
				_ => TemplateLoader.PlaceOtherEntities, 
			};
		}
	}

	[Serialize]
	private Prefab[] spawnInfos;

	[Serialize]
	private bool hasPlacedTemplates;

	private List<Spawnable> spawnables = new List<Spawnable>();

	public bool SpawnsRemain()
	{
		return spawnables.Count > 0;
	}

	public void SpawnEverything()
	{
		for (int i = 0; i < spawnables.Count; i++)
		{
			spawnables[i].TrySpawn();
		}
	}

	public void SpawnTag(string id)
	{
		for (int i = 0; i < spawnables.Count; i++)
		{
			if (spawnables[i].spawnInfo.id == id)
			{
				spawnables[i].TrySpawn();
			}
		}
	}

	public void ClearSpawnersInArea(Vector2 root_position, CellOffset[] area)
	{
		for (int i = 0; i < spawnables.Count; i++)
		{
			if (Grid.IsCellOffsetOf(Grid.PosToCell(root_position), spawnables[i].cell, area))
			{
				spawnables[i].FreeResources();
			}
		}
	}

	protected override void OnSpawn()
	{
		if (!hasPlacedTemplates)
		{
			Debug.Assert(SaveLoader.Instance.ClusterLayout != null, "Trying to place templates for an already-loaded save, no worldgen data available");
			DoReveal(SaveLoader.Instance.ClusterLayout);
			PlaceTemplates(SaveLoader.Instance.ClusterLayout);
			hasPlacedTemplates = true;
		}
		if (spawnInfos != null)
		{
			for (int i = 0; i < spawnInfos.Length; i++)
			{
				AddSpawnable(spawnInfos[i]);
			}
		}
	}

	[OnSerializing]
	private void OnSerializing()
	{
		List<Prefab> list = new List<Prefab>();
		for (int i = 0; i < spawnables.Count; i++)
		{
			Spawnable spawnable = spawnables[i];
			if (!spawnable.isSpawned)
			{
				list.Add(spawnable.spawnInfo);
			}
		}
		spawnInfos = list.ToArray();
	}

	private void AddSpawnable(Prefab prefab)
	{
		spawnables.Add(new Spawnable(prefab));
	}

	public void AddLegacySpawner(Tag tag, int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		AddSpawnable(new Prefab(tag.Name, Prefab.Type.Other, vector2I.x, vector2I.y, SimHashes.Carbon));
	}

	public List<Tag> GetUnspawnedWithType<T>(int worldID) where T : KMonoBehaviour
	{
		List<Tag> list = new List<Tag>();
		foreach (Spawnable item in spawnables.FindAll((Spawnable match) => !match.isSpawned && Grid.WorldIdx[match.cell] == worldID && Assets.GetPrefab(match.spawnInfo.id) != null && (Object)Assets.GetPrefab(match.spawnInfo.id).GetComponent<T>() != (Object)null))
		{
			list.Add(item.spawnInfo.id);
		}
		return list;
	}

	public List<Tag> GetSpawnersWithTag(Tag tag, int worldID, bool includeSpawned = false)
	{
		List<Tag> list = new List<Tag>();
		foreach (Spawnable item in spawnables.FindAll((Spawnable match) => (includeSpawned || !match.isSpawned) && Grid.WorldIdx[match.cell] == worldID && match.spawnInfo.id == tag))
		{
			list.Add(item.spawnInfo.id);
		}
		return list;
	}

	private void PlaceTemplates(Cluster clusterLayout)
	{
		spawnables = new List<Spawnable>();
		foreach (WorldGen world in clusterLayout.worlds)
		{
			foreach (Prefab building in world.SpawnData.buildings)
			{
				building.location_x += world.data.world.offset.x;
				building.location_y += world.data.world.offset.y;
				building.type = Prefab.Type.Building;
				AddSpawnable(building);
			}
			foreach (Prefab elementalOre in world.SpawnData.elementalOres)
			{
				elementalOre.location_x += world.data.world.offset.x;
				elementalOre.location_y += world.data.world.offset.y;
				elementalOre.type = Prefab.Type.Ore;
				AddSpawnable(elementalOre);
			}
			foreach (Prefab otherEntity in world.SpawnData.otherEntities)
			{
				otherEntity.location_x += world.data.world.offset.x;
				otherEntity.location_y += world.data.world.offset.y;
				otherEntity.type = Prefab.Type.Other;
				AddSpawnable(otherEntity);
			}
			foreach (Prefab pickupable in world.SpawnData.pickupables)
			{
				pickupable.location_x += world.data.world.offset.x;
				pickupable.location_y += world.data.world.offset.y;
				pickupable.type = Prefab.Type.Pickupable;
				AddSpawnable(pickupable);
			}
			world.SpawnData.buildings.Clear();
			world.SpawnData.elementalOres.Clear();
			world.SpawnData.otherEntities.Clear();
			world.SpawnData.pickupables.Clear();
		}
	}

	private void DoReveal(Cluster clusterLayout)
	{
		foreach (WorldGen world in clusterLayout.worlds)
		{
			Game.Instance.Reset(world.SpawnData, world.WorldOffset);
		}
		for (int i = 0; i < Grid.CellCount; i++)
		{
			Grid.Revealed[i] = false;
			Grid.Spawnable[i] = 0;
		}
		float innerRadius = 16.5f;
		int radius = 18;
		Vector2I baseStartPos = clusterLayout.currentWorld.SpawnData.baseStartPos;
		baseStartPos += clusterLayout.currentWorld.WorldOffset;
		GridVisibility.Reveal(baseStartPos.x, baseStartPos.y, radius, innerRadius);
	}
}
