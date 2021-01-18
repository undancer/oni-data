using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class IrrigationMonitor : GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>
{
	public class Def : BaseDef, IGameObjectEffectDescriptor
	{
		public Tag wrongIrrigationTestTag;

		public PlantElementAbsorber.ConsumeInfo[] consumedElements;

		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			if (consumedElements.Length != 0)
			{
				List<Descriptor> list = new List<Descriptor>();
				PlantElementAbsorber.ConsumeInfo[] array = consumedElements;
				for (int i = 0; i < array.Length; i++)
				{
					PlantElementAbsorber.ConsumeInfo consumeInfo = array[i];
					list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.IDEAL_FERTILIZER, consumeInfo.tag.ProperName(), GameUtil.GetFormattedMass(0f - consumeInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.IDEAL_FERTILIZER, consumeInfo.tag.ProperName(), GameUtil.GetFormattedMass(consumeInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle)), Descriptor.DescriptorType.Requirement));
				}
				return list;
			}
			return null;
		}
	}

	public class VariableIrrigationStates : State
	{
		public State normal;

		public State wrongLiquid;
	}

	public class Irrigated : State
	{
		public VariableIrrigationStates absorbing;
	}

	public class ReplantedStates : State
	{
		public Irrigated irrigated;

		public VariableIrrigationStates starved;
	}

	public new class Instance : GameInstance, IWiltCause
	{
		public AttributeModifier consumptionRate;

		public AttributeModifier absorptionRate;

		protected AmountInstance irrigation;

		private float total_available_mass;

		private Storage storage;

		private HandleVector<int>.Handle absorberHandle = HandleVector<int>.InvalidHandle;

		public float total_fertilizer_available => total_available_mass;

		public WiltCondition.Condition[] Conditions => new WiltCondition.Condition[1]
		{
			WiltCondition.Condition.Irrigation
		};

		public string WiltStateString
		{
			get
			{
				string result = "";
				if (base.smi.IsInsideState(base.smi.sm.replanted.irrigated.absorbing.wrongLiquid))
				{
					result = GetIncorrectLiquidStatusItem().resolveStringCallback(CREATURES.STATUSITEMS.WRONGIRRIGATION.NAME, this);
				}
				else if (base.smi.IsInsideState(base.smi.sm.replanted.starved))
				{
					result = GetStarvedStatusItem().resolveStringCallback(CREATURES.STATUSITEMS.NEEDSIRRIGATION.NAME, this);
				}
				else if (base.smi.IsInsideState(base.smi.sm.replanted.starved.wrongLiquid))
				{
					result = GetIncorrectLiquidStatusItemMajor().resolveStringCallback(CREATURES.STATUSITEMS.WRONGIRRIGATIONMAJOR.NAME, this);
				}
				return result;
			}
		}

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			AddAmounts(base.gameObject);
			MakeModifiers();
			master.Subscribe(1309017699, SetStorage);
		}

		public virtual StatusItem GetStarvedStatusItem()
		{
			return Db.Get().CreatureStatusItems.NeedsIrrigation;
		}

		public virtual StatusItem GetIncorrectLiquidStatusItem()
		{
			return Db.Get().CreatureStatusItems.WrongIrrigation;
		}

		public virtual StatusItem GetIncorrectLiquidStatusItemMajor()
		{
			return Db.Get().CreatureStatusItems.WrongIrrigationMajor;
		}

		protected virtual void AddAmounts(GameObject gameObject)
		{
			Amounts amounts = gameObject.GetAmounts();
			irrigation = amounts.Add(new AmountInstance(Db.Get().Amounts.Irrigation, gameObject));
		}

		protected virtual void MakeModifiers()
		{
			consumptionRate = new AttributeModifier(Db.Get().Amounts.Irrigation.deltaAttribute.Id, -355f / (678f * (float)Math.PI), CREATURES.STATS.IRRIGATION.CONSUME_MODIFIER);
			absorptionRate = new AttributeModifier(Db.Get().Amounts.Irrigation.deltaAttribute.Id, 1.6666666f, CREATURES.STATS.IRRIGATION.ABSORBING_MODIFIER);
		}

		public static void DumpIncorrectFertilizers(Storage storage, GameObject go)
		{
			if (!(storage == null) && !(go == null))
			{
				Instance sMI = go.GetSMI<Instance>();
				PlantElementAbsorber.ConsumeInfo[] consumed_infos = null;
				if (sMI != null)
				{
					consumed_infos = sMI.def.consumedElements;
				}
				DumpIncorrectFertilizers(storage, consumed_infos, validate_solids: false);
				FertilizationMonitor.Instance sMI2 = go.GetSMI<FertilizationMonitor.Instance>();
				PlantElementAbsorber.ConsumeInfo[] consumed_infos2 = null;
				if (sMI2 != null)
				{
					consumed_infos2 = sMI2.def.consumedElements;
				}
				DumpIncorrectFertilizers(storage, consumed_infos2, validate_solids: true);
			}
		}

		private static void DumpIncorrectFertilizers(Storage storage, PlantElementAbsorber.ConsumeInfo[] consumed_infos, bool validate_solids)
		{
			if (storage == null)
			{
				return;
			}
			for (int num = storage.items.Count - 1; num >= 0; num--)
			{
				GameObject gameObject = storage.items[num];
				if (gameObject == null)
				{
					continue;
				}
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (component == null || gameObject.GetComponent<ElementChunk>() == null)
				{
					continue;
				}
				if (validate_solids)
				{
					if (!component.Element.IsSolid)
					{
						continue;
					}
				}
				else if (!component.Element.IsLiquid)
				{
					continue;
				}
				bool flag = false;
				KPrefabID component2 = component.GetComponent<KPrefabID>();
				if (consumed_infos != null)
				{
					for (int i = 0; i < consumed_infos.Length; i++)
					{
						PlantElementAbsorber.ConsumeInfo consumeInfo = consumed_infos[i];
						if (component2.HasTag(consumeInfo.tag))
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					storage.Drop(gameObject);
				}
			}
		}

		public void SetStorage(object obj)
		{
			storage = (Storage)obj;
			base.sm.resourceStorage.Set(storage, base.smi);
			DumpIncorrectFertilizers(storage, base.smi.gameObject);
			ManualDeliveryKG[] components = base.smi.gameObject.GetComponents<ManualDeliveryKG>();
			foreach (ManualDeliveryKG manualDeliveryKG in components)
			{
				bool flag = false;
				PlantElementAbsorber.ConsumeInfo[] consumedElements = base.def.consumedElements;
				for (int j = 0; j < consumedElements.Length; j++)
				{
					PlantElementAbsorber.ConsumeInfo consumeInfo = consumedElements[j];
					if (manualDeliveryKG.requestedItemTag == consumeInfo.tag)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					manualDeliveryKG.SetStorage(storage);
					manualDeliveryKG.enabled = !storage.gameObject.GetComponent<PlantablePlot>().has_liquid_pipe_input;
				}
			}
		}

		public virtual bool AcceptsLiquid()
		{
			PlantablePlot component = base.sm.resourceStorage.Get(this).GetComponent<PlantablePlot>();
			if (component != null)
			{
				return component.AcceptsIrrigation;
			}
			return false;
		}

		public bool Starved()
		{
			return irrigation.value == 0f;
		}

		public void UpdateIrrigation(float dt)
		{
			if (base.def.consumedElements == null)
			{
				return;
			}
			Storage storage = base.sm.resourceStorage.Get<Storage>(base.smi);
			bool flag = true;
			bool value = false;
			bool flag2 = true;
			if (storage != null)
			{
				List<GameObject> items = storage.items;
				for (int i = 0; i < base.def.consumedElements.Length; i++)
				{
					float num = 0f;
					PlantElementAbsorber.ConsumeInfo consumeInfo = base.def.consumedElements[i];
					for (int j = 0; j < items.Count; j++)
					{
						GameObject gameObject = items[j];
						if (gameObject.HasTag(consumeInfo.tag))
						{
							num += gameObject.GetComponent<PrimaryElement>().Mass;
						}
						else if (gameObject.HasTag(base.def.wrongIrrigationTestTag))
						{
							value = true;
						}
					}
					total_available_mass = num;
					if (num < consumeInfo.massConsumptionRate * dt)
					{
						flag = false;
						break;
					}
					if (num < consumeInfo.massConsumptionRate * (dt * 30f))
					{
						flag2 = false;
						break;
					}
				}
			}
			else
			{
				flag = false;
				flag2 = false;
				value = false;
			}
			base.sm.hasCorrectLiquid.Set(flag, base.smi);
			base.sm.hasIncorrectLiquid.Set(value, base.smi);
			base.sm.enoughCorrectLiquidToRecover.Set(flag2 && flag, base.smi);
		}

		public void UpdateAbsorbing(bool allow)
		{
			bool flag = allow && !base.smi.gameObject.HasTag(GameTags.Wilting);
			if (flag == absorberHandle.IsValid())
			{
				return;
			}
			if (flag)
			{
				if (base.def.consumedElements != null && base.def.consumedElements.Length != 0)
				{
					absorberHandle = Game.Instance.plantElementAbsorbers.Add(storage, base.def.consumedElements);
				}
			}
			else
			{
				absorberHandle = Game.Instance.plantElementAbsorbers.Remove(absorberHandle);
			}
		}
	}

	public TargetParameter resourceStorage;

	public BoolParameter hasCorrectLiquid;

	public BoolParameter hasIncorrectLiquid;

	public BoolParameter enoughCorrectLiquidToRecover;

	public GameHashes ResourceRecievedEvent = GameHashes.LiquidResourceRecieved;

	public GameHashes ResourceDepletedEvent = GameHashes.LiquidResourceEmpty;

	public State wild;

	public State unfertilizable;

	public ReplantedStates replanted;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = wild;
		base.serializable = SerializeType.Never;
		wild.ParamTransition(resourceStorage, unfertilizable, (Instance smi, GameObject p) => p != null);
		unfertilizable.Enter(delegate(Instance smi)
		{
			if (smi.AcceptsLiquid())
			{
				smi.GoTo(replanted.irrigated);
			}
		});
		replanted.Enter(delegate(Instance smi)
		{
			ManualDeliveryKG[] components = smi.gameObject.GetComponents<ManualDeliveryKG>();
			foreach (ManualDeliveryKG manualDeliveryKG in components)
			{
				manualDeliveryKG.Pause(pause: false, "replanted");
			}
			smi.UpdateIrrigation(71f / (678f * (float)Math.PI));
		}).Target(resourceStorage).EventHandler(GameHashes.OnStorageChange, delegate(Instance smi)
		{
			smi.UpdateIrrigation(0.2f);
		})
			.Target(masterTarget);
		replanted.irrigated.DefaultState(replanted.irrigated.absorbing).TriggerOnEnter(ResourceRecievedEvent);
		replanted.irrigated.absorbing.DefaultState(replanted.irrigated.absorbing.normal).ParamTransition(hasCorrectLiquid, replanted.starved, GameStateMachine<IrrigationMonitor, Instance, IStateMachineTarget, Def>.IsFalse).ToggleAttributeModifier("Absorbing", (Instance smi) => smi.absorptionRate)
			.Enter(delegate(Instance smi)
			{
				smi.UpdateAbsorbing(allow: true);
			})
			.EventHandler(GameHashes.TagsChanged, delegate(Instance smi)
			{
				smi.UpdateAbsorbing(allow: true);
			})
			.Exit(delegate(Instance smi)
			{
				smi.UpdateAbsorbing(allow: false);
			});
		replanted.irrigated.absorbing.normal.ParamTransition(hasIncorrectLiquid, replanted.irrigated.absorbing.wrongLiquid, GameStateMachine<IrrigationMonitor, Instance, IStateMachineTarget, Def>.IsTrue);
		replanted.irrigated.absorbing.wrongLiquid.ParamTransition(hasIncorrectLiquid, replanted.irrigated.absorbing.normal, GameStateMachine<IrrigationMonitor, Instance, IStateMachineTarget, Def>.IsFalse);
		replanted.starved.DefaultState(replanted.starved.normal).TriggerOnEnter(ResourceDepletedEvent).ParamTransition(enoughCorrectLiquidToRecover, replanted.irrigated.absorbing, (Instance smi, bool p) => p && hasCorrectLiquid.Get(smi))
			.ParamTransition(hasCorrectLiquid, replanted.irrigated.absorbing, (Instance smi, bool p) => p && enoughCorrectLiquidToRecover.Get(smi));
		replanted.starved.normal.ParamTransition(hasIncorrectLiquid, replanted.starved.wrongLiquid, GameStateMachine<IrrigationMonitor, Instance, IStateMachineTarget, Def>.IsTrue);
		replanted.starved.wrongLiquid.ParamTransition(hasIncorrectLiquid, replanted.starved.normal, GameStateMachine<IrrigationMonitor, Instance, IStateMachineTarget, Def>.IsFalse);
	}
}
