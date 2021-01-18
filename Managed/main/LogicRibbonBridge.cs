public class LogicRibbonBridge : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		Rotatable component2 = GetComponent<Rotatable>();
		switch (component2.GetOrientation())
		{
		case Orientation.Neutral:
			component.Play("0");
			break;
		case Orientation.R90:
			component.Play("90");
			break;
		case Orientation.R180:
			component.Play("180");
			break;
		case Orientation.R270:
			component.Play("270");
			break;
		}
	}
}
