using System;
using System.Collections.Generic;
using FMOD.Studio;

public class LadderBed : GameStateMachine<LadderBed, LadderBed.Instance, IStateMachineTarget, LadderBed.Def>
{
	public class Def : BaseDef
	{
		public CellOffset[] offsets;
	}

	public new class Instance : GameInstance
	{
		private List<HandleVector<int>.Handle> m_partitionEntires = new List<HandleVector<int>.Handle>();

		private int m_cell;

		[MyCmpGet]
		private Ownable m_ownable;

		[MyCmpGet]
		private Sleepable m_sleepable;

		[MyCmpGet]
		private AttachableBuilding m_attachable;

		private int numBelow;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			_ = GameScenePartitioner.Instance.objectLayers[40];
			m_cell = Grid.PosToCell(master.gameObject);
			CellOffset[] offsets = def.offsets;
			foreach (CellOffset offset in offsets)
			{
				int cell = Grid.OffsetCell(m_cell, offset);
				if (Grid.IsValidCell(m_cell) && Grid.IsValidCell(cell))
				{
					m_partitionEntires.Add(GameScenePartitioner.Instance.Add("LadderBed.Constructor", base.gameObject, cell, GameScenePartitioner.Instance.pickupablesChangedLayer, OnMoverChanged));
					OnMoverChanged(null);
				}
			}
			AttachableBuilding attachable = m_attachable;
			attachable.onAttachmentNetworkChanged = (Action<object>)Delegate.Combine(attachable.onAttachmentNetworkChanged, new Action<object>(OnAttachmentChanged));
			OnAttachmentChanged(null);
			Subscribe(-717201811, OnSleepDisturbedByMovement);
			master.GetComponent<KAnimControllerBase>().GetLayering().GetLink()
				.syncTint = false;
		}

		private void OnSleepDisturbedByMovement(object obj)
		{
			GetComponent<KAnimControllerBase>().Play("interrupt_light");
			EventInstance instance = SoundEvent.BeginOneShot(lightBedShakeSoundPath, base.smi.transform.GetPosition());
			instance.setParameterByName(LADDER_BED_COUNT_BELOW_PARAMETER, numBelow);
			SoundEvent.EndOneShot(instance);
		}

		private void OnAttachmentChanged(object data)
		{
			numBelow = AttachableBuilding.CountAttachedBelow(m_attachable);
		}

		private void OnMoverChanged(object obj)
		{
			Pickupable pickupable = obj as Pickupable;
			if (pickupable != null && pickupable.gameObject != null && pickupable.HasTag(GameTags.Minion) && pickupable.GetComponent<Navigator>().CurrentNavType == NavType.Ladder)
			{
				if (m_sleepable.worker == null)
				{
					GetComponent<KAnimControllerBase>().Play("interrupt_light_nodupe");
					EventInstance instance = SoundEvent.BeginOneShot(noDupeBedShakeSoundPath, base.smi.transform.GetPosition());
					instance.setParameterByName(LADDER_BED_COUNT_BELOW_PARAMETER, numBelow);
					SoundEvent.EndOneShot(instance);
				}
				else if (pickupable.gameObject != m_sleepable.worker.gameObject)
				{
					m_sleepable.worker.Trigger(-717201811);
				}
			}
		}

		protected override void OnCleanUp()
		{
			foreach (HandleVector<int>.Handle partitionEntire in m_partitionEntires)
			{
				HandleVector<int>.Handle handle = partitionEntire;
				GameScenePartitioner.Instance.Free(ref handle);
			}
			AttachableBuilding attachable = m_attachable;
			attachable.onAttachmentNetworkChanged = (Action<object>)Delegate.Remove(attachable.onAttachmentNetworkChanged, new Action<object>(OnAttachmentChanged));
			base.OnCleanUp();
		}
	}

	public static string lightBedShakeSoundPath = GlobalAssets.GetSound("LadderBed_LightShake");

	public static string noDupeBedShakeSoundPath = GlobalAssets.GetSound("LadderBed_Shake");

	public static string LADDER_BED_COUNT_BELOW_PARAMETER = "bed_count";

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
	}
}
