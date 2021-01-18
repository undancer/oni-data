public class SaveActive : KScreen
{
	[MyCmpGet]
	private KBatchedAnimController controller;

	private Game.CansaveCB readyForSaveCallback = null;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Game.Instance.SetAutoSaveCallbacks(ActiveateSaveIndicator, SetActiveSaveIndicator, DeactivateSaveIndicator);
	}

	private void DoCallBack(HashedString name)
	{
		controller.onAnimComplete -= DoCallBack;
		readyForSaveCallback();
		readyForSaveCallback = null;
	}

	private void ActiveateSaveIndicator(Game.CansaveCB cb)
	{
		readyForSaveCallback = cb;
		controller.onAnimComplete += DoCallBack;
		controller.Play("working_pre");
	}

	private void SetActiveSaveIndicator()
	{
		controller.Play("working_loop");
	}

	private void DeactivateSaveIndicator()
	{
		controller.Play("working_pst");
	}

	public override void OnKeyDown(KButtonEvent e)
	{
	}
}
