using System.Collections.Generic;

public class SuitWearer : GameStateMachine<SuitWearer, SuitWearer.Instance>
{
	public new class Instance : GameInstance
	{
		private List<int> suitReservations = new List<int>();

		private List<int> emptyLockerReservations = new List<int>();

		private Navigator navigator;

		private int prefabInstanceID;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			navigator = master.GetComponent<Navigator>();
			navigator.SetFlags(PathFinder.PotentialPath.Flags.PerformSuitChecks);
			prefabInstanceID = navigator.GetComponent<KPrefabID>().InstanceID;
			master.GetComponent<KBatchedAnimController>().SetSymbolVisiblity("snapto_neck", is_visible: false);
		}

		public void OnPathAdvanced(object data)
		{
			if (navigator.CurrentNavType == NavType.Hover && (navigator.flags & PathFinder.PotentialPath.Flags.HasJetPack) == 0)
			{
				navigator.SetCurrentNavType(NavType.Floor);
			}
			UnreserveSuits();
			ReserveSuits();
		}

		public void ReserveSuits()
		{
			PathFinder.Path path = navigator.path;
			if (path.nodes == null)
			{
				return;
			}
			bool flag = (navigator.flags & PathFinder.PotentialPath.Flags.HasAtmoSuit) != 0;
			bool flag2 = (navigator.flags & PathFinder.PotentialPath.Flags.HasJetPack) != 0;
			bool flag3 = (navigator.flags & PathFinder.PotentialPath.Flags.HasOxygenMask) != 0;
			for (int i = 0; i < path.nodes.Count - 1; i++)
			{
				int cell = path.nodes[i].cell;
				Grid.SuitMarker.Flags flags = (Grid.SuitMarker.Flags)0;
				PathFinder.PotentialPath.Flags pathFlags = PathFinder.PotentialPath.Flags.None;
				if (!Grid.TryGetSuitMarkerFlags(cell, out flags, out pathFlags))
				{
					continue;
				}
				bool flag4 = (pathFlags & PathFinder.PotentialPath.Flags.HasAtmoSuit) != 0;
				bool flag5 = (pathFlags & PathFinder.PotentialPath.Flags.HasJetPack) != 0;
				bool flag6 = (pathFlags & PathFinder.PotentialPath.Flags.HasOxygenMask) != 0;
				bool flag7 = flag2 || flag || flag3;
				bool flag8 = flag4 == flag && flag5 == flag2 && flag6 == flag3;
				bool flag9 = SuitMarker.DoesTraversalDirectionRequireSuit(cell, path.nodes[i + 1].cell, flags);
				if (flag9 && !flag7)
				{
					Grid.ReserveSuit(cell, prefabInstanceID, reserve: true);
					suitReservations.Add(cell);
					if (flag4)
					{
						flag = true;
					}
					if (flag5)
					{
						flag2 = true;
					}
					if (flag6)
					{
						flag3 = true;
					}
				}
				else if (!flag9 && flag8 && Grid.HasEmptyLocker(cell, prefabInstanceID))
				{
					Grid.ReserveEmptyLocker(cell, prefabInstanceID, reserve: true);
					emptyLockerReservations.Add(cell);
					if (flag4)
					{
						flag = false;
					}
					if (flag5)
					{
						flag2 = false;
					}
					if (flag6)
					{
						flag3 = false;
					}
				}
			}
		}

		public void UnreserveSuits()
		{
			foreach (int suitReservation in suitReservations)
			{
				if (Grid.HasSuitMarker[suitReservation])
				{
					Grid.ReserveSuit(suitReservation, prefabInstanceID, reserve: false);
				}
			}
			suitReservations.Clear();
			foreach (int emptyLockerReservation in emptyLockerReservations)
			{
				if (Grid.HasSuitMarker[emptyLockerReservation])
				{
					Grid.ReserveEmptyLocker(emptyLockerReservation, prefabInstanceID, reserve: false);
				}
			}
			emptyLockerReservations.Clear();
		}
	}

	public State suit;

	public State nosuit;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.EventHandler(GameHashes.PathAdvanced, delegate(Instance smi, object data)
		{
			smi.OnPathAdvanced(data);
		}).DoNothing();
		suit.DoNothing();
		nosuit.DoNothing();
	}
}
