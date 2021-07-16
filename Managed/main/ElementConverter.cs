using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ElementConverter : StateMachineComponent<ElementConverter.StatesInstance>, IGameObjectEffectDescriptor
{
	[Serializable]
	[DebuggerDisplay("{tag} {massConsumptionRate}")]
	public struct ConsumedElement
	{
		public Tag tag;

		public float massConsumptionRate;

		public HandleVector<int>.Handle accumulator;

		public string Name => tag.ProperName();

		public float Rate => Game.Instance.accumulators.GetAverageRate(accumulator);

		public ConsumedElement(Tag tag, float kgPerSecond)
		{
			this.tag = tag;
			massConsumptionRate = kgPerSecond;
			accumulator = HandleVector<int>.InvalidHandle;
		}
	}

	[Serializable]
	public struct OutputElement
	{
		public SimHashes elementHash;

		public float minOutputTemperature;

		public bool useEntityTemperature;

		public float massGenerationRate;

		public bool storeOutput;

		public Vector2 outputElementOffset;

		public HandleVector<int>.Handle accumulator;

		public float diseaseWeight;

		public byte addedDiseaseIdx;

		public int addedDiseaseCount;

		public string Name => ElementLoader.FindElementByHash(elementHash).tag.ProperName();

		public float Rate => Game.Instance.accumulators.GetAverageRate(accumulator);

		public OutputElement(float kgPerSecond, SimHashes element, float minOutputTemperature, bool useEntityTemperature = false, bool storeOutput = false, float outputElementOffsetx = 0f, float outputElementOffsety = 0.5f, float diseaseWeight = 1f, byte addedDiseaseIdx = byte.MaxValue, int addedDiseaseCount = 0)
		{
			elementHash = element;
			this.minOutputTemperature = minOutputTemperature;
			this.useEntityTemperature = useEntityTemperature;
			this.storeOutput = storeOutput;
			massGenerationRate = kgPerSecond;
			outputElementOffset = new Vector2(outputElementOffsetx, outputElementOffsety);
			accumulator = HandleVector<int>.InvalidHandle;
			this.diseaseWeight = diseaseWeight;
			this.addedDiseaseIdx = addedDiseaseIdx;
			this.addedDiseaseCount = addedDiseaseCount;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, ElementConverter, object>.GameInstance
	{
		private List<Guid> statusItemEntries = new List<Guid>();

		public StatesInstance(ElementConverter smi)
			: base(smi)
		{
		}

		public void AddStatusItems()
		{
			ConsumedElement[] consumedElements = base.master.consumedElements;
			foreach (ConsumedElement consumedElement in consumedElements)
			{
				Guid item = base.master.GetComponent<KSelectable>().AddStatusItem(ElementConverterInput, consumedElement);
				statusItemEntries.Add(item);
			}
			OutputElement[] outputElements = base.master.outputElements;
			foreach (OutputElement outputElement in outputElements)
			{
				Guid item2 = base.master.GetComponent<KSelectable>().AddStatusItem(ElementConverterOutput, outputElement);
				statusItemEntries.Add(item2);
			}
		}

		public void RemoveStatusItems()
		{
			foreach (Guid statusItemEntry in statusItemEntries)
			{
				base.master.GetComponent<KSelectable>().RemoveStatusItem(statusItemEntry);
			}
			statusItemEntries.Clear();
		}
	}

	public class States : GameStateMachine<States, StatesInstance, ElementConverter>
	{
		public State disabled;

		public State converting;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = disabled;
			disabled.EventTransition(GameHashes.ActiveChanged, converting, (StatesInstance smi) => smi.master.operational == null || smi.master.operational.IsActive);
			converting.Enter("AddStatusItems", delegate(StatesInstance smi)
			{
				smi.AddStatusItems();
			}).Exit("RemoveStatusItems", delegate(StatesInstance smi)
			{
				smi.RemoveStatusItems();
			}).EventTransition(GameHashes.ActiveChanged, disabled, (StatesInstance smi) => smi.master.operational != null && !smi.master.operational.IsActive)
				.Update("ConvertMass", delegate(StatesInstance smi, float dt)
				{
					smi.master.ConvertMass();
				}, UpdateRate.SIM_1000ms, load_balance: true);
		}
	}

	[MyCmpGet]
	private Operational operational;

	[MyCmpReq]
	private Storage storage;

	public Action<float> onConvertMass;

	private float totalDiseaseWeight = float.MaxValue;

	private AttributeInstance machinerySpeedAttribute;

	private float workSpeedMultiplier = 1f;

	public bool showDescriptors = true;

	private const float BASE_INTERVAL = 1f;

	[SerializeField]
	public ConsumedElement[] consumedElements;

	[SerializeField]
	public OutputElement[] outputElements;

	private float outputMultiplier = 1f;

	private static StatusItem ElementConverterInput;

	private static StatusItem ElementConverterOutput;

	public float OutputMultiplier
	{
		get
		{
			return outputMultiplier;
		}
		set
		{
			outputMultiplier = value;
		}
	}

	public float AverageConvertRate => Game.Instance.accumulators.GetAverageRate(outputElements[0].accumulator);

	public void SetWorkSpeedMultiplier(float speed)
	{
		workSpeedMultiplier = speed;
	}

	public void SetStorage(Storage storage)
	{
		this.storage = storage;
	}

	public bool HasEnoughMass(Tag tag)
	{
		bool result = false;
		List<GameObject> items = storage.items;
		ConsumedElement[] array = consumedElements;
		for (int i = 0; i < array.Length; i++)
		{
			ConsumedElement consumedElement = array[i];
			if (!(tag == consumedElement.tag))
			{
				continue;
			}
			float num = 0f;
			for (int j = 0; j < items.Count; j++)
			{
				GameObject gameObject = items[j];
				if (!(gameObject == null) && gameObject.HasTag(tag))
				{
					num += gameObject.GetComponent<PrimaryElement>().Mass;
				}
			}
			result = num >= consumedElement.massConsumptionRate;
			break;
		}
		return result;
	}

	public bool HasEnoughMassToStartConverting()
	{
		return HasEnoughMass();
	}

	public bool CanConvertAtAll()
	{
		bool result = true;
		List<GameObject> items = storage.items;
		for (int i = 0; i < consumedElements.Length; i++)
		{
			ConsumedElement consumedElement = consumedElements[i];
			bool flag = false;
			for (int j = 0; j < items.Count; j++)
			{
				GameObject gameObject = items[j];
				if (!(gameObject == null) && gameObject.HasTag(consumedElement.tag) && gameObject.GetComponent<PrimaryElement>().Mass > 0f)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	private float GetSpeedMultiplier()
	{
		return machinerySpeedAttribute.GetTotalValue() * workSpeedMultiplier;
	}

	private bool HasEnoughMass()
	{
		float speedMultiplier = GetSpeedMultiplier();
		float num = 1f * speedMultiplier;
		bool result = true;
		List<GameObject> items = storage.items;
		for (int i = 0; i < consumedElements.Length; i++)
		{
			ConsumedElement consumedElement = consumedElements[i];
			float num2 = 0f;
			for (int j = 0; j < items.Count; j++)
			{
				GameObject gameObject = items[j];
				if (!(gameObject == null) && gameObject.HasTag(consumedElement.tag))
				{
					num2 += gameObject.GetComponent<PrimaryElement>().Mass;
				}
			}
			if (num2 < consumedElement.massConsumptionRate * num)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	private void ConvertMass()
	{
		float speedMultiplier = GetSpeedMultiplier();
		float num = 1f * speedMultiplier;
		float num2 = 1f;
		for (int i = 0; i < consumedElements.Length; i++)
		{
			ConsumedElement consumedElement = consumedElements[i];
			float num3 = consumedElement.massConsumptionRate * num * num2;
			if (num3 <= 0f)
			{
				num2 = 0f;
				break;
			}
			float num4 = 0f;
			for (int j = 0; j < storage.items.Count; j++)
			{
				GameObject gameObject = storage.items[j];
				if (!(gameObject == null) && gameObject.HasTag(consumedElement.tag))
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					float num5 = Mathf.Min(num3, component.Mass);
					num4 += num5 / num3;
				}
			}
			num2 = Mathf.Min(num2, num4);
		}
		if (num2 <= 0f)
		{
			return;
		}
		SimUtil.DiseaseInfo diseaseInfo = SimUtil.DiseaseInfo.Invalid;
		diseaseInfo.idx = byte.MaxValue;
		diseaseInfo.count = 0;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		for (int k = 0; k < consumedElements.Length; k++)
		{
			ConsumedElement consumedElement2 = consumedElements[k];
			float num9 = consumedElement2.massConsumptionRate * num * num2;
			Game.Instance.accumulators.Accumulate(consumedElement2.accumulator, num9);
			for (int l = 0; l < storage.items.Count; l++)
			{
				GameObject gameObject2 = storage.items[l];
				if (gameObject2 == null)
				{
					continue;
				}
				if (gameObject2.HasTag(consumedElement2.tag))
				{
					PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
					component2.KeepZeroMassObject = true;
					float num10 = Mathf.Min(num9, component2.Mass);
					int num11 = (int)(num10 / component2.Mass * (float)component2.DiseaseCount);
					float num12 = num10 * component2.Element.specificHeatCapacity;
					num8 += num12;
					num7 += num12 * component2.Temperature;
					component2.Mass -= num10;
					component2.ModifyDiseaseCount(-num11, "ElementConverter.ConvertMass");
					num6 += num10;
					diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(diseaseInfo.idx, diseaseInfo.count, component2.DiseaseIdx, num11);
					num9 -= num10;
					if (num9 <= 0f)
					{
						break;
					}
				}
				if (num9 <= 0f)
				{
					Debug.Assert(num9 <= 0f);
				}
			}
		}
		float num13 = ((num8 > 0f) ? (num7 / num8) : 0f);
		if (onConvertMass != null && num6 > 0f)
		{
			onConvertMass(num6);
		}
		if (outputElements != null && outputElements.Length != 0)
		{
			for (int m = 0; m < outputElements.Length; m++)
			{
				OutputElement outputElement = outputElements[m];
				SimUtil.DiseaseInfo a = diseaseInfo;
				if (totalDiseaseWeight <= 0f)
				{
					a.idx = byte.MaxValue;
					a.count = 0;
				}
				else
				{
					float num14 = outputElement.diseaseWeight / totalDiseaseWeight;
					a.count = (int)((float)a.count * num14);
				}
				if (outputElement.addedDiseaseIdx != byte.MaxValue)
				{
					a = SimUtil.CalculateFinalDiseaseInfo(a, new SimUtil.DiseaseInfo
					{
						idx = outputElement.addedDiseaseIdx,
						count = outputElement.addedDiseaseCount
					});
				}
				float num15 = outputElement.massGenerationRate * OutputMultiplier * num * num2;
				Game.Instance.accumulators.Accumulate(outputElement.accumulator, num15);
				float num16 = 0f;
				num16 = ((!outputElement.useEntityTemperature && (num13 != 0f || outputElement.minOutputTemperature != 0f)) ? Mathf.Max(outputElement.minOutputTemperature, num13) : GetComponent<PrimaryElement>().Temperature);
				Element element = ElementLoader.FindElementByHash(outputElement.elementHash);
				if (outputElement.storeOutput)
				{
					PrimaryElement primaryElement = storage.AddToPrimaryElement(outputElement.elementHash, num15, num16);
					if (primaryElement == null)
					{
						if (element.IsGas)
						{
							storage.AddGasChunk(outputElement.elementHash, num15, num16, a.idx, a.count, keep_zero_mass: true);
						}
						else if (element.IsLiquid)
						{
							storage.AddLiquid(outputElement.elementHash, num15, num16, a.idx, a.count, keep_zero_mass: true);
						}
						else
						{
							GameObject go = element.substance.SpawnResource(base.transform.GetPosition(), num15, num16, a.idx, a.count, prevent_merge: true);
							storage.Store(go, hide_popups: true);
						}
					}
					else
					{
						primaryElement.AddDisease(a.idx, a.count, "ElementConverter.ConvertMass");
					}
				}
				else
				{
					Vector3 vector = new Vector3(base.transform.GetPosition().x + outputElement.outputElementOffset.x, base.transform.GetPosition().y + outputElement.outputElementOffset.y, 0f);
					int num17 = Grid.PosToCell(vector);
					if (element.IsLiquid)
					{
						int idx = element.idx;
						FallingWater.instance.AddParticle(num17, (byte)idx, num15, num16, a.idx, a.count, skip_sound: true);
					}
					else if (element.IsSolid)
					{
						element.substance.SpawnResource(vector, num15, num16, a.idx, a.count);
					}
					else
					{
						SimMessages.AddRemoveSubstance(num17, outputElement.elementHash, CellEventLogger.Instance.OxygenModifierSimUpdate, num15, num16, a.idx, a.count);
					}
				}
				if (outputElement.elementHash == SimHashes.Oxygen || outputElement.elementHash == SimHashes.ContaminatedOxygen)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, num15, base.gameObject.GetProperName());
				}
			}
		}
		storage.Trigger(-1697596308, base.gameObject);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Attributes attributes = base.gameObject.GetAttributes();
		machinerySpeedAttribute = attributes.Add(Db.Get().Attributes.MachinerySpeed);
		if (ElementConverterInput == null)
		{
			ElementConverterInput = new StatusItem("ElementConverterInput", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: true, OverlayModes.None.ID).SetResolveStringCallback(delegate(string str, object data)
			{
				ConsumedElement consumedElement = (ConsumedElement)data;
				str = str.Replace("{ElementTypes}", consumedElement.Name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedByTag(consumedElement.tag, consumedElement.Rate, GameUtil.TimeSlice.PerSecond));
				return str;
			});
		}
		if (ElementConverterOutput == null)
		{
			ElementConverterOutput = new StatusItem("ElementConverterOutput", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: true, OverlayModes.None.ID).SetResolveStringCallback(delegate(string str, object data)
			{
				OutputElement outputElement = (OutputElement)data;
				str = str.Replace("{ElementTypes}", outputElement.Name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(outputElement.Rate, GameUtil.TimeSlice.PerSecond));
				return str;
			});
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		for (int i = 0; i < consumedElements.Length; i++)
		{
			consumedElements[i].accumulator = Game.Instance.accumulators.Add("ElementsConsumed", this);
		}
		totalDiseaseWeight = 0f;
		for (int j = 0; j < outputElements.Length; j++)
		{
			outputElements[j].accumulator = Game.Instance.accumulators.Add("OutputElements", this);
			totalDiseaseWeight += outputElements[j].diseaseWeight;
		}
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		for (int i = 0; i < consumedElements.Length; i++)
		{
			Game.Instance.accumulators.Remove(consumedElements[i].accumulator);
		}
		for (int j = 0; j < outputElements.Length; j++)
		{
			Game.Instance.accumulators.Remove(outputElements[j].accumulator);
		}
		base.OnCleanUp();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (!showDescriptors)
		{
			return list;
		}
		if (consumedElements != null)
		{
			ConsumedElement[] array = consumedElements;
			for (int i = 0; i < array.Length; i++)
			{
				ConsumedElement consumedElement = array[i];
				Descriptor item = default(Descriptor);
				item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMED, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.massConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.massConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
				list.Add(item);
			}
		}
		if (outputElements != null)
		{
			OutputElement[] array2 = outputElements;
			for (int i = 0; i < array2.Length; i++)
			{
				OutputElement outputElement = array2[i];
				Descriptor item2 = default(Descriptor);
				LocString loc_string;
				LocString loc_string2;
				if (outputElement.useEntityTemperature)
				{
					loc_string = UI.BUILDINGEFFECTS.ELEMENTEMITTED_ENTITYTEMP;
					loc_string2 = UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_ENTITYTEMP;
				}
				else if (outputElement.minOutputTemperature > 0f)
				{
					loc_string = UI.BUILDINGEFFECTS.ELEMENTEMITTED_MINTEMP;
					loc_string2 = UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_MINTEMP;
				}
				else
				{
					loc_string = UI.BUILDINGEFFECTS.ELEMENTEMITTED_INPUTTEMP;
					loc_string2 = UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_INPUTTEMP;
				}
				item2.SetupDescriptor(string.Format(loc_string, outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"), GameUtil.GetFormattedTemperature(outputElement.minOutputTemperature)), string.Format(loc_string2, outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"), GameUtil.GetFormattedTemperature(outputElement.minOutputTemperature)));
				list.Add(item2);
			}
		}
		return list;
	}
}
