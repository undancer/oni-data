using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/ComplexFabricator")]
public class ComplexFabricator : KMonoBehaviour, ISim200ms, ISim1000ms
{
	private const int MaxPrefetchCount = 2;

	public bool duplicantOperated = true;

	protected ComplexFabricatorWorkable workable;

	[SerializeField]
	public HashedString fetchChoreTypeIdHash = Db.Get().ChoreTypes.FabricateFetch.IdHash;

	[SerializeField]
	public float heatedTemperature = 0f;

	[SerializeField]
	public bool storeProduced = false;

	public ComplexFabricatorSideScreen.StyleSetting sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;

	public bool labelByResult = true;

	public Vector3 outputOffset = Vector3.zero;

	public ChoreType choreType;

	public bool keepExcessLiquids;

	public TagBits keepAdditionalTags = default(TagBits);

	public static int MAX_QUEUE_SIZE = 99;

	public static int QUEUE_INFINITE = -1;

	[Serialize]
	private Dictionary<string, int> recipeQueueCounts = new Dictionary<string, int>();

	private int nextOrderIdx;

	private bool nextOrderIsWorkable;

	private int workingOrderIdx = -1;

	[Serialize]
	private string lastWorkingRecipe;

	[Serialize]
	private float orderProgress = 0f;

	private List<int> openOrderCounts = new List<int>();

	private bool queueDirty = true;

	private bool hasOpenOrders;

	private List<FetchList2> fetchListList = new List<FetchList2>();

	private Chore chore;

	private bool cancelling;

	private ComplexRecipe[] recipe_list;

	private Dictionary<Tag, float> materialNeedCache = new Dictionary<Tag, float>();

	[SerializeField]
	public Storage inStorage;

	[SerializeField]
	public Storage buildStorage;

	[SerializeField]
	public Storage outStorage;

	[MyCmpAdd]
	private LoopingSounds loopingSounds;

	[MyCmpReq]
	protected Operational operational;

	[MyCmpAdd]
	private ComplexFabricatorSM fabricatorSM;

	private ProgressBar progressBar;

	private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>(delegate(ComplexFabricator component, object data)
	{
		component.OnStorageChange(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnDroppedAllDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>(delegate(ComplexFabricator component, object data)
	{
		component.OnDroppedAll(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>(delegate(ComplexFabricator component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>(delegate(ComplexFabricator component, object data)
	{
		component.OnCopySettings(data);
	});

	public ComplexFabricatorWorkable Workable => workable;

	public int CurrentOrderIdx => nextOrderIdx;

	public ComplexRecipe CurrentWorkingOrder => HasWorkingOrder ? recipe_list[workingOrderIdx] : null;

	public ComplexRecipe NextOrder => nextOrderIsWorkable ? recipe_list[nextOrderIdx] : null;

	public float OrderProgress
	{
		get
		{
			return orderProgress;
		}
		set
		{
			orderProgress = value;
		}
	}

	public bool HasAnyOrder => HasWorkingOrder || hasOpenOrders;

	public bool HasWorker => !duplicantOperated || workable.worker != null;

	public bool WaitingForWorker => HasWorkingOrder && !HasWorker;

	private bool HasWorkingOrder => workingOrderIdx > -1;

	public List<FetchList2> DebugFetchLists => fetchListList;

	[OnDeserialized]
	protected virtual void OnDeserializedMethod()
	{
		List<string> list = new List<string>();
		foreach (string key in recipeQueueCounts.Keys)
		{
			ComplexRecipe recipe = ComplexRecipeManager.Get().GetRecipe(key);
			if (recipe == null)
			{
				list.Add(key);
			}
		}
		foreach (string item in list)
		{
			Debug.LogWarningFormat("{1} removing missing recipe from queue: {0}", item, base.name);
			recipeQueueCounts.Remove(item);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GetRecipes();
		simRenderLoadBalance = true;
		choreType = Db.Get().ChoreTypes.Fabricate;
		Subscribe(-1957399615, OnDroppedAllDelegate);
		Subscribe(-592767678, OnOperationalChangedDelegate);
		Subscribe(-905833192, OnCopySettingsDelegate);
		Subscribe(-1697596308, OnStorageChangeDelegate);
		workable = GetComponent<ComplexFabricatorWorkable>();
		Components.ComplexFabricators.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		InitRecipeQueueCount();
		foreach (string key in recipeQueueCounts.Keys)
		{
			if (recipeQueueCounts[key] == 100)
			{
				recipeQueueCounts[key] = QUEUE_INFINITE;
			}
		}
		buildStorage.Transfer(inStorage, block_events: true, hide_popups: true);
		DropExcessIngredients(inStorage);
		int num = FindRecipeIndex(lastWorkingRecipe);
		if (num > -1)
		{
			nextOrderIdx = num;
		}
	}

	protected override void OnCleanUp()
	{
		CancelAllOpenOrders();
		CancelChore();
		Components.ComplexFabricators.Remove(this);
		base.OnCleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		if ((bool)data)
		{
			queueDirty = true;
		}
		else
		{
			CancelAllOpenOrders();
		}
		UpdateChore();
	}

	public void Sim1000ms(float dt)
	{
		RefreshAndStartNextOrder();
		if (materialNeedCache.Count > 0 && fetchListList.Count == 0)
		{
			Debug.LogWarningFormat(base.gameObject, "{0} has material needs cached, but no open fetches. materialNeedCache={1}, fetchListList={2}", base.gameObject, materialNeedCache.Count, fetchListList.Count);
			queueDirty = true;
		}
	}

	public void Sim200ms(float dt)
	{
		if (!operational.IsOperational)
		{
			return;
		}
		operational.SetActive(HasWorkingOrder && HasWorker);
		if (!duplicantOperated && HasWorkingOrder)
		{
			ComplexRecipe complexRecipe = recipe_list[workingOrderIdx];
			orderProgress += dt / complexRecipe.time;
			if (orderProgress >= 1f)
			{
				CompleteWorkingOrder();
			}
		}
	}

	private void RefreshAndStartNextOrder()
	{
		if (operational.IsOperational)
		{
			if (queueDirty)
			{
				RefreshQueue();
			}
			if (!HasWorkingOrder && nextOrderIsWorkable)
			{
				StartWorkingOrder(nextOrderIdx);
			}
		}
	}

	public void SetQueueDirty()
	{
		queueDirty = true;
	}

	private void RefreshQueue()
	{
		queueDirty = false;
		ValidateWorkingOrder();
		ValidateNextOrder();
		UpdateOpenOrders();
		DropExcessIngredients(inStorage);
		Trigger(1721324763, this);
	}

	private void StartWorkingOrder(int index)
	{
		Debug.Assert(!HasWorkingOrder, "machineOrderIdx already set");
		workingOrderIdx = index;
		if (recipe_list[workingOrderIdx].id != lastWorkingRecipe)
		{
			orderProgress = 0f;
			lastWorkingRecipe = recipe_list[workingOrderIdx].id;
		}
		TransferCurrentRecipeIngredientsForBuild();
		Debug.Assert(openOrderCounts[workingOrderIdx] > 0, "openOrderCount invalid");
		openOrderCounts[workingOrderIdx]--;
		UpdateChore();
		AdvanceNextOrder();
	}

	private void CancelWorkingOrder()
	{
		Debug.Assert(HasWorkingOrder, "machineOrderIdx not set");
		buildStorage.Transfer(inStorage, block_events: true, hide_popups: true);
		workingOrderIdx = -1;
		orderProgress = 0f;
		UpdateChore();
	}

	public void CompleteWorkingOrder()
	{
		if (!HasWorkingOrder)
		{
			Debug.LogWarning("CompleteWorkingOrder called with no working order.", base.gameObject);
			return;
		}
		ComplexRecipe recipe = recipe_list[workingOrderIdx];
		SpawnOrderProduct(recipe);
		float num = buildStorage.MassStored();
		if (num != 0f)
		{
			Debug.LogWarningFormat(base.gameObject, "{0} build storage contains mass {1} after order completion.", base.gameObject, num);
			buildStorage.Transfer(inStorage, block_events: true, hide_popups: true);
		}
		DecrementRecipeQueueCountInternal(recipe);
		workingOrderIdx = -1;
		orderProgress = 0f;
		CancelChore();
		if (!cancelling)
		{
			RefreshAndStartNextOrder();
		}
	}

	private void ValidateWorkingOrder()
	{
		if (HasWorkingOrder)
		{
			ComplexRecipe recipe = recipe_list[workingOrderIdx];
			if (!IsRecipeQueued(recipe))
			{
				CancelWorkingOrder();
			}
		}
	}

	private void UpdateChore()
	{
		if (duplicantOperated)
		{
			bool flag = operational.IsOperational && HasWorkingOrder;
			if (flag && chore == null)
			{
				CreateChore();
			}
			else if (!flag && chore != null)
			{
				CancelChore();
			}
		}
	}

	private void AdvanceNextOrder()
	{
		for (int i = 0; i < recipe_list.Length; i++)
		{
			nextOrderIdx = (nextOrderIdx + 1) % recipe_list.Length;
			ComplexRecipe recipe = recipe_list[nextOrderIdx];
			nextOrderIsWorkable = GetRemainingQueueCount(recipe) > 0 && HasIngredients(recipe, inStorage);
			if (nextOrderIsWorkable)
			{
				break;
			}
		}
	}

	private void ValidateNextOrder()
	{
		ComplexRecipe recipe = recipe_list[nextOrderIdx];
		nextOrderIsWorkable = GetRemainingQueueCount(recipe) > 0 && HasIngredients(recipe, inStorage);
		if (!nextOrderIsWorkable)
		{
			AdvanceNextOrder();
		}
	}

	private void CancelAllOpenOrders()
	{
		for (int i = 0; i < openOrderCounts.Count; i++)
		{
			openOrderCounts[i] = 0;
		}
		ClearMaterialNeeds();
		CancelFetches();
	}

	private void UpdateOpenOrders()
	{
		ComplexRecipe[] recipes = GetRecipes();
		if (recipes.Length != openOrderCounts.Count)
		{
			Debug.LogErrorFormat(base.gameObject, "Recipe count {0} doesn't match open order count {1}", recipes.Length, openOrderCounts.Count);
		}
		bool flag = false;
		hasOpenOrders = false;
		for (int i = 0; i < recipes.Length; i++)
		{
			ComplexRecipe recipe = recipes[i];
			int recipePrefetchCount = GetRecipePrefetchCount(recipe);
			if (recipePrefetchCount > 0)
			{
				hasOpenOrders = true;
			}
			int num = openOrderCounts[i];
			if (num != recipePrefetchCount)
			{
				if (recipePrefetchCount < num)
				{
					flag = true;
				}
				openOrderCounts[i] = recipePrefetchCount;
			}
		}
		DictionaryPool<Tag, float, ComplexFabricator>.PooledDictionary pooledDictionary = DictionaryPool<Tag, float, ComplexFabricator>.Allocate();
		DictionaryPool<Tag, float, ComplexFabricator>.PooledDictionary pooledDictionary2 = DictionaryPool<Tag, float, ComplexFabricator>.Allocate();
		for (int j = 0; j < openOrderCounts.Count; j++)
		{
			int num2 = openOrderCounts[j];
			if (num2 > 0)
			{
				ComplexRecipe complexRecipe = recipe_list[j];
				ComplexRecipe.RecipeElement[] ingredients = complexRecipe.ingredients;
				ComplexRecipe.RecipeElement[] array = ingredients;
				foreach (ComplexRecipe.RecipeElement recipeElement in array)
				{
					pooledDictionary[recipeElement.material] = inStorage.GetAmountAvailable(recipeElement.material);
				}
			}
		}
		for (int l = 0; l < recipe_list.Length; l++)
		{
			int num3 = openOrderCounts[l];
			if (num3 <= 0)
			{
				continue;
			}
			ComplexRecipe complexRecipe2 = recipe_list[l];
			ComplexRecipe.RecipeElement[] ingredients2 = complexRecipe2.ingredients;
			ComplexRecipe.RecipeElement[] array2 = ingredients2;
			foreach (ComplexRecipe.RecipeElement recipeElement2 in array2)
			{
				float num4 = recipeElement2.amount * (float)num3;
				float num5 = num4 - pooledDictionary[recipeElement2.material];
				if (num5 > 0f)
				{
					pooledDictionary2.TryGetValue(recipeElement2.material, out var value);
					pooledDictionary2[recipeElement2.material] = value + num5;
					pooledDictionary[recipeElement2.material] = 0f;
				}
				else
				{
					pooledDictionary[recipeElement2.material] -= num4;
				}
			}
		}
		if (flag)
		{
			CancelFetches();
		}
		if (pooledDictionary2.Count > 0)
		{
			UpdateFetches(pooledDictionary2);
		}
		UpdateMaterialNeeds(pooledDictionary2);
		pooledDictionary2.Recycle();
		pooledDictionary.Recycle();
	}

	private void UpdateMaterialNeeds(Dictionary<Tag, float> missingAmounts)
	{
		ClearMaterialNeeds();
		foreach (KeyValuePair<Tag, float> missingAmount in missingAmounts)
		{
			MaterialNeeds.UpdateNeed(missingAmount.Key, missingAmount.Value, base.gameObject.GetMyWorldId());
			materialNeedCache.Add(missingAmount.Key, missingAmount.Value);
		}
	}

	private void ClearMaterialNeeds()
	{
		foreach (KeyValuePair<Tag, float> item in materialNeedCache)
		{
			MaterialNeeds.UpdateNeed(item.Key, 0f - item.Value, base.gameObject.GetMyWorldId());
		}
		materialNeedCache.Clear();
	}

	private void OnFetchComplete()
	{
		for (int num = fetchListList.Count - 1; num >= 0; num--)
		{
			FetchList2 fetchList = fetchListList[num];
			if (fetchList.IsComplete)
			{
				fetchListList.RemoveAt(num);
				queueDirty = true;
			}
		}
	}

	private void OnStorageChange(object data)
	{
		queueDirty = true;
	}

	private void OnDroppedAll(object data)
	{
		if (HasWorkingOrder)
		{
			CancelWorkingOrder();
		}
		CancelAllOpenOrders();
		RefreshQueue();
	}

	private void DropExcessIngredients(Storage storage)
	{
		TagBits search_tags = default(TagBits);
		search_tags.Or(ref keepAdditionalTags);
		for (int i = 0; i < recipe_list.Length; i++)
		{
			ComplexRecipe complexRecipe = recipe_list[i];
			if (IsRecipeQueued(complexRecipe))
			{
				ComplexRecipe.RecipeElement[] ingredients = complexRecipe.ingredients;
				foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
				{
					search_tags.SetTag(recipeElement.material);
				}
			}
		}
		for (int num = storage.items.Count - 1; num >= 0; num--)
		{
			GameObject gameObject = storage.items[num];
			if (!(gameObject == null))
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (!(component == null) && (!keepExcessLiquids || !component.Element.IsLiquid))
				{
					KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
					if ((bool)component2 && !component2.HasAnyTags(ref search_tags))
					{
						storage.Drop(gameObject);
					}
				}
			}
		}
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == null)
		{
			return;
		}
		ComplexFabricator component = gameObject.GetComponent<ComplexFabricator>();
		if (component == null)
		{
			return;
		}
		ComplexRecipe[] array = recipe_list;
		foreach (ComplexRecipe complexRecipe in array)
		{
			if (!component.recipeQueueCounts.TryGetValue(complexRecipe.id, out var value))
			{
				value = 0;
			}
			SetRecipeQueueCountInternal(complexRecipe, value);
		}
		RefreshQueue();
	}

	private int CompareRecipe(ComplexRecipe a, ComplexRecipe b)
	{
		if (a.sortOrder != b.sortOrder)
		{
			return a.sortOrder - b.sortOrder;
		}
		return StringComparer.InvariantCulture.Compare(a.id, b.id);
	}

	public ComplexRecipe[] GetRecipes()
	{
		if (recipe_list == null)
		{
			KPrefabID component = GetComponent<KPrefabID>();
			Tag prefabTag = component.PrefabTag;
			List<ComplexRecipe> recipes = ComplexRecipeManager.Get().recipes;
			List<ComplexRecipe> list = new List<ComplexRecipe>();
			foreach (ComplexRecipe item in recipes)
			{
				foreach (Tag fabricator in item.fabricators)
				{
					if (fabricator == prefabTag)
					{
						list.Add(item);
					}
				}
			}
			recipe_list = list.ToArray();
			Array.Sort(recipe_list, CompareRecipe);
		}
		return recipe_list;
	}

	private void InitRecipeQueueCount()
	{
		ComplexRecipe[] recipes = GetRecipes();
		foreach (ComplexRecipe complexRecipe in recipes)
		{
			bool flag = false;
			foreach (string key in recipeQueueCounts.Keys)
			{
				if (key == complexRecipe.id)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				recipeQueueCounts.Add(complexRecipe.id, 0);
			}
			openOrderCounts.Add(0);
		}
	}

	private int FindRecipeIndex(string id)
	{
		for (int i = 0; i < recipe_list.Length; i++)
		{
			if (recipe_list[i].id == id)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetRecipeQueueCount(ComplexRecipe recipe)
	{
		return recipeQueueCounts[recipe.id];
	}

	public bool IsRecipeQueued(ComplexRecipe recipe)
	{
		int num = recipeQueueCounts[recipe.id];
		Debug.Assert(num >= 0 || num == QUEUE_INFINITE);
		return num != 0;
	}

	public int GetRecipePrefetchCount(ComplexRecipe recipe)
	{
		int remainingQueueCount = GetRemainingQueueCount(recipe);
		Debug.Assert(remainingQueueCount >= 0);
		return Mathf.Min(2, remainingQueueCount);
	}

	private int GetRemainingQueueCount(ComplexRecipe recipe)
	{
		int num = recipeQueueCounts[recipe.id];
		Debug.Assert(num >= 0 || num == QUEUE_INFINITE);
		if (num == QUEUE_INFINITE)
		{
			return MAX_QUEUE_SIZE;
		}
		if (num > 0)
		{
			if (IsCurrentRecipe(recipe))
			{
				num--;
			}
			return num;
		}
		return 0;
	}

	private bool IsCurrentRecipe(ComplexRecipe recipe)
	{
		if (workingOrderIdx < 0)
		{
			return false;
		}
		return recipe_list[workingOrderIdx].id == recipe.id;
	}

	public void SetRecipeQueueCount(ComplexRecipe recipe, int count)
	{
		SetRecipeQueueCountInternal(recipe, count);
		RefreshQueue();
	}

	private void SetRecipeQueueCountInternal(ComplexRecipe recipe, int count)
	{
		recipeQueueCounts[recipe.id] = count;
	}

	public void IncrementRecipeQueueCount(ComplexRecipe recipe)
	{
		if (recipeQueueCounts[recipe.id] == QUEUE_INFINITE)
		{
			recipeQueueCounts[recipe.id] = 0;
		}
		else if (recipeQueueCounts[recipe.id] >= MAX_QUEUE_SIZE)
		{
			recipeQueueCounts[recipe.id] = QUEUE_INFINITE;
		}
		else
		{
			recipeQueueCounts[recipe.id]++;
		}
		RefreshQueue();
	}

	public void DecrementRecipeQueueCount(ComplexRecipe recipe, bool respectInfinite = true)
	{
		DecrementRecipeQueueCountInternal(recipe, respectInfinite);
		RefreshQueue();
	}

	private void DecrementRecipeQueueCountInternal(ComplexRecipe recipe, bool respectInfinite = true)
	{
		if (!respectInfinite || recipeQueueCounts[recipe.id] != QUEUE_INFINITE)
		{
			if (recipeQueueCounts[recipe.id] == QUEUE_INFINITE)
			{
				recipeQueueCounts[recipe.id] = MAX_QUEUE_SIZE;
			}
			else if (recipeQueueCounts[recipe.id] == 0)
			{
				recipeQueueCounts[recipe.id] = QUEUE_INFINITE;
			}
			else
			{
				recipeQueueCounts[recipe.id]--;
			}
		}
	}

	private void CreateChore()
	{
		Debug.Assert(chore == null, "chore should be null");
		chore = workable.CreateWorkChore(choreType, orderProgress);
	}

	private void CancelChore()
	{
		if (!cancelling)
		{
			cancelling = true;
			if (chore != null)
			{
				chore.Cancel("order cancelled");
				chore = null;
			}
			cancelling = false;
		}
	}

	private void UpdateFetches(DictionaryPool<Tag, float, ComplexFabricator>.PooledDictionary missingAmounts)
	{
		ChoreType byHash = Db.Get().ChoreTypes.GetByHash(fetchChoreTypeIdHash);
		foreach (KeyValuePair<Tag, float> missingAmount in missingAmounts)
		{
			if (missingAmount.Value >= PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT && !HasPendingFetch(missingAmount.Key))
			{
				FetchList2 fetchList = new FetchList2(inStorage, byHash);
				fetchList.Add(missingAmount.Key, null, null, missingAmount.Value);
				fetchList.ShowStatusItem = false;
				fetchList.Submit(OnFetchComplete, check_storage_contents: false);
				fetchListList.Add(fetchList);
			}
		}
	}

	private bool HasPendingFetch(Tag tag)
	{
		foreach (FetchList2 fetchList in fetchListList)
		{
			fetchList.MinimumAmount.TryGetValue(tag, out var value);
			if (value > 0f)
			{
				return true;
			}
		}
		return false;
	}

	private void CancelFetches()
	{
		foreach (FetchList2 fetchList in fetchListList)
		{
			fetchList.Cancel("cancel all orders");
		}
		fetchListList.Clear();
	}

	protected virtual void TransferCurrentRecipeIngredientsForBuild()
	{
		ComplexRecipe.RecipeElement[] ingredients = recipe_list[workingOrderIdx].ingredients;
		ComplexRecipe.RecipeElement[] array = ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in array)
		{
			while (true)
			{
				float num = recipeElement.amount - buildStorage.GetAmountAvailable(recipeElement.material);
				if (num <= 0f)
				{
					break;
				}
				if (inStorage.GetAmountAvailable(recipeElement.material) <= 0f)
				{
					Debug.LogWarningFormat("TransferCurrentRecipeIngredientsForBuild ran out of {0} but still needed {1} more.", recipeElement.material, num);
					break;
				}
				inStorage.Transfer(buildStorage, recipeElement.material, num, block_events: false, hide_popups: true);
			}
		}
	}

	protected virtual bool HasIngredients(ComplexRecipe recipe, Storage storage)
	{
		ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
		ComplexRecipe.RecipeElement[] array = ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in array)
		{
			float amountAvailable = storage.GetAmountAvailable(recipeElement.material);
			float num = recipeElement.amount - amountAvailable;
			if (num >= PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT)
			{
				return false;
			}
		}
		return true;
	}

	protected virtual List<GameObject> SpawnOrderProduct(ComplexRecipe recipe)
	{
		List<GameObject> list = new List<GameObject>();
		SimUtil.DiseaseInfo diseaseInfo = default(SimUtil.DiseaseInfo);
		diseaseInfo.count = 0;
		diseaseInfo.idx = 0;
		float num = 0f;
		float num2 = 0f;
		ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			num2 += recipeElement.amount;
		}
		Element element = null;
		ComplexRecipe.RecipeElement[] ingredients2 = recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement2 in ingredients2)
		{
			float num3 = recipeElement2.amount / num2;
			if (recipeElement2.inheritElement)
			{
				GameObject gameObject = buildStorage.FindFirst(recipeElement2.material);
				element = gameObject.GetComponent<PrimaryElement>().Element;
			}
			buildStorage.ConsumeAndGetDisease(recipeElement2.material, recipeElement2.amount, out var _, out var disease_info, out var aggregate_temperature);
			if (disease_info.count > diseaseInfo.count)
			{
				diseaseInfo = disease_info;
			}
			num += aggregate_temperature * num3;
		}
		ComplexRecipe.RecipeElement[] results = recipe.results;
		foreach (ComplexRecipe.RecipeElement recipeElement3 in results)
		{
			GameObject gameObject2 = buildStorage.FindFirst(recipeElement3.material);
			if (gameObject2 != null)
			{
				Edible component = gameObject2.GetComponent<Edible>();
				if ((bool)component)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, 0f - component.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.CRAFTED_USED, "{0}", component.GetProperName()), UI.ENDOFDAYREPORT.NOTES.CRAFTED_CONTEXT);
				}
			}
			switch (recipeElement3.temperatureOperation)
			{
			case ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature:
			case ComplexRecipe.RecipeElement.TemperatureOperation.Heated:
			{
				GameObject prefab = Assets.GetPrefab(recipeElement3.material);
				GameObject gameObject3 = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore);
				int cell = Grid.PosToCell(this);
				gameObject3.transform.SetPosition(Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore) + outputOffset);
				PrimaryElement component2 = gameObject3.GetComponent<PrimaryElement>();
				component2.Units = recipeElement3.amount;
				component2.Temperature = ((recipeElement3.temperatureOperation == ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature) ? num : heatedTemperature);
				if (element != null)
				{
					component2.SetElement(element.id, addTags: false);
				}
				gameObject3.SetActive(value: true);
				float num4 = recipeElement3.amount / recipe.TotalResultUnits();
				component2.AddDisease(diseaseInfo.idx, Mathf.RoundToInt((float)diseaseInfo.count * num4), "ComplexFabricator.CompleteOrder");
				gameObject3.GetComponent<KMonoBehaviour>().Trigger(748399584);
				list.Add(gameObject3);
				if (storeProduced || recipeElement3.storeElement)
				{
					outStorage.Store(gameObject3);
				}
				break;
			}
			case ComplexRecipe.RecipeElement.TemperatureOperation.Melted:
				if (storeProduced || recipeElement3.storeElement)
				{
					float temperature = ElementLoader.GetElement(recipeElement3.material).defaultValues.temperature;
					outStorage.AddLiquid(ElementLoader.GetElementID(recipeElement3.material), recipeElement3.amount, temperature, 0, 0);
				}
				break;
			}
			if (list.Count <= 0)
			{
				continue;
			}
			SymbolOverrideController component3 = GetComponent<SymbolOverrideController>();
			if (component3 != null)
			{
				KBatchedAnimController component4 = list[0].GetComponent<KBatchedAnimController>();
				KAnim.Build build = component4.AnimFiles[0].GetData().build;
				KAnim.Build.Symbol symbol = build.GetSymbol(build.name);
				if (symbol != null)
				{
					component3.TryRemoveSymbolOverride("output_tracker");
					component3.AddSymbolOverride("output_tracker", symbol);
				}
				else
				{
					Debug.LogWarning(component3.name + " is missing symbol " + build.name);
				}
			}
		}
		return list;
	}

	public virtual List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		ComplexRecipe[] recipes = GetRecipes();
		if (recipes.Length != 0)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(UI.BUILDINGEFFECTS.PROCESSES, UI.BUILDINGEFFECTS.TOOLTIPS.PROCESSES);
			list.Add(item);
		}
		ComplexRecipe[] array = recipes;
		foreach (ComplexRecipe complexRecipe in array)
		{
			string text = "";
			string uIName = complexRecipe.GetUIName(includeAmounts: false);
			ComplexRecipe.RecipeElement[] ingredients = complexRecipe.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
			{
				text = text + "• " + string.Format(UI.BUILDINGEFFECTS.PROCESSEDITEM, recipeElement.material.ProperName(), recipeElement.amount) + "\n";
			}
			Descriptor item2 = new Descriptor(uIName, string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.FABRICATOR_INGREDIENTS, text));
			item2.IncreaseIndent();
			list.Add(item2);
		}
		return list;
	}

	public virtual List<Descriptor> AdditionalEffectsForRecipe(ComplexRecipe recipe)
	{
		return new List<Descriptor>();
	}

	public string GetConversationTopic()
	{
		if (HasWorkingOrder)
		{
			ComplexRecipe complexRecipe = recipe_list[workingOrderIdx];
			if (complexRecipe != null)
			{
				return complexRecipe.results[0].material.Name;
			}
		}
		return null;
	}
}