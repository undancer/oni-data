using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("KMonoBehaviour/Plugins/KScreen")]
public class KScreen : KMonoBehaviour, IInputHandler, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public delegate void PointerEnterActions(PointerEventData eventData);

	public delegate void PointerExitActions(PointerEventData eventData);

	[SerializeField]
	public bool activateOnSpawn;

	private Canvas _canvas;

	private RectTransform _rectTransform;

	private bool isActive;

	protected bool mouseOver;

	public WidgetTransition.TransitionType transitionType;

	public bool fadeIn;

	public string displayName;

	public PointerEnterActions pointerEnterActions;

	public PointerExitActions pointerExitActions;

	private bool hasFocus;

	public string handlerName => base.gameObject.name;

	public KInputHandler inputHandler
	{
		get;
		set;
	}

	public virtual bool HasFocus => hasFocus;

	public Canvas canvas => _canvas;

	public string screenName
	{
		get;
		private set;
	}

	public bool GetMouseOver => mouseOver;

	public bool ConsumeMouseScroll
	{
		get;
		set;
	}

	public virtual float GetSortKey()
	{
		return 0f;
	}

	public virtual void SetHasFocus(bool has_focus)
	{
		hasFocus = has_focus;
	}

	public KScreen()
	{
		screenName = GetType().ToString();
		if (displayName == null || displayName == "")
		{
			displayName = screenName;
		}
	}

	protected override void OnPrefabInit()
	{
		if (fadeIn)
		{
			InitWidgetTransition();
		}
	}

	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		mouseOver = true;
		if (pointerEnterActions != null)
		{
			pointerEnterActions(eventData);
		}
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		mouseOver = false;
		if (pointerExitActions != null)
		{
			pointerExitActions(eventData);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		_canvas = GetComponentInParent<Canvas>();
		if (_canvas != null)
		{
			_rectTransform = _canvas.GetComponentInParent<RectTransform>();
		}
		if (activateOnSpawn && KScreenManager.Instance != null)
		{
			Activate();
		}
		if (ConsumeMouseScroll && !IsActive())
		{
			Debug.LogWarning("ConsumeMouseScroll is true on" + base.gameObject.name + " , but screen has not been activated. Mouse scrolling might not work properly on this screen.");
		}
	}

	public virtual void OnKeyDown(KButtonEvent e)
	{
		if (mouseOver && ConsumeMouseScroll && !e.Consumed && !e.TryConsume(Action.ZoomIn))
		{
			e.TryConsume(Action.ZoomOut);
		}
	}

	public virtual void OnKeyUp(KButtonEvent e)
	{
	}

	public virtual bool IsModal()
	{
		return false;
	}

	public virtual void ScreenUpdate(bool topLevel)
	{
	}

	public bool IsActive()
	{
		return isActive;
	}

	public void Activate()
	{
		base.gameObject.SetActive(value: true);
		KScreenManager.Instance.PushScreen(this);
		OnActivate();
		isActive = true;
	}

	protected virtual void OnActivate()
	{
	}

	public virtual void Deactivate()
	{
		if (Application.isPlaying)
		{
			OnDeactivate();
			isActive = false;
			KScreenManager.Instance.PopScreen(this);
			if (this != null && base.gameObject != null)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	protected override void OnCleanUp()
	{
		if (isActive)
		{
			Deactivate();
		}
	}

	protected virtual void OnDeactivate()
	{
	}

	public string Name()
	{
		return screenName;
	}

	public Vector3 WorldToScreen(Vector3 pos)
	{
		if (_rectTransform == null)
		{
			Debug.LogWarning("Hey you are calling this function too early!");
			return Vector3.zero;
		}
		Camera main = Camera.main;
		Vector3 vector = main.WorldToViewportPoint(pos);
		vector.y = vector.y * main.rect.height + main.rect.y;
		return new Vector2((vector.x - 0.5f) * _rectTransform.sizeDelta.x, (vector.y - 0.5f) * _rectTransform.sizeDelta.y);
	}

	protected virtual void OnShow(bool show)
	{
		if (show && fadeIn)
		{
			base.gameObject.FindOrAddUnityComponent<WidgetTransition>().StartTransition();
		}
	}

	public void Show(bool show = true)
	{
		mouseOver = false;
		base.gameObject.SetActive(show);
		OnShow(show);
	}

	public void SetShouldFadeIn(bool bShouldFade)
	{
		fadeIn = bShouldFade;
		InitWidgetTransition();
	}

	private void InitWidgetTransition()
	{
		base.gameObject.FindOrAddUnityComponent<WidgetTransition>().SetTransitionType(transitionType);
	}
}
