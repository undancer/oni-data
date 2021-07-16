using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/Slideshow")]
public class Slideshow : KMonoBehaviour
{
	public delegate void onBeforeAndEndPlayDelegate();

	public RawImage imageTarget;

	private string[] files;

	private Sprite currentSlideImage;

	private Sprite[] sprites;

	public float timePerSlide = 1f;

	public float timeFactorForLastSlide = 3f;

	private int currentSlide;

	private float timeUntilNextSlide;

	private bool paused;

	public bool playInThumbnail;

	public SlideshowUpdateType updateType;

	[SerializeField]
	private bool isExpandable;

	[SerializeField]
	private KButton button;

	[SerializeField]
	private bool transparentIfEmpty = true;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton prevButton;

	[SerializeField]
	private KButton nextButton;

	[SerializeField]
	private KButton pauseButton;

	[SerializeField]
	private Image pauseIcon;

	[SerializeField]
	private Image unpauseIcon;

	public onBeforeAndEndPlayDelegate onBeforePlay;

	public onBeforeAndEndPlayDelegate onEndingPlay;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		timeUntilNextSlide = timePerSlide;
		if (transparentIfEmpty && sprites != null && sprites.Length == 0)
		{
			imageTarget.color = Color.clear;
		}
		if (isExpandable)
		{
			button = GetComponent<KButton>();
			button.onClick += delegate
			{
				if (onBeforePlay != null)
				{
					onBeforePlay();
				}
				switch (updateType)
				{
				case SlideshowUpdateType.preloadedSprites:
					VideoScreen.Instance.PlaySlideShow(sprites);
					break;
				case SlideshowUpdateType.loadOnDemand:
					VideoScreen.Instance.PlaySlideShow(files);
					break;
				}
			};
		}
		if (nextButton != null)
		{
			nextButton.onClick += delegate
			{
				nextSlide();
			};
		}
		if (prevButton != null)
		{
			prevButton.onClick += delegate
			{
				prevSlide();
			};
		}
		if (pauseButton != null)
		{
			pauseButton.onClick += delegate
			{
				SetPaused(!paused);
			};
		}
		if (!(closeButton != null))
		{
			return;
		}
		closeButton.onClick += delegate
		{
			VideoScreen.Instance.Stop();
			if (onEndingPlay != null)
			{
				onEndingPlay();
			}
		};
	}

	public void SetPaused(bool state)
	{
		paused = state;
		if (pauseIcon != null)
		{
			pauseIcon.gameObject.SetActive(!paused);
		}
		if (unpauseIcon != null)
		{
			unpauseIcon.gameObject.SetActive(paused);
		}
		if (prevButton != null)
		{
			prevButton.gameObject.SetActive(paused);
		}
		if (nextButton != null)
		{
			nextButton.gameObject.SetActive(paused);
		}
	}

	private void resetSlide(bool enable)
	{
		timeUntilNextSlide = timePerSlide;
		currentSlide = 0;
		if (enable)
		{
			imageTarget.color = Color.white;
		}
		else if (transparentIfEmpty)
		{
			imageTarget.color = Color.clear;
		}
	}

	private Sprite loadSlide(string file)
	{
		_ = Time.realtimeSinceStartup;
		Texture2D texture2D = new Texture2D(512, 768);
		texture2D.filterMode = FilterMode.Point;
		texture2D.LoadImage(File.ReadAllBytes(file));
		return Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2(texture2D.width, texture2D.height)), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect);
	}

	public void SetFiles(string[] files, int loadFrame = -1)
	{
		if (files != null)
		{
			this.files = files;
			bool flag = files.Length != 0 && files[0] != null;
			resetSlide(flag);
			if (flag)
			{
				int num = ((loadFrame != -1) ? loadFrame : (files.Length - 1));
				string file = files[num];
				Sprite slide = loadSlide(file);
				setSlide(slide);
				currentSlideImage = slide;
			}
		}
	}

	public void updateSize(Sprite sprite)
	{
		Vector2 fittedSize = GetFittedSize(sprite, 960f, 960f);
		GetComponent<RectTransform>().sizeDelta = fittedSize;
	}

	public void SetSprites(Sprite[] sprites)
	{
		if (sprites != null)
		{
			this.sprites = sprites;
			resetSlide(sprites.Length != 0 && sprites[0] != null);
			if (sprites.Length != 0 && sprites[0] != null)
			{
				setSlide(sprites[0]);
			}
		}
	}

	public Vector2 GetFittedSize(Sprite sprite, float maxWidth, float maxHeight)
	{
		if (sprite == null || sprite.texture == null)
		{
			return Vector2.zero;
		}
		int width = sprite.texture.width;
		int height = sprite.texture.height;
		float num = maxWidth / (float)width;
		float num2 = maxHeight / (float)height;
		if (num < num2)
		{
			return new Vector2((float)width * num, (float)height * num);
		}
		return new Vector2((float)width * num2, (float)height * num2);
	}

	public void setSlide(Sprite slide)
	{
		if (!(slide == null))
		{
			imageTarget.texture = slide.texture;
			updateSize(slide);
		}
	}

	public void nextSlide()
	{
		setSlideIndex(currentSlide + 1);
	}

	public void prevSlide()
	{
		setSlideIndex(currentSlide - 1);
	}

	private void setSlideIndex(int slideIndex)
	{
		timeUntilNextSlide = timePerSlide;
		switch (updateType)
		{
		case SlideshowUpdateType.preloadedSprites:
			if (slideIndex < 0)
			{
				slideIndex = sprites.Length + slideIndex;
			}
			currentSlide = slideIndex % sprites.Length;
			if (currentSlide == sprites.Length - 1)
			{
				timeUntilNextSlide *= timeFactorForLastSlide;
			}
			if (playInThumbnail)
			{
				setSlide(sprites[currentSlide]);
			}
			break;
		case SlideshowUpdateType.loadOnDemand:
			if (slideIndex < 0)
			{
				slideIndex = files.Length + slideIndex;
			}
			currentSlide = slideIndex % files.Length;
			if (currentSlide == files.Length - 1)
			{
				timeUntilNextSlide *= timeFactorForLastSlide;
			}
			if (playInThumbnail)
			{
				if (currentSlideImage != null)
				{
					UnityEngine.Object.Destroy(currentSlideImage.texture);
					UnityEngine.Object.Destroy(currentSlideImage);
					GC.Collect();
				}
				currentSlideImage = loadSlide(files[currentSlide]);
				setSlide(currentSlideImage);
			}
			break;
		}
	}

	private void Update()
	{
		if ((updateType != 0 || (sprites != null && sprites.Length != 0)) && (updateType != SlideshowUpdateType.loadOnDemand || (files != null && files.Length != 0)) && !paused)
		{
			timeUntilNextSlide -= Time.unscaledDeltaTime;
			if (timeUntilNextSlide <= 0f)
			{
				nextSlide();
			}
		}
	}
}
