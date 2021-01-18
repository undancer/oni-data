using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[AddComponentMenu("KMonoBehaviour/scripts/VideoWidget")]
public class VideoWidget : KMonoBehaviour
{
	[SerializeField]
	private VideoClip clip;

	[SerializeField]
	private VideoPlayer thumbnailPlayer;

	[SerializeField]
	private KButton button;

	[SerializeField]
	private string overlayName;

	[SerializeField]
	private List<string> texts;

	private RenderTexture renderTexture;

	private RawImage rawImage;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		button.onClick += Clicked;
		rawImage = thumbnailPlayer.GetComponent<RawImage>();
	}

	private void Clicked()
	{
		VideoScreen.Instance.PlayVideo(clip);
		if (!string.IsNullOrEmpty(overlayName))
		{
			VideoScreen.Instance.SetOverlayText(overlayName, texts);
		}
	}

	public void SetClip(VideoClip clip, string overlayName = null, List<string> texts = null)
	{
		if (clip == null)
		{
			Debug.LogWarning("Tried to assign null video clip to VideoWidget");
			return;
		}
		this.clip = clip;
		this.overlayName = overlayName;
		this.texts = texts;
		renderTexture = new RenderTexture(Convert.ToInt32(clip.width), Convert.ToInt32(clip.height), 16);
		thumbnailPlayer.targetTexture = renderTexture;
		rawImage.texture = renderTexture;
		StartCoroutine(ConfigureThumbnail());
	}

	private IEnumerator ConfigureThumbnail()
	{
		thumbnailPlayer.clip = clip;
		thumbnailPlayer.time = 0.0;
		thumbnailPlayer.Play();
		yield return null;
	}

	private void Update()
	{
		if (thumbnailPlayer.isPlaying && thumbnailPlayer.time > 2.0)
		{
			thumbnailPlayer.Pause();
		}
	}
}
