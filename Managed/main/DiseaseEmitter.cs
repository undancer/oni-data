using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DiseaseEmitter")]
public class DiseaseEmitter : KMonoBehaviour
{
	[Serialize]
	public float emitRate = 1f;

	[Serialize]
	public byte emitRange;

	[Serialize]
	public int emitCount;

	[Serialize]
	public byte[] emitDiseases;

	public int[] simHandles;

	[Serialize]
	private bool enableEmitter;

	public float EmitRate => emitRate;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (emitDiseases != null)
		{
			simHandles = new int[emitDiseases.Length];
			for (int i = 0; i < simHandles.Length; i++)
			{
				simHandles[i] = -1;
			}
		}
		SimRegister();
	}

	protected override void OnCleanUp()
	{
		SimUnregister();
		base.OnCleanUp();
	}

	public void SetEnable(bool enable)
	{
		if (enableEmitter != enable)
		{
			enableEmitter = enable;
			if (enableEmitter)
			{
				SimRegister();
			}
			else
			{
				SimUnregister();
			}
		}
	}

	private void OnCellChanged()
	{
		if (simHandles == null || !enableEmitter)
		{
			return;
		}
		int cell = Grid.PosToCell(this);
		if (!Grid.IsValidCell(cell))
		{
			return;
		}
		for (int i = 0; i < emitDiseases.Length; i++)
		{
			if (Sim.IsValidHandle(simHandles[i]))
			{
				SimMessages.ModifyDiseaseEmitter(simHandles[i], cell, emitRange, emitDiseases[i], emitRate, emitCount);
			}
		}
	}

	private void SimRegister()
	{
		if (simHandles == null || !enableEmitter)
		{
			return;
		}
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChanged, "DiseaseEmitter.Modify");
		for (int i = 0; i < simHandles.Length; i++)
		{
			if (simHandles[i] == -1)
			{
				simHandles[i] = -2;
				SimMessages.AddDiseaseEmitter(Game.Instance.simComponentCallbackManager.Add(OnSimRegisteredCallback, this, "DiseaseEmitter").index);
			}
		}
	}

	private void SimUnregister()
	{
		if (simHandles == null)
		{
			return;
		}
		for (int i = 0; i < simHandles.Length; i++)
		{
			if (Sim.IsValidHandle(simHandles[i]))
			{
				SimMessages.RemoveDiseaseEmitter(-1, simHandles[i]);
			}
			simHandles[i] = -1;
		}
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChanged);
	}

	private static void OnSimRegisteredCallback(int handle, object data)
	{
		((DiseaseEmitter)data).OnSimRegistered(handle);
	}

	private void OnSimRegistered(int handle)
	{
		bool flag = false;
		if (this != null)
		{
			for (int i = 0; i < simHandles.Length; i++)
			{
				if (simHandles[i] == -2)
				{
					simHandles[i] = handle;
					flag = true;
					break;
				}
			}
			OnCellChanged();
		}
		if (!flag)
		{
			SimMessages.RemoveDiseaseEmitter(-1, handle);
		}
	}

	public void SetDiseases(List<Disease> diseases)
	{
		emitDiseases = new byte[diseases.Count];
		for (int i = 0; i < diseases.Count; i++)
		{
			emitDiseases[i] = Db.Get().Diseases.GetIndex(diseases[i].id);
		}
	}
}
