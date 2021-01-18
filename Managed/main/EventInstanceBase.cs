using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class EventInstanceBase : ISaveLoadable
{
	[Serialize]
	public int frame;

	[Serialize]
	public int eventHash;

	public EventBase ev;

	public EventInstanceBase(EventBase ev)
	{
		frame = GameClock.Instance.GetFrame();
		eventHash = ev.hash;
		this.ev = ev;
	}

	public override string ToString()
	{
		string str = "[" + frame + "] ";
		if (ev != null)
		{
			return str + ev.GetDescription(this);
		}
		return str + "Unknown event";
	}
}
