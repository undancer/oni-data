using UnityEngine;

public class CargoDropperStorage : GameStateMachine<CargoDropperStorage, CargoDropperStorage.StatesInstance, IStateMachineTarget, CargoDropperStorage.Def>
{
	public class Def : BaseDef
	{
		public Vector3 dropOffset;
	}

	public class StatesInstance : GameInstance
	{
		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void JettisonCargo(object data)
		{
			Vector3 position = base.master.transform.GetPosition() + base.def.dropOffset;
			Storage component = GetComponent<Storage>();
			if (!(component != null))
			{
				return;
			}
			GameObject gameObject = component.FindFirst("ScoutRover");
			if (gameObject != null)
			{
				component.Drop(gameObject);
				Vector3 position2 = base.master.transform.GetPosition();
				position2.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
				gameObject.transform.SetPosition(position2);
				ChoreProvider component2 = gameObject.GetComponent<ChoreProvider>();
				if (component2 != null)
				{
					KBatchedAnimController component3 = gameObject.GetComponent<KBatchedAnimController>();
					if (component3 != null)
					{
						component3.Play("enter");
					}
					new EmoteChore(component2, Db.Get().ChoreTypes.EmoteHighPriority, null, new HashedString[1] { "enter" }, KAnim.PlayMode.Once);
				}
			}
			component.DropAll(position);
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.EventHandler(GameHashes.JettisonCargo, delegate(StatesInstance smi, object data)
		{
			smi.JettisonCargo(data);
		});
	}
}
