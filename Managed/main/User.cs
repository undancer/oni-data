using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/User")]
public class User : KMonoBehaviour
{
	public void OnStateMachineStop(string reason, StateMachine.Status status)
	{
		if (status == StateMachine.Status.Success)
		{
			Trigger(58624316);
		}
		else
		{
			Trigger(1572098533);
		}
	}
}
