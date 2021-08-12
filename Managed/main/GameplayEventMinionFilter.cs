public class GameplayEventMinionFilter
{
	public delegate bool FilterFn(MinionIdentity minion);

	public string id;

	public FilterFn filter;
}
