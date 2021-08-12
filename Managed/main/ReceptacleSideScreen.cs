using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceptacleSideScreen : SideScreenContent, IRender1000ms
{
	protected class SelectableEntity
	{
		public Tag tag;

		public SingleEntityReceptacle.ReceptacleDirection direction;

		public GameObject asset;

		public float lastAmount = -1f;
	}

	[SerializeField]
	protected KButton requestSelectedEntityBtn;

	[SerializeField]
	private string requestStringDeposit;

	[SerializeField]
	private string requestStringCancelDeposit;

	[SerializeField]
	private string requestStringRemove;

	[SerializeField]
	private string requestStringCancelRemove;

	public GameObject activeEntityContainer;

	public GameObject nothingDiscoveredContainer;

	[SerializeField]
	protected LocText descriptionLabel;

	protected Dictionary<SingleEntityReceptacle, int> entityPreviousSelectionMap = new Dictionary<SingleEntityReceptacle, int>();

	[SerializeField]
	private string subtitleStringSelect;

	[SerializeField]
	private string subtitleStringSelectDescription;

	[SerializeField]
	private string subtitleStringAwaitingSelection;

	[SerializeField]
	private string subtitleStringAwaitingDelivery;

	[SerializeField]
	private string subtitleStringEntityDeposited;

	[SerializeField]
	private string subtitleStringAwaitingRemoval;

	[SerializeField]
	private LocText subtitleLabel;

	[SerializeField]
	private List<DescriptorPanel> descriptorPanels;

	public Material defaultMaterial;

	public Material desaturatedMaterial;

	[SerializeField]
	private GameObject requestObjectList;

	[SerializeField]
	private GameObject requestObjectListContainer;

	[SerializeField]
	private GameObject scrollBarContainer;

	[SerializeField]
	private GameObject entityToggle;

	[SerializeField]
	private Sprite buttonSelectedBG;

	[SerializeField]
	private Sprite buttonNormalBG;

	[SerializeField]
	private Sprite elementPlaceholderSpr;

	[SerializeField]
	private bool hideUndiscoveredEntities;

	protected ReceptacleToggle selectedEntityToggle;

	protected SingleEntityReceptacle targetReceptacle;

	protected Tag selectedDepositObjectTag;

	protected Tag selectedDepositObjectAdditionalTag;

	protected Dictionary<ReceptacleToggle, SelectableEntity> depositObjectMap;

	protected List<ReceptacleToggle> entityToggles = new List<ReceptacleToggle>();

	private int onObjectDestroyedHandle = -1;

	private int onOccupantValidChangedHandle = -1;

	private int onStorageChangedHandle = -1;

	public override string GetTitle()
	{
		if (targetReceptacle == null)
		{
			return Strings.Get(titleKey).ToString().Replace("{0}", "");
		}
		return string.Format(Strings.Get(titleKey), targetReceptacle.GetProperName());
	}

	public void Initialize(SingleEntityReceptacle target)
	{
		if (target == null)
		{
			Debug.LogError("SingleObjectReceptacle provided was null.");
			return;
		}
		targetReceptacle = target;
		base.gameObject.SetActive(value: true);
		depositObjectMap = new Dictionary<ReceptacleToggle, SelectableEntity>();
		entityToggles.ForEach(delegate(ReceptacleToggle rbi)
		{
			Object.Destroy(rbi.gameObject);
		});
		entityToggles.Clear();
		Tag[] possibleDepositObjectTags = target.possibleDepositObjectTags;
		for (int i = 0; i < possibleDepositObjectTags.Length; i++)
		{
			List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(possibleDepositObjectTags[i]);
			if (targetReceptacle.rotatable == null)
			{
				prefabsWithTag.RemoveAll(delegate(GameObject go)
				{
					IReceptacleDirection component3 = go.GetComponent<IReceptacleDirection>();
					return component3 != null && component3.Direction != targetReceptacle.Direction;
				});
			}
			List<IHasSortOrder> list = new List<IHasSortOrder>();
			foreach (GameObject item in prefabsWithTag)
			{
				IHasSortOrder component = item.GetComponent<IHasSortOrder>();
				if (component != null)
				{
					list.Add(component);
				}
			}
			Debug.Assert(list.Count == prefabsWithTag.Count, "Not all entities in this receptacle implement IHasSortOrder!");
			list.Sort((IHasSortOrder a, IHasSortOrder b) => a.sortOrder - b.sortOrder);
			foreach (IHasSortOrder item2 in list)
			{
				GameObject gameObject = (item2 as MonoBehaviour).gameObject;
				GameObject gameObject2 = Util.KInstantiateUI(entityToggle, requestObjectList);
				gameObject2.SetActive(value: true);
				ReceptacleToggle newToggle = gameObject2.GetComponent<ReceptacleToggle>();
				IReceptacleDirection component2 = gameObject.GetComponent<IReceptacleDirection>();
				string properName = gameObject.GetProperName();
				newToggle.title.text = properName;
				Sprite entityIcon = GetEntityIcon(gameObject.PrefabID());
				if (entityIcon == null)
				{
					entityIcon = elementPlaceholderSpr;
				}
				newToggle.image.sprite = entityIcon;
				newToggle.toggle.onClick += delegate
				{
					ToggleClicked(newToggle);
				};
				newToggle.toggle.onPointerEnter += delegate
				{
					CheckAmountsAndUpdate(null);
				};
				depositObjectMap.Add(newToggle, new SelectableEntity
				{
					tag = gameObject.PrefabID(),
					direction = (component2?.Direction ?? SingleEntityReceptacle.ReceptacleDirection.Top),
					asset = gameObject
				});
				entityToggles.Add(newToggle);
			}
		}
		RestoreSelectionFromOccupant();
		selectedEntityToggle = null;
		if (entityToggles.Count > 0)
		{
			if (entityPreviousSelectionMap.ContainsKey(targetReceptacle))
			{
				int index = entityPreviousSelectionMap[targetReceptacle];
				ToggleClicked(entityToggles[index]);
			}
			else
			{
				subtitleLabel.SetText(Strings.Get(subtitleStringSelect).ToString());
				requestSelectedEntityBtn.isInteractable = false;
				descriptionLabel.SetText(Strings.Get(subtitleStringSelectDescription).ToString());
				HideAllDescriptorPanels();
			}
		}
		onStorageChangedHandle = targetReceptacle.gameObject.Subscribe(-1697596308, CheckAmountsAndUpdate);
		onOccupantValidChangedHandle = targetReceptacle.gameObject.Subscribe(-1820564715, OnOccupantValidChanged);
		UpdateState(null);
		SimAndRenderScheduler.instance.Add(this);
	}

	protected virtual void UpdateState(object data)
	{
		requestSelectedEntityBtn.ClearOnClick();
		if (targetReceptacle == null)
		{
			return;
		}
		if (CheckReceptacleOccupied())
		{
			Uprootable uprootable = targetReceptacle.Occupant.GetComponent<Uprootable>();
			if (uprootable != null && uprootable.IsMarkedForUproot)
			{
				requestSelectedEntityBtn.onClick += delegate
				{
					uprootable.ForceCancelUproot();
					UpdateState(null);
				};
				requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(requestStringCancelRemove).ToString();
				subtitleLabel.SetText(string.Format(Strings.Get(subtitleStringAwaitingRemoval).ToString(), targetReceptacle.Occupant.GetProperName()));
			}
			else
			{
				requestSelectedEntityBtn.onClick += delegate
				{
					targetReceptacle.OrderRemoveOccupant();
					UpdateState(null);
				};
				requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(requestStringRemove).ToString();
				subtitleLabel.SetText(string.Format(Strings.Get(subtitleStringEntityDeposited).ToString(), targetReceptacle.Occupant.GetProperName()));
			}
			requestSelectedEntityBtn.isInteractable = true;
			ToggleObjectPicker(Show: false);
			Tag tag = targetReceptacle.Occupant.GetComponent<KSelectable>().PrefabID();
			ConfigureActiveEntity(tag);
			SetResultDescriptions(targetReceptacle.Occupant);
		}
		else if (targetReceptacle.GetActiveRequest != null)
		{
			requestSelectedEntityBtn.onClick += delegate
			{
				targetReceptacle.CancelActiveRequest();
				ClearSelection();
				UpdateAvailableAmounts(null);
				UpdateState(null);
			};
			requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(requestStringCancelDeposit).ToString();
			requestSelectedEntityBtn.isInteractable = true;
			ToggleObjectPicker(Show: false);
			ConfigureActiveEntity(targetReceptacle.GetActiveRequest.tags[0]);
			GameObject prefab = Assets.GetPrefab(targetReceptacle.GetActiveRequest.tags[0]);
			if (prefab != null)
			{
				subtitleLabel.SetText(string.Format(Strings.Get(subtitleStringAwaitingDelivery).ToString(), prefab.GetProperName()));
				SetResultDescriptions(prefab);
			}
		}
		else if (selectedEntityToggle != null)
		{
			requestSelectedEntityBtn.onClick += delegate
			{
				targetReceptacle.CreateOrder(selectedDepositObjectTag, selectedDepositObjectAdditionalTag);
				UpdateAvailableAmounts(null);
				UpdateState(null);
			};
			requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(requestStringDeposit).ToString();
			targetReceptacle.SetPreview(depositObjectMap[selectedEntityToggle].tag);
			bool flag = CanDepositEntity(depositObjectMap[selectedEntityToggle]);
			requestSelectedEntityBtn.isInteractable = flag;
			SetImageToggleState(selectedEntityToggle.toggle, flag ? ImageToggleState.State.Active : ImageToggleState.State.DisabledActive);
			ToggleObjectPicker(Show: true);
			GameObject prefab2 = Assets.GetPrefab(selectedDepositObjectTag);
			if (prefab2 != null)
			{
				subtitleLabel.SetText(string.Format(Strings.Get(subtitleStringAwaitingSelection).ToString(), prefab2.GetProperName()));
				SetResultDescriptions(prefab2);
			}
		}
		else
		{
			requestSelectedEntityBtn.GetComponentInChildren<LocText>().text = Strings.Get(requestStringDeposit).ToString();
			requestSelectedEntityBtn.isInteractable = false;
			ToggleObjectPicker(Show: true);
		}
		UpdateAvailableAmounts(null);
		UpdateListeners();
	}

	private void UpdateListeners()
	{
		if (CheckReceptacleOccupied())
		{
			if (onObjectDestroyedHandle == -1)
			{
				onObjectDestroyedHandle = targetReceptacle.Occupant.gameObject.Subscribe(1969584890, delegate
				{
					UpdateState(null);
				});
			}
		}
		else if (onObjectDestroyedHandle != -1)
		{
			onObjectDestroyedHandle = -1;
		}
	}

	private void OnOccupantValidChanged(object obj)
	{
		if (!(targetReceptacle == null) && !CheckReceptacleOccupied() && targetReceptacle.GetActiveRequest != null)
		{
			bool flag = false;
			if (depositObjectMap.TryGetValue(selectedEntityToggle, out var value))
			{
				flag = CanDepositEntity(value);
			}
			if (!flag)
			{
				targetReceptacle.CancelActiveRequest();
				ClearSelection();
				UpdateState(null);
				UpdateAvailableAmounts(null);
			}
		}
	}

	private bool CanDepositEntity(SelectableEntity entity)
	{
		if (ValidRotationForDeposit(entity.direction) && (!RequiresAvailableAmountToDeposit() || GetAvailableAmount(entity.tag) > 0f))
		{
			return AdditionalCanDepositTest();
		}
		return false;
	}

	protected virtual bool AdditionalCanDepositTest()
	{
		return true;
	}

	protected virtual bool RequiresAvailableAmountToDeposit()
	{
		return true;
	}

	private void ClearSelection()
	{
		foreach (KeyValuePair<ReceptacleToggle, SelectableEntity> item in depositObjectMap)
		{
			item.Key.toggle.Deselect();
		}
	}

	private void ToggleObjectPicker(bool Show)
	{
		requestObjectListContainer.SetActive(Show);
		if (scrollBarContainer != null)
		{
			scrollBarContainer.SetActive(Show);
		}
		requestObjectList.SetActive(Show);
		activeEntityContainer.SetActive(!Show);
	}

	private void ConfigureActiveEntity(Tag tag)
	{
		string properName = Assets.GetPrefab(tag).GetProperName();
		activeEntityContainer.GetComponentInChildrenOnly<LocText>().text = properName;
		activeEntityContainer.transform.GetChild(0).gameObject.GetComponentInChildrenOnly<Image>().sprite = GetEntityIcon(tag);
	}

	protected virtual Sprite GetEntityIcon(Tag prefabTag)
	{
		return Def.GetUISprite(Assets.GetPrefab(prefabTag)).first;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		if (target.GetComponent<SingleEntityReceptacle>() != null && target.GetComponent<PlantablePlot>() == null)
		{
			return target.GetComponent<EggIncubator>() == null;
		}
		return false;
	}

	public override void SetTarget(GameObject target)
	{
		SingleEntityReceptacle component = target.GetComponent<SingleEntityReceptacle>();
		if (component == null)
		{
			Debug.LogError("The object selected doesn't have a SingleObjectReceptacle!");
			return;
		}
		Initialize(component);
		UpdateState(null);
	}

	protected virtual void RestoreSelectionFromOccupant()
	{
	}

	public override void ClearTarget()
	{
		if (targetReceptacle != null)
		{
			if (CheckReceptacleOccupied())
			{
				targetReceptacle.Occupant.gameObject.Unsubscribe(onObjectDestroyedHandle);
				onObjectDestroyedHandle = -1;
			}
			targetReceptacle.Unsubscribe(onStorageChangedHandle);
			onStorageChangedHandle = -1;
			targetReceptacle.Unsubscribe(onOccupantValidChangedHandle);
			onOccupantValidChangedHandle = -1;
			if (targetReceptacle.GetActiveRequest == null)
			{
				targetReceptacle.SetPreview(Tag.Invalid);
			}
			SimAndRenderScheduler.instance.Remove(this);
			targetReceptacle = null;
		}
	}

	protected void SetImageToggleState(KToggle toggle, ImageToggleState.State state)
	{
		switch (state)
		{
		case ImageToggleState.State.Active:
			toggle.GetComponent<ImageToggleState>().SetActive();
			toggle.gameObject.GetComponentInChildrenOnly<Image>().material = defaultMaterial;
			break;
		case ImageToggleState.State.Inactive:
			toggle.GetComponent<ImageToggleState>().SetInactive();
			toggle.gameObject.GetComponentInChildrenOnly<Image>().material = defaultMaterial;
			break;
		case ImageToggleState.State.Disabled:
			toggle.GetComponent<ImageToggleState>().SetDisabled();
			toggle.gameObject.GetComponentInChildrenOnly<Image>().material = desaturatedMaterial;
			break;
		case ImageToggleState.State.DisabledActive:
			toggle.GetComponent<ImageToggleState>().SetDisabledActive();
			toggle.gameObject.GetComponentInChildrenOnly<Image>().material = desaturatedMaterial;
			break;
		}
	}

	public void Render1000ms(float dt)
	{
		CheckAmountsAndUpdate(null);
	}

	private void CheckAmountsAndUpdate(object data)
	{
		if (!(targetReceptacle == null) && UpdateAvailableAmounts(null))
		{
			UpdateState(null);
		}
	}

	private bool UpdateAvailableAmounts(object data)
	{
		bool result = false;
		foreach (KeyValuePair<ReceptacleToggle, SelectableEntity> item in depositObjectMap)
		{
			if (!DebugHandler.InstantBuildMode && hideUndiscoveredEntities && !DiscoveredResources.Instance.IsDiscovered(item.Value.tag))
			{
				item.Key.gameObject.SetActive(value: false);
			}
			else if (!item.Key.gameObject.activeSelf)
			{
				item.Key.gameObject.SetActive(value: true);
			}
			float availableAmount = GetAvailableAmount(item.Value.tag);
			if (item.Value.lastAmount != availableAmount)
			{
				result = true;
				item.Value.lastAmount = availableAmount;
				item.Key.amount.text = availableAmount.ToString();
			}
			if (!ValidRotationForDeposit(item.Value.direction) || availableAmount <= 0f)
			{
				if (selectedEntityToggle != item.Key)
				{
					SetImageToggleState(item.Key.toggle, ImageToggleState.State.Disabled);
				}
				else
				{
					SetImageToggleState(item.Key.toggle, ImageToggleState.State.DisabledActive);
				}
			}
			else if (selectedEntityToggle != item.Key)
			{
				SetImageToggleState(item.Key.toggle, ImageToggleState.State.Inactive);
			}
			else
			{
				SetImageToggleState(item.Key.toggle, ImageToggleState.State.Active);
			}
		}
		return result;
	}

	private float GetAvailableAmount(Tag tag)
	{
		return targetReceptacle.GetMyWorld().worldInventory.GetAmount(tag, includeRelatedWorlds: true);
	}

	private bool ValidRotationForDeposit(SingleEntityReceptacle.ReceptacleDirection depositDir)
	{
		if (!(targetReceptacle.rotatable == null))
		{
			return depositDir == targetReceptacle.Direction;
		}
		return true;
	}

	protected virtual void ToggleClicked(ReceptacleToggle toggle)
	{
		if (!depositObjectMap.ContainsKey(toggle))
		{
			Debug.LogError("Recipe not found on recipe list.");
			return;
		}
		if (selectedEntityToggle != null)
		{
			bool flag = CanDepositEntity(depositObjectMap[selectedEntityToggle]);
			requestSelectedEntityBtn.isInteractable = flag;
			SetImageToggleState(selectedEntityToggle.toggle, flag ? ImageToggleState.State.Inactive : ImageToggleState.State.Disabled);
		}
		selectedEntityToggle = toggle;
		entityPreviousSelectionMap[targetReceptacle] = entityToggles.IndexOf(toggle);
		selectedDepositObjectTag = depositObjectMap[toggle].tag;
		UpdateAvailableAmounts(null);
		UpdateState(null);
	}

	private void CreateOrder(bool isInfinite)
	{
		targetReceptacle.CreateOrder(selectedDepositObjectTag, selectedDepositObjectAdditionalTag);
	}

	protected bool CheckReceptacleOccupied()
	{
		if (targetReceptacle != null && targetReceptacle.Occupant != null)
		{
			return true;
		}
		return false;
	}

	protected virtual void SetResultDescriptions(GameObject go)
	{
		string text = "";
		InfoDescription component = go.GetComponent<InfoDescription>();
		if ((bool)component)
		{
			text = component.description;
		}
		else
		{
			KPrefabID component2 = go.GetComponent<KPrefabID>();
			if (component2 != null)
			{
				Element element = ElementLoader.GetElement(component2.PrefabID());
				if (element != null)
				{
					text = element.Description();
				}
			}
			else
			{
				text = go.GetProperName();
			}
		}
		descriptionLabel.SetText(text);
	}

	protected virtual void HideAllDescriptorPanels()
	{
		for (int i = 0; i < descriptorPanels.Count; i++)
		{
			descriptorPanels[i].gameObject.SetActive(value: false);
		}
	}
}
