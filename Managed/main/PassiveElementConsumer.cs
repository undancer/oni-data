public class PassiveElementConsumer : ElementConsumer, IGameObjectEffectDescriptor
{
	protected override bool IsActive()
	{
		return true;
	}
}
