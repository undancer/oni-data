using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BuildingElementEmitter")]
public class BuildingElementEmitter : KMonoBehaviour, IGameObjectEffectDescriptor, IElementEmitter, ISim200ms
{
	[SerializeField]
	public float emitRate = 0.3f;

	[SerializeField]
	[Serialize]
	public float temperature = 293f;

	[SerializeField]
	[HashedEnum]
	public SimHashes element = SimHashes.Oxygen;

	[SerializeField]
	public Vector2 modifierOffset;

	[SerializeField]
	public byte emitRange = 1;

	[SerializeField]
	public byte emitDiseaseIdx = byte.MaxValue;

	[SerializeField]
	public int emitDiseaseCount = 0;

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

	private int simHandle = -1;

	private bool simActive = false;

	private bool dirty = true;

	private Guid statusHandle;

	private static readonly EventSystem.IntraObjectHandler<BuildingElementEmitter> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<BuildingElementEmitter>(delegate(BuildingElementEmitter component, object data)
	{
		component.OnActiveChanged(data);
	});

	public float AverageEmitRate => Game.Instance.accumulators.GetAverageRate(accumulator);

	public float EmitRate => emitRate;

	public SimHashes Element => element;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		accumulator = Game.Instance.accumulators.Add("Element", this);
		Subscribe(824508782, OnActiveChangedDelegate);
		SimRegister();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.accumulators.Remove(accumulator);
		SimUnregister();
		base.OnCleanUp();
	}

	private void OnActiveChanged(object data)
	{
		simActive = ((Operational)data).IsActive;
		dirty = true;
	}

	public void Sim200ms(float dt)
	{
		UnsafeUpdate(dt);
	}

	private unsafe void UnsafeUpdate(float dt)
	{
		if (!Sim.IsValidHandle(simHandle))
		{
			return;
		}
		UpdateSimState();
		int handleIndex = Sim.GetHandleIndex(simHandle);
		Sim.EmittedMassInfo emittedMassInfo = Game.Instance.simData.emittedMassEntries[handleIndex];
		if (emittedMassInfo.mass > 0f)
		{
			Game.Instance.accumulators.Accumulate(accumulator, emittedMassInfo.mass);
			if (element == SimHashes.Oxygen)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, emittedMassInfo.mass, base.gameObject.GetProperName());
			}
		}
	}

	private void UpdateSimState()
	{
		if (!dirty)
		{
			return;
		}
		dirty = false;
		if (simActive)
		{
			if (element != 0 && emitRate > 0f)
			{
				Vector3 pos = new Vector3(base.transform.GetPosition().x + modifierOffset.x, base.transform.GetPosition().y + modifierOffset.y, 0f);
				int game_cell = Grid.PosToCell(pos);
				SimMessages.ModifyElementEmitter(simHandle, game_cell, emitRange, element, 0.2f, emitRate * 0.2f, temperature, float.MaxValue, emitDiseaseIdx, emitDiseaseCount);
			}
			statusHandle = GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.EmittingElement, this);
		}
		else
		{
			SimMessages.ModifyElementEmitter(simHandle, 0, 0, SimHashes.Vacuum, 0f, 0f, 0f, 0f, byte.MaxValue, 0);
			statusHandle = GetComponent<KSelectable>().RemoveStatusItem(statusHandle, this);
		}
	}

	private void SimRegister()
	{
		if (base.isSpawned && simHandle == -1)
		{
			simHandle = -2;
			SimMessages.AddElementEmitter(float.MaxValue, Game.Instance.simComponentCallbackManager.Add(OnSimRegisteredCallback, this, "BuildingElementEmitter").index);
		}
	}

	private void SimUnregister()
	{
		if (simHandle != -1)
		{
			if (Sim.IsValidHandle(simHandle))
			{
				SimMessages.RemoveElementEmitter(-1, simHandle);
			}
			simHandle = -1;
		}
	}

	private static void OnSimRegisteredCallback(int handle, object data)
	{
		((BuildingElementEmitter)data).OnSimRegistered(handle);
	}

	private void OnSimRegistered(int handle)
	{
		if (this != null)
		{
			simHandle = handle;
		}
		else
		{
			SimMessages.RemoveElementEmitter(-1, handle);
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(this.element);
		string arg = element.tag.ProperName();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED_FIXEDTEMP, arg, GameUtil.GetFormattedMass(EmitRate, GameUtil.TimeSlice.PerSecond), GameUtil.GetFormattedTemperature(temperature)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_FIXEDTEMP, arg, GameUtil.GetFormattedMass(EmitRate, GameUtil.TimeSlice.PerSecond), GameUtil.GetFormattedTemperature(temperature)));
		list.Add(item);
		return list;
	}
}
