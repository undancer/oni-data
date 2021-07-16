public class ClosestEdibleSensor : Sensor
{
	private static TagBits edibleTagBits = new TagBits(GameTags.Edible);

	private Edible edible;

	private bool hasEdible;

	public bool edibleInReachButNotPermitted;

	public ClosestEdibleSensor(Sensors sensors)
		: base(sensors)
	{
	}

	public override void Update()
	{
		TagBits forbid_tags = new TagBits(GetComponent<ConsumableConsumer>().forbiddenTags);
		Pickupable pickupable = Game.Instance.fetchManager.FindEdibleFetchTarget(GetComponent<Storage>(), ref edibleTagBits, ref TagBits.None, ref forbid_tags, 0f);
		bool flag = edibleInReachButNotPermitted;
		Edible x = null;
		bool flag2 = false;
		if (pickupable != null)
		{
			x = pickupable.GetComponent<Edible>();
			flag2 = true;
			flag = false;
		}
		else
		{
			flag = Game.Instance.fetchManager.FindFetchTarget(GetComponent<Storage>(), ref edibleTagBits, ref TagBits.None, ref TagBits.None, 0f) != null;
		}
		if (x != edible || hasEdible != flag2)
		{
			edible = x;
			hasEdible = flag2;
			edibleInReachButNotPermitted = flag;
			Trigger(86328522, edible);
		}
	}

	public Edible GetEdible()
	{
		return edible;
	}
}
