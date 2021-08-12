public abstract class ConduitSensor : Switch
{
	public ConduitType conduitType;

	protected bool wasOn;

	protected KBatchedAnimController animController;

	protected static readonly HashedString[] ON_ANIMS = new HashedString[2] { "on_pre", "on" };

	protected static readonly HashedString[] OFF_ANIMS = new HashedString[2] { "on_pst", "off" };

	protected abstract void ConduitUpdate(float dt);

	protected override void OnSpawn()
	{
		base.OnSpawn();
		animController = GetComponent<KBatchedAnimController>();
		base.OnToggle += OnSwitchToggled;
		UpdateLogicCircuit();
		UpdateVisualState(force: true);
		wasOn = switchedOn;
		if (conduitType == ConduitType.Liquid || conduitType == ConduitType.Gas)
		{
			Conduit.GetFlowManager(conduitType).AddConduitUpdater(ConduitUpdate);
		}
		else
		{
			SolidConduit.GetFlowManager().AddConduitUpdater(ConduitUpdate);
		}
	}

	protected override void OnCleanUp()
	{
		if (conduitType == ConduitType.Liquid || conduitType == ConduitType.Gas)
		{
			Conduit.GetFlowManager(conduitType).RemoveConduitUpdater(ConduitUpdate);
		}
		else
		{
			SolidConduit.GetFlowManager().RemoveConduitUpdater(ConduitUpdate);
		}
		base.OnCleanUp();
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		UpdateLogicCircuit();
		UpdateVisualState();
	}

	private void UpdateLogicCircuit()
	{
		GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, switchedOn ? 1 : 0);
	}

	protected virtual void UpdateVisualState(bool force = false)
	{
		if (wasOn != switchedOn || force)
		{
			wasOn = switchedOn;
			if (switchedOn)
			{
				animController.Play(ON_ANIMS, KAnim.PlayMode.Loop);
			}
			else
			{
				animController.Play(OFF_ANIMS);
			}
		}
	}
}
