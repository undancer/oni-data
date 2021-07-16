using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Uncoverable")]
public class Uncoverable : KMonoBehaviour
{
	[MyCmpReq]
	private OccupyArea occupyArea;

	[Serialize]
	private bool hasBeenUncovered;

	private HandleVector<int>.Handle partitionerEntry;

	private static readonly Func<int, object, bool> IsCellBlockedDelegate = (int cell, object data) => IsCellBlocked(cell, data);

	private bool IsAnyCellShowing()
	{
		int rootCell = Grid.PosToCell(this);
		return !occupyArea.TestArea(rootCell, null, IsCellBlockedDelegate);
	}

	private static bool IsCellBlocked(int cell, object data)
	{
		if (Grid.Element[cell].IsSolid)
		{
			return !Grid.Foundation[cell];
		}
		return false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (IsAnyCellShowing())
		{
			hasBeenUncovered = true;
		}
		if (!hasBeenUncovered)
		{
			GetComponent<KSelectable>().IsSelectable = false;
			Extents extents = occupyArea.GetExtents();
			partitionerEntry = GameScenePartitioner.Instance.Add("Uncoverable.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, OnSolidChanged);
		}
	}

	private void OnSolidChanged(object data)
	{
		if (IsAnyCellShowing() && !hasBeenUncovered && partitionerEntry.IsValid())
		{
			GameScenePartitioner.Instance.Free(ref partitionerEntry);
			hasBeenUncovered = true;
			GetComponent<KSelectable>().IsSelectable = true;
			Notification notification = new Notification(MISC.STATUSITEMS.BURIEDITEM.NOTIFICATION, NotificationType.Good, OnNotificationToolTip, this);
			base.gameObject.AddOrGet<Notifier>().Add(notification);
		}
	}

	private static string OnNotificationToolTip(List<Notification> notifications, object data)
	{
		Uncoverable cmp = (Uncoverable)data;
		return MISC.STATUSITEMS.BURIEDITEM.NOTIFICATION_TOOLTIP.Replace("{Uncoverable}", cmp.GetProperName());
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
	}
}
