using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/CommandModuleWorkable")]
public class CommandModuleWorkable : Workable
{
	private static CellOffset[] entryOffsets = new CellOffset[5]
	{
		new CellOffset(0, 0),
		new CellOffset(0, 1),
		new CellOffset(0, 2),
		new CellOffset(0, 3),
		new CellOffset(0, 4)
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SetOffsets(entryOffsets);
		synchronizeAnims = false;
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_incubator_kanim")
		};
		SetWorkTime(float.PositiveInfinity);
		showProgressBar = false;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (worker != null)
		{
			if (DlcManager.IsExpansion1Active())
			{
				GameObject gameObject = worker.gameObject;
				CompleteWork(worker);
				GetComponent<ClustercraftExteriorDoor>().FerryMinion(gameObject);
				return true;
			}
			GameObject gameObject2 = worker.gameObject;
			CompleteWork(worker);
			GetComponent<MinionStorage>().SerializeMinion(gameObject2);
			return true;
		}
		return base.OnWorkTick(worker, dt);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
	}
}
