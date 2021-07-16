using UnityEngine;

public class TrapTrigger : KMonoBehaviour
{
	private HandleVector<int>.Handle partitionerEntry;

	public Tag[] trappableCreatures;

	public Vector2 trappedOffset = Vector2.zero;

	[MyCmpReq]
	private Storage storage;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameObject gameObject = base.gameObject;
		partitionerEntry = GameScenePartitioner.Instance.Add("Trap", gameObject, Grid.PosToCell(gameObject), GameScenePartitioner.Instance.trapsLayer, OnCreatureOnTrap);
		foreach (GameObject item in storage.items)
		{
			SetStoredPosition(item);
			KBoxCollider2D component = item.GetComponent<KBoxCollider2D>();
			if (component != null)
			{
				component.enabled = true;
			}
		}
	}

	private void SetStoredPosition(GameObject go)
	{
		Vector3 position = Grid.CellToPosCBC(Grid.PosToCell(base.transform.GetPosition()), Grid.SceneLayer.BuildingBack);
		position.x += trappedOffset.x;
		position.y += trappedOffset.y;
		go.transform.SetPosition(position);
		go.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.BuildingBack);
	}

	public void OnCreatureOnTrap(object data)
	{
		if (!storage.IsEmpty())
		{
			return;
		}
		Trappable trappable = (Trappable)data;
		if (trappable.HasTag(GameTags.Stored) || trappable.HasTag(GameTags.Trapped) || trappable.HasTag(GameTags.Creatures.Bagged))
		{
			return;
		}
		bool flag = false;
		Tag[] array = trappableCreatures;
		foreach (Tag tag in array)
		{
			if (trappable.HasTag(tag))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			storage.Store(trappable.gameObject, hide_popups: true);
			SetStoredPosition(trappable.gameObject);
			Trigger(-358342870, trappable.gameObject);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
	}
}
