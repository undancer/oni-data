using UnityEngine;

public class ScreenResolutionMonitor : MonoBehaviour
{
	[SerializeField]
	private Vector2 previousSize;

	private static bool previousGamepadUIMode;

	private const float HIGH_DPI = 130f;

	private void Awake()
	{
		previousSize = new Vector2(Screen.width, Screen.height);
	}

	private void Update()
	{
		if ((previousSize.x != (float)Screen.width || previousSize.y != (float)Screen.height) && Game.Instance != null)
		{
			Game.Instance.Trigger(445618876);
			previousSize.x = Screen.width;
			previousSize.y = Screen.height;
		}
		UpdateShouldUseGamepadUIMode();
	}

	public static bool UsingGamepadUIMode()
	{
		return previousGamepadUIMode;
	}

	private void UpdateShouldUseGamepadUIMode()
	{
		bool flag = (Screen.dpi > 130f && Screen.height < 900) || KInputManager.currentControllerIsGamepad;
		if (flag != previousGamepadUIMode)
		{
			previousGamepadUIMode = flag;
			if (!(Game.Instance == null))
			{
				Game.Instance.Trigger(-442024484);
			}
		}
	}
}
