using System.Collections.Generic;
using KSerialization;

public class ArtifactAnalysisStation : GameStateMachine<ArtifactAnalysisStation, ArtifactAnalysisStation.StatesInstance, IStateMachineTarget, ArtifactAnalysisStation.Def>
{
	public class Def : BaseDef
	{
	}

	public class StatesInstance : GameInstance
	{
		[MyCmpReq]
		public Storage storage;

		[MyCmpReq]
		public ManualDeliveryKG manualDelivery;

		[MyCmpReq]
		public ArtifactAnalysisStationWorkable workable;

		[Serialize]
		private HashSet<Tag> forbiddenSeeds;

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			workable.statesInstance = this;
		}

		public override void StartSM()
		{
			base.StartSM();
		}
	}

	public State inoperational;

	public State operational;

	public State ready;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = inoperational;
		inoperational.EventTransition(GameHashes.OperationalChanged, ready, IsOperational);
		operational.EventTransition(GameHashes.OperationalChanged, inoperational, GameStateMachine<ArtifactAnalysisStation, StatesInstance, IStateMachineTarget, Def>.Not(IsOperational)).EventTransition(GameHashes.OnStorageChange, ready, HasArtifactToStudy);
		ready.EventTransition(GameHashes.OperationalChanged, inoperational, GameStateMachine<ArtifactAnalysisStation, StatesInstance, IStateMachineTarget, Def>.Not(IsOperational)).EventTransition(GameHashes.OnStorageChange, operational, GameStateMachine<ArtifactAnalysisStation, StatesInstance, IStateMachineTarget, Def>.Not(HasArtifactToStudy)).ToggleChore(CreateChore, operational);
	}

	private bool HasArtifactToStudy(StatesInstance smi)
	{
		return smi.storage.GetMassAvailable(GameTags.CharmedArtifact) >= 1f;
	}

	private bool IsOperational(StatesInstance smi)
	{
		return smi.GetComponent<Operational>().IsOperational;
	}

	private Chore CreateChore(StatesInstance smi)
	{
		return new WorkChore<ArtifactAnalysisStationWorkable>(Db.Get().ChoreTypes.AnalyzeArtifact, smi.workable);
	}
}
