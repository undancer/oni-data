public class GameplayEventPrecondition
{
	public delegate bool PreconditionFn();

	public string description;

	public PreconditionFn condition;

	public bool required;

	public int priorityModifier;
}
