public class DevPump : Filterable, ISim1000ms
{
	public ElementState elementState = ElementState.Liquid;

	[MyCmpReq]
	private Storage storage;

	private Element element
	{
		get
		{
			if (base.SelectedTag.IsValid)
			{
				return ElementLoader.GetElement(base.SelectedTag);
			}
			return ElementLoader.FindElementByHash(SimHashes.Void);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (elementState == ElementState.Liquid)
		{
			base.SelectedTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		}
		else if (elementState == ElementState.Gas)
		{
			base.SelectedTag = ElementLoader.FindElementByHash(SimHashes.Oxygen).tag;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		filterElementState = elementState;
	}

	public void Sim1000ms(float dt)
	{
		float num = 10f - storage.GetAmountAvailable(element.tag);
		if (!(num <= 0f))
		{
			if (element.IsLiquid)
			{
				storage.AddLiquid(element.id, num, element.defaultValues.temperature, byte.MaxValue, 0);
			}
			else if (element.IsGas)
			{
				storage.AddGasChunk(element.id, num, element.defaultValues.temperature, byte.MaxValue, 0, keep_zero_mass: false);
			}
		}
	}
}
