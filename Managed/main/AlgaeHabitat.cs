using UnityEngine;

public class AlgaeHabitat : StateMachineComponent<AlgaeHabitat.SMInstance>
{
	public class SMInstance : GameStateMachine<States, SMInstance, AlgaeHabitat, object>.GameInstance
	{
		public ElementConverter converter;

		public Chore emptyChore;

		public SMInstance(AlgaeHabitat master)
			: base(master)
		{
			converter = master.GetComponent<ElementConverter>();
		}

		public bool HasEnoughMass(Tag tag)
		{
			return converter.HasEnoughMass(tag);
		}

		public bool NeedsEmptying()
		{
			return base.smi.master.pollutedWaterStorage.RemainingCapacity() <= 0f;
		}

		public void CreateEmptyChore()
		{
			if (emptyChore != null)
			{
				emptyChore.Cancel("dupe");
			}
			AlgaeHabitatEmpty component = base.master.GetComponent<AlgaeHabitatEmpty>();
			emptyChore = new WorkChore<AlgaeHabitatEmpty>(Db.Get().ChoreTypes.EmptyStorage, component, null, run_until_complete: true, OnEmptyComplete, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: true, PriorityScreen.PriorityClass.basic, 5, ignore_building_assignment: true);
		}

		public void CancelEmptyChore()
		{
			if (emptyChore != null)
			{
				emptyChore.Cancel("Cancelled");
				emptyChore = null;
			}
		}

		private void OnEmptyComplete(Chore chore)
		{
			emptyChore = null;
			base.master.pollutedWaterStorage.DropAll(vent_gas: true);
		}
	}

	public class States : GameStateMachine<States, SMInstance, AlgaeHabitat>
	{
		public State generatingOxygen;

		public State stoppedGeneratingOxygen;

		public State stoppedGeneratingOxygenTransition;

		public State noWater;

		public State noAlgae;

		public State needsEmptying;

		public State gotAlgae;

		public State gotWater;

		public State gotEmptied;

		public State lostAlgae;

		public State notoperational;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = noAlgae;
			root.EventTransition(GameHashes.OperationalChanged, notoperational, (SMInstance smi) => !smi.master.operational.IsOperational).EventTransition(GameHashes.OperationalChanged, noAlgae, (SMInstance smi) => smi.master.operational.IsOperational);
			notoperational.QueueAnim("off");
			gotAlgae.PlayAnim("on_pre").OnAnimQueueComplete(noWater);
			gotEmptied.PlayAnim("on_pre").OnAnimQueueComplete(generatingOxygen);
			lostAlgae.PlayAnim("on_pst").OnAnimQueueComplete(noAlgae);
			noAlgae.QueueAnim("off").EventTransition(GameHashes.OnStorageChange, gotAlgae, (SMInstance smi) => smi.HasEnoughMass(GameTags.Algae)).Enter(delegate(SMInstance smi)
			{
				smi.master.operational.SetActive(value: false);
			});
			noWater.QueueAnim("on").Enter(delegate(SMInstance smi)
			{
				smi.master.GetComponent<PassiveElementConsumer>().EnableConsumption(enabled: true);
			}).EventTransition(GameHashes.OnStorageChange, lostAlgae, (SMInstance smi) => !smi.HasEnoughMass(GameTags.Algae))
				.EventTransition(GameHashes.OnStorageChange, gotWater, (SMInstance smi) => smi.HasEnoughMass(GameTags.Algae) && smi.HasEnoughMass(GameTags.Water));
			needsEmptying.QueueAnim("off").Enter(delegate(SMInstance smi)
			{
				smi.CreateEmptyChore();
			}).Exit(delegate(SMInstance smi)
			{
				smi.CancelEmptyChore();
			})
				.ToggleStatusItem(Db.Get().BuildingStatusItems.HabitatNeedsEmptying)
				.EventTransition(GameHashes.OnStorageChange, noAlgae, (SMInstance smi) => !smi.HasEnoughMass(GameTags.Algae) || !smi.HasEnoughMass(GameTags.Water))
				.EventTransition(GameHashes.OnStorageChange, gotEmptied, (SMInstance smi) => smi.HasEnoughMass(GameTags.Algae) && smi.HasEnoughMass(GameTags.Water) && !smi.NeedsEmptying());
			gotWater.PlayAnim("working_pre").OnAnimQueueComplete(needsEmptying);
			generatingOxygen.Enter(delegate(SMInstance smi)
			{
				smi.master.operational.SetActive(value: true);
			}).Exit(delegate(SMInstance smi)
			{
				smi.master.operational.SetActive(value: false);
			}).Update("GeneratingOxygen", delegate(SMInstance smi, float dt)
			{
				int num = Grid.PosToCell(smi.master.transform.GetPosition());
				smi.converter.OutputMultiplier = ((Grid.LightCount[num] > 0) ? smi.master.lightBonusMultiplier : 1f);
			})
				.QueueAnim("working_loop", loop: true)
				.EventTransition(GameHashes.OnStorageChange, stoppedGeneratingOxygen, (SMInstance smi) => !smi.HasEnoughMass(GameTags.Water) || !smi.HasEnoughMass(GameTags.Algae) || smi.NeedsEmptying());
			stoppedGeneratingOxygen.PlayAnim("working_pst").OnAnimQueueComplete(stoppedGeneratingOxygenTransition);
			stoppedGeneratingOxygenTransition.EventTransition(GameHashes.OnStorageChange, needsEmptying, (SMInstance smi) => smi.NeedsEmptying()).EventTransition(GameHashes.OnStorageChange, noWater, (SMInstance smi) => !smi.HasEnoughMass(GameTags.Water)).EventTransition(GameHashes.OnStorageChange, lostAlgae, (SMInstance smi) => !smi.HasEnoughMass(GameTags.Algae))
				.EventTransition(GameHashes.OnStorageChange, gotWater, (SMInstance smi) => smi.HasEnoughMass(GameTags.Water) && smi.HasEnoughMass(GameTags.Algae));
		}
	}

	[MyCmpGet]
	private Operational operational;

	private Storage pollutedWaterStorage;

	[SerializeField]
	public float lightBonusMultiplier = 1.1f;

	public CellOffset pressureSampleOffset = CellOffset.none;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		GameScheduler.Instance.Schedule("WaterFetchingTutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater);
		});
		ConfigurePollutedWaterOutput();
		Tutorial.Instance.oxygenGenerators.Add(base.gameObject);
	}

	protected override void OnCleanUp()
	{
		Tutorial.Instance.oxygenGenerators.Remove(base.gameObject);
		base.OnCleanUp();
	}

	private void ConfigurePollutedWaterOutput()
	{
		Storage storage = null;
		Tag tag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		Storage[] components = GetComponents<Storage>();
		foreach (Storage storage2 in components)
		{
			if (storage2.storageFilters.Contains(tag))
			{
				storage = storage2;
				break;
			}
		}
		ElementConverter[] components2 = GetComponents<ElementConverter>();
		foreach (ElementConverter elementConverter in components2)
		{
			ElementConverter.OutputElement[] outputElements = elementConverter.outputElements;
			for (int j = 0; j < outputElements.Length; j++)
			{
				if (outputElements[j].elementHash == SimHashes.DirtyWater)
				{
					elementConverter.SetStorage(storage);
					break;
				}
			}
		}
		pollutedWaterStorage = storage;
	}
}
