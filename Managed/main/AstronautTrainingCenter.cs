using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/AstronautTrainingCenter")]
public class AstronautTrainingCenter : Workable
{
	public float daysToMasterRole;

	private Chore chore;

	public Chore.Precondition IsNotMarkedForDeconstruction = new Chore.Precondition
	{
		id = "IsNotMarkedForDeconstruction",
		description = DUPLICANTS.CHORES.PRECONDITIONS.IS_MARKED_FOR_DECONSTRUCTION,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Deconstructable deconstructable = data as Deconstructable;
			return deconstructable == null || !deconstructable.IsMarkedForDeconstruction();
		}
	};

	protected override void OnSpawn()
	{
		base.OnSpawn();
		chore = CreateChore();
	}

	private Chore CreateChore()
	{
		return new WorkChore<AstronautTrainingCenter>(Db.Get().ChoreTypes.Train, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: false);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		GetComponent<Operational>().SetActive(value: true);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		_ = worker == null;
		return true;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		if (chore != null && !chore.isComplete)
		{
			chore.Cancel("completed but not complete??");
		}
		chore = CreateChore();
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		GetComponent<Operational>().SetActive(value: false);
	}

	public override float GetPercentComplete()
	{
		_ = base.worker == null;
		return 0f;
	}
}
