using UnityEngine;

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

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			workable.statesInstance = this;
		}

		public override void StartSM()
		{
			base.StartSM();
			Tag targetAsSeed = GetTargetAsSeed();
			if (targetAsSeed.IsValid)
			{
				manualDelivery.RequestedItemTag = targetAsSeed;
			}
		}

		public void SetTargetPlant(Tag targetPlantID)
		{
			Tag a = base.sm.selectedPlant.Get(this);
			if (a != targetPlantID)
			{
				if (!targetPlantID.IsValid)
				{
					base.sm.selectedPlant.Set(Tag.Invalid, this);
					manualDelivery.RequestedItemTag = Tag.Invalid;
					return;
				}
				base.sm.selectedPlant.Set(targetPlantID, this);
				Tag targetAsSeed = GetTargetAsSeed();
				storage.DropAll();
				manualDelivery.RequestedItemTag = targetAsSeed;
			}
		}

		public Tag GetTargetPlant()
		{
			return base.sm.selectedPlant.Get(this);
		}

		public Tag GetTargetAsSeed()
		{
			Tag tag = base.sm.selectedPlant.Get(this);
			if (!tag.IsValid)
			{
				return Tag.Invalid;
			}
			GameObject prefab = Assets.GetPrefab(tag);
			SeedProducer component = prefab.GetComponent<SeedProducer>();
			SeedProducer.SeedInfo seedInfo = component.seedInfo;
			return seedInfo.seedId.ToTag();
		}
	}

	public const int SEEDS_TO_COMPLETE = 20;

	public State idle;

	public State mutationSelected;

	public State ready;

	public TagParameter selectedPlant;

	public override void InitializeStates(out BaseState default_state)
	{
		base.serializable = SerializeType.ParamsOnly;
		default_state = idle;
		root.EventHandler(GameHashes.PlantSubspeciesComplete, (StatesInstance smi) => PlantSubSpeciesCatalog.instance, OnPlantSubspeciesComplete);
		idle.ParamTransition(selectedPlant, ready, (StatesInstance smi, Tag p) => p.IsValid);
		mutationSelected.ParamTransition(selectedPlant, idle, (StatesInstance smi, Tag p) => !p.IsValid).EventTransition(GameHashes.OnStorageChange, ready, HasSeedToStudy);
		ready.ParamTransition(selectedPlant, idle, (StatesInstance smi, Tag p) => !p.IsValid).EventTransition(GameHashes.OnStorageChange, mutationSelected, GameStateMachine<GeneticAnalysisStation, StatesInstance, IStateMachineTarget, Def>.Not(HasSeedToStudy)).ToggleChore(CreateChore, mutationSelected);
	}

	private bool HasSeedToStudy(StatesInstance smi)
	{
		return smi.storage.GetMassAvailable(GameTags.Seed) >= 1f;
	}

	private Chore CreateChore(StatesInstance smi)
	{
		return new WorkChore<GeneticAnalysisStationWorkable>(Db.Get().ChoreTypes.Research, smi.workable);
	}

	private void OnPlantSubspeciesComplete(StatesInstance smi, object data)
	{
		Tag a = (Tag)data;
		if (a == selectedPlant.Get(smi))
		{
			selectedPlant.Set(Tag.Invalid, smi);
		}
	}
}
