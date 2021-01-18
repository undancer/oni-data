using KSerialization;

public class ClusterDestinationSelector : KMonoBehaviour
{
	[Serialize]
	protected AxialI m_destination;

	public bool assignable;

	public bool requireAsteroidDestination;

	public bool canNavigateFogOfWar;

	public bool requireLaunchPadOnAsteroidDestination;

	public bool shouldPointTowardsPath;

	public int GetDestinationWorld()
	{
		return ClusterUtil.GetAsteroidWorldIdAtLocation(m_destination);
	}

	public AxialI GetDestination()
	{
		return m_destination;
	}

	public virtual void SetDestination(AxialI location)
	{
		if (requireAsteroidDestination)
		{
			Debug.Assert(ClusterUtil.GetAsteroidWorldIdAtLocation(location) != -1, $"Cannot SetDestination to {location} as there is no world there");
		}
		m_destination = location;
		Trigger(543433792, location);
	}

	public bool HasAsteroidDestination()
	{
		return ClusterUtil.GetAsteroidWorldIdAtLocation(m_destination) != -1;
	}

	public bool IsAtDestination()
	{
		AxialI myWorldLocation = this.GetMyWorldLocation();
		return myWorldLocation == m_destination;
	}
}
