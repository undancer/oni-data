using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Approachable")]
public class Approachable : KMonoBehaviour, IApproachable
{
	public CellOffset[] GetOffsets()
	{
		return OffsetGroups.Use;
	}

	public int GetCell()
	{
		return Grid.PosToCell(this);
	}
}
