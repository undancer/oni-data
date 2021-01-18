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
	}

	private void OnClickView()
	{
		ClusterManager.Instance.SetActiveWorld(targetEntity.GetComponent<WorldContainer>().id);
		ManagementMenu.Instance.CloseAll();
	}
}
