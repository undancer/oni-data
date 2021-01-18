using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Edible")]
public class Edible : Workable, IGameObjectEffectDescriptor
{
	public class EdibleStartWorkInfo : Worker.StartWorkInfo
	{
		public float amount
		{
			get;
			private set;
		}

		public EdibleStartWorkInfo(Workable workable, float amount)
			: base(workable)
		{
			this.amount = amount;
		}
	}

	public string FoodID;

	private EdiblesManager.FoodInfo foodInfo;

	public float unitsConsumed = float.NaN;

	public float caloriesConsumed = float.NaN;

	private float totalFeedingTime = float.NaN;

	private float totalUnits = float.NaN;

	private float totalConsumableCalories = float.NaN;

	private AttributeModifier caloriesModifier = new AttributeModifier("CaloriesDelta", 50000f, DUPLICANTS.MODIFIERS.EATINGCALORIES.NAME, is_multiplier: false, uiOnly: true);

	private AttributeModifier caloriesLitSpaceModifier = new AttributeModifier("CaloriesDelta", (1f + DUPLICANTSTATS.LIGHT.LIGHT_WORK_EFFICIENCY_BONUS) / 2E-05f, DUPLICANTS.MODIFIERS.EATINGCALORIES.NAME, is_multiplier: false, uiOnly: true);

	private AttributeModifier currentModifier = null;

	private static readonly EventSystem.IntraObjectHandler<Edible> OnCraftDelegate = new EventSystem.IntraObjectHandler<Edible>(delegate(Edible component, object data)
	{
		component.OnCraft(data);
	});

	private static readonly HashedString[] normalWorkAnims = new HashedString[2]
	{
		"working_pre",
		"working_loop"
	};

	private static readonly HashedString[] hatWorkAnims = new HashedString[2]
	{
		"hat_pre",
		"working_loop"
	};

	private static readonly HashedString[] saltWorkAnims = new HashedString[2]
	{
		"salt_pre",
		"salt_loop"
	};

	private static readonly HashedString[] saltHatWorkAnims = new HashedString[2]
	{
		"salt_hat_pre",
		"salt_hat_loop"
	};

	private static readonly HashedString[] normalWorkPstAnim = new HashedString[1]
	{
		"working_pst"
	};

	private static readonly HashedString[] hatWorkPstAnim = new HashedString[1]
	{
		"hat_pst"
	};

	private static readonly HashedString[] saltWorkPstAnim = new HashedString[1]
	{
		"salt_pst"
	};

	private static readonly HashedString[] saltHatWorkPstAnim = new HashedString[1]
	{
		"salt_hat_pst"
	};

	private static Dictionary<int, string> qualityEffects = new Dictionary<int, string>
	{
		{
			-1,
			"EdibleMinus3"
		},
		{
			0,
			"EdibleMinus2"
		},
		{
			1,
			"EdibleMinus1"
		},
		{
			2,
			"Edible0"
		},
		{
			3,
			"Edible1"
		},
		{
			4,
			"Edible2"
		},
		{
			5,
			"Edible3"
		}
	};

	public float Units
	{
		get
		{
			return GetComponent<PrimaryElement>().Units;
		}
		set
		{
			GetComponent<PrimaryElement>().Units = value;
		}
	}

	public float Calories
	{
		get
		{
			return Units * foodInfo.CaloriesPerUnit;
		}
		set
		{
			Units = value / foodInfo.CaloriesPerUnit;
		}
	}

	public EdiblesManager.FoodInfo FoodInfo
	{
		get
		{
			return foodInfo;
		}
		set
		{
			foodInfo = value;
			FoodID = foodInfo.Id;
		}
	}

	public bool isBeingConsumed
	{
		get;
		private set;
	}

	private Edible()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
		showProgressBar = false;
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
		shouldTransferDiseaseWithWorker = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (foodInfo == null)
		{
			if (FoodID == null)
			{
				Debug.LogError("No food FoodID");
			}
			foodInfo = EdiblesManager.GetFoodInfo(FoodID);
		}
		GetComponent<KPrefabID>().AddTag(GameTags.Edible);
		Subscribe(748399584, OnCraftDelegate);
		Subscribe(1272413801, OnCraftDelegate);
		workerStatusItem = Db.Get().DuplicantStatusItems.Eating;
		synchronizeAnims = false;
		Components.Edibles.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.Edible, this);
	}

	public override HashedString[] GetWorkAnims(Worker worker)
	{
		bool flag = worker.GetSMI<EatChore.StatesInstance>()?.UseSalt() ?? false;
		MinionResume component = worker.GetComponent<MinionResume>();
		if (component != null && component.CurrentHat != null)
		{
			return flag ? saltHatWorkAnims : hatWorkAnims;
		}
		return flag ? saltWorkAnims : normalWorkAnims;
	}

	public override HashedString[] GetWorkPstAnims(Worker worker, bool successfully_completed)
	{
		bool flag = worker.GetSMI<EatChore.StatesInstance>()?.UseSalt() ?? false;
		MinionResume component = worker.GetComponent<MinionResume>();
		if (component != null && component.CurrentHat != null)
		{
			return flag ? saltHatWorkPstAnim : hatWorkPstAnim;
		}
		return flag ? saltWorkPstAnim : normalWorkPstAnim;
	}

	private void OnCraft(object data)
	{
		RationTracker.Get().RegisterCaloriesProduced(Calories);
	}

	public float GetFeedingTime(Worker worker)
	{
		float num = Calories * 2E-05f;
		if (worker != null && (worker.GetSMI<BingeEatChore.StatesInstance>()?.IsBingeEating() ?? false))
		{
			num /= 2f;
		}
		return num;
	}

	protected override void OnStartWork(Worker worker)
	{
		totalFeedingTime = GetFeedingTime(worker);
		SetWorkTime(totalFeedingTime);
		caloriesConsumed = 0f;
		unitsConsumed = 0f;
		totalUnits = Units;
		KPrefabID component = worker.GetComponent<KPrefabID>();
		component.AddTag(GameTags.AlwaysConverse);
		totalConsumableCalories = Units * foodInfo.CaloriesPerUnit;
		StartConsuming();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (currentlyLit)
		{
			if (currentModifier != caloriesLitSpaceModifier)
			{
				worker.GetAttributes().Remove(currentModifier);
				worker.GetAttributes().Add(caloriesLitSpaceModifier);
				currentModifier = caloriesLitSpaceModifier;
			}
		}
		else if (currentModifier != caloriesModifier)
		{
			worker.GetAttributes().Remove(currentModifier);
			worker.GetAttributes().Add(caloriesModifier);
			currentModifier = caloriesModifier;
		}
		return OnTickConsume(worker, dt);
	}

	protected override void OnStopWork(Worker worker)
	{
		if (currentModifier != null)
		{
			worker.GetAttributes().Remove(currentModifier);
			currentModifier = null;
		}
		KPrefabID component = worker.GetComponent<KPrefabID>();
		component.RemoveTag(GameTags.AlwaysConverse);
		StopConsuming(worker);
	}

	private bool OnTickConsume(Worker worker, float dt)
	{
		if (!isBeingConsumed)
		{
			DebugUtil.DevLogError("OnTickConsume while we're not eating, this would set a NaN mass on this Edible");
			return true;
		}
		bool result = false;
		float num = dt / totalFeedingTime;
		float num2 = num * totalConsumableCalories;
		if (caloriesConsumed + num2 > totalConsumableCalories)
		{
			num2 = totalConsumableCalories - caloriesConsumed;
		}
		caloriesConsumed += num2;
		worker.GetAmounts().Get("Calories").value += num2;
		float num3 = totalUnits * num;
		if (Units - num3 < 0f)
		{
			num3 = Units;
		}
		Units -= num3;
		unitsConsumed += num3;
		if (Units <= 0f)
		{
			result = true;
		}
		return result;
	}

	private void StartConsuming()
	{
		DebugUtil.DevAssert(!isBeingConsumed, "Can't StartConsuming()...we've already started");
		isBeingConsumed = true;
		base.worker.Trigger(1406130139, this);
	}

	private void StopConsuming(Worker worker)
	{
		DebugUtil.DevAssert(isBeingConsumed, "StopConsuming() called without StartConsuming()");
		isBeingConsumed = false;
		PrimaryElement component = base.gameObject.GetComponent<PrimaryElement>();
		if (component != null && component.DiseaseCount > 0)
		{
			new EmoteChore(worker.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_react_contaminated_food_kanim", new HashedString[1]
			{
				"react"
			}, null);
		}
		for (int i = 0; i < foodInfo.Effects.Count; i++)
		{
			worker.GetComponent<Effects>().Add(foodInfo.Effects[i], should_save: true);
		}
		ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, 0f - caloriesConsumed, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.EATEN, "{0}", this.GetProperName()), worker.GetProperName());
		AddQualityEffects(worker);
		worker.Trigger(1121894420, this);
		Trigger(-10536414, worker.gameObject);
		unitsConsumed = float.NaN;
		caloriesConsumed = float.NaN;
		totalUnits = float.NaN;
		if (Units <= 0f)
		{
			base.gameObject.DeleteObject();
		}
	}

	public static string GetEffectForFoodQuality(int qualityLevel)
	{
		qualityLevel = Mathf.Clamp(qualityLevel, -1, 5);
		return qualityEffects[qualityLevel];
	}

	private void AddQualityEffects(Worker worker)
	{
		Attributes attributes = worker.GetAttributes();
		AttributeInstance attributeInstance = attributes.Add(Db.Get().Attributes.FoodExpectation);
		float totalValue = attributeInstance.GetTotalValue();
		int num = Mathf.RoundToInt(totalValue);
		int qualityLevel = FoodInfo.Quality + num;
		Effects component = worker.GetComponent<Effects>();
		component.Add(GetEffectForFoodQuality(qualityLevel), should_save: true);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Edibles.Remove(this);
	}

	public int GetQuality()
	{
		return foodInfo.Quality;
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.CALORIES, GameUtil.GetFormattedCalories(foodInfo.CaloriesPerUnit)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.CALORIES, GameUtil.GetFormattedCalories(foodInfo.CaloriesPerUnit)), Descriptor.DescriptorType.Information));
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(foodInfo.Quality)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(foodInfo.Quality))));
		foreach (string effect in foodInfo.Effects)
		{
			list.Add(new Descriptor(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + effect.ToUpper() + ".NAME"), Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + effect.ToUpper() + ".DESCRIPTION")));
		}
		return list;
	}
}
