public interface ISidescreenButtonControl
{
	string SidescreenButtonText { get; }

	string SidescreenButtonTooltip { get; }

	bool SidescreenEnabled();

	bool SidescreenButtonInteractable();

	void OnSidescreenButtonPressed();

	int ButtonSideScreenSortOrder();
}
