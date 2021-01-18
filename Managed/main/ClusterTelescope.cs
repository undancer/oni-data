using System;
using System.Collections.Generic;
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
	}

	public class ReadyStates : State
	{
		public State no_visibility;

		public State ready_to_work;
	}

	public new class Instance : GameInstance
	{
		private float m_percentClear = 0f;

		[Serialize]
		private bool m_hasAnalyzeTarget;

		[Serialize]
		private AxialI m_analyzeTarget;

		[MyCmpAdd]
		private ClusterTelescopeWorkable m_workable;

		public float PercentClear => m_percentClear;

		public Instance(IStateMachineTarget smi, Def def)
			: base(smi, def)
		{
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
			return new WorkChore<ClusterTelescopeWorkable>(Db.Get().ChoreTypes.Research, m_workable);
		}

		public AxialI GetAnalyzeTarget()
		{
			Debug.Assert(m_hasAnalyzeTarget, "GetAnalyzeTarget called but this telescope has no target assigned.");
			return m_analyzeTarget;
		}

		public bool HasSkyVisibility()
		{
			Building component = GetComponent<Building>();
			Extents extents = component.GetExtents();
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
			bool flag = num6 < num3;
			m_percentClear = (float)num6 / (float)num3;
			return m_percentClear > 0f;
		}
	}

	public class ClusterTelescopeWorkable : Workable
	{
		[MySmiReq]
		private Instance m_telescope;

		private ClusterFogOfWarManager.Instance m_fowManager = null;

		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
			attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE;
			skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
			skillExperienceMultiplier = SKILLS.ALL_DAY_EXPERIENCE;
			overrideAnims = new KAnimFile[1]
			{
				Assets.GetAnim("anim_interacts_telescope_low_kanim")
			};
			requiredSkillPerk = Db.Get().SkillPerks.CanUseClusterTelescope.Id;
			workLayer = Grid.SceneLayer.BuildingFront;
		}

		protected override void OnSpawn()
		{
			base.OnSpawn();
			OnWorkableEventCB = (Action<WorkableEvent>)Delegate.Combine(OnWorkableEventCB, new Action<WorkableEvent>(OnWorkableEvent));
			m_fowManager = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
			SetWorkTime(float.PositiveInfinity);
		}

		private void OnWorkableEvent(WorkableEvent ev)
		{
			Worker worker = base.worker;
			if (worker == null)
			{
				return;
			}
			KPrefabID component = worker.GetComponent<KPrefabID>();
			switch (ev)
			{
			case WorkableEvent.WorkStarted:
				ShowProgressBar(show: true);
				progressBar.SetUpdateFunc(delegate
				{
					AxialI analyzeTarget = m_telescope.GetAnalyzeTarget();
					return m_fowManager.GetRevealCompleteFraction(analyzeTarget);
				});
				break;
			case WorkableEvent.WorkStopped:
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
			float num = ROCKETRY.CLUSTER_FOW.POINTS_TO_REVEAL / ROCKETRY.CLUSTER_FOW.DEFAULT_CYCLES_PER_REVEAL;
			float num2 = num / 600f;
			float points = dt * num2;
			m_fowManager.EarnRevealPointsForLocation(analyzeTarget, points);
			return base.OnWorkTick(worker, dt);
		}
	}

	private static StatusItem noVisibilityStatusItem = new StatusItem("SPACE_VISIBILITY_NONE", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: true, 129022, GetStatusItemString);

	public State all_work_complete;

	public ReadyStates ready;

	private static string GetStatusItemString(string src_str, object data)
	{
		Instance instance = (Instance)data;
		string text = src_str;
		text = text.Replace("{VISIBILITY}", GameUtil.GetFormattedPercent(instance.PercentClear * 100f));
		return text.Replace("{RADIUS}", instance.def.clearScanCellRadius.ToString());
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
