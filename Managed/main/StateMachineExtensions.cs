public static class StateMachineExtensions
{
	public static bool IsNullOrStopped(this StateMachine.Instance smi)
	{
		if (smi != null)
		{
			return !smi.IsRunning();
		}
		return true;
	}
}
