using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AnimEventHandler")]
public class AnimEventHandler : KMonoBehaviour
{
	private delegate void SetPos(Vector3 pos);

	private KBatchedAnimController controller;

	private KBoxCollider2D animCollider;

	private Vector3 targetPos;

	private Vector2 baseOffset;

	private HashedString context;

	private event SetPos onWorkTargetSet;

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
		controller = GetComponent<KBatchedAnimController>();
		animCollider = GetComponent<KBoxCollider2D>();
		baseOffset = animCollider.offset;
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
		Vector3 pivotSymbolPosition = controller.GetPivotSymbolPosition();
		animCollider.offset = new Vector2(baseOffset.x + pivotSymbolPosition.x - base.transform.GetPosition().x, baseOffset.y + pivotSymbolPosition.y - base.transform.GetPosition().y);
	}
}
