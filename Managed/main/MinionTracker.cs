public abstract class MinionTracker : Tracker
{
	public MinionIdentity identity;

	public MinionTracker(MinionIdentity identity)
	{
		this.identity = identity;
	}
}
