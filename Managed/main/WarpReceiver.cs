using System.Linq;
using KSerialization;

public class WarpReceiver : Workable
{
	public class WarpReceiverSM : GameStateMachine<WarpReceiverSM, WarpReceiverSM.Instance, WarpReceiver>
	{
		public new class Instance : GameInstance
		{
			public Instance(WarpReceiver master)
				: base(master)
			{
			}
		}

		public State idle;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.PlayAnim("idle");
		}
	}

	[MyCmpAdd]
	public Notifier notifier;

	private WarpReceiverSM.Instance warpReceiverSMI;

	private Notification notification;

	[Serialize]
	public bool IsConsumed;

	private Chore chore;

	[Serialize]
	public bool Used;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		warpReceiverSMI = new WarpReceiverSM.Instance(this);
		warpReceiverSMI.StartSM();
		Components.WarpReceivers.Add(this);
	}

	public void ReceiveWarpedDuplicant(Worker dupe)
	{
		dupe.transform.SetPosition(Grid.CellToPos(Grid.PosToCell(this), CellAlignment.Bottom, Grid.SceneLayer.Move));
		Debug.Assert(chore == null);
		KAnimFile anim = Assets.GetAnim("anim_interacts_warp_portal_receiver_kanim");
		ChoreType migrate = Db.Get().ChoreTypes.Migrate;
		KAnimFile override_anims = anim;
		chore = new WorkChore<Workable>(migrate, this, dupe.GetComponent<ChoreProvider>(), run_until_complete: true, delegate
		{
			CompleteChore();
		}, null, null, allow_in_red_alert: true, null, ignore_schedule_block: true, only_when_operational: true, override_anims, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.compulsory);
		Workable component = GetComponent<Workable>();
		component.workLayer = Grid.SceneLayer.Building;
		component.workAnims = new HashedString[2] { "printing_pre", "printing_loop" };
		component.workingPstComplete = new HashedString[1] { "printing_pst" };
		component.workingPstFailed = new HashedString[1] { "printing_pst" };
		component.synchronizeAnims = true;
		float num = 0f;
		KAnimFileData data = anim.GetData();
		for (int i = 0; i < data.animCount; i++)
		{
			KAnim.Anim anim2 = data.GetAnim(i);
			if (component.workAnims.Contains(anim2.hash))
			{
				num += anim2.totalTime;
			}
		}
		component.SetWorkTime(num);
		Used = true;
	}

	private void CompleteChore()
	{
		chore.Cleanup();
		chore = null;
		warpReceiverSMI.GoTo(warpReceiverSMI.sm.idle);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.WarpReceivers.Remove(this);
	}
}
