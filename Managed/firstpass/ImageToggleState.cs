using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/Plugins/ImageToggleState")]
public class ImageToggleState : KMonoBehaviour
{
	public enum State
	{
		Disabled,
		Inactive,
		Active,
		DisabledActive
	}

	public Image TargetImage;

	public Sprite ActiveSprite;

	public Sprite InactiveSprite;

	public Sprite DisabledSprite;

	public Sprite DisabledActiveSprite;

	public bool useSprites;

	public Color ActiveColour = Color.white;

	public Color InactiveColour = Color.white;

	public Color DisabledColour = Color.white;

	public Color DisabledActiveColour = Color.white;

	public Color HoverColour = Color.white;

	public Color DisabledHoverColor = Color.white;

	public ColorStyleSetting colorStyleSetting;

	private bool isActive;

	private State currentState = State.Inactive;

	public bool useStartingState;

	public State startingState = State.Inactive;

	public bool IsDisabled
	{
		get
		{
			if (currentState != 0)
			{
				return currentState == State.DisabledActive;
			}
			return true;
		}
	}

	public new void Awake()
	{
		base.Awake();
		RefreshColorStyle();
		if (useStartingState)
		{
			SetState(startingState);
		}
	}

	[ContextMenu("Refresh Colour Style")]
	public void RefreshColorStyle()
	{
		if (colorStyleSetting != null)
		{
			ActiveColour = colorStyleSetting.activeColor;
			InactiveColour = colorStyleSetting.inactiveColor;
			DisabledColour = colorStyleSetting.disabledColor;
			DisabledActiveColour = colorStyleSetting.disabledActiveColor;
			HoverColour = colorStyleSetting.hoverColor;
			DisabledHoverColor = colorStyleSetting.disabledhoverColor;
		}
	}

	public void SetSprites(Sprite disabled, Sprite inactive, Sprite active, Sprite disabledActive)
	{
		if (disabled != null)
		{
			DisabledSprite = disabled;
		}
		if (inactive != null)
		{
			InactiveSprite = inactive;
		}
		if (active != null)
		{
			ActiveSprite = active;
		}
		if (disabledActive != null)
		{
			DisabledActiveSprite = disabledActive;
		}
		useSprites = true;
	}

	public bool GetIsActive()
	{
		return isActive;
	}

	private void SetTargetImageColor(Color color)
	{
		TargetImage.color = color;
	}

	public void SetState(State newState)
	{
		if (currentState != newState)
		{
			switch (newState)
			{
			case State.Inactive:
				SetInactive();
				break;
			case State.Active:
				SetActive();
				break;
			case State.Disabled:
				SetDisabled();
				break;
			case State.DisabledActive:
				SetDisabledActive();
				break;
			}
		}
	}

	public void SetActiveState(bool active)
	{
		if (active)
		{
			SetActive();
		}
		else
		{
			SetInactive();
		}
	}

	public void SetActive()
	{
		if (currentState == State.Active)
		{
			return;
		}
		isActive = true;
		currentState = State.Active;
		if (TargetImage == null)
		{
			return;
		}
		SetTargetImageColor(ActiveColour);
		if (useSprites)
		{
			if (ActiveSprite != null && TargetImage.sprite != ActiveSprite)
			{
				TargetImage.sprite = ActiveSprite;
			}
			else if (ActiveSprite == null)
			{
				TargetImage.sprite = null;
			}
		}
	}

	public void SetColorStyle(ColorStyleSetting style)
	{
		colorStyleSetting = style;
		RefreshColorStyle();
		ResetColor();
	}

	public void ResetColor()
	{
		switch (currentState)
		{
		case State.Active:
			SetTargetImageColor(ActiveColour);
			break;
		case State.Inactive:
			SetTargetImageColor(InactiveColour);
			break;
		case State.Disabled:
			SetTargetImageColor(DisabledColour);
			break;
		case State.DisabledActive:
			SetTargetImageColor(DisabledActiveColour);
			break;
		}
	}

	public void OnHoverIn()
	{
		SetTargetImageColor((currentState == State.Disabled || currentState == State.DisabledActive) ? DisabledHoverColor : HoverColour);
	}

	public void OnHoverOut()
	{
		ResetColor();
	}

	public void SetInactive()
	{
		if (currentState == State.Inactive)
		{
			return;
		}
		isActive = false;
		currentState = State.Inactive;
		SetTargetImageColor(InactiveColour);
		if (!(TargetImage == null) && useSprites)
		{
			if (InactiveSprite != null && TargetImage.sprite != InactiveSprite)
			{
				TargetImage.sprite = InactiveSprite;
			}
			else if (InactiveSprite == null)
			{
				TargetImage.sprite = null;
			}
		}
	}

	public void SetDisabled()
	{
		if (currentState == State.Disabled)
		{
			SetTargetImageColor(DisabledColour);
			return;
		}
		isActive = false;
		currentState = State.Disabled;
		SetTargetImageColor(DisabledColour);
		if (!(TargetImage == null) && useSprites)
		{
			if (DisabledSprite != null && TargetImage.sprite != DisabledSprite)
			{
				TargetImage.sprite = DisabledSprite;
			}
			else if (DisabledSprite == null)
			{
				TargetImage.sprite = null;
			}
		}
	}

	public void SetDisabledActive()
	{
		isActive = false;
		currentState = State.DisabledActive;
		if (TargetImage == null)
		{
			return;
		}
		SetTargetImageColor(DisabledActiveColour);
		if (useSprites)
		{
			if (DisabledActiveSprite != null && TargetImage.sprite != DisabledActiveSprite)
			{
				TargetImage.sprite = DisabledActiveSprite;
			}
			else if (DisabledActiveSprite == null)
			{
				TargetImage.sprite = null;
			}
		}
	}
}
