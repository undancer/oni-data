using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Rotatable")]
public class Rotatable : KMonoBehaviour, ISaveLoadable
{
	[MyCmpReq]
	private KBatchedAnimController batchedAnimController;

	[MyCmpGet]
	private Building building;

	[Serialize]
	[SerializeField]
	private Orientation orientation = Orientation.Neutral;

	[SerializeField]
	private Vector3 pivot = Vector3.zero;

	[SerializeField]
	private Vector3 visualizerOffset = Vector3.zero;

	public PermittedRotations permittedRotations = PermittedRotations.Unrotatable;

	[SerializeField]
	private int width;

	[SerializeField]
	private int height;

	public bool IsRotated => orientation != Orientation.Neutral;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (building != null)
		{
			BuildingDef def = GetComponent<Building>().Def;
			SetSize(def.WidthInCells, def.HeightInCells);
		}
		OrientVisualizer(orientation);
		OrientCollider(orientation);
	}

	public void SetSize(int width, int height)
	{
		this.width = width;
		this.height = height;
		if (width % 2 == 0)
		{
			pivot = new Vector3(-0.5f, 0.5f, 0f);
			visualizerOffset = new Vector3(0.5f, 0f, 0f);
		}
		else
		{
			pivot = new Vector3(0f, 0.5f, 0f);
			visualizerOffset = Vector3.zero;
		}
	}

	public Orientation Rotate()
	{
		switch (permittedRotations)
		{
		case PermittedRotations.R360:
			orientation = (Orientation)((int)(orientation + 1) % 4);
			break;
		case PermittedRotations.R90:
			orientation = ((orientation == Orientation.Neutral) ? Orientation.R90 : Orientation.Neutral);
			break;
		case PermittedRotations.FlipH:
			orientation = ((orientation == Orientation.Neutral) ? Orientation.FlipH : Orientation.Neutral);
			break;
		case PermittedRotations.FlipV:
			orientation = ((orientation == Orientation.Neutral) ? Orientation.FlipV : Orientation.Neutral);
			break;
		}
		OrientVisualizer(orientation);
		return orientation;
	}

	public void SetOrientation(Orientation new_orientation)
	{
		orientation = new_orientation;
		OrientVisualizer(new_orientation);
		OrientCollider(new_orientation);
	}

	public void Match(Rotatable other)
	{
		pivot = other.pivot;
		visualizerOffset = other.visualizerOffset;
		permittedRotations = other.permittedRotations;
		orientation = other.orientation;
		OrientVisualizer(orientation);
		OrientCollider(orientation);
	}

	public float GetVisualizerRotation()
	{
		PermittedRotations permittedRotations = this.permittedRotations;
		PermittedRotations permittedRotations2 = permittedRotations;
		if ((uint)(permittedRotations2 - 1) <= 1u)
		{
			return -90f * (float)orientation;
		}
		return 0f;
	}

	public bool GetVisualizerFlipX()
	{
		return orientation == Orientation.FlipH;
	}

	public bool GetVisualizerFlipY()
	{
		return orientation == Orientation.FlipV;
	}

	public Vector3 GetVisualizerPivot()
	{
		Vector3 result = pivot;
		switch (orientation)
		{
		case Orientation.FlipH:
			result.x = 0f - pivot.x;
			break;
		}
		return result;
	}

	private Vector3 GetVisualizerOffset()
	{
		Vector3 result;
		switch (orientation)
		{
		default:
			return visualizerOffset;
		case Orientation.FlipH:
			result = new Vector3(0f - visualizerOffset.x, visualizerOffset.y, visualizerOffset.z);
			break;
		case Orientation.FlipV:
			result = new Vector3(visualizerOffset.x, 1f, visualizerOffset.z);
			break;
		}
		return result;
	}

	private void OrientVisualizer(Orientation orientation)
	{
		float visualizerRotation = GetVisualizerRotation();
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		component.Pivot = GetVisualizerPivot();
		component.Rotation = visualizerRotation;
		component.Offset = GetVisualizerOffset();
		component.FlipX = GetVisualizerFlipX();
		component.FlipY = GetVisualizerFlipY();
		Trigger(-1643076535, this);
	}

	private void OrientCollider(Orientation orientation)
	{
		KBoxCollider2D component = GetComponent<KBoxCollider2D>();
		if (!(component == null))
		{
			float num = 0f;
			switch (orientation)
			{
			case Orientation.R90:
				num = -90f;
				break;
			case Orientation.R180:
				num = -180f;
				break;
			case Orientation.R270:
				num = -270f;
				break;
			case Orientation.FlipH:
				component.offset = new Vector2(width % 2 - 1, 0.5f * (float)height);
				component.size = new Vector2(width, height);
				break;
			case Orientation.FlipV:
				component.offset = new Vector2(0f, -0.5f * (float)(height - 2));
				component.size = new Vector2(width, height);
				break;
			default:
				component.offset = new Vector2(0f, 0.5f * (float)height);
				component.size = new Vector2(width, height);
				break;
			}
			if (num != 0f)
			{
				Matrix2x3 n = Matrix2x3.Translate(-pivot);
				Matrix2x3 n2 = Matrix2x3.Rotate(num * ((float)Math.PI / 180f));
				Matrix2x3 m = Matrix2x3.Translate(pivot);
				Matrix2x3 matrix2x = m * n2 * n;
				Vector2 v = new Vector2(-0.5f * (float)width, 0f);
				Vector2 v2 = new Vector2(0.5f * (float)width, height);
				Vector2 v3 = new Vector2(0f, 0.5f * (float)height);
				v = matrix2x.MultiplyPoint(v);
				v2 = matrix2x.MultiplyPoint(v2);
				v3 = matrix2x.MultiplyPoint(v3);
				float num2 = Mathf.Min(v.x, v2.x);
				float num3 = Mathf.Max(v.x, v2.x);
				float num4 = Mathf.Min(v.y, v2.y);
				float num5 = Mathf.Max(v.y, v2.y);
				component.offset = v3;
				component.size = new Vector2(num3 - num2, num5 - num4);
			}
		}
	}

	public CellOffset GetRotatedCellOffset(CellOffset offset)
	{
		return GetRotatedCellOffset(offset, orientation);
	}

	public static CellOffset GetRotatedCellOffset(CellOffset offset, Orientation orientation)
	{
		return orientation switch
		{
			Orientation.R90 => new CellOffset(offset.y, -offset.x), 
			Orientation.R180 => new CellOffset(-offset.x, -offset.y), 
			Orientation.R270 => new CellOffset(-offset.y, offset.x), 
			Orientation.FlipH => new CellOffset(-offset.x, offset.y), 
			Orientation.FlipV => new CellOffset(offset.x, -offset.y), 
			_ => offset, 
		};
	}

	public static CellOffset GetRotatedCellOffset(int x, int y, Orientation orientation)
	{
		return GetRotatedCellOffset(new CellOffset(x, y), orientation);
	}

	public Vector3 GetRotatedOffset(Vector3 offset)
	{
		return GetRotatedOffset(offset, orientation);
	}

	public static Vector3 GetRotatedOffset(Vector3 offset, Orientation orientation)
	{
		return orientation switch
		{
			Orientation.R90 => new Vector3(offset.y, 0f - offset.x), 
			Orientation.R180 => new Vector3(0f - offset.x, 0f - offset.y), 
			Orientation.R270 => new Vector3(0f - offset.y, offset.x), 
			Orientation.FlipH => new Vector3(0f - offset.x, offset.y), 
			Orientation.FlipV => new Vector3(offset.x, 0f - offset.y), 
			_ => offset, 
		};
	}

	public Orientation GetOrientation()
	{
		return orientation;
	}
}
