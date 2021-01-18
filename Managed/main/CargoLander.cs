using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CargoLander : GameStateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>
{
	public class Def : BaseDef
	{
		public Vector3 cargoDropOffset;

		public Tag previewTag;

		public bool deployOnLanding = true;
	}

	public class CrashedStates : State
	{
		public State loaded;

		public State emptying;

		public State empty;
	}

	public class StatesInstance : GameInstance, ISidescreenButtonControl
	{
		[Serialize]
		public float flightAnimOffset = 50f;

		public float exhaustEmitRate = 2f;

		public float exhaustTemperature = 1000f;

		public SimHashes exhaustElement = SimHashes.CarbonDioxide;

		public float topSpeed = 5f;

		private GameObject landingPreview;

		public string SidescreenButtonText => "_Cargo Lander";

		public string SidescreenButtonTooltip => "_Cargo Lander tooltip";

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void ResetAnimPosition()
		{
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			component.Offset = Vector3.up * flightAnimOffset;
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
			Vector3 pos = base.gameObject.transform.GetPosition() + new Vector3(0f, flightAnimOffset, 0f);
			int num = Grid.PosToCell(pos);
			if (Grid.IsValidCell(num))
			{
				SimMessages.EmitMass(num, (byte)ElementLoader.GetElementIndex(exhaustElement), dt * exhaustEmitRate, exhaustTemperature, 0, 0);
			}
		}

		public void DoLand()
		{
			KBatchedAnimController component = base.smi.master.GetComponent<KBatchedAnimController>();
			component.Offset = Vector3.zero;
			OccupyArea component2 = base.smi.GetComponent<OccupyArea>();
			if (component2 != null)
			{
				component2.ApplyToCells = true;
			}
			if (base.def.deployOnLanding && CheckIfLoaded())
			{
				base.sm.emptyCargo.Trigger(this);
			}
		}

		public void JettisonCargo()
		{
			Vector3 position = base.master.transform.GetPosition() + base.def.cargoDropOffset;
			MinionStorage component = GetComponent<MinionStorage>();
			if (component != null)
			{
				List<MinionStorage.Info> storedMinionInfo = component.GetStoredMinionInfo();
				for (int num = storedMinionInfo.Count - 1; num >= 0; num--)
				{
					GameObject gameObject = component.DeserializeMinion(storedMinionInfo[num].id, base.transform.GetPosition() + base.def.cargoDropOffset);
					gameObject.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
					ChoreProvider component2 = gameObject.GetComponent<ChoreProvider>();
					if (component2 != null)
					{
						new EmoteChore(component2, Db.Get().ChoreTypes.EmoteHighPriority, "anim_loco_new_kanim", new HashedString[1]
						{
							"fall_pst"
						}, KAnim.PlayMode.Once);
					}
				}
			}
			Storage component3 = GetComponent<Storage>();
			if (component3 != null)
			{
				component3.DropAll(position);
			}
			Trigger(-602000519, base.gameObject);
			CheckIfLoaded();
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

		public bool SidescreenEnabled()
		{
			return true;
		}

		public void OnSidescreenButtonPressed()
		{
			base.sm.emptyCargo.Trigger(this);
		}

		public bool SidescreenButtonInteractable()
		{
			return IsInsideState(base.sm.grounded.loaded);
		}

		public int ButtonSideScreenSortOrder()
		{
			return 20;
		}
	}

	public BoolParameter hasCargo;

	public Signal emptyCargo;

	public State stored;

	public State landing;

	public State land;

	public CrashedStates grounded;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = stored;
		root.InitializeOperationalFlag(RocketModule.landedFlag).Enter(delegate(StatesInstance smi)
		{
			smi.CheckIfLoaded();
		}).EventHandler(GameHashes.OnStorageChange, delegate(StatesInstance smi)
		{
			smi.CheckIfLoaded();
		});
		stored.TagTransition(GameTags.Stored, landing, on_remove: true).EventHandler(GameHashes.JettisonedLander, delegate(StatesInstance smi)
		{
			smi.OnJettisoned();
		});
		landing.PlayAnim("launch", KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
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
		});
		grounded.loaded.PlayAnim("loaded").ParamTransition(hasCargo, grounded.empty, GameStateMachine<CargoLander, StatesInstance, IStateMachineTarget, Def>.IsFalse).OnSignal(emptyCargo, grounded.emptying)
			.Enter(delegate(StatesInstance smi)
			{
				smi.DoLand();
			});
		grounded.emptying.PlayAnim("deploying").Enter(delegate(StatesInstance smi)
		{
			smi.JettisonCargo();
		}).OnAnimQueueComplete(grounded.empty);
		grounded.empty.PlayAnim("deployed").ParamTransition(hasCargo, grounded.loaded, GameStateMachine<CargoLander, StatesInstance, IStateMachineTarget, Def>.IsTrue);
	}
}
