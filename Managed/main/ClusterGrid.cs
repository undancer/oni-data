using System;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;

public class ClusterGrid
{
	public static ClusterGrid Instance;

	public const float NodeDistanceScale = 600f;

	private const float MIN_OFFSET_WITHIN_HEX = 0.1f;

	private const float MAX_OFFSET_WITHIN_HEX = 0.25f;

	private ClusterFogOfWarManager.Instance m_fowManager;

	private Action<object> m_onClusterLocationChangedDelegate;

	public Dictionary<AxialI, List<ClusterGridEntity>> cellContents = new Dictionary<AxialI, List<ClusterGridEntity>>();

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
		return entity.IsVisible && IsCellVisible(entity.Location);
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
		List<ClusterGridEntity> visibleEntitiesAtCell = GetVisibleEntitiesAtCell(cell);
		return visibleEntitiesAtCell.Find((ClusterGridEntity x) => x.Layer == entityLayer);
	}

	public ClusterGridEntity GetVisibleEntityOfLayerAtAdjacentCell(AxialI cell, EntityLayer entityLayer)
	{
		return (from entity in AxialUtil.GetRing(cell, 1).SelectMany((AxialI c) => GetVisibleEntitiesAtCell(c))
			where entity.Layer == entityLayer
			select entity).FirstOrDefault();
	}

	public List<ClusterGridEntity> GetHiddenEntitiesAtCell(AxialI cell)
	{
		if (cellContents.ContainsKey(cell) && !GetFOWManager().IsLocationRevealed(cell))
		{
			return cellContents[cell].Where((ClusterGridEntity entity) => entity.IsVisible).ToList();
		}
		return new List<ClusterGridEntity>();
	}

	public List<ClusterGridEntity> GetNotVisibleEntitiesOfLayerAtAdjacentCell(AxialI cell, EntityLayer entityLayer)
	{
		return (from entity in AxialUtil.GetRing(cell, 1).SelectMany((AxialI c) => GetHiddenEntitiesAtCell(c))
			where entity.Layer == entityLayer
			select entity).ToList();
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
		float num = (Mathf.Abs(vector3I2.x - vector3I3.x) + Mathf.Abs(vector3I2.y - vector3I3.y) + Mathf.Abs(vector3I2.z - vector3I3.z)) / 2;
		return (int)num;
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
		float num = entity.Location.R;
		float num2 = entity.Location.Q;
		List<ClusterGridEntity> list = cellContents[entity.Location];
		if (list.Count > 1 && entity.SpaceOutInSameHex())
		{
			int num3 = 0;
			int num4 = 0;
			foreach (ClusterGridEntity item in list)
			{
				if (entity == item)
				{
					num3 = num4;
				}
				if (item.SpaceOutInSameHex())
				{
					num4++;
				}
			}
			AxialI axialI = AxialI.DIRECTIONS[num3 * num4 switch
			{
				2 => 3, 
				3 => 2, 
				_ => 1, 
			} % AxialI.DIRECTIONS.Count];
			float num5 = 0.1f;
			if (num4 >= AxialI.DIRECTIONS.Count)
			{
				float num6 = 0.15f;
				float num7 = num6 / (float)(num4 / AxialI.DIRECTIONS.Count);
				num5 += num7 * (float)num3 / (float)AxialI.DIRECTIONS.Count;
			}
			num += (float)axialI.R * num5;
			num2 += (float)axialI.Q * num5;
		}
		return AxialUtil.AxialToWorld(num, num2);
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
		List<ClusterGridEntity> visibleEntitiesAtCell = GetVisibleEntitiesAtCell(location);
		ClusterGridEntity clusterGridEntity = visibleEntitiesAtCell.Find((ClusterGridEntity x) => x.Layer == EntityLayer.Asteroid);
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
