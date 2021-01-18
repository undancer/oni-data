public class LandingBeacon : KMonoBehaviour
{
	public bool isInUse
	{
		get;
		set;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.LandingBeacons.Add(this);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.LandingBeacons.Remove(this);
	}
}
