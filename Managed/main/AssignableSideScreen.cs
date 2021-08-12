using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class AssignableSideScreen : SideScreenContent
{
	[SerializeField]
	private AssignableSideScreenRow rowPrefab;

	[SerializeField]
	private GameObject rowGroup;

	[SerializeField]
	private LocText currentOwnerText;

	[SerializeField]
	private MultiToggle dupeSortingToggle;

	[SerializeField]
	private MultiToggle generalSortingToggle;

	private MultiToggle activeSortToggle;

	private Comparison<IAssignableIdentity> activeSortFunction;

	private bool sortReversed;

	private int targetAssignableSubscriptionHandle = -1;

	private UIPool<AssignableSideScreenRow> rowPool;

	private Dictionary<IAssignableIdentity, AssignableSideScreenRow> identityRowMap = new Dictionary<IAssignableIdentity, AssignableSideScreenRow>();

	private List<MinionAssignablesProxy> identityList = new List<MinionAssignablesProxy>();

	public Assignable targetAssignable { get; private set; }

	public override string GetTitle()
	{
		if (targetAssignable != null)
		{
			return string.Format(base.GetTitle(), targetAssignable.GetProperName());
		}
		return base.GetTitle();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		MultiToggle multiToggle = dupeSortingToggle;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			SortByName(reselect: true);
		});
		MultiToggle multiToggle2 = generalSortingToggle;
		multiToggle2.onClick = (System.Action)Delegate.Combine(multiToggle2.onClick, (System.Action)delegate
		{
			SortByAssignment(reselect: true);
		});
		Subscribe(Game.Instance.gameObject, 875045922, OnRefreshData);
	}

	private void OnRefreshData(object obj)
	{
		SetTarget(targetAssignable.gameObject);
	}

	public override void ClearTarget()
	{
		if (targetAssignableSubscriptionHandle != -1 && targetAssignable != null)
		{
			targetAssignable.Unsubscribe(targetAssignableSubscriptionHandle);
			targetAssignableSubscriptionHandle = -1;
		}
		targetAssignable = null;
		Components.LiveMinionIdentities.OnAdd -= OnMinionIdentitiesChanged;
		Components.LiveMinionIdentities.OnRemove -= OnMinionIdentitiesChanged;
		base.ClearTarget();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		if (target.GetComponent<Assignable>() != null && target.GetComponent<Assignable>().CanBeAssigned)
		{
			return target.GetComponent<AssignmentGroupController>() == null;
		}
		return false;
	}

	public override void SetTarget(GameObject target)
	{
		Components.LiveMinionIdentities.OnAdd += OnMinionIdentitiesChanged;
		Components.LiveMinionIdentities.OnRemove += OnMinionIdentitiesChanged;
		if (targetAssignableSubscriptionHandle != -1 && targetAssignable != null)
		{
			targetAssignable.Unsubscribe(targetAssignableSubscriptionHandle);
		}
		targetAssignable = target.GetComponent<Assignable>();
		if (targetAssignable == null)
		{
			Debug.LogError($"{target.GetProperName()} selected has no Assignable component.");
			return;
		}
		if (rowPool == null)
		{
			rowPool = new UIPool<AssignableSideScreenRow>(rowPrefab);
		}
		base.gameObject.SetActive(value: true);
		identityList = new List<MinionAssignablesProxy>(Components.MinionAssignablesProxy.Items);
		dupeSortingToggle.ChangeState(0);
		generalSortingToggle.ChangeState(0);
		activeSortToggle = null;
		activeSortFunction = null;
		if (!targetAssignable.CanBeAssigned)
		{
			HideScreen(hide: true);
		}
		else
		{
			HideScreen(hide: false);
		}
		targetAssignableSubscriptionHandle = targetAssignable.Subscribe(684616645, OnAssigneeChanged);
		Refresh(identityList);
		SortByAssignment(reselect: false);
	}

	private void OnMinionIdentitiesChanged(MinionIdentity change)
	{
		identityList = new List<MinionAssignablesProxy>(Components.MinionAssignablesProxy.Items);
		Refresh(identityList);
	}

	private void OnAssigneeChanged(object data = null)
	{
		foreach (KeyValuePair<IAssignableIdentity, AssignableSideScreenRow> item in identityRowMap)
		{
			item.Value.Refresh();
		}
	}

	private void Refresh(List<MinionAssignablesProxy> identities)
	{
		ClearContent();
		currentOwnerText.text = string.Format(UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.UNASSIGNED);
		if (targetAssignable == null)
		{
			return;
		}
		if (targetAssignable.GetComponent<Equippable>() == null && !targetAssignable.HasTag(GameTags.NotRoomAssignable))
		{
			Room room = null;
			room = Game.Instance.roomProber.GetRoomOfGameObject(targetAssignable.gameObject);
			if (room != null)
			{
				RoomType roomType = room.roomType;
				if (roomType.primary_constraint != null && !roomType.primary_constraint.building_criteria(targetAssignable.GetComponent<KPrefabID>()))
				{
					AssignableSideScreenRow freeElement = rowPool.GetFreeElement(rowGroup, forceActive: true);
					freeElement.sideScreen = this;
					identityRowMap.Add(room, freeElement);
					freeElement.SetContent(room, OnRowClicked, this);
					return;
				}
			}
		}
		if (targetAssignable.canBePublic)
		{
			AssignableSideScreenRow freeElement2 = rowPool.GetFreeElement(rowGroup, forceActive: true);
			freeElement2.sideScreen = this;
			freeElement2.transform.SetAsFirstSibling();
			identityRowMap.Add(Game.Instance.assignmentManager.assignment_groups["public"], freeElement2);
			freeElement2.SetContent(Game.Instance.assignmentManager.assignment_groups["public"], OnRowClicked, this);
		}
		foreach (MinionAssignablesProxy identity in identities)
		{
			AssignableSideScreenRow freeElement3 = rowPool.GetFreeElement(rowGroup, forceActive: true);
			freeElement3.sideScreen = this;
			identityRowMap.Add(identity, freeElement3);
			freeElement3.SetContent(identity, OnRowClicked, this);
		}
		ExecuteSort(activeSortFunction);
	}

	private void SortByName(bool reselect)
	{
		SelectSortToggle(dupeSortingToggle, reselect);
		ExecuteSort((IAssignableIdentity i1, IAssignableIdentity i2) => i1.GetProperName().CompareTo(i2.GetProperName()) * ((!sortReversed) ? 1 : (-1)));
	}

	private void SortByAssignment(bool reselect)
	{
		SelectSortToggle(generalSortingToggle, reselect);
		Comparison<IAssignableIdentity> sortFunction = delegate(IAssignableIdentity i1, IAssignableIdentity i2)
		{
			int num = 0;
			num = targetAssignable.CanAssignTo(i1).CompareTo(targetAssignable.CanAssignTo(i2));
			if (num != 0)
			{
				return num * -1;
			}
			num = identityRowMap[i1].currentState.CompareTo(identityRowMap[i2].currentState);
			return (num != 0) ? (num * ((!sortReversed) ? 1 : (-1))) : i1.GetProperName().CompareTo(i2.GetProperName());
		};
		ExecuteSort(sortFunction);
	}

	private void SelectSortToggle(MultiToggle toggle, bool reselect)
	{
		dupeSortingToggle.ChangeState(0);
		generalSortingToggle.ChangeState(0);
		if (toggle != null)
		{
			if (reselect && activeSortToggle == toggle)
			{
				sortReversed = !sortReversed;
			}
			activeSortToggle = toggle;
		}
		activeSortToggle.ChangeState((!sortReversed) ? 1 : 2);
	}

	private void ExecuteSort(Comparison<IAssignableIdentity> sortFunction)
	{
		if (sortFunction != null)
		{
			List<IAssignableIdentity> list = new List<IAssignableIdentity>(identityRowMap.Keys);
			list.Sort(sortFunction);
			for (int i = 0; i < list.Count; i++)
			{
				identityRowMap[list[i]].transform.SetSiblingIndex(i);
			}
			activeSortFunction = sortFunction;
		}
	}

	private void ClearContent()
	{
		if (rowPool != null)
		{
			rowPool.DestroyAll();
		}
		foreach (KeyValuePair<IAssignableIdentity, AssignableSideScreenRow> item in identityRowMap)
		{
			item.Value.targetIdentity = null;
		}
		identityRowMap.Clear();
	}

	private void HideScreen(bool hide)
	{
		if (hide)
		{
			base.transform.localScale = Vector3.zero;
		}
		else if (base.transform.localScale != Vector3.one)
		{
			base.transform.localScale = Vector3.one;
		}
	}

	private void OnRowClicked(IAssignableIdentity identity)
	{
		if (targetAssignable.assignee != identity)
		{
			ChangeAssignment(identity);
		}
		else if (CanDeselect(identity))
		{
			ChangeAssignment(null);
		}
	}

	private bool CanDeselect(IAssignableIdentity identity)
	{
		return identity is MinionAssignablesProxy;
	}

	private void ChangeAssignment(IAssignableIdentity new_identity)
	{
		targetAssignable.Unassign();
		if (new_identity != null)
		{
			targetAssignable.Assign(new_identity);
		}
	}

	private void OnValidStateChanged(bool state)
	{
		if (base.gameObject.activeInHierarchy)
		{
			Refresh(identityList);
		}
	}
}
