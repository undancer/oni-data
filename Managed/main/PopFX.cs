using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/PopFX")]
public class PopFX : KMonoBehaviour
{
	private float Speed = 2f;

	private Sprite icon;

	private string text;

	private Transform targetTransform;

	private Vector3 offset;

	public Image IconDisplay;

	public LocText TextDisplay;

	public CanvasGroup canvasGroup;

	private Camera uiCamera;

	private float lifetime;

	private float lifeElapsed;

	private bool trackTarget;

	private Vector3 startPos;

	private bool isLive;

	private bool isActiveWorld;

	public void Recycle()
	{
		icon = null;
		text = "";
		targetTransform = null;
		lifeElapsed = 0f;
		trackTarget = false;
		startPos = Vector3.zero;
		IconDisplay.color = Color.white;
		TextDisplay.color = Color.white;
		PopFXManager.Instance.RecycleFX(this);
		canvasGroup.alpha = 0f;
		base.gameObject.SetActive(value: false);
		isLive = false;
		isActiveWorld = false;
		Game.Instance.Unsubscribe(1983128072, OnActiveWorldChanged);
	}

	public void Spawn(Sprite Icon, string Text, Transform TargetTransform, Vector3 Offset, float LifeTime = 1.5f, bool TrackTarget = false)
	{
		icon = Icon;
		text = Text;
		targetTransform = TargetTransform;
		trackTarget = TrackTarget;
		lifetime = LifeTime;
		offset = Offset;
		if (targetTransform != null)
		{
			startPos = targetTransform.GetPosition();
			Grid.PosToXY(startPos, out var _, out var y);
			if (y % 2 != 0)
			{
				startPos.x += 0.5f;
			}
		}
		TextDisplay.text = text;
		IconDisplay.sprite = icon;
		canvasGroup.alpha = 1f;
		isLive = true;
		Game.Instance.Subscribe(1983128072, OnActiveWorldChanged);
		SetWorldActive(ClusterManager.Instance.activeWorldId);
		Update();
	}

	private void OnActiveWorldChanged(object data)
	{
		Tuple<int, int> tuple = (Tuple<int, int>)data;
		if (isLive)
		{
			SetWorldActive(tuple.first);
		}
	}

	private void SetWorldActive(int worldId)
	{
		int num = Grid.PosToCell((trackTarget && targetTransform != null) ? targetTransform.position : (startPos + offset));
		isActiveWorld = !Grid.IsValidCell(num) || Grid.WorldIdx[num] == worldId;
	}

	private void Update()
	{
		if (isLive && PopFXManager.Instance.Ready())
		{
			lifeElapsed += Time.unscaledDeltaTime;
			if (lifeElapsed >= lifetime)
			{
				Recycle();
			}
			if (trackTarget && targetTransform != null)
			{
				Vector3 vector = PopFXManager.Instance.WorldToScreen(targetTransform.GetPosition() + offset + Vector3.up * lifeElapsed * (Speed * lifeElapsed));
				vector.z = 0f;
				base.gameObject.rectTransform().anchoredPosition = vector;
			}
			else
			{
				Vector3 vector2 = PopFXManager.Instance.WorldToScreen(startPos + offset + Vector3.up * lifeElapsed * (Speed * (lifeElapsed / 2f)));
				vector2.z = 0f;
				base.gameObject.rectTransform().anchoredPosition = vector2;
			}
			canvasGroup.alpha = (isActiveWorld ? (1.5f * ((lifetime - lifeElapsed) / lifetime)) : 0f);
		}
	}
}
