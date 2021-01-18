public interface ILogicRibbonBitSelector
{
	string SideScreenTitle
	{
		get;
	}

	string SideScreenDescription
	{
		get;
	}

	void SetBitSelection(int bit);

	int GetBitSelection();

	int GetBitDepth();

	bool SideScreenDisplayWriterDescription();

	bool SideScreenDisplayReaderDescription();

	bool IsBitActive(int bit);

	int GetOutputValue();

	int GetInputValue();

	void UpdateVisuals();
}
