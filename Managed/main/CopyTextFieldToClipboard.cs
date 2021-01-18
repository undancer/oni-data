using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CopyTextFieldToClipboard")]
public class CopyTextFieldToClipboard : KMonoBehaviour
{
	public KButton button;

	public Func<string> GetText;

	protected override void OnPrefabInit()
	{
		button.onClick += OnClick;
	}

	private void OnClick()
	{
		TextEditor textEditor = new TextEditor();
		textEditor.text = GetText();
		textEditor.SelectAll();
		textEditor.Copy();
	}
}
