using UnityEngine;

public class PatchNotesScreen : KModalScreen
{
	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton okButton;

	[SerializeField]
	private KButton fullPatchNotes;

	[SerializeField]
	private KButton previousVersion;

	[SerializeField]
	private LocText changesLabel;

	private static string m_patchNotesUrl;

	private static string m_patchNotesText;

	private static int PatchNotesVersion = 9;

	private static PatchNotesScreen instance;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		changesLabel.text = m_patchNotesText;
		closeButton.onClick += MarkAsReadAndClose;
		closeButton.soundPlayer.widget_sound_events()[0].OverrideAssetName = "HUD_Click_Close";
		okButton.onClick += MarkAsReadAndClose;
		previousVersion.onClick += delegate
		{
			App.OpenWebURL("http://support.kleientertainment.com/customer/portal/articles/2776550");
		};
		fullPatchNotes.onClick += OnPatchNotesClick;
		instance = this;
	}

	protected override void OnCleanUp()
	{
		instance = null;
	}

	public static bool ShouldShowScreen()
	{
		return KPlayerPrefs.GetInt("PatchNotesVersion") < PatchNotesVersion;
	}

	private void MarkAsReadAndClose()
	{
		KPlayerPrefs.SetInt("PatchNotesVersion", PatchNotesVersion);
		Deactivate();
	}

	public static void UpdatePatchNotes(string patchNotesSummary, string url)
	{
		m_patchNotesUrl = url;
		m_patchNotesText = patchNotesSummary;
		if (instance != null)
		{
			instance.changesLabel.text = m_patchNotesText;
		}
	}

	private void OnPatchNotesClick()
	{
		App.OpenWebURL(m_patchNotesUrl);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
		{
			MarkAsReadAndClose();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}
}
