using System.Collections.Generic;
using UnityEngine;

public class PlantElementAbsorbers : KCompactedVector<PlantElementAbsorber>
{
	private bool updating;

	private List<HandleVector<int>.Handle> queuedRemoves = new List<HandleVector<int>.Handle>();

	public HandleVector<int>.Handle Add(Storage storage, PlantElementAbsorber.ConsumeInfo[] consumed_elements)
	{
		if (consumed_elements == null || consumed_elements.Length == 0)
		{
			return HandleVector<int>.InvalidHandle;
		}
		HandleVector<int>.Handle[] array = new HandleVector<int>.Handle[consumed_elements.Length];
		for (int i = 0; i < consumed_elements.Length; i++)
		{
			array[i] = Game.Instance.accumulators.Add("ElementsConsumed", storage);
		}
		HandleVector<int>.Handle invalidHandle = HandleVector<int>.InvalidHandle;
		PlantElementAbsorber initial_data;
		PlantElementAbsorber.LocalInfo localInfo;
		if (consumed_elements.Length == 1)
		{
			initial_data = new PlantElementAbsorber
			{
				storage = storage,
				consumedElements = null,
				accumulators = array
			};
			localInfo = new PlantElementAbsorber.LocalInfo
			{
				tag = consumed_elements[0].tag,
				massConsumptionRate = consumed_elements[0].massConsumptionRate
			};
			initial_data.localInfo = localInfo;
			return Allocate(initial_data);
		}
		initial_data = new PlantElementAbsorber
		{
			storage = storage,
			consumedElements = consumed_elements,
			accumulators = array
		};
		localInfo = (initial_data.localInfo = new PlantElementAbsorber.LocalInfo
		{
			tag = Tag.Invalid,
			massConsumptionRate = 0f
		});
		return Allocate(initial_data);
	}

	public HandleVector<int>.Handle Remove(HandleVector<int>.Handle h)
	{
		if (updating)
		{
			queuedRemoves.Add(h);
		}
		else
		{
			Free(h);
		}
		return HandleVector<int>.InvalidHandle;
	}

	public void Sim200ms(float dt)
	{
		int count = data.Count;
		updating = true;
		for (int i = 0; i < count; i++)
		{
			PlantElementAbsorber value = data[i];
			if (value.storage == null)
			{
				continue;
			}
			if (value.consumedElements == null)
			{
				float num = value.localInfo.massConsumptionRate * dt;
				PrimaryElement primaryElement = value.storage.FindFirstWithMass(value.localInfo.tag);
				if (primaryElement != null)
				{
					float num2 = Mathf.Min(num, primaryElement.Mass);
					primaryElement.Mass -= num2;
					num -= num2;
					Game.Instance.accumulators.Accumulate(value.accumulators[0], num2);
					value.storage.Trigger(-1697596308, primaryElement.gameObject);
				}
			}
			else
			{
				for (int j = 0; j < value.consumedElements.Length; j++)
				{
					float num3 = value.consumedElements[j].massConsumptionRate * dt;
					PrimaryElement primaryElement2 = value.storage.FindFirstWithMass(value.consumedElements[j].tag);
					while (primaryElement2 != null)
					{
						float num4 = Mathf.Min(num3, primaryElement2.Mass);
						primaryElement2.Mass -= num4;
						num3 -= num4;
						Game.Instance.accumulators.Accumulate(value.accumulators[j], num4);
						value.storage.Trigger(-1697596308, primaryElement2.gameObject);
						if (num3 <= 0f)
						{
							break;
						}
						primaryElement2 = value.storage.FindFirstWithMass(value.consumedElements[j].tag);
					}
				}
			}
			data[i] = value;
		}
		updating = false;
		for (int k = 0; k < queuedRemoves.Count; k++)
		{
			HandleVector<int>.Handle h = queuedRemoves[k];
			Remove(h);
		}
		queuedRemoves.Clear();
	}

	public override void Clear()
	{
		base.Clear();
		for (int i = 0; i < data.Count; i++)
		{
			data[i].Clear();
		}
		data.Clear();
		handles.Clear();
	}

	public PlantElementAbsorbers()
		: base(0)
	{
	}
}
