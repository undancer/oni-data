using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Chattable")]
public class Chattable : KMonoBehaviour, IApproachable
{
	public CellOffset[] GetOffsets()
	{
		return OffsetGroups.Chat;
	}

	public int GetCell()
	{
		return Grid.PosToCell(this);
	}
}
