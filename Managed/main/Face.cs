public class Face : Resource
{
	public HashedString hash;

	public Face(string id)
		: base(id)
	{
		hash = new HashedString(id);
	}
}
