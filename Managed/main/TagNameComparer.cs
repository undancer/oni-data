using System.Collections.Generic;

public class TagNameComparer : IComparer<Tag>
{
	private Tag firstTag;

	public TagNameComparer()
	{
	}

	public TagNameComparer(Tag firstTag)
	{
		this.firstTag = firstTag;
	}

	public int Compare(Tag x, Tag y)
	{
		if (x == y)
		{
			return 0;
		}
		if (firstTag.IsValid)
		{
			if (x == firstTag && y != firstTag)
			{
				return 1;
			}
			if (x != firstTag && y == firstTag)
			{
				return -1;
			}
		}
		return x.ProperNameStripLink().CompareTo(y.ProperNameStripLink());
	}
}
