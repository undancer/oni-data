using UnityEngine;
using UnityEngine.UI;

public class Vignette : KMonoBehaviour
{
	[SerializeField]
	private Image image;

	public Color defaultColor;

	public Color redAlertColor = new Color(1f, 0f, 0f, 0.3f);

	public Color yellowAlertColor = new Color(1f, 1f, 0f, 0.3f);

	public static Vignette Instance;

	private LoopingSounds looping_sounds;

	private bool showingRedAlert = false;

	private bool showingYellowAlert = false;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnSpawn()
	{
		looping_sounds = GetComponent<LoopingSounds>();
		base.OnSpawn();
		Instance = this;
		defaultColor = image.color;
		Game.Instance.Subscribe(1983128072, Refresh);
		Game.Instance.Subscribe(1585324898, Refresh);
		Game.Instance.Subscribe(-1393151672, Refresh);
		Game.Instance.Subscribe(-741654735, Refresh);
		Game.Instance.Subscribe(-2062778933, Refresh);
	}

	public void SetColor(Color color)
	{
		image.color = color;
	}

	public void Refresh(object data)
	{
		AlertStateManager.Instance alertManager = ClusterManager.Instance.activeWorld.AlertManager;
		if (alertManager == null)
		{
			return;
		}
		if (alertManager.IsYellowAlert())
		{
			SetColor(yellowAlertColor);
			if (!showingYellowAlert)
			{
				looping_sounds.StartSound(GlobalAssets.GetSound("YellowAlert_LP"), pause_on_game_pause: true, enable_culling: false);
				showingYellowAlert = true;
			}
		}
		else
		{
			showingYellowAlert = false;
			looping_sounds.StopSound(GlobalAssets.GetSound("YellowAlert_LP"));
		}
		if (alertManager.IsRedAlert())
		{
			SetColor(redAlertColor);
			if (!showingRedAlert)
			{
				looping_sounds.StartSound(GlobalAssets.GetSound("RedAlert_LP"), pause_on_game_pause: true, enable_culling: false);
				showingRedAlert = true;
			}
		}
		else
		{
			showingRedAlert = false;
			looping_sounds.StopSound(GlobalAssets.GetSound("RedAlert_LP"));
		}
		if (!showingRedAlert && !showingYellowAlert)
		{
			Reset();
		}
	}

	public void Reset()
	{
		SetColor(defaultColor);
		showingRedAlert = false;
		showingYellowAlert = false;
		looping_sounds.StopSound(GlobalAssets.GetSound("RedAlert_LP"));
		looping_sounds.StopSound(GlobalAssets.GetSound("YellowAlert_LP"));
	}
}
