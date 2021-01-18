using System.Collections.Generic;
using UnityEngine;

public class KBatchedAnimTracker : MonoBehaviour
{
	[SerializeField]
	public KBatchedAnimController controller;

	[SerializeField]
	public Vector3 offset = Vector3.zero;

	public HashedString symbol;

	public Vector3 targetPoint = Vector3.zero;

	public Vector3 previousTargetPoint;

	public bool useTargetPoint;

	public bool fadeOut = true;

	public bool skipInitialDisable;

	public bool forceAlwaysVisible;

	public bool matchParentOffset;

	private bool alive = true;

	private bool forceUpdate;

	private Matrix2x3 previousMatrix;

	private Vector3 previousPosition;

	private KBatchedAnimController myAnim;

	private void Start()
	{
		if (controller == null)
		{
			Transform parent = base.transform.parent;
			while (parent != null)
			{
				controller = parent.GetComponent<KBatchedAnimController>();
				if (controller != null)
				{
					break;
				}
				parent = parent.parent;
			}
		}
		if (controller == null)
		{
			Debug.Log("Controller Null for tracker on " + base.gameObject.name, base.gameObject);
			base.enabled = false;
			return;
		}
		controller.onAnimEnter += OnAnimStart;
		controller.onAnimComplete += OnAnimStop;
		controller.onLayerChanged += OnLayerChanged;
		forceUpdate = true;
		myAnim = GetComponent<KBatchedAnimController>();
		List<KAnimControllerBase> list = new List<KAnimControllerBase>(GetComponentsInChildren<KAnimControllerBase>(includeInactive: true));
		if (!skipInitialDisable)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				base.transform.GetChild(i).gameObject.SetActive(value: false);
			}
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].gameObject == base.gameObject)
			{
				list.RemoveAt(num);
			}
		}
	}

	private void OnDestroy()
	{
		if (controller != null)
		{
			controller.onAnimEnter -= OnAnimStart;
			controller.onAnimComplete -= OnAnimStop;
			controller.onLayerChanged -= OnLayerChanged;
			controller = null;
		}
		myAnim = null;
	}

	private void LateUpdate()
	{
		if (controller != null && (controller.IsVisible() || forceAlwaysVisible || forceUpdate))
		{
			UpdateFrame();
		}
		if (!alive)
		{
			base.enabled = false;
		}
	}

	private void UpdateFrame()
	{
		forceUpdate = false;
		bool symbolVisible = false;
		if (controller.CurrentAnim != null)
		{
			Matrix2x3 symbolLocalTransform = controller.GetSymbolLocalTransform(symbol, out symbolVisible);
			Vector3 position = controller.transform.GetPosition();
			if (symbolVisible && (previousMatrix != symbolLocalTransform || position != previousPosition || (useTargetPoint && targetPoint != previousTargetPoint) || (matchParentOffset && myAnim.Offset != controller.Offset)))
			{
				previousMatrix = symbolLocalTransform;
				previousPosition = position;
				Matrix2x3 overrideTransformMatrix = controller.GetTransformMatrix() * symbolLocalTransform;
				float z = base.transform.GetPosition().z;
				base.transform.SetPosition(overrideTransformMatrix.MultiplyPoint(offset));
				if (useTargetPoint)
				{
					previousTargetPoint = targetPoint;
					Vector3 position2 = base.transform.GetPosition();
					position2.z = 0f;
					Vector3 from = targetPoint - position2;
					float num = Vector3.Angle(from, Vector3.right);
					if (from.y < 0f)
					{
						num = 360f - num;
					}
					base.transform.localRotation = Quaternion.identity;
					base.transform.RotateAround(position2, new Vector3(0f, 0f, 1f), num);
					float sqrMagnitude = from.sqrMagnitude;
					myAnim.GetBatchInstanceData().SetClipRadius(base.transform.GetPosition().x, base.transform.GetPosition().y, sqrMagnitude, do_clip: true);
				}
				else
				{
					Vector3 v = (controller.FlipX ? Vector3.left : Vector3.right);
					Vector3 v2 = (controller.FlipY ? Vector3.down : Vector3.up);
					base.transform.up = overrideTransformMatrix.MultiplyVector(v2);
					base.transform.right = overrideTransformMatrix.MultiplyVector(v);
					if (myAnim != null)
					{
						myAnim.GetBatchInstanceData()?.SetOverrideTransformMatrix(overrideTransformMatrix);
					}
				}
				base.transform.SetPosition(new Vector3(base.transform.GetPosition().x, base.transform.GetPosition().y, z));
				if (matchParentOffset)
				{
					myAnim.Offset = controller.Offset;
				}
				myAnim.SetDirty();
			}
		}
		if (myAnim != null && symbolVisible != myAnim.enabled)
		{
			myAnim.enabled = symbolVisible;
		}
	}

	[ContextMenu("ForceAlive")]
	private void OnAnimStart(HashedString name)
	{
		alive = true;
		base.enabled = true;
		forceUpdate = true;
	}

	private void OnAnimStop(HashedString name)
	{
		alive = false;
	}

	private void OnLayerChanged(int layer)
	{
		myAnim.SetLayer(layer);
	}

	public void SetTarget(Vector3 target)
	{
		targetPoint = target;
		targetPoint.z = 0f;
	}
}
