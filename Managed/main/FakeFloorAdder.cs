public class FakeFloorAdder : KMonoBehaviour
{
	public CellOffset[] floorOffsets;

	public bool initiallyActive = true;

	private bool isActive = false;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (initiallyActive)
		{
			SetFloor(active: true);
		}
	}

	public void SetFloor(bool active)
	{
		if (isActive != active)
		{
			int cell = Grid.PosToCell(this);
			Building component = GetComponent<Building>();
			CellOffset[] array = floorOffsets;
			foreach (CellOffset offset in array)
			{
				CellOffset rotatedOffset = component.GetRotatedOffset(offset);
				int num = Grid.OffsetCell(cell, rotatedOffset);
				Grid.FakeFloor[num] = active;
				Pathfinding.Instance.AddDirtyNavGridCell(num);
			}
			isActive = active;
		}
	}

	protected override void OnCleanUp()
	{
		SetFloor(active: false);
		base.OnCleanUp();
	}
}
