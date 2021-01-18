using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class FertilityMonitor : GameStateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>
{
	[Serializable]
	public class BreedingChance
	{
		public Tag egg;

		public float weight;
	}

	public class Def : BaseDef
	{
		public Tag eggPrefab;

		public List<BreedingChance> initialBreedingWeights;

		public float baseFertileCycles;

		public override void Configure(GameObject prefab)
		{
			prefab.AddOrGet<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Fertility.Id);
		}
	}

	public new class Instance : GameInstance
	{
		public AmountInstance fertility;

		private GameObject egg;

		[Serialize]
		public List<BreedingChance> breedingChances;

		public Effect fertileEffect;

		private static HashedString targetEggSymbol = "snapto_egg";

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			fertility = Db.Get().Amounts.Fertility.Lookup(base.gameObject);
			if (GenericGameSettings.instance.acceleratedLifecycle)
			{
				fertility.deltaAttribute.Add(new AttributeModifier(fertility.deltaAttribute.Id, 33.333332f, "Accelerated Lifecycle"));
			}
			float value = 100f / (def.baseFertileCycles * 600f);
			fertileEffect = new Effect("Fertile", CREATURES.MODIFIERS.BASE_FERTILITY.NAME, CREATURES.MODIFIERS.BASE_FERTILITY.TOOLTIP, 0f, show_in_ui: false, trigger_floating_text: false, is_bad: false);
			fertileEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, value, CREATURES.MODIFIERS.BASE_FERTILITY.NAME));
			InitializeBreedingChances();
		}

		[OnDeserialized]
		private void OnDeserialized()
		{
			if (breedingChances.Count == 0)
			{
				InitializeBreedingChances();
			}
		}

		private void InitializeBreedingChances()
		{
			breedingChances = new List<BreedingChance>();
			if (base.def.initialBreedingWeights == null)
			{
				return;
			}
			foreach (BreedingChance initialBreedingWeight in base.def.initialBreedingWeights)
			{
				breedingChances.Add(new BreedingChance
				{
					egg = initialBreedingWeight.egg,
					weight = initialBreedingWeight.weight
				});
				foreach (FertilityModifier item in Db.Get().FertilityModifiers.GetForTag(initialBreedingWeight.egg))
				{
					item.ApplyFunction(this, initialBreedingWeight.egg);
				}
			}
			NormalizeBreedingChances();
		}

		public void ShowEgg()
		{
			if (!(egg != null))
			{
				return;
			}
			bool symbolVisible;
			Vector3 vector = GetComponent<KBatchedAnimController>().GetSymbolTransform(targetEggSymbol, out symbolVisible).MultiplyPoint3x4(Vector3.zero);
			if (symbolVisible)
			{
				vector.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
				int num = Grid.PosToCell(vector);
				if (Grid.IsValidCell(num) && !Grid.Solid[num])
				{
					egg.transform.SetPosition(vector);
				}
			}
			egg.SetActive(value: true);
			Db.Get().Amounts.Wildness.Copy(egg, base.gameObject);
			egg = null;
		}

		public void LayEgg()
		{
			fertility.value = 0f;
			Vector3 position = base.smi.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
			float num = UnityEngine.Random.value;
			Tag invalid = Tag.Invalid;
			foreach (BreedingChance breedingChance in breedingChances)
			{
				num -= breedingChance.weight;
				if (num <= 0f)
				{
					invalid = breedingChance.egg;
					break;
				}
			}
			if (GenericGameSettings.instance.acceleratedLifecycle)
			{
				float num2 = 0f;
				foreach (BreedingChance breedingChance2 in breedingChances)
				{
					if (breedingChance2.weight > num2)
					{
						num2 = breedingChance2.weight;
						invalid = breedingChance2.egg;
					}
				}
			}
			Debug.Assert(invalid != Tag.Invalid, "Didn't pick an egg to lay. Weights weren't normalized?");
			GameObject prefab = Assets.GetPrefab(invalid);
			GameObject gameObject = (egg = Util.KInstantiate(prefab, position));
			SymbolOverrideController component = GetComponent<SymbolOverrideController>();
			string str = "egg01";
			CreatureBrain component2 = Assets.GetPrefab(prefab.GetDef<IncubationMonitor.Def>().spawnedCreature).GetComponent<CreatureBrain>();
			if (!string.IsNullOrEmpty(component2.symbolPrefix))
			{
				str = component2.symbolPrefix + "egg01";
			}
			KAnim.Build.Symbol symbol = egg.GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbol(str);
			if (symbol != null)
			{
				component.AddSymbolOverride(targetEggSymbol, symbol);
			}
			Trigger(1193600993, egg);
		}

		public bool IsReadyToLayEgg()
		{
			return base.smi.fertility.value >= base.smi.fertility.GetMax();
		}

		public void AddBreedingChance(Tag type, float addedPercentChance)
		{
			foreach (BreedingChance breedingChance in breedingChances)
			{
				if (breedingChance.egg == type)
				{
					float num = Mathf.Min(1f - breedingChance.weight, Mathf.Max(0f - breedingChance.weight, addedPercentChance));
					breedingChance.weight += num;
				}
			}
			NormalizeBreedingChances();
			base.master.Trigger(1059811075, breedingChances);
		}

		private void NormalizeBreedingChances()
		{
			float num = 0f;
			foreach (BreedingChance breedingChance in breedingChances)
			{
				num += breedingChance.weight;
			}
			foreach (BreedingChance breedingChance2 in breedingChances)
			{
				breedingChance2.weight /= num;
			}
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			if (egg != null)
			{
				UnityEngine.Object.Destroy(egg);
				egg = null;
			}
		}
	}

	private State fertile;

	private State infertile;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = fertile;
		base.serializable = true;
		root.DefaultState(fertile);
		fertile.ToggleBehaviour(GameTags.Creatures.Fertile, (Instance smi) => smi.IsReadyToLayEgg()).ToggleEffect((Instance smi) => smi.fertileEffect).Transition(infertile, GameStateMachine<FertilityMonitor, Instance, IStateMachineTarget, Def>.Not(IsFertile), UpdateRate.SIM_1000ms);
		infertile.Transition(fertile, IsFertile, UpdateRate.SIM_1000ms);
	}

	public static bool IsFertile(Instance smi)
	{
		if (smi.HasTag(GameTags.Creatures.Confined))
		{
			return false;
		}
		if (smi.HasTag(GameTags.Creatures.Expecting))
		{
			return false;
		}
		return true;
	}
}
