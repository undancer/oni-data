using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LightSymbolTracker")]
public class LightSymbolTracker : KMonoBehaviour, IRenderEveryTick
{
	public HashedString targetSymbol;

	public void RenderEveryTick(float dt)
	{
		Vector3 zero = Vector3.zero;
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		zero = (component.GetTransformMatrix() * component.GetSymbolLocalTransform(targetSymbol, out var _)).MultiplyPoint(Vector3.zero) - base.transform.GetPosition();
		GetComponent<Light2D>().Offset = zero;
	}
}
