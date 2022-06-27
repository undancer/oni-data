using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class NuclearResearchCenter : StateMachineComponent<NuclearResearchCenter.StatesInstance>, IResearchCenter, IGameObjectEffectDescriptor
{
	public class States : GameStateMachine<States, StatesInstance, NuclearResearchCenter>
	{
		public class RequirementsState : State
		{
			public State highEnergyParticlesNeeded;

			public State noResearchSelected;

			public State noApplicableResearch;
		}

		public class ReadyState : State
		{
			public State idle;

			public State working;
		}

		public State inoperational;

		public RequirementsState requirements;

		public ReadyState ready;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = inoperational;
			inoperational.PlayAnim("off").TagTransition(GameTags.Operational, requirements);
			requirements.PlayAnim("on").TagTransition(GameTags.Operational, inoperational, on_remove: true).DefaultState(requirements.highEnergyParticlesNeeded);
			requirements.highEnergyParticlesNeeded.ToggleMainStatusItem(Db.Get().BuildingStatusItems.WaitingForHighEnergyParticles).EventTransition(GameHashes.OnParticleStorageChanged, requirements.noResearchSelected, IsReady);
			requirements.noResearchSelected.ToggleMainStatusItem(Db.Get().BuildingStatusItems.NoResearchSelected).EventTransition(GameHashes.ActiveResearchChanged, requirements.noApplicableResearch, IsResearchSelected);
			requirements.noApplicableResearch.EventTransition(GameHashes.ActiveResearchChanged, ready, IsResearchApplicable).EventTransition(GameHashes.ActiveResearchChanged, requirements, GameStateMachine<States, StatesInstance, NuclearResearchCenter, object>.Not(IsResearchSelected));
			ready.Enter(delegate(StatesInstance smi)
			{
				smi.CreateChore();
			}).TagTransition(GameTags.Operational, inoperational, on_remove: true).DefaultState(ready.idle)
				.Exit(delegate(StatesInstance smi)
				{
					smi.DestroyChore();
				})
				.EventTransition(GameHashes.ActiveResearchChanged, requirements.noResearchSelected, GameStateMachine<States, StatesInstance, NuclearResearchCenter, object>.Not(IsResearchSelected))
				.EventTransition(GameHashes.ActiveResearchChanged, requirements.noApplicableResearch, GameStateMachine<States, StatesInstance, NuclearResearchCenter, object>.Not(IsResearchApplicable))
				.EventTransition(GameHashes.ResearchPointsChanged, requirements.noApplicableResearch, GameStateMachine<States, StatesInstance, NuclearResearchCenter, object>.Not(IsResearchApplicable))
				.EventTransition(GameHashes.OnParticleStorageEmpty, requirements.highEnergyParticlesNeeded, GameStateMachine<States, StatesInstance, NuclearResearchCenter, object>.Not(HasRadiation));
			ready.idle.WorkableStartTransition((StatesInstance smi) => smi.master.GetComponent<NuclearResearchCenterWorkable>(), ready.working);
			ready.working.Enter("SetActive(true)", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: true);
			}).Exit("SetActive(false)", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: false);
			}).WorkableStopTransition((StatesInstance smi) => smi.master.GetComponent<NuclearResearchCenterWorkable>(), ready.idle)
				.WorkableCompleteTransition((StatesInstance smi) => smi.master.GetComponent<NuclearResearchCenterWorkable>(), ready.idle);
		}

		private bool IsReady(StatesInstance smi)
		{
			return smi.GetComponent<HighEnergyParticleStorage>().Particles > smi.master.materialPerPoint;
		}

		private bool IsResearchSelected(StatesInstance smi)
		{
			return Research.Instance.GetActiveResearch() != null;
		}

		private bool IsResearchApplicable(StatesInstance smi)
		{
			TechInstance activeResearch = Research.Instance.GetActiveResearch();
			if (activeResearch != null && activeResearch.tech.costsByResearchTypeID.ContainsKey(smi.master.researchTypeID))
			{
				float num = activeResearch.progressInventory.PointsByTypeID[smi.master.researchTypeID];
				float num2 = activeResearch.tech.costsByResearchTypeID[smi.master.researchTypeID];
				return num < num2;
			}
			return false;
		}

		private bool HasRadiation(StatesInstance smi)
		{
			return !smi.GetComponent<HighEnergyParticleStorage>().IsEmpty();
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, NuclearResearchCenter, object>.GameInstance
	{
		private WorkChore<NuclearResearchCenterWorkable> chore;

		public StatesInstance(NuclearResearchCenter master)
			: base(master)
		{
		}

		public void CreateChore()
		{
			Workable component = base.smi.master.GetComponent<NuclearResearchCenterWorkable>();
			chore = new WorkChore<NuclearResearchCenterWorkable>(Db.Get().ChoreTypes.Research, component, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: true);
			chore.preemption_cb = CanPreemptCB;
		}

		public void DestroyChore()
		{
			chore.Cancel("destroy me!");
			chore = null;
		}

		private static bool CanPreemptCB(Chore.Precondition.Context context)
		{
			Worker component = context.chore.driver.GetComponent<Worker>();
			float num = Db.Get().AttributeConverters.ResearchSpeed.Lookup(component).Evaluate();
			Worker worker = context.consumerState.worker;
			float num2 = Db.Get().AttributeConverters.ResearchSpeed.Lookup(worker).Evaluate();
			TechInstance activeResearch = Research.Instance.GetActiveResearch();
			if (activeResearch != null)
			{
				StatesInstance sMI = context.chore.gameObject.GetSMI<StatesInstance>();
				if (sMI != null)
				{
					if (num2 > num)
					{
						return activeResearch.PercentageCompleteResearchType(sMI.master.researchTypeID) < 1f;
					}
					return false;
				}
			}
			return false;
		}
	}

	[MyCmpGet]
	private Operational operational;

	public string researchTypeID;

	public float materialPerPoint = 50f;

	public float timePerPoint;

	public Tag inputMaterial;

	[MyCmpReq]
	private HighEnergyParticleStorage particleStorage;

	public Meter.Offset particleMeterOffset;

	private MeterController particleMeter;

	private static readonly EventSystem.IntraObjectHandler<NuclearResearchCenter> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<NuclearResearchCenter>(delegate(NuclearResearchCenter component, object data)
	{
		component.OnStorageChange(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.ResearchCenters.Add(this);
		particleMeter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", particleMeterOffset, Grid.SceneLayer.NoLayer, "meter_target");
		Subscribe(-1837862626, OnStorageChangeDelegate);
		RefreshMeter();
		base.smi.StartSM();
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Radiation);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.ResearchCenters.Remove(this);
	}

	public string GetResearchType()
	{
		return researchTypeID;
	}

	private void OnStorageChange(object data)
	{
		RefreshMeter();
	}

	private void RefreshMeter()
	{
		float positionPercent = Mathf.Clamp01(particleStorage.Particles / particleStorage.Capacity());
		particleMeter.SetPositionPercent(positionPercent);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.BUILDINGEFFECTS.RESEARCH_MATERIALS, inputMaterial.ProperName(), GameUtil.GetFormattedByTag(inputMaterial, materialPerPoint)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.RESEARCH_MATERIALS, inputMaterial.ProperName(), GameUtil.GetFormattedByTag(inputMaterial, materialPerPoint)), Descriptor.DescriptorType.Requirement),
			new Descriptor(string.Format(UI.BUILDINGEFFECTS.PRODUCES_RESEARCH_POINTS, Research.Instance.researchTypes.GetResearchType(researchTypeID).name), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.PRODUCES_RESEARCH_POINTS, Research.Instance.researchTypes.GetResearchType(researchTypeID).name))
		};
	}
}
