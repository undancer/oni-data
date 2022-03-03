using STRINGS;

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
			App.OpenWebURL("https://forums.kleientertainment.com/klei-bug-tracker/oni/");
		};
		suggestionForumsButton.onClick += delegate
		{
			App.OpenWebURL("https://forums.kleientertainment.com/forums/forum/133-oxygen-not-included-suggestions-and-feedback/");
		};
		logsDirectoryButton.onClick += delegate
		{
			App.OpenWebURL(Util.LogsFolder());
		};
		saveFilesDirectoryButton.onClick += delegate
		{
			App.OpenWebURL(SaveLoader.GetSavePrefix());
		};
	}
}
