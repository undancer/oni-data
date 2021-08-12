namespace EventSystem2Syntax
{
	internal class NewExample : KMonoBehaviour2
	{
		private struct ObjectDestroyedEvent : IEventData
		{
			public bool parameter;
		}

		protected override void OnPrefabInit()
		{
			Subscribe<NewExample, ObjectDestroyedEvent>(OnObjectDestroyed);
			Trigger(new ObjectDestroyedEvent
			{
				parameter = false
			});
		}

		private static void OnObjectDestroyed(NewExample example, ObjectDestroyedEvent evt)
		{
		}
	}
}
