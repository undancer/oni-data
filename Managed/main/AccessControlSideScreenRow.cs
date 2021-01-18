using System;
using STRINGS;
using UnityEngine;

public class AccessControlSideScreenRow : AccessControlSideScreenDoor
{
	[SerializeField]
	private CrewPortrait crewPortraitPrefab;

	private CrewPortrait portraitInstance;

	public KToggle defaultButton;

	public GameObject defaultControls;

	public GameObject customControls;

	private Action<MinionAssignablesProxy, bool> defaultClickedCallback;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		defaultButton.onValueChanged += OnDefaultButtonChanged;
	}

	private void OnDefaultButtonChanged(bool state)
	{
		UpdateButtonStates(!state);
		if (defaultClickedCallback != null)
		{
			defaultClickedCallback(targetIdentity, !state);
		}
	}

	protected override void UpdateButtonStates(bool isDefault)
	{
		base.UpdateButtonStates(isDefault);
		defaultButton.GetComponent<ToolTip>().SetSimpleTooltip(isDefault ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.SET_TO_CUSTOM : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.SET_TO_DEFAULT);
		defaultControls.SetActive(isDefault);
		customControls.SetActive(!isDefault);
	}

	public void SetMinionContent(MinionAssignablesProxy identity, AccessControl.Permission permission, bool isDefault, Action<MinionAssignablesProxy, AccessControl.Permission> onPermissionChange, Action<MinionAssignablesProxy, bool> onDefaultClick)
	{
		SetContent(permission, onPermissionChange);
		if (identity == null)
		{
			Debug.LogError("Invalid data received.");
			return;
		}
		if (portraitInstance == null)
		{
			portraitInstance = Util.KInstantiateUI<CrewPortrait>(crewPortraitPrefab.gameObject, defaultButton.gameObject);
			portraitInstance.SetAlpha(1f);
		}
		targetIdentity = identity;
		portraitInstance.SetIdentityObject(identity, jobEnabled: false);
		portraitInstance.SetSubTitle(isDefault ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.USING_DEFAULT : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.USING_CUSTOM);
		defaultClickedCallback = null;
		defaultButton.isOn = !isDefault;
		defaultClickedCallback = onDefaultClick;
	}
}
