using System.Collections.Generic;
using Klei;
using STRINGS;
using UnityEngine;

public class CreditsScreen : KModalScreen
{
	public GameObject entryPrefab;

	public Transform entryContainer;

	public KButton CloseButton;

	public TextAsset TeamCreditsFile;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		AddCredits(TeamCreditsFile);
		string[] strings = LocString.GetStrings(typeof(UI.CREDITSSCREEN.THIRD_PARTY));
		foreach (string text in strings)
		{
			Util.KInstantiateUI(entryPrefab, entryContainer.gameObject, force_active: true).GetComponent<LocText>().text = text;
		}
		CloseButton.onClick += Close;
	}

	public void Close()
	{
		Deactivate();
	}

	private void AddCredits(TextAsset csv)
	{
		string[,] array = CSVReader.SplitCsvGrid(csv.text, csv.name);
		List<string> list = new List<string>();
		for (int i = 0; i < array.GetLength(1); i++)
		{
			string text = $"{array[0, i]} {array[1, i]}";
			if (!(text == " "))
			{
				list.Add(text);
			}
		}
		list.Shuffle();
		foreach (string item in list)
		{
			Util.KInstantiateUI(entryPrefab, entryContainer.gameObject, force_active: true).GetComponent<LocText>().text = item;
		}
	}
}
