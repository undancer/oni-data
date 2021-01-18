using System;
using Database;
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
		if (DlcManager.IsExpansion1Active())
		{
			asteroidImage.gameObject.SetActive(value: false);
			ProcGen.World getStartWorld = newAsteroidData.GetStartWorld;
			AsteroidType typeOrDefault = Db.Get().AsteroidTypes.GetTypeOrDefault(getStartWorld.asteroidType);
			KAnimFile anim = Assets.GetAnim(typeOrDefault.animName);
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
