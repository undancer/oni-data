using System.Collections.Generic;
using KSerialization;

public class GeneticAnalysisStation : GameStateMachine<GeneticAnalysisStation, GeneticAnalysisStation.StatesInstance, IStateMachineTarget, GeneticAnalysisStation.Def>
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
		public GeneticAnalysisStationWorkable workable;

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
			RefreshFetchTags();
		}

		public void SetSeedForbidden(Tag seedID, bool forbidden)
		{
			if (forbiddenSeeds == null)
			{
				forbiddenSeeds = new HashSet<Tag>();
			}
			if ((!forbidden) ? forbiddenSeeds.Remove(seedID) : forbiddenSeeds.Add(seedID))
			{
				RefreshFetchTags();
			}
		}

		public bool GetSeedForbidden(Tag seedID)
		{
			if (forbiddenSeeds == null)
			{
				forbiddenSeeds = new HashSet<Tag>();
			}
			return forbiddenSeeds.Contains(seedID);
		}

		private void RefreshFetchTags()
		{
			if (forbiddenSeeds == null)
			{
				manualDelivery.ForbiddenTags = null;
				return;
			}
			Tag[] array = new Tag[forbiddenSeeds.Count];
			int num = 0;
			foreach (Tag forbiddenSeed in forbiddenSeeds)
			{
				array[num++] = forbiddenSeed;
				storage.Drop(forbiddenSeed);
			}
			manualDelivery.ForbiddenTags = array;
		}
	}

	public State inoperational;

	public State operational;

	public State ready;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = inoperational;
		inoperational.EventTransition(GameHashes.OperationalChanged, ready, IsOperational);
		operational.EventTransition(GameHashes.OperationalChanged, inoperational, GameStateMachine<GeneticAnalysisStation, StatesInstance, IStateMachineTarget, Def>.Not(IsOperational)).EventTransition(GameHashes.OnStorageChange, ready, HasSeedToStudy);
		ready.EventTransition(GameHashes.OperationalChanged, inoperational, GameStateMachine<GeneticAnalysisStation, StatesInstance, IStateMachineTarget, Def>.Not(IsOperational)).EventTransition(GameHashes.OnStorageChange, operational, GameStateMachine<GeneticAnalysisStation, StatesInstance, IStateMachineTarget, Def>.Not(HasSeedToStudy)).ToggleChore(CreateChore, operational);
	}

	private bool HasSeedToStudy(StatesInstance smi)
	{
		return smi.storage.GetMassAvailable(GameTags.UnidentifiedSeed) >= 1f;
	}

	private bool IsOperational(StatesInstance smi)
	{
		return smi.GetComponent<Operational>().IsOperational;
	}

	private Chore CreateChore(StatesInstance smi)
	{
		return new WorkChore<GeneticAnalysisStationWorkable>(Db.Get().ChoreTypes.Research, smi.workable);
	}
}
