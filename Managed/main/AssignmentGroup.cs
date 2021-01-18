using System.Collections.Generic;
using System.Collections.ObjectModel;

public class AssignmentGroup : IAssignableIdentity
{
	private List<IAssignableIdentity> members = new List<IAssignableIdentity>();

	public List<Ownables> current_owners = new List<Ownables>();

	public string id
	{
		get;
		private set;
	}

	public string name
	{
		get;
		private set;
	}

	public AssignmentGroup(string id, IAssignableIdentity[] members, string name)
	{
		this.id = id;
		this.name = name;
		foreach (IAssignableIdentity item in members)
		{
			this.members.Add(item);
		}
		if (Game.Instance != null)
		{
			Game.Instance.assignmentManager.assignment_groups.Add(id, this);
			Game.Instance.Trigger(-1123234494, this);
		}
	}

	public void AddMember(IAssignableIdentity member)
	{
		if (!members.Contains(member))
		{
			members.Add(member);
		}
		Game.Instance.Trigger(-1123234494, this);
	}

	public void RemoveMember(IAssignableIdentity member)
	{
		members.Remove(member);
		Game.Instance.Trigger(-1123234494, this);
	}

	public string GetProperName()
	{
		return name;
	}

	public bool HasMember(IAssignableIdentity member)
	{
		return members.Contains(member);
	}

	public bool IsNull()
	{
		return false;
	}

	public ReadOnlyCollection<IAssignableIdentity> GetMembers()
	{
		return members.AsReadOnly();
	}

	public List<Ownables> GetOwners()
	{
		current_owners.Clear();
		foreach (IAssignableIdentity member in members)
		{
			current_owners.AddRange(member.GetOwners());
		}
		return current_owners;
	}

	public Ownables GetSoleOwner()
	{
		if (members.Count == 1)
		{
			return members[0] as Ownables;
		}
		Debug.LogWarningFormat("GetSoleOwner called on AssignmentGroup with {0} members", members.Count);
		return null;
	}

	public bool HasOwner(Assignables owner)
	{
		foreach (IAssignableIdentity member in members)
		{
			if (member.HasOwner(owner))
			{
				return true;
			}
		}
		return false;
	}

	public int NumOwners()
	{
		int num = 0;
		foreach (IAssignableIdentity member in members)
		{
			num += member.NumOwners();
		}
		return num;
	}
}
