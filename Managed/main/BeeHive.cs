using Klei.AI;
using UnityEngine;

public class BeeHive : GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>
{
	public class Def : BaseDef
	{
		public string beePrefabID;

		public string larvaPrefabID;
	}

	public class GrowingStates : State
	{
		public State idle;
	}

	public class GrownStates : State
	{
		public State dayTime;

		public State nightTime;
	}

	public class EnabledStates : State
	{
		public GrowingStates growingStates;

		public GrownStates grownStates;
	}

	public class StatesInstance : GameInstance
	{
		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			Subscribe(1119167081, OnNewGameSpawn);
			Components.BeeHives.Add(this);
		}

		public void SetUpNewHive()
		{
			base.sm.hiveGrowth.Set(0f, this);
		}

		protected override void OnCleanUp()
		{
			Components.BeeHives.Remove(this);
			base.OnCleanUp();
		}

		private void OnNewGameSpawn(object data)
		{
			NewGamePopulateHive();
		}

		private void NewGamePopulateHive()
		{
			int num = 1;
			for (int i = 0; i < num; i++)
			{
				SpawnNewBeeFromHive();
			}
			num = 1;
			for (int j = 0; j < num; j++)
			{
				SpawnNewLarvaFromHive();
			}
		}

		public bool IsFullyGrown()
		{
			return base.sm.hiveGrowth.Get(this) >= 1f;
		}

		public void DeltaGrowth(float delta)
		{
			float num = base.sm.hiveGrowth.Get(this);
			num += delta;
			Mathf.Clamp01(num);
			base.sm.hiveGrowth.Set(num, this);
		}

		public void SpawnNewLarvaFromHive()
		{
			Util.KInstantiate(Assets.GetPrefab(base.def.larvaPrefabID), base.transform.GetPosition()).SetActive(value: true);
		}

		public void SpawnNewBeeFromHive()
		{
			Util.KInstantiate(Assets.GetPrefab(base.def.beePrefabID), base.transform.GetPosition()).SetActive(value: true);
		}

		public bool IsDisabled()
		{
			KPrefabID component = GetComponent<KPrefabID>();
			if (!component.HasTag(GameTags.Creatures.HasNoFoundation) && !component.HasTag(GameTags.Entombed))
			{
				return component.HasTag(GameTags.Creatures.Drowning);
			}
			return true;
		}
	}

	public State disabled;

	public EnabledStates enabled;

	public FloatParameter hiveGrowth = new FloatParameter(1f);

	public override void InitializeStates(out BaseState default_state)
	{
		base.serializable = SerializeType.ParamsOnly;
		default_state = enabled.grownStates;
		root.DoTutorial(Tutorial.TutorialMessages.TM_Radiation).Enter(delegate(StatesInstance smi)
		{
			AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(smi.gameObject);
			if (amountInstance != null)
			{
				amountInstance.hide = true;
			}
		}).EventHandler(GameHashes.Died, delegate(StatesInstance smi)
		{
			PrimaryElement component = smi.GetComponent<PrimaryElement>();
			Storage component2 = smi.GetComponent<Storage>();
			component2.AddOre(disease_idx: Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.id), element: SimHashes.NuclearWaste, mass: BeeHiveTuning.WASTE_DROPPED_ON_DEATH, temperature: component.Temperature, disease_count: BeeHiveTuning.GERMS_DROPPED_ON_DEATH);
			component2.DropAll(smi.master.transform.position, vent_gas: true, dump_liquid: true);
		});
		disabled.ToggleTag(GameTags.Creatures.Behaviours.DisableCreature).EventTransition(GameHashes.FoundationChanged, enabled, (StatesInstance smi) => !smi.IsDisabled()).EventTransition(GameHashes.EntombedChanged, enabled, (StatesInstance smi) => !smi.IsDisabled())
			.EventTransition(GameHashes.EnteredBreathableArea, enabled, (StatesInstance smi) => !smi.IsDisabled());
		enabled.EventTransition(GameHashes.FoundationChanged, disabled, (StatesInstance smi) => smi.IsDisabled()).EventTransition(GameHashes.EntombedChanged, disabled, (StatesInstance smi) => smi.IsDisabled()).EventTransition(GameHashes.Drowning, disabled, (StatesInstance smi) => smi.IsDisabled())
			.DefaultState(enabled.grownStates);
		enabled.growingStates.ParamTransition(hiveGrowth, enabled.grownStates, (StatesInstance smi, float f) => f >= 1f).DefaultState(enabled.growingStates.idle);
		enabled.growingStates.idle.Update(delegate(StatesInstance smi, float dt)
		{
			smi.DeltaGrowth(dt / 600f / BeeHiveTuning.HIVE_GROWTH_TIME);
		}, UpdateRate.SIM_4000ms);
		enabled.grownStates.ParamTransition(hiveGrowth, enabled.growingStates, (StatesInstance smi, float f) => f < 1f).DefaultState(enabled.grownStates.dayTime);
		enabled.grownStates.dayTime.EventTransition(GameHashes.Nighttime, (StatesInstance smi) => GameClock.Instance, enabled.grownStates.nightTime, (StatesInstance smi) => GameClock.Instance.IsNighttime());
		enabled.grownStates.nightTime.EventTransition(GameHashes.NewDay, (StatesInstance smi) => GameClock.Instance, enabled.grownStates.dayTime, (StatesInstance smi) => GameClock.Instance.GetTimeSinceStartOfCycle() <= 1f).Exit(delegate(StatesInstance smi)
		{
			if (!GameClock.Instance.IsNighttime())
			{
				smi.SpawnNewLarvaFromHive();
			}
		});
	}
}
