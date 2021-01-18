public class NavTeleportTransitionLayer : TransitionDriver.OverrideLayer
{
	public NavTeleportTransitionLayer(Navigator navigator)
		: base(navigator)
	{
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		if (transition.start == NavType.Teleport)
		{
			int num = Grid.PosToCell(navigator);
			Grid.CellToXY(num, out var x, out var y);
			int num2 = navigator.NavGrid.teleportTransitions[num];
			Grid.CellToXY(navigator.NavGrid.teleportTransitions[num], out var x2, out var y2);
			transition.x = x2 - x;
			transition.y = y2 - y;
		}
	}
}
