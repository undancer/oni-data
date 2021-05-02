public class Face : Resource
{
	public HashedString hash;

	public HashedString headFXHash;

	private const string SYMBOL_PREFIX = "headfx_";

	public Face(string id, string headFXSymbol = null)
		: base(id)
	{
		hash = new HashedString(id);
		headFXHash = headFXSymbol;
	}
}
