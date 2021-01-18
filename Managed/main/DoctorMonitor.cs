public class DoctorMonitor : GameStateMachine<DoctorMonitor, DoctorMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		base.serializable = SerializeType.Both_DEPRECATED;
		root.ToggleUrge(Db.Get().Urges.Doctor);
	}
}
