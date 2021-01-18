using System;
using KSerialization;
using ProcGen;
using UnityEngine;

public class DiggerMonitor : GameStateMachine<DiggerMonitor, DiggerMonitor.Instance, IStateMachineTarget, DiggerMonitor.Def>
{
	public class Def : BaseDef
	{
		public int depthToDig
		{
			get;
			set;
		}
	}

	public new class Instance : GameInstance
	{
		[Serialize]
		public int lastDigCell = -1;

		private Action<object> OnDestinationReachedDelegate;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(OnSolidChanged));
			OnDestinationReachedDelegate = OnDestinationReached;
			master.Subscribe(387220196, OnDestinationReachedDelegate);
			master.Subscribe(-766531887, OnDestinationReachedDelegate);
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(OnSolidChanged));
			base.master.Unsubscribe(387220196, OnDestinationReachedDelegate);
			base.master.Unsubscribe(-766531887, OnDestinationReachedDelegate);
		}

		private void OnDestinationReached(object data)
		{
			CheckInSolid();
		}

		private void CheckInSolid()
		{
			Navigator component = base.gameObject.GetComponent<Navigator>();
			if (!(component == null))
			{
				int cell = Grid.PosToCell(base.gameObject);
				if (component.CurrentNavType != NavType.Solid && Grid.IsSolidCell(cell))
				{
					component.SetCurrentNavType(NavType.Solid);
				}
				else if (component.CurrentNavType == NavType.Solid && !Grid.IsSolidCell(cell))
				{
					component.SetCurrentNavType(NavType.Floor);
					base.gameObject.AddTag(GameTags.Creatures.Falling);
				}
			}
		}

		private void OnSolidChanged(int cell)
		{
			CheckInSolid();
		}

		public bool CanTunnel()
		{
			int num = Grid.PosToCell(this);
			if (World.Instance.zoneRenderData.GetSubWorldZoneType(num) == SubWorld.ZoneType.Space)
			{
				int num2 = num;
				while (Grid.IsValidCell(num2) && !Grid.Solid[num2])
				{
					num2 = Grid.CellAbove(num2);
				}
				if (!Grid.IsValidCell(num2))
				{
					return FoundValidDigCell();
				}
			}
			return false;
		}

		private bool FoundValidDigCell()
		{
			int num = base.smi.def.depthToDig;
			int num2 = (lastDigCell = Grid.PosToCell(base.smi.master.gameObject));
			int cell = Grid.CellBelow(num2);
			while (IsValidDigCell(cell) && num > 0)
			{
				cell = Grid.CellBelow(cell);
				num--;
			}
			if (num > 0)
			{
				cell = GameUtil.FloodFillFind<object>(IsValidDigCell, null, num2, base.smi.def.depthToDig, stop_at_solid: false, stop_at_liquid: true);
			}
			lastDigCell = cell;
			return lastDigCell != -1;
		}

		private bool IsValidDigCell(int cell, object arg = null)
		{
			if (Grid.IsValidCell(cell) && Grid.Solid[cell])
			{
				if (!Grid.HasDoor[cell] && !Grid.Foundation[cell])
				{
					byte index = Grid.ElementIdx[cell];
					Element element = ElementLoader.elements[index];
					if (Grid.Element[cell].hardness < 150)
					{
						return !element.HasTag(GameTags.RefinedMetal);
					}
					return false;
				}
				GameObject gameObject = Grid.Objects[cell, 1];
				if (gameObject != null)
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					if (Grid.Element[cell].hardness < 150)
					{
						return !component.Element.HasTag(GameTags.RefinedMetal);
					}
					return false;
				}
			}
			return false;
		}
	}

	public State loop;

	public State dig;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = loop;
		loop.EventTransition(GameHashes.BeginMeteorBombardment, (Instance smi) => Game.Instance, dig, (Instance smi) => smi.CanTunnel());
		dig.ToggleBehaviour(GameTags.Creatures.Tunnel, (Instance smi) => true).GoTo(loop);
	}
}
