using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Deconstructable")]
public class Deconstructable : Workable
{
	public Chore chore;

	public bool allowDeconstruction = true;

	public string audioSize;

	[Serialize]
	private bool isMarkedForDeconstruction;

	[Serialize]
	public Tag[] constructionElements;

	private static readonly EventSystem.IntraObjectHandler<Deconstructable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Deconstructable>(delegate(Deconstructable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Deconstructable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Deconstructable>(delegate(Deconstructable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Deconstructable> OnDeconstructDelegate = new EventSystem.IntraObjectHandler<Deconstructable>(delegate(Deconstructable component, object data)
	{
		component.OnDeconstruct(data);
	});

	private static readonly Vector2 INITIAL_VELOCITY_RANGE = new Vector2(0.5f, 4f);

	private bool destroyed;

	private CellOffset[] placementOffsets
	{
		get
		{
			Building component = GetComponent<Building>();
			if (component != null)
			{
				return component.Def.PlacementOffsets;
			}
			OccupyArea component2 = GetComponent<OccupyArea>();
			if (component2 != null)
			{
				return component2.OccupiedCellsOffsets;
			}
			Debug.Assert(condition: false, "Ack! We put a Deconstructable on something that's neither a Building nor OccupyArea!", this);
			return null;
		}
	}

	public bool HasBeenDestroyed => destroyed;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		faceTargetWhenWorking = true;
		synchronizeAnims = false;
		workerStatusItem = Db.Get().DuplicantStatusItems.Deconstructing;
		attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		minimumAttributeMultiplier = 0.75f;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
		skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		multitoolContext = "build";
		multitoolHitEffectTag = EffectConfigs.BuildSplashId;
		workingPstComplete = null;
		workingPstFailed = null;
		CellOffset[][] table = OffsetGroups.InvertedStandardTable;
		CellOffset[] filter = null;
		Building component = GetComponent<Building>();
		if (component != null && component.Def.IsTilePiece)
		{
			table = OffsetGroups.InvertedStandardTableWithCorners;
			filter = component.Def.ConstructionOffsetFilter;
			SetWorkTime(component.Def.ConstructionTime * 0.5f);
		}
		else
		{
			SetWorkTime(30f);
		}
		CellOffset[][] offsetTable = OffsetGroups.BuildReachabilityTable(placementOffsets, table, filter);
		SetOffsetTable(offsetTable);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-111137758, OnRefreshUserMenuDelegate);
		Subscribe(2127324410, OnCancelDelegate);
		Subscribe(-790448070, OnDeconstructDelegate);
		if (constructionElements == null || constructionElements.Length == 0)
		{
			constructionElements = new Tag[1];
			constructionElements[0] = GetComponent<PrimaryElement>().Element.tag;
		}
		if (isMarkedForDeconstruction)
		{
			QueueDeconstruction();
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		progressBar.barColor = ProgressBarsConfig.Instance.GetBarColor("DeconstructBar");
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction);
		Trigger(1830962028, this);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Building component = GetComponent<Building>();
		SimCellOccupier component2 = GetComponent<SimCellOccupier>();
		if (DetailsScreen.Instance != null && DetailsScreen.Instance.CompareTargetWith(base.gameObject))
		{
			DetailsScreen.Instance.Show(show: false);
		}
		PrimaryElement component3 = GetComponent<PrimaryElement>();
		float temperature = component3.Temperature;
		byte disease_idx = component3.DiseaseIdx;
		int disease_count = component3.DiseaseCount;
		if (component2 != null)
		{
			if (component.Def.TileLayer != ObjectLayer.NumLayers)
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				if (Grid.Objects[num, (int)component.Def.TileLayer] == base.gameObject)
				{
					Grid.Objects[num, (int)component.Def.ObjectLayer] = null;
					Grid.Objects[num, (int)component.Def.TileLayer] = null;
					Grid.Foundation[num] = false;
					TileVisualizer.RefreshCell(num, component.Def.TileLayer, component.Def.ReplacementLayer);
				}
			}
			component2.DestroySelf(delegate
			{
				TriggerDestroy(temperature, disease_idx, disease_count);
			});
		}
		else
		{
			TriggerDestroy(temperature, disease_idx, disease_count);
		}
		if (component == null || component.Def.PlayConstructionSounds)
		{
			string sound = GlobalAssets.GetSound("Finish_Deconstruction_" + ((!audioSize.IsNullOrWhiteSpace()) ? audioSize : component.Def.AudioSize));
			if (sound != null)
			{
				KMonoBehaviour.PlaySound3DAtLocation(sound, base.gameObject.transform.GetPosition());
			}
		}
		Trigger(-702296337, this);
	}

	private void TriggerDestroy(float temperature, byte disease_idx, int disease_count)
	{
		if (!(this == null) && !destroyed)
		{
			SpawnItemsFromConstruction(temperature, disease_idx, disease_count);
			destroyed = true;
			base.gameObject.DeleteObject();
		}
	}

	private void QueueDeconstruction()
	{
		if (DebugHandler.InstantBuildMode)
		{
			OnCompleteWork(null);
		}
		else
		{
			if (chore != null)
			{
				return;
			}
			BuildingComplete component = GetComponent<BuildingComplete>();
			if (component != null && component.Def.ReplacementLayer != ObjectLayer.NumLayers)
			{
				int cell = Grid.PosToCell(component);
				if (Grid.Objects[cell, (int)component.Def.ReplacementLayer] != null)
				{
					return;
				}
			}
			Prioritizable.AddRef(base.gameObject);
			chore = new WorkChore<Deconstructable>(Db.Get().ChoreTypes.Deconstruct, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false, null, is_preemptable: true, allow_in_context_menu: true, allow_prioritization: true, PriorityScreen.PriorityClass.basic, 5, ignore_building_assignment: true);
			GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, this);
			isMarkedForDeconstruction = true;
			Trigger(2108245096, "Deconstruct");
		}
	}

	private void OnDeconstruct()
	{
		if (chore == null)
		{
			QueueDeconstruction();
		}
		else
		{
			CancelDeconstruction();
		}
	}

	public bool IsMarkedForDeconstruction()
	{
		return chore != null;
	}

	public void SetAllowDeconstruction(bool allow)
	{
		allowDeconstruction = allow;
		if (!allowDeconstruction)
		{
			CancelDeconstruction();
		}
	}

	public void SpawnItemsFromConstruction()
	{
		PrimaryElement component = GetComponent<PrimaryElement>();
		float temperature = component.Temperature;
		byte diseaseIdx = component.DiseaseIdx;
		int diseaseCount = component.DiseaseCount;
		SpawnItemsFromConstruction(temperature, diseaseIdx, diseaseCount);
	}

	private void SpawnItemsFromConstruction(float temperature, byte disease_idx, int disease_count)
	{
		Building component = GetComponent<Building>();
		float[] array = ((!(component != null)) ? new float[1]
		{
			GetComponent<PrimaryElement>().Mass
		} : component.Def.Mass);
		for (int i = 0; i < constructionElements.Length && array.Length > i; i++)
		{
			GameObject gameObject = SpawnItem(base.transform.GetPosition(), constructionElements[i], array[i], temperature, disease_idx, disease_count);
			int num = Grid.PosToCell(gameObject.transform.GetPosition());
			int num2 = Grid.CellAbove(num);
			Vector2 initial_velocity = (((Grid.IsValidCell(num) && Grid.Solid[num]) || (Grid.IsValidCell(num2) && Grid.Solid[num2])) ? Vector2.zero : new Vector2(Random.Range(-1f, 1f) * INITIAL_VELOCITY_RANGE.x, INITIAL_VELOCITY_RANGE.y));
			if (GameComps.Fallers.Has(gameObject))
			{
				GameComps.Fallers.Remove(gameObject);
			}
			GameComps.Fallers.Add(gameObject, initial_velocity);
		}
	}

	public GameObject SpawnItem(Vector3 position, Tag src_element, float src_mass, float src_temperature, byte disease_idx, int disease_count)
	{
		GameObject gameObject = null;
		int cell = Grid.PosToCell(position);
		CellOffset[] placementOffsets = this.placementOffsets;
		Element element = ElementLoader.GetElement(src_element);
		if (element != null)
		{
			float num = src_mass;
			for (int i = 0; (float)i < src_mass / 400f; i++)
			{
				int num2 = i % placementOffsets.Length;
				int cell2 = Grid.OffsetCell(cell, placementOffsets[num2]);
				float mass = num;
				if (num > 400f)
				{
					mass = 400f;
					num -= 400f;
				}
				gameObject = element.substance.SpawnResource(Grid.CellToPosCBC(cell2, Grid.SceneLayer.Ore), mass, src_temperature, disease_idx, disease_count);
			}
		}
		else
		{
			for (int j = 0; (float)j < src_mass; j++)
			{
				int num3 = j % placementOffsets.Length;
				int cell3 = Grid.OffsetCell(cell, placementOffsets[num3]);
				gameObject = GameUtil.KInstantiate(Assets.GetPrefab(src_element), Grid.CellToPosCBC(cell3, Grid.SceneLayer.Ore), Grid.SceneLayer.Ore);
				gameObject.SetActive(value: true);
			}
		}
		return gameObject;
	}

	private void OnRefreshUserMenu(object data)
	{
		if (allowDeconstruction)
		{
			KIconButtonMenu.ButtonInfo button = ((chore == null) ? new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.DECONSTRUCT.NAME, OnDeconstruct, Action.NumActions, null, null, null, UI.USERMENUACTIONS.DECONSTRUCT.TOOLTIP) : new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.DECONSTRUCT.NAME_OFF, OnDeconstruct, Action.NumActions, null, null, null, UI.USERMENUACTIONS.DECONSTRUCT.TOOLTIP_OFF));
			Game.Instance.userMenu.AddButton(base.gameObject, button, 0f);
		}
	}

	public void CancelDeconstruction()
	{
		if (chore != null)
		{
			chore.Cancel("Cancelled deconstruction");
			chore = null;
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction);
			ShowProgressBar(show: false);
			isMarkedForDeconstruction = false;
			Prioritizable.RemoveRef(base.gameObject);
		}
	}

	private void OnCancel(object data)
	{
		CancelDeconstruction();
	}

	private void OnDeconstruct(object data)
	{
		if (allowDeconstruction || DebugHandler.InstantBuildMode)
		{
			QueueDeconstruction();
		}
	}
}
