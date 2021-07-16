using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class PodLander : StateMachineComponent<PodLander.StatesInstance>, IGameObjectEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, PodLander, object>.GameInstance
	{
		public StatesInstance(PodLander master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, PodLander>
	{
		public State landing;

		public State crashed;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = landing;
			base.serializable = SerializeType.Both_DEPRECATED;
			landing.PlayAnim("launch_loop", KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
			{
				smi.master.flightAnimOffset = 50f;
			}).Update(delegate(StatesInstance smi, float dt)
			{
				float num = 10f;
				smi.master.rocketSpeed = num - Mathf.Clamp(Mathf.Pow(smi.timeinstate / 3.5f, 4f), 0f, num - 2f);
				smi.master.flightAnimOffset -= dt * smi.master.rocketSpeed;
				KBatchedAnimController component = smi.master.GetComponent<KBatchedAnimController>();
				component.Offset = Vector3.up * smi.master.flightAnimOffset;
				_ = component.PositionIncludingOffset;
				int num2 = Grid.PosToCell(smi.master.gameObject.transform.GetPosition() + smi.master.GetComponent<KBatchedAnimController>().Offset);
				if (Grid.IsValidCell(num2))
				{
					SimMessages.EmitMass(num2, (byte)ElementLoader.GetElementIndex(smi.master.exhaustElement), dt * smi.master.exhaustEmitRate, smi.master.exhaustTemperature, 0, 0);
				}
				if (component.Offset.y <= 0f)
				{
					smi.GoTo(crashed);
				}
			}, UpdateRate.SIM_33ms);
			crashed.PlayAnim("grounded").Enter(delegate(StatesInstance smi)
			{
				smi.master.GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
				smi.master.rocketSpeed = 0f;
				smi.master.ReleaseAstronaut();
			});
		}
	}

	[Serialize]
	private int landOffLocation;

	[Serialize]
	private float flightAnimOffset;

	private float rocketSpeed;

	public float exhaustEmitRate = 2f;

	public float exhaustTemperature = 1000f;

	public SimHashes exhaustElement = SimHashes.CarbonDioxide;

	private GameObject soundSpeakerObject;

	private bool releasingAstronaut;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void ReleaseAstronaut()
	{
		if (!releasingAstronaut)
		{
			releasingAstronaut = true;
			MinionStorage component = GetComponent<MinionStorage>();
			List<MinionStorage.Info> storedMinionInfo = component.GetStoredMinionInfo();
			for (int num = storedMinionInfo.Count - 1; num >= 0; num--)
			{
				component.DeserializeMinion(storedMinionInfo[num].id, Grid.CellToPos(Grid.PosToCell(base.smi.master.transform.GetPosition())));
			}
			releasingAstronaut = false;
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return null;
	}
}
