public interface IFuelTank
{
	IStorage Storage
	{
		get;
	}

	bool ConsumeFuelOnLand
	{
		get;
	}

	void DEBUG_FillTank();
}
