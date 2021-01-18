using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Placeable")]
public class Placeable : KMonoBehaviour
{
	[MyCmpReq]
	private KPrefabID prefabId;

	[Serialize]
	private int targetCell = -1;

	public Tag previewTag;

	public Tag spawnOnPlaceTag;

	private GameObject preview;

	private FetchChore chore;

	private static readonly EventSystem.IntraObjectHandler<Placeable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Placeable>(delegate(Placeable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		prefabId.AddTag(new Tag(prefabId.InstanceID.ToString()));
		if (targetCell != -1)
		{
			QueuePlacement(targetCell);
		}
	}

	protected override void OnCleanUp()
	{
		if (preview != null)
		{
			preview.DeleteObject();
		}
		base.OnCleanUp();
	}

	public void QueuePlacement(int target)
	{
		targetCell = target;
		Vector3 position = Grid.CellToPosCBC(targetCell, Grid.SceneLayer.Front);
		if (preview == null)
		{
			preview = GameUtil.KInstantiate(Assets.GetPrefab(previewTag), position, Grid.SceneLayer.Front);
			preview.SetActive(value: true);
		}
		else
		{
			preview.transform.SetPosition(position);
		}
		if (chore != null)
		{
			chore.Cancel("new target");
		}
		chore = new FetchChore(Db.Get().ChoreTypes.Fetch, preview.GetComponent<Storage>(), 1f, new Tag[1]
		{
			new Tag(prefabId.InstanceID.ToString())
		}, null, null, null, run_until_complete: true, OnChoreComplete, null, null, FetchOrder2.OperationalRequirement.None);
	}

	private void OnChoreComplete(Chore completed_chore)
	{
		Place(targetCell);
	}

	public void Place(int target)
	{
		Vector3 position = Grid.CellToPosCBC(target, Grid.SceneLayer.Front);
		GameUtil.KInstantiate(Assets.GetPrefab(spawnOnPlaceTag), position, Grid.SceneLayer.Front).SetActive(value: true);
		this.DeleteObject();
	}

	private void OpenPlaceTool()
	{
		PlaceTool.Instance.Activate(this, previewTag);
	}

	private void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo button = ((targetCell == -1) ? new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.RELOCATE.NAME, OpenPlaceTool, Action.NumActions, null, null, null, UI.USERMENUACTIONS.RELOCATE.TOOLTIP) : new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.RELOCATE.NAME_OFF, CancelRelocation, Action.NumActions, null, null, null, UI.USERMENUACTIONS.RELOCATE.TOOLTIP_OFF));
		Game.Instance.userMenu.AddButton(base.gameObject, button);
	}

	private void CancelRelocation()
	{
		if (preview != null)
		{
			preview.DeleteObject();
			preview = null;
		}
		targetCell = -1;
	}
}
