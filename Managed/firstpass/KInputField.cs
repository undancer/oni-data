using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class KInputField : KScreen
{
	private bool isEditing;

	[SerializeField]
	private TMP_InputField inputField;

	public TMP_InputField field => inputField;

	public event System.Action onStartEdit;

	public event System.Action onEndEdit;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		TMP_InputField tMP_InputField = inputField;
		tMP_InputField.onFocus = (System.Action)Delegate.Combine(tMP_InputField.onFocus, new System.Action(OnEditStart));
		inputField.onEndEdit.AddListener(delegate
		{
			OnEditEnd(inputField.text);
		});
	}

	private void OnEditStart()
	{
		isEditing = true;
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
		if (isEditing)
		{
			yield return new WaitForEndOfFrame();
			StopEditing();
		}
	}

	private void StopEditing()
	{
		isEditing = false;
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

	public override void OnKeyDown(KButtonEvent e)
	{
		if (isEditing)
		{
			e.Consumed = true;
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public override float GetSortKey()
	{
		if (isEditing)
		{
			return 10f;
		}
		return base.GetSortKey();
	}
}
