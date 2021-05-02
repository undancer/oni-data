using System.Collections.Generic;
using KSerialization;
using TUNING;
using UnityEngine;

public class ClusterFogOfWarManager : GameStateMachine<ClusterFogOfWarManager, ClusterFogOfWarManager.Instance, IStateMachineTarget, ClusterFogOfWarManager.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		[Serialize]
		private Dictionary<AxialI, float> m_revealPointsByCell = new Dictionary<AxialI, float>();

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void Initialize()
		{
			UpdateRevealedCellsFromDiscoveredWorlds();
			EnsureRevealedTilesHavePeek();
		}

		public ClusterRevealLevel GetCellRevealLevel(AxialI location)
		{
			if (GetRevealCompleteFraction(location) >= 1f)
			{
				return ClusterRevealLevel.Visible;
			}
			if (GetRevealCompleteFraction(location) > 0f)
			{
				return ClusterRevealLevel.Peeked;
			}
			return ClusterRevealLevel.Hidden;
		}

		public bool IsLocationRevealed(AxialI location)
		{
			return GetRevealCompleteFraction(location) >= 1f;
		}

		private void EnsureRevealedTilesHavePeek()
		{
			foreach (KeyValuePair<AxialI, List<ClusterGridEntity>> cellContent in ClusterGrid.Instance.cellContents)
			{
				if (IsLocationRevealed(cellContent.Key))
				{
					PeekLocation(cellContent.Key, 2);
				}
			}
		}

		public void PeekLocation(AxialI location, int radius)
		{
			foreach (AxialI item in AxialUtil.GetAllPointsWithinRadius(location, radius))
			{
				if (m_revealPointsByCell.ContainsKey(item))
				{
					m_revealPointsByCell[item] = Mathf.Max(m_revealPointsByCell[item], 0.01f);
				}
				else
				{
					m_revealPointsByCell[item] = 0.01f;
				}
			}
		}

		public void RevealLocation(AxialI location, int radius = 0)
		{
			bool flag = false;
			foreach (AxialI item in AxialUtil.GetAllPointsWithinRadius(location, radius))
			{
				flag |= RevealCellIfValid(item);
			}
			if (flag)
			{
				Game.Instance.Trigger(-1991583975, location);
			}
		}

		public void EarnRevealPointsForLocation(AxialI location, float points)
		{
			Debug.Assert(ClusterGrid.Instance.IsValidCell(location), $"EarnRevealPointsForLocation called with invalid location: {location}");
			if (!IsLocationRevealed(location))
			{
				if (m_revealPointsByCell.ContainsKey(location))
				{
					m_revealPointsByCell[location] += points;
				}
				else
				{
					m_revealPointsByCell[location] = points;
					Game.Instance.Trigger(-1554423969, location);
				}
				if (IsLocationRevealed(location))
				{
					PeekLocation(location, 2);
					Game.Instance.Trigger(-1991583975, location);
				}
			}
		}

		public float GetRevealCompleteFraction(AxialI location)
		{
			Debug.Assert(ClusterGrid.Instance.IsValidCell(location), $"GetRevealCompleteFraction called with invalid location: {location}");
			if (DebugHandler.RevealFogOfWar)
			{
				return 1f;
			}
			if (m_revealPointsByCell.TryGetValue(location, out var value))
			{
				return Mathf.Min(value / ROCKETRY.CLUSTER_FOW.POINTS_TO_REVEAL, 1f);
			}
			return 0f;
		}

		private bool RevealCellIfValid(AxialI cell)
		{
			if (ClusterGrid.Instance.IsValidCell(cell))
			{
				if (IsLocationRevealed(cell))
				{
					return false;
				}
				m_revealPointsByCell[cell] = ROCKETRY.CLUSTER_FOW.POINTS_TO_REVEAL;
				PeekLocation(cell, 2);
				return true;
			}
			return false;
		}

		public bool GetUnrevealedLocationWithinRadius(AxialI center, int radius, out AxialI result)
		{
			for (int i = 0; i <= radius; i++)
			{
				List<AxialI> ring = AxialUtil.GetRing(center, i);
				foreach (AxialI item in ring)
				{
					if (ClusterGrid.Instance.IsValidCell(item) && !IsLocationRevealed(item))
					{
						result = item;
						return true;
					}
				}
			}
			result = AxialI.ZERO;
			return false;
		}

		public void UpdateRevealedCellsFromDiscoveredWorlds()
		{
			foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
			{
				if (worldContainer.IsDiscovered && !DebugHandler.RevealFogOfWar)
				{
					RevealLocation(worldContainer.GetComponent<ClusterGridEntity>().Location);
				}
			}
		}
	}

	public const int AUTOMATIC_PEEK_RADIUS = 2;

	public override void InitializeStates(out BaseState defaultState)
	{
		defaultState = root;
		root.Enter(delegate(Instance smi)
		{
			smi.Initialize();
		}).EventHandler(GameHashes.DiscoveredWorldsChanged, (Instance smi) => Game.Instance, delegate(Instance smi)
		{
			smi.UpdateRevealedCellsFromDiscoveredWorlds();
		});
	}
}
