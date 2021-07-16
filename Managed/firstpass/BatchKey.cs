using System;

public struct BatchKey : IEquatable<BatchKey>
{
	private float _z;

	private int _layer;

	private KAnimBatchGroup.MaterialType _materialType;

	private HashedString _groupID;

	private Vector2I _idx;

	private int _hash;

	public float z => _z;

	public int layer => _layer;

	public HashedString groupID => _groupID;

	public Vector2I idx => _idx;

	public KAnimBatchGroup.MaterialType materialType => _materialType;

	public int hash => _hash;

	private BatchKey(KAnimConverter.IAnimConverter controller)
	{
		_layer = controller.GetLayer();
		_groupID = controller.GetBatchGroupID();
		_materialType = controller.GetMaterialType();
		_z = controller.GetZ();
		_idx = KAnimBatchManager.ControllerToChunkXY(controller);
		_hash = 0;
	}

	private BatchKey(KAnimConverter.IAnimConverter controller, Vector2I idx)
		: this(controller)
	{
		_idx = idx;
	}

	private void CalculateHash()
	{
		_hash = _z.GetHashCode() ^ _layer ^ (int)_materialType ^ _groupID.HashValue ^ _idx.GetHashCode();
	}

	public static BatchKey Create(KAnimConverter.IAnimConverter controller, Vector2I idx)
	{
		BatchKey result = new BatchKey(controller, idx);
		result.CalculateHash();
		return result;
	}

	public static BatchKey Create(KAnimConverter.IAnimConverter controller)
	{
		BatchKey result = new BatchKey(controller);
		result.CalculateHash();
		return result;
	}

	public bool Equals(BatchKey other)
	{
		if (_z == other._z && _layer == other._layer && _materialType == other._materialType && _groupID == other._groupID)
		{
			return _idx == other._idx;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _hash;
	}

	public override string ToString()
	{
		return "[" + idx.x + "," + idx.y + "] [" + groupID.HashValue + "] [" + layer + "] [" + z + "]" + materialType;
	}
}
