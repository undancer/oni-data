using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CargoLander : GameStateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>
{
	public class Def : BaseDef
	{
		public Tag previewTag;

		public bool deployOnLanding = true;
	}

	public class CrashedStates : State
	{
		public State loaded;

		public State emptying;

		public State empty;
	}

	public class StatesInstance : GameInstance
	{
		[Serialize]
		public float flightAnimOffset = 50f;

		public float exhaustEmitRate = 2f;

		public float exhaustTemperature = 1000f;

		public SimHashes exhaustElement = SimHashes.CarbonDioxide;

		public float topSpeed = 5f;

		private GameObject landingPreview;

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void ResetAnimPosition()
		{
			GetComponent<KBatchedAnimController>().Offset = Vector3.up * flightAnimOffset;
		}

		public void OnJettisoned()
		{
			flightAnimOffset = 50f;
		}

		public void ShowLandingPreview(bool show)
		{
			if (show)
			{
				landingPreview = Util.KInstantiate(Assets.GetPrefab(base.def.previewTag), base.transform.GetPosition(), Quaternion.identity, base.gameObject);
				landingPreview.SetActive(value: true);
			}
			else
			{
				landingPreview.DeleteObject();
				landingPreview = null;
			}
		}

		public void LandingUpdate(float dt)
		{
			flightAnimOffset = Mathf.Max(flightAnimOffset - dt * topSpeed, 0f);
			ResetAnimPosition();
			int num = Grid.PosToCell(base.gameObject.transform.GetPosition() + new Vector3(0f, flightAnimOffset, 0f));
			if (Grid.IsValidCell(num))
			{
				SimMessages.EmitMass(num, (byte)ElementLoader.GetElementIndex(exhaustElement), dt * exhaustEmitRate, exhaustTemperature, 0, 0);
			}
		}

		public void DoLand()
		{
			base.smi.master.GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
			OccupyArea component = base.smi.GetComponent<OccupyArea>();
			if (component != null)
			{
				component.ApplyToCells = true;
			}
			if (base.def.deployOnLanding && CheckIfLoaded())
			{
				base.sm.emptyCargo.Trigger(this);
			}
			base.smi.master.gameObject.Trigger(1591811118, this);
		}

		public bool CheckIfLoaded()
		{
			bool flag = false;
			MinionStorage component = GetComponent<MinionStorage>();
			if (component != null)
			{
				flag |= component.GetStoredMinionInfo().Count > 0;
			}
			Storage component2 = GetComponent<Storage>();
			if (component2 != null && !component2.IsEmpty())
			{
				flag = true;
			}
			if (flag != base.sm.hasCargo.Get(this))
			{
				base.sm.hasCargo.Set(flag, this);
			}
			return flag;
		}
	}

	public BoolParameter hasCargo;

	public Signal emptyCargo;

	public State init;

	public State stored;

	public State landing;

	public State land;

	public CrashedStates grounded;

	public BoolParameter isLanded = new BoolParameter(default_value: false);

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = init;
		base.serializable = SerializeType.ParamsOnly;
		root.InitializeOperationalFlag(RocketModule.landedFlag).Enter(delegate(StatesInstance smi)
		{
			smi.CheckIfLoaded();
		}).EventHandler(GameHashes.OnStorageChange, delegate(StatesInstance smi)
		{
			smi.CheckIfLoaded();
		});
		init.ParamTransition(isLanded, grounded, GameStateMachine<CargoLander, StatesInstance, IStateMachineTarget, Def>.IsTrue).GoTo(stored);
		stored.TagTransition(GameTags.Stored, landing, on_remove: true).EventHandler(GameHashes.JettisonedLander, delegate(StatesInstance smi)
		{
			smi.OnJettisoned();
		});
		landing.PlayAnim("landing", KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
		{
			smi.ShowLandingPreview(show: true);
		}).Exit(delegate(StatesInstance smi)
		{
			smi.ShowLandingPreview(show: false);
		})
			.Enter(delegate(StatesInstance smi)
			{
				smi.ResetAnimPosition();
			})
			.Update(delegate(StatesInstance smi, float dt)
			{
				smi.LandingUpdate(dt);
			}, UpdateRate.SIM_EVERY_TICK)
			.Transition(land, (StatesInstance smi) => smi.flightAnimOffset <= 0f);
		land.PlayAnim("grounded_pre").OnAnimQueueComplete(grounded);
		grounded.DefaultState(grounded.loaded).ToggleOperationalFlag(RocketModule.landedFlag).Enter(delegate(StatesInstance smi)
		{
			smi.CheckIfLoaded();
		})
			.Enter(delegate(StatesInstance smi)
			{
				smi.sm.isLanded.Set(value: true, smi);
			});
		grounded.loaded.PlayAnim("grounded").ParamTransition(hasCargo, grounded.empty, GameStateMachine<CargoLander, StatesInstance, IStateMachineTarget, Def>.IsFalse).OnSignal(emptyCargo, grounded.emptying)
			.Enter(delegate(StatesInstance smi)
			{
				smi.DoLand();
			});
		grounded.emptying.PlayAnim("deploying").TriggerOnEnter(GameHashes.JettisonCargo).OnAnimQueueComplete(grounded.empty);
		grounded.empty.PlayAnim("deployed").ParamTransition(hasCargo, grounded.loaded, GameStateMachine<CargoLander, StatesInstance, IStateMachineTarget, Def>.IsTrue);
	}
}
