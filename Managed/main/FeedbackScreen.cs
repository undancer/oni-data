using STRINGS;
using UnityEngine;

public class FeedbackScreen : KModalScreen
{
	public LocText title;

	public KButton dismissButton;

	public KButton closeButton;

	public KButton bugForumsButton;

	public KButton suggestionForumsButton;

	public KButton logsDirectoryButton;

	public KButton saveFilesDirectoryButton;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		title.SetText(UI.FRONTEND.FEEDBACK_SCREEN.TITLE);
		dismissButton.onClick += delegate
		{
			Deactivate();
		};
		closeButton.onClick += delegate
		{
			Deactivate();
		};
		bugForumsButton.onClick += delegate
		{
			Application.OpenURL("https://forums.kleientertainment.com/klei-bug-tracker/oni/");
		};
		suggestionForumsButton.onClick += delegate
		{
			Application.OpenURL("https://forums.kleientertainment.com/forums/forum/133-oxygen-not-included-suggestions-and-feedback/");
		};
		logsDirectoryButton.onClick += delegate
		{
			Application.OpenURL(Util.LogsFolder());
		};
		saveFilesDirectoryButton.onClick += delegate
		{
			Application.OpenURL(SaveLoader.GetSavePrefix());
		};
	}
}
