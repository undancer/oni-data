using System.Collections.Generic;

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
			List<ClusterGridEntity> notVisibleEntitiesOfLayerAtAdjacentCell = ClusterGrid.Instance.GetNotVisibleEntitiesOfLayerAtAdjacentCell(location, EntityLayer.Asteroid);
			foreach (ClusterGridEntity item in notVisibleEntitiesOfLayerAtAdjacentCell)
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
			bool canNavigateFogOfWar = false;
			ClusterDestinationSelector clusterDestinationSelector = craftInterface.GetClusterDestinationSelector();
			foreach (Ref<RocketModuleCluster> clusterModule in craftInterface.ClusterModules)
			{
				if (clusterModule.Get() != null && clusterModule.Get().GetDef<Def>() != null)
				{
					canNavigateFogOfWar = true;
					break;
				}
			}
			clusterDestinationSelector.canNavigateFogOfWar = canNavigateFogOfWar;
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
