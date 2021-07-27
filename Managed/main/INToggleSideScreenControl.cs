using System.Collections.Generic;

public interface INToggleSideScreenControl
{
	string SidescreenTitleKey { get; }

	List<LocString> Options { get; }

	List<LocString> Tooltips { get; }

	string Description { get; }

	int SelectedOption { get; }

	int QueuedOption { get; }

	void QueueSelectedOption(int option);
}
