using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class BuildingInternalConstructor : GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>
{
	public class Def : BaseDef
	{
		public DefComponent<Storage> storage;

		public float constructionMass;

		public List<string> outputIDs;

		public bool spawnIntoStorage;

		public string constructionSymbol;
	}

	public class OperationalStates : State
	{
		public State constructionRequired;

		public State constructionHappening;

		public State constructionSatisfied;
	}

	public new class Instance : GameInstance, ISidescreenButtonControl
	{
		private Storage storage;

		[Serialize]
		private float constructionElapsed;

		private ProgressBar progressBar;

		public string SidescreenButtonText
		{
			get
			{
				if (!base.smi.sm.constructionRequested.Get(base.smi))
				{
					return string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.ALLOW_INTERNAL_CONSTRUCTOR.text, Assets.GetPrefab(base.def.outputIDs[0]).GetProperName());
				}
				return string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.DISALLOW_INTERNAL_CONSTRUCTOR.text, Assets.GetPrefab(base.def.outputIDs[0]).GetProperName());
			}
		}

		public string SidescreenButtonTooltip
		{
			get
			{
				if (!base.smi.sm.constructionRequested.Get(base.smi))
				{
					return string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.ALLOW_INTERNAL_CONSTRUCTOR_TOOLTIP.text, Assets.GetPrefab(base.def.outputIDs[0]).GetProperName());
				}
				return string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.DISALLOW_INTERNAL_CONSTRUCTOR_TOOLTIP.text, Assets.GetPrefab(base.def.outputIDs[0]).GetProperName());
			}
		}

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			storage = def.storage.Get(this);
			GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new InternalConstructionCompleteCondition(this));
		}

		protected override void OnCleanUp()
		{
			Element element = null;
			float num = 0f;
			float num2 = 0f;
			byte disease_idx = byte.MaxValue;
			int disease_count = 0;
			foreach (string outputID in base.def.outputIDs)
			{
				GameObject gameObject = storage.FindFirst(outputID);
				if (gameObject != null)
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					Debug.Assert(element == null || element == component.Element);
					element = component.Element;
					num2 = GameUtil.GetFinalTemperature(num, num2, component.Mass, component.Temperature);
					num += component.Mass;
					gameObject.DeleteObject();
				}
			}
			element?.substance.SpawnResource(base.transform.GetPosition(), num, num2, disease_idx, disease_count);
			base.OnCleanUp();
		}

		public FetchList2 CreateFetchList()
		{
			FetchList2 fetchList = new FetchList2(storage, Db.Get().ChoreTypes.Fetch);
			PrimaryElement component = GetComponent<PrimaryElement>();
			fetchList.Add(component.Element.tag, null, null, base.def.constructionMass);
			return fetchList;
		}

		public PrimaryElement GetMassForConstruction()
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			return storage.FindFirstWithMass(component.Element.tag, base.def.constructionMass);
		}

		public bool HasOutputInStorage()
		{
			return storage.FindFirst(base.def.outputIDs[0].ToTag());
		}

		public bool IsRequestingConstruction()
		{
			base.sm.constructionRequested.Get(this);
			return base.smi.sm.constructionRequested.Get(base.smi);
		}

		public void ConstructionComplete(bool force = false)
		{
			SimHashes element_id;
			if (!force)
			{
				PrimaryElement massForConstruction = GetMassForConstruction();
				element_id = massForConstruction.ElementID;
				float mass = massForConstruction.Mass;
				float num = massForConstruction.Temperature * massForConstruction.Mass;
				massForConstruction.Mass -= base.def.constructionMass;
				Mathf.Clamp(num / mass, 288.15f, 318.15f);
			}
			else
			{
				element_id = SimHashes.Cuprite;
				_ = GetComponent<PrimaryElement>().Temperature;
			}
			foreach (string outputID in base.def.outputIDs)
			{
				GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(outputID), base.transform.GetPosition(), Grid.SceneLayer.Ore);
				gameObject.GetComponent<PrimaryElement>().SetElement(element_id, addTags: false);
				gameObject.SetActive(value: true);
				if (base.def.spawnIntoStorage)
				{
					storage.Store(gameObject);
				}
			}
		}

		public WorkChore<BuildingInternalConstructorWorkable> CreateWorkChore()
		{
			return new WorkChore<BuildingInternalConstructorWorkable>(Db.Get().ChoreTypes.Build, base.master);
		}

		public void ShowConstructionSymbol(bool show)
		{
			KBatchedAnimController component = base.master.GetComponent<KBatchedAnimController>();
			if (component != null)
			{
				component.SetSymbolVisiblity(base.def.constructionSymbol, show);
			}
		}

		public void OnSidescreenButtonPressed()
		{
			base.smi.sm.constructionRequested.Set(!base.smi.sm.constructionRequested.Get(base.smi), base.smi);
			if (DebugHandler.InstantBuildMode && base.smi.sm.constructionRequested.Get(base.smi) && !HasOutputInStorage())
			{
				ConstructionComplete(force: true);
			}
		}

		public bool SidescreenEnabled()
		{
			return true;
		}

		public bool SidescreenButtonInteractable()
		{
			return true;
		}

		public int ButtonSideScreenSortOrder()
		{
			return 20;
		}
	}

	public State inoperational;

	public OperationalStates operational;

	public BoolParameter constructionRequested = new BoolParameter(default_value: true);

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = inoperational;
		base.serializable = SerializeType.ParamsOnly;
		inoperational.EventTransition(GameHashes.OperationalChanged, operational, (Instance smi) => smi.GetComponent<Operational>().IsOperational).Enter(delegate(Instance smi)
		{
			smi.ShowConstructionSymbol(show: false);
		});
		operational.DefaultState(operational.constructionRequired).EventTransition(GameHashes.OperationalChanged, inoperational, (Instance smi) => !smi.GetComponent<Operational>().IsOperational);
		operational.constructionRequired.EventTransition(GameHashes.OnStorageChange, operational.constructionHappening, (Instance smi) => smi.GetMassForConstruction() != null).EventTransition(GameHashes.OnStorageChange, operational.constructionSatisfied, (Instance smi) => smi.HasOutputInStorage()).ToggleFetch((Instance smi) => smi.CreateFetchList(), operational.constructionHappening)
			.ParamTransition(constructionRequested, operational.constructionSatisfied, GameStateMachine<BuildingInternalConstructor, Instance, IStateMachineTarget, Def>.IsFalse)
			.Enter(delegate(Instance smi)
			{
				smi.ShowConstructionSymbol(show: true);
			})
			.Exit(delegate(Instance smi)
			{
				smi.ShowConstructionSymbol(show: false);
			});
		operational.constructionHappening.EventTransition(GameHashes.OnStorageChange, operational.constructionSatisfied, (Instance smi) => smi.HasOutputInStorage()).EventTransition(GameHashes.OnStorageChange, operational.constructionRequired, (Instance smi) => smi.GetMassForConstruction() == null).ToggleChore((Instance smi) => smi.CreateWorkChore(), operational.constructionHappening, operational.constructionHappening)
			.ParamTransition(constructionRequested, operational.constructionSatisfied, GameStateMachine<BuildingInternalConstructor, Instance, IStateMachineTarget, Def>.IsFalse)
			.Enter(delegate(Instance smi)
			{
				smi.ShowConstructionSymbol(show: true);
			})
			.Exit(delegate(Instance smi)
			{
				smi.ShowConstructionSymbol(show: false);
			});
		operational.constructionSatisfied.EventTransition(GameHashes.OnStorageChange, operational.constructionRequired, (Instance smi) => !smi.HasOutputInStorage() && constructionRequested.Get(smi)).ParamTransition(constructionRequested, operational.constructionRequired, (Instance smi, bool p) => p && !smi.HasOutputInStorage());
	}
}
