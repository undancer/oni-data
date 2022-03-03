using Klei.AI;
using UnityEngine;

public class BalloonStandConfig : IEntityConfig
{
	public static readonly string ID = "BalloonStand";

	private Chore.Precondition HasNoBalloon = new Chore.Precondition
	{
		id = "HasNoBalloon",
		description = "Duplicant doesn't have a balloon already",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.consumerState.consumer == null)
			{
				return false;
			}
			return (!context.consumerState.gameObject.GetComponent<Effects>().HasEffect("HasBalloon")) ? true : false;
		}
	};

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, ID, is_selectable: false);
		KAnimFile[] overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_balloon_receiver_kanim") };
		GetBalloonWorkable getBalloonWorkable = gameObject.AddOrGet<GetBalloonWorkable>();
		getBalloonWorkable.workTime = 2f;
		getBalloonWorkable.workLayer = Grid.SceneLayer.BuildingFront;
		getBalloonWorkable.overrideAnims = overrideAnims;
		getBalloonWorkable.synchronizeAnims = false;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		GetBalloonWorkable component = inst.GetComponent<GetBalloonWorkable>();
		WorkChore<GetBalloonWorkable> workChore = new WorkChore<GetBalloonWorkable>(Db.Get().ChoreTypes.JoyReaction, component, null, run_until_complete: true, MakeNewBalloonChore, null, null, allow_in_red_alert: true, Db.Get().ScheduleBlockTypes.Recreation, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: true, PriorityScreen.PriorityClass.high, 5, ignore_building_assignment: true);
		workChore.AddPrecondition(HasNoBalloon, workChore);
		workChore.AddPrecondition(ChorePreconditions.instance.IsNotARobot, workChore);
	}

	private void MakeNewBalloonChore(Chore chore)
	{
		GetBalloonWorkable component = chore.target.GetComponent<GetBalloonWorkable>();
		WorkChore<GetBalloonWorkable> workChore = new WorkChore<GetBalloonWorkable>(Db.Get().ChoreTypes.JoyReaction, component, null, run_until_complete: true, MakeNewBalloonChore, null, null, allow_in_red_alert: true, Db.Get().ScheduleBlockTypes.Recreation, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: true, PriorityScreen.PriorityClass.high, 5, ignore_building_assignment: true);
		workChore.AddPrecondition(HasNoBalloon, workChore);
		workChore.AddPrecondition(ChorePreconditions.instance.IsNotARobot, workChore);
	}
}
