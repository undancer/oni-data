using System;
using System.Collections;
using STRINGS;
using UnityEngine;

public class EditableTitleBar : TitleBar
{
	public KButton editNameButton;

	public KButton randomNameButton;

	public KInputTextField inputField;

	private Coroutine postEndEdit;

	private Coroutine preToggleNameEditing;

	public event Action<string> OnNameChanged;

	public event System.Action OnStartedEditing;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (randomNameButton != null)
		{
			randomNameButton.onClick += GenerateRandomName;
		}
		if (editNameButton != null)
		{
			EnableEditButtonClick();
		}
		if (inputField != null)
		{
			inputField.onEndEdit.AddListener(OnEndEdit);
		}
	}

	public void UpdateRenameTooltip(GameObject target)
	{
		if (editNameButton != null && target != null)
		{
			if (target.GetComponent<MinionBrain>() != null)
			{
				editNameButton.GetComponent<ToolTip>().toolTip = UI.TOOLTIPS.EDITNAME;
			}
			else
			{
				editNameButton.GetComponent<ToolTip>().toolTip = string.Format(UI.TOOLTIPS.EDITNAMEGENERIC, target.GetProperName());
			}
		}
	}

	private void OnEndEdit(string finalStr)
	{
		finalStr = Localization.FilterDirtyWords(finalStr);
		SetEditingState(state: false);
		if (!string.IsNullOrEmpty(finalStr))
		{
			if (this.OnNameChanged != null)
			{
				this.OnNameChanged(finalStr);
			}
			titleText.text = finalStr;
			if (postEndEdit != null)
			{
				StopCoroutine(postEndEdit);
			}
			if (base.gameObject.activeInHierarchy && base.enabled)
			{
				postEndEdit = StartCoroutine(PostOnEndEditRoutine());
			}
		}
	}

	private IEnumerator PostOnEndEditRoutine()
	{
		int i = 0;
		while (i < 10)
		{
			i++;
			yield return new WaitForEndOfFrame();
		}
		EnableEditButtonClick();
		if (randomNameButton != null)
		{
			randomNameButton.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator PreToggleNameEditingRoutine()
	{
		yield return new WaitForEndOfFrame();
		ToggleNameEditing();
		preToggleNameEditing = null;
	}

	private void EnableEditButtonClick()
	{
		editNameButton.onClick += delegate
		{
			if (preToggleNameEditing == null)
			{
				preToggleNameEditing = StartCoroutine(PreToggleNameEditingRoutine());
			}
		};
	}

	private void GenerateRandomName()
	{
		if (postEndEdit != null)
		{
			StopCoroutine(postEndEdit);
		}
		string text = GameUtil.GenerateRandomDuplicantName();
		if (this.OnNameChanged != null)
		{
			this.OnNameChanged(text);
		}
		titleText.text = text;
		SetEditingState(state: true);
	}

	private void ToggleNameEditing()
	{
		editNameButton.ClearOnClick();
		bool flag = !inputField.gameObject.activeInHierarchy;
		if (randomNameButton != null)
		{
			randomNameButton.gameObject.SetActive(flag);
		}
		SetEditingState(flag);
	}

	private void SetEditingState(bool state)
	{
		titleText.gameObject.SetActive(!state);
		if (setCameraControllerState)
		{
			CameraController.Instance.DisableUserCameraControl = state;
		}
		if (inputField == null)
		{
			return;
		}
		inputField.gameObject.SetActive(state);
		if (state)
		{
			inputField.text = titleText.text;
			inputField.Select();
			inputField.ActivateInputField();
			if (this.OnStartedEditing != null)
			{
				this.OnStartedEditing();
			}
		}
		else
		{
			inputField.DeactivateInputField();
		}
	}

	public void ForceStopEditing()
	{
		if (postEndEdit != null)
		{
			StopCoroutine(postEndEdit);
		}
		editNameButton.ClearOnClick();
		SetEditingState(state: false);
		EnableEditButtonClick();
	}

	public void SetUserEditable(bool editable)
	{
		userEditable = editable;
		editNameButton.gameObject.SetActive(editable);
		editNameButton.ClearOnClick();
		EnableEditButtonClick();
	}
}
