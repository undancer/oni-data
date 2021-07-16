using UnityEngine;

public interface ILaunchableRocket
{
	LaunchableRocketRegisterType registerType
	{
		get;
	}

	GameObject LaunchableGameObject
	{
		get;
	}

	float rocketSpeed
	{
		get;
	}

	bool isLanding
	{
		get;
	}
}
