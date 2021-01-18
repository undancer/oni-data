using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/RocketThrustWidget")]
public class RocketThrustWidget : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Image graphBar;

	public Image graphDot;

	public LocText graphDotText;

	public Image hoverMarker;

	public ToolTip hoverTooltip;

	public RectTransform markersContainer;

	public Image markerTemplate;

	private RectTransform rectTransform;

	private float maxMass = 20000f;

	private float totalWidth = 5f;

	private bool mouseOver = false;

	public CommandModule commandModule = null;

	protected override void OnPrefabInit()
	{
	}

	public void Draw(CommandModule commandModule)
	{
		if (rectTransform == null)
		{
			rectTransform = graphBar.gameObject.GetComponent<RectTransform>();
		}
		this.commandModule = commandModule;
		totalWidth = rectTransform.rect.width;
		UpdateGraphDotPos(commandModule);
	}

	private void UpdateGraphDotPos(CommandModule rocket)
	{
		totalWidth = rectTransform.rect.width;
		float value = Mathf.Lerp(0f, totalWidth, rocket.rocketStats.GetTotalMass() / maxMass);
		value = Mathf.Clamp(value, 0f, totalWidth);
		graphDot.rectTransform.SetLocalPosition(new Vector3(value, 0f, 0f));
		graphDotText.text = "-" + Util.FormatWholeNumber(rocket.rocketStats.GetTotalThrust() - rocket.rocketStats.GetRocketMaxDistance()) + "km";
	}

	private void Update()
	{
		if (mouseOver)
		{
			if (rectTransform == null)
			{
				rectTransform = graphBar.gameObject.GetComponent<RectTransform>();
			}
			Vector3 position = rectTransform.GetPosition();
			Vector2 size = rectTransform.rect.size;
			float value = KInputManager.GetMousePos().x - position.x + size.x / 2f;
			value = Mathf.Clamp(value, 0f, totalWidth);
			hoverMarker.rectTransform.SetLocalPosition(new Vector3(value, 0f, 0f));
			float num = Mathf.Lerp(0f, maxMass, value / totalWidth);
			float totalThrust = commandModule.rocketStats.GetTotalThrust();
			float rocketMaxDistance = commandModule.rocketStats.GetRocketMaxDistance();
			hoverTooltip.SetSimpleTooltip(string.Concat(UI.STARMAP.ROCKETWEIGHT.MASS, GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram), "\n", UI.STARMAP.ROCKETWEIGHT.MASSPENALTY, Util.FormatWholeNumber(ROCKETRY.CalculateMassWithPenalty(num)), UI.UNITSUFFIXES.DISTANCE.KILOMETER, "\n\n", UI.STARMAP.ROCKETWEIGHT.CURRENTMASS, GameUtil.GetFormattedMass(commandModule.rocketStats.GetTotalMass(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram), "\n", UI.STARMAP.ROCKETWEIGHT.CURRENTMASSPENALTY, Util.FormatWholeNumber(totalThrust - rocketMaxDistance), UI.UNITSUFFIXES.DISTANCE.KILOMETER));
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		mouseOver = true;
		hoverMarker.SetAlpha(1f);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		mouseOver = false;
		hoverMarker.SetAlpha(0f);
	}
}
