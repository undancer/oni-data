using System.Collections.Generic;
using UnityEngine;

public interface IStorage
{
	bool allowUIItemRemoval
	{
		get;
		set;
	}

	bool ShouldShowInUI();

	GameObject Drop(GameObject go, bool do_disease_transfer = true);

	List<GameObject> GetItems();

	bool IsFull();

	bool IsEmpty();

	float Capacity();

	float RemainingCapacity();

	float GetAmountAvailable(Tag tag);

	void ConsumeIgnoringDisease(Tag tag, float amount);
}
