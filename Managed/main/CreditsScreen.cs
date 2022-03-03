using System.Collections.Generic;
using Klei;
using UnityEngine;

public class CreditsScreen : KModalScreen
{
	public GameObject entryPrefab;

	public GameObject teamHeaderPrefab;

	private Dictionary<string, GameObject> teamContainers = new Dictionary<string, GameObject>();

	public Transform entryContainer;

	public KButton CloseButton;

	public TextAsset[] creditsFiles;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		TextAsset[] array = creditsFiles;
		foreach (TextAsset csv in array)
		{
			AddCredits(csv);
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
		for (int i = 1; i < array.GetLength(1); i++)
		{
			string text = $"{array[0, i]} {array[1, i]}";
			if (!(text == " "))
			{
				list.Add(text);
			}
		}
		list.Shuffle();
		string text2 = array[0, 0];
		GameObject gameObject = Util.KInstantiateUI(teamHeaderPrefab, entryContainer.gameObject, force_active: true);
		gameObject.GetComponent<LocText>().text = text2;
		teamContainers.Add(text2, gameObject);
		foreach (string item in list)
		{
			Util.KInstantiateUI(entryPrefab, teamContainers[text2], force_active: true).GetComponent<LocText>().text = item;
		}
	}
}
