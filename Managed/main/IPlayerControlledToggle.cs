public interface IPlayerControlledToggle
{
	string SideScreenTitleKey
	{
		get;
	}

	bool ToggleRequested
	{
		get;
		set;
	}

	void ToggledByPlayer();

	bool ToggledOn();

	KSelectable GetSelectable();
}
