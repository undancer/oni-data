using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class Ownables : Assignables
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public void UnassignAll()
	{
		foreach (AssignableSlotInstance slot in slots)
		{
			if (slot.assignable != null)
			{
				slot.assignable.Unassign();
			}
		}
	}
}
