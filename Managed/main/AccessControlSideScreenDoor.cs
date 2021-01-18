using System;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AccessControlSideScreenDoor")]
public class AccessControlSideScreenDoor : KMonoBehaviour
{
	public KToggle leftButton;

	public KToggle rightButton;

	private Action<MinionAssignablesProxy, AccessControl.Permission> permissionChangedCallback;

	private bool isUpDown;

	protected MinionAssignablesProxy targetIdentity;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		leftButton.onClick += OnPermissionButtonClicked;
		rightButton.onClick += OnPermissionButtonClicked;
	}

	private void OnPermissionButtonClicked()
	{
		AccessControl.Permission arg = (leftButton.isOn ? ((!rightButton.isOn) ? AccessControl.Permission.GoLeft : AccessControl.Permission.Both) : ((!rightButton.isOn) ? AccessControl.Permission.Neither : AccessControl.Permission.GoRight));
		UpdateButtonStates(isDefault: false);
		permissionChangedCallback(targetIdentity, arg);
	}

	protected virtual void UpdateButtonStates(bool isDefault)
	{
		ToolTip component = leftButton.GetComponent<ToolTip>();
		ToolTip component2 = rightButton.GetComponent<ToolTip>();
		if (isUpDown)
		{
			component.SetSimpleTooltip(leftButton.isOn ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_UP_ENABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_UP_DISABLED);
			component2.SetSimpleTooltip(rightButton.isOn ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_DOWN_ENABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_DOWN_DISABLED);
		}
		else
		{
			component.SetSimpleTooltip(leftButton.isOn ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_LEFT_ENABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_LEFT_DISABLED);
			component2.SetSimpleTooltip(rightButton.isOn ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_RIGHT_ENABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_RIGHT_DISABLED);
		}
	}

	public void SetRotated(bool rotated)
	{
		isUpDown = rotated;
	}

	public void SetContent(AccessControl.Permission permission, Action<MinionAssignablesProxy, AccessControl.Permission> onPermissionChange)
	{
		permissionChangedCallback = onPermissionChange;
		leftButton.isOn = permission == AccessControl.Permission.Both || permission == AccessControl.Permission.GoLeft;
		rightButton.isOn = permission == AccessControl.Permission.Both || permission == AccessControl.Permission.GoRight;
		UpdateButtonStates(isDefault: false);
	}
}
