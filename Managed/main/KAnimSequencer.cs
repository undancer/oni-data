using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/KAnimSequencer")]
public class KAnimSequencer : KMonoBehaviour, ISaveLoadable
{
	[Serializable]
	[SerializationConfig(MemberSerialization.OptOut)]
	public class KAnimSequence
	{
		public string anim;

		public float speed = 1f;

		public KAnim.PlayMode mode = KAnim.PlayMode.Once;
	}

	[Serialize]
	public bool autoRun;

	[Serialize]
	public KAnimSequence[] sequence = new KAnimSequence[0];

	private int currentIndex;

	private KBatchedAnimController kbac;

	private MinionBrain mb;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		kbac = GetComponent<KBatchedAnimController>();
		mb = GetComponent<MinionBrain>();
		if (autoRun)
		{
			PlaySequence();
		}
	}

	public void Reset()
	{
		currentIndex = 0;
	}

	public void PlaySequence()
	{
		if (sequence != null && sequence.Length != 0)
		{
			if (mb != null)
			{
				mb.Suspend("AnimSequencer");
			}
			kbac.onAnimComplete += PlayNext;
			PlayNext(null);
		}
	}

	private void PlayNext(HashedString name)
	{
		if (sequence.Length > currentIndex)
		{
			kbac.Play(new HashedString(sequence[currentIndex].anim), sequence[currentIndex].mode, sequence[currentIndex].speed);
			currentIndex++;
			return;
		}
		kbac.onAnimComplete -= PlayNext;
		if (mb != null)
		{
			mb.Resume("AnimSequencer");
		}
	}
}
