using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class PartialLightBlocking : KMonoBehaviour
{
	private const byte PartialLightBlockingProperties = 48;

	protected override void OnSpawn()
	{
		SetLightBlocking();
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		ClearLightBlocking();
		base.OnCleanUp();
	}

	public void SetLightBlocking()
	{
		int[] placementCells = GetComponent<Building>().PlacementCells;
		for (int i = 0; i < placementCells.Length; i++)
		{
			SimMessages.SetCellProperties(placementCells[i], 48);
		}
	}

	public void ClearLightBlocking()
	{
		int[] placementCells = GetComponent<Building>().PlacementCells;
		for (int i = 0; i < placementCells.Length; i++)
		{
			SimMessages.ClearCellProperties(placementCells[i], 48);
		}
	}
}
