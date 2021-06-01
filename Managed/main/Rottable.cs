using System.Collections.Generic;
using System.Runtime.Serialization;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Rottable : GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>
{
	public class Def : BaseDef
	{
		public float spoilTime;

		public float staleTime;

		public float preserveTemperature = 255.15f;

		public float rotTemperature = 277.15f;
	}

	private class RotCB : UpdateBucketWithUpdater<Instance>.IUpdater
	{
		public void Update(Instance smi, float dt)
		{
			smi.Rot(smi, dt);
		}
	}

	public new class Instance : GameInstance, IRottable
	{
		private AmountInstance rotAmountInstance;

		private static AttributeModifier unrefrigeratedModifier;

		private static AttributeModifier refrigeratedModifier;

		private static AttributeModifier frozenModifier;

		private static AttributeModifier contaminatedAtmosphereModifier;

		private static AttributeModifier normalAtmosphereModifier;

		private static AttributeModifier sterileAtmosphereModifier;

		public PrimaryElement primaryElement;

		public Pickupable pickupable;

		public float RotValue
		{
			get
			{
				return rotAmountInstance.value;
			}
			set
			{
				base.sm.rotParameter.Set(value, this);
				rotAmountInstance.SetValue(value);
			}
		}

		public float RotConstitutionPercentage => RotValue / base.def.spoilTime;

		public float RotTemperature => base.def.rotTemperature;

		public float PreserveTemperature => base.def.preserveTemperature;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			pickupable = base.gameObject.RequireComponent<Pickupable>();
			base.master.Subscribe(-2064133523, OnAbsorb);
			base.master.Subscribe(1335436905, OnSplitFromChunk);
			primaryElement = base.gameObject.GetComponent<PrimaryElement>();
			Amounts amounts = master.gameObject.GetAmounts();
			rotAmountInstance = amounts.Add(new AmountInstance(Db.Get().Amounts.Rot, master.gameObject));
			rotAmountInstance.maxAttribute.Add(new AttributeModifier("Rot", def.spoilTime));
			rotAmountInstance.SetValue(def.spoilTime);
			base.sm.rotParameter.Set(rotAmountInstance.value, base.smi);
			if (unrefrigeratedModifier == null)
			{
				unrefrigeratedModifier = new AttributeModifier(rotAmountInstance.amount.Id, -0.7f, DUPLICANTS.MODIFIERS.ROTTEMPERATURE.UNREFRIGERATED);
				refrigeratedModifier = new AttributeModifier(rotAmountInstance.amount.Id, -0.2f, DUPLICANTS.MODIFIERS.ROTTEMPERATURE.REFRIGERATED);
				frozenModifier = new AttributeModifier(rotAmountInstance.amount.Id, -0f, DUPLICANTS.MODIFIERS.ROTTEMPERATURE.FROZEN);
				contaminatedAtmosphereModifier = new AttributeModifier(rotAmountInstance.amount.Id, -1f, DUPLICANTS.MODIFIERS.ROTATMOSPHERE.CONTAMINATED);
				normalAtmosphereModifier = new AttributeModifier(rotAmountInstance.amount.Id, -0.3f, DUPLICANTS.MODIFIERS.ROTATMOSPHERE.NORMAL);
				sterileAtmosphereModifier = new AttributeModifier(rotAmountInstance.amount.Id, -0f, DUPLICANTS.MODIFIERS.ROTATMOSPHERE.STERILE);
			}
			RefreshModifiers(0f);
		}

		[OnDeserialized]
		private void OnDeserialized()
		{
			if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 23))
			{
				rotAmountInstance.SetValue(rotAmountInstance.value * 2f);
			}
		}

		public string StateString()
		{
			string result = "";
			if (base.smi.GetCurrentState() == base.sm.Fresh)
			{
				result = Db.Get().CreatureStatusItems.Fresh.resolveStringCallback(CREATURES.STATUSITEMS.FRESH.NAME, this);
			}
			if (base.smi.GetCurrentState() == base.sm.Stale)
			{
				result = Db.Get().CreatureStatusItems.Fresh.resolveStringCallback(CREATURES.STATUSITEMS.STALE.NAME, this);
			}
			return result;
		}

		public void Rot(Instance smi, float deltaTime)
		{
			smi.sm.rotParameter.Set(rotAmountInstance.value, smi);
			RefreshModifiers(deltaTime);
			if (smi.pickupable.storage != null)
			{
				smi.pickupable.storage.Trigger(-1197125120);
			}
		}

		public void RefreshModifiers(float dt)
		{
			IStateMachineTarget master = GetMaster();
			if (master.isNull)
			{
				return;
			}
			int cell = Grid.PosToCell(base.gameObject);
			if (!Grid.IsValidCell(cell))
			{
				return;
			}
			rotAmountInstance.deltaAttribute.ClearModifiers();
			KPrefabID component = GetComponent<KPrefabID>();
			if (!component.HasAnyTags(PRESERVED_TAGS))
			{
				switch (RefrigerationLevel(this))
				{
				case RotRefrigerationLevel.Frozen:
					rotAmountInstance.deltaAttribute.Add(frozenModifier);
					break;
				case RotRefrigerationLevel.Refrigerated:
					rotAmountInstance.deltaAttribute.Add(refrigeratedModifier);
					break;
				default:
					rotAmountInstance.deltaAttribute.Add(unrefrigeratedModifier);
					break;
				}
				switch (AtmosphereQuality(this))
				{
				case RotAtmosphereQuality.Contaminating:
					rotAmountInstance.deltaAttribute.Add(contaminatedAtmosphereModifier);
					break;
				case RotAtmosphereQuality.Sterilizing:
					rotAmountInstance.deltaAttribute.Add(sterileAtmosphereModifier);
					break;
				default:
					rotAmountInstance.deltaAttribute.Add(normalAtmosphereModifier);
					break;
				}
			}
			SetStatusItems(this);
		}

		private void OnAbsorb(object data)
		{
			Pickupable pickupable = (Pickupable)data;
			if (pickupable != null)
			{
				PrimaryElement component = base.gameObject.GetComponent<PrimaryElement>();
				PrimaryElement primaryElement = pickupable.PrimaryElement;
				Instance sMI = pickupable.gameObject.GetSMI<Instance>();
				if (component != null && primaryElement != null && sMI != null)
				{
					float num = component.Units * base.sm.rotParameter.Get(base.smi);
					float num2 = primaryElement.Units * base.sm.rotParameter.Get(sMI);
					float value = (num + num2) / (component.Units + primaryElement.Units);
					base.sm.rotParameter.Set(value, base.smi);
				}
			}
		}

		public bool IsRotLevelStackable(Instance other)
		{
			float num = Mathf.Abs(RotConstitutionPercentage - other.RotConstitutionPercentage);
			return num < 0.1f;
		}

		public string GetToolTip()
		{
			return rotAmountInstance.GetTooltip();
		}

		private void OnSplitFromChunk(object data)
		{
			Pickupable pickupable = (Pickupable)data;
			if (pickupable != null)
			{
				Instance sMI = pickupable.GetSMI<Instance>();
				if (sMI != null)
				{
					RotValue = sMI.RotValue;
				}
			}
		}

		public void OnPreserved(object data)
		{
			if ((bool)data)
			{
				base.smi.GoTo(base.sm.Preserved);
			}
			else
			{
				base.smi.GoTo(base.sm.Fresh);
			}
		}
	}

	public enum RotAtmosphereQuality
	{
		Normal,
		Sterilizing,
		Contaminating
	}

	public enum RotRefrigerationLevel
	{
		Normal,
		Refrigerated,
		Frozen
	}

	public FloatParameter rotParameter;

	public State Preserved;

	public State Fresh;

	public State Stale_Pre;

	public State Stale;

	public State Spoiled;

	private static readonly Tag[] PRESERVED_TAGS = new Tag[2]
	{
		GameTags.Preserved,
		GameTags.Entombed
	};

	private static readonly RotCB rotCB = new RotCB();

	public static Dictionary<int, RotAtmosphereQuality> AtmosphereModifier = new Dictionary<int, RotAtmosphereQuality>
	{
		{
			721531317,
			RotAtmosphereQuality.Contaminating
		},
		{
			1887387588,
			RotAtmosphereQuality.Contaminating
		},
		{
			-1528777920,
			RotAtmosphereQuality.Normal
		},
		{
			1836671383,
			RotAtmosphereQuality.Normal
		},
		{
			1960575215,
			RotAtmosphereQuality.Sterilizing
		},
		{
			-899515856,
			RotAtmosphereQuality.Sterilizing
		},
		{
			-1554872654,
			RotAtmosphereQuality.Sterilizing
		},
		{
			-1858722091,
			RotAtmosphereQuality.Sterilizing
		},
		{
			758759285,
			RotAtmosphereQuality.Sterilizing
		},
		{
			-1046145888,
			RotAtmosphereQuality.Sterilizing
		},
		{
			-1324664829,
			RotAtmosphereQuality.Sterilizing
		},
		{
			-1406916018,
			RotAtmosphereQuality.Sterilizing
		},
		{
			-432557516,
			RotAtmosphereQuality.Sterilizing
		},
		{
			-805366663,
			RotAtmosphereQuality.Sterilizing
		},
		{
			1966552544,
			RotAtmosphereQuality.Sterilizing
		}
	};

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = Fresh;
		base.serializable = SerializeType.Both_DEPRECATED;
		root.TagTransition(GameTags.Preserved, Preserved).TagTransition(GameTags.Entombed, Preserved);
		Fresh.ToggleStatusItem(Db.Get().CreatureStatusItems.Fresh, (Instance smi) => smi).ParamTransition(rotParameter, Stale_Pre, (Instance smi, float p) => p <= smi.def.spoilTime - (smi.def.spoilTime - smi.def.staleTime)).FastUpdate("Rot", rotCB, UpdateRate.SIM_1000ms, load_balance: true);
		Preserved.TagTransition(PRESERVED_TAGS, Fresh, on_remove: true).Enter("RefreshModifiers", delegate(Instance smi)
		{
			smi.RefreshModifiers(0f);
		});
		Stale_Pre.Enter(delegate(Instance smi)
		{
			smi.GoTo(Stale);
		});
		Stale.ToggleStatusItem(Db.Get().CreatureStatusItems.Stale, (Instance smi) => smi).ParamTransition(rotParameter, Fresh, (Instance smi, float p) => p > smi.def.spoilTime - (smi.def.spoilTime - smi.def.staleTime)).ParamTransition(rotParameter, Spoiled, GameStateMachine<Rottable, Instance, IStateMachineTarget, Def>.IsLTEZero)
			.FastUpdate("Rot", rotCB, UpdateRate.SIM_1000ms);
		Spoiled.Enter(delegate(Instance smi)
		{
			GameObject gameObject = Scenario.SpawnPrefab(Grid.PosToCell(smi.master.gameObject), 0, 0, "RotPile");
			gameObject.gameObject.GetComponent<KSelectable>().SetName(string.Concat(UI.GAMEOBJECTEFFECTS.ROTTEN, " ", smi.master.gameObject.GetProperName()));
			gameObject.transform.SetPosition(smi.master.transform.GetPosition());
			gameObject.GetComponent<PrimaryElement>().Mass = smi.master.GetComponent<PrimaryElement>().Mass;
			gameObject.GetComponent<PrimaryElement>().Temperature = smi.master.GetComponent<PrimaryElement>().Temperature;
			gameObject.SetActive(value: true);
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, ITEMS.FOOD.ROTPILE.NAME, gameObject.transform);
			Edible component = smi.GetComponent<Edible>();
			if (component != null)
			{
				if (component.worker != null)
				{
					ChoreDriver component2 = component.worker.GetComponent<ChoreDriver>();
					if (component2 != null && component2.GetCurrentChore() != null)
					{
						component2.GetCurrentChore().Fail("food rotted");
					}
				}
				ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, 0f - component.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.ROTTED, "{0}", smi.gameObject.GetProperName()), UI.ENDOFDAYREPORT.NOTES.ROTTED_CONTEXT);
			}
			Util.KDestroyGameObject(smi.gameObject);
		});
	}

	private static string OnStaleTooltip(List<Notification> notifications, object data)
	{
		string text = "\n";
		foreach (Notification notification in notifications)
		{
			if (notification.tooltipData != null)
			{
				GameObject gameObject = (GameObject)notification.tooltipData;
				if (gameObject != null)
				{
					text = text + "\n" + gameObject.GetProperName();
				}
			}
		}
		return string.Format(MISC.NOTIFICATIONS.FOODSTALE.TOOLTIP, text);
	}

	public static void SetStatusItems(IRottable rottable)
	{
		int num = Grid.PosToCell(rottable.gameObject);
		KSelectable component = rottable.gameObject.GetComponent<KSelectable>();
		switch (RefrigerationLevel(rottable))
		{
		case RotRefrigerationLevel.Frozen:
			component.SetStatusItem(Db.Get().StatusItemCategories.PreservationTemperature, Db.Get().CreatureStatusItems.RefrigeratedFrozen, rottable);
			break;
		case RotRefrigerationLevel.Refrigerated:
			component.SetStatusItem(Db.Get().StatusItemCategories.PreservationTemperature, Db.Get().CreatureStatusItems.Refrigerated, rottable);
			break;
		default:
			component.SetStatusItem(Db.Get().StatusItemCategories.PreservationTemperature, Db.Get().CreatureStatusItems.Unrefrigerated, rottable);
			break;
		}
		switch (AtmosphereQuality(rottable))
		{
		case RotAtmosphereQuality.Contaminating:
			component.SetStatusItem(Db.Get().StatusItemCategories.PreservationAtmosphere, Db.Get().CreatureStatusItems.ContaminatedAtmosphere);
			break;
		case RotAtmosphereQuality.Sterilizing:
			component.SetStatusItem(Db.Get().StatusItemCategories.PreservationAtmosphere, Db.Get().CreatureStatusItems.SterilizingAtmosphere);
			break;
		default:
			component.SetStatusItem(Db.Get().StatusItemCategories.PreservationAtmosphere, null);
			break;
		}
	}

	public static bool IsInActiveFridge(IRottable rottable)
	{
		Pickupable component = rottable.gameObject.GetComponent<Pickupable>();
		if (component != null && component.storage != null)
		{
			Refrigerator component2 = component.storage.GetComponent<Refrigerator>();
			return component2 != null && component2.IsActive();
		}
		return false;
	}

	public static RotRefrigerationLevel RefrigerationLevel(IRottable rottable)
	{
		int num = Grid.PosToCell(rottable.gameObject);
		if (!Grid.IsValidCell(num))
		{
			return RotRefrigerationLevel.Normal;
		}
		PrimaryElement component = rottable.gameObject.GetComponent<PrimaryElement>();
		float num2 = Mathf.Min(Grid.Temperature[num], component.Temperature);
		if (num2 < rottable.PreserveTemperature)
		{
			return RotRefrigerationLevel.Frozen;
		}
		if (num2 < rottable.RotTemperature || IsInActiveFridge(rottable))
		{
			return RotRefrigerationLevel.Refrigerated;
		}
		return RotRefrigerationLevel.Normal;
	}

	public static RotAtmosphereQuality AtmosphereQuality(IRottable rottable)
	{
		int num = Grid.PosToCell(rottable.gameObject);
		int num2 = Grid.CellAbove(num);
		SimHashes id = Grid.Element[num].id;
		RotAtmosphereQuality value = RotAtmosphereQuality.Normal;
		AtmosphereModifier.TryGetValue((int)id, out value);
		SimHashes simHashes = SimHashes.Unobtanium;
		RotAtmosphereQuality value2 = RotAtmosphereQuality.Normal;
		if (Grid.IsValidCell(num2))
		{
			simHashes = Grid.Element[num2].id;
			if (!AtmosphereModifier.TryGetValue((int)simHashes, out value2))
			{
				value2 = value;
			}
		}
		else
		{
			value2 = value;
		}
		if (value == value2)
		{
			return value;
		}
		if (value == RotAtmosphereQuality.Contaminating || value2 == RotAtmosphereQuality.Contaminating)
		{
			return RotAtmosphereQuality.Contaminating;
		}
		if (value == RotAtmosphereQuality.Normal || value2 == RotAtmosphereQuality.Normal)
		{
			return RotAtmosphereQuality.Normal;
		}
		return RotAtmosphereQuality.Sterilizing;
	}
}
