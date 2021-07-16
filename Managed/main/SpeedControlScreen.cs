using System;
using System.Collections;
using FMOD.Studio;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class SpeedControlScreen : KScreen
{
	public GameObject playButtonWidget;

	public GameObject pauseButtonWidget;

	public Image playIcon;

	public Image pauseIcon;

	[SerializeField]
	private TextStyleSetting TooltipTextStyle;

	public GameObject speedButtonWidget_slow;

	public GameObject speedButtonWidget_medium;

	public GameObject speedButtonWidget_fast;

	public GameObject mainMenuWidget;

	public float normalSpeed;

	public float fastSpeed;

	public float ultraSpeed;

	private KToggle pauseButton;

	private KToggle slowButton;

	private KToggle mediumButton;

	private KToggle fastButton;

	private int speed;

	private int pauseCount;

	private float stepTime;

	public static SpeedControlScreen Instance
	{
		get;
		private set;
	}

	public bool IsPaused => pauseCount > 0;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		pauseButton = pauseButtonWidget.GetComponent<KToggle>();
		slowButton = speedButtonWidget_slow.GetComponent<KToggle>();
		mediumButton = speedButtonWidget_medium.GetComponent<KToggle>();
		fastButton = speedButtonWidget_fast.GetComponent<KToggle>();
		KToggle[] array = new KToggle[4]
		{
			pauseButton,
			slowButton,
			mediumButton,
			fastButton
		};
		for (int i = 0; i < array.Length; i++)
		{
			array[i].soundPlayer.Enabled = false;
		}
		slowButton.onClick += delegate
		{
			PlaySpeedChangeSound(1f);
			SetSpeed(0);
		};
		mediumButton.onClick += delegate
		{
			PlaySpeedChangeSound(2f);
			SetSpeed(1);
		};
		fastButton.onClick += delegate
		{
			PlaySpeedChangeSound(3f);
			SetSpeed(2);
		};
		pauseButton.onClick += delegate
		{
			TogglePause();
		};
		speedButtonWidget_slow.GetComponent<ToolTip>().AddMultiStringTooltip(GameUtil.ReplaceHotkeyString(UI.TOOLTIPS.SPEEDBUTTON_SLOW, Action.CycleSpeed), TooltipTextStyle);
		speedButtonWidget_medium.GetComponent<ToolTip>().AddMultiStringTooltip(GameUtil.ReplaceHotkeyString(UI.TOOLTIPS.SPEEDBUTTON_MEDIUM, Action.CycleSpeed), TooltipTextStyle);
		speedButtonWidget_fast.GetComponent<ToolTip>().AddMultiStringTooltip(GameUtil.ReplaceHotkeyString(UI.TOOLTIPS.SPEEDBUTTON_FAST, Action.CycleSpeed), TooltipTextStyle);
		playButtonWidget.GetComponent<KButton>().onClick += delegate
		{
			TogglePause();
		};
	}

	protected override void OnSpawn()
	{
		if (SaveGame.Instance != null)
		{
			speed = SaveGame.Instance.GetSpeed();
			SetSpeed(speed);
		}
		base.OnSpawn();
		OnChanged();
	}

	public int GetSpeed()
	{
		return speed;
	}

	public void SetSpeed(int Speed)
	{
		speed = Speed % 3;
		switch (speed)
		{
		case 0:
			slowButton.Select();
			slowButton.isOn = true;
			mediumButton.isOn = false;
			fastButton.isOn = false;
			break;
		case 1:
			mediumButton.Select();
			slowButton.isOn = false;
			mediumButton.isOn = true;
			fastButton.isOn = false;
			break;
		case 2:
			fastButton.Select();
			slowButton.isOn = false;
			mediumButton.isOn = false;
			fastButton.isOn = true;
			break;
		}
		OnSpeedChange();
	}

	public void ToggleRidiculousSpeed()
	{
		if (ultraSpeed == 3f)
		{
			ultraSpeed = 10f;
		}
		else
		{
			ultraSpeed = 3f;
		}
		speed = 2;
		OnChanged();
	}

	public void TogglePause(bool playsound = true)
	{
		if (IsPaused)
		{
			Unpause(playsound);
		}
		else
		{
			Pause(playsound);
		}
	}

	public void Pause(bool playSound = true)
	{
		pauseCount++;
		if (pauseCount != 1)
		{
			return;
		}
		if (playSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Speed_Pause"));
			if (SoundListenerController.Instance != null)
			{
				SoundListenerController.Instance.SetLoopingVolume(0f);
			}
		}
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().SpeedPausedMigrated);
		MusicManager.instance.SetDynamicMusicPaused();
		pauseButtonWidget.GetComponent<ToolTip>().ClearMultiStringTooltip();
		pauseButtonWidget.GetComponent<ToolTip>().AddMultiStringTooltip(GameUtil.ReplaceHotkeyString(UI.TOOLTIPS.UNPAUSE, Action.TogglePause), TooltipTextStyle);
		pauseButton.isOn = true;
		OnPause();
	}

	public void Unpause(bool playSound = true)
	{
		pauseCount = Mathf.Max(0, pauseCount - 1);
		if (pauseCount != 0)
		{
			return;
		}
		if (playSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Speed_Unpause"));
			if (SoundListenerController.Instance != null)
			{
				SoundListenerController.Instance.SetLoopingVolume(1f);
			}
		}
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().SpeedPausedMigrated);
		MusicManager.instance.SetDynamicMusicUnpaused();
		pauseButtonWidget.GetComponent<ToolTip>().ClearMultiStringTooltip();
		pauseButtonWidget.GetComponent<ToolTip>().AddMultiStringTooltip(GameUtil.ReplaceHotkeyString(UI.TOOLTIPS.PAUSE, Action.TogglePause), TooltipTextStyle);
		pauseButton.isOn = false;
		SetSpeed(speed);
		OnPlay();
	}

	private void OnPause()
	{
		OnChanged();
	}

	private void OnPlay()
	{
		OnChanged();
	}

	public void OnSpeedChange()
	{
		if (!Game.IsQuitting())
		{
			OnChanged();
		}
	}

	private void OnChanged()
	{
		if (IsPaused)
		{
			Time.timeScale = 0f;
		}
		else if (speed == 0)
		{
			Time.timeScale = normalSpeed;
		}
		else if (speed == 1)
		{
			Time.timeScale = fastSpeed;
		}
		else if (speed == 2)
		{
			Time.timeScale = ultraSpeed;
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.TogglePause))
		{
			TogglePause();
		}
		else if (e.TryConsume(Action.CycleSpeed))
		{
			PlaySpeedChangeSound((speed + 1) % 3 + 1);
			SetSpeed(speed + 1);
			OnSpeedChange();
		}
		else if (e.TryConsume(Action.SpeedUp))
		{
			speed++;
			speed = Math.Min(speed, 2);
			SetSpeed(speed);
		}
		else if (e.TryConsume(Action.SlowDown))
		{
			speed--;
			speed = Math.Max(speed, 0);
			SetSpeed(speed);
		}
	}

	private void PlaySpeedChangeSound(float speed)
	{
		string sound = GlobalAssets.GetSound("Speed_Change");
		if (sound != null)
		{
			EventInstance instance = SoundEvent.BeginOneShot(sound, Vector3.zero);
			instance.setParameterByName("Speed", speed);
			SoundEvent.EndOneShot(instance);
		}
	}

	public void DebugStepFrame()
	{
		DebugUtil.LogArgs($"Stepping one frame {GameClock.Instance.GetTime()} ({GameClock.Instance.GetTime() / 600f})");
		stepTime = Time.time;
		Unpause(playSound: false);
		StartCoroutine(DebugStepFrameDelay());
	}

	private IEnumerator DebugStepFrameDelay()
	{
		yield return null;
		DebugUtil.LogArgs("Stepped one frame", Time.time - stepTime, "seconds");
		Pause(playSound: false);
	}
}
