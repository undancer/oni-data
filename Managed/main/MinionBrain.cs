using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;

public class MinionBrain : Brain
{
	[MyCmpReq]
	public Navigator Navigator;

	[MyCmpGet]
	public OxygenBreather OxygenBreather;

	private float lastResearchCompleteEmoteTime;

	private static readonly EventSystem.IntraObjectHandler<MinionBrain> AnimTrackStoredItemDelegate = new EventSystem.IntraObjectHandler<MinionBrain>(delegate(MinionBrain component, object data)
	{
		component.AnimTrackStoredItem(data);
	});

	private static readonly EventSystem.IntraObjectHandler<MinionBrain> OnUnstableGroundImpactDelegate = new EventSystem.IntraObjectHandler<MinionBrain>(delegate(MinionBrain component, object data)
	{
		component.OnUnstableGroundImpact(data);
	});

	public bool IsCellClear(int cell)
	{
		if (Grid.Reserved[cell])
		{
			return false;
		}
		GameObject gameObject = Grid.Objects[cell, 0];
		if (gameObject != null && base.gameObject != gameObject && !gameObject.GetComponent<Navigator>().IsMoving())
		{
			return false;
		}
		return true;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Navigator.SetAbilities(new MinionPathFinderAbilities(Navigator));
		Subscribe(-1697596308, AnimTrackStoredItemDelegate);
		Subscribe(-975551167, OnUnstableGroundImpactDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		foreach (GameObject item in GetComponent<Storage>().items)
		{
			AddAnimTracker(item);
		}
		Game.Instance.Subscribe(-107300940, OnResearchComplete);
	}

	private void AnimTrackStoredItem(object data)
	{
		Storage component = GetComponent<Storage>();
		GameObject gameObject = (GameObject)data;
		RemoveTracker(gameObject);
		if (component.items.Contains(gameObject))
		{
			AddAnimTracker(gameObject);
		}
	}

	private void AddAnimTracker(GameObject go)
	{
		KAnimControllerBase component = go.GetComponent<KAnimControllerBase>();
		if (!(component == null) && component.AnimFiles != null && component.AnimFiles.Length != 0 && component.AnimFiles[0] != null && component.GetComponent<Pickupable>().trackOnPickup)
		{
			KBatchedAnimTracker kBatchedAnimTracker = go.AddComponent<KBatchedAnimTracker>();
			kBatchedAnimTracker.useTargetPoint = false;
			kBatchedAnimTracker.fadeOut = false;
			kBatchedAnimTracker.symbol = new HashedString("snapTo_chest");
			kBatchedAnimTracker.forceAlwaysVisible = true;
		}
	}

	private void RemoveTracker(GameObject go)
	{
		KBatchedAnimTracker component = go.GetComponent<KBatchedAnimTracker>();
		if (component != null)
		{
			Object.Destroy(component);
		}
	}

	public override void UpdateBrain()
	{
		base.UpdateBrain();
		if (Game.Instance == null)
		{
			return;
		}
		if (!Game.Instance.savedInfo.discoveredSurface)
		{
			int cell = Grid.PosToCell(base.gameObject);
			if (World.Instance.zoneRenderData.GetSubWorldZoneType(cell) == SubWorld.ZoneType.Space)
			{
				Game.Instance.savedInfo.discoveredSurface = true;
				DiscoveredSpaceMessage message = new DiscoveredSpaceMessage(base.gameObject.transform.GetPosition());
				Messenger.Instance.QueueMessage(message);
				Game.Instance.Trigger(-818188514, base.gameObject);
			}
		}
		if (!Game.Instance.savedInfo.discoveredOilField)
		{
			int cell2 = Grid.PosToCell(base.gameObject);
			if (World.Instance.zoneRenderData.GetSubWorldZoneType(cell2) == SubWorld.ZoneType.OilField)
			{
				Game.Instance.savedInfo.discoveredOilField = true;
			}
		}
	}

	private void RegisterReactEmotePair(string reactable_id, string kanim_file_name, float max_trigger_time)
	{
		if (!(base.gameObject == null))
		{
			ReactionMonitor.Instance sMI = base.gameObject.GetSMI<ReactionMonitor.Instance>();
			if (sMI != null)
			{
				EmoteChore emoteChore = new EmoteChore(base.gameObject.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteIdle, kanim_file_name, new HashedString[1] { "react" }, null);
				SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(base.gameObject, reactable_id, Db.Get().ChoreTypes.Cough, kanim_file_name, max_trigger_time);
				emoteChore.PairReactable(selfEmoteReactable);
				selfEmoteReactable.AddStep(new EmoteReactable.EmoteStep
				{
					anim = "react"
				});
				selfEmoteReactable.PairEmote(emoteChore);
				sMI.AddOneshotReactable(selfEmoteReactable);
			}
		}
	}

	private void OnResearchComplete(object data)
	{
		if (Time.time - lastResearchCompleteEmoteTime > 1f)
		{
			RegisterReactEmotePair("ResearchComplete", "anim_react_research_complete_kanim", 3f);
			lastResearchCompleteEmoteTime = Time.time;
		}
	}

	public Notification CreateCollapseNotification()
	{
		MinionIdentity component = GetComponent<MinionIdentity>();
		return new Notification(MISC.NOTIFICATIONS.TILECOLLAPSE.NAME, NotificationType.Bad, (List<Notification> notificationList, object data) => string.Concat(MISC.NOTIFICATIONS.TILECOLLAPSE.TOOLTIP, notificationList.ReduceMessages(countNames: false)), "/t• " + component.GetProperName());
	}

	public void RemoveCollapseNotification(Notification notification)
	{
		Vector3 position = notification.clickFocus.GetPosition();
		position.z = -40f;
		WorldContainer myWorld = notification.clickFocus.gameObject.GetMyWorld();
		if (myWorld != null && myWorld.IsDiscovered)
		{
			CameraController.Instance.ActiveWorldStarWipe(myWorld.id, position);
		}
		base.gameObject.AddOrGet<Notifier>().Remove(notification);
	}

	private void OnUnstableGroundImpact(object data)
	{
		GameObject telepad = GameUtil.GetTelepad(base.gameObject.GetMyWorld().id);
		Navigator component = GetComponent<Navigator>();
		Assignable assignable = GetComponent<MinionIdentity>().GetSoleOwner().GetAssignable(Db.Get().AssignableSlots.Bed);
		bool num = assignable != null && component.CanReach(Grid.PosToCell(assignable.transform.GetPosition()));
		bool flag = telepad != null && component.CanReach(Grid.PosToCell(telepad.transform.GetPosition()));
		if (!num && !flag)
		{
			RegisterReactEmotePair("UnstableGroundShock", "anim_react_shock_kanim", 1f);
			Notification notification = CreateCollapseNotification();
			notification.customClickCallback = delegate
			{
				RemoveCollapseNotification(notification);
			};
			base.gameObject.AddOrGet<Notifier>().Add(notification);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Game.Instance.Unsubscribe(-107300940, OnResearchComplete);
	}
}
