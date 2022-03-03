using UnityEngine;

public class RocketRestrictionSideScreen : SideScreenContent
{
	private RocketControlStation controlStation;

	[Header("Buttons")]
	public KToggle unrestrictedButton;

	public KToggle spaceRestrictedButton;

	public GameObject automationControlled;

	private int controlStationLogicSubHandle = -1;

	protected override void OnSpawn()
	{
		unrestrictedButton.onClick += ClickNone;
		spaceRestrictedButton.onClick += ClickSpace;
	}

	public override int GetSideScreenSortOrder()
	{
		return 0;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetSMI<RocketControlStation.StatesInstance>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		controlStation = new_target.GetComponent<RocketControlStation>();
		controlStationLogicSubHandle = controlStation.Subscribe(1861523068, UpdateButtonStates);
		UpdateButtonStates();
	}

	public override void ClearTarget()
	{
		if (controlStationLogicSubHandle != -1 && controlStation != null)
		{
			controlStation.Unsubscribe(controlStationLogicSubHandle);
			controlStationLogicSubHandle = -1;
		}
		controlStation = null;
	}

	private void UpdateButtonStates(object data = null)
	{
		bool flag = controlStation.IsLogicInputConnected();
		if (!flag)
		{
			unrestrictedButton.isOn = !controlStation.RestrictWhenGrounded;
			spaceRestrictedButton.isOn = controlStation.RestrictWhenGrounded;
		}
		unrestrictedButton.gameObject.SetActive(!flag);
		spaceRestrictedButton.gameObject.SetActive(!flag);
		automationControlled.gameObject.SetActive(flag);
	}

	private void ClickNone()
	{
		controlStation.RestrictWhenGrounded = false;
		UpdateButtonStates();
	}

	private void ClickSpace()
	{
		controlStation.RestrictWhenGrounded = true;
		UpdateButtonStates();
	}
}
