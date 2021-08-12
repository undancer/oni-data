using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ClusterGridWorldSideScreen : SideScreenContent
{
	public Image icon;

	public KButton viewButton;

	private AsteroidGridEntity targetEntity;

	protected override void OnSpawn()
	{
		viewButton.onClick += OnClickView;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<AsteroidGridEntity>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		targetEntity = target.GetComponent<AsteroidGridEntity>();
		icon.sprite = Def.GetUISprite(targetEntity).first;
		WorldContainer component = targetEntity.GetComponent<WorldContainer>();
		bool flag = component != null && component.IsDiscovered;
		viewButton.isInteractable = flag;
		if (!flag)
		{
			viewButton.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.CLUSTERWORLDSIDESCREEN.VIEW_WORLD_DISABLE_TOOLTIP);
		}
		else
		{
			viewButton.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.CLUSTERWORLDSIDESCREEN.VIEW_WORLD_TOOLTIP);
		}
	}

	private void OnClickView()
	{
		WorldContainer component = targetEntity.GetComponent<WorldContainer>();
		if (!component.IsDupeVisited)
		{
			component.LookAtSurface();
		}
		ClusterManager.Instance.SetActiveWorld(component.id);
		ManagementMenu.Instance.CloseAll();
	}
}
