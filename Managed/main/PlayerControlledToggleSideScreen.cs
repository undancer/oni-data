using UnityEngine;

public class PlayerControlledToggleSideScreen : SideScreenContent, IRenderEveryTick
{
	public IPlayerControlledToggle target;

	public KButton toggleButton;

	protected static readonly HashedString[] ON_ANIMS = new HashedString[2]
	{
		"on_pre",
		"on"
	};

	protected static readonly HashedString[] OFF_ANIMS = new HashedString[2]
	{
		"off_pre",
		"off"
	};

	public float animScaleBase = 0.25f;

	private StatusItem togglePendingStatusItem;

	[SerializeField]
	private KBatchedAnimController kbac;

	private float lastKeyboardShortcutTime;

	private const float KEYBOARD_COOLDOWN = 0.1f;

	private bool keyDown = false;

	private bool currentState = false;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		toggleButton.onClick += ClickToggle;
		togglePendingStatusItem = new StatusItem("PlayerControlledToggleSideScreen", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<IPlayerControlledToggle>() != null;
	}

	public void RenderEveryTick(float dt)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (!keyDown && (Input.GetKeyDown(KeyCode.Return) & (Time.unscaledTime - lastKeyboardShortcutTime > 0.1f)))
		{
			if (SpeedControlScreen.Instance.IsPaused)
			{
				RequestToggle();
			}
			else
			{
				Toggle();
			}
			lastKeyboardShortcutTime = Time.unscaledTime;
			keyDown = true;
		}
		if (keyDown && Input.GetKeyUp(KeyCode.Return))
		{
			keyDown = false;
		}
	}

	private void ClickToggle()
	{
		if (SpeedControlScreen.Instance.IsPaused)
		{
			RequestToggle();
		}
		else
		{
			Toggle();
		}
	}

	private void RequestToggle()
	{
		target.ToggleRequested = !target.ToggleRequested;
		if (target.ToggleRequested && SpeedControlScreen.Instance.IsPaused)
		{
			target.GetSelectable().SetStatusItem(Db.Get().StatusItemCategories.Main, togglePendingStatusItem, this);
		}
		else
		{
			target.GetSelectable().SetStatusItem(Db.Get().StatusItemCategories.Main, null);
		}
		UpdateVisuals(target.ToggleRequested ? (!target.ToggledOn()) : target.ToggledOn(), smooth: true);
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		target = new_target.GetComponent<IPlayerControlledToggle>();
		if (target == null)
		{
			Debug.LogError("The gameObject received is not an IPlayerControlledToggle");
			return;
		}
		UpdateVisuals(target.ToggleRequested ? (!target.ToggledOn()) : target.ToggledOn(), smooth: false);
		titleKey = target.SideScreenTitleKey;
	}

	private void Toggle()
	{
		target.ToggledByPlayer();
		UpdateVisuals(target.ToggledOn(), smooth: true);
		target.ToggleRequested = false;
		target.GetSelectable().RemoveStatusItem(togglePendingStatusItem);
	}

	private void UpdateVisuals(bool state, bool smooth)
	{
		if (state != currentState)
		{
			if (smooth)
			{
				kbac.Play(state ? ON_ANIMS : OFF_ANIMS);
			}
			else
			{
				kbac.Play(state ? ON_ANIMS[1] : OFF_ANIMS[1]);
			}
		}
		currentState = state;
	}
}
