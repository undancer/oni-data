public class HEPBridgeTileVisualizer : KMonoBehaviour, IHighEnergyParticleDirection
{
	private static readonly EventSystem.IntraObjectHandler<HEPBridgeTileVisualizer> OnRotateDelegate = new EventSystem.IntraObjectHandler<HEPBridgeTileVisualizer>(delegate(HEPBridgeTileVisualizer component, object data)
	{
		component.OnRotate();
	});

	public EightDirection Direction
	{
		get
		{
			EightDirection result = EightDirection.Right;
			Rotatable component = GetComponent<Rotatable>();
			if (component != null)
			{
				switch (component.Orientation)
				{
				case Orientation.Neutral:
					result = EightDirection.Left;
					break;
				case Orientation.R90:
					result = EightDirection.Up;
					break;
				case Orientation.R180:
					result = EightDirection.Right;
					break;
				case Orientation.R270:
					result = EightDirection.Down;
					break;
				}
			}
			return result;
		}
		set
		{
		}
	}

	protected override void OnSpawn()
	{
		Subscribe(-1643076535, OnRotateDelegate);
		OnRotate();
	}

	public void OnRotate()
	{
		Game.Instance.ForceOverlayUpdate(clearLastMode: true);
	}
}
