using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteListDialogScreen : KModalScreen
{
	private struct Button
	{
		public System.Action action;

		public GameObject gameObject;

		public string label;
	}

	public System.Action onDeactivateCB;

	[SerializeField]
	private GameObject buttonPrefab;

	[SerializeField]
	private GameObject buttonPanel;

	[SerializeField]
	private LocText titleText;

	[SerializeField]
	private LocText popupMessage;

	[SerializeField]
	private GameObject listPanel;

	[SerializeField]
	private GameObject listPrefab;

	private List<Button> buttons;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.SetActive(value: false);
		buttons = new List<Button>();
	}

	public override bool IsModal()
	{
		return true;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape))
		{
			Deactivate();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public void AddOption(string text, System.Action action)
	{
		GameObject gameObject = Util.KInstantiateUI(buttonPrefab, buttonPanel, force_active: true);
		buttons.Add(new Button
		{
			label = text,
			action = action,
			gameObject = gameObject
		});
	}

	public void AddSprite(Sprite sprite, string text)
	{
		GameObject gameObject = Util.KInstantiateUI(listPrefab, listPanel, force_active: true);
		gameObject.GetComponentInChildren<LocText>().text = text;
		Image componentInChildren = gameObject.GetComponentInChildren<Image>();
		componentInChildren.sprite = sprite;
		float num2 = (componentInChildren.gameObject.GetComponent<AspectRatioFitter>().aspectRatio = sprite.rect.width / sprite.rect.height);
	}

	public void PopupConfirmDialog(string text, string title_text = null)
	{
		foreach (Button button in buttons)
		{
			button.gameObject.GetComponentInChildren<LocText>().text = button.label;
			button.gameObject.GetComponent<KButton>().onClick += button.action;
		}
		if (title_text != null)
		{
			titleText.text = title_text;
		}
		popupMessage.text = text;
	}

	protected override void OnDeactivate()
	{
		if (onDeactivateCB != null)
		{
			onDeactivateCB();
		}
		base.OnDeactivate();
	}
}
