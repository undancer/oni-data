using System;

[SkipSaveFileSerialization]
public class CancellableDig : Cancellable
{
	protected override void OnCancel(object data)
	{
		if (data != null && (bool)data)
		{
			OnAnimationDone("ScaleDown");
			return;
		}
		EasingAnimations componentInChildren = GetComponentInChildren<EasingAnimations>();
		int num = Grid.PosToCell(this);
		if (componentInChildren.IsPlaying && Grid.Element[num].hardness == byte.MaxValue)
		{
			componentInChildren.OnAnimationDone = (Action<string>)Delegate.Combine(componentInChildren.OnAnimationDone, new Action<string>(DoCancelAnim));
			return;
		}
		componentInChildren.OnAnimationDone = (Action<string>)Delegate.Combine(componentInChildren.OnAnimationDone, new Action<string>(OnAnimationDone));
		componentInChildren.PlayAnimation("ScaleDown", 0.1f);
	}

	private void DoCancelAnim(string animName)
	{
		EasingAnimations componentInChildren = GetComponentInChildren<EasingAnimations>();
		componentInChildren.OnAnimationDone = (Action<string>)Delegate.Remove(componentInChildren.OnAnimationDone, new Action<string>(DoCancelAnim));
		componentInChildren.OnAnimationDone = (Action<string>)Delegate.Combine(componentInChildren.OnAnimationDone, new Action<string>(OnAnimationDone));
		componentInChildren.PlayAnimation("ScaleDown", 0.1f);
	}

	private void OnAnimationDone(string animationName)
	{
		if (!(animationName != "ScaleDown"))
		{
			EasingAnimations componentInChildren = GetComponentInChildren<EasingAnimations>();
			componentInChildren.OnAnimationDone = (Action<string>)Delegate.Remove(componentInChildren.OnAnimationDone, new Action<string>(OnAnimationDone));
			this.DeleteObject();
		}
	}
}
