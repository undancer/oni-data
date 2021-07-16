using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class Teleporter : KMonoBehaviour
{
	[MyCmpReq]
	private Operational operational;

	[Serialize]
	public Ref<Teleporter> teleportTarget = new Ref<Teleporter>();

	public int ID_LENGTH = 4;

	private static readonly EventSystem.IntraObjectHandler<Teleporter> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<Teleporter>(delegate(Teleporter component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	[Serialize]
	public int teleporterID
	{
		get;
		private set;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.Teleporters.Add(this);
		SetTeleporterID(0);
		Subscribe(-801688580, OnLogicValueChangedDelegate);
	}

	private void OnLogicValueChanged(object data)
	{
		LogicPorts component = GetComponent<LogicPorts>();
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		List<int> list = new List<int>();
		int num = 0;
		int num2 = Mathf.Min(ID_LENGTH, component.inputPorts.Count);
		for (int i = 0; i < num2; i++)
		{
			int logicUICell = component.inputPorts[i].GetLogicUICell();
			int item = logicCircuitManager.GetNetworkForCell(logicUICell)?.OutputValue ?? 1;
			list.Add(item);
		}
		foreach (int item2 in list)
		{
			num = (num << 1) | item2;
		}
		SetTeleporterID(num);
	}

	protected override void OnCleanUp()
	{
		Components.Teleporters.Remove(this);
		base.OnCleanUp();
	}

	public bool HasTeleporterTarget()
	{
		return FindTeleportTarget() != null;
	}

	public bool IsValidTeleportTarget(Teleporter from_tele)
	{
		if (from_tele.teleporterID == teleporterID)
		{
			return operational.IsOperational;
		}
		return false;
	}

	public Teleporter FindTeleportTarget()
	{
		List<Teleporter> list = new List<Teleporter>();
		foreach (Teleporter teleporter in Components.Teleporters)
		{
			if (teleporter.IsValidTeleportTarget(this) && teleporter != this)
			{
				list.Add(teleporter);
			}
		}
		Teleporter result = null;
		if (list.Count > 0)
		{
			result = list.GetRandom();
		}
		return result;
	}

	public void SetTeleporterID(int ID)
	{
		teleporterID = ID;
		foreach (Teleporter teleporter in Components.Teleporters)
		{
			teleporter.Trigger(-1266722732);
		}
	}

	public void SetTeleportTarget(Teleporter target)
	{
		teleportTarget.Set(target);
	}

	public void TeleportObjects()
	{
		Teleporter teleporter = teleportTarget.Get();
		int widthInCells = GetComponent<Building>().Def.WidthInCells;
		int num = GetComponent<Building>().Def.HeightInCells - 1;
		Vector3 position = base.transform.GetPosition();
		if (teleporter != null)
		{
			ListPool<ScenePartitionerEntry, Teleporter>.PooledList pooledList = ListPool<ScenePartitionerEntry, Teleporter>.Allocate();
			GameScenePartitioner.Instance.GatherEntries((int)position.x - widthInCells / 2 + 1, (int)position.y - num / 2 + 1, widthInCells, num, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
			int cell = Grid.PosToCell(teleporter);
			foreach (ScenePartitionerEntry item in pooledList)
			{
				GameObject gameObject = (item.obj as Pickupable).gameObject;
				Vector3 b = gameObject.transform.GetPosition() - position;
				MinionIdentity component = gameObject.GetComponent<MinionIdentity>();
				if (component != null)
				{
					new EmoteChore(component.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", Telepad.PortalBirthAnim, null);
				}
				else
				{
					b += Vector3.up;
				}
				gameObject.transform.SetLocalPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move) + b);
			}
			pooledList.Recycle();
		}
		TeleportalPad.StatesInstance sMI = teleportTarget.Get().GetSMI<TeleportalPad.StatesInstance>();
		sMI.sm.doTeleport.Trigger(sMI);
		teleportTarget.Set(null);
	}
}
