using System;
using System.Collections.Generic;
using UnityEngine;

public class FetchList2 : IFetchList
{
	private System.Action OnComplete;

	private ChoreType choreType;

	public Guid waitingForMaterialsHandle = Guid.Empty;

	public Guid materialsUnavailableForRefillHandle = Guid.Empty;

	public Guid materialsUnavailableHandle = Guid.Empty;

	public Dictionary<Tag, float> MinimumAmount = new Dictionary<Tag, float>();

	public List<FetchOrder2> FetchOrders = new List<FetchOrder2>();

	private Dictionary<Tag, float> Remaining = new Dictionary<Tag, float>();

	private bool bShowStatusItem = true;

	public bool ShowStatusItem
	{
		get
		{
			return bShowStatusItem;
		}
		set
		{
			bShowStatusItem = value;
		}
	}

	public bool IsComplete => FetchOrders.Count == 0;

	public bool InProgress
	{
		get
		{
			if (FetchOrders.Count < 0)
			{
				return false;
			}
			bool result = false;
			foreach (FetchOrder2 fetchOrder in FetchOrders)
			{
				if (fetchOrder.InProgress)
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}

	public Storage Destination
	{
		get;
		private set;
	}

	public int PriorityMod
	{
		get;
		private set;
	}

	public FetchList2(Storage destination, ChoreType chore_type)
	{
		Destination = destination;
		choreType = chore_type;
	}

	public void SetPriorityMod(int priorityMod)
	{
		PriorityMod = priorityMod;
		for (int i = 0; i < FetchOrders.Count; i++)
		{
			FetchOrders[i].SetPriorityMod(PriorityMod);
		}
	}

	public void Add(Tag[] tags, Tag[] required_tags = null, Tag[] forbidden_tags = null, float amount = 1f, FetchOrder2.OperationalRequirement operationalRequirementDEPRECATED = FetchOrder2.OperationalRequirement.None)
	{
		foreach (Tag key in tags)
		{
			if (!MinimumAmount.ContainsKey(key))
			{
				MinimumAmount[key] = amount;
			}
		}
		FetchOrder2 item = new FetchOrder2(choreType, tags, required_tags, forbidden_tags, Destination, amount, operationalRequirementDEPRECATED, PriorityMod);
		FetchOrders.Add(item);
	}

	public void Add(Tag tag, Tag[] required_tags = null, Tag[] forbidden_tags = null, float amount = 1f, FetchOrder2.OperationalRequirement operationalRequirementDEPRECATED = FetchOrder2.OperationalRequirement.None)
	{
		Add(new Tag[1]
		{
			tag
		}, required_tags, forbidden_tags, amount, operationalRequirementDEPRECATED);
	}

	public float GetMinimumAmount(Tag tag)
	{
		float value = 0f;
		MinimumAmount.TryGetValue(tag, out value);
		return value;
	}

	private void OnFetchOrderComplete(FetchOrder2 fetch_order, Pickupable fetched_item)
	{
		FetchOrders.Remove(fetch_order);
		if (FetchOrders.Count == 0)
		{
			if (OnComplete != null)
			{
				OnComplete();
			}
			FetchListStatusItemUpdater.instance.RemoveFetchList(this);
			ClearStatus();
		}
	}

	public void Cancel(string reason)
	{
		foreach (FetchOrder2 fetchOrder in FetchOrders)
		{
			fetchOrder.Cancel(reason);
		}
		ClearStatus();
		FetchListStatusItemUpdater.instance.RemoveFetchList(this);
	}

	public void UpdateRemaining()
	{
		Remaining.Clear();
		for (int i = 0; i < FetchOrders.Count; i++)
		{
			FetchOrder2 fetchOrder = FetchOrders[i];
			for (int j = 0; j < fetchOrder.Tags.Length; j++)
			{
				Tag key = fetchOrder.Tags[j];
				float value = 0f;
				Remaining.TryGetValue(key, out value);
				Remaining[key] = value + fetchOrder.AmountWaitingToFetch();
			}
		}
	}

	public Dictionary<Tag, float> GetRemaining()
	{
		return Remaining;
	}

	public Dictionary<Tag, float> GetRemainingMinimum()
	{
		Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
		foreach (FetchOrder2 fetchOrder in FetchOrders)
		{
			Tag[] tags = fetchOrder.Tags;
			foreach (Tag key in tags)
			{
				dictionary[key] = MinimumAmount[key];
			}
		}
		foreach (GameObject item in Destination.items)
		{
			if (!(item != null))
			{
				continue;
			}
			Pickupable component = item.GetComponent<Pickupable>();
			if (!(component != null))
			{
				continue;
			}
			KPrefabID component2 = component.GetComponent<KPrefabID>();
			foreach (Tag tag in component2.Tags)
			{
				if (dictionary.ContainsKey(tag))
				{
					dictionary[tag] = Math.Max(dictionary[tag] - component.TotalAmount, 0f);
				}
			}
		}
		return dictionary;
	}

	public void Suspend(string reason)
	{
		foreach (FetchOrder2 fetchOrder in FetchOrders)
		{
			fetchOrder.Suspend(reason);
		}
	}

	public void Resume(string reason)
	{
		foreach (FetchOrder2 fetchOrder in FetchOrders)
		{
			fetchOrder.Resume(reason);
		}
	}

	public void Submit(System.Action on_complete, bool check_storage_contents)
	{
		OnComplete = on_complete;
		List<FetchOrder2> range = FetchOrders.GetRange(0, FetchOrders.Count);
		foreach (FetchOrder2 item in range)
		{
			item.Submit(OnFetchOrderComplete, check_storage_contents);
		}
		if (!IsComplete && ShowStatusItem)
		{
			FetchListStatusItemUpdater.instance.AddFetchList(this);
		}
	}

	private void ClearStatus()
	{
		if (Destination != null)
		{
			KSelectable component = Destination.GetComponent<KSelectable>();
			if (component != null)
			{
				waitingForMaterialsHandle = component.RemoveStatusItem(waitingForMaterialsHandle);
				materialsUnavailableHandle = component.RemoveStatusItem(materialsUnavailableHandle);
				materialsUnavailableForRefillHandle = component.RemoveStatusItem(materialsUnavailableForRefillHandle);
			}
		}
	}

	public void UpdateStatusItem(MaterialsStatusItem status_item, ref Guid handle, bool should_add)
	{
		bool flag = handle != Guid.Empty;
		if (should_add == flag)
		{
			return;
		}
		if (should_add)
		{
			KSelectable component = Destination.GetComponent<KSelectable>();
			if (component != null)
			{
				handle = component.AddStatusItem(status_item, this);
				GameScheduler.Instance.Schedule("Digging Tutorial", 2f, delegate
				{
					Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Digging);
				});
			}
		}
		else
		{
			KSelectable component2 = Destination.GetComponent<KSelectable>();
			if (component2 != null)
			{
				handle = component2.RemoveStatusItem(handle);
			}
		}
	}
}
