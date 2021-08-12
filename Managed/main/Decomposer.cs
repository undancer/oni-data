using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Decomposer")]
public class Decomposer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		StateMachineController component = GetComponent<StateMachineController>();
		if (!(component == null))
		{
			DecompositionMonitor.Instance instance = new DecompositionMonitor.Instance(this, null, 1f, spawnRotMonsters: false);
			component.AddStateMachineInstance(instance);
			instance.StartSM();
			instance.dirtyWaterMaxRange = 3;
		}
	}
}
