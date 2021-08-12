using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicViewer : KScreen
{
	public GameObject panelPrefab;

	public GameObject contentContainer;

	public List<GameObject> activePanels = new List<GameObject>();

	public KButton closeButton;

	public System.Action OnStop;

	public void ShowComic(ComicData comic, bool isVictoryComic)
	{
		for (int i = 0; i < Mathf.Max(comic.images.Length, comic.stringKeys.Length); i++)
		{
			GameObject gameObject = Util.KInstantiateUI(panelPrefab, contentContainer, force_active: true);
			activePanels.Add(gameObject);
			gameObject.GetComponentInChildren<Image>().sprite = comic.images[i];
			gameObject.GetComponentInChildren<LocText>().SetText(comic.stringKeys[i]);
		}
		closeButton.ClearOnClick();
		if (isVictoryComic)
		{
			closeButton.onClick += delegate
			{
				Stop();
				Show(show: false);
			};
		}
		else
		{
			closeButton.onClick += delegate
			{
				Stop();
			};
		}
	}

	public void Stop()
	{
		OnStop();
		Show(show: false);
		base.gameObject.SetActive(value: false);
	}
}
