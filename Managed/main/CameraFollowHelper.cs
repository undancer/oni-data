using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CameraFollowHelper")]
public class CameraFollowHelper : KMonoBehaviour
{
	private void LateUpdate()
	{
		if (CameraController.Instance != null)
		{
			CameraController.Instance.UpdateFollowTarget();
		}
	}
}
