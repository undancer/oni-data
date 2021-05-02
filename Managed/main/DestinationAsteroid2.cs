using System;
using ProcGen;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/DestinationAsteroid2")]
public class DestinationAsteroid2 : KMonoBehaviour
{
	[SerializeField]
	private Image asteroidImage;

	[SerializeField]
	private KButton button;

	[SerializeField]
	private KBatchedAnimController animController;

	private ColonyDestinationAsteroidBeltData asteroidData;

	public event Action<ColonyDestinationAsteroidBeltData> OnClicked;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		button.onClick += OnClickInternal;
	}

	public void SetAsteroid(ColonyDestinationAsteroidBeltData newAsteroidData)
	{
		if (asteroidData != null && !(newAsteroidData.beltPath != asteroidData.beltPath))
		{
			return;
		}
		asteroidData = newAsteroidData;
		ProcGen.World getStartWorld = newAsteroidData.GetStartWorld;
		string s = (getStartWorld.asteroidIcon.IsNullOrWhiteSpace() ? AsteroidGridEntity.DEFAULT_ASTEROID_ICON_ANIM : getStartWorld.asteroidIcon);
		Assets.TryGetAnim(s, out var anim);
		if (DlcManager.IsExpansion1Active() && anim != null)
		{
			asteroidImage.gameObject.SetActive(value: false);
			animController.AnimFiles = new KAnimFile[1]
			{
				anim
			};
			animController.initialMode = KAnim.PlayMode.Loop;
			animController.initialAnim = "idle_loop";
			animController.gameObject.SetActive(value: true);
			if (animController.HasAnimation(animController.initialAnim))
			{
				animController.Play(animController.initialAnim, KAnim.PlayMode.Loop);
			}
		}
		else
		{
			animController.gameObject.SetActive(value: false);
			asteroidImage.gameObject.SetActive(value: true);
			asteroidImage.sprite = asteroidData.sprite;
		}
	}

	private void OnClickInternal()
	{
		DebugUtil.LogArgs("Clicked asteroid belt", asteroidData.beltPath);
		this.OnClicked(asteroidData);
	}
}
