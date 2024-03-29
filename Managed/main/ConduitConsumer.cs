using System;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/ConduitConsumer")]
public class ConduitConsumer : KMonoBehaviour, IConduitConsumer
{
	public enum WrongElementResult
	{
		Destroy,
		Dump,
		Store
	}

	[SerializeField]
	public ConduitType conduitType;

	[SerializeField]
	public bool ignoreMinMassCheck;

	[SerializeField]
	public Tag capacityTag = GameTags.Any;

	[SerializeField]
	public float capacityKG = float.PositiveInfinity;

	[SerializeField]
	public bool forceAlwaysSatisfied;

	[SerializeField]
	public bool alwaysConsume;

	[SerializeField]
	public bool keepZeroMassObject = true;

	[SerializeField]
	public bool useSecondaryInput;

	[SerializeField]
	public bool isOn = true;

	[NonSerialized]
	public bool isConsuming = true;

	[NonSerialized]
	public bool consumedLastTick = true;

	[MyCmpReq]
	public Operational operational;

	[MyCmpReq]
	private Building building;

	public ISecondaryInput targetSecondaryInput;

	[MyCmpGet]
	public Storage storage;

	[MyCmpGet]
	private BuildingComplete m_buildingComplete;

	private int utilityCell = -1;

	public float consumptionRate = float.PositiveInfinity;

	public SimHashes lastConsumedElement = SimHashes.Vacuum;

	private HandleVector<int>.Handle partitionerEntry;

	private bool satisfied;

	public WrongElementResult wrongElementResult;

	public Storage Storage => storage;

	public ConduitType ConduitType => conduitType;

	public bool IsConnected
	{
		get
		{
			if (Grid.Objects[utilityCell, (conduitType == ConduitType.Gas) ? 12 : 16] != null)
			{
				return m_buildingComplete != null;
			}
			return false;
		}
	}

	public bool CanConsume
	{
		get
		{
			bool result = false;
			if (IsConnected)
			{
				result = GetConduitManager().GetContents(utilityCell).mass > 0f;
			}
			return result;
		}
	}

	public float stored_mass
	{
		get
		{
			if (!(storage == null))
			{
				if (!(capacityTag != GameTags.Any))
				{
					return storage.MassStored();
				}
				return storage.GetMassAvailable(capacityTag);
			}
			return 0f;
		}
	}

	public float space_remaining_kg
	{
		get
		{
			float num = capacityKG - stored_mass;
			if (!(storage == null))
			{
				return Mathf.Min(storage.RemainingCapacity(), num);
			}
			return num;
		}
	}

	public ConduitType TypeOfConduit => conduitType;

	public bool IsAlmostEmpty
	{
		get
		{
			if (!ignoreMinMassCheck)
			{
				return MassAvailable < ConsumptionRate * 30f;
			}
			return false;
		}
	}

	public bool IsEmpty
	{
		get
		{
			if (!ignoreMinMassCheck)
			{
				if (MassAvailable != 0f)
				{
					return MassAvailable < ConsumptionRate;
				}
				return true;
			}
			return false;
		}
	}

	public float ConsumptionRate => consumptionRate;

	public bool IsSatisfied
	{
		get
		{
			if (!satisfied)
			{
				return !isConsuming;
			}
			return true;
		}
		set
		{
			satisfied = value || forceAlwaysSatisfied;
		}
	}

	public float MassAvailable
	{
		get
		{
			ConduitFlow conduitManager = GetConduitManager();
			int inputCell = GetInputCell(conduitManager.conduitType);
			return conduitManager.GetContents(inputCell).mass;
		}
	}

	public void SetConduitData(ConduitType type)
	{
		conduitType = type;
	}

	private ConduitFlow GetConduitManager()
	{
		return conduitType switch
		{
			ConduitType.Gas => Game.Instance.gasConduitFlow, 
			ConduitType.Liquid => Game.Instance.liquidConduitFlow, 
			_ => null, 
		};
	}

	private int GetInputCell(ConduitType inputConduitType)
	{
		if (useSecondaryInput)
		{
			ISecondaryInput[] components = GetComponents<ISecondaryInput>();
			ISecondaryInput[] array = components;
			foreach (ISecondaryInput secondaryInput in array)
			{
				if (secondaryInput.HasSecondaryConduitType(inputConduitType))
				{
					return Grid.OffsetCell(building.NaturalBuildingCell(), secondaryInput.GetSecondaryConduitOffset(inputConduitType));
				}
			}
			Debug.LogWarning("No secondaryInput of type was found");
			return Grid.OffsetCell(building.NaturalBuildingCell(), components[0].GetSecondaryConduitOffset(inputConduitType));
		}
		return building.GetUtilityInputCell();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("PlumbingTutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Plumbing);
		});
		ConduitFlow conduitManager = GetConduitManager();
		utilityCell = GetInputCell(conduitManager.conduitType);
		ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[(conduitType == ConduitType.Gas) ? 12 : 16];
		partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, utilityCell, layer, OnConduitConnectionChanged);
		GetConduitManager().AddConduitUpdater(ConduitUpdate);
		OnConduitConnectionChanged(null);
	}

	protected override void OnCleanUp()
	{
		GetConduitManager().RemoveConduitUpdater(ConduitUpdate);
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	private void OnConduitConnectionChanged(object data)
	{
		Trigger(-2094018600, IsConnected);
	}

	public void SetOnState(bool onState)
	{
		isOn = onState;
	}

	private void ConduitUpdate(float dt)
	{
		if (isConsuming && isOn)
		{
			ConduitFlow conduitManager = GetConduitManager();
			Consume(dt, conduitManager);
		}
	}

	private void Consume(float dt, ConduitFlow conduit_mgr)
	{
		IsSatisfied = false;
		consumedLastTick = true;
		if (building.Def.CanMove)
		{
			utilityCell = GetInputCell(conduit_mgr.conduitType);
		}
		if (!IsConnected)
		{
			return;
		}
		ConduitFlow.ConduitContents contents = conduit_mgr.GetContents(utilityCell);
		if (contents.mass <= 0f)
		{
			return;
		}
		IsSatisfied = true;
		if (!alwaysConsume && !operational.IsOperational)
		{
			return;
		}
		float a = ConsumptionRate * dt;
		a = Mathf.Min(a, space_remaining_kg);
		Element element = ElementLoader.FindElementByHash(contents.element);
		if (contents.element != lastConsumedElement)
		{
			DiscoveredResources.Instance.Discover(element.tag, element.materialCategory);
		}
		float num = 0f;
		if (a > 0f)
		{
			ConduitFlow.ConduitContents conduitContents = conduit_mgr.RemoveElement(utilityCell, a);
			num = conduitContents.mass;
			lastConsumedElement = conduitContents.element;
		}
		bool flag = element.HasTag(capacityTag);
		if (num > 0f && capacityTag != GameTags.Any && !flag)
		{
			Trigger(-794517298, new BuildingHP.DamageSourceInfo
			{
				damage = 1,
				source = BUILDINGS.DAMAGESOURCES.BAD_INPUT_ELEMENT,
				popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.WRONG_ELEMENT
			});
		}
		if (flag || wrongElementResult == WrongElementResult.Store || contents.element == SimHashes.Vacuum || capacityTag == GameTags.Any)
		{
			if (!(num > 0f))
			{
				return;
			}
			consumedLastTick = false;
			int disease_count = (int)((float)contents.diseaseCount * (num / contents.mass));
			Element element2 = ElementLoader.FindElementByHash(contents.element);
			switch (conduitType)
			{
			case ConduitType.Liquid:
				if (element2.IsLiquid)
				{
					storage.AddLiquid(contents.element, num, contents.temperature, contents.diseaseIdx, disease_count, keepZeroMassObject, do_disease_transfer: false);
				}
				else
				{
					Debug.LogWarning("Liquid conduit consumer consuming non liquid: " + element2.id);
				}
				break;
			case ConduitType.Gas:
				if (element2.IsGas)
				{
					storage.AddGasChunk(contents.element, num, contents.temperature, contents.diseaseIdx, disease_count, keepZeroMassObject, do_disease_transfer: false);
				}
				else
				{
					Debug.LogWarning("Gas conduit consumer consuming non gas: " + element2.id);
				}
				break;
			}
		}
		else if (num > 0f)
		{
			consumedLastTick = false;
			if (wrongElementResult == WrongElementResult.Dump)
			{
				int disease_count2 = (int)((float)contents.diseaseCount * (num / contents.mass));
				SimMessages.AddRemoveSubstance(Grid.PosToCell(base.transform.GetPosition()), contents.element, CellEventLogger.Instance.ConduitConsumerWrongElement, num, contents.temperature, contents.diseaseIdx, disease_count2);
			}
		}
	}
}
