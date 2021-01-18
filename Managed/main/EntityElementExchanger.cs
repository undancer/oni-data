using UnityEngine;

public class EntityElementExchanger : StateMachineComponent<EntityElementExchanger.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, EntityElementExchanger, object>.GameInstance
	{
		public StatesInstance(EntityElementExchanger master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, EntityElementExchanger>
	{
		public State exchanging;

		public State paused;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = exchanging;
			base.serializable = true;
			exchanging.Enter(delegate(StatesInstance smi)
			{
				WiltCondition component = smi.master.gameObject.GetComponent<WiltCondition>();
				if (component != null && component.IsWilting())
				{
					smi.GoTo(smi.sm.paused);
				}
			}).EventTransition(GameHashes.Wilt, paused).ToggleStatusItem(Db.Get().CreatureStatusItems.ExchangingElementConsume)
				.ToggleStatusItem(Db.Get().CreatureStatusItems.ExchangingElementOutput)
				.Update("EntityElementExchanger", delegate(StatesInstance smi, float dt)
				{
					SimMessages.ConsumeMass(callbackIdx: Game.Instance.massConsumedCallbackManager.Add(OnSimConsumeCallback, smi.master, "EntityElementExchanger").index, gameCell: Grid.PosToCell(smi.master.gameObject), element: smi.master.consumedElement, mass: smi.master.consumeRate * dt, radius: 3);
				}, UpdateRate.SIM_1000ms);
			paused.EventTransition(GameHashes.WiltRecover, exchanging);
		}
	}

	public Vector3 outputOffset = Vector3.zero;

	public bool reportExchange;

	[MyCmpReq]
	private KSelectable selectable;

	public SimHashes consumedElement;

	public SimHashes emittedElement;

	public float consumeRate;

	public float exchangeRatio;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void SetConsumptionRate(float consumptionRate)
	{
		consumeRate = consumptionRate;
	}

	private static void OnSimConsumeCallback(Sim.MassConsumedCallback mass_cb_info, object data)
	{
		EntityElementExchanger entityElementExchanger = (EntityElementExchanger)data;
		if (entityElementExchanger != null)
		{
			entityElementExchanger.OnSimConsume(mass_cb_info);
		}
	}

	private void OnSimConsume(Sim.MassConsumedCallback mass_cb_info)
	{
		float num = mass_cb_info.mass * base.smi.master.exchangeRatio;
		if (reportExchange && base.smi.master.emittedElement == SimHashes.Oxygen)
		{
			string text = base.gameObject.GetProperName();
			ReceptacleMonitor component = GetComponent<ReceptacleMonitor>();
			if (component != null && component.GetReceptacle() != null)
			{
				text = text + " (" + component.GetReceptacle().gameObject.GetProperName() + ")";
			}
			ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, num, text);
		}
		SimMessages.EmitMass(Grid.PosToCell(base.smi.master.transform.GetPosition() + outputOffset), ElementLoader.FindElementByHash(base.smi.master.emittedElement).idx, num, ElementLoader.FindElementByHash(base.smi.master.emittedElement).defaultValues.temperature, byte.MaxValue, 0);
	}
}
