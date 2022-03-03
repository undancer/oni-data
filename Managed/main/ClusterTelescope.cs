using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

public class ClusterTelescope : GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>
{
	public class Def : BaseDef
	{
		public int clearScanCellRadius = 15;

		public int analyzeClusterRadius = 3;

		public KAnimFile[] workableOverrideAnims;

		public bool providesOxygen;
	}

	public class ReadyStates : State
	{
		public State no_visibility;

		public State ready_to_work;
	}

	public new class Instance : GameInstance
	{
		private float m_percentClear;

		[Serialize]
		private bool m_hasAnalyzeTarget;

		[Serialize]
		private AxialI m_analyzeTarget;

		[MyCmpAdd]
		private ClusterTelescopeWorkable m_workable;

		public KAnimFile[] workableOverrideAnims;

		public bool providesOxygen;

		public float PercentClear => m_percentClear;

		public Instance(IStateMachineTarget smi, Def def)
			: base(smi, def)
		{
			workableOverrideAnims = def.workableOverrideAnims;
			providesOxygen = def.providesOxygen;
		}

		public bool CheckHasAnalyzeTarget()
		{
			ClusterFogOfWarManager.Instance sMI = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
			if (m_hasAnalyzeTarget && !sMI.IsLocationRevealed(m_analyzeTarget))
			{
				return true;
			}
			AxialI myWorldLocation = this.GetMyWorldLocation();
			m_hasAnalyzeTarget = sMI.GetUnrevealedLocationWithinRadius(myWorldLocation, base.def.analyzeClusterRadius, out m_analyzeTarget);
			return m_hasAnalyzeTarget;
		}

		public Chore CreateChore()
		{
			WorkChore<ClusterTelescopeWorkable> workChore = new WorkChore<ClusterTelescopeWorkable>(Db.Get().ChoreTypes.Research, m_workable);
			if (providesOxygen)
			{
				workChore.AddPrecondition(Telescope.ContainsOxygen);
			}
			return workChore;
		}

		public AxialI GetAnalyzeTarget()
		{
			Debug.Assert(m_hasAnalyzeTarget, "GetAnalyzeTarget called but this telescope has no target assigned.");
			return m_analyzeTarget;
		}

		public bool HasSkyVisibility()
		{
			Extents extents = GetComponent<Building>().GetExtents();
			int num = Mathf.Max(0, extents.x - base.def.clearScanCellRadius);
			int num2 = Mathf.Min(extents.x + base.def.clearScanCellRadius);
			int y = extents.y + extents.height - 3;
			int num3 = num2 - num + 1;
			int num4 = Grid.XYToCell(num, y);
			int num5 = Grid.XYToCell(num2, y);
			int num6 = 0;
			for (int i = num4; i <= num5; i++)
			{
				if (Grid.ExposedToSunlight[i] >= 253)
				{
					num6++;
				}
			}
			m_percentClear = (float)num6 / (float)num3;
			return m_percentClear > 0f;
		}
	}

	public class ClusterTelescopeWorkable : Workable, OxygenBreather.IGasProvider
	{
		[MySmiReq]
		private Instance m_telescope;

		private ClusterFogOfWarManager.Instance m_fowManager;

		private GameObject telescopeTargetMarker;

		private AxialI currentTarget;

		private OxygenBreather.IGasProvider workerGasProvider;

		[MyCmpGet]
		private Storage storage;

		private AttributeModifier radiationShielding;

		private float checkMarkerTimer;

		private float checkMarkerFrequency = 1f;

		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
			attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE;
			skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
			skillExperienceMultiplier = SKILLS.ALL_DAY_EXPERIENCE;
			requiredSkillPerk = Db.Get().SkillPerks.CanUseClusterTelescope.Id;
			workLayer = Grid.SceneLayer.BuildingUse;
			radiationShielding = new AttributeModifier(Db.Get().Attributes.RadiationResistance.Id, FIXEDTRAITS.COSMICRADIATION.TELESCOPE_RADIATION_SHIELDING, STRINGS.BUILDINGS.PREFABS.CLUSTERTELESCOPEENCLOSED.NAME);
		}

		protected override void OnCleanUp()
		{
			if (telescopeTargetMarker != null)
			{
				Util.KDestroyGameObject(telescopeTargetMarker);
			}
			base.OnCleanUp();
		}

		protected override void OnSpawn()
		{
			base.OnSpawn();
			OnWorkableEventCB = (Action<WorkableEvent>)Delegate.Combine(OnWorkableEventCB, new Action<WorkableEvent>(OnWorkableEvent));
			m_fowManager = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
			SetWorkTime(float.PositiveInfinity);
			overrideAnims = m_telescope.workableOverrideAnims;
		}

		private void OnWorkableEvent(WorkableEvent ev)
		{
			Worker worker = base.worker;
			if (worker == null)
			{
				return;
			}
			KPrefabID component = worker.GetComponent<KPrefabID>();
			OxygenBreather component2 = worker.GetComponent<OxygenBreather>();
			Attributes attributes = worker.GetAttributes();
			switch (ev)
			{
			case WorkableEvent.WorkStarted:
				ShowProgressBar(show: true);
				progressBar.SetUpdateFunc(() => m_fowManager.GetRevealCompleteFraction(currentTarget));
				currentTarget = m_telescope.GetAnalyzeTarget();
				if (!ClusterGrid.Instance.GetEntityOfLayerAtCell(currentTarget, EntityLayer.Telescope))
				{
					telescopeTargetMarker = GameUtil.KInstantiate(Assets.GetPrefab("TelescopeTarget"), Grid.SceneLayer.Background);
					telescopeTargetMarker.SetActive(value: true);
					telescopeTargetMarker.GetComponent<TelescopeTarget>().Init(currentTarget);
				}
				if (m_telescope.providesOxygen)
				{
					attributes.Add(radiationShielding);
					workerGasProvider = component2.GetGasProvider();
					component2.SetGasProvider(this);
					component2.GetComponent<CreatureSimTemperatureTransfer>().enabled = false;
					component.AddTag(GameTags.Shaded);
				}
				GetComponent<Operational>().SetActive(value: true);
				checkMarkerFrequency = UnityEngine.Random.Range(2f, 5f);
				break;
			case WorkableEvent.WorkStopped:
				if (m_telescope.providesOxygen)
				{
					attributes.Remove(radiationShielding);
					component2.SetGasProvider(workerGasProvider);
					component2.GetComponent<CreatureSimTemperatureTransfer>().enabled = true;
					component.RemoveTag(GameTags.Shaded);
				}
				GetComponent<Operational>().SetActive(value: false);
				if (telescopeTargetMarker != null)
				{
					Util.KDestroyGameObject(telescopeTargetMarker);
				}
				ShowProgressBar(show: false);
				break;
			}
		}

		public override List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> descriptors = base.GetDescriptors(go);
			Element element = ElementLoader.FindElementByHash(SimHashes.Oxygen);
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(element.tag.ProperName(), string.Format(STRINGS.BUILDINGS.PREFABS.TELESCOPE.REQUIREMENT_TOOLTIP, element.tag.ProperName()), Descriptor.DescriptorType.Requirement);
			descriptors.Add(item);
			return descriptors;
		}

		protected override bool OnWorkTick(Worker worker, float dt)
		{
			AxialI analyzeTarget = m_telescope.GetAnalyzeTarget();
			bool flag = false;
			if (analyzeTarget != currentTarget)
			{
				if ((bool)telescopeTargetMarker)
				{
					telescopeTargetMarker.GetComponent<TelescopeTarget>().Init(analyzeTarget);
				}
				currentTarget = analyzeTarget;
				flag = true;
			}
			if (!flag && checkMarkerTimer > checkMarkerFrequency)
			{
				checkMarkerTimer = 0f;
				if (!telescopeTargetMarker && !ClusterGrid.Instance.GetEntityOfLayerAtCell(currentTarget, EntityLayer.Telescope))
				{
					telescopeTargetMarker = GameUtil.KInstantiate(Assets.GetPrefab("TelescopeTarget"), Grid.SceneLayer.Background);
					telescopeTargetMarker.SetActive(value: true);
					telescopeTargetMarker.GetComponent<TelescopeTarget>().Init(currentTarget);
				}
			}
			checkMarkerTimer += dt;
			float num = ROCKETRY.CLUSTER_FOW.POINTS_TO_REVEAL / ROCKETRY.CLUSTER_FOW.DEFAULT_CYCLES_PER_REVEAL / 600f;
			float points = dt * num;
			m_fowManager.EarnRevealPointsForLocation(currentTarget, points);
			return base.OnWorkTick(worker, dt);
		}

		public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
		{
		}

		public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
		{
		}

		public bool ShouldEmitCO2()
		{
			return false;
		}

		public bool ShouldStoreCO2()
		{
			return false;
		}

		public bool ConsumeGas(OxygenBreather oxygen_breather, float amount)
		{
			if (storage.items.Count <= 0)
			{
				return false;
			}
			GameObject gameObject = storage.items[0];
			if (gameObject == null)
			{
				return false;
			}
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			bool result = component.Mass >= amount;
			component.Mass = Mathf.Max(0f, component.Mass - amount);
			return result;
		}
	}

	private static StatusItem noVisibilityStatusItem = new StatusItem("SPACE_VISIBILITY_NONE", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: true, 129022, GetStatusItemString);

	public State all_work_complete;

	public ReadyStates ready;

	private static string GetStatusItemString(string src_str, object data)
	{
		Instance instance = (Instance)data;
		return src_str.Replace("{VISIBILITY}", GameUtil.GetFormattedPercent(instance.PercentClear * 100f)).Replace("{RADIUS}", instance.def.clearScanCellRadius.ToString());
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = ready.no_visibility;
		ready.EventTransition(GameHashes.ClusterFogOfWarRevealed, (Instance smi) => Game.Instance, all_work_complete, (Instance smi) => !smi.CheckHasAnalyzeTarget());
		ready.no_visibility.UpdateTransition(ready.ready_to_work, (Instance smi, float dt) => smi.HasSkyVisibility()).ToggleStatusItem(noVisibilityStatusItem);
		ready.ready_to_work.UpdateTransition(ready.no_visibility, (Instance smi, float dt) => !smi.HasSkyVisibility()).ToggleChore((Instance smi) => smi.CreateChore(), ready.no_visibility);
		all_work_complete.ToggleMainStatusItem(Db.Get().BuildingStatusItems.ClusterTelescopeAllWorkComplete).EventTransition(GameHashes.ClusterLocationChanged, (Instance smi) => Game.Instance, ready.no_visibility, (Instance smi) => smi.CheckHasAnalyzeTarget());
	}
}
