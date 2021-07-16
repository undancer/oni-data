using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Facing")]
public class Facing : KMonoBehaviour
{
	[MyCmpGet]
	private KAnimControllerBase kanimController;

	private LoggerFS log;

	private bool facingLeft;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		log = new LoggerFS("Facing");
	}

	public void Face(float target_x)
	{
		float x = base.transform.GetLocalPosition().x;
		if (target_x < x)
		{
			facingLeft = true;
			UpdateMirror();
		}
		else if (target_x > x)
		{
			facingLeft = false;
			UpdateMirror();
		}
	}

	public void Face(Vector3 target_pos)
	{
		int num = Grid.CellColumn(Grid.PosToCell(base.transform.GetLocalPosition()));
		int num2 = Grid.CellColumn(Grid.PosToCell(target_pos));
		if (num > num2)
		{
			facingLeft = true;
			UpdateMirror();
		}
		else if (num2 > num)
		{
			facingLeft = false;
			UpdateMirror();
		}
	}

	[ContextMenu("Flip")]
	public void SwapFacing()
	{
		facingLeft = !facingLeft;
		UpdateMirror();
	}

	private void UpdateMirror()
	{
		if (kanimController != null && kanimController.FlipX != facingLeft)
		{
			kanimController.FlipX = facingLeft;
			_ = facingLeft;
		}
	}

	public bool GetFacing()
	{
		return facingLeft;
	}

	public void SetFacing(bool mirror_x)
	{
		facingLeft = mirror_x;
		UpdateMirror();
	}

	public int GetFrontCell()
	{
		int cell = Grid.PosToCell(this);
		if (GetFacing())
		{
			return Grid.CellLeft(cell);
		}
		return Grid.CellRight(cell);
	}

	public int GetBackCell()
	{
		int cell = Grid.PosToCell(this);
		if (!GetFacing())
		{
			return Grid.CellLeft(cell);
		}
		return Grid.CellRight(cell);
	}
}
