using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class BuildingChoresPanel : TargetScreen
{
	public class DupeEntryData : IComparable<DupeEntryData>
	{
		public ChoreConsumer consumer;

		public Chore.Precondition.Context context;

		public int personalPriority;

		public int rank;

		public int CompareTo(DupeEntryData other)
		{
			if (personalPriority != other.personalPriority)
			{
				return other.personalPriority.CompareTo(personalPriority);
			}
			if (rank != other.rank)
			{
				return rank.CompareTo(other.rank);
			}
			if (consumer.GetProperName() != other.consumer.GetProperName())
			{
				return consumer.GetProperName().CompareTo(other.consumer.GetProperName());
			}
			return consumer.GetInstanceID().CompareTo(other.consumer.GetInstanceID());
		}
	}

	public GameObject choreGroupPrefab;

	public GameObject chorePrefab;

	public BuildingChoresPanelDupeRow dupePrefab;

	private GameObject detailsPanel;

	private DetailsPanelDrawer drawer;

	private HierarchyReferences choreGroup;

	private List<HierarchyReferences> choreEntries = new List<HierarchyReferences>();

	private int activeChoreEntries = 0;

	private List<BuildingChoresPanelDupeRow> dupeEntries = new List<BuildingChoresPanelDupeRow>();

	private int activeDupeEntries = 0;

	private List<DupeEntryData> DupeEntryDatas = new List<DupeEntryData>();

	public override bool IsValidForTarget(GameObject target)
	{
		KPrefabID component = target.GetComponent<KPrefabID>();
		return component != null && component.HasTag(GameTags.HasChores) && !component.HasTag(GameTags.Minion);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		choreGroup = Util.KInstantiateUI<HierarchyReferences>(choreGroupPrefab, base.gameObject);
		choreGroup.gameObject.SetActive(value: true);
	}

	private void Update()
	{
		Refresh();
	}

	public override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		Refresh();
	}

	public override void OnDeselectTarget(GameObject target)
	{
		base.OnDeselectTarget(target);
	}

	private void Refresh()
	{
		RefreshDetails();
	}

	private void RefreshDetails()
	{
		List<Chore> chores = GlobalChoreProvider.Instance.chores;
		foreach (Chore item in chores)
		{
			if (!item.isNull && item.gameObject == selectedTarget)
			{
				AddChoreEntry(item);
			}
		}
		for (int i = activeDupeEntries; i < dupeEntries.Count; i++)
		{
			dupeEntries[i].gameObject.SetActive(value: false);
		}
		activeDupeEntries = 0;
		for (int j = activeChoreEntries; j < choreEntries.Count; j++)
		{
			choreEntries[j].gameObject.SetActive(value: false);
		}
		activeChoreEntries = 0;
	}

	private void AddChoreEntry(Chore chore)
	{
		HierarchyReferences choreEntry = GetChoreEntry(GameUtil.GetChoreName(chore, null), chore.choreType, choreGroup.GetReference<RectTransform>("EntriesContainer"));
		FetchChore fetchChore = chore as FetchChore;
		ListPool<Chore.Precondition.Context, BuildingChoresPanel>.PooledList pooledList = ListPool<Chore.Precondition.Context, BuildingChoresPanel>.Allocate();
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			pooledList.Clear();
			ChoreConsumer component = item.GetComponent<ChoreConsumer>();
			Chore.Precondition.Context context = default(Chore.Precondition.Context);
			ChoreConsumer.PreconditionSnapshot lastPreconditionSnapshot = component.GetLastPreconditionSnapshot();
			if (lastPreconditionSnapshot.doFailedContextsNeedSorting)
			{
				lastPreconditionSnapshot.failedContexts.Sort();
				lastPreconditionSnapshot.doFailedContextsNeedSorting = false;
			}
			pooledList.AddRange(lastPreconditionSnapshot.failedContexts);
			pooledList.AddRange(lastPreconditionSnapshot.succeededContexts);
			int num = -1;
			int num2 = 0;
			for (int num3 = pooledList.Count - 1; num3 >= 0; num3--)
			{
				if (!(pooledList[num3].chore.driver != null) || !(pooledList[num3].chore.driver != component.choreDriver))
				{
					bool flag = pooledList[num3].IsPotentialSuccess();
					if (flag)
					{
						num2++;
					}
					FetchAreaChore fetchAreaChore = pooledList[num3].chore as FetchAreaChore;
					if (pooledList[num3].chore == chore || (fetchChore != null && fetchAreaChore != null && fetchAreaChore.smi.SameDestination(fetchChore)))
					{
						num = (flag ? num2 : int.MaxValue);
						context = pooledList[num3];
						break;
					}
				}
			}
			if (num >= 0)
			{
				DupeEntryDatas.Add(new DupeEntryData
				{
					consumer = component,
					context = context,
					personalPriority = component.GetPersonalPriority(chore.choreType),
					rank = num
				});
			}
		}
		pooledList.Recycle();
		DupeEntryDatas.Sort();
		foreach (DupeEntryData dupeEntryData in DupeEntryDatas)
		{
			GetDupeEntry(dupeEntryData, choreEntry.GetReference<RectTransform>("DupeContainer"));
		}
		DupeEntryDatas.Clear();
	}

	private HierarchyReferences GetChoreEntry(string label, ChoreType choreType, RectTransform parent)
	{
		HierarchyReferences hierarchyReferences;
		if (activeChoreEntries >= choreEntries.Count)
		{
			hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(chorePrefab, parent.gameObject);
			choreEntries.Add(hierarchyReferences);
		}
		else
		{
			hierarchyReferences = choreEntries[activeChoreEntries];
			hierarchyReferences.transform.SetParent(parent);
			hierarchyReferences.transform.SetAsLastSibling();
		}
		activeChoreEntries++;
		hierarchyReferences.GetReference<LocText>("ChoreLabel").text = label;
		hierarchyReferences.GetReference<LocText>("ChoreSubLabel").text = GameUtil.ChoreGroupsForChoreType(choreType);
		Image reference = hierarchyReferences.GetReference<Image>("Icon");
		if (choreType.groups.Length != 0)
		{
			Sprite sprite2 = (reference.sprite = Assets.GetSprite(choreType.groups[0].sprite));
			reference.gameObject.SetActive(value: true);
			reference.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.BUILDING_CHORES.CHORE_TYPE_TOOLTIP, choreType.groups[0].Name);
		}
		else
		{
			reference.gameObject.SetActive(value: false);
		}
		Image reference2 = hierarchyReferences.GetReference<Image>("Icon2");
		if (choreType.groups.Length > 1)
		{
			Sprite sprite4 = (reference2.sprite = Assets.GetSprite(choreType.groups[1].sprite));
			reference2.gameObject.SetActive(value: true);
			reference2.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.BUILDING_CHORES.CHORE_TYPE_TOOLTIP, choreType.groups[1].Name);
		}
		else
		{
			reference2.gameObject.SetActive(value: false);
		}
		hierarchyReferences.gameObject.SetActive(value: true);
		return hierarchyReferences;
	}

	private BuildingChoresPanelDupeRow GetDupeEntry(DupeEntryData data, RectTransform parent)
	{
		BuildingChoresPanelDupeRow buildingChoresPanelDupeRow;
		if (activeDupeEntries >= dupeEntries.Count)
		{
			buildingChoresPanelDupeRow = Util.KInstantiateUI<BuildingChoresPanelDupeRow>(dupePrefab.gameObject, parent.gameObject);
			dupeEntries.Add(buildingChoresPanelDupeRow);
		}
		else
		{
			buildingChoresPanelDupeRow = dupeEntries[activeDupeEntries];
			buildingChoresPanelDupeRow.transform.SetParent(parent);
			buildingChoresPanelDupeRow.transform.SetAsLastSibling();
		}
		activeDupeEntries++;
		buildingChoresPanelDupeRow.Init(data);
		buildingChoresPanelDupeRow.gameObject.SetActive(value: true);
		return buildingChoresPanelDupeRow;
	}
}
