using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using KSerialization;

public class AssignmentGroupController : KMonoBehaviour
{
	public bool generateGroupOnStart = false;

	[Serialize]
	private string _assignmentGroupID;

	[Serialize]
	private Ref<MinionAssignablesProxy>[] minionsInGroupAtLoad;

	public string AssignmentGroupID
	{
		get
		{
			return _assignmentGroupID;
		}
		private set
		{
			_assignmentGroupID = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	[OnDeserialized]
	protected void CreateOrRestoreGroupID()
	{
		if (string.IsNullOrEmpty(AssignmentGroupID))
		{
			GenerateGroupID();
		}
		else
		{
			Game.Instance.assignmentManager.TryCreateAssignmentGroup(AssignmentGroupID, new IAssignableIdentity[0], base.gameObject.GetProperName());
		}
	}

	public void SetGroupID(string id)
	{
		DebugUtil.DevAssert(!string.IsNullOrEmpty(id), "Trying to set Assignment group on " + base.gameObject.name + " to null or empty.");
		if (!string.IsNullOrEmpty(id))
		{
			AssignmentGroupID = id;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		RestoreGroupAssignees();
	}

	private void GenerateGroupID()
	{
		if (generateGroupOnStart && string.IsNullOrEmpty(AssignmentGroupID))
		{
			SetGroupID(GetComponent<KPrefabID>().PrefabID().ToString() + "_" + GetComponent<KPrefabID>().InstanceID + "_assignmentGroup");
			Game.Instance.assignmentManager.TryCreateAssignmentGroup(AssignmentGroupID, new IAssignableIdentity[0], base.gameObject.GetProperName());
		}
	}

	private void RestoreGroupAssignees()
	{
		if (generateGroupOnStart)
		{
			CreateOrRestoreGroupID();
			if (minionsInGroupAtLoad == null)
			{
				minionsInGroupAtLoad = new Ref<MinionAssignablesProxy>[0];
			}
			for (int i = 0; i < minionsInGroupAtLoad.Length; i++)
			{
				Game.Instance.assignmentManager.AddToAssignmentGroup(AssignmentGroupID, minionsInGroupAtLoad[i].Get());
			}
			Ownable component = GetComponent<Ownable>();
			if (component != null)
			{
				component.Assign(Game.Instance.assignmentManager.assignment_groups[AssignmentGroupID]);
				component.SetCanBeAssigned(state: false);
			}
		}
	}

	public bool CheckMinionIsMember(MinionAssignablesProxy minion)
	{
		if (string.IsNullOrEmpty(AssignmentGroupID))
		{
			GenerateGroupID();
		}
		return Game.Instance.assignmentManager.assignment_groups[AssignmentGroupID].HasMember(minion);
	}

	public void SetMember(MinionAssignablesProxy minion, bool isAllowed)
	{
		Debug.Assert(DlcManager.IsExpansion1Active());
		if (!isAllowed)
		{
			Game.Instance.assignmentManager.RemoveFromAssignmentGroup(AssignmentGroupID, minion);
		}
		else if (!CheckMinionIsMember(minion))
		{
			Game.Instance.assignmentManager.AddToAssignmentGroup(AssignmentGroupID, minion);
		}
	}

	protected override void OnCleanUp()
	{
		if (generateGroupOnStart)
		{
			Game.Instance.assignmentManager.RemoveAssignmentGroup(AssignmentGroupID);
		}
		base.OnCleanUp();
	}

	[OnSerializing]
	private void OnSerialize()
	{
		Debug.Assert(!string.IsNullOrEmpty(AssignmentGroupID), "Assignment group on " + base.gameObject.name + " has null or empty ID");
		AssignmentGroup assignmentGroup = Game.Instance.assignmentManager.assignment_groups[AssignmentGroupID];
		ReadOnlyCollection<IAssignableIdentity> members = assignmentGroup.GetMembers();
		minionsInGroupAtLoad = new Ref<MinionAssignablesProxy>[members.Count];
		for (int i = 0; i < members.Count; i++)
		{
			minionsInGroupAtLoad[i] = new Ref<MinionAssignablesProxy>((MinionAssignablesProxy)members[i]);
		}
	}

	public ReadOnlyCollection<IAssignableIdentity> GetMembers()
	{
		return Game.Instance.assignmentManager.assignment_groups[AssignmentGroupID].GetMembers();
	}
}
