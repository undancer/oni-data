using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class HEPBattery : GameStateMachine<HEPBattery, HEPBattery.Instance, IStateMachineTarget, HEPBattery.Def>
{
	public class Def : BaseDef
	{
		public float particleDecayRate;

		public float minLaunchInterval;

		public float minSlider;

		public float maxSlider;

		public EightDirection direction;
	}

	public new class Instance : GameInstance, ISingleSliderControl, ISliderControl
	{
		[MyCmpReq]
		public HighEnergyParticleStorage particleStorage;

		[MyCmpGet]
		public Operational operational;

		[Serialize]
		public float launcherTimer;

		[Serialize]
		public float particleThreshold = 50f;

		public bool ShowWorkingStatus;

		private bool m_skipFirstUpdate = true;

		private MeterController meterController;

		private Guid statusHandle = Guid.Empty;

		private bool hasLogicWire;

		private bool isLogicActive;

		public bool AllowSpawnParticles
		{
			get
			{
				if (hasLogicWire)
				{
					return isLogicActive;
				}
				return false;
			}
		}

		public bool HasLogicWire => hasLogicWire;

		public bool IsLogicActive => isLogicActive;

		public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TITLE";

		public string SliderUnits => UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			Subscribe(-801688580, OnLogicValueChanged);
			meterController = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer);
			UpdateMeter();
		}

		public void DoConsumeParticlesWhileDisabled(float dt)
		{
			if (m_skipFirstUpdate)
			{
				m_skipFirstUpdate = false;
				return;
			}
			particleStorage.ConsumeAndGet(dt * base.def.particleDecayRate);
			UpdateMeter();
		}

		public void UpdateMeter(object data = null)
		{
			meterController.SetPositionPercent(particleStorage.Particles / particleStorage.Capacity());
		}

		public void UpdateDecayStatusItem(bool hasPower)
		{
			if (!hasPower)
			{
				if (particleStorage.Particles > 0f)
				{
					if (statusHandle == Guid.Empty)
					{
						statusHandle = GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.LosingRadbolts);
					}
				}
				else if (statusHandle != Guid.Empty)
				{
					GetComponent<KSelectable>().RemoveStatusItem(statusHandle);
					statusHandle = Guid.Empty;
				}
			}
			else if (statusHandle != Guid.Empty)
			{
				GetComponent<KSelectable>().RemoveStatusItem(statusHandle);
				statusHandle = Guid.Empty;
			}
		}

		private LogicCircuitNetwork GetNetwork()
		{
			int portCell = GetComponent<LogicPorts>().GetPortCell(FIRE_PORT_ID);
			return Game.Instance.logicCircuitManager.GetNetworkForCell(portCell);
		}

		private void OnLogicValueChanged(object data)
		{
			LogicValueChanged logicValueChanged = (LogicValueChanged)data;
			if (logicValueChanged.portID == FIRE_PORT_ID)
			{
				isLogicActive = logicValueChanged.newValue > 0;
				hasLogicWire = GetNetwork() != null;
			}
		}

		public int SliderDecimalPlaces(int index)
		{
			return 0;
		}

		public float GetSliderMin(int index)
		{
			return base.def.minSlider;
		}

		public float GetSliderMax(int index)
		{
			return base.def.maxSlider;
		}

		public float GetSliderValue(int index)
		{
			return particleThreshold;
		}

		public void SetSliderValue(float value, int index)
		{
			particleThreshold = value;
		}

		public string GetSliderTooltipKey(int index)
		{
			return "STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TOOLTIP";
		}

		string ISliderControl.GetSliderTooltip()
		{
			return string.Format(Strings.Get("STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TOOLTIP"), particleThreshold);
		}
	}

	public static readonly HashedString FIRE_PORT_ID = "HEPBatteryFire";

	public State inoperational;

	public State operational;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = inoperational;
		inoperational.PlayAnim("off").TagTransition(GameTags.Operational, operational).Update(delegate(Instance smi, float dt)
		{
			smi.DoConsumeParticlesWhileDisabled(dt);
			smi.UpdateDecayStatusItem(hasPower: false);
		});
		operational.Enter("SetActive(true)", delegate(Instance smi)
		{
			smi.operational.SetActive(value: true);
		}).Exit("SetActive(false)", delegate(Instance smi)
		{
			smi.operational.SetActive(value: false);
		}).PlayAnim("on", KAnim.PlayMode.Loop)
			.TagTransition(GameTags.Operational, inoperational, on_remove: true)
			.Update(LauncherUpdate);
	}

	public void LauncherUpdate(Instance smi, float dt)
	{
		smi.UpdateDecayStatusItem(hasPower: true);
		smi.UpdateMeter();
		smi.operational.SetActive(smi.particleStorage.Particles > 0f);
		smi.launcherTimer += dt;
		if (!(smi.launcherTimer < smi.def.minLaunchInterval) && smi.AllowSpawnParticles && smi.particleStorage.Particles >= smi.particleThreshold)
		{
			smi.launcherTimer = 0f;
			Fire(smi);
		}
	}

	public void Fire(Instance smi)
	{
		int highEnergyParticleOutputCell = smi.GetComponent<Building>().GetHighEnergyParticleOutputCell();
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab("HighEnergyParticle"), Grid.CellToPosCCC(highEnergyParticleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
		gameObject.SetActive(value: true);
		if (gameObject != null)
		{
			HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
			component.payload = smi.particleStorage.ConsumeAndGet(smi.particleThreshold);
			component.SetDirection(smi.def.direction);
		}
	}
}
