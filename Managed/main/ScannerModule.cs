public class ScannerModule : GameStateMachine<ScannerModule, ScannerModule.Instance, IStateMachineTarget, ScannerModule.Def>
{
	public class Def : BaseDef
	{
		public int scanRadius = 1;
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void Scan()
		{
			Clustercraft component = GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			if (component.Status != Clustercraft.CraftStatus.InFlight)
			{
				return;
			}
			ClusterFogOfWarManager.Instance sMI = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
			AxialI location = component.Location;
			sMI.RevealLocation(location, base.def.scanRadius);
			foreach (ClusterGridEntity item in ClusterGrid.Instance.GetNotVisibleEntitiesAtAdjacentCell(location))
			{
				sMI.RevealLocation(item.Location);
			}
		}

		public void SetFogOfWarAllowed()
		{
			CraftModuleInterface craftInterface = GetComponent<RocketModuleCluster>().CraftInterface;
			if (!craftInterface.HasClusterDestinationSelector())
			{
				return;
			}
			bool flag = false;
			ClusterDestinationSelector clusterDestinationSelector = craftInterface.GetClusterDestinationSelector();
			bool canNavigateFogOfWar = clusterDestinationSelector.canNavigateFogOfWar;
			foreach (Ref<RocketModuleCluster> clusterModule in craftInterface.ClusterModules)
			{
				if (clusterModule.Get()?.GetSMI<Instance>() != null)
				{
					flag = true;
					break;
				}
			}
			clusterDestinationSelector.canNavigateFogOfWar = flag;
			if (canNavigateFogOfWar && !flag)
			{
				craftInterface.GetComponent<ClusterTraveler>()?.RevalidatePath();
			}
			craftInterface.GetComponent<Clustercraft>().Trigger(-688990705);
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.Enter(delegate(Instance smi)
		{
			smi.SetFogOfWarAllowed();
		}).EventHandler(GameHashes.RocketLaunched, delegate(Instance smi)
		{
			smi.Scan();
		}).EventHandler(GameHashes.ClusterLocationChanged, (Instance smi) => smi.GetComponent<RocketModuleCluster>().CraftInterface, delegate(Instance smi)
		{
			smi.Scan();
		})
			.EventHandler(GameHashes.RocketModuleChanged, (Instance smi) => smi.GetComponent<RocketModuleCluster>().CraftInterface, delegate(Instance smi)
			{
				smi.SetFogOfWarAllowed();
			})
			.Exit(delegate(Instance smi)
			{
				smi.SetFogOfWarAllowed();
			});
	}
}
