using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class PlantablePlot : SingleEntityReceptacle, ISaveLoadable, IGameObjectEffectDescriptor
{
	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	[Serialize]
	private Ref<KPrefabID> plantRef;

	public Vector3 occupyingObjectVisualOffset = Vector3.zero;

	public Grid.SceneLayer plantLayer = Grid.SceneLayer.BuildingBack;

	private EntityPreview plantPreview;

	[SerializeField]
	private bool accepts_fertilizer;

	[SerializeField]
	private bool accepts_irrigation = true;

	[SerializeField]
	public bool has_liquid_pipe_input;

	private static readonly EventSystem.IntraObjectHandler<PlantablePlot> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<PlantablePlot>(delegate(PlantablePlot component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<PlantablePlot> OnUpdateRoomDelegate = new EventSystem.IntraObjectHandler<PlantablePlot>(delegate(PlantablePlot component, object data)
	{
		if (component.plantRef.Get() != null)
		{
			component.plantRef.Get().Trigger(144050788, data);
		}
	});

	public KPrefabID plant
	{
		get
		{
			return plantRef.Get();
		}
		set
		{
			plantRef.Set(value);
		}
	}

	public bool ValidPlant
	{
		get
		{
			if (!(plantPreview == null))
			{
				return plantPreview.Valid;
			}
			return true;
		}
	}

	public bool AcceptsFertilizer => accepts_fertilizer;

	public bool AcceptsIrrigation => accepts_irrigation;

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (!DlcManager.FeaturePlantMutationsEnabled())
		{
			requestedEntityAdditionalFilterTag = Tag.Invalid;
		}
		else if (requestedEntityTag.IsValid && requestedEntityAdditionalFilterTag.IsValid && !PlantSubSpeciesCatalog.Instance.IsValidPlantableSeed(requestedEntityTag, requestedEntityAdditionalFilterTag))
		{
			requestedEntityAdditionalFilterTag = Tag.Invalid;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		statusItemNeed = Db.Get().BuildingStatusItems.NeedSeed;
		statusItemNoneAvailable = Db.Get().BuildingStatusItems.NoAvailableSeed;
		statusItemAwaitingDelivery = Db.Get().BuildingStatusItems.AwaitingSeedDelivery;
		plantRef = new Ref<KPrefabID>();
		Subscribe(-905833192, OnCopySettingsDelegate);
		Subscribe(144050788, OnUpdateRoomDelegate);
		if (this.HasTag(GameTags.FarmTiles))
		{
			storage.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
			DropAllWorkable component = GetComponent<DropAllWorkable>();
			if (component != null)
			{
				component.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
			}
			Toggleable component2 = GetComponent<Toggleable>();
			if (component2 != null)
			{
				component2.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
			}
		}
	}

	private void OnCopySettings(object data)
	{
		PlantablePlot component = ((GameObject)data).GetComponent<PlantablePlot>();
		if (!(component != null))
		{
			return;
		}
		if (base.occupyingObject == null && (requestedEntityTag != component.requestedEntityTag || component.occupyingObject != null))
		{
			Tag entityTag = component.requestedEntityTag;
			Tag requestedEntityAdditionalFilterTag = component.requestedEntityAdditionalFilterTag;
			if (component.occupyingObject != null)
			{
				SeedProducer component2 = component.occupyingObject.GetComponent<SeedProducer>();
				if (component2 != null)
				{
					entityTag = TagManager.Create(component2.seedInfo.seedId);
				}
			}
			CancelActiveRequest();
			CreateOrder(entityTag, requestedEntityAdditionalFilterTag);
		}
		if (!(base.occupyingObject != null))
		{
			return;
		}
		Prioritizable component3 = GetComponent<Prioritizable>();
		if (component3 != null)
		{
			Prioritizable component4 = base.occupyingObject.GetComponent<Prioritizable>();
			if (component4 != null)
			{
				component4.SetMasterPriority(component3.GetMasterPriority());
			}
		}
	}

	public override void CreateOrder(Tag entityTag, Tag additionalFilterTag)
	{
		SetPreview(entityTag);
		if (ValidPlant)
		{
			base.CreateOrder(entityTag, additionalFilterTag);
		}
		else
		{
			SetPreview(Tag.Invalid);
		}
	}

	private void SyncPriority(PrioritySetting priority)
	{
		Prioritizable component = GetComponent<Prioritizable>();
		if (!object.Equals(component.GetMasterPriority(), priority))
		{
			component.SetMasterPriority(priority);
		}
		if (base.occupyingObject != null)
		{
			Prioritizable component2 = base.occupyingObject.GetComponent<Prioritizable>();
			if (component2 != null && !object.Equals(component2.GetMasterPriority(), priority))
			{
				component2.SetMasterPriority(component.GetMasterPriority());
			}
		}
	}

	protected override void OnSpawn()
	{
		if (plant != null)
		{
			RegisterWithPlant(plant.gameObject);
		}
		base.OnSpawn();
		autoReplaceEntity = false;
		Components.PlantablePlots.Add(this);
		Prioritizable component = GetComponent<Prioritizable>();
		component.onPriorityChanged = (Action<PrioritySetting>)Delegate.Combine(component.onPriorityChanged, new Action<PrioritySetting>(SyncPriority));
	}

	public void SetFertilizationFlags(bool fertilizer, bool liquid_piping)
	{
		accepts_fertilizer = fertilizer;
		has_liquid_pipe_input = liquid_piping;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (plantPreview != null)
		{
			Util.KDestroyGameObject(plantPreview.gameObject);
		}
		if ((bool)base.occupyingObject)
		{
			base.occupyingObject.Trigger(-216549700);
		}
		Components.PlantablePlots.Remove(this);
	}

	protected override GameObject SpawnOccupyingObject(GameObject depositedEntity)
	{
		PlantableSeed component = depositedEntity.GetComponent<PlantableSeed>();
		if (component != null)
		{
			Vector3 position = Grid.CellToPosCBC(Grid.PosToCell(this), plantLayer);
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(component.PlantID), position, plantLayer);
			MutantPlant component2 = gameObject.GetComponent<MutantPlant>();
			if (component2 != null)
			{
				component.GetComponent<MutantPlant>().CopyMutationsTo(component2);
			}
			gameObject.SetActive(value: true);
			destroyEntityOnDeposit = true;
			return gameObject;
		}
		destroyEntityOnDeposit = false;
		return depositedEntity;
	}

	protected override void ConfigureOccupyingObject(GameObject newPlant)
	{
		KPrefabID component = newPlant.GetComponent<KPrefabID>();
		plantRef.Set(component);
		RegisterWithPlant(newPlant);
		UprootedMonitor component2 = newPlant.GetComponent<UprootedMonitor>();
		if ((bool)component2)
		{
			component2.canBeUprooted = false;
		}
		autoReplaceEntity = false;
		Prioritizable component3 = GetComponent<Prioritizable>();
		if (component3 != null)
		{
			Prioritizable component4 = newPlant.GetComponent<Prioritizable>();
			if (component4 != null)
			{
				component4.SetMasterPriority(component3.GetMasterPriority());
				component4.onPriorityChanged = (Action<PrioritySetting>)Delegate.Combine(component4.onPriorityChanged, new Action<PrioritySetting>(SyncPriority));
			}
		}
	}

	public void ReplacePlant(GameObject plant, bool keepStorage)
	{
		if (keepStorage)
		{
			UnsubscribeFromOccupant();
			base.occupyingObject = null;
		}
		ForceDeposit(plant);
	}

	protected override void PositionOccupyingObject()
	{
		base.PositionOccupyingObject();
		KBatchedAnimController component = base.occupyingObject.GetComponent<KBatchedAnimController>();
		component.SetSceneLayer(plantLayer);
		OffsetAnim(component, occupyingObjectVisualOffset);
	}

	private void RegisterWithPlant(GameObject plant)
	{
		base.occupyingObject = plant;
		ReceptacleMonitor component = plant.GetComponent<ReceptacleMonitor>();
		if ((bool)component)
		{
			component.SetReceptacle(this);
		}
		plant.Trigger(1309017699, storage);
	}

	protected override void SubscribeToOccupant()
	{
		base.SubscribeToOccupant();
		if (base.occupyingObject != null)
		{
			Subscribe(base.occupyingObject, -216549700, OnOccupantUprooted);
		}
	}

	protected override void UnsubscribeFromOccupant()
	{
		base.UnsubscribeFromOccupant();
		if (base.occupyingObject != null)
		{
			Unsubscribe(base.occupyingObject, -216549700, OnOccupantUprooted);
		}
	}

	private void OnOccupantUprooted(object data)
	{
		autoReplaceEntity = false;
		requestedEntityTag = Tag.Invalid;
	}

	public override void OrderRemoveOccupant()
	{
		if (!(base.Occupant == null))
		{
			Uprootable component = base.Occupant.GetComponent<Uprootable>();
			if (!(component == null))
			{
				component.MarkForUproot();
			}
		}
	}

	public override void SetPreview(Tag entityTag, bool solid = false)
	{
		PlantableSeed plantableSeed = null;
		if (entityTag.IsValid)
		{
			GameObject prefab = Assets.GetPrefab(entityTag);
			if (prefab == null)
			{
				DebugUtil.LogWarningArgs(base.gameObject, "Planter tried previewing a tag with no asset! If this was the 'Empty' tag, ignore it, that will go away in new save games. Otherwise... Eh? Tag was: ", entityTag);
				return;
			}
			plantableSeed = prefab.GetComponent<PlantableSeed>();
		}
		if (plantPreview != null)
		{
			KPrefabID component = plantPreview.GetComponent<KPrefabID>();
			if (plantableSeed != null && component != null && component.PrefabTag == plantableSeed.PreviewID)
			{
				return;
			}
			plantPreview.gameObject.Unsubscribe(-1820564715, OnValidChanged);
			Util.KDestroyGameObject(plantPreview.gameObject);
		}
		if (!(plantableSeed != null))
		{
			return;
		}
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(plantableSeed.PreviewID), Grid.SceneLayer.Front);
		plantPreview = gameObject.GetComponent<EntityPreview>();
		gameObject.transform.SetPosition(Vector3.zero);
		gameObject.transform.SetParent(base.gameObject.transform, worldPositionStays: false);
		gameObject.transform.SetLocalPosition(Vector3.zero);
		if (rotatable != null)
		{
			if (plantableSeed.direction == ReceptacleDirection.Top)
			{
				gameObject.transform.SetLocalPosition(occupyingObjectRelativePosition);
			}
			else if (plantableSeed.direction == ReceptacleDirection.Side)
			{
				gameObject.transform.SetLocalPosition(Rotatable.GetRotatedOffset(occupyingObjectRelativePosition, Orientation.R90));
			}
			else
			{
				gameObject.transform.SetLocalPosition(Rotatable.GetRotatedOffset(occupyingObjectRelativePosition, Orientation.R180));
			}
		}
		else
		{
			gameObject.transform.SetLocalPosition(occupyingObjectRelativePosition);
		}
		KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
		OffsetAnim(component2, occupyingObjectVisualOffset);
		gameObject.SetActive(value: true);
		gameObject.Subscribe(-1820564715, OnValidChanged);
		if (solid)
		{
			plantPreview.SetSolid();
		}
		plantPreview.UpdateValidity();
	}

	private void OffsetAnim(KBatchedAnimController kanim, Vector3 offset)
	{
		if (rotatable != null)
		{
			offset = rotatable.GetRotatedOffset(offset);
		}
		kanim.Offset = offset;
	}

	private void OnValidChanged(object obj)
	{
		Trigger(-1820564715, obj);
		if (!plantPreview.Valid && base.GetActiveRequest != null)
		{
			CancelActiveRequest();
		}
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(UI.BUILDINGEFFECTS.ENABLESDOMESTICGROWTH, UI.BUILDINGEFFECTS.TOOLTIPS.ENABLESDOMESTICGROWTH);
		list.Add(item);
		return list;
	}
}
