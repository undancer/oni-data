using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/KAnimGraphTileVisualizer")]
public class KAnimGraphTileVisualizer : KMonoBehaviour, ISaveLoadable, IUtilityItem
{
	public enum ConnectionSource
	{
		Gas,
		Liquid,
		Electrical,
		Logic,
		Tube,
		Solid
	}

	[Serialize]
	private UtilityConnections _connections = (UtilityConnections)0;

	public bool isPhysicalBuilding;

	public bool skipCleanup = false;

	public bool skipRefresh = false;

	public ConnectionSource connectionSource;

	[NonSerialized]
	public IUtilityNetworkMgr connectionManager = null;

	public UtilityConnections Connections
	{
		get
		{
			return _connections;
		}
		set
		{
			_connections = value;
			Trigger(-1041684577, _connections);
		}
	}

	public IUtilityNetworkMgr ConnectionManager => connectionSource switch
	{
		ConnectionSource.Gas => Game.Instance.gasConduitSystem, 
		ConnectionSource.Liquid => Game.Instance.liquidConduitSystem, 
		ConnectionSource.Solid => Game.Instance.solidConduitSystem, 
		ConnectionSource.Electrical => Game.Instance.electricalConduitSystem, 
		ConnectionSource.Logic => Game.Instance.logicCircuitSystem, 
		ConnectionSource.Tube => Game.Instance.travelTubeSystem, 
		_ => null, 
	};

	protected override void OnSpawn()
	{
		base.OnSpawn();
		connectionManager = ConnectionManager;
		int cell = Grid.PosToCell(base.transform.GetPosition());
		connectionManager.SetConnections(Connections, cell, isPhysicalBuilding);
		Building component = GetComponent<Building>();
		TileVisualizer.RefreshCell(cell, component.Def.TileLayer, component.Def.ReplacementLayer);
	}

	protected override void OnCleanUp()
	{
		if (connectionManager != null && !skipCleanup)
		{
			skipRefresh = true;
			int cell = Grid.PosToCell(base.transform.GetPosition());
			connectionManager.ClearCell(cell, isPhysicalBuilding);
			Building component = GetComponent<Building>();
			TileVisualizer.RefreshCell(cell, component.Def.TileLayer, component.Def.ReplacementLayer);
		}
	}

	[ContextMenu("Refresh")]
	public void Refresh()
	{
		if (connectionManager == null || skipRefresh)
		{
			return;
		}
		int cell = Grid.PosToCell(base.transform.GetPosition());
		Connections = connectionManager.GetConnections(cell, isPhysicalBuilding);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			string text = connectionManager.GetVisualizerString(cell);
			BuildingUnderConstruction component2 = GetComponent<BuildingUnderConstruction>();
			if (component2 != null && component.HasAnimation(text + "_place"))
			{
				text += "_place";
			}
			if (text != null && text != "")
			{
				component.Play(text);
			}
		}
	}

	public int GetNetworkID()
	{
		return GetNetwork()?.id ?? (-1);
	}

	private UtilityNetwork GetNetwork()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		return connectionManager.GetNetworkForDirection(cell, Direction.None);
	}

	public UtilityNetwork GetNetworkForDirection(Direction d)
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		return connectionManager.GetNetworkForDirection(cell, d);
	}

	public void UpdateConnections(UtilityConnections new_connections)
	{
		_connections = new_connections;
		if (connectionManager != null)
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			connectionManager.SetConnections(new_connections, cell, isPhysicalBuilding);
		}
	}

	public KAnimGraphTileVisualizer GetNeighbour(Direction d)
	{
		KAnimGraphTileVisualizer result = null;
		Grid.PosToXY(base.transform.GetPosition(), out var xy);
		int num = -1;
		switch (d)
		{
		case Direction.Up:
			if (xy.y < Grid.HeightInCells - 1)
			{
				num = Grid.XYToCell(xy.x, xy.y + 1);
			}
			break;
		case Direction.Down:
			if (xy.y > 0)
			{
				num = Grid.XYToCell(xy.x, xy.y - 1);
			}
			break;
		case Direction.Left:
			if (xy.x > 0)
			{
				num = Grid.XYToCell(xy.x - 1, xy.y);
			}
			break;
		case Direction.Right:
			if (xy.x < Grid.WidthInCells - 1)
			{
				num = Grid.XYToCell(xy.x + 1, xy.y);
			}
			break;
		}
		if (num != -1)
		{
			GameObject gameObject = Grid.Objects[num, connectionSource switch
			{
				ConnectionSource.Gas => 13, 
				ConnectionSource.Liquid => 17, 
				ConnectionSource.Electrical => 27, 
				ConnectionSource.Logic => 32, 
				ConnectionSource.Tube => 35, 
				ConnectionSource.Solid => 21, 
				_ => throw new ArgumentNullException("wtf"), 
			}];
			if (gameObject != null)
			{
				result = gameObject.GetComponent<KAnimGraphTileVisualizer>();
			}
		}
		return result;
	}
}
