using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AssignmentManager")]
public class AssignmentManager : KMonoBehaviour
{
	private List<Assignable> assignables = new List<Assignable>();

	public Dictionary<string, AssignmentGroup> assignment_groups = new Dictionary<string, AssignmentGroup>
	{
		{
			"public",
			new AssignmentGroup("public", new IAssignableIdentity[0], UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.PUBLIC)
		}
	};

	private static readonly EventSystem.IntraObjectHandler<AssignmentManager> MinionMigrationDelegate = new EventSystem.IntraObjectHandler<AssignmentManager>(delegate(AssignmentManager component, object data)
	{
		component.MinionMigration(data);
	});

	private List<Assignable> PreferredAssignableResults = new List<Assignable>();

	public IEnumerator<Assignable> GetEnumerator()
	{
		return assignables.GetEnumerator();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Game.Instance.Subscribe(586301400, MinionMigrationDelegate);
	}

	protected void MinionMigration(object data)
	{
		foreach (Assignable assignable in assignables)
		{
			if (assignable.assignee != null)
			{
				Ownables soleOwner = assignable.assignee.GetSoleOwner();
				if (soleOwner != null && soleOwner.GetComponent<MinionAssignablesProxy>() != null && assignable.assignee.GetSoleOwner().GetComponent<MinionAssignablesProxy>().GetTargetGameObject() == ((MinionIdentity)data).gameObject)
				{
					assignable.Unassign();
				}
			}
		}
	}

	public void Add(Assignable assignable)
	{
		assignables.Add(assignable);
	}

	public void Remove(Assignable assignable)
	{
		assignables.Remove(assignable);
	}

	public AssignmentGroup TryCreateAssignmentGroup(string id, IAssignableIdentity[] members, string name)
	{
		if (assignment_groups.ContainsKey(id))
		{
			return assignment_groups[id];
		}
		return new AssignmentGroup(id, members, name);
	}

	public void RemoveAssignmentGroup(string id)
	{
		if (!assignment_groups.ContainsKey(id))
		{
			Debug.LogError("Assignment group with id " + id + " doesn't exists");
		}
		else
		{
			assignment_groups.Remove(id);
		}
	}

	public void AddToAssignmentGroup(string group_id, IAssignableIdentity member)
	{
		Debug.Assert(assignment_groups.ContainsKey(group_id));
		assignment_groups[group_id].AddMember(member);
	}

	public void RemoveFromAssignmentGroup(string group_id, IAssignableIdentity member)
	{
		Debug.Assert(assignment_groups.ContainsKey(group_id));
		assignment_groups[group_id].RemoveMember(member);
	}

	public void RemoveFromAllGroups(IAssignableIdentity member)
	{
		foreach (Assignable assignable in assignables)
		{
			if (assignable.assignee == member)
			{
				assignable.Unassign();
			}
		}
		foreach (KeyValuePair<string, AssignmentGroup> assignment_group in assignment_groups)
		{
			if (assignment_group.Value.HasMember(member))
			{
				assignment_group.Value.RemoveMember(member);
			}
		}
	}

	public void RemoveFromWorld(IAssignableIdentity minionIdentity, int world_id)
	{
		foreach (Assignable assignable in assignables)
		{
			if (assignable.assignee != null)
			{
				Ownables soleOwner = assignable.assignee.GetSoleOwner();
				if (soleOwner != null && soleOwner.GetComponent<MinionAssignablesProxy>() != null && assignable.assignee == minionIdentity && assignable.GetMyWorldId() == world_id)
				{
					assignable.Unassign();
				}
			}
		}
	}

	public List<Assignable> GetPreferredAssignables(Assignables owner, AssignableSlot slot)
	{
		PreferredAssignableResults.Clear();
		int num = int.MaxValue;
		foreach (Assignable assignable in assignables)
		{
			if (assignable.slot == slot && assignable.assignee != null && assignable.assignee.HasOwner(owner))
			{
				Room room = assignable.assignee as Room;
				if (room != null && room.roomType.priority_building_use)
				{
					PreferredAssignableResults.Clear();
					PreferredAssignableResults.Add(assignable);
					return PreferredAssignableResults;
				}
				int num2 = assignable.assignee.NumOwners();
				if (num2 == num)
				{
					PreferredAssignableResults.Add(assignable);
				}
				else if (num2 < num)
				{
					num = num2;
					PreferredAssignableResults.Clear();
					PreferredAssignableResults.Add(assignable);
				}
			}
		}
		return PreferredAssignableResults;
	}

	public bool IsPreferredAssignable(Assignables owner, Assignable candidate)
	{
		IAssignableIdentity assignee = candidate.assignee;
		if (assignee == null || !assignee.HasOwner(owner))
		{
			return false;
		}
		int num = assignee.NumOwners();
		Room room = assignee as Room;
		if (room != null && room.roomType.priority_building_use)
		{
			return true;
		}
		foreach (Assignable assignable in assignables)
		{
			if (assignable.slot == candidate.slot && assignable.assignee != assignee)
			{
				Room room2 = assignable.assignee as Room;
				if (room2 != null && room2.roomType.priority_building_use && assignable.assignee.HasOwner(owner))
				{
					return false;
				}
				if (assignable.assignee.NumOwners() < num && assignable.assignee.HasOwner(owner))
				{
					return false;
				}
			}
		}
		return true;
	}
}
