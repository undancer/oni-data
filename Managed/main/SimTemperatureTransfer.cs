using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/SimTemperatureTransfer")]
public class SimTemperatureTransfer : KMonoBehaviour
{
	private const float SIM_FREEZE_SPAWN_ORE_PERCENT = 0.8f;

	private const float MIN_MASS_FOR_TEMPERATURE_TRANSFER = 0.01f;

	public float deltaKJ;

	public Action<SimTemperatureTransfer> onSimRegistered;

	protected int simHandle = -1;

	private float pendingEnergyModifications;

	[SerializeField]
	protected float surfaceArea = 10f;

	[SerializeField]
	protected float thickness = 0.01f;

	[SerializeField]
	protected float groundTransferScale = 0.0625f;

	private static Dictionary<int, SimTemperatureTransfer> handleInstanceMap = new Dictionary<int, SimTemperatureTransfer>();

	public float SurfaceArea
	{
		get
		{
			return surfaceArea;
		}
		set
		{
			surfaceArea = value;
		}
	}

	public float Thickness
	{
		get
		{
			return thickness;
		}
		set
		{
			thickness = value;
		}
	}

	public float GroundTransferScale
	{
		get
		{
			return GroundTransferScale;
		}
		set
		{
			groundTransferScale = value;
		}
	}

	public int SimHandle => simHandle;

	public static void ClearInstanceMap()
	{
		handleInstanceMap.Clear();
	}

	public static void DoOreMeltTransition(int sim_handle)
	{
		SimTemperatureTransfer value = null;
		if (!handleInstanceMap.TryGetValue(sim_handle, out value) || value == null || value.HasTag(GameTags.Sealed))
		{
			return;
		}
		PrimaryElement component = value.GetComponent<PrimaryElement>();
		Element element = component.Element;
		bool flag = component.Temperature >= element.highTemp;
		bool flag2 = component.Temperature <= element.lowTemp;
		DebugUtil.DevAssert(flag || flag2, "An ore got a melt message from the sim but it's still the correct temperature for its state!", component);
		if ((flag && element.highTempTransitionTarget == SimHashes.Unobtanium) || (flag2 && element.lowTempTransitionTarget == SimHashes.Unobtanium))
		{
			return;
		}
		if (component.Mass > 0f)
		{
			int gameCell = Grid.PosToCell(value.transform.GetPosition());
			float num = component.Mass;
			int num2 = component.DiseaseCount;
			SimHashes new_element = (flag ? element.highTempTransitionTarget : element.lowTempTransitionTarget);
			SimHashes simHashes = (flag ? element.highTempTransitionOreID : element.lowTempTransitionOreID);
			float num3 = (flag ? element.highTempTransitionOreMassConversion : element.lowTempTransitionOreMassConversion);
			if ((byte)simHashes != byte.MaxValue)
			{
				float num4 = num * num3;
				int num5 = (int)((float)num2 * num3);
				if (num4 > 0.001f)
				{
					num -= num4;
					num2 -= num5;
					Element element2 = ElementLoader.FindElementByHash(simHashes);
					if (element2.IsSolid)
					{
						GameObject gameObject = element2.substance.SpawnResource(value.transform.GetPosition(), num4, component.Temperature, component.DiseaseIdx, num5, prevent_merge: true, forceTemperature: false, manual_activation: true);
						element2.substance.ActivateSubstanceGameObject(gameObject, component.DiseaseIdx, num5);
					}
					else
					{
						SimMessages.AddRemoveSubstance(gameCell, element2.id, CellEventLogger.Instance.OreMelted, num4, component.Temperature, component.DiseaseIdx, num5);
					}
				}
			}
			SimMessages.AddRemoveSubstance(gameCell, new_element, CellEventLogger.Instance.OreMelted, num, component.Temperature, component.DiseaseIdx, num2);
		}
		value.OnCleanUp();
		Util.KDestroyGameObject(value.gameObject);
	}

	protected override void OnPrefabInit()
	{
		PrimaryElement component = GetComponent<PrimaryElement>();
		component.getTemperatureCallback = OnGetTemperature;
		component.setTemperatureCallback = OnSetTemperature;
		component.onDataChanged = (Action<PrimaryElement>)Delegate.Combine(component.onDataChanged, new Action<PrimaryElement>(OnDataChanged));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		PrimaryElement component = GetComponent<PrimaryElement>();
		Element element = component.Element;
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChanged, "SimTemperatureTransfer.OnSpawn");
		if (!Grid.IsValidCell(Grid.PosToCell(this)) || component.Element.HasTag(GameTags.Special) || element.specificHeatCapacity == 0f)
		{
			base.enabled = false;
		}
		SimRegister();
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		SimRegister();
		if (Sim.IsValidHandle(simHandle))
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			OnSetTemperature(component, component.Temperature);
		}
	}

	protected override void OnCmpDisable()
	{
		if (Sim.IsValidHandle(simHandle))
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			float temperature = component.Temperature;
			component.InternalTemperature = component.Temperature;
			SimMessages.SetElementChunkData(simHandle, temperature, 0f);
		}
		base.OnCmpDisable();
	}

	private void OnCellChanged()
	{
		int cell = Grid.PosToCell(this);
		if (!Grid.IsValidCell(cell))
		{
			base.enabled = false;
			return;
		}
		SimRegister();
		if (Sim.IsValidHandle(simHandle))
		{
			SimMessages.MoveElementChunk(simHandle, cell);
		}
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChanged);
		SimUnregister();
		base.OnForcedCleanUp();
	}

	public void ModifyEnergy(float delta_kilojoules)
	{
		if (Sim.IsValidHandle(simHandle))
		{
			SimMessages.ModifyElementChunkEnergy(simHandle, delta_kilojoules);
		}
		else
		{
			pendingEnergyModifications += delta_kilojoules;
		}
	}

	private unsafe static float OnGetTemperature(PrimaryElement primary_element)
	{
		SimTemperatureTransfer component = primary_element.GetComponent<SimTemperatureTransfer>();
		float result;
		if (Sim.IsValidHandle(component.simHandle))
		{
			int handleIndex = Sim.GetHandleIndex(component.simHandle);
			result = Game.Instance.simData.elementChunks[handleIndex].temperature;
			component.deltaKJ = Game.Instance.simData.elementChunks[handleIndex].deltaKJ;
		}
		else
		{
			result = primary_element.InternalTemperature;
		}
		return result;
	}

	private unsafe static void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		if (temperature <= 0f)
		{
			KCrashReporter.Assert(condition: false, "STT.OnSetTemperature - Tried to set <= 0 degree temperature");
			temperature = 293f;
		}
		SimTemperatureTransfer component = primary_element.GetComponent<SimTemperatureTransfer>();
		if (Sim.IsValidHandle(component.simHandle))
		{
			float mass = primary_element.Mass;
			float heat_capacity = ((mass >= 0.01f) ? (mass * primary_element.Element.specificHeatCapacity) : 0f);
			SimMessages.SetElementChunkData(component.simHandle, temperature, heat_capacity);
			int handleIndex = Sim.GetHandleIndex(component.simHandle);
			Game.Instance.simData.elementChunks[handleIndex].temperature = temperature;
		}
		else
		{
			primary_element.InternalTemperature = temperature;
		}
	}

	private void OnDataChanged(PrimaryElement primary_element)
	{
		if (Sim.IsValidHandle(simHandle))
		{
			float heat_capacity = ((primary_element.Mass >= 0.01f) ? (primary_element.Mass * primary_element.Element.specificHeatCapacity) : 0f);
			SimMessages.SetElementChunkData(simHandle, primary_element.Temperature, heat_capacity);
		}
	}

	protected void SimRegister()
	{
		if (!base.isSpawned || simHandle != -1 || !base.enabled)
		{
			return;
		}
		PrimaryElement component = GetComponent<PrimaryElement>();
		if (component.Mass > 0f && !component.Element.IsTemperatureInsulated)
		{
			int gameCell = Grid.PosToCell(base.transform.GetPosition());
			simHandle = -2;
			HandleVector<Game.ComplexCallbackInfo<int>>.Handle handle = Game.Instance.simComponentCallbackManager.Add(OnSimRegisteredCallback, this, "SimTemperatureTransfer.SimRegister");
			float num = component.InternalTemperature;
			if (num <= 0f)
			{
				component.InternalTemperature = 293f;
				num = 293f;
			}
			SimMessages.AddElementChunk(gameCell, component.ElementID, component.Mass, num, surfaceArea, thickness, groundTransferScale, handle.index);
		}
	}

	protected unsafe void SimUnregister()
	{
		if (simHandle != -1 && !KMonoBehaviour.isLoadingScene)
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			if (Sim.IsValidHandle(simHandle))
			{
				int handleIndex = Sim.GetHandleIndex(simHandle);
				component.InternalTemperature = Game.Instance.simData.elementChunks[handleIndex].temperature;
				SimMessages.RemoveElementChunk(simHandle, -1);
				handleInstanceMap.Remove(simHandle);
			}
			simHandle = -1;
		}
	}

	private static void OnSimRegisteredCallback(int handle, object data)
	{
		((SimTemperatureTransfer)data).OnSimRegistered(handle);
	}

	private unsafe void OnSimRegistered(int handle)
	{
		if (this != null && simHandle == -2)
		{
			simHandle = handle;
			int handleIndex = Sim.GetHandleIndex(handle);
			if (Game.Instance.simData.elementChunks[handleIndex].temperature <= 0f)
			{
				KCrashReporter.Assert(condition: false, "Bad temperature");
			}
			handleInstanceMap[simHandle] = this;
			if (pendingEnergyModifications > 0f)
			{
				ModifyEnergy(pendingEnergyModifications);
				pendingEnergyModifications = 0f;
			}
			if (onSimRegistered != null)
			{
				onSimRegistered(this);
			}
		}
		else
		{
			SimMessages.RemoveElementChunk(handle, -1);
		}
	}
}
