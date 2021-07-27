using STRINGS;
using UnityEngine;

public class MinionSelectScreen : CharacterSelectionController
{
	[SerializeField]
	private NewBaseScreen newBasePrefab;

	[SerializeField]
	private WattsonMessage wattsonMessagePrefab;

	public const string WattsonGameObjName = "WattsonMessage";

	public KButton backButton;

	protected override void OnPrefabInit()
	{
		base.IsStarterMinion = true;
		base.OnPrefabInit();
		if (MusicManager.instance.SongIsPlaying("Music_FrontEnd"))
		{
			MusicManager.instance.SetSongParameter("Music_FrontEnd", "songSection", 2f);
		}
		GameObject parent = GameObject.Find("ScreenSpaceOverlayCanvas");
		GameObject obj = Util.KInstantiateUI(wattsonMessagePrefab.gameObject, parent);
		obj.name = "WattsonMessage";
		obj.SetActive(value: false);
		Game.Instance.Subscribe(-1992507039, OnBaseAlreadyCreated);
		backButton.onClick += delegate
		{
			LoadScreen.ForceStopGame();
			App.LoadScene("frontend");
		};
		InitializeContainers();
	}

	public void SetProceedButtonActive(bool state, string tooltip = null)
	{
		if (state)
		{
			EnableProceedButton();
		}
		else
		{
			DisableProceedButton();
		}
		ToolTip component = proceedButton.GetComponent<ToolTip>();
		if (component != null)
		{
			if (tooltip != null)
			{
				component.toolTip = tooltip;
			}
			else
			{
				component.ClearMultiStringTooltip();
			}
		}
	}

	protected override void OnSpawn()
	{
		OnDeliverableAdded();
		EnableProceedButton();
		proceedButton.GetComponentInChildren<LocText>().text = UI.IMMIGRANTSCREEN.EMBARK;
		containers.ForEach(delegate(ITelepadDeliverableContainer container)
		{
			CharacterContainer characterContainer = container as CharacterContainer;
			if (characterContainer != null)
			{
				characterContainer.DisableSelectButton();
			}
		});
	}

	protected override void OnProceed()
	{
		Util.KInstantiateUI(newBasePrefab.gameObject, GameScreenManager.Instance.ssOverlayCanvas);
		MusicManager.instance.StopSong("Music_FrontEnd");
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().NewBaseSetupSnapshot);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndWorldGenerationSnapshot);
		selectedDeliverables.Clear();
		foreach (CharacterContainer container in containers)
		{
			selectedDeliverables.Add(container.Stats);
		}
		NewBaseScreen.Instance.Init(SaveLoader.Instance.ClusterLayout, selectedDeliverables.ToArray());
		if (OnProceedEvent != null)
		{
			OnProceedEvent();
		}
		Game.Instance.Trigger(-838649377);
		BuildWatermark.Instance.gameObject.SetActive(value: false);
		Deactivate();
	}

	private void OnBaseAlreadyCreated(object data)
	{
		Game.Instance.StopFE();
		Game.Instance.StartBE();
		Game.Instance.SetGameStarted();
		Deactivate();
	}

	private void ReshuffleAll()
	{
		if (OnReshuffleEvent != null)
		{
			OnReshuffleEvent(base.IsStarterMinion);
		}
	}

	public override void OnPressBack()
	{
		foreach (ITelepadDeliverableContainer container in containers)
		{
			CharacterContainer characterContainer = container as CharacterContainer;
			if (characterContainer != null)
			{
				characterContainer.ForceStopEditingTitle();
			}
		}
	}
}
