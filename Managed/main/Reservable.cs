using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Reservable")]
public class Reservable : KMonoBehaviour
{
	private GameObject reservedBy;

	public GameObject ReservedBy => reservedBy;

	public bool isReserved => !(reservedBy == null);

	public bool Reserve(GameObject reserver)
	{
		if (reservedBy == null)
		{
			reservedBy = reserver;
			return true;
		}
		return false;
	}

	public void ClearReservation(GameObject reserver)
	{
		if (reservedBy == reserver)
		{
			reservedBy = null;
		}
	}
}
