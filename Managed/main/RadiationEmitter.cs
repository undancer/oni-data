using System;
using UnityEngine;

public class RadiationEmitter : SimComponent
{
	public enum RadiationEmitterType
	{
		Constant,
		Pulsing,
		PulsingAveraged,
		SimplePulse,
		RadialBeams,
		Attractor
	}

	public bool radiusProportionalToRads = false;

	[SerializeField]
	public short emitRadiusX = 4;

	[SerializeField]
	public short emitRadiusY = 4;

	[SerializeField]
	public float emitRads = 10f;

	[SerializeField]
	public float emitRate = 1f;

	[SerializeField]
	public float emitSpeed = 1f;

	[SerializeField]
	public float emitDirection = 0f;

	[SerializeField]
	public float emitAngle = 360f;

	[SerializeField]
	public RadiationEmitterType emitType = RadiationEmitterType.Constant;

	[SerializeField]
	public Vector3 emissionOffset = Vector3.zero;

	protected override void OnSpawn()
	{
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChange, "RadiationEmitter.OnSpawn");
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChange);
		base.OnCleanUp();
	}

	public void SetEmitting(bool emitting)
	{
		SetSimActive(emitting);
	}

	public int GetEmissionCell()
	{
		return Grid.PosToCell(base.transform.GetPosition() + emissionOffset);
	}

	public void Refresh()
	{
		int emissionCell = GetEmissionCell();
		if (radiusProportionalToRads)
		{
			SetRadiusProportionalToRads();
		}
		SimMessages.ModifyRadiationEmitter(simHandle, emissionCell, emitRadiusX, emitRadiusY, emitRads, emitRate, emitSpeed, emitDirection, emitAngle, emitType);
	}

	private void OnCellChange()
	{
		Refresh();
	}

	private void SetRadiusProportionalToRads()
	{
		emitRadiusX = (short)Mathf.Clamp(Mathf.RoundToInt(emitRads * 1f), 1, 128);
		emitRadiusY = (short)Mathf.Clamp(Mathf.RoundToInt(emitRads * 1f), 1, 128);
	}

	protected override void OnSimActivate()
	{
		int emissionCell = GetEmissionCell();
		if (radiusProportionalToRads)
		{
			SetRadiusProportionalToRads();
		}
		SimMessages.ModifyRadiationEmitter(simHandle, emissionCell, emitRadiusX, emitRadiusY, emitRads, emitRate, emitSpeed, emitDirection, emitAngle, emitType);
	}

	protected override void OnSimDeactivate()
	{
		int emissionCell = GetEmissionCell();
		SimMessages.ModifyRadiationEmitter(simHandle, emissionCell, 0, 0, 0f, 0f, 0f, 0f, 0f, emitType);
	}

	protected override void OnSimRegister(HandleVector<Game.ComplexCallbackInfo<int>>.Handle cb_handle)
	{
		Game.Instance.simComponentCallbackManager.GetItem(cb_handle);
		int emissionCell = GetEmissionCell();
		SimMessages.AddRadiationEmitter(cb_handle.index, emissionCell, emitRadiusX, emitRadiusY, emitRads, emitRate, emitSpeed, emitDirection, emitAngle, emitType);
	}

	protected override void OnSimUnregister()
	{
		StaticUnregister(simHandle);
	}

	private static void StaticUnregister(int sim_handle)
	{
		Debug.Assert(Sim.IsValidHandle(sim_handle));
		SimMessages.RemoveRadiationEmitter(-1, sim_handle);
	}

	private void OnDrawGizmosSelected()
	{
		int emissionCell = GetEmissionCell();
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(Grid.CellToPos(emissionCell) + Vector3.right / 2f + Vector3.up / 2f, 0.2f);
	}

	protected override Action<int> GetStaticUnregister()
	{
		return StaticUnregister;
	}
}
