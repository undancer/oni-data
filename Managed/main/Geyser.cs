using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Geyser : StateMachineComponent<Geyser.StatesInstance>, IGameObjectEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Geyser, object>.GameInstance
	{
		public StatesInstance(Geyser smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Geyser>
	{
		public class EruptState : State
		{
			public State erupting;

			public State overpressure;
		}

		public State dormant;

		public State idle;

		public State pre_erupt;

		public EruptState erupt;

		public State post_erupt;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			base.serializable = SerializeType.Both_DEPRECATED;
			root.DefaultState(idle).Enter(delegate(StatesInstance smi)
			{
				smi.master.emitter.SetEmitting(emitting: false);
			});
			dormant.PlayAnim("inactive", KAnim.PlayMode.Loop).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutDormant).ScheduleGoTo((StatesInstance smi) => smi.master.RemainingDormantTime(), pre_erupt);
			idle.PlayAnim("inactive", KAnim.PlayMode.Loop).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutIdle).Enter(delegate(StatesInstance smi)
			{
				if (smi.master.ShouldGoDormant())
				{
					smi.GoTo(dormant);
				}
			})
				.ScheduleGoTo((StatesInstance smi) => smi.master.RemainingIdleTime(), pre_erupt);
			pre_erupt.PlayAnim("shake", KAnim.PlayMode.Loop).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutPressureBuilding).ScheduleGoTo((StatesInstance smi) => smi.master.RemainingEruptPreTime(), erupt);
			erupt.DefaultState(erupt.erupting).ScheduleGoTo((StatesInstance smi) => smi.master.RemainingEruptTime(), post_erupt).Enter(delegate(StatesInstance smi)
			{
				smi.master.emitter.SetEmitting(emitting: true);
			})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.emitter.SetEmitting(emitting: false);
				});
			erupt.erupting.EventTransition(GameHashes.EmitterBlocked, erupt.overpressure, (StatesInstance smi) => smi.GetComponent<ElementEmitter>().isEmitterBlocked).PlayAnim("erupt", KAnim.PlayMode.Loop);
			erupt.overpressure.EventTransition(GameHashes.EmitterUnblocked, erupt.erupting, (StatesInstance smi) => !smi.GetComponent<ElementEmitter>().isEmitterBlocked).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutOverPressure).PlayAnim("inactive", KAnim.PlayMode.Loop);
			post_erupt.PlayAnim("shake", KAnim.PlayMode.Loop).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutIdle).ScheduleGoTo((StatesInstance smi) => smi.master.RemainingEruptPostTime(), idle);
		}
	}

	public enum Phase
	{
		Pre,
		On,
		Pst,
		Off,
		Any
	}

	[MyCmpAdd]
	private ElementEmitter emitter;

	[Serialize]
	public GeyserConfigurator.GeyserInstanceConfiguration configuration;

	public Vector2I outputOffset;

	private const float PRE_PCT = 0.1f;

	private const float POST_PCT = 0.05f;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Prioritizable.AddRef(base.gameObject);
		if (configuration == null || configuration.typeId == HashedString.Invalid)
		{
			configuration = GetComponent<GeyserConfigurator>().MakeConfiguration();
		}
		emitter.emitRange = 2;
		emitter.maxPressure = configuration.GetMaxPressure();
		emitter.outputElement = new ElementConverter.OutputElement(configuration.GetEmitRate(), configuration.GetElement(), configuration.GetTemperature(), useEntityTemperature: false, storeOutput: false, outputOffset.x, outputOffset.y, 1f, configuration.GetDiseaseIdx(), Mathf.RoundToInt((float)configuration.GetDiseaseCount() * configuration.GetEmitRate()));
		base.smi.StartSM();
		Workable component = GetComponent<Studyable>();
		if (component != null)
		{
			component.alwaysShowProgressBar = true;
		}
	}

	public float RemainingPhaseTimeFrom2(float onDuration, float offDuration, float time, Phase expectedPhase)
	{
		float num = onDuration + offDuration;
		float num2 = time % num;
		float result;
		Phase phase;
		if (num2 < onDuration)
		{
			result = Mathf.Max(onDuration - num2, 0f);
			phase = Phase.On;
		}
		else
		{
			result = Mathf.Max(onDuration + offDuration - num2, 0f);
			phase = Phase.Off;
		}
		if (expectedPhase != Phase.Any && phase != expectedPhase)
		{
			return 0f;
		}
		return result;
	}

	public float RemainingPhaseTimeFrom4(float onDuration, float pstDuration, float offDuration, float preDuration, float time, Phase expectedPhase)
	{
		float num = onDuration + pstDuration + offDuration + preDuration;
		float num2 = time % num;
		float result;
		Phase phase;
		if (num2 < onDuration)
		{
			result = onDuration - num2;
			phase = Phase.On;
		}
		else if (num2 < onDuration + pstDuration)
		{
			result = onDuration + pstDuration - num2;
			phase = Phase.Pst;
		}
		else if (num2 < onDuration + pstDuration + offDuration)
		{
			result = onDuration + pstDuration + offDuration - num2;
			phase = Phase.Off;
		}
		else
		{
			result = onDuration + pstDuration + offDuration + preDuration - num2;
			phase = Phase.Pre;
		}
		if (expectedPhase != Phase.Any && phase != expectedPhase)
		{
			return 0f;
		}
		return result;
	}

	private float IdleDuration()
	{
		return configuration.GetOffDuration() * 0.84999996f;
	}

	private float PreDuration()
	{
		return configuration.GetOffDuration() * 0.1f;
	}

	private float PostDuration()
	{
		return configuration.GetOffDuration() * 0.05f;
	}

	private float EruptDuration()
	{
		return configuration.GetOnDuration();
	}

	public bool ShouldGoDormant()
	{
		return RemainingActiveTime() <= 0f;
	}

	public float RemainingIdleTime()
	{
		return RemainingPhaseTimeFrom4(EruptDuration(), PostDuration(), IdleDuration(), PreDuration(), GameClock.Instance.GetTime(), Phase.Off);
	}

	public float RemainingEruptPreTime()
	{
		return RemainingPhaseTimeFrom4(EruptDuration(), PostDuration(), IdleDuration(), PreDuration(), GameClock.Instance.GetTime(), Phase.Pre);
	}

	public float RemainingEruptTime()
	{
		return RemainingPhaseTimeFrom2(configuration.GetOnDuration(), configuration.GetOffDuration(), GameClock.Instance.GetTime(), Phase.On);
	}

	public float RemainingEruptPostTime()
	{
		return RemainingPhaseTimeFrom4(EruptDuration(), PostDuration(), IdleDuration(), PreDuration(), GameClock.Instance.GetTime(), Phase.Pst);
	}

	public float RemainingNonEruptTime()
	{
		return RemainingPhaseTimeFrom2(configuration.GetOnDuration(), configuration.GetOffDuration(), GameClock.Instance.GetTime(), Phase.Off);
	}

	public float RemainingDormantTime()
	{
		return RemainingPhaseTimeFrom2(configuration.GetYearOnDuration(), configuration.GetYearOffDuration(), GameClock.Instance.GetTime(), Phase.Off);
	}

	public float RemainingActiveTime()
	{
		return RemainingPhaseTimeFrom2(configuration.GetYearOnDuration(), configuration.GetYearOffDuration(), GameClock.Instance.GetTime(), Phase.On);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		string arg = ElementLoader.FindElementByHash(configuration.GetElement()).tag.ProperName();
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_PRODUCTION, arg, GameUtil.GetFormattedMass(configuration.GetEmitRate(), GameUtil.TimeSlice.PerSecond), GameUtil.GetFormattedTemperature(configuration.GetTemperature())), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PRODUCTION, ElementLoader.FindElementByHash(configuration.GetElement()).name, GameUtil.GetFormattedMass(configuration.GetEmitRate(), GameUtil.TimeSlice.PerSecond), GameUtil.GetFormattedTemperature(configuration.GetTemperature()))));
		if (configuration.GetDiseaseIdx() != byte.MaxValue)
		{
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_DISEASE, GameUtil.GetFormattedDiseaseName(configuration.GetDiseaseIdx())), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_DISEASE, GameUtil.GetFormattedDiseaseName(configuration.GetDiseaseIdx()))));
		}
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_PERIOD, GameUtil.GetFormattedTime(configuration.GetOnDuration()), GameUtil.GetFormattedTime(configuration.GetIterationLength())), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PERIOD, GameUtil.GetFormattedTime(configuration.GetOnDuration()), GameUtil.GetFormattedTime(configuration.GetIterationLength()))));
		Studyable component = GetComponent<Studyable>();
		if ((bool)component && !component.Studied)
		{
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_UNSTUDIED), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_UNSTUDIED)));
		}
		else
		{
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_PERIOD, GameUtil.GetFormattedCycles(configuration.GetYearOnDuration()), GameUtil.GetFormattedCycles(configuration.GetYearLength())), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_PERIOD, GameUtil.GetFormattedCycles(configuration.GetYearOnDuration()), GameUtil.GetFormattedCycles(configuration.GetYearLength()))));
			if (base.smi.IsInsideState(base.smi.sm.dormant))
			{
				list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_NEXT_ACTIVE, GameUtil.GetFormattedCycles(RemainingDormantTime())), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_NEXT_ACTIVE, GameUtil.GetFormattedCycles(RemainingDormantTime()))));
			}
			else
			{
				list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_NEXT_DORMANT, GameUtil.GetFormattedCycles(RemainingActiveTime())), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_NEXT_DORMANT, GameUtil.GetFormattedCycles(RemainingActiveTime()))));
			}
		}
		return list;
	}
}
