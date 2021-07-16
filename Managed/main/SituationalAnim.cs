using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SituationalAnim")]
public class SituationalAnim : KMonoBehaviour
{
	[Flags]
	public enum Situation
	{
		Left = 0x1,
		Right = 0x2,
		Top = 0x4,
		Bottom = 0x8
	}

	public enum MustSatisfy
	{
		None,
		Any,
		All
	}

	public List<Tuple<Situation, string>> anims;

	public Func<int, bool> test;

	public MustSatisfy mustSatisfy;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Situation situation = GetSituation();
		DebugUtil.LogArgs("Situation is", situation);
		SetAnimForSituation(situation);
	}

	private void SetAnimForSituation(Situation situation)
	{
		foreach (Tuple<Situation, string> anim in anims)
		{
			if ((anim.first & situation) == anim.first)
			{
				DebugUtil.LogArgs("Chose Anim", anim.first, anim.second);
				SetAnim(anim.second);
				break;
			}
		}
	}

	private void SetAnim(string animName)
	{
		GetComponent<KBatchedAnimController>().Play(animName);
	}

	private Situation GetSituation()
	{
		Situation situation = (Situation)0;
		Extents extents = GetComponent<Building>().GetExtents();
		int x = extents.x;
		int num = extents.x + extents.width - 1;
		int y = extents.y;
		int num2 = extents.y + extents.height - 1;
		if (DoesSatisfy(GetSatisfactionForEdge(x, num, y - 1, y - 1), mustSatisfy))
		{
			situation |= Situation.Bottom;
		}
		if (DoesSatisfy(GetSatisfactionForEdge(x - 1, x - 1, y, num2), mustSatisfy))
		{
			situation |= Situation.Left;
		}
		if (DoesSatisfy(GetSatisfactionForEdge(x, num, num2 + 1, num2 + 1), mustSatisfy))
		{
			situation |= Situation.Top;
		}
		if (DoesSatisfy(GetSatisfactionForEdge(num + 1, num + 1, y, num2), mustSatisfy))
		{
			situation |= Situation.Right;
		}
		return situation;
	}

	private bool DoesSatisfy(MustSatisfy result, MustSatisfy requirement)
	{
		return requirement switch
		{
			MustSatisfy.All => result == MustSatisfy.All, 
			MustSatisfy.Any => result != MustSatisfy.None, 
			_ => result == MustSatisfy.None, 
		};
	}

	private MustSatisfy GetSatisfactionForEdge(int minx, int maxx, int miny, int maxy)
	{
		bool flag = false;
		bool flag2 = true;
		for (int i = minx; i <= maxx; i++)
		{
			for (int j = miny; j <= maxy; j++)
			{
				int arg = Grid.XYToCell(i, j);
				if (test(arg))
				{
					flag = true;
				}
				else
				{
					flag2 = false;
				}
			}
		}
		if (flag2)
		{
			return MustSatisfy.All;
		}
		if (flag)
		{
			return MustSatisfy.Any;
		}
		return MustSatisfy.None;
	}
}
