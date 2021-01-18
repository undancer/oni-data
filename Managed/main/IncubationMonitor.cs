using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class IncubationMonitor : GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>
{
	public class Def : BaseDef
	{
		public float baseIncubationRate;

		public Tag spawnedCreature;

		public override void Configure(GameObject prefab)
		{
			List<string> initialAmounts = prefab.GetComponent<Modifiers>().initialAmounts;
			initialAmounts.Add(Db.Get().Amounts.Wildness.Id);
			initialAmounts.Add(Db.Get().Amounts.Incubation.Id);
			initialAmounts.Add(Db.Get().Amounts.Viability.Id);
		}
	}

	public new class Instance : GameInstance
	{
		public AmountInstance incubation;

		public AmountInstance wildness;

		public AmountInstance viability;

		public EggIncubator incubator;

		public Effect incubatingEffect;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			incubation = Db.Get().Amounts.Incubation.Lookup(base.gameObject);
			Action<object> handler = OnStore;
			master.Subscribe(856640610, handler);
			master.Subscribe(1309017699, handler);
			Action<object> handler2 = OnOperationalChanged;
			master.Subscribe(1628751838, handler2);
			master.Subscribe(960378201, handler2);
			wildness = Db.Get().Amounts.Wildness.Lookup(base.gameObject);
			wildness.value = wildness.GetMax();
			viability = Db.Get().Amounts.Viability.Lookup(base.gameObject);
			viability.value = viability.GetMax();
			float value = def.baseIncubationRate;
			if (GenericGameSettings.instance.acceleratedLifecycle)
			{
				value = 33.333332f;
			}
			AttributeModifier modifier = new AttributeModifier(Db.Get().Amounts.Incubation.deltaAttribute.Id, value, CREATURES.MODIFIERS.BASE_INCUBATION_RATE.NAME);
			incubatingEffect = new Effect("Incubating", CREATURES.MODIFIERS.INCUBATING.NAME, CREATURES.MODIFIERS.INCUBATING.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: false);
			incubatingEffect.Add(modifier);
		}

		public Storage GetStorage()
		{
			return (base.transform.parent != null) ? base.transform.parent.GetComponent<Storage>() : null;
		}

		public void OnStore(object data)
		{
			Storage storage = data as Storage;
			bool stored = (bool)storage || (data != null && (bool)data);
			EggIncubator eggIncubator = (storage ? storage.GetComponent<EggIncubator>() : null);
			UpdateIncubationState(stored, eggIncubator);
		}

		public void OnOperationalChanged(object data = null)
		{
			bool stored = base.gameObject.HasTag(GameTags.Stored);
			Storage storage = GetStorage();
			EggIncubator eggIncubator = (storage ? storage.GetComponent<EggIncubator>() : null);
			UpdateIncubationState(stored, eggIncubator);
		}

		private void UpdateIncubationState(bool stored, EggIncubator incubator)
		{
			this.incubator = incubator;
			base.smi.sm.inIncubator.Set(incubator != null, base.smi);
			bool value = stored && !incubator;
			base.smi.sm.isSuppressed.Set(value, base.smi);
			Operational operational = (incubator ? incubator.GetComponent<Operational>() : null);
			bool value2 = (bool)incubator && (operational == null || operational.IsOperational);
			base.smi.sm.incubatorIsActive.Set(value2, base.smi);
		}

		public void ApplySongBuff()
		{
			GetComponent<Effects>().Add("EggSong", should_save: true);
		}

		public bool HasSongBuff()
		{
			return GetComponent<Effects>().HasEffect("EggSong");
		}
	}

	public BoolParameter incubatorIsActive;

	public BoolParameter inIncubator;

	public BoolParameter isSuppressed;

	public State incubating;

	public State entombed;

	public State suppressed;

	public State hatching_pre;

	public State hatching_pst;

	public State not_viable;

	private Effect suppressedEffect;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = incubating;
		root.Enter(delegate(Instance smi)
		{
			smi.OnOperationalChanged();
		}).Enter(delegate(Instance smi)
		{
			Components.IncubationMonitors.Add(smi);
		}).Exit(delegate(Instance smi)
		{
			Components.IncubationMonitors.Remove(smi);
		});
		incubating.PlayAnim("idle", KAnim.PlayMode.Loop).Transition(hatching_pre, IsReadyToHatch, UpdateRate.SIM_1000ms).TagTransition(GameTags.Entombed, entombed)
			.ParamTransition(isSuppressed, suppressed, GameStateMachine<IncubationMonitor, Instance, IStateMachineTarget, Def>.IsTrue)
			.ToggleEffect((Instance smi) => smi.incubatingEffect);
		entombed.TagTransition(GameTags.Entombed, incubating, on_remove: true);
		suppressed.ToggleEffect((Instance smi) => suppressedEffect).ParamTransition(isSuppressed, incubating, GameStateMachine<IncubationMonitor, Instance, IStateMachineTarget, Def>.IsFalse).TagTransition(GameTags.Entombed, entombed)
			.Transition(not_viable, NoLongerViable, UpdateRate.SIM_1000ms);
		hatching_pre.Enter(DropSelfFromStorage).PlayAnim("hatching_pre").OnAnimQueueComplete(hatching_pst);
		hatching_pst.Enter(SpawnBaby).PlayAnim("hatching_pst").OnAnimQueueComplete(null)
			.Exit(DeleteSelf);
		not_viable.Enter(SpawnGenericEgg).GoTo(null).Exit(DeleteSelf);
		suppressedEffect = new Effect("IncubationSuppressed", CREATURES.MODIFIERS.INCUBATING_SUPPRESSED.NAME, CREATURES.MODIFIERS.INCUBATING_SUPPRESSED.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: true);
		suppressedEffect.Add(new AttributeModifier(Db.Get().Amounts.Viability.deltaAttribute.Id, -0.016666668f, CREATURES.MODIFIERS.INCUBATING_SUPPRESSED.NAME));
	}

	private static bool IsReadyToHatch(Instance smi)
	{
		if (smi.gameObject.HasTag(GameTags.Entombed))
		{
			return false;
		}
		return smi.incubation.value >= smi.incubation.GetMax();
	}

	private static void SpawnBaby(Instance smi)
	{
		Vector3 position = smi.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(smi.def.spawnedCreature), position);
		gameObject.SetActive(value: true);
		gameObject.GetSMI<AnimInterruptMonitor.Instance>().Play("hatching_pst");
		KSelectable component = smi.gameObject.GetComponent<KSelectable>();
		if (SelectTool.Instance != null && SelectTool.Instance.selected != null && SelectTool.Instance.selected == component)
		{
			SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>());
		}
		Db.Get().Amounts.Wildness.Copy(gameObject, smi.gameObject);
		if (smi.incubator != null)
		{
			smi.incubator.StoreBaby(gameObject);
		}
		SpawnShell(smi);
		SaveLoader.Instance.saveManager.Unregister(smi.GetComponent<SaveLoadRoot>());
	}

	private static bool NoLongerViable(Instance smi)
	{
		if (smi.gameObject.HasTag(GameTags.Entombed))
		{
			return false;
		}
		return smi.viability.value <= smi.viability.GetMin();
	}

	private static GameObject SpawnShell(Instance smi)
	{
		Vector3 position = smi.transform.GetPosition();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("EggShell"), position);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		PrimaryElement component2 = smi.GetComponent<PrimaryElement>();
		component.Mass = component2.Mass * 0.5f;
		gameObject.SetActive(value: true);
		return gameObject;
	}

	private static GameObject SpawnEggInnards(Instance smi)
	{
		Vector3 position = smi.transform.GetPosition();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("RawEgg"), position);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		PrimaryElement component2 = smi.GetComponent<PrimaryElement>();
		component.Mass = component2.Mass * 0.5f;
		gameObject.SetActive(value: true);
		return gameObject;
	}

	private static void SpawnGenericEgg(Instance smi)
	{
		SpawnShell(smi);
		GameObject gameObject = SpawnEggInnards(smi);
		KSelectable component = smi.gameObject.GetComponent<KSelectable>();
		if (SelectTool.Instance != null && SelectTool.Instance.selected != null && SelectTool.Instance.selected == component)
		{
			SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>());
		}
	}

	private static void DeleteSelf(Instance smi)
	{
		smi.gameObject.DeleteObject();
	}

	private static void DropSelfFromStorage(Instance smi)
	{
		if (!smi.sm.inIncubator.Get(smi))
		{
			Storage storage = smi.GetStorage();
			if ((bool)storage)
			{
				storage.Drop(smi.gameObject);
			}
			smi.gameObject.AddTag(GameTags.StoredPrivate);
		}
	}
}
