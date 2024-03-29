using System;
using STRINGS;
using UnityEngine;

public class TopLeftControlScreen : KScreen
{
	public static TopLeftControlScreen Instance;

	[SerializeField]
	private MultiToggle SandboxToggle;

	[SerializeField]
	private LocText locText;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Instance = this;
		RefreshName();
		KInputManager.InputChange.AddListener(ResetToolTip);
		UpdateSandboxToggleState();
		MultiToggle sandboxToggle = SandboxToggle;
		sandboxToggle.onClick = (System.Action)Delegate.Combine(sandboxToggle.onClick, new System.Action(OnClickSandboxToggle));
		Game.Instance.Subscribe(-1948169901, delegate
		{
			UpdateSandboxToggleState();
		});
	}

	protected override void OnForcedCleanUp()
	{
		KInputManager.InputChange.RemoveListener(ResetToolTip);
		base.OnForcedCleanUp();
	}

	public void RefreshName()
	{
		if (SaveGame.Instance != null)
		{
			locText.text = SaveGame.Instance.BaseName;
		}
	}

	public void ResetToolTip()
	{
		if (CheckSandboxModeLocked())
		{
			SandboxToggle.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString(UI.SANDBOX_TOGGLE.TOOLTIP_LOCKED, Action.ToggleSandboxTools));
		}
		else
		{
			SandboxToggle.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString(UI.SANDBOX_TOGGLE.TOOLTIP_UNLOCKED, Action.ToggleSandboxTools));
		}
	}

	public void UpdateSandboxToggleState()
	{
		if (CheckSandboxModeLocked())
		{
			SandboxToggle.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString(UI.SANDBOX_TOGGLE.TOOLTIP_LOCKED, Action.ToggleSandboxTools));
			SandboxToggle.ChangeState(0);
		}
		else
		{
			SandboxToggle.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString(UI.SANDBOX_TOGGLE.TOOLTIP_UNLOCKED, Action.ToggleSandboxTools));
			SandboxToggle.ChangeState((!Game.Instance.SandboxModeActive) ? 1 : 2);
		}
		SandboxToggle.gameObject.SetActive(SaveGame.Instance.sandboxEnabled);
	}

	private void OnClickSandboxToggle()
	{
		if (CheckSandboxModeLocked())
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
		}
		else
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
			Game.Instance.SandboxModeActive = !Game.Instance.SandboxModeActive;
		}
		UpdateSandboxToggleState();
	}

	private bool CheckSandboxModeLocked()
	{
		return !SaveGame.Instance.sandboxEnabled;
	}
}
