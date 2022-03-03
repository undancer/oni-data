using System;
using System.Collections;
using UnityEngine;

public class KInputField : KScreen
{
	[SerializeField]
	private KInputTextField inputField;

	public KInputTextField field => inputField;

	public event System.Action onStartEdit;

	public event System.Action onEndEdit;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KInputTextField kInputTextField = inputField;
		kInputTextField.onFocus = (System.Action)Delegate.Combine(kInputTextField.onFocus, new System.Action(OnEditStart));
		inputField.onEndEdit.AddListener(delegate
		{
			OnEditEnd(inputField.text);
		});
	}

	private void OnEditStart()
	{
		base.isEditing = true;
		inputField.Select();
		inputField.ActivateInputField();
		KScreenManager.Instance.RefreshStack();
		if (this.onStartEdit != null)
		{
			this.onStartEdit();
		}
	}

	private void OnEditEnd(string input)
	{
		if (base.gameObject.activeInHierarchy)
		{
			ProcessInput(input);
			StartCoroutine(DelayedEndEdit());
		}
		else
		{
			StopEditing();
		}
	}

	private IEnumerator DelayedEndEdit()
	{
		if (base.isEditing)
		{
			yield return new WaitForEndOfFrame();
			StopEditing();
		}
	}

	private void StopEditing()
	{
		base.isEditing = false;
		inputField.DeactivateInputField();
		if (this.onEndEdit != null)
		{
			this.onEndEdit();
		}
	}

	protected virtual void ProcessInput(string input)
	{
		SetDisplayValue(input);
	}

	public void SetDisplayValue(string input)
	{
		inputField.text = input;
	}
}
