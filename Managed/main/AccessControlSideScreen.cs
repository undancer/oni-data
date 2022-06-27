using System;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class AccessControlSideScreen : SideScreenContent
{
	private static class MinionIdentitySort
	{
		public class SortInfo : IListableOption
		{
			public LocString name;

			public Comparison<MinionAssignablesProxy> compare;

			public string GetProperName()
			{
				return name;
			}
		}

		public static readonly SortInfo[] SortInfos = new SortInfo[2]
		{
			new SortInfo
			{
				name = UI.MINION_IDENTITY_SORT.NAME,
				compare = CompareByName
			},
			new SortInfo
			{
				name = UI.MINION_IDENTITY_SORT.ROLE,
				compare = CompareByRole
			}
		};

		public static int CompareByName(MinionAssignablesProxy a, MinionAssignablesProxy b)
		{
			return a.GetProperName().CompareTo(b.GetProperName());
		}

		public static int CompareByRole(MinionAssignablesProxy a, MinionAssignablesProxy b)
		{
			Debug.Assert(a, "a was null");
			Debug.Assert(b, "b was null");
			GameObject targetGameObject = a.GetTargetGameObject();
			GameObject targetGameObject2 = b.GetTargetGameObject();
			MinionResume minionResume = (targetGameObject ? targetGameObject.GetComponent<MinionResume>() : null);
			MinionResume minionResume2 = (targetGameObject2 ? targetGameObject2.GetComponent<MinionResume>() : null);
			if (minionResume2 == null)
			{
				return 1;
			}
			if (minionResume == null)
			{
				return -1;
			}
			int num = minionResume.CurrentRole.CompareTo(minionResume2.CurrentRole);
			if (num != 0)
			{
				return num;
			}
			return CompareByName(a, b);
		}
	}

	[SerializeField]
	private AccessControlSideScreenRow rowPrefab;

	[SerializeField]
	private GameObject rowGroup;

	[SerializeField]
	private AccessControlSideScreenDoor defaultsRow;

	[SerializeField]
	private Toggle sortByNameToggle;

	[SerializeField]
	private Toggle sortByPermissionToggle;

	[SerializeField]
	private Toggle sortByRoleToggle;

	[SerializeField]
	private GameObject disabledOverlay;

	[SerializeField]
	private KImage headerBG;

	private AccessControl target;

	private Door doorTarget;

	private UIPool<AccessControlSideScreenRow> rowPool;

	private MinionIdentitySort.SortInfo sortInfo = MinionIdentitySort.SortInfos[0];

	private Dictionary<MinionAssignablesProxy, AccessControlSideScreenRow> identityRowMap = new Dictionary<MinionAssignablesProxy, AccessControlSideScreenRow>();

	private List<MinionAssignablesProxy> identityList = new List<MinionAssignablesProxy>();

	public override string GetTitle()
	{
		if (target != null)
		{
			return string.Format(base.GetTitle(), target.GetProperName());
		}
		return base.GetTitle();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		sortByNameToggle.onValueChanged.AddListener(delegate(bool reverse_sort)
		{
			SortEntries(reverse_sort, MinionIdentitySort.CompareByName);
		});
		sortByRoleToggle.onValueChanged.AddListener(delegate(bool reverse_sort)
		{
			SortEntries(reverse_sort, MinionIdentitySort.CompareByRole);
		});
		sortByPermissionToggle.onValueChanged.AddListener(SortByPermission);
	}

	public override bool IsValidForTarget(GameObject target)
	{
		if (target.GetComponent<AccessControl>() != null)
		{
			return target.GetComponent<AccessControl>().controlEnabled;
		}
		return false;
	}

	public override void SetTarget(GameObject target)
	{
		if (this.target != null)
		{
			ClearTarget();
		}
		this.target = target.GetComponent<AccessControl>();
		doorTarget = target.GetComponent<Door>();
		if (!(this.target == null))
		{
			target.Subscribe(1734268753, OnDoorStateChanged);
			target.Subscribe(-1525636549, OnAccessControlChanged);
			if (rowPool == null)
			{
				rowPool = new UIPool<AccessControlSideScreenRow>(rowPrefab);
			}
			base.gameObject.SetActive(value: true);
			identityList = new List<MinionAssignablesProxy>(Components.MinionAssignablesProxy.Items);
			Refresh(identityList, rebuild: true);
		}
	}

	public override void ClearTarget()
	{
		base.ClearTarget();
		if (target != null)
		{
			target.Unsubscribe(1734268753, OnDoorStateChanged);
			target.Unsubscribe(-1525636549, OnAccessControlChanged);
		}
	}

	private void Refresh(List<MinionAssignablesProxy> identities, bool rebuild)
	{
		Rotatable component = target.GetComponent<Rotatable>();
		bool rotated = component != null && component.IsRotated;
		defaultsRow.SetRotated(rotated);
		defaultsRow.SetContent(target.DefaultPermission, OnDefaultPermissionChanged);
		if (rebuild)
		{
			ClearContent();
		}
		foreach (MinionAssignablesProxy identity in identities)
		{
			AccessControlSideScreenRow accessControlSideScreenRow;
			if (rebuild)
			{
				accessControlSideScreenRow = rowPool.GetFreeElement(rowGroup, forceActive: true);
				identityRowMap.Add(identity, accessControlSideScreenRow);
			}
			else
			{
				accessControlSideScreenRow = identityRowMap[identity];
			}
			AccessControl.Permission setPermission = target.GetSetPermission(identity);
			bool isDefault = target.IsDefaultPermission(identity);
			accessControlSideScreenRow.SetRotated(rotated);
			accessControlSideScreenRow.SetMinionContent(identity, setPermission, isDefault, OnPermissionChanged, OnPermissionDefault);
		}
		RefreshOnline();
		ContentContainer.SetActive(target.controlEnabled);
	}

	private void RefreshOnline()
	{
		bool flag = target.Online && (doorTarget == null || doorTarget.CurrentState == Door.ControlState.Auto);
		disabledOverlay.SetActive(!flag);
		headerBG.ColorState = ((!flag) ? KImage.ColorSelector.Inactive : KImage.ColorSelector.Active);
	}

	private void SortByPermission(bool state)
	{
		ExecuteSort(sortByPermissionToggle, state, (MinionAssignablesProxy identity) => (int)((!target.IsDefaultPermission(identity)) ? target.GetSetPermission(identity) : ((AccessControl.Permission)(-1))));
	}

	private void ExecuteSort<T>(Toggle toggle, bool state, Func<MinionAssignablesProxy, T> sortFunction, bool refresh = false)
	{
		toggle.GetComponent<ImageToggleState>().SetActiveState(state);
		if (!state)
		{
			return;
		}
		identityList = (state ? identityList.OrderBy(sortFunction).ToList() : identityList.OrderByDescending(sortFunction).ToList());
		if (refresh)
		{
			Refresh(identityList, rebuild: false);
			return;
		}
		for (int i = 0; i < identityList.Count; i++)
		{
			if (identityRowMap.ContainsKey(identityList[i]))
			{
				identityRowMap[identityList[i]].transform.SetSiblingIndex(i);
			}
		}
	}

	private void SortEntries(bool reverse_sort, Comparison<MinionAssignablesProxy> compare)
	{
		identityList.Sort(compare);
		if (reverse_sort)
		{
			identityList.Reverse();
		}
		for (int i = 0; i < identityList.Count; i++)
		{
			if (identityRowMap.ContainsKey(identityList[i]))
			{
				identityRowMap[identityList[i]].transform.SetSiblingIndex(i);
			}
		}
	}

	private void ClearContent()
	{
		if (rowPool != null)
		{
			rowPool.ClearAll();
		}
		identityRowMap.Clear();
	}

	private void OnDefaultPermissionChanged(MinionAssignablesProxy identity, AccessControl.Permission permission)
	{
		target.DefaultPermission = permission;
		Refresh(identityList, rebuild: false);
		foreach (MinionAssignablesProxy identity2 in identityList)
		{
			if (target.IsDefaultPermission(identity2))
			{
				target.ClearPermission(identity2);
			}
		}
	}

	private void OnPermissionChanged(MinionAssignablesProxy identity, AccessControl.Permission permission)
	{
		target.SetPermission(identity, permission);
	}

	private void OnPermissionDefault(MinionAssignablesProxy identity, bool isDefault)
	{
		if (isDefault)
		{
			target.ClearPermission(identity);
		}
		else
		{
			target.SetPermission(identity, target.DefaultPermission);
		}
		Refresh(identityList, rebuild: false);
	}

	private void OnAccessControlChanged(object data)
	{
		RefreshOnline();
	}

	private void OnDoorStateChanged(object data)
	{
		RefreshOnline();
	}

	private void OnSelectSortFunc(IListableOption role, object data)
	{
		if (role == null)
		{
			return;
		}
		MinionIdentitySort.SortInfo[] sortInfos = MinionIdentitySort.SortInfos;
		foreach (MinionIdentitySort.SortInfo sortInfo in sortInfos)
		{
			if (!(sortInfo.name == role.GetProperName()))
			{
				continue;
			}
			this.sortInfo = sortInfo;
			identityList.Sort(this.sortInfo.compare);
			for (int j = 0; j < identityList.Count; j++)
			{
				if (identityRowMap.ContainsKey(identityList[j]))
				{
					identityRowMap[identityList[j]].transform.SetSiblingIndex(j);
				}
			}
			break;
		}
	}
}
