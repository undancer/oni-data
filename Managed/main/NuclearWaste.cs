using KSerialization;

public class NuclearWaste : GameStateMachine<NuclearWaste, NuclearWaste.Instance, IStateMachineTarget, NuclearWaste.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		[Serialize]
		public float timeAlive;

		private float percentageRemaining;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public string GetAnimToPlay()
		{
			percentageRemaining = 1f - base.smi.timeAlive / 600f;
			if (percentageRemaining <= 0.33f)
			{
				return "idle1";
			}
			if (percentageRemaining <= 0.66f)
			{
				return "idle2";
			}
			return "idle3";
		}
	}

	private const float lifetime = 600f;

	public State idle;

	public State decayed;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		idle.PlayAnim((Instance smi) => smi.GetAnimToPlay()).Update(delegate(Instance smi, float dt)
		{
			smi.timeAlive += dt;
			string animToPlay2 = smi.GetAnimToPlay();
			if (smi.GetComponent<KBatchedAnimController>().GetCurrentAnim().name != animToPlay2)
			{
				smi.Play(animToPlay2);
			}
			if (smi.timeAlive >= 600f)
			{
				smi.GoTo(decayed);
			}
		}, UpdateRate.SIM_4000ms).EventHandler(GameHashes.Absorb, delegate(Instance smi, object otherObject)
		{
			Pickupable obj = (Pickupable)otherObject;
			float timeAlive = obj.GetSMI<Instance>().timeAlive;
			float mass = obj.GetComponent<PrimaryElement>().Mass;
			float mass2 = smi.master.GetComponent<PrimaryElement>().Mass;
			float num = (smi.timeAlive = ((mass2 - mass) * smi.timeAlive + mass * timeAlive) / mass2);
			string animToPlay = smi.GetAnimToPlay();
			if (smi.GetComponent<KBatchedAnimController>().GetCurrentAnim().name != animToPlay)
			{
				smi.Play(animToPlay);
			}
			if (smi.timeAlive >= 600f)
			{
				smi.GoTo(decayed);
			}
		});
		decayed.Enter(delegate(Instance smi)
		{
			smi.GetComponent<Dumpable>().Dump();
			Util.KDestroyGameObject(smi.master.gameObject);
		});
	}
}
