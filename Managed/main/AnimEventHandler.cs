using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AnimEventHandler")]
public class AnimEventHandler : KMonoBehaviour
{
	private delegate void SetPos(Vector3 pos);

	private const int UPDATE_FRAME_RATE = 3;

	[MyCmpGet]
	private KBatchedAnimController controller;

	[MyCmpGet]
	private KBoxCollider2D animCollider;

	[MyCmpGet]
	private Navigator navigator;

	private Vector3 targetPos;

	public Vector2 baseOffset;

	public int isDirty;

	private HashedString context;

	private int instanceIndex;

	private static int InstanceSequence;

	private event SetPos onWorkTargetSet;

	public void SetDirty()
	{
		isDirty = 2;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimTracker[] componentsInChildren = GetComponentsInChildren<KBatchedAnimTracker>(includeInactive: true);
		foreach (KBatchedAnimTracker kBatchedAnimTracker in componentsInChildren)
		{
			if (kBatchedAnimTracker.useTargetPoint)
			{
				onWorkTargetSet += kBatchedAnimTracker.SetTarget;
			}
		}
		baseOffset = animCollider.offset;
		instanceIndex = InstanceSequence++;
		SetDirty();
	}

	protected override void OnForcedCleanUp()
	{
		navigator = null;
		base.OnForcedCleanUp();
	}

	public HashedString GetContext()
	{
		return context;
	}

	public void UpdateWorkTarget(Vector3 pos)
	{
		if (this.onWorkTargetSet != null)
		{
			this.onWorkTargetSet(pos);
		}
	}

	public void SetContext(HashedString context)
	{
		this.context = context;
	}

	public void SetTargetPos(Vector3 target_pos)
	{
		targetPos = target_pos;
	}

	public Vector3 GetTargetPos()
	{
		return targetPos;
	}

	public void ClearContext()
	{
		context = default(HashedString);
	}

	public void LateUpdate()
	{
		int num = Time.frameCount % 3;
		int num2 = instanceIndex % 3;
		if (num == num2 || isDirty > 0)
		{
			UpdateOffset();
		}
	}

	public void UpdateOffset()
	{
		Vector3 pivotSymbolPosition = controller.GetPivotSymbolPosition();
		Vector3 vector = navigator.NavGrid.GetNavTypeData(navigator.CurrentNavType).animControllerOffset;
		animCollider.offset = new Vector2(baseOffset.x + pivotSymbolPosition.x - base.transform.GetPosition().x - vector.x, baseOffset.y + pivotSymbolPosition.y - base.transform.GetPosition().y + vector.y);
		isDirty = Mathf.Max(0, isDirty - 1);
	}
}
