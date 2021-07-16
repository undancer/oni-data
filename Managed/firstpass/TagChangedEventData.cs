public struct TagChangedEventData
{
	public Tag tag;

	public bool added;

	public TagChangedEventData(Tag tag, bool added)
	{
		this.tag = tag;
		this.added = added;
	}
}
