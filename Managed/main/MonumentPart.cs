using System.Collections.Generic;
using KSerialization;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MonumentPart")]
public class MonumentPart : KMonoBehaviour
{
	public enum Part
	{
		Bottom,
		Middle,
		Top
	}

	public Part part;

	public List<Tuple<string, string>> selectableStatesAndSymbols = new List<Tuple<string, string>>();

	public string stateUISymbol;

	[Serialize]
	private string chosenState;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.MonumentParts.Add(this);
		if (!string.IsNullOrEmpty(chosenState))
		{
			SetState(chosenState);
		}
		UpdateMonumentDecor();
	}

	protected override void OnCleanUp()
	{
		Components.MonumentParts.Remove(this);
		RemoveMonumentPiece();
		base.OnCleanUp();
	}

	public void SetState(string state)
	{
		GetComponent<KBatchedAnimController>().Play(state);
		chosenState = state;
	}

	public bool IsMonumentCompleted()
	{
		bool flag = GetMonumentPart(Part.Top) != null;
		bool flag2 = GetMonumentPart(Part.Middle) != null;
		bool flag3 = GetMonumentPart(Part.Bottom) != null;
		return flag && flag3 && flag2;
	}

	public void UpdateMonumentDecor()
	{
		GameObject monumentPart = GetMonumentPart(Part.Middle);
		if (!IsMonumentCompleted())
		{
			return;
		}
		DecorProvider component = monumentPart.GetComponent<DecorProvider>();
		component.SetValues(BUILDINGS.DECOR.BONUS.MONUMENT.COMPLETE);
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>());
		foreach (GameObject item in attachedNetwork)
		{
			if (item != monumentPart)
			{
				DecorProvider component2 = item.GetComponent<DecorProvider>();
				component2.SetValues(BUILDINGS.DECOR.NONE);
			}
		}
	}

	public void RemoveMonumentPiece()
	{
		if (!IsMonumentCompleted())
		{
			return;
		}
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>());
		foreach (GameObject item in attachedNetwork)
		{
			if (item.GetComponent<MonumentPart>() != this)
			{
				DecorProvider component = item.GetComponent<DecorProvider>();
				component.SetValues(BUILDINGS.DECOR.BONUS.MONUMENT.INCOMPLETE);
			}
		}
	}

	private GameObject GetMonumentPart(Part requestPart)
	{
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>());
		foreach (GameObject item in attachedNetwork)
		{
			MonumentPart component = item.GetComponent<MonumentPart>();
			if (component == null || component.part != requestPart)
			{
				continue;
			}
			return item;
		}
		return null;
	}
}
