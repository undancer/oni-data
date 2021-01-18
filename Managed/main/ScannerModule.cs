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
			Clustercraft component = GetComponent<RocketModule>().CraftInterface.GetComponent<Clustercraft>();
			if (component.Status == Clustercraft.CraftStatus.InFlight)
			{
				ClusterFogOfWarManager.Instance sMI = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
				AxialI location = component.Location;
				sMI.RevealLocation(location, base.def.scanRadius);
			}
		}

		public void SetFogOfWarAllowed()
		{
			CraftModuleInterface craftInterface = GetComponent<RocketModule>().CraftInterface;
			if (craftInterface.HasClusterDestinationSelector())
			{
				ClusterDestinationSelector clusterDestinationSelector = craftInterface.GetClusterDestinationSelector();
				clusterDestinationSelector.canNavigateFogOfWar = true;
			}
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
		}).EventHandler(GameHashes.ClusterLocationChanged, (Instance smi) => smi.GetComponent<RocketModule>().CraftInterface, delegate(Instance smi)
		{
			smi.Scan();
		})
			.EventHandler(GameHashes.RocketModuleAdded, (Instance smi) => smi.GetComponent<RocketModule>().CraftInterface, delegate(Instance smi)
			{
				smi.SetFogOfWarAllowed();
			});
	}
}
