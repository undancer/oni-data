using System;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;

public class ClusterGrid
{
	public static ClusterGrid Instance;

	public const float NodeDistanceScale = 600f;

	private const float MAX_OFFSET_RADIUS = 0.5f;

	public int numRings;

	private ClusterFogOfWarManager.Instance m_fowManager;

	private Action<object> m_onClusterLocationChangedDelegate;

	public Dictionary<AxialI, List<ClusterGridEntity>> cellContents = new Dictionary<AxialI, List<ClusterGridEntity>>();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	private ClusterFogOfWarManager.Instance GetFOWManager()
	{
		if (m_fowManager == null)
		{
			m_fowManager = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
		}
		return m_fowManager;
	}

	public bool IsValidCell(AxialI cell)
	{
		return cellContents.ContainsKey(cell);
	}

	public ClusterGrid(int numRings)
	{
		Instance = this;
		GenerateGrid(numRings);
		m_onClusterLocationChangedDelegate = OnClusterLocationChanged;
	}

	public ClusterRevealLevel GetCellRevealLevel(AxialI cell)
	{
		return GetFOWManager().GetCellRevealLevel(cell);
	}

	public bool IsCellVisible(AxialI cell)
	{
		return GetFOWManager().IsLocationRevealed(cell);
	}

	public float GetRevealCompleteFraction(AxialI cell)
	{
		return GetFOWManager().GetRevealCompleteFraction(cell);
	}

	public bool IsVisible(ClusterGridEntity entity)
	{
		if (entity.IsVisible)
		{
			return IsCellVisible(entity.Location);
		}
		return false;
	}

	public List<ClusterGridEntity> GetVisibleEntitiesAtCell(AxialI cell)
	{
		if (IsValidCell(cell) && GetFOWManager().IsLocationRevealed(cell))
		{
			return cellContents[cell].Where((ClusterGridEntity entity) => entity.IsVisible).ToList();
		}
		return new List<ClusterGridEntity>();
	}

	public ClusterGridEntity GetVisibleEntityOfLayerAtCell(AxialI cell, EntityLayer entityLayer)
	{
		if (IsValidCell(cell) && GetFOWManager().IsLocationRevealed(cell))
		{
			foreach (ClusterGridEntity item in cellContents[cell])
			{
				if (item.IsVisible && item.Layer == entityLayer)
				{
					return item;
				}
			}
		}
		return null;
	}

	public ClusterGridEntity GetVisibleEntityOfLayerAtAdjacentCell(AxialI cell, EntityLayer entityLayer)
	{
		return AxialUtil.GetRing(cell, 1).SelectMany(GetVisibleEntitiesAtCell).FirstOrDefault((ClusterGridEntity entity) => entity.Layer == entityLayer);
	}

	public List<ClusterGridEntity> GetHiddenEntitiesOfLayerAtCell(AxialI cell, EntityLayer entityLayer)
	{
		return (from entity in AxialUtil.GetRing(cell, 0).SelectMany(GetHiddenEntitiesAtCell)
			where entity.Layer == entityLayer
			select entity).ToList();
	}

	public ClusterGridEntity GetEntityOfLayerAtCell(AxialI cell, EntityLayer entityLayer)
	{
		return AxialUtil.GetRing(cell, 0).SelectMany(GetEntitiesOnCell).FirstOrDefault((ClusterGridEntity entity) => entity.Layer == entityLayer);
	}

	public List<ClusterGridEntity> GetHiddenEntitiesAtCell(AxialI cell)
	{
		if (cellContents.ContainsKey(cell) && !GetFOWManager().IsLocationRevealed(cell))
		{
			return cellContents[cell].Where((ClusterGridEntity entity) => entity.IsVisible).ToList();
		}
		return new List<ClusterGridEntity>();
	}

	public List<ClusterGridEntity> GetNotVisibleEntitiesAtAdjacentCell(AxialI cell)
	{
		return AxialUtil.GetRing(cell, 1).SelectMany(GetHiddenEntitiesAtCell).ToList();
	}

	public List<ClusterGridEntity> GetNotVisibleEntitiesOfLayerAtAdjacentCell(AxialI cell, EntityLayer entityLayer)
	{
		return (from entity in AxialUtil.GetRing(cell, 1).SelectMany(GetHiddenEntitiesAtCell)
			where entity.Layer == entityLayer
			select entity).ToList();
	}

	public ClusterGridEntity GetAsteroidAtCell(AxialI cell)
	{
		if (!cellContents.ContainsKey(cell))
		{
			return null;
		}
		return cellContents[cell].Where((ClusterGridEntity e) => e.Layer == EntityLayer.Asteroid).FirstOrDefault();
	}

	public bool HasVisibleAsteroidAtCell(AxialI cell)
	{
		return GetVisibleEntityOfLayerAtCell(cell, EntityLayer.Asteroid) != null;
	}

	public void RegisterEntity(ClusterGridEntity entity)
	{
		cellContents[entity.Location].Add(entity);
		entity.Subscribe(-1298331547, m_onClusterLocationChangedDelegate);
	}

	public void UnregisterEntity(ClusterGridEntity entity)
	{
		cellContents[entity.Location].Remove(entity);
		entity.Unsubscribe(-1298331547, m_onClusterLocationChangedDelegate);
	}

	public void OnClusterLocationChanged(object data)
	{
		ClusterLocationChangedEvent clusterLocationChangedEvent = (ClusterLocationChangedEvent)data;
		Debug.Assert(IsValidCell(clusterLocationChangedEvent.oldLocation), $"ChangeEntityCell move order FROM invalid location: {clusterLocationChangedEvent.oldLocation} {clusterLocationChangedEvent.entity}");
		Debug.Assert(IsValidCell(clusterLocationChangedEvent.newLocation), $"ChangeEntityCell move order TO invalid location: {clusterLocationChangedEvent.newLocation} {clusterLocationChangedEvent.entity}");
		cellContents[clusterLocationChangedEvent.oldLocation].Remove(clusterLocationChangedEvent.entity);
		cellContents[clusterLocationChangedEvent.newLocation].Add(clusterLocationChangedEvent.entity);
	}

	private AxialI GetNeighbor(AxialI cell, AxialI direction)
	{
		return cell + direction;
	}

	public int GetCellRing(AxialI cell)
	{
		Vector3I vector3I = cell.ToCube();
		Vector3I vector3I2 = new Vector3I(vector3I.x, vector3I.y, vector3I.z);
		Vector3I vector3I3 = new Vector3I(0, 0, 0);
		return (int)(float)((Mathf.Abs(vector3I2.x - vector3I3.x) + Mathf.Abs(vector3I2.y - vector3I3.y) + Mathf.Abs(vector3I2.z - vector3I3.z)) / 2);
	}

	private void CleanUpGrid()
	{
		cellContents.Clear();
	}

	private int GetHexDistance(AxialI a, AxialI b)
	{
		Vector3I vector3I = a.ToCube();
		Vector3I vector3I2 = b.ToCube();
		return Mathf.Max(Mathf.Abs(vector3I.x - vector3I2.x), Mathf.Abs(vector3I.y - vector3I2.y), Mathf.Abs(vector3I.z - vector3I2.z));
	}

	public List<ClusterGridEntity> GetEntitiesInRange(AxialI center, int range = 1)
	{
		List<ClusterGridEntity> list = new List<ClusterGridEntity>();
		foreach (KeyValuePair<AxialI, List<ClusterGridEntity>> cellContent in cellContents)
		{
			if (GetHexDistance(cellContent.Key, center) <= range)
			{
				list.AddRange(cellContent.Value);
			}
		}
		return list;
	}

	public List<ClusterGridEntity> GetEntitiesOnCell(AxialI cell)
	{
		return cellContents[cell];
	}

	public bool IsInRange(AxialI a, AxialI b, int range = 1)
	{
		return GetHexDistance(a, b) <= range;
	}

	private void GenerateGrid(int rings)
	{
		CleanUpGrid();
		numRings = rings;
		for (int i = -rings + 1; i < rings; i++)
		{
			for (int j = -rings + 1; j < rings; j++)
			{
				for (int k = -rings + 1; k < rings; k++)
				{
					if (i + j + k == 0)
					{
						AxialI key = new AxialI(i, j);
						cellContents.Add(key, new List<ClusterGridEntity>());
					}
				}
			}
		}
	}

	public Vector3 GetPosition(ClusterGridEntity entity)
	{
		float r = entity.Location.R;
		float q = entity.Location.Q;
		List<ClusterGridEntity> list = cellContents[entity.Location];
		if (list.Count > 1 && entity.SpaceOutInSameHex())
		{
			int num = 0;
			int num2 = 0;
			foreach (ClusterGridEntity item in list)
			{
				if (entity == item)
				{
					num = num2;
				}
				if (item.SpaceOutInSameHex())
				{
					num2++;
				}
			}
			if (list.Count > num2)
			{
				num2 += 5;
				num += 5;
			}
			else if (num2 > 0)
			{
				num2++;
				num++;
			}
			if (num2 == 0 || num2 == 1)
			{
				return AxialUtil.AxialToWorld(r, q);
			}
			float num3 = Mathf.Min(Mathf.Pow(num2, 0.5f), 1f) * 0.5f;
			float num4 = Mathf.Pow((float)num / (float)num2, 0.5f);
			float num5 = 0.81f;
			float num6 = Mathf.Pow(num2, 0.5f) * num5;
			float f = (float)Math.PI * 2f * num6 * num4;
			float x = Mathf.Cos(f) * num3 * num4;
			float y = Mathf.Sin(f) * num3 * num4;
			return AxialUtil.AxialToWorld(r, q) + new Vector3(x, y, 0f);
		}
		return AxialUtil.AxialToWorld(r, q);
	}

	public List<AxialI> GetPath(AxialI start, AxialI end, ClusterDestinationSelector destination_selector)
	{
		string fail_reason;
		return GetPath(start, end, destination_selector, out fail_reason);
	}

	public List<AxialI> GetPath(AxialI start, AxialI end, ClusterDestinationSelector destination_selector, out string fail_reason)
	{
		fail_reason = null;
		if (!destination_selector.canNavigateFogOfWar && !IsCellVisible(end))
		{
			fail_reason = UI.CLUSTERMAP.TOOLTIP_INVALID_DESTINATION_FOG_OF_WAR;
			return null;
		}
		ClusterGridEntity visibleEntityOfLayerAtCell = GetVisibleEntityOfLayerAtCell(end, EntityLayer.Asteroid);
		if (visibleEntityOfLayerAtCell != null && destination_selector.requireLaunchPadOnAsteroidDestination)
		{
			bool flag = false;
			foreach (LaunchPad launchPad in Components.LaunchPads)
			{
				if (launchPad.GetMyWorldLocation() == visibleEntityOfLayerAtCell.Location)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				fail_reason = UI.CLUSTERMAP.TOOLTIP_INVALID_DESTINATION_NO_LAUNCH_PAD;
				return null;
			}
		}
		if (visibleEntityOfLayerAtCell == null && destination_selector.requireAsteroidDestination)
		{
			fail_reason = UI.CLUSTERMAP.TOOLTIP_INVALID_DESTINATION_REQUIRE_ASTEROID;
			return null;
		}
		HashSet<AxialI> frontier = new HashSet<AxialI>();
		HashSet<AxialI> visited = new HashSet<AxialI>();
		HashSet<AxialI> buffer = new HashSet<AxialI>();
		Dictionary<AxialI, AxialI> cameFrom = new Dictionary<AxialI, AxialI>();
		frontier.Add(start);
		while (!frontier.Contains(end) && frontier.Count > 0)
		{
			ExpandFrontier();
		}
		if (frontier.Contains(end))
		{
			List<AxialI> list = new List<AxialI>();
			AxialI axialI = end;
			while (axialI != start)
			{
				list.Add(axialI);
				axialI = cameFrom[axialI];
			}
			list.Reverse();
			return list;
		}
		fail_reason = UI.CLUSTERMAP.TOOLTIP_INVALID_DESTINATION_NO_PATH;
		return null;
		void ExpandFrontier()
		{
			buffer.Clear();
			foreach (AxialI item in frontier)
			{
				foreach (AxialI dIRECTION in AxialI.DIRECTIONS)
				{
					AxialI neighbor = GetNeighbor(item, dIRECTION);
					if (!visited.Contains(neighbor) && IsValidCell(neighbor) && (IsCellVisible(neighbor) || destination_selector.canNavigateFogOfWar) && (!HasVisibleAsteroidAtCell(neighbor) || !(neighbor != start) || !(neighbor != end)))
					{
						buffer.Add(neighbor);
						if (!cameFrom.ContainsKey(neighbor))
						{
							cameFrom.Add(neighbor, item);
						}
					}
				}
				visited.Add(item);
			}
			HashSet<AxialI> hashSet = frontier;
			frontier = buffer;
			buffer = hashSet;
		}
	}

	public void GetLocationDescription(AxialI location, out Sprite sprite, out string label, out string sublabel)
	{
		ClusterGridEntity clusterGridEntity = GetVisibleEntitiesAtCell(location).Find((ClusterGridEntity x) => x.Layer == EntityLayer.Asteroid);
		ClusterGridEntity visibleEntityOfLayerAtAdjacentCell = GetVisibleEntityOfLayerAtAdjacentCell(location, EntityLayer.Asteroid);
		if (clusterGridEntity != null)
		{
			sprite = clusterGridEntity.GetUISprite();
			label = clusterGridEntity.Name;
			WorldContainer component = clusterGridEntity.GetComponent<WorldContainer>();
			sublabel = Strings.Get(component.worldType);
		}
		else if (visibleEntityOfLayerAtAdjacentCell != null)
		{
			sprite = visibleEntityOfLayerAtAdjacentCell.GetUISprite();
			label = UI.SPACEDESTINATIONS.ORBIT.NAME_FMT.Replace("{Name}", visibleEntityOfLayerAtAdjacentCell.Name);
			WorldContainer component2 = visibleEntityOfLayerAtAdjacentCell.GetComponent<WorldContainer>();
			sublabel = Strings.Get(component2.worldType);
		}
		else if (IsCellVisible(location))
		{
			sprite = Assets.GetSprite("hex_unknown");
			label = UI.SPACEDESTINATIONS.EMPTY_SPACE.NAME;
			sublabel = "";
		}
		else
		{
			sprite = Assets.GetSprite("unknown_far");
			label = UI.SPACEDESTINATIONS.FOG_OF_WAR_SPACE.NAME;
			sublabel = "";
		}
	}
}
