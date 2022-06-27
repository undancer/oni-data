using STRINGS;
using UnityEngine;

public class ImmigrantScreen : CharacterSelectionController
{
	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton rejectButton;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private GameObject rejectConfirmationScreen;

	[SerializeField]
	private KButton confirmRejectionBtn;

	[SerializeField]
	private KButton cancelRejectionBtn;

	public static ImmigrantScreen instance;

	private bool hasShown;

	public Telepad telepad { get; private set; }

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		activateOnSpawn = false;
		base.ConsumeMouseScroll = false;
		base.OnSpawn();
		base.IsStarterMinion = false;
		rejectButton.onClick += OnRejectAll;
		confirmRejectionBtn.onClick += OnRejectionConfirmed;
		cancelRejectionBtn.onClick += OnRejectionCancelled;
		instance = this;
		title.text = UI.IMMIGRANTSCREEN.IMMIGRANTSCREENTITLE;
		proceedButton.GetComponentInChildren<LocText>().text = UI.IMMIGRANTSCREEN.PROCEEDBUTTON;
		closeButton.onClick += delegate
		{
			Show(show: false);
		};
		Show(show: false);
	}

	protected override void OnShow(bool show)
	{
		if (show)
		{
			KFMOD.PlayUISound(GlobalAssets.GetSound("Dialog_Popup"));
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
			MusicManager.instance.PlaySong("Music_SelectDuplicant");
			hasShown = true;
		}
		else
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
			if (MusicManager.instance.SongIsPlaying("Music_SelectDuplicant"))
			{
				MusicManager.instance.StopSong("Music_SelectDuplicant");
			}
			if (Immigration.Instance.ImmigrantsAvailable && hasShown)
			{
				AudioMixer.instance.Start(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot);
			}
		}
		base.OnShow(show);
	}

	public void DebugShuffleOptions()
	{
		OnRejectionConfirmed();
		Immigration.Instance.timeBeforeSpawn = 0f;
	}

	public override void OnPressBack()
	{
		if (rejectConfirmationScreen.activeSelf)
		{
			OnRejectionCancelled();
		}
		else
		{
			base.OnPressBack();
		}
	}

	public override void Deactivate()
	{
		Show(show: false);
	}

	public static void InitializeImmigrantScreen(Telepad telepad)
	{
		instance.Initialize(telepad);
		instance.Show();
	}

	private void Initialize(Telepad telepad)
	{
		InitializeContainers();
		foreach (ITelepadDeliverableContainer container in containers)
		{
			CharacterContainer characterContainer = container as CharacterContainer;
			if (characterContainer != null)
			{
				characterContainer.SetReshufflingState(enable: false);
			}
		}
		this.telepad = telepad;
	}

	protected override void OnProceed()
	{
		telepad.OnAcceptDelivery(selectedDeliverables[0]);
		Show(show: false);
		containers.ForEach(delegate(ITelepadDeliverableContainer cc)
		{
			Object.Destroy(cc.GetGameObject());
		});
		containers.Clear();
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot);
		MusicManager.instance.PlaySong("Stinger_NewDuplicant");
	}

	private void OnRejectAll()
	{
		rejectConfirmationScreen.transform.SetAsLastSibling();
		rejectConfirmationScreen.SetActive(value: true);
	}

	private void OnRejectionCancelled()
	{
		rejectConfirmationScreen.SetActive(value: false);
	}

	private void OnRejectionConfirmed()
	{
		telepad.RejectAll();
		containers.ForEach(delegate(ITelepadDeliverableContainer cc)
		{
			Object.Destroy(cc.GetGameObject());
		});
		containers.Clear();
		rejectConfirmationScreen.SetActive(value: false);
		Show(show: false);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot);
	}
}
