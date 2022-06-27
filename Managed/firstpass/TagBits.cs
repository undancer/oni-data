using System.Collections.Generic;

public struct TagBits
{
	private static Dictionary<Tag, int> tagTable = new Dictionary<Tag, int>();

	private static List<Tag> inverseTagTable = new List<Tag>();

	private const int Capacity = 576;

	private ulong bits0;

	private ulong bits1;

	private ulong bits2;

	private ulong bits3;

	private ulong bits4;

	private ulong bits5;

	private ulong bits6;

	private ulong bits7;

	private ulong bits8;

	public static TagBits None = default(TagBits);

	public TagBits(ref TagBits other)
	{
		bits0 = other.bits0;
		bits1 = other.bits1;
		bits2 = other.bits2;
		bits3 = other.bits3;
		bits4 = other.bits4;
		bits5 = other.bits5;
		bits6 = other.bits6;
		bits7 = other.bits7;
		bits8 = other.bits8;
	}

	public TagBits(Tag tag)
	{
		bits0 = 0uL;
		bits1 = 0uL;
		bits2 = 0uL;
		bits3 = 0uL;
		bits4 = 0uL;
		bits5 = 0uL;
		bits6 = 0uL;
		bits7 = 0uL;
		bits8 = 0uL;
		SetTag(tag);
	}

	public TagBits(Tag[] tags)
	{
		bits0 = 0uL;
		bits1 = 0uL;
		bits2 = 0uL;
		bits3 = 0uL;
		bits4 = 0uL;
		bits5 = 0uL;
		bits6 = 0uL;
		bits7 = 0uL;
		bits8 = 0uL;
		if (tags != null)
		{
			foreach (Tag tag in tags)
			{
				SetTag(tag);
			}
		}
	}

	public List<Tag> GetTagsVerySlow()
	{
		List<Tag> list = new List<Tag>();
		GetTagsVerySlow(0, bits0, list);
		GetTagsVerySlow(1, bits1, list);
		GetTagsVerySlow(2, bits2, list);
		GetTagsVerySlow(3, bits3, list);
		GetTagsVerySlow(4, bits4, list);
		GetTagsVerySlow(5, bits5, list);
		GetTagsVerySlow(6, bits6, list);
		GetTagsVerySlow(7, bits7, list);
		GetTagsVerySlow(8, bits8, list);
		return list;
	}

	private void GetTagsVerySlow(int bits_idx, ulong bits, List<Tag> tags)
	{
		for (int i = 0; i < 64; i++)
		{
			if ((bits & (ulong)(1L << i)) != 0L)
			{
				int index = 64 * bits_idx + i;
				tags.Add(inverseTagTable[index]);
			}
		}
	}

	private static int ManifestFlagIndex(Tag tag)
	{
		if (tagTable.TryGetValue(tag, out var value))
		{
			return value;
		}
		value = tagTable.Count;
		tagTable.Add(tag, value);
		inverseTagTable.Add(tag);
		DebugUtil.Assert(inverseTagTable.Count == value + 1);
		if (tagTable.Count >= 576)
		{
			string text = "Out of tag bits:\n";
			int num = 0;
			foreach (KeyValuePair<Tag, int> item in tagTable)
			{
				text = text + item.Key.ToString() + ", ";
				num++;
				if (num % 64 == 0)
				{
					text += "\n";
				}
			}
			Debug.LogError(text);
		}
		return value;
	}

	public void SetTag(Tag tag)
	{
		int num = ManifestFlagIndex(tag);
		if (num < 64)
		{
			bits0 |= (ulong)(1L << num);
		}
		else if (num < 128)
		{
			bits1 |= (ulong)(1L << num);
		}
		else if (num < 192)
		{
			bits2 |= (ulong)(1L << num);
		}
		else if (num < 256)
		{
			bits3 |= (ulong)(1L << num);
		}
		else if (num < 320)
		{
			bits4 |= (ulong)(1L << num);
		}
		else if (num < 384)
		{
			bits5 |= (ulong)(1L << num);
		}
		else if (num < 448)
		{
			bits6 |= (ulong)(1L << num);
		}
		else if (num < 512)
		{
			bits7 |= (ulong)(1L << num);
		}
		else if (num < 576)
		{
			bits8 |= (ulong)(1L << num);
		}
		else
		{
			Debug.LogError("Out of bits!");
		}
	}

	public void Clear(Tag tag)
	{
		int num = ManifestFlagIndex(tag);
		if (num < 64)
		{
			bits0 &= (ulong)(~(1L << num));
		}
		else if (num < 128)
		{
			bits1 &= (ulong)(~(1L << num));
		}
		else if (num < 192)
		{
			bits2 &= (ulong)(~(1L << num));
		}
		else if (num < 256)
		{
			bits3 &= (ulong)(~(1L << num));
		}
		else if (num < 320)
		{
			bits4 &= (ulong)(~(1L << num));
		}
		else if (num < 384)
		{
			bits5 &= (ulong)(~(1L << num));
		}
		else if (num < 448)
		{
			bits6 &= (ulong)(~(1L << num));
		}
		else if (num < 512)
		{
			bits7 &= (ulong)(~(1L << num));
		}
		else if (num < 576)
		{
			bits8 &= (ulong)(~(1L << num));
		}
		else
		{
			Debug.LogError("Out of bits!");
		}
	}

	public void ClearAll()
	{
		bits0 = 0uL;
		bits1 = 0uL;
		bits2 = 0uL;
		bits3 = 0uL;
		bits4 = 0uL;
		bits5 = 0uL;
		bits6 = 0uL;
		bits7 = 0uL;
		bits8 = 0uL;
	}

	public bool HasAll(ref TagBits tag_bits)
	{
		if ((bits0 & tag_bits.bits0) == tag_bits.bits0 && (bits1 & tag_bits.bits1) == tag_bits.bits1 && (bits2 & tag_bits.bits2) == tag_bits.bits2 && (bits3 & tag_bits.bits3) == tag_bits.bits3 && (bits4 & tag_bits.bits4) == tag_bits.bits4 && (bits5 & tag_bits.bits5) == tag_bits.bits5 && (bits6 & tag_bits.bits6) == tag_bits.bits6 && (bits7 & tag_bits.bits7) == tag_bits.bits7)
		{
			return (bits8 & tag_bits.bits8) == tag_bits.bits8;
		}
		return false;
	}

	public bool HasAny(ref TagBits tag_bits)
	{
		return ((bits0 & tag_bits.bits0) | (bits1 & tag_bits.bits1) | (bits2 & tag_bits.bits2) | (bits3 & tag_bits.bits3) | (bits4 & tag_bits.bits4) | (bits5 & tag_bits.bits5) | (bits6 & tag_bits.bits6) | (bits7 & tag_bits.bits7) | (bits8 & tag_bits.bits8)) != 0;
	}

	public bool AreEqual(ref TagBits tag_bits)
	{
		if (tag_bits.bits0 == bits0 && tag_bits.bits1 == bits1 && tag_bits.bits2 == bits2 && tag_bits.bits3 == bits3 && tag_bits.bits4 == bits4 && tag_bits.bits5 == bits5 && tag_bits.bits6 == bits6 && tag_bits.bits7 == bits7)
		{
			return tag_bits.bits8 == bits8;
		}
		return false;
	}

	public void And(ref TagBits rhs)
	{
		bits0 &= rhs.bits0;
		bits1 &= rhs.bits1;
		bits2 &= rhs.bits2;
		bits3 &= rhs.bits3;
		bits4 &= rhs.bits4;
		bits5 &= rhs.bits5;
		bits6 &= rhs.bits6;
		bits7 &= rhs.bits7;
		bits8 &= rhs.bits8;
	}

	public void Or(ref TagBits rhs)
	{
		bits0 |= rhs.bits0;
		bits1 |= rhs.bits1;
		bits2 |= rhs.bits2;
		bits3 |= rhs.bits3;
		bits4 |= rhs.bits4;
		bits5 |= rhs.bits5;
		bits6 |= rhs.bits6;
		bits7 |= rhs.bits7;
		bits8 |= rhs.bits8;
	}

	public void Xor(ref TagBits rhs)
	{
		bits0 ^= rhs.bits0;
		bits1 ^= rhs.bits1;
		bits2 ^= rhs.bits2;
		bits3 ^= rhs.bits3;
		bits4 ^= rhs.bits4;
		bits5 ^= rhs.bits5;
		bits6 ^= rhs.bits6;
		bits7 ^= rhs.bits7;
		bits8 ^= rhs.bits8;
	}

	public void Complement()
	{
		bits0 = ~bits0;
		bits1 = ~bits1;
		bits2 = ~bits2;
		bits3 = ~bits3;
		bits4 = ~bits4;
		bits5 = ~bits5;
		bits6 = ~bits6;
		bits7 = ~bits7;
		bits8 = ~bits8;
	}

	public static TagBits MakeComplement(ref TagBits rhs)
	{
		TagBits result = new TagBits(ref rhs);
		result.Complement();
		return result;
	}
}
