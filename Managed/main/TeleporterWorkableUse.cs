public class TeleporterWorkableUse : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SetWorkTime(5f);
		resetProgressOnStop = true;
	}

	protected override void OnStartWork(Worker worker)
	{
		Teleporter component = GetComponent<Teleporter>();
		Teleporter teleporter = component.FindTeleportTarget();
		component.SetTeleportTarget(teleporter);
		TeleportalPad.StatesInstance sMI = teleporter.GetSMI<TeleportalPad.StatesInstance>();
		sMI.sm.targetTeleporter.Trigger(sMI);
	}

	protected override void OnStopWork(Worker worker)
	{
		TeleportalPad.StatesInstance sMI = this.GetSMI<TeleportalPad.StatesInstance>();
		sMI.sm.doTeleport.Trigger(sMI);
	}
}
