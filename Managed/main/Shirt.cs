public class Shirt : Resource
{
	public HashedString hash;

	public Shirt(string id)
		: base(id)
	{
		hash = new HashedString(id);
	}
}
