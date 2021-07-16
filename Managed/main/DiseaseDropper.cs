using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class DiseaseDropper : GameStateMachine<DiseaseDropper, DiseaseDropper.Instance, IStateMachineTarget, DiseaseDropper.Def>
{
	public class Def : BaseDef, IGameObjectEffectDescriptor
	{
		public byte diseaseIdx = byte.MaxValue;

		public int singleEmitQuantity;

		public int averageEmitPerSecond;

		public float emitFrequency = 1f;

		public List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> list = new List<Descriptor>();
			if (singleEmitQuantity > 0)
			{
				list.Add(new Descriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.DISEASE_DROPPER_BURST.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(diseaseIdx)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(singleEmitQuantity)), UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.DISEASE_DROPPER_BURST.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(diseaseIdx)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(singleEmitQuantity))));
			}
			if (averageEmitPerSecond > 0)
			{
				list.Add(new Descriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.DISEASE_DROPPER_CONSTANT.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(diseaseIdx)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(averageEmitPerSecond, GameUtil.TimeSlice.PerSecond)), UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.DISEASE_DROPPER_CONSTANT.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(diseaseIdx)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(averageEmitPerSecond, GameUtil.TimeSlice.PerSecond))));
			}
			return list;
		}
	}

	public new class Instance : GameInstance
	{
		private float timeSinceLastDrop;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public bool ShouldDropDisease()
		{
			return true;
		}

		public void DropSingleEmit()
		{
			DropDisease(base.def.diseaseIdx, base.def.singleEmitQuantity);
		}

		public void DropPeriodic(float dt)
		{
			timeSinceLastDrop += dt;
			if (base.def.averageEmitPerSecond > 0 && base.def.emitFrequency > 0f)
			{
				while (timeSinceLastDrop > base.def.emitFrequency)
				{
					DropDisease(base.def.diseaseIdx, (int)((float)base.def.averageEmitPerSecond * base.def.emitFrequency));
					timeSinceLastDrop -= base.def.emitFrequency;
				}
			}
		}

		public void DropDisease(byte disease_idx, int disease_count)
		{
			if (disease_count > 0 && disease_idx != byte.MaxValue)
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				if (Grid.IsValidCell(num))
				{
					SimMessages.ModifyDiseaseOnCell(num, disease_idx, disease_count);
				}
			}
		}

		public bool IsValidDropCell()
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			if (!Grid.IsValidCell(num))
			{
				return false;
			}
			if (!Grid.IsGas(num))
			{
				return false;
			}
			if (Grid.Mass[num] > 1f)
			{
				return false;
			}
			return true;
		}
	}

	public State working;

	public State stopped;

	public Signal cellChangedSignal;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = stopped;
		root.EventHandler(GameHashes.BurstEmitDisease, delegate(Instance smi)
		{
			smi.DropSingleEmit();
		});
		working.TagTransition(GameTags.PreventEmittingDisease, stopped).Update(delegate(Instance smi, float dt)
		{
			smi.DropPeriodic(dt);
		});
		stopped.TagTransition(GameTags.PreventEmittingDisease, working, on_remove: true);
	}
}
