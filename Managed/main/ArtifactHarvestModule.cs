using UnityEngine;

public class ArtifactHarvestModule : GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>
{
	public class Def : BaseDef
	{
	}

	public class NotGroundedStates : State
	{
		public State not_harvesting;

		public State harvesting;
	}

	public class StatesInstance : GameInstance
	{
		[MyCmpReq]
		private Storage storage;

		[MyCmpReq]
		private SingleEntityReceptacle receptacle;

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void HarvestFromPOI(float dt)
		{
			Clustercraft component = GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			ClusterGridEntity pOIAtCurrentLocation = component.GetPOIAtCurrentLocation();
			if (pOIAtCurrentLocation.IsNullOrDestroyed())
			{
				return;
			}
			ArtifactPOIStates.Instance sMI = pOIAtCurrentLocation.GetSMI<ArtifactPOIStates.Instance>();
			if (((bool)pOIAtCurrentLocation.GetComponent<ArtifactPOIClusterGridEntity>() || (bool)pOIAtCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>()) && !sMI.IsNullOrDestroyed())
			{
				bool flag = false;
				string artifactToHarvest = sMI.GetArtifactToHarvest();
				GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(artifactToHarvest), base.transform.position);
				gameObject.SetActive(value: true);
				receptacle.ForceDeposit(gameObject);
				storage.Store(gameObject);
				sMI.HarvestArtifact();
				if (sMI.configuration.DestroyOnHarvest())
				{
					flag = true;
				}
				if (flag)
				{
					pOIAtCurrentLocation.gameObject.DeleteObject();
				}
			}
		}

		public bool CheckIfCanHarvest()
		{
			Clustercraft component = GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			if (component == null)
			{
				return false;
			}
			ClusterGridEntity pOIAtCurrentLocation = component.GetPOIAtCurrentLocation();
			if (pOIAtCurrentLocation != null && ((bool)pOIAtCurrentLocation.GetComponent<ArtifactPOIClusterGridEntity>() || (bool)pOIAtCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>()))
			{
				ArtifactPOIStates.Instance sMI = pOIAtCurrentLocation.GetSMI<ArtifactPOIStates.Instance>();
				if (sMI != null && sMI.CanHarvestArtifact() && receptacle.Occupant == null)
				{
					base.sm.canHarvest.Set(value: true, this);
					return true;
				}
			}
			base.sm.canHarvest.Set(value: false, this);
			return false;
		}
	}

	public BoolParameter canHarvest;

	public State grounded;

	public NotGroundedStates not_grounded;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = grounded;
		root.Enter(delegate(StatesInstance smi)
		{
			smi.CheckIfCanHarvest();
		});
		grounded.TagTransition(GameTags.RocketNotOnGround, not_grounded);
		not_grounded.DefaultState(not_grounded.not_harvesting).EventHandler(GameHashes.ClusterLocationChanged, (StatesInstance smi) => Game.Instance, delegate(StatesInstance smi)
		{
			smi.CheckIfCanHarvest();
		}).EventHandler(GameHashes.OnStorageChange, delegate(StatesInstance smi)
		{
			smi.CheckIfCanHarvest();
		})
			.TagTransition(GameTags.RocketNotOnGround, grounded, on_remove: true);
		not_grounded.not_harvesting.PlayAnim("loaded").ParamTransition(canHarvest, not_grounded.harvesting, GameStateMachine<ArtifactHarvestModule, StatesInstance, IStateMachineTarget, Def>.IsTrue);
		not_grounded.harvesting.PlayAnim("deploying").Update(delegate(StatesInstance smi, float dt)
		{
			smi.HarvestFromPOI(dt);
		}, UpdateRate.SIM_4000ms).ParamTransition(canHarvest, not_grounded.not_harvesting, GameStateMachine<ArtifactHarvestModule, StatesInstance, IStateMachineTarget, Def>.IsFalse);
	}
}
