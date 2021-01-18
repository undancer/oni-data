using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class DoorToggleSideScreen : SideScreenContent
{
	private struct DoorButtonInfo
	{
		public KToggle button;

		public Door.ControlState state;

		public string currentString;

		public string pendingString;
	}

	[SerializeField]
	private KToggle openButton;

	[SerializeField]
	private KToggle autoButton;

	[SerializeField]
	private KToggle closeButton;

	[SerializeField]
	private LocText description;

	private Door target;

	private AccessControl accessTarget;

	private List<DoorButtonInfo> buttonList = new List<DoorButtonInfo>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		InitButtons();
	}

	private void InitButtons()
	{
		List<DoorButtonInfo> list = buttonList;
		DoorButtonInfo item = new DoorButtonInfo
		{
			button = openButton,
			state = Door.ControlState.Opened,
			currentString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.OPEN,
			pendingString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.OPEN_PENDING
		};
		list.Add(item);
		List<DoorButtonInfo> list2 = buttonList;
		item = new DoorButtonInfo
		{
			button = autoButton,
			state = Door.ControlState.Auto,
			currentString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.AUTO,
			pendingString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.AUTO_PENDING
		};
		list2.Add(item);
		List<DoorButtonInfo> list3 = buttonList;
		item = new DoorButtonInfo
		{
			button = closeButton,
			state = Door.ControlState.Locked,
			currentString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.CLOSE,
			pendingString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.CLOSE_PENDING
		};
		list3.Add(item);
		foreach (DoorButtonInfo info in buttonList)
		{
			info.button.onClick += delegate
			{
				target.QueueStateChange(info.state);
				Refresh();
			};
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<Door>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		if (this.target != null)
		{
			ClearTarget();
		}
		base.SetTarget(target);
		this.target = target.GetComponent<Door>();
		accessTarget = target.GetComponent<AccessControl>();
		if (!(this.target == null))
		{
			target.Subscribe(1734268753, OnDoorStateChanged);
			target.Subscribe(-1525636549, OnAccessControlChanged);
			Refresh();
			base.gameObject.SetActive(value: true);
		}
	}

	public override void ClearTarget()
	{
		if (target != null)
		{
			target.Unsubscribe(1734268753, OnDoorStateChanged);
			target.Unsubscribe(-1525636549, OnAccessControlChanged);
		}
		target = null;
	}

	private void Refresh()
	{
		string text = null;
		string text2 = null;
		if (buttonList == null || buttonList.Count == 0)
		{
			InitButtons();
		}
		foreach (DoorButtonInfo button in buttonList)
		{
			if (target.CurrentState == button.state && target.RequestedState == button.state)
			{
				button.button.isOn = true;
				text = button.currentString;
				ImageToggleState[] componentsInChildren = button.button.GetComponentsInChildren<ImageToggleState>();
				foreach (ImageToggleState imageToggleState in componentsInChildren)
				{
					imageToggleState.SetActive();
					imageToggleState.SetActive();
				}
				button.button.GetComponent<ImageToggleStateThrobber>().enabled = false;
			}
			else if (target.RequestedState == button.state)
			{
				button.button.isOn = true;
				text2 = button.pendingString;
				ImageToggleState[] componentsInChildren2 = button.button.GetComponentsInChildren<ImageToggleState>();
				foreach (ImageToggleState imageToggleState2 in componentsInChildren2)
				{
					imageToggleState2.SetActive();
					imageToggleState2.SetActive();
				}
				button.button.GetComponent<ImageToggleStateThrobber>().enabled = true;
			}
			else
			{
				button.button.isOn = false;
				ImageToggleState[] componentsInChildren3 = button.button.GetComponentsInChildren<ImageToggleState>();
				foreach (ImageToggleState imageToggleState3 in componentsInChildren3)
				{
					imageToggleState3.SetInactive();
					imageToggleState3.SetInactive();
				}
				button.button.GetComponent<ImageToggleStateThrobber>().enabled = false;
			}
		}
		string text3 = text;
		if (text2 != null)
		{
			text3 = string.Format(UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.PENDING_FORMAT, text3, text2);
		}
		if (accessTarget != null && !accessTarget.Online)
		{
			text3 = string.Format(UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.ACCESS_FORMAT, text3, UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.ACCESS_OFFLINE);
		}
		if (target.building.Def.PrefabID == POIDoorInternalConfig.ID)
		{
			text3 = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.POI_INTERNAL;
			foreach (DoorButtonInfo button2 in buttonList)
			{
				button2.button.gameObject.SetActive(value: false);
			}
		}
		else
		{
			foreach (DoorButtonInfo button3 in buttonList)
			{
				bool active = button3.state != 0 || target.allowAutoControl;
				button3.button.gameObject.SetActive(active);
			}
		}
		description.text = text3;
		description.gameObject.SetActive(!string.IsNullOrEmpty(text3));
		ContentContainer.SetActive(!target.isSealed);
	}

	private void OnDoorStateChanged(object data)
	{
		Refresh();
	}

	private void OnAccessControlChanged(object data)
	{
		Refresh();
	}
}
