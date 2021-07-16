using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class TemporalTearOpener : GameStateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>
{
	public class Def : BaseDef
	{
		public float consumeRate;

		public float numParticlesToOpen;
	}

	private class ChargingState : State
	{
		public State idle;

		public State consuming;
	}

	private class CheckRequirementsState : State
	{
		public State has_target;

		public State has_los;
	}

	public new class Instance : GameInstance, ISidescreenButtonControl
	{
		[Serialize]
		private float m_particlesConsumed = 0f;

		private MeterController m_meter;

		public string SidescreenButtonText => BUILDINGS.PREFABS.TEMPORALTEAROPENER.SIDESCREEN.TEXT;

		public string SidescreenButtonTooltip => BUILDINGS.PREFABS.TEMPORALTEAROPENER.SIDESCREEN.TOOLTIP;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			m_meter = new MeterController(base.gameObject.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer);
		}

		public bool HasLineOfSight()
		{
			Building component = GetComponent<Building>();
			Extents extents = component.GetExtents();
			int x = extents.x;
			int num = extents.x + extents.width - 1;
			for (int i = x; i <= num; i++)
			{
				int i2 = Grid.XYToCell(i, extents.y);
				if ((float)(int)Grid.ExposedToSunlight[i2] < 15f)
				{
					return false;
				}
			}
			return true;
		}

		public bool ConsumeParticlesAndCheckComplete(float dt)
		{
			float amount = Mathf.Min(dt * base.def.consumeRate, base.def.numParticlesToOpen - m_particlesConsumed);
			float num = GetComponent<HighEnergyParticleStorage>().ConsumeAndGet(amount);
			m_particlesConsumed += num;
			UpdateMeter();
			return m_particlesConsumed >= base.def.numParticlesToOpen;
		}

		public void UpdateMeter()
		{
			m_meter.SetPositionPercent(GetAmountComplete());
		}

		private float GetAmountComplete()
		{
			return Mathf.Min(m_particlesConsumed / base.def.numParticlesToOpen, 1f);
		}

		public float GetPercentComplete()
		{
			return GetAmountComplete() * 100f;
		}

		public void CreateBeamFX()
		{
			Vector3 position = base.gameObject.transform.position;
			position.y += 3.25f;
			Quaternion rotation = Quaternion.Euler(-90f, 90f, 0f);
			GameObject gameObject = Util.KInstantiate(EffectPrefabs.Instance.OpenTemporalTearBeam, position, rotation, base.gameObject);
		}

		public void OpenTemporalTear()
		{
			ClusterManager.Instance.GetClusterPOIManager().OpenTemporalTear(this.GetMyWorldId());
		}

		public bool SidescreenEnabled()
		{
			return GetCurrentState() == base.sm.ready;
		}

		public bool SidescreenButtonInteractable()
		{
			return true;
		}

		public void OnSidescreenButtonPressed()
		{
			base.smi.GoTo(base.sm.opening_tear_beam);
		}

		public int ButtonSideScreenSortOrder()
		{
			return 20;
		}
	}

	private const float MIN_SUNLIGHT_EXPOSURE = 15f;

	private static StatusItem s_noLosStatus = new StatusItem("Temporal_Tear_Opener_No_Los", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);

	private static StatusItem s_noTargetStatus = new StatusItem("Temporal_Tear_Opener_No_Target", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);

	private static StatusItem s_progressStatus = CreateProgressStatusItem();

	private CheckRequirementsState check_requirements;

	private ChargingState charging;

	private State opening_tear_beam;

	private State opening_tear_finish;

	private State ready;

	private State inert;

	private static StatusItem CreateProgressStatusItem()
	{
		StatusItem statusItem = new StatusItem("Temporal_Tear_Opener_Progress", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
		statusItem.resolveStringCallback = delegate(string str, object data)
		{
			Instance instance = (Instance)data;
			str = str.Replace("{progress}", GameUtil.GetFormattedPercent(instance.GetPercentComplete()));
			return str;
		};
		return statusItem;
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.Enter(delegate(Instance smi)
		{
			smi.UpdateMeter();
			if (ClusterManager.Instance.GetClusterPOIManager().IsTemporalTearOpen())
			{
				smi.GoTo(inert);
			}
			else
			{
				smi.GoTo(check_requirements);
			}
		});
		check_requirements.PlayAnim("off").DefaultState(check_requirements.has_target);
		check_requirements.has_target.ToggleStatusItem(s_noTargetStatus).UpdateTransition(check_requirements.has_los, (Instance smi, float dt) => ClusterManager.Instance.GetClusterPOIManager().IsTemporalTearRevealed());
		check_requirements.has_los.ToggleStatusItem(s_noLosStatus).UpdateTransition(charging, (Instance smi, float dt) => smi.HasLineOfSight());
		charging.DefaultState(charging.idle).PlayAnim("on").ToggleStatusItem(s_progressStatus, (Instance smi) => smi)
			.UpdateTransition(check_requirements.has_los, (Instance smi, float dt) => !smi.HasLineOfSight());
		charging.idle.EventTransition(GameHashes.OnParticleStorageChanged, charging.consuming, (Instance smi) => !smi.GetComponent<HighEnergyParticleStorage>().IsEmpty());
		charging.consuming.EventTransition(GameHashes.OnParticleStorageChanged, charging.idle, (Instance smi) => smi.GetComponent<HighEnergyParticleStorage>().IsEmpty()).UpdateTransition(ready, (Instance smi, float dt) => smi.ConsumeParticlesAndCheckComplete(dt));
		ready.ToggleNotification((Instance smi) => new Notification(BUILDING.STATUSITEMS.TEMPORAL_TEAR_OPENER_READY.NOTIFICATION, NotificationType.Good, (List<Notification> a, object b) => BUILDING.STATUSITEMS.TEMPORAL_TEAR_OPENER_READY.NOTIFICATION_TOOLTIP, null, expires: false)).PlayAnim("working_pre").QueueAnim("working_loop", loop: true);
		opening_tear_beam.Enter(delegate(Instance smi)
		{
			smi.CreateBeamFX();
		}).ScheduleGoTo(5f, opening_tear_finish);
		opening_tear_finish.PlayAnim("working_pst").OnAnimQueueComplete(inert);
		inert.Enter(delegate(Instance smi)
		{
			smi.OpenTemporalTear();
		});
	}
}