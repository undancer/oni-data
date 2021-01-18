using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DestroyAfter")]
public class DestroyAfter : KMonoBehaviour
{
	private ParticleSystem[] particleSystems;

	protected override void OnSpawn()
	{
		particleSystems = base.gameObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
	}

	private bool IsAlive()
	{
		for (int i = 0; i < particleSystems.Length; i++)
		{
			if (particleSystems[i].IsAlive(withChildren: false))
			{
				return true;
			}
		}
		return false;
	}

	private void Update()
	{
		if (particleSystems != null && !IsAlive())
		{
			this.DeleteObject();
		}
	}
}
