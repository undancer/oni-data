using FMODUnity;
using Klei.AI;
using ProcGenGame;
using UnityEngine;

public class NewBaseScreen : KScreen
{
	public static NewBaseScreen Instance;

	[SerializeField]
	private CanvasGroup[] disabledUIElements;

	[EventRef]
	public string ScanSoundMigrated;

	[EventRef]
	public string BuildBaseSoundMigrated;

	private ITelepadDeliverable[] m_minionStartingStats;

	private Cluster m_clusterLayout;

	public override float GetSortKey()
	{
		return 1f;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		base.OnPrefabInit();
		TimeOfDay.Instance.SetScale(0f);
	}

	protected override void OnForcedCleanUp()
	{
		Instance = null;
		base.OnForcedCleanUp();
	}

	public static Vector2I SetInitialCamera()
	{
		Vector2I baseStartPos = SaveLoader.Instance.cachedGSD.baseStartPos;
		baseStartPos += ClusterManager.Instance.GetStartWorld().WorldOffset;
		Vector3 pos = Grid.CellToPosCCC(Grid.OffsetCell(Grid.OffsetCell(0, baseStartPos.x, baseStartPos.y), 0, -2), Grid.SceneLayer.Background);
		CameraController.Instance.SetMaxOrthographicSize(40f);
		CameraController.Instance.SnapTo(pos);
		CameraController.Instance.SetTargetPos(pos, 20f, playSound: false);
		CameraController.Instance.SetOrthographicsSize(40f);
		CameraSaveData.valid = false;
		return baseStartPos;
	}

	protected override void OnActivate()
	{
		if (disabledUIElements != null)
		{
			CanvasGroup[] array = disabledUIElements;
			foreach (CanvasGroup canvasGroup in array)
			{
				if (canvasGroup != null)
				{
					canvasGroup.interactable = false;
				}
			}
		}
		SetInitialCamera();
		if (SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Unpause(playSound: false);
		}
		Final();
	}

	public void Init(Cluster clusterLayout, ITelepadDeliverable[] startingMinionStats)
	{
		m_clusterLayout = clusterLayout;
		m_minionStartingStats = startingMinionStats;
	}

	protected override void OnDeactivate()
	{
		Game.Instance.Trigger(-122303817);
		if (disabledUIElements == null)
		{
			return;
		}
		CanvasGroup[] array = disabledUIElements;
		foreach (CanvasGroup canvasGroup in array)
		{
			if (canvasGroup != null)
			{
				canvasGroup.interactable = true;
			}
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		Action[] array = new Action[4]
		{
			Action.SpeedUp,
			Action.SlowDown,
			Action.TogglePause,
			Action.CycleSpeed
		};
		if (!e.Consumed)
		{
			for (int i = 0; i < array.Length && !e.TryConsume(array[i]); i++)
			{
			}
		}
	}

	private void Final()
	{
		SpeedControlScreen.Instance.Unpause(playSound: false);
		GameObject telepad = GameUtil.GetTelepad(ClusterManager.Instance.GetStartWorld().id);
		if ((bool)telepad)
		{
			SpawnMinions(Grid.PosToCell(telepad));
		}
		Game.Instance.baseAlreadyCreated = true;
		Deactivate();
	}

	private void SpawnMinions(int headquartersCell)
	{
		if (headquartersCell == -1)
		{
			Debug.LogWarning("No headquarters in saved base template. Cannot place minions. Confirm there is a headquarters saved to the base template, or consider creating a new one.");
			return;
		}
		Grid.CellToXY(headquartersCell, out var x, out var y);
		if (Grid.WidthInCells < 64)
		{
			return;
		}
		int baseLeft = m_clusterLayout.currentWorld.BaseLeft;
		int baseRight = m_clusterLayout.currentWorld.BaseRight;
		Effect a_new_hope = Db.Get().effects.Get("AnewHope");
		for (int i = 0; i < m_minionStartingStats.Length; i++)
		{
			int x2 = x + i % (baseRight - baseLeft) + 1;
			int y2 = y;
			int cell = Grid.XYToCell(x2, y2);
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID));
			Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
			gameObject.transform.SetLocalPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
			gameObject.SetActive(value: true);
			((MinionStartingStats)m_minionStartingStats[i]).Apply(gameObject);
			GameScheduler.Instance.Schedule("ANewHope", 3f + 0.5f * (float)i, delegate(object m)
			{
				GameObject gameObject2 = m as GameObject;
				if (!(gameObject2 == null))
				{
					gameObject2.GetComponent<Effects>().Add(a_new_hope, should_save: true);
				}
			}, gameObject);
		}
		ClusterManager.Instance.activeWorld.SetDupeVisited();
	}
}
