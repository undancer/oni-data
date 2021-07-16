namespace EventSystem2Syntax
{
	internal class OldExample : KMonoBehaviour2
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Subscribe(0, OnObjectDestroyed);
			bool flag = false;
			Trigger(0, flag);
		}

		private void OnObjectDestroyed(object data)
		{
			Debug.Log((bool)data);
		}
	}
}
