using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/RobotExhaustPipe")]
public class RobotExhaustPipe : KMonoBehaviour, ISim4000ms
{
	private float CO2_RATE = 0.001f;

	public void Sim4000ms(float dt)
	{
		Facing component = GetComponent<Facing>();
		bool flip = false;
		if ((bool)component)
		{
			flip = component.GetFacing();
		}
		CO2Manager.instance.SpawnBreath(Grid.CellToPos(Grid.PosToCell(base.gameObject)), dt * CO2_RATE, 303.15f, flip);
	}
}
