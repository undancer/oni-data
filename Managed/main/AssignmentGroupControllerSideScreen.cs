using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class AssignmentGroupControllerSideScreen : KScreen
{
	private struct RowSortHelper
	{
		public MinionAssignablesProxy minion;

		public bool isPilot;

		public bool isSameWorld;
	}

	[SerializeField]
	private GameObject header;

	[SerializeField]
	private GameObject minionRowPrefab;

	[SerializeField]
	private GameObject footer;

	[SerializeField]
	private GameObject minionRowContainer;

	private AssignmentGroupController target;

	private List<GameObject> identityRowMap = new List<GameObject>();

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			RefreshRows();
		}
	}

	protected override void OnCmpDisable()
	{
		for (int i = 0; i < identityRowMap.Count; i++)
		{
			Object.Destroy(identityRowMap[i]);
		}
		identityRowMap.Clear();
		base.OnCmpDisable();
	}

	public void SetTarget(GameObject target)
	{
		this.target = target.GetComponent<AssignmentGroupController>();
		RefreshRows();
	}

	private void RefreshRows()
	{
		int num = 0;
		WorldContainer myWorld = target.GetMyWorld();
		ClustercraftExteriorDoor component = target.GetComponent<ClustercraftExteriorDoor>();
		if (component != null)
		{
			myWorld = component.GetInteriorDoor().GetMyWorld();
		}
		List<RowSortHelper> list = new List<RowSortHelper>();
		for (int i = 0; i < Components.MinionAssignablesProxy.Count; i++)
		{
			MinionAssignablesProxy minionAssignablesProxy = Components.MinionAssignablesProxy[i];
			GameObject targetGameObject = minionAssignablesProxy.GetTargetGameObject();
			WorldContainer myWorld2 = targetGameObject.GetMyWorld();
			if (!(targetGameObject == null) && !targetGameObject.HasTag(GameTags.Dead))
			{
				MinionResume component2 = minionAssignablesProxy.GetTargetGameObject().GetComponent<MinionResume>();
				bool isPilot = false;
				if (component2 != null && component2.HasPerk(Db.Get().SkillPerks.CanUseRocketControlStation))
				{
					isPilot = true;
				}
				bool isSameWorld = myWorld2.ParentWorldId == myWorld.ParentWorldId;
				list.Add(new RowSortHelper
				{
					minion = minionAssignablesProxy,
					isPilot = isPilot,
					isSameWorld = isSameWorld
				});
			}
		}
		list.Sort(delegate(RowSortHelper a, RowSortHelper b)
		{
			int num2 = b.isSameWorld.CompareTo(a.isSameWorld);
			return (num2 != 0) ? num2 : b.isPilot.CompareTo(a.isPilot);
		});
		foreach (RowSortHelper item in list)
		{
			MinionAssignablesProxy minion = item.minion;
			GameObject gameObject = null;
			if (num >= identityRowMap.Count)
			{
				gameObject = Util.KInstantiateUI(minionRowPrefab, minionRowContainer, force_active: true);
				identityRowMap.Add(gameObject);
			}
			else
			{
				gameObject = identityRowMap[num];
				gameObject.SetActive(value: true);
			}
			num++;
			HierarchyReferences component3 = gameObject.GetComponent<HierarchyReferences>();
			MultiToggle toggle = component3.GetReference<MultiToggle>("Toggle");
			toggle.ChangeState(target.CheckMinionIsMember(minion) ? 1 : 0);
			component3.GetReference<CrewPortrait>("Portrait").SetIdentityObject(minion, jobEnabled: false);
			LocText reference = component3.GetReference<LocText>("Label");
			LocText reference2 = component3.GetReference<LocText>("Designation");
			if (item.isSameWorld)
			{
				if (item.isPilot)
				{
					reference2.text = UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.PILOT;
				}
				else
				{
					reference2.text = "";
				}
				reference.color = Color.black;
				reference2.color = Color.black;
			}
			else
			{
				reference.color = Color.grey;
				reference2.color = Color.grey;
				reference2.text = UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.OFFWORLD;
				gameObject.transform.SetAsLastSibling();
			}
			toggle.onClick = delegate
			{
				target.SetMember(minion, !target.CheckMinionIsMember(minion));
				toggle.ChangeState(target.CheckMinionIsMember(minion) ? 1 : 0);
				RefreshRows();
			};
			string simpleTooltip = UpdateToolTip(minion, !item.isSameWorld);
			toggle.GetComponent<ToolTip>().SetSimpleTooltip(simpleTooltip);
		}
		for (int j = num; j < identityRowMap.Count; j++)
		{
			identityRowMap[j].SetActive(value: false);
		}
	}

	private string UpdateToolTip(MinionAssignablesProxy minion, bool offworld)
	{
		string text = (target.CheckMinionIsMember(minion) ? UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.TOOLTIPS.UNASSIGN : UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.TOOLTIPS.ASSIGN);
		if (offworld)
		{
			text = string.Concat(text, "\n\n", UIConstants.ColorPrefixYellow, UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.TOOLTIPS.DIFFERENT_WORLD, UIConstants.ColorSuffix);
		}
		return text;
	}
}
