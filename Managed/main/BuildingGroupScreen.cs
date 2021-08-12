public class BuildingGroupScreen : KScreen
{
	public static BuildingGroupScreen Instance;

	protected override void OnPrefabInit()
	{
		Instance = this;
		base.OnPrefabInit();
		base.ConsumeMouseScroll = true;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		base.ConsumeMouseScroll = true;
	}
}
