using UnityEngine;

public class AttackTool : DragTool
{
	protected override void OnDragComplete(Vector3 downPos, Vector3 upPos)
	{
		Vector2 regularizedPos = GetRegularizedPos(Vector2.Min(downPos, upPos), minimize: true);
		Vector2 regularizedPos2 = GetRegularizedPos(Vector2.Max(downPos, upPos), minimize: false);
		MarkForAttack(regularizedPos, regularizedPos2, mark: true);
	}

	public static void MarkForAttack(Vector2 min, Vector2 max, bool mark)
	{
		foreach (FactionAlignment item in Components.FactionAlignments.Items)
		{
			Vector2 vector = Grid.PosToXY(item.transform.GetPosition());
			if (!(vector.x >= min.x) || !(vector.x < max.x) || !(vector.y >= min.y) || !(vector.y < max.y))
			{
				continue;
			}
			if (mark)
			{
				if (FactionManager.Instance.GetDisposition(FactionManager.FactionID.Duplicant, item.Alignment) != 0)
				{
					item.SetPlayerTargeted(state: true);
				}
			}
			else
			{
				item.gameObject.Trigger(2127324410);
			}
		}
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		ToolMenu.Instance.PriorityScreen.Show();
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenu.Instance.PriorityScreen.Show(show: false);
	}
}
