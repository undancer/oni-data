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
		bool num = GetMonumentPart(Part.Top) != null;
		bool flag = GetMonumentPart(Part.Middle) != null;
		bool flag2 = GetMonumentPart(Part.Bottom) != null;
		return num && flag2 && flag;
	}

	public void UpdateMonumentDecor()
	{
		GameObject monumentPart = GetMonumentPart(Part.Middle);
		if (!IsMonumentCompleted())
		{
			return;
		}
		monumentPart.GetComponent<DecorProvider>().SetValues(BUILDINGS.DECOR.BONUS.MONUMENT.COMPLETE);
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>()))
		{
			if (item != monumentPart)
			{
				item.GetComponent<DecorProvider>().SetValues(BUILDINGS.DECOR.NONE);
			}
		}
	}

	public void RemoveMonumentPiece()
	{
		if (!IsMonumentCompleted())
		{
			return;
		}
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>()))
		{
			if (item.GetComponent<MonumentPart>() != this)
			{
				item.GetComponent<DecorProvider>().SetValues(BUILDINGS.DECOR.BONUS.MONUMENT.INCOMPLETE);
			}
		}
	}

	private GameObject GetMonumentPart(Part requestPart)
	{
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>()))
		{
			MonumentPart component = item.GetComponent<MonumentPart>();
			if (!(component == null) && component.part == requestPart)
			{
				return item;
			}
		}
		return null;
	}
}
