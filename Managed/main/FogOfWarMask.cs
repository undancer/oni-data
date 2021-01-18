using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FogOfWarMask")]
public class FogOfWarMask : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Grid.OnReveal = (Action<int>)Delegate.Combine(Grid.OnReveal, new Action<int>(OnReveal));
	}

	private void OnReveal(int cell)
	{
		if (Grid.PosToCell(this) == cell)
		{
			Grid.OnReveal = (Action<int>)Delegate.Remove(Grid.OnReveal, new Action<int>(OnReveal));
			base.gameObject.DeleteObject();
		}
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		GameUtil.FloodCollectCells(Grid.PosToCell(this), delegate(int cell)
		{
			Grid.Visible[cell] = 0;
			Grid.PreventFogOfWarReveal[cell] = true;
			return !Grid.Solid[cell];
		});
		GameUtil.FloodCollectCells(Grid.PosToCell(this), delegate(int cell)
		{
			bool flag = Grid.PreventFogOfWarReveal[cell];
			if (Grid.Solid[cell] && Grid.Foundation[cell])
			{
				Grid.PreventFogOfWarReveal[cell] = true;
				Grid.Visible[cell] = 0;
				GameObject gameObject = Grid.Objects[cell, 1];
				if (gameObject != null && gameObject.GetComponent<KPrefabID>().PrefabTag.ToString() == "POIBunkerExteriorDoor")
				{
					Grid.PreventFogOfWarReveal[cell] = false;
					Grid.Visible[cell] = byte.MaxValue;
				}
			}
			return flag || Grid.Foundation[cell];
		});
	}

	public static void ClearMask(int cell)
	{
		if (Grid.PreventFogOfWarReveal[cell])
		{
			GameUtil.FloodCollectCells(cell, RevealFogOfWarMask);
		}
	}

	public static bool RevealFogOfWarMask(int cell)
	{
		bool flag = Grid.PreventFogOfWarReveal[cell];
		if (flag)
		{
			Grid.PreventFogOfWarReveal[cell] = false;
			Grid.Reveal(cell);
		}
		return flag;
	}
}
