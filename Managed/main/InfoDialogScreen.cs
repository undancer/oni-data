using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class InfoDialogScreen : KModalScreen
{
	[SerializeField]
	private InfoScreenPlainText subHeaderTemplate;

	[SerializeField]
	private InfoScreenPlainText plainTextTemplate;

	[SerializeField]
	private InfoScreenLineItem lineItemTemplate;

	[SerializeField]
	private InfoScreenSpriteItem spriteItemTemplate;

	[Space(10f)]
	[SerializeField]
	private LocText header;

	[SerializeField]
	private GameObject contentContainer;

	[SerializeField]
	private GameObject leftButtonPrefab;

	[SerializeField]
	private GameObject rightButtonPrefab;

	[SerializeField]
	private GameObject leftButtonPanel;

	[SerializeField]
	private GameObject rightButtonPanel;

	private bool escapeCloses = false;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.SetActive(value: false);
	}

	public override bool IsModal()
	{
		return true;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!escapeCloses)
		{
			e.TryConsume(Action.Escape);
		}
		else if (e.TryConsume(Action.Escape))
		{
			Deactivate();
		}
		else if (PlayerController.Instance != null && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
		{
			Deactivate();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public InfoDialogScreen AddDefaultOK(bool escapeCloses = false)
	{
		AddOption(UI.CONFIRMDIALOG.OK, delegate(InfoDialogScreen d)
		{
			d.Deactivate();
		}, rightSide: true);
		this.escapeCloses = escapeCloses;
		return this;
	}

	public InfoDialogScreen AddDefaultCancel()
	{
		AddOption(UI.CONFIRMDIALOG.CANCEL, delegate(InfoDialogScreen d)
		{
			d.Deactivate();
		});
		escapeCloses = true;
		return this;
	}

	public InfoDialogScreen AddOption(string text, Action<InfoDialogScreen> action, bool rightSide = false)
	{
		GameObject gameObject = Util.KInstantiateUI(rightSide ? rightButtonPrefab : leftButtonPrefab, rightSide ? rightButtonPanel : leftButtonPanel, force_active: true);
		gameObject.gameObject.GetComponentInChildren<LocText>().text = text;
		gameObject.gameObject.GetComponent<KButton>().onClick += delegate
		{
			action(this);
		};
		return this;
	}

	public InfoDialogScreen SetHeader(string header)
	{
		this.header.text = header;
		return this;
	}

	public InfoDialogScreen AddSprite(Sprite sprite)
	{
		InfoScreenSpriteItem infoScreenSpriteItem = Util.KInstantiateUI<InfoScreenSpriteItem>(spriteItemTemplate.gameObject, contentContainer);
		infoScreenSpriteItem.SetSprite(sprite);
		return this;
	}

	public InfoDialogScreen AddPlainText(string text)
	{
		InfoScreenPlainText infoScreenPlainText = Util.KInstantiateUI<InfoScreenPlainText>(plainTextTemplate.gameObject, contentContainer);
		infoScreenPlainText.SetText(text);
		return this;
	}

	public InfoDialogScreen AddLineItem(string text, string tooltip)
	{
		InfoScreenLineItem infoScreenLineItem = Util.KInstantiateUI<InfoScreenLineItem>(lineItemTemplate.gameObject, contentContainer);
		infoScreenLineItem.SetText(text);
		infoScreenLineItem.SetTooltip(tooltip);
		return this;
	}

	public InfoDialogScreen AddSubHeader(string text)
	{
		InfoScreenPlainText infoScreenPlainText = Util.KInstantiateUI<InfoScreenPlainText>(subHeaderTemplate.gameObject, contentContainer);
		infoScreenPlainText.SetText(text);
		return this;
	}

	public InfoDialogScreen AddDescriptors(List<Descriptor> descriptors)
	{
		for (int i = 0; i < descriptors.Count; i++)
		{
			AddLineItem(descriptors[i].IndentedText(), descriptors[i].tooltipText);
		}
		return this;
	}
}
