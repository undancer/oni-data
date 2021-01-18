public class TemporalTearAnalyzer : GameStateMachine<TemporalTearAnalyzer, TemporalTearAnalyzer.Instance, IStateMachineTarget, TemporalTearAnalyzer.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		[MyCmpAdd]
		private Studyable m_studyable;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void Init()
		{
			m_studyable.overrideAnims = new KAnimFile[1]
			{
				Assets.GetAnim("anim_interacts_temporal_tear_analyzer_kanim")
			};
		}
	}

	private State ready;

	private State studying;

	private State studying_finish;

	private State studying_cancel;

	private State inert;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = ready;
		root.Enter(delegate(Instance smi)
		{
			smi.Init();
		}).EventTransition(GameHashes.StudyComplete, studying_finish);
		ready.Enter(delegate(Instance smi)
		{
			Studyable component = smi.GetComponent<Studyable>();
			if (component.Studied)
			{
				smi.GoTo(inert);
			}
			else if (component.Studying)
			{
				smi.GoTo(studying);
			}
		}).WorkableStartTransition((Instance smi) => smi.GetComponent<Studyable>(), studying).PlayAnim("on");
		studying.EventTransition(GameHashes.StudyCancel, studying_cancel).WorkableStopTransition((Instance smi) => smi.GetComponent<Studyable>(), studying_cancel).PlayAnim("working_pre")
			.QueueAnim("working_loop", loop: true);
		studying_cancel.PlayAnim("working_pst").OnAnimQueueComplete(ready);
		studying_finish.PlayAnim("working_pst").OnAnimQueueComplete(inert);
		inert.PlayAnim("off").Enter(delegate
		{
			ClusterManager.Instance.GetClusterPOIManager().RevealTemporalTear();
		});
	}
}
