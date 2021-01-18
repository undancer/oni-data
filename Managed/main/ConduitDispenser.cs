using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/ConduitDispenser")]
public class ConduitDispenser : KMonoBehaviour, ISaveLoadable, IConduitDispenser
{
	[SerializeField]
	public ConduitType conduitType;

	[SerializeField]
	public SimHashes[] elementFilter = null;

	[SerializeField]
	public bool invertElementFilter = false;

	[SerializeField]
	public bool alwaysDispense = false;

	[SerializeField]
	public bool isOn = true;

	[SerializeField]
	public bool blocked = false;

	[SerializeField]
	public bool empty = true;

	[SerializeField]
	public bool useSecondaryOutput = false;

	private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	public Storage storage;

	private HandleVector<int>.Handle partitionerEntry;

	private int utilityCell = -1;

	private int elementOutputOffset = 0;

	public Storage Storage => storage;

	public ConduitType ConduitType => conduitType;

	public ConduitType TypeOfConduit => conduitType;

	public ConduitFlow.ConduitContents ConduitContents => GetConduitManager().GetContents(utilityCell);

	public bool IsConnected
	{
		get
		{
			GameObject gameObject = Grid.Objects[utilityCell, (conduitType == ConduitType.Gas) ? 12 : 16];
			return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
		}
	}

	public void SetConduitData(ConduitType type)
	{
		conduitType = type;
	}

	public ConduitFlow GetConduitManager()
	{
		return conduitType switch
		{
			ConduitType.Gas => Game.Instance.gasConduitFlow, 
			ConduitType.Liquid => Game.Instance.liquidConduitFlow, 
			_ => null, 
		};
	}

	private void OnConduitConnectionChanged(object data)
	{
		Trigger(-2094018600, IsConnected);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("PlumbingTutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Plumbing);
		});
		utilityCell = GetOutputCell();
		ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[(conduitType == ConduitType.Gas) ? 12 : 16];
		partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, utilityCell, layer, OnConduitConnectionChanged);
		GetConduitManager().AddConduitUpdater(ConduitUpdate, ConduitFlowPriority.Dispense);
		OnConduitConnectionChanged(null);
	}

	protected override void OnCleanUp()
	{
		GetConduitManager().RemoveConduitUpdater(ConduitUpdate);
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	public void SetOnState(bool onState)
	{
		isOn = onState;
	}

	private void ConduitUpdate(float dt)
	{
		operational.SetFlag(outputConduitFlag, IsConnected);
		blocked = false;
		if (isOn)
		{
			Dispense(dt);
		}
	}

	private void Dispense(float dt)
	{
		if (!operational.IsOperational && !alwaysDispense)
		{
			return;
		}
		PrimaryElement primaryElement = FindSuitableElement();
		if (primaryElement != null)
		{
			empty = false;
			ConduitFlow conduitManager = GetConduitManager();
			float num = conduitManager.AddElement(utilityCell, primaryElement.ElementID, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
			if (num > 0f)
			{
				float num2 = num / primaryElement.Mass;
				int num3 = (int)(num2 * (float)primaryElement.DiseaseCount);
				primaryElement.ModifyDiseaseCount(-num3, "ConduitDispenser.ConduitUpdate");
				primaryElement.Mass -= num;
				Trigger(-1697596308, primaryElement.gameObject);
			}
			else
			{
				blocked = true;
			}
		}
		else
		{
			empty = true;
		}
	}

	private PrimaryElement FindSuitableElement()
	{
		List<GameObject> items = storage.items;
		int count = items.Count;
		for (int i = 0; i < count; i++)
		{
			int index = (i + elementOutputOffset) % count;
			PrimaryElement component = items[index].GetComponent<PrimaryElement>();
			if (component != null && component.Mass > 0f && ((conduitType == ConduitType.Liquid) ? component.Element.IsLiquid : component.Element.IsGas) && (elementFilter == null || elementFilter.Length == 0 || (!invertElementFilter && IsFilteredElement(component.ElementID)) || (invertElementFilter && !IsFilteredElement(component.ElementID))))
			{
				elementOutputOffset = (elementOutputOffset + 1) % count;
				return component;
			}
		}
		return null;
	}

	private bool IsFilteredElement(SimHashes element)
	{
		for (int i = 0; i != elementFilter.Length; i++)
		{
			if (elementFilter[i] == element)
			{
				return true;
			}
		}
		return false;
	}

	private int GetOutputCell()
	{
		Building component = GetComponent<Building>();
		if (useSecondaryOutput)
		{
			ISecondaryOutput component2 = GetComponent<ISecondaryOutput>();
			return Grid.OffsetCell(component.NaturalBuildingCell(), component2.GetSecondaryConduitOffset(conduitType));
		}
		return component.GetUtilityOutputCell();
	}
}
