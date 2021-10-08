using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/SingleEntityReceptacle")]
public class SingleEntityReceptacle : Workable, IRender1000ms
{
	public enum ReceptacleDirection
	{
		Top,
		Side,
		Bottom
	}

	[MyCmpGet]
	protected Operational operational;

	[MyCmpReq]
	protected Storage storage;

	[MyCmpGet]
	public Rotatable rotatable;

	protected FetchChore fetchChore;

	[Serialize]
	public bool autoReplaceEntity;

	[Serialize]
	public Tag requestedEntityTag;

	[Serialize]
	public Tag requestedEntityAdditionalFilterTag;

	[Serialize]
	protected Ref<KSelectable> occupyObjectRef = new Ref<KSelectable>();

	[SerializeField]
	private List<Tag> possibleDepositTagsList = new List<Tag>();

	[SerializeField]
	protected bool destroyEntityOnDeposit;

	[SerializeField]
	protected ReceptacleDirection direction;

	public Vector3 occupyingObjectRelativePosition = new Vector3(0f, 1f, 3f);

	protected StatusItem statusItemAwaitingDelivery;

	protected StatusItem statusItemNeed;

	protected StatusItem statusItemNoneAvailable;

	private static readonly EventSystem.IntraObjectHandler<SingleEntityReceptacle> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SingleEntityReceptacle>(delegate(SingleEntityReceptacle component, object data)
	{
		component.OnOperationalChanged(data);
	});

	public FetchChore GetActiveRequest => fetchChore;

	protected GameObject occupyingObject
	{
		get
		{
			if (occupyObjectRef.Get() != null)
			{
				return occupyObjectRef.Get().gameObject;
			}
			return null;
		}
		set
		{
			if (value == null)
			{
				occupyObjectRef.Set(null);
			}
			else
			{
				occupyObjectRef.Set(value.GetComponent<KSelectable>());
			}
		}
	}

	public GameObject Occupant => occupyingObject;

	public Tag[] possibleDepositObjectTags => possibleDepositTagsList.ToArray();

	public ReceptacleDirection Direction => direction;

	public bool HasDepositTag(Tag tag)
	{
		return possibleDepositTagsList.Contains(tag);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (occupyingObject != null)
		{
			PositionOccupyingObject();
			SubscribeToOccupant();
		}
		UpdateStatusItem();
		if (occupyingObject == null && !requestedEntityTag.IsValid)
		{
			requestedEntityAdditionalFilterTag = null;
		}
		if (occupyingObject == null && requestedEntityTag.IsValid)
		{
			CreateOrder(requestedEntityTag, requestedEntityAdditionalFilterTag);
		}
		Subscribe(-592767678, OnOperationalChangedDelegate);
	}

	public void AddDepositTag(Tag t)
	{
		possibleDepositTagsList.Add(t);
	}

	public void SetReceptacleDirection(ReceptacleDirection d)
	{
		direction = d;
	}

	public virtual void SetPreview(Tag entityTag, bool solid = false)
	{
	}

	public virtual void CreateOrder(Tag entityTag, Tag additionalFilterTag)
	{
		requestedEntityTag = entityTag;
		requestedEntityAdditionalFilterTag = additionalFilterTag;
		CreateFetchChore(requestedEntityTag, requestedEntityAdditionalFilterTag);
		SetPreview(entityTag, solid: true);
		UpdateStatusItem();
	}

	public void Render1000ms(float dt)
	{
		UpdateStatusItem();
	}

	protected void UpdateStatusItem()
	{
		KSelectable component = GetComponent<KSelectable>();
		if (Occupant != null)
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, null);
		}
		else if (fetchChore != null)
		{
			bool flag = fetchChore.fetcher != null;
			WorldContainer myWorld = this.GetMyWorld();
			if (!flag && myWorld != null)
			{
				Tag[] tags = fetchChore.tags;
				foreach (Tag tag in tags)
				{
					if (myWorld.worldInventory.GetTotalAmount(tag, includeRelatedWorlds: true) > 0f)
					{
						if (myWorld.worldInventory.GetTotalAmount(requestedEntityAdditionalFilterTag, includeRelatedWorlds: true) > 0f || requestedEntityAdditionalFilterTag == Tag.Invalid)
						{
							flag = true;
						}
						break;
					}
				}
			}
			if (flag)
			{
				component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, statusItemAwaitingDelivery);
			}
			else
			{
				component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, statusItemNoneAvailable);
			}
		}
		else
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, statusItemNeed);
		}
	}

	protected void CreateFetchChore(Tag entityTag, Tag additionalRequiredTag)
	{
		if (fetchChore == null && entityTag.IsValid && entityTag != GameTags.Empty)
		{
			fetchChore = new FetchChore(Db.Get().ChoreTypes.FarmFetch, storage, 1f, new Tag[1] { entityTag }, (!additionalRequiredTag.IsValid || !(additionalRequiredTag != GameTags.Empty)) ? null : new Tag[2] { entityTag, additionalRequiredTag }, null, null, run_until_complete: true, OnFetchComplete, delegate
			{
				UpdateStatusItem();
			}, delegate
			{
				UpdateStatusItem();
			}, FetchOrder2.OperationalRequirement.Functional);
			MaterialNeeds.UpdateNeed(requestedEntityTag, 1f, base.gameObject.GetMyWorldId());
			UpdateStatusItem();
		}
	}

	public virtual void OrderRemoveOccupant()
	{
		ClearOccupant();
	}

	protected virtual void ClearOccupant()
	{
		if ((bool)occupyingObject)
		{
			UnsubscribeFromOccupant();
			storage.DropAll();
		}
		occupyingObject = null;
		UpdateActive();
		UpdateStatusItem();
		Trigger(-731304873, occupyingObject);
	}

	public void CancelActiveRequest()
	{
		if (fetchChore != null)
		{
			MaterialNeeds.UpdateNeed(requestedEntityTag, -1f, base.gameObject.GetMyWorldId());
			fetchChore.Cancel("User canceled");
			fetchChore = null;
		}
		requestedEntityTag = Tag.Invalid;
		UpdateStatusItem();
		SetPreview(Tag.Invalid);
	}

	private void OnOccupantDestroyed(object data)
	{
		occupyingObject = null;
		ClearOccupant();
		if (autoReplaceEntity && requestedEntityTag.IsValid && requestedEntityTag != GameTags.Empty)
		{
			CreateOrder(requestedEntityTag, requestedEntityAdditionalFilterTag);
		}
	}

	protected virtual void SubscribeToOccupant()
	{
		if (occupyingObject != null)
		{
			Subscribe(occupyingObject, 1969584890, OnOccupantDestroyed);
		}
	}

	protected virtual void UnsubscribeFromOccupant()
	{
		if (occupyingObject != null)
		{
			Unsubscribe(occupyingObject, 1969584890, OnOccupantDestroyed);
		}
	}

	private void OnFetchComplete(Chore chore)
	{
		if (fetchChore == null)
		{
			Debug.LogWarningFormat(base.gameObject, "{0} OnFetchComplete fetchChore null", base.gameObject);
		}
		else if (fetchChore.fetchTarget == null)
		{
			Debug.LogWarningFormat(base.gameObject, "{0} OnFetchComplete fetchChore.fetchTarget null", base.gameObject);
		}
		else
		{
			OnDepositObject(fetchChore.fetchTarget.gameObject);
		}
	}

	public void ForceDeposit(GameObject depositedObject)
	{
		if (occupyingObject != null)
		{
			ClearOccupant();
		}
		OnDepositObject(depositedObject);
	}

	private void OnDepositObject(GameObject depositedObject)
	{
		SetPreview(Tag.Invalid);
		MaterialNeeds.UpdateNeed(requestedEntityTag, -1f, base.gameObject.GetMyWorldId());
		KBatchedAnimController component = depositedObject.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.GetBatchInstanceData().ClearOverrideTransformMatrix();
		}
		occupyingObject = SpawnOccupyingObject(depositedObject);
		if (occupyingObject != null)
		{
			ConfigureOccupyingObject(occupyingObject);
			occupyingObject.SetActive(value: true);
			PositionOccupyingObject();
			SubscribeToOccupant();
		}
		else
		{
			Debug.LogWarning(base.gameObject.name + " EntityReceptacle did not spawn occupying entity.");
		}
		if (fetchChore != null)
		{
			fetchChore.Cancel("receptacle filled");
			fetchChore = null;
		}
		if (!autoReplaceEntity)
		{
			requestedEntityTag = Tag.Invalid;
		}
		UpdateActive();
		UpdateStatusItem();
		if (destroyEntityOnDeposit)
		{
			Util.KDestroyGameObject(depositedObject);
		}
		Trigger(-731304873, occupyingObject);
	}

	protected virtual GameObject SpawnOccupyingObject(GameObject depositedEntity)
	{
		return depositedEntity;
	}

	protected virtual void ConfigureOccupyingObject(GameObject source)
	{
	}

	protected virtual void PositionOccupyingObject()
	{
		if (rotatable != null)
		{
			occupyingObject.transform.SetPosition(base.gameObject.transform.GetPosition() + rotatable.GetRotatedOffset(occupyingObjectRelativePosition));
		}
		else
		{
			occupyingObject.transform.SetPosition(base.gameObject.transform.GetPosition() + occupyingObjectRelativePosition);
		}
		KBatchedAnimController component = occupyingObject.GetComponent<KBatchedAnimController>();
		component.enabled = false;
		component.enabled = true;
	}

	private void UpdateActive()
	{
		if (!Equals(null) && !(this == null) && !base.gameObject.Equals(null) && !(base.gameObject == null) && operational != null)
		{
			operational.SetActive(operational.IsOperational && occupyingObject != null);
		}
	}

	protected override void OnCleanUp()
	{
		CancelActiveRequest();
		UnsubscribeFromOccupant();
		base.OnCleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		UpdateActive();
		if ((bool)occupyingObject)
		{
			occupyingObject.Trigger(operational.IsOperational ? 1628751838 : 960378201);
		}
	}
}
