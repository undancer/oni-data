using System;
using KSerialization;
using UnityEngine;

[Serializable]
[SerializationConfig(MemberSerialization.OptIn)]
public struct HashedString : IComparable<HashedString>, IEquatable<HashedString>, ISerializationCallbackReceiver
{
	public static HashedString Invalid;

	[SerializeField]
	[Serialize]
	private int hash;

	public bool IsValid => HashValue != 0;

	public int HashValue
	{
		get
		{
			return hash;
		}
		set
		{
			hash = value;
		}
	}

	public static implicit operator HashedString(string s)
	{
		return new HashedString(s);
	}

	public HashedString(string name)
	{
		hash = global::Hash.SDBMLower(name);
	}

	public static int Hash(string name)
	{
		return global::Hash.SDBMLower(name);
	}

	public HashedString(int initial_hash)
	{
		hash = initial_hash;
	}

	public int CompareTo(HashedString obj)
	{
		return hash - obj.hash;
	}

	public override bool Equals(object obj)
	{
		HashedString hashedString = (HashedString)obj;
		return hash == hashedString.hash;
	}

	public bool Equals(HashedString other)
	{
		return hash == other.hash;
	}

	public override int GetHashCode()
	{
		return hash;
	}

	public static bool operator ==(HashedString x, HashedString y)
	{
		return x.hash == y.hash;
	}

	public static bool operator !=(HashedString x, HashedString y)
	{
		return x.hash != y.hash;
	}

	public static implicit operator HashedString(KAnimHashedString hash)
	{
		return new HashedString(hash.HashValue);
	}

	public override string ToString()
	{
		return "0x" + hash.ToString("X");
	}

	public void OnAfterDeserialize()
	{
	}

	public void OnBeforeSerialize()
	{
	}
}
