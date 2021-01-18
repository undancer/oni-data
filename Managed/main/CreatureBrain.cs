public class CreatureBrain : Brain
{
	public string symbolPrefix;

	public Tag species;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Navigator component = GetComponent<Navigator>();
		if (component != null)
		{
			component.SetAbilities(new CreaturePathFinderAbilities(component));
		}
	}
}
