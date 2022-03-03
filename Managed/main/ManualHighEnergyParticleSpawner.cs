using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ManualHighEnergyParticleSpawner : StateMachineComponent<ManualHighEnergyParticleSpawner.StatesInstance>, IHighEnergyParticleDirection
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, ManualHighEnergyParticleSpawner, object>.GameInstance
	{
		public StatesInstance(ManualHighEnergyParticleSpawner smi)
			: base(smi)
		{
		}

		public bool IsComplexFabricatorWorkable(object data)
		{
			return data as ComplexFabricatorWorkable != null;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, ManualHighEnergyParticleSpawner>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State working;
		}

		public State inoperational;

		public ReadyStates ready;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = inoperational;
			inoperational.Enter(delegate(StatesInstance smi)
			{
				smi.master.radiationEmitter.SetEmitting(emitting: false);
			}).TagTransition(GameTags.Operational, ready);
			ready.DefaultState(ready.idle).TagTransition(GameTags.Operational, inoperational, on_remove: true).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.LauncherUpdate();
			});
			ready.idle.EventHandlerTransition(GameHashes.WorkableStartWork, ready.working, (StatesInstance smi, object data) => smi.IsComplexFabricatorWorkable(data));
			ready.working.Enter(delegate(StatesInstance smi)
			{
				smi.master.radiationEmitter.SetEmitting(emitting: true);
			}).EventHandlerTransition(GameHashes.WorkableCompleteWork, ready.idle, (StatesInstance smi, object data) => smi.IsComplexFabricatorWorkable(data)).EventHandlerTransition(GameHashes.WorkableStopWork, ready.idle, (StatesInstance smi, object data) => smi.IsComplexFabricatorWorkable(data))
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.radiationEmitter.SetEmitting(emitting: false);
				});
		}
	}

	[MyCmpReq]
	private HighEnergyParticleStorage particleStorage;

	[MyCmpGet]
	private RadiationEmitter radiationEmitter;

	[Serialize]
	private EightDirection _direction;

	private EightDirectionController directionController;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<ManualHighEnergyParticleSpawner> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ManualHighEnergyParticleSpawner>(delegate(ManualHighEnergyParticleSpawner component, object data)
	{
		component.OnCopySettings(data);
	});

	public EightDirection Direction
	{
		get
		{
			return _direction;
		}
		set
		{
			_direction = value;
			if (directionController != null)
			{
				directionController.SetRotation(45 * EightDirectionUtil.GetDirectionIndex(_direction));
				directionController.controller.enabled = false;
				directionController.controller.enabled = true;
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		radiationEmitter.SetEmitting(emitting: false);
		directionController = new EightDirectionController(GetComponent<KBatchedAnimController>(), "redirector_target", "redirect", EightDirectionController.Offset.Infront);
		Direction = Direction;
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Radiation);
	}

	private void OnCopySettings(object data)
	{
		ManualHighEnergyParticleSpawner component = ((GameObject)data).GetComponent<ManualHighEnergyParticleSpawner>();
		if (component != null)
		{
			Direction = component.Direction;
		}
	}

	public void LauncherUpdate()
	{
		if (particleStorage.Particles > 0f)
		{
			int highEnergyParticleOutputCell = GetComponent<Building>().GetHighEnergyParticleOutputCell();
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab("HighEnergyParticle"), Grid.CellToPosCCC(highEnergyParticleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
			gameObject.SetActive(value: true);
			if (gameObject != null)
			{
				HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
				component.payload = particleStorage.ConsumeAndGet(particleStorage.Particles);
				component.SetDirection(Direction);
				directionController.PlayAnim("redirect_send");
				directionController.controller.Queue("redirect");
			}
		}
	}
}
