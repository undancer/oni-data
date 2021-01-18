using System;
using System.Collections.Generic;
using UnityEngine;

public class FetchOrder2
{
	public enum OperationalRequirement
	{
		Operational,
		Functional,
		None
	}

	public Action<FetchOrder2, Pickupable> OnComplete;

	public Action<FetchOrder2, Pickupable> OnBegin;

	public List<FetchChore> Chores = new List<FetchChore>();

	private ChoreType choreType;

	private float _UnfetchedAmount;

	private bool checkStorageContents;

	private OperationalRequirement operationalRequirement = OperationalRequirement.None;

	public float TotalAmount
	{
		get;
		set;
	}

	public int PriorityMod
	{
		get;
		set;
	}

	public Tag[] Tags
	{
		get;
		protected set;
	}

	public Tag[] RequiredTags
	{
		get;
		protected set;
	}

	public Tag[] ForbiddenTags
	{
		get;
		protected set;
	}

	public Storage Destination
	{
		get;
		set;
	}

	private float UnfetchedAmount
	{
		get
		{
			return _UnfetchedAmount;
		}
		set
		{
			_UnfetchedAmount = value;
			Assert(_UnfetchedAmount <= TotalAmount, "_UnfetchedAmount <= TotalAmount");
			Assert(_UnfetchedAmount >= 0f, "_UnfetchedAmount >= 0");
		}
	}

	public bool InProgress
	{
		get
		{
			bool result = false;
			foreach (FetchChore chore in Chores)
			{
				if (chore.InProgress())
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}

	public FetchOrder2(ChoreType chore_type, Tag[] tags, Tag[] required_tags, Tag[] forbidden_tags, Storage destination, float amount, OperationalRequirement operationalRequirementDEPRECATED = OperationalRequirement.None, int priorityMod = 0)
	{
		if (amount <= PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT)
		{
			DebugUtil.LogWarningArgs(string.Format("FetchOrder2 {0} is requesting {1} {2} to {3}", chore_type.Id, tags[0], amount, (destination != null) ? destination.name : "to nowhere"));
		}
		choreType = chore_type;
		Tags = tags;
		RequiredTags = required_tags;
		ForbiddenTags = forbidden_tags;
		Destination = destination;
		TotalAmount = amount;
		UnfetchedAmount = amount;
		PriorityMod = priorityMod;
		operationalRequirement = operationalRequirementDEPRECATED;
	}

	private void IssueTask()
	{
		if (UnfetchedAmount > 0f)
		{
			SetFetchTask(UnfetchedAmount);
			UnfetchedAmount = 0f;
		}
	}

	public void SetPriorityMod(int priorityMod)
	{
		PriorityMod = priorityMod;
		for (int i = 0; i < Chores.Count; i++)
		{
			Chores[i].SetPriorityMod(PriorityMod);
		}
	}

	private void SetFetchTask(float amount)
	{
		FetchChore item = new FetchChore(choreType, Destination, amount, Tags, RequiredTags, ForbiddenTags, null, run_until_complete: true, OnFetchChoreComplete, OnFetchChoreBegin, OnFetchChoreEnd, operationalRequirement, PriorityMod);
		Chores.Add(item);
	}

	private void OnFetchChoreEnd(Chore chore)
	{
		FetchChore fetchChore = (FetchChore)chore;
		if (Chores.Contains(fetchChore))
		{
			UnfetchedAmount += fetchChore.amount;
			fetchChore.Cancel("FetchChore Redistribution");
			Chores.Remove(fetchChore);
			IssueTask();
		}
	}

	private void OnFetchChoreComplete(Chore chore)
	{
		FetchChore fetchChore = (FetchChore)chore;
		Chores.Remove(fetchChore);
		if (Chores.Count == 0 && OnComplete != null)
		{
			OnComplete(this, fetchChore.fetchTarget);
		}
	}

	private void OnFetchChoreBegin(Chore chore)
	{
		FetchChore fetchChore = (FetchChore)chore;
		UnfetchedAmount += fetchChore.originalAmount - fetchChore.amount;
		IssueTask();
		if (OnBegin != null)
		{
			OnBegin(this, fetchChore.fetchTarget);
		}
	}

	public void Cancel(string reason)
	{
		while (Chores.Count > 0)
		{
			FetchChore fetchChore = Chores[0];
			fetchChore.Cancel(reason);
			Chores.Remove(fetchChore);
		}
	}

	public void Suspend(string reason)
	{
		Debug.LogError("UNIMPLEMENTED!");
	}

	public void Resume(string reason)
	{
		Debug.LogError("UNIMPLEMENTED!");
	}

	public void Submit(Action<FetchOrder2, Pickupable> on_complete, bool check_storage_contents, Action<FetchOrder2, Pickupable> on_begin = null)
	{
		OnComplete = on_complete;
		OnBegin = on_begin;
		checkStorageContents = check_storage_contents;
		if (check_storage_contents)
		{
			Pickupable out_item = null;
			UnfetchedAmount = GetRemaining(out out_item);
			if (UnfetchedAmount <= Destination.storageFullMargin)
			{
				if (OnComplete != null)
				{
					OnComplete(this, out_item);
				}
			}
			else
			{
				IssueTask();
			}
		}
		else
		{
			IssueTask();
		}
	}

	public bool IsMaterialOnStorage(Storage storage, ref float amount, ref Pickupable out_item)
	{
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
			KPrefabID kPrefabID = component.KPrefabID;
			Tag[] tags = Tags;
			foreach (Tag tag in tags)
			{
				if (kPrefabID.HasTag(tag))
				{
					amount = component.TotalAmount;
					out_item = component;
					return true;
				}
			}
		}
		return false;
	}

	public float AmountWaitingToFetch()
	{
		if (!checkStorageContents)
		{
			float num = UnfetchedAmount;
			for (int i = 0; i < Chores.Count; i++)
			{
				num += Chores[i].AmountWaitingToFetch();
			}
			return num;
		}
		Pickupable out_item;
		return GetRemaining(out out_item);
	}

	public float GetRemaining(out Pickupable out_item)
	{
		float num = TotalAmount;
		float amount = 0f;
		out_item = null;
		if (IsMaterialOnStorage(Destination, ref amount, ref out_item))
		{
			num = Math.Max(num - amount, 0f);
		}
		return num;
	}

	public bool IsComplete()
	{
		for (int i = 0; i < Chores.Count; i++)
		{
			if (!Chores[i].isComplete)
			{
				return false;
			}
		}
		return true;
	}

	private void Assert(bool condition, string message)
	{
		if (!condition)
		{
			string str = "FetchOrder error: " + message;
			str = ((!(Destination == null)) ? (str + "\nDestination: " + Destination.name) : (str + "\nDestination: None"));
			str = str + "\nTotal Amount: " + TotalAmount;
			str = str + "\nUnfetched Amount: " + _UnfetchedAmount;
			Debug.LogError(str);
		}
	}
}
