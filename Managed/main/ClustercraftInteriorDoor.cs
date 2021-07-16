using STRINGS;

public class ClustercraftInteriorDoor : KMonoBehaviour, ISidescreenButtonControl
{
	public string SidescreenButtonText => UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWEXTERIOR.LABEL;

	public string SidescreenButtonTooltip => SidescreenButtonInteractable() ? UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWEXTERIOR.LABEL : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWEXTERIOR.INVALID;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.ClusterCraftInteriorDoors.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.ClusterCraftInteriorDoors.Remove(this);
		base.OnCleanUp();
	}

	public bool SidescreenEnabled()
	{
		return true;
	}

	public bool SidescreenButtonInteractable()
	{
		WorldContainer myWorld = base.gameObject.GetMyWorld();
		if (myWorld.ParentWorldId != ClusterManager.INVALID_WORLD_IDX)
		{
			return myWorld.ParentWorldId != myWorld.id;
		}
		return false;
	}

	public void OnSidescreenButtonPressed()
	{
		ClusterManager.Instance.SetActiveWorld(base.gameObject.GetMyWorld().ParentWorldId);
	}

	public int ButtonSideScreenSortOrder()
	{
		return 20;
	}
}
