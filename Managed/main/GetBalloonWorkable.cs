using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/GetBalloonWorkable")]
public class GetBalloonWorkable : Workable
{
	private static readonly HashedString[] GET_BALLOON_ANIMS = new HashedString[2]
	{
		"working_pre",
		"working_loop"
	};

	private static readonly HashedString PST_ANIM = new HashedString("working_pst");

	private BalloonArtistChore.StatesInstance balloonArtist;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		faceTargetWhenWorking = true;
		workerStatusItem = null;
		workingStatusItem = null;
		workAnims = GET_BALLOON_ANIMS;
		workingPstComplete = new HashedString[1]
		{
			PST_ANIM
		};
		workingPstFailed = new HashedString[1]
		{
			PST_ANIM
		};
	}

	protected override void OnCompleteWork(Worker worker)
	{
		balloonArtist.GiveBalloon();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("EquippableBalloon"), worker.transform.GetPosition());
		gameObject.GetComponent<Equippable>().Assign(worker.GetComponent<MinionIdentity>());
		gameObject.GetComponent<Equippable>().isEquipped = true;
		gameObject.SetActive(value: true);
		base.OnCompleteWork(worker);
	}

	public override Vector3 GetFacingTarget()
	{
		return balloonArtist.master.transform.GetPosition();
	}

	public void SetBalloonArtist(BalloonArtistChore.StatesInstance chore)
	{
		balloonArtist = chore;
	}
}
